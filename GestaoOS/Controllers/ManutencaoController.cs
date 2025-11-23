using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoOS.Controllers
{
    [Authorize(Roles = RolesGlobais.Manutencao)]
    public class ManutencaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ManutencaoController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_userManager.GetUserId(User));
        }


        public async Task<IActionResult> MinhasOrdens()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                var ordensDeServico = await _context.OrdensDeServico
                    .Include(o => o.Ativo)
                        .ThenInclude(a => a.Sala)
                    .Include(o => o.Solicitante)
                    .Where(o => o.ResponsavelId == userId)
                    .OrderBy(o => o.Status == "Concluída" ? 2 : o.Status == "Em Espera" ? 1 : 0)
                    .ThenBy(o => o.DataCriacao) 
                    .ToListAsync();

                return View(ordensDeServico);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao carregar suas ordens de serviço.";
                return View(new List<OrdemDeServico>());
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarOS(int ordemDeServicoId, DateTime? slaAlvo, string status, string observacao)
        {
            var userId = GetCurrentUserId();
            var os = await _context.OrdensDeServico.FirstOrDefaultAsync(o => o.Id == ordemDeServicoId && o.ResponsavelId == userId);

            if (os == null)
            {
                TempData["Error"] = "Ordem de Serviço não encontrada ou você não tem permissão para editá-la.";
                return RedirectToAction(nameof(MinhasOrdens));
            }

            var statusValidos = new[] { "Em Andamento", "Em Espera", "Concluída" };
            if (string.IsNullOrWhiteSpace(status) || !statusValidos.Contains(status))
            {
                TempData["Error"] = "O status fornecido é inválido.";
                return RedirectToAction(nameof(MinhasOrdens));
            }

            os.SlaAlvo = slaAlvo;
            os.Status = status;


       
            if (status == "Em Espera")
            {
                if (string.IsNullOrWhiteSpace(observacao))
                {
                    TempData["Error"] = "É obrigatório preencher a justificativa para o status 'Em Espera'.";
                    return RedirectToAction(nameof(MinhasOrdens));
                }
                os.Observacao = observacao;
            }

            if (status == "Concluída")
            {
                os.DataConclusao = DateTime.Now;
                
            }

            try
            {
                _context.Update(os);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Ordem de Serviço #{os.Id} atualizada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocorreu um erro ao salvar as alterações: " + ex.Message;
            }

            return RedirectToAction(nameof(MinhasOrdens));
        }
    }
}