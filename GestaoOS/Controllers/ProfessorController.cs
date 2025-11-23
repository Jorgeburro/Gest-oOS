using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoOS.Controllers
{
    [Authorize(Roles = RolesGlobais.Professor)]
    public class ProfessorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ProfessorController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User));
        }

        // GET: /Professor/AbrirChamado
        // Lógica similar ao Ativos/Gerenciar, para mostrar o mapa ao professor.
        public async Task<IActionResult> AbrirChamado(int? salaId)
        {
            ViewData["Salas"] = new SelectList(await _context.Salas.OrderBy(s => s.Nome).ToListAsync(), "Id", "Nome", salaId);

            Sala salaSelecionada = null;
            if (salaId.HasValue)
            {
                salaSelecionada = await _context.Salas
                    .Include(s => s.Posicoes.OrderBy(p => p.NumeroPosicao))
                        .ThenInclude(p => p.Ativos)
                            .ThenInclude(a => a.TipoAtivo)
                    .Include(s => s.Ativos) // Inclui TODOS os ativos para a próxima etapa
                    .FirstOrDefaultAsync(s => s.Id == salaId.Value);

                if (salaSelecionada == null) return NotFound();

              
            }

            return View(salaSelecionada);
        }

        // GET: /Professor/DetalhesPosicaoParaOSPartial
        // Chamado via AJAX para popular o modal quando um professor clica na posição do mapa.
        public async Task<IActionResult> DetalhesPosicaoParaOSPartial(int posicaoId)
        {
            var posicao = await _context.Posicoes
                .Include(p => p.Ativos)
                    .ThenInclude(a => a.TipoAtivo)
                .FirstOrDefaultAsync(p => p.Id == posicaoId);

            if (posicao == null) return NotFound();

            // Retorna a partial view que contém o formulário para abrir a OS.
            return PartialView("_DetalhesPosicaoParaOSPartial", posicao);
        }

        // GET: Professor/MeusChamados
        // Lista as OS criadas pelo professor OU de salas onde ele é responsável.
        public async Task<IActionResult> MeusChamados()
        {
            var userId = GetCurrentUserId();

            var ordensDeServico = await _context.OrdensDeServico
                .Include(o => o.Ativo)
                    .ThenInclude(a => a.Sala)
                .Include(o => o.Solicitante)
                .Include(o => o.Responsavel)
                .Where(o =>
                    o.SolicitanteId == userId ||             // OS que eu abri
                    (o.Ativo != null && o.Ativo.Sala.ResponsavelId == userId)) // OS da sala que sou responsável
                .OrderByDescending(o => o.DataCriacao)
                .ToListAsync();

            return View(ordensDeServico);
        }

        public async Task<IActionResult> Validar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordemDeServico = await _context.OrdensDeServico
                .Include(o => o.Ativo).ThenInclude(a => a.Sala)
                .Include(o => o.Responsavel) // Técnico que executou
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordemDeServico == null || ordemDeServico.Status != "Concluída")
            {
                TempData["Error"] = "Ordem de Serviço não encontrada ou ainda não foi concluída pelo técnico.";
                return RedirectToAction(nameof(MeusChamados));
            }

            // Verificação de segurança: garante que o usuário logado pode validar esta OS.
            var userId = GetCurrentUserId();
            bool isSolicitante = ordemDeServico.SolicitanteId == userId;
            bool isResponsavelSala = ordemDeServico.Ativo?.Sala?.ResponsavelId == userId;

            if (!isSolicitante && !isResponsavelSala)
            {
                TempData["Error"] = "Você não tem permissão para validar esta Ordem de Serviço.";
                return RedirectToAction(nameof(MeusChamados));
            }

            return View(ordemDeServico);
        }

        // POST: Professor/Validar
        [HttpPost, ActionName("Validar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarConfirmado(int id, string observacaoValidacao)
        {
            var osOriginal = await _context.OrdensDeServico
                .Include(o => o.Ativo.Sala)       
                .Include(o => o.Ativo.TipoAtivo)  
                .Include(o => o.Solicitante)      
                .Include(o => o.Responsavel)      
                .FirstOrDefaultAsync(o => o.Id == id);

            if (osOriginal?.Ativo == null || osOriginal.Solicitante == null) 
            {
                TempData["Error"] = "Erro ao carregar dados da OS original para arquivamento.";
                // Tenta encontrar a OS básica para redirecionar se a carga completa falhar
                var osBasica = await _context.OrdensDeServico.FindAsync(id);
                return RedirectToAction(nameof(Validar), new { id = osBasica?.Id ?? id });
            }
            if (osOriginal.Status != "Concluída")
            {
                TempData["Error"] = "Esta OS não está no status 'Concluída' e não pode ser validada.";
                return RedirectToAction(nameof(MeusChamados));
            }


            // Busca o usuário logado que está validando
            var usuarioValidando = await _userManager.GetUserAsync(User);
            if (usuarioValidando == null)
            {
                return Challenge(); 
            }

            // --- Início da Lógica de Arquivamento (Atualizada) ---

            // 1. Criar o registro de Histórico copiando os VALORES
            var historico = new OrdemDeServicoHistorico
            {
                OrdemDeServicoIdOriginal = osOriginal.Id,

                // Snapshot do Ativo
                AtivoIdOriginal = osOriginal.AtivoId,
                AtivoNome = osOriginal.Ativo.Nome,
                AtivoNumeroSerie = osOriginal.Ativo.NumeroSerie,
                AtivoSalaNome = osOriginal.Ativo.Sala?.Nome ?? "Sala Desconhecida", 
                AtivoTipoNome = osOriginal.Ativo.TipoAtivo?.Nome ?? "Tipo Desconhecido", 

                // Snapshot dos Usuários
                SolicitanteIdOriginal = osOriginal.SolicitanteId,
                SolicitanteNome = osOriginal.Solicitante.Nome, 
                ResponsavelIdOriginal = osOriginal.ResponsavelId,
                ResponsavelNome = osOriginal.Responsavel?.Nome,
                ResponsavelValidacaoIdOriginal = usuarioValidando.Id,
                ResponsavelValidacaoNome = usuarioValidando.Nome, 

                // Dados da OS
                Descricao = osOriginal.Descricao,
                SolucaoAplicada = osOriginal.Observacao, 
                ObservacaoValidacao = observacaoValidacao, 

                // Datas
                DataCriacao = osOriginal.DataCriacao,
                DataInicioExecucao = osOriginal.DataInicioExecucao,
                DataConclusao = osOriginal.DataConclusao ?? DateTime.Now,
                DataValidacao = DateTime.Now
            };

         
            osOriginal.Ativo.Status = "Funcional";
            _context.Update(osOriginal.Ativo);

            _context.OrdensDeServicoHistorico.Add(historico);
            _context.OrdensDeServico.Remove(osOriginal);

            await _context.SaveChangesAsync();


            TempData["Success"] = $"Ordem de Serviço #{osOriginal.Id} validada e arquivada com sucesso!";
            return RedirectToAction(nameof(MeusChamados));
        }
    }
}