using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoOS.Controllers
{
    [Authorize(Roles = RolesGlobais.Gestor)]
    public class AtivosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AtivosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Ativos/Gerenciar
        public async Task<IActionResult> Gerenciar(int? salaId)
        {
            ViewData["Salas"] = new SelectList(await _context.Salas.OrderBy(s => s.Nome).ToListAsync(), "Id", "Nome", salaId);

            Sala salaSelecionada = null;
            if (salaId.HasValue)
            {
                salaSelecionada = await _context.Salas
                    .Include(s => s.Posicoes.OrderBy(p => p.NumeroPosicao))
                        .ThenInclude(p => p.Ativos)
                            .ThenInclude(a => a.TipoAtivo)
                    .Include(s => s.Ativos.Where(a => a.PosicaoId == null)) // Inclui ativos móveis
                        .ThenInclude(a => a.TipoAtivo)
                    .FirstOrDefaultAsync(s => s.Id == salaId.Value);

                if (salaSelecionada == null) return NotFound();
            }

            return View(salaSelecionada);
        }

        // GET: /Ativos/DetalhesPosicaoPartial
        public async Task<IActionResult> DetalhesPosicaoPartial(int posicaoId)
        {
            var posicao = await _context.Posicoes
                .Include(p => p.Ativos)
                    .ThenInclude(a => a.TipoAtivo)
                .FirstOrDefaultAsync(p => p.Id == posicaoId);

            if (posicao == null) return NotFound();

            ViewData["TipoAtivoId"] = new SelectList(
                _context.TipoAtivos.Where(t => t.Nome == "Computador" || t.Nome == "Teclado" || t.Nome == "Mouse" || t.Nome == "Mesa"),
                "Id", "Nome"
            );

            return PartialView("_DetalhesPosicaoPartial", posicao);
        }

        // POST: /Ativos/AdicionarAtivoFixo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarAtivoFixo(Ativo ativo)
        {
            ModelState.Remove("Sala");
            ModelState.Remove("TipoAtivo");
            ModelState.Remove("Posicao");

            // Regra: não permitir duplicidade de tipo na mesma posição
            if (ativo.PosicaoId.HasValue)
            {
                var jaExisteMesmoTipoNaPosicao = await _context.Ativos
                    .AnyAsync(a => a.PosicaoId == ativo.PosicaoId && a.TipoAtivoId == ativo.TipoAtivoId);

                if (jaExisteMesmoTipoNaPosicao)
                {
                    return BadRequest(new { success = false, message = "Já existe um ativo desse tipo nesta posição." });
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(ativo);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Ativo adicionado com sucesso!" });
            }
            return BadRequest(new { success = false, message = "Dados inválidos." });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is not null)
            {
                var ativo = _context.Ativos.Include(x => x.Sala).FirstOrDefault(x => x.Id == id);
                return View(ativo);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: /Ativos/GerenciarCadeiras?salaId=5
        // Este método prepara e exibe a NOVA PÁGINA de gerenciamento de cadeiras.
        public async Task<IActionResult> GerenciarCadeiras(int salaId)
        {
            var sala = await _context.Salas.FindAsync(salaId);
            if (sala == null)
            {
                return NotFound();
            }

            var cadeiras = await _context.Ativos
                .Where(a => a.SalaId == salaId && a.TipoAtivo.Nome == "Cadeira")
                .OrderBy(a => a.Nome)
                .ToListAsync();

            ViewBag.Sala = sala; 

            var descricoes = new List<SelectListItem>
            {
                new SelectListItem { Value = "Cadeira com rodas", Text = "Com rodas" },
                new SelectListItem { Value = "Cadeira com pé fixo", Text = "Com pé fixo" }
            };
            ViewData["DescricaoOptions"] = new SelectList(descricoes, "Value", "Text");

            return View(cadeiras); 
        }

        // POST: /Ativos/AdicionarCadeira
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarCadeira(int salaId, string nome, string descricao)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(descricao))
            {
                TempData["Error"] = "O Nome e a Descrição da cadeira são obrigatórios.";
                return RedirectToAction("GerenciarCadeiras", new { salaId });
            }

            var tipoAtivoCadeira = await _context.TipoAtivos.FirstOrDefaultAsync(t => t.Nome == "Cadeira");
            if (tipoAtivoCadeira == null)
            {
                TempData["Error"] = "Tipo de ativo 'Cadeira' não encontrado no banco de dados.";
                return RedirectToAction("GerenciarCadeiras", new { salaId });
            }

            var novaCadeira = new Ativo
            {
                Nome = nome,
                Descricao = descricao,
                SalaId = salaId,
                TipoAtivoId = tipoAtivoCadeira.Id,
                PosicaoId = null
            };

            _context.Add(novaCadeira);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cadeira adicionada com sucesso!";
            return RedirectToAction("GerenciarCadeiras", new { salaId });
        }

        // POST: /Ativos/ExcluirAtivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirAtivo(int ativoId)
        {
            var ativo = await _context.Ativos.FindAsync(ativoId);
            if (ativo == null) return NotFound();

            int salaId = ativo.SalaId;

            _context.Ativos.Remove(ativo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Gerenciar", new { salaId }); // Redireciona para a mesma sala
        }
        public async Task<IActionResult> EditAtivo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ativo = await _context.Ativos.FindAsync(id);
            if (ativo == null)
            {
                return NotFound();
            }

            // Se o ativo for uma cadeira, prepara o dropdown de descrição
            if (ativo.TipoAtivoId == 4) // Assumindo que o ID 4 é "Cadeira"
            {
                var descricoes = new List<SelectListItem>
        {
            new SelectListItem { Value = "Cadeira com rodas", Text = "Com rodas" },
            new SelectListItem { Value = "Cadeira com pé fixo", Text = "Com pé fixo" }
        };
                ViewData["DescricaoOptions"] = new SelectList(descricoes, "Value", "Text", ativo.Descricao);
            }

            return View(ativo);
        }

        // POST: Ativos/EditAtivo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAtivo(int id, [Bind("Id,Nome,Descricao,NumeroSerie,Status,SalaId,TipoAtivoId,PosicaoId")] Ativo ativo)
        {
            if (id != ativo.Id)
            {
                return NotFound();
            }

            // Removemos a validação de propriedades de navegação que não são postadas
            ModelState.Remove("Sala");
            ModelState.Remove("TipoAtivo");
            ModelState.Remove("Posicao");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ativo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Ativos.Any(e => e.Id == ativo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Ativo atualizado com sucesso!";
                // Redireciona de volta para o mapa da sala onde o ativo está
                return RedirectToAction(nameof(Gerenciar), new { salaId = ativo.SalaId });
            }
            return View(ativo);
        }
    }
}