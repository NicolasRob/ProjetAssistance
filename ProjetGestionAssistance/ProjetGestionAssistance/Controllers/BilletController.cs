using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjetGestionAssistance.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProjetGestionAssistance.Controllers
{
    public class BilletController : Controller
    {
        //Initialisation de la variable _context pour int�ragir avec la base de donn�es
        private readonly ProjetGestionAssistanceContext _context;

        public BilletController(ProjetGestionAssistanceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement);
            return View(await projetGestionAssistanceContext.ToListAsync());
        }


        public IActionResult Creation()
        {
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel");
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Id");
            return View("Creation");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creation([Bind("Id,Titre,Description,Etat,Image,Commentaires,AuteurId,DepartementId")] Billet billet)
        {
            if (ModelState.IsValid)
            {
                billet.Etat = "Nouveau";
                _context.Add(billet);
                await _context.SaveChangesAsync();

                //� CHANGER pour la vue des billets fait par Joel
                return RedirectToAction("Index", "Billet");
            }
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel", billet.AuteurId);
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Id", billet.DepartementId);
            return View(billet);
        }


    }
}