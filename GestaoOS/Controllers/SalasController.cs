using GestaoOS.Data;
using GestaoOS.Models;
using GestaoOS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoOS.Controllers
{
    [Authorize] 
    public class SalasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;


        public SalasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        public async Task<IActionResult> Index()
        {
            var professores = await _userManager.GetUsersInRoleAsync(RolesGlobais.Professor);
            ViewData["ResponsavelId"] = new SelectList(professores, "Id", "Nome");

            return View(await _context.Salas.ToListAsync()); 
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        public async Task<IActionResult> Create()
        {
            var professores = await _userManager.GetUsersInRoleAsync(RolesGlobais.Professor);

            ViewData["ResponsavelId"] = new SelectList(professores, "Id", "Nome");
            ViewData["SalasCadastradas"] = _context.Salas.ToList();

            return View();
        }

        public async Task<IActionResult> Filter(string texto)
        {
            var professores = await _userManager.GetUsersInRoleAsync(RolesGlobais.Professor);

            ViewData["ResponsavelId"] = new SelectList(professores, "Id", "Nome");
            ViewData["SalasCadastradas"] = _context.Salas.Where(x => x.Nome.Contains(texto)).ToList();

            return View("Create");
        }

        // POST: Salas/Create
      
        // POST: Salas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Bloco,Descricao,ResponsavelId")] Sala sala)
        {
            // ... (Removemos as validações de propriedades que não vêm do formulário)
            ModelState.Remove("Responsavel");
            ModelState.Remove("Posicoes");
            ModelState.Remove("Ativos");

            if (ModelState.IsValid)
            {
                // 1. Salva a nova sala no banco de dados
                _context.Add(sala);
                await _context.SaveChangesAsync();

                var posicoesParaAdicionar = new List<Posicao>();
                for (int i = 1; i <= 40; i++)
                {
                    posicoesParaAdicionar.Add(new Posicao { NumeroPosicao = i, SalaId = sala.Id });
                }
        
                await _context.Posicoes.AddRangeAsync(posicoesParaAdicionar);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sala e suas 40 posições criadas com sucesso!";
                return RedirectToAction(nameof(Create));
            }
    
            ViewData["ResponsavelId"] = new SelectList(_context.Users, "Id", "Nome", sala.ResponsavelId);
            return View(sala);
        }
        // GET: Salas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null) return NotFound();

            var professores = await _userManager.GetUsersInRoleAsync(RolesGlobais.Professor);

            // 2. Cria o SelectList, já pré-selecionando o responsável atual da sala.
            ViewData["ResponsavelId"] = new SelectList(professores, "Id", "Nome", sala.ResponsavelId);

            return View(sala);
        }
        // POST: Salas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,ResponsavelId,Bloco")] Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sala);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id))
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
            return View(sala);
        }

        // GET: Salas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // POST: Salas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala != null)
            {
                _context.Salas.Remove(sala);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }
    }
}