using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjetGestionAssistance.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ProjetGestionAssistance.Controllers
{
    public class BilletController : Controller
    {
        //Initialisation de la variable _context pour intéragir avec la base de données
        private readonly ProjetGestionAssistanceContext _context;
        //compteur qui sera ajouté au nom de fichier de l'image du billet, pour rendre le nom unique.
        private int compteurImage = 0;

        public BilletController(ProjetGestionAssistanceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement);
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
                .Include(b => b.Departement)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (billet == null)
            {
                return NotFound();
            }

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
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel", billet.AuteurId);
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Id", billet.DepartementId);
            return View(billet);
        }

        // POST: Billets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,Etat,Image,Commentaires,AuteurId,DepartementId")] Billet billet)
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
                
            }
            return RedirectToAction("Index");
        }

        
        //Get : Création d'un billet
        public IActionResult Creation()
        {
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel");
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom");
            return View("Creation");
        }
        

        //Post : Création d'un billet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Creation([Bind("Id,Titre,Description,Etat,Image,Commentaires,AuteurId,DepartementId")] Billet billet, IFormFile fichierPhoto)
        {
            if (ModelState.IsValid)
            {
                // Attribut AuteurId du billet = Id de l'utilisateur en cours
                billet.AuteurId = (int)HttpContext.Session.GetInt32("_Id");
                //L'état d'un billet est initialisé à "Nouveau"
                billet.Etat = "Nouveau";

                //Le path du fichier de l'image est construit à partir de l'ID de l'auteur et de l'ID du billet
                //Il faudra trouver une autre manière de nommer les fichiers si on accepte plus d'une photo par billet
                Billet billetTemp = _context.Billet.LastOrDefault();
                int idBilletTemp = billetTemp.Id+1;
                var filePath = "./images/billet"+billet.AuteurId+"-"+idBilletTemp;  // À MODIFIER : Il faut trouver un moyen de construire des noms de fichiers uniques.
                compteurImage++;
                //Copie du fichierPhoto dans notre dossier local
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fichierPhoto.CopyToAsync(stream);
                }
                
                billet.Image = filePath; //copie du chemin d'accès du fichier dans l'attribut Image du billet

                //Enregistrement du billet dans la base de données
                _context.Add(billet);
         
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Billet");
            }
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel", billet.AuteurId);
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Id", billet.DepartementId);
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
                .Include(b => b.Departement)
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

    