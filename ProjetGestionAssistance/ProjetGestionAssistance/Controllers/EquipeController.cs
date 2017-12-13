using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetGestionAssistance.Models;
using Microsoft.AspNetCore.Http;

namespace ProjetGestionAssistance.Controllers
{
    public class EquipeController : Controller
    {
        private readonly ProjetGestionAssistanceContext _context;

        public EquipeController(ProjetGestionAssistanceContext context)
        {
            _context = context;    
        }

        // GET: Equipes
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("_Id") == null)
                return RedirectToAction("Login", "Compte");

            var projetGestionAssistanceContext = _context.Equipe.Include(e => e.Departement);
            return View(await projetGestionAssistanceContext.ToListAsync());
        }

        // GET: Equipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null)
                return RedirectToAction("Login", "Compte");

            if (id == null)
            {
                return NotFound();
            }

            var equipe = await _context.Equipe
                .Include(e => e.Departement)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (equipe == null)
            {
                return NotFound();
            }

            return View(equipe);
        }

        // GET: Equipes/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("_Id") == null)
                return RedirectToAction("Login", "Compte");

            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom");
            return View();
        }

        // POST: Equipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,DepartementId")] Equipe equipe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipe);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom", equipe.DepartementId);
            return View(equipe);
        }

        // GET: Equipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null)
                return RedirectToAction("Login", "Compte");

            if (id == null)
            {
                return NotFound();
            }

            var equipe = await _context.Equipe.SingleOrDefaultAsync(m => m.Id == id);
            if (equipe == null)
            {
                return NotFound();
            }
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom", equipe.DepartementId);
            return View(equipe);
        }

        // POST: Equipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,DepartementId")] Equipe equipe)
        {
            if (HttpContext.Session.GetInt32("_Id") == null)
                return RedirectToAction("Login", "Compte");

            if (id != equipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipeExists(equipe.Id))
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
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Id", equipe.DepartementId);
            return View(equipe);
        }

        // GET: Equipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null)
                return RedirectToAction("Login", "Compte");

            if (id == null)
            {
                return NotFound();
            }

            var equipe = await _context.Equipe
                .Include(e => e.Departement)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (equipe == null)
            {
                return NotFound();
            }

            return View(equipe);
        }

        // POST: Equipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipe = await _context.Equipe.SingleOrDefaultAsync(m => m.Id == id);
            _context.Equipe.Remove(equipe);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EquipeExists(int id)
        {
            return _context.Equipe.Any(e => e.Id == id);
        }
    }
}
