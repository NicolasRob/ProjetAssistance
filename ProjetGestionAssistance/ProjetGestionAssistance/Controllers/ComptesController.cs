using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetGestionAssistance.Models;

namespace ProjetGestionAssistance.Controllers
{
    public class ComptesController : Controller
    {
        private readonly ProjetGestionAssistanceContext _context;

        public ComptesController(ProjetGestionAssistanceContext context)
        {
            _context = context;    
        }

        // GET: Comptes
        public async Task<IActionResult> Index()
        {
            var projetGestionAssistanceContext = _context.Compte.Include(c => c.Equipe);
            return View(await projetGestionAssistanceContext.ToListAsync());
        }

        // GET: Comptes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compte = await _context.Compte
                .Include(c => c.Equipe)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (compte == null)
            {
                return NotFound();
            }

            return View(compte);
        }

        // GET: Comptes/Create
        public IActionResult Create()
        {
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Id");
            return View();
        }

        // POST: Comptes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Courriel,MotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(compte);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Id", compte.EquipeId);
            return View(compte);
        }

        // GET: Comptes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compte = await _context.Compte.SingleOrDefaultAsync(m => m.Id == id);
            if (compte == null)
            {
                return NotFound();
            }
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Id", compte.EquipeId);
            return View(compte);
        }

        // POST: Comptes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Courriel,MotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
            if (id != compte.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compte);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompteExists(compte.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Id", compte.EquipeId);
            return View(compte);
        }

        // GET: Comptes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compte = await _context.Compte
                .Include(c => c.Equipe)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (compte == null)
            {
                return NotFound();
            }

            return View(compte);
        }

        // POST: Comptes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compte = await _context.Compte.SingleOrDefaultAsync(m => m.Id == id);
            _context.Compte.Remove(compte);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CompteExists(int id)
        {
            return _context.Compte.Any(e => e.Id == id);
        }
    }
}
