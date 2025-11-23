using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Models.ViewModels;
using GestaoOS.Services; // Precisa deste using para RolesGlobais
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace GestaoOS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly ApplicationDbContext _context;



        public HomeController(ILogger<HomeController> logger, UserManager<Usuario> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return Challenge();
            }

            if (await _userManager.IsInRoleAsync(usuario, RolesGlobais.Gestor))
            {

                return RedirectToAction("DashboardGestor", "Home");

            }

            if (await _userManager.IsInRoleAsync(usuario, RolesGlobais.Professor))
            {
                return RedirectToAction("AbrirChamado", "Professor");
            }

            if (await _userManager.IsInRoleAsync(usuario, RolesGlobais.Manutencao))
            {
                return RedirectToAction("MinhasOrdens", "Manutencao");
            }

            return Forbid();
        }

        [Authorize(Roles = RolesGlobais.Gestor)]
        public async Task<IActionResult> DashboardGestor()
        {
            var kpiData = await _context.OrdensDeServico
                .GroupBy(os => 1)
                .Select(g => new
                {
                    AbertasNaoAtribuidas = g.Count(os => os.ResponsavelId == null && os.Status == "Aberta"),
                    EmExecucao = g.Count(os => os.Status == "Em Andamento"),
                    EmEspera = g.Count(os => os.Status == "Em Espera")
                })
                .FirstOrDefaultAsync();

            var hoje = DateTime.Now;
            var dataLimite = hoje.AddMonths(-5);
            var dadosMensais = await _context.OrdensDeServico
                .Where(os => os.DataCriacao >= new DateTime(dataLimite.Year, dataLimite.Month, 1))
                .GroupBy(os => new { Ano = os.DataCriacao.Year, Mes = os.DataCriacao.Month })
                .Select(g => new { g.Key.Ano, g.Key.Mes, Contagem = g.Count() })
                .OrderBy(r => r.Ano).ThenBy(r => r.Mes)
                .ToListAsync();

            var labelsGrafico = new List<string>();
            var dadosGrafico = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var dataIteracao = hoje.AddMonths(-i);
                labelsGrafico.Add(dataIteracao.ToString("MMM/yy"));
                var dadoDoMes = dadosMensais.FirstOrDefault(d => d.Ano == dataIteracao.Year && d.Mes == dataIteracao.Month);
                dadosGrafico.Add(dadoDoMes?.Contagem ?? 0);
            }

            // --- Montagem do ViewModel ---
            var viewModel = new DashboardGestorViewModel
            {
                OrdensAbertasNaoAtribuidas = kpiData?.AbertasNaoAtribuidas ?? 0,
                OrdensEmExecucao = kpiData?.EmExecucao ?? 0,
                OrdensEmEspera = kpiData?.EmEspera ?? 0,
                GraficoMesesLabels = labelsGrafico,
                GraficoContagemDados = dadosGrafico,
                OrdensNaoAtribuidasRecentes = await _context.OrdensDeServico
                    .Include(os => os.Ativo)
                    .Include(os => os.Solicitante)
                    .Where(os => os.ResponsavelId == null && os.Status == "Aberta")
                    .OrderByDescending(os => os.DataCriacao)
                    .Take(5)
                    .ToListAsync()
            };

            var tecnicos = await _userManager.GetUsersInRoleAsync(RolesGlobais.Manutencao);
            ViewData["TecnicosManutencao"] = new SelectList(tecnicos.OrderBy(u => u.Nome), "Id", "Nome");

            return View(viewModel);
        }

        // [MODIFICADO]
        [Authorize(Roles = RolesGlobais.Gestor)]
        public async Task<IActionResult> DashboardGraficos()
        {
            // --- A LÓGICA DE KPI DE TEMPO MÉDIO FOI MOVIDA para 'DashboardGraficosData()'
            //     pois agora ela é dinâmica e depende do filtro de data.

            // --- Lógica de Ranking (estática, 'All-Time') ---
            // Esta lógica permanece aqui, pois não depende do filtro de data.
            var historicoCompleto = await _context.OrdensDeServicoHistorico.ToListAsync();

            // Top 5 Técnicos (usando ResponsavelNome)
            var topTecnicos = historicoCompleto
                .Where(h => !string.IsNullOrEmpty(h.ResponsavelNome))
                .GroupBy(h => h.ResponsavelNome)
                .Select(g => new RankingItem { Nome = g.Key, Contagem = g.Count() })
                .OrderByDescending(r => r.Contagem)
                .Take(5)
                .ToList();

            // Top 5 Ativos (usando AtivoNome)
            var topAtivos = historicoCompleto
                .Where(h => !string.IsNullOrEmpty(h.AtivoNome))
                .GroupBy(h => h.AtivoNome)
                .Select(g => new RankingItem { Nome = g.Key, Contagem = g.Count() })
                .OrderByDescending(r => r.Contagem)
                .Take(5)
                .ToList();
            // --- FIM DA LÓGICA DE RANKING ---

            var viewModel = new DashboardGraficosViewModel
            {
                // Define valores placeholder que serão substituídos pelo JavaScript
                TempoMedioAtendimento = "...",
                TempoMedioConclusao = "...",

                // KPIs estáticos (não dependem do filtro)
                AtivasAbertas = await _context.OrdensDeServico.CountAsync(os => os.Status == "Aberta"),
                AtivasEmAndamento = await _context.OrdensDeServico.CountAsync(os => os.Status == "Em Andamento"),
                AtivasEmEspera = await _context.OrdensDeServico.CountAsync(os => os.Status == "Em Espera"),

                // Rankings estáticos (não dependem do filtro)
                TopTecnicos = topTecnicos,
                TopAtivos = topAtivos
            };

            return View(viewModel);
        }


        // [MODIFICADO]
        // ---------- API DE DADOS PARA OS GRÁFICOS (CORRIGIDA) ----------
        [Authorize(Roles = RolesGlobais.Gestor)]
        [HttpGet]
        public async Task<IActionResult> DashboardGraficosData(DateTime? startDate, DateTime? endDate)
        {
            var inicio = startDate?.Date ?? DateTime.Today.AddMonths(-6).Date;
            var fim = (endDate?.Date ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);

            // --- 1. CÁLCULO DOS KPIs DINÂMICOS (FINALIDADE 1) ---
            // A lógica que estava em 'DashboardGraficos' foi movida para cá
            // e agora usa o período filtrado (inicio/fim).
            
            string kpiTempoAtendimento = "N/D";
            string kpiTempoConclusao = "N/D";

            // Busca os dados para os KPIs, FILTRADOS pelo período
            // Usamos 'DataConclusao' para filtrar, pois o KPI só existe para OS concluídas
            var historicoFiltradoKPI = await _context.OrdensDeServicoHistorico
                .Where(h => h.DataInicioExecucao.HasValue && h.DataConclusao.HasValue &&
                            h.DataConclusao >= inicio && h.DataConclusao <= fim) 
                .ToListAsync();

            if (historicoFiltradoKPI.Any())
            {
                try
                {
                    double mediaHorasAtendimento = historicoFiltradoKPI
                        .Average(h => (h.DataInicioExecucao.Value - h.DataCriacao).TotalHours);
                    kpiTempoAtendimento = $"{mediaHorasAtendimento:F1} horas";

                    double mediaDiasConclusao = historicoFiltradoKPI
                        .Average(h => (h.DataConclusao.Value - h.DataCriacao).TotalDays);
                    kpiTempoConclusao = $"{mediaDiasConclusao:F1} dias";
                }
                catch (Exception)
                {
                    kpiTempoAtendimento = "Erro Calc.";
                    kpiTempoConclusao = "Erro Calc.";
                }
            }

            // --- 2. CÁLCULO DOS GRÁFICOS ---

            // Consulta base na tabela de HISTÓRICO (Filtrada por DataCRIACAO)
            var dadosHistorico = await _context.OrdensDeServicoHistorico
                .Where(o => o.DataCriacao >= inicio && o.DataCriacao <= fim)
                .Select(o => new
                {
                    o.DataCriacao,
                    o.AtivoSalaNome,
                    o.AtivoTipoNome
                })
                .ToListAsync();

            // --- 2.1 Processamento para Gráfico Mensal (COM MESES ZERADOS - FINALIDADE 2) ---
            
            // a) Agrupa os dados REAIS em um dicionário para consulta rápida
            var dadosReaisMensais = dadosHistorico
                .GroupBy(d => new { d.DataCriacao.Year, d.DataCriacao.Month })
                .ToDictionary(
                    g => new { g.Key.Year, g.Key.Month }, // Chave
                    g => g.Count()                       // Valor
                );

            // b) Cria a lista "modelo" com TODOS os meses no período
            var labelsMensais = new List<string>();
            var dadosMensais = new List<int>();
            
            // Itera mês a mês, do início ao fim do filtro
            var dataCorrente = new DateTime(inicio.Year, inicio.Month, 1);
            while (dataCorrente <= fim)
            {
                // Adiciona o Label (Ex: "Nov/25")
                labelsMensais.Add(dataCorrente.ToString("MMM/yy", new CultureInfo("pt-BR")));

                // c) Procura o dado real no dicionário

                var chave = new { Year = dataCorrente.Year, Month = dataCorrente.Month };

                if (dadosReaisMensais.TryGetValue(chave, out int contagem))
                {
                    dadosMensais.Add(contagem); // Encontrou
                }
                else
                {
                    dadosMensais.Add(0); // Não encontrou, preenche com 0
                }

                // Vai para o próximo mês
                dataCorrente = dataCorrente.AddMonths(1);
            }

            // 2.2 Processamento para Gráfico Semanal (lógica original mantida)
            // (Nota: esta lógica ainda pode ter "semanas faltando" se não houver dados.
            //  Poderia ser aplicada a mesma técnica do gráfico mensal se necessário.)
            static DateTime StartOfWeek(DateTime dt) { return dt.Date.AddDays(-1 * ((7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7)); }
            var semanal = dadosHistorico
                .GroupBy(d => StartOfWeek(d.DataCriacao))
                .OrderBy(g => g.Key)
                .Select(g => new { Label = $"{g.Key:dd/MM} - {g.Key.AddDays(6):dd/MM}", Count = g.Count() })
                .ToList();

            // 2.3 Processamento para Gráfico de Salas (lógica original mantida)
            var topSalas = dadosHistorico
                .GroupBy(d => d.AtivoSalaNome)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { Label = g.Key ?? "Sem Sala", Count = g.Count() })
                .ToList();

            // 2.4 Processamento para Gráfico de Tipos de Ativo (lógica original mantida)
            var topTipos = dadosHistorico
                .GroupBy(d => d.AtivoTipoNome)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { Label = g.Key ?? "Sem Tipo", Count = g.Count() })
                .ToList();

            // --- 3. Monta o objeto de retorno ---
            // Retorna um JSON anônimo para corresponder ao JS que espera
            // (kpiTempoAtendimento, kpiTempoConclusao, mensalLabels, etc.)
            return Json(new
            {
                // KPIs Dinâmicos (FINALIDADE 1)
                kpiTempoAtendimento,
                kpiTempoConclusao,

                // Gráfico Mensal (FINALIDADE 2)
                mensalLabels = labelsMensais,
                mensalData = dadosMensais,

                // Outros Gráficos (lógica original)
                semanalLabels = semanal.Select(s => s.Label).ToList(),
                semanalData = semanal.Select(s => s.Count).ToList(),
                topSalasLabels = topSalas.Select(t => t.Label).ToList(),
                topSalasData = topSalas.Select(t => t.Count).ToList(),
                topTiposLabels = topTipos.Select(t => t.Label).ToList(),
                topTiposData = topTipos.Select(t => t.Count).ToList()
            });
        }
        public IActionResult Privacy()
        {
            return View();
        }


    }
}