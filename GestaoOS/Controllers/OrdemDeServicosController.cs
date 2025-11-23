using GestaoOS.Controllers;
using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace GestaoOS.Controllers
{
    [Authorize] 
    public class OrdemDeServicosController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<Usuario> _userManager;

        public OrdemDeServicosController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: OrdemDeServicos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrdensDeServico.Include(o => o.Ativo).Include(o => o.Responsavel).Include(o => o.Solicitante);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrdemDeServicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // --- INÍCIO DA CORREÇÃO ---
            var ordemDeServico = await _context.OrdensDeServico
                .Include(o => o.Ativo)
                    .ThenInclude(a => a.Sala)
                .Include(o => o.Responsavel)
                .Include(o => o.Solicitante)
                .FirstOrDefaultAsync(m => m.Id == id);
            // --- FIM DA CORREÇÃO ---

            if (ordemDeServico == null)
            {
                return NotFound();
            }

            return View(ordemDeServico);
        }
        [HttpPost]
        [Authorize(Roles = RolesGlobais.Gestor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarEspera(int id)
        {
            var os = await _context.OrdensDeServico.FindAsync(id);
            if (os == null)
            {
                return NotFound();
            }

            if (os.Status == "Em Espera")
            {
                // Muda o status de volta para "Em Andamento"
                os.Status = "Em Andamento";
                _context.Update(os);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"A Ordem de Serviço #{os.Id} foi liberada da espera e retornou para 'Em Andamento'.";
            }
            else
            {
                TempData["Error"] = "Esta Ordem de Serviço não está 'Em Espera'.";
            }

            // Volta para a página de detalhes da mesma OS
            return RedirectToAction(nameof(Details), new { id });
        }
        // GET: OrdemDeServicos/Create
        public async Task<IActionResult> Create(int? salaId)
        {
            // Sempre carregamos a lista de salas para preencher o dropdown de filtro.
            ViewData["Salas"] = new SelectList(_context.Salas, "Id", "Nome");

            if (salaId.HasValue)
            {
                // Se uma sala foi selecionada, buscamos seus detalhes e ativos.
                var salaSelecionada = await _context.Salas
                    .Include(s => s.Ativos) // Incluímos os ativos relacionados
                    .FirstOrDefaultAsync(s => s.Id == salaId.Value);

                if (salaSelecionada == null)
                {
                    return NotFound();
                }

                // Retornamos a view com os dados da sala para renderizar o mapa
                return View(salaSelecionada);
            }

     
            return View();
        }


        // POST: OrdemDeServicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int AtivoId, string Descricao, IFormFile Imagem)
        {

            // 1. Buscar o ativo no banco de dados primeiro.
            var ativo = await _context.Ativos.FindAsync(AtivoId);

            // 2. Verificar se o ativo existe e se o status dele permite abrir uma nova OS.
            if (ativo == null)
            {
                TempData["Error"] = "O ativo selecionado não foi encontrado.";
                return RedirectToAction("AbrirChamado", "Professor");
            }

            if (ativo.Status != "Operacional")
            {
                TempData["Error"] = $"Não é possível abrir um chamado para o ativo '{ativo.Nome}', pois seu status atual é '{ativo.Status}'.";
                return RedirectToAction("AbrirChamado", "Professor", new { salaId = ativo.SalaId });
            }
            // --- FIM DA NOVA LÓGICA DE VALIDAÇÃO ---


            if (string.IsNullOrWhiteSpace(Descricao))
            {
                TempData["Error"] = "A descrição do problema é obrigatória.";
                return RedirectToAction("AbrirChamado", "Professor", new { salaId = ativo.SalaId });
            }

            var ordemDeServico = new OrdemDeServico
            {
                AtivoId = AtivoId,
                Descricao = Descricao,
                SolicitanteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                DataCriacao = DateTime.Now,
                Status = "Aberta"
            };

            if (Imagem != null && Imagem.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Imagem.CopyToAsync(memoryStream);
                    ordemDeServico.ImagemConteudo = memoryStream.ToArray();
                }
            }

            _context.Add(ordemDeServico);

           
            ativo.Status = "Manutencao";
            _context.Update(ativo);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Ordem de Serviço aberta com sucesso!";
            return RedirectToAction("MeusChamados", "Professor");
        }

        [HttpGet]
        public async Task<JsonResult> BuscarCadeiras(int salaId, string termo)
        {
            var cadeiras = await _context.Ativos
                .Where(a => a.SalaId == salaId &&
                             a.TipoAtivo.Nome == "Cadeira" &&
                             (a.Nome.Contains(termo) || a.NumeroSerie.Contains(termo)))
                .Select(a => new { id = a.Id, text = $"{a.Nome} ({a.NumeroSerie ?? "S/N"})" })
                .Take(10) 
                .ToListAsync();

            return Json(cadeiras);
        }
        [HttpGet]
        public async Task<IActionResult> GetAtivoDetails(int id)
        {
            var ativo = await _context.Ativos
                                      .Include(a => a.TipoAtivo)
                                      .FirstOrDefaultAsync(a => a.Id == id);

            if (ativo == null)
            {
                return NotFound();
            }

            // Retorna uma View Parcial com os detalhes do ativo e o formulário de OS
            return PartialView("_AtivoDetailsPartial", ativo);
        }
        [Authorize(Roles = RolesGlobais.Gestor)]
        public async Task<IActionResult> ListagemCompleta(string statusFiltro = "")
        {
            try
            {
                var query = _context.OrdensDeServico
                    .Include(o => o.Ativo)
                        .ThenInclude(a => a.Sala)
                    .Include(o => o.Solicitante)
                    .Include(o => o.Responsavel)
                    .AsQueryable();

                // Aplicar filtro por status se fornecido
                if (!string.IsNullOrEmpty(statusFiltro))
                {
                    query = query.Where(o => o.Status == statusFiltro);
                }

                // Ordenação especial: Em Espera primeiro, depois por data (mais antigas primeiro), Concluídas por último
                var ordensDeServico = await query
                    .OrderBy(o => o.Status == "Em Espera" ? 0 : o.Status == "Concluída" ? 2 : 1)
                    .ThenBy(o => o.DataCriacao)
                    .ToListAsync();

                // Carregar técnicos para o dropdown de atribuição
                var tecnicosManutencao = await _userManager.GetUsersInRoleAsync(RolesGlobais.Manutencao);
                ViewBag.TecnicosManutencao = new SelectList(tecnicosManutencao, "Id", "Nome");

                // Dados para o filtro
                ViewBag.StatusFiltro = statusFiltro;
                ViewBag.StatusOptions = new SelectList(new[]
                {
                    new { Value = "", Text = "Todos os Status" },
                    new { Value = "Aberta", Text = "Aberta" },
                    new { Value = "Em Andamento", Text = "Em Andamento" },
                    new { Value = "Em Espera", Text = "Em Espera" },
                    new { Value = "Concluída", Text = "Concluída" }
                }, "Value", "Text", statusFiltro);

                return View(ordensDeServico);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao carregar as ordens de serviço.";
                return View(new List<OrdemDeServico>());
            }
        }
        [HttpPost]
        [Authorize(Roles = RolesGlobais.Gestor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtribuirResponsavel(int ordemDeServicoId, int responsavelId, string returnUrl)
        {
            var os = await _context.OrdensDeServico.FindAsync(ordemDeServicoId);
            if (os == null)
            {
                return NotFound();
            }

            var tecnico = await _userManager.FindByIdAsync(responsavelId.ToString());
            if (tecnico == null)
            {
                TempData["Error"] = "Técnico de manutenção não encontrado.";
                return LocalRedirect(returnUrl ?? "/"); 
            }

            os.ResponsavelId = responsavelId;
            os.Status = "Em Andamento";

            if (!os.DataInicioExecucao.HasValue)
            {
                os.DataInicioExecucao = DateTime.Now;
            }
            _context.Update(os);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"OS #{os.Id} atribuída com sucesso para {tecnico.Nome}.";

         
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("DashboardGestor", "Home");
        }
        // GET: OrdemDeServicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordemDeServico = await _context.OrdensDeServico.FindAsync(id);
            if (ordemDeServico == null)
            {
                return NotFound();
            }
            ViewData["AtivoId"] = new SelectList(_context.Ativos, "Id", "Nome", ordemDeServico.AtivoId);
            ViewData["ResponsavelId"] = new SelectList(_context.Users, "Id", "Email", ordemDeServico.ResponsavelId);
            ViewData["SolicitanteId"] = new SelectList(_context.Users, "Id", "Email", ordemDeServico.SolicitanteId);
            return View(ordemDeServico);
        }

        // POST: OrdemDeServicos/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AtivoId,SolicitanteId,ResponsavelId,Descricao,Observacao,Status,DataCriacao,DataInicioExecucao,SlaAlvo")] OrdemDeServico ordemDeServico)
        {
            if (id != ordemDeServico.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordemDeServico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdemDeServicoExists(ordemDeServico.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AtivoId"] = new SelectList(_context.Ativos, "Id", "Nome", ordemDeServico.AtivoId);
            ViewData["ResponsavelId"] = new SelectList(_context.Users, "Id", "Email", ordemDeServico.ResponsavelId);
            ViewData["SolicitanteId"] = new SelectList(_context.Users, "Id", "Email", ordemDeServico.SolicitanteId);
            return View(ordemDeServico);
        }

        // GET: OrdemDeServicos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordemDeServico = await _context.OrdensDeServico
                .Include(o => o.Ativo)
                .Include(o => o.Responsavel)
                .Include(o => o.Solicitante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordemDeServico == null)
            {
                return NotFound();
            }

            return View(ordemDeServico);
        }

        // POST: OrdemDeServicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordemDeServico = await _context.OrdensDeServico.FindAsync(id);
            if (ordemDeServico != null)
            {
                _context.OrdensDeServico.Remove(ordemDeServico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdemDeServicoExists(int id)
        {
            return _context.OrdensDeServico.Any(e => e.Id == id);
        }
    }
}
