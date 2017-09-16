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
    public class BilletsController : Controller
    {
        private readonly ProjetGestionAssistanceContext _context;

        public BilletsController(ProjetGestionAssistanceContext context)
        {
            _context = context;    
        }

        // GET: Billets
        public async Task<IActionResult> Index()
        {
            var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur);
            return View(await projetGestionAssistanceContext.ToListAsync());
        }

        // GET: Billets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billet = await _context.Billet
                .Include(b => b.Auteur)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (billet == null)
            {
                return NotFound();
            }

            return View(billet);
        }

        // GET: Billets/Create
        public IActionResult Create()
        {
            ViewData["AuteurId"] = new SelectList(_context.Set<Compte>(), "Id", "Id");
            return View();
        }

        // POST: Billets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,Etat,Image,Commentaires,AuteurId")] Billet billet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billet);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["AuteurId"] = new SelectList(_context.Set<Compte>(), "Id", "Id", billet.AuteurId);
            return View(billet);
        }

        // GET: Billets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            if (billet == null)
            {
                return NotFound();
            }
            ViewData["AuteurId"] = new SelectList(_context.Set<Compte>(), "Id", "Id", billet.AuteurId);
            return View(billet);
        }

        // POST: Billets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,Etat,Image,Commentaires,AuteurId")] Billet billet)
        {
            if (id != billet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BilletExists(billet.Id))
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
            ViewData["AuteurId"] = new SelectList(_context.Set<Compte>(), "Id", "Id", billet.AuteurId);
            return View(billet);
        }

        // GET: Billets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var billet = await _context.Billet
                .Include(b => b.Auteur)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (billet == null)
            {
                return NotFound();
            }

            return View(billet);
        }

        // POST: Billets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            _context.Billet.Remove(billet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BilletExists(int id)
        {
            return _context.Billet.Any(e => e.Id == id);
        }
    }
}
