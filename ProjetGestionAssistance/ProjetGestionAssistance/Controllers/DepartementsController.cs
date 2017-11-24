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
    public class DepartementsController : Controller
    {
        private readonly ProjetGestionAssistanceContext _context;

        public DepartementsController(ProjetGestionAssistanceContext context)
        {
            _context = context;    
        }

        // GET: Departements
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

            return View(await _context.Departement.ToListAsync());
        }

        // GET: Departements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

            if (id == null)
            {
                return NotFound();
            }

            var departement = await _context.Departement
                .SingleOrDefaultAsync(m => m.Id == id);
            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        // GET: Departements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom")] Departement departement)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }
            else if (HttpContext.Session.GetInt32("_Type") >= 3) {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Add(departement);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(departement);
        }

        // GET: Departements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }
            else if (HttpContext.Session.GetInt32("_Type") >= 3) {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var departement = await _context.Departement.SingleOrDefaultAsync(m => m.Id == id);
            if (departement == null)
            {
                return NotFound();
            }
            return View(departement);
        }

        // POST: Departements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom")] Departement departement)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }
            else if (HttpContext.Session.GetInt32("_Type") >= 3) {
                return RedirectToAction("Index", "Home");
            }

            if (id != departement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartementExists(departement.Id))
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
            return View(departement);
        }

        // GET: Departements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }
            else if (HttpContext.Session.GetInt32("_Type") >= 3) {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var departement = await _context.Departement
                .SingleOrDefaultAsync(m => m.Id == id);

            if (departement == null)
            {
                return NotFound();
            }

            return View(departement);
        }

        // POST: Departements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }
            else if (HttpContext.Session.GetInt32("_Type") >= 3) {
                return RedirectToAction("Index", "Home");
            }

            var billets = await _context.Billet.Where(b => b.DepartementId == id).ToListAsync();
            foreach (var b in billets)
            {
                b.DepartementId = null;
            }
            var departement = await _context.Departement.SingleOrDefaultAsync(m => m.Id == id);
            _context.Departement.Remove(departement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool DepartementExists(int id)
        {
            return _context.Departement.Any(e => e.Id == id);
        }
    }
}
