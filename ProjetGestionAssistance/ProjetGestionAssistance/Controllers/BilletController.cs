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
using ProjetGestionAssistance.Models.Services;

namespace ProjetGestionAssistance.Controllers
{
    public class BilletController : Controller
    {
        //Initialisation de la variable _context pour intéragir avec la base de données
        private readonly ProjetGestionAssistanceContext _context;

        public BilletController(ProjetGestionAssistanceContext context)
        {
            _context = context;
        }
        //Joel Lutumba - 2017-09-30
        //Paramètre ordre est utilisé pour savoir quels billets aller chercher & afficher
        //Paramètre page peut être null; il sert à la pagination.
        public async Task<IActionResult> Index(String ordre, int? page)
        {
            //Compte connecté
            Compte compte = _context.Compte.SingleOrDefault(cpt => cpt.Id == HttpContext.Session.GetInt32("_Id"));
            
            //nombre de billet par page
            int nbElementParPage = 5;

            // renvoie la vue billet seulement avec les billets que l'utiilisateur connecté à composé
            if (ordre == "compose")
            {
                ViewData["NomListeBillet"] = "composés";
                ViewData["ordre"] = "compose";
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.AuteurId == compte.Id);
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            
            // vérifie si l'utilisateur est minimalement un employé de service   => mise en place pour l'ajout d'assignation
            else if (ordre == "assigne" && compte.Type >= 1)
            {
                ViewData["NomListeBillet"] = "assignés";
                ViewData["ordre"] = "assigne";
                //var equipe = _context.Compte.SingleOrDefault(e => e.Id == compte.EquipeId);
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.EquipeId == compte.EquipeId);
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            
            // vérifie si l'utilisateur est minimalement un gestionnaire
            else if (ordre == "departement" && compte.Type >= 2)
            {
                ViewData["NomListeBillet"] = "du département";
                ViewData["ordre"] = "departement";
                //Joel Lutumba - 2017-10-03
                /*Impossible d'acceder à l'id du département avec compte.Equipe.DepartementId sans
                  chercher l'équipe dont fait partie l'utilisateur connecté donc*/
                var equipe = _context.Equipe.SingleOrDefault(e => e.Id == compte.EquipeId);

                // option 1 - cherche les billets en comparant l'id de son déparment et l'id du département de l'équipe de l'utilisateur connecté
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.DepartementId == equipe.DepartementId);
                
                // option 2 - vu que c'est maintenant possible on peut aussi faire
                //var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.DepartementId == compte.Equipe.DepartementId);

                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            
            // revoie la vue billet avec tous les billets crées
            else if (ordre == "entreprise" && compte.Type >= 3)
            {
                ViewData["ordre"] = "entreprise";
                ViewData["NomListeBillet"] = "de tous les départements";
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement);
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            //si l'utilisateur n'est qu'un demandeur alors sa vue par defaut sera billets composés sinon ce sera la vue billets assignés
            else
            {
                //Pour l'instant lorsque le paramètre n'est pas définit on renvoie la vue des billets composés
                ViewData["ordre"] = "compose";
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.AuteurId == HttpContext.Session.GetInt32("_Id"));
                //return View(await projetGestionAssistanceContext.ToListAsync());
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));

            }

        }

        // GET: Billet/Details/5
        public async Task<IActionResult> Details(int? id, String ordrePrecedent, int? pagePrecedente)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ordrePrecedent"] = ordrePrecedent;
            ViewData["pagePrecedente"] = pagePrecedente ?? 1;
            var billet = await _context.Billet
                .Include(b => b.Auteur)
                .Include(b => b.Departement)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (billet == null)
            {
                return NotFound();
            }
            ViewData["commentaires"] = _context.Commentaire
                .Include(c => c.Auteur)
                .Include(c => c.Billet)
                .Where(c => c.BilletId == id)
                .ToList();
            return View(billet);
        }

        // GET: Billet/Modification/5
        public async Task<IActionResult> Modification(int? id, String ordrePrecedent, int? pagePrecedente)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["ordrePrecedent"] = ordrePrecedent;
            ViewData["pagePrecedente"] = pagePrecedente ?? 1;
            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            if (billet == null)
            {
                return NotFound();
            }
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel", billet.AuteurId);
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom", billet.DepartementId);
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom",billet.EquipeId);
            return View(billet);
        }

        // POST: Billet/Modification/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modification(int id, [Bind("Id,Titre,Description,Etat,Image,Commentaires,AuteurId,DepartementId,EquipeId")] Billet billet)
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
            // Cherche la valeur des paramètres cachés dans du formulaire qui a envoyé la request post et les envoie comme paramètre à L'action Billet.Index() 
            var ordrePrecedent = HttpContext.Request.Form["ordrePrecedent"];
            var pagePrecedente = HttpContext.Request.Form["pagePrecedente"];
                return RedirectToAction("Index", new { @ordre=ordrePrecedent, @page=pagePrecedente});
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
                billet.CompteId = null;
                billet.EquipeId = null;


                if (fichierPhoto != null)
                {
                    //Le path du fichier de l'image est construit à partir de l'ID de l'auteur et de l'ID du billet
                    //Cette méthode fonctionne si on ajoute UNE SEULE photo par billet.

                    //On va chercher le ID du dernier billet, pour ajouter l'ID du billet présent dans le path.
                    Billet billetTemp = _context.Billet.LastOrDefault();
                    int idBilletTemp;
                    //S'il n'y a pas encore de billet dans la BD, on initalise l'ID à 0
                    if (billetTemp == null)
                        idBilletTemp = 0;
                    else
                        idBilletTemp = billetTemp.Id + 1;
                    var filePath = "./images/billet"+billet.AuteurId+"-"+idBilletTemp;
                    try
                    {
                        //Copie du fichierPhoto dans notre dossier local
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fichierPhoto.CopyToAsync(stream);
                        }

                        billet.Image = filePath; //copie du chemin d'accès du fichier dans l'attribut Image du billet
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine("Erreur : " + e.Message);
                    }

                }

                else
                    billet.Image = "";

                //Enregistrement du billet dans la base de données
                _context.Add(billet);
         
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Billet");
            }
            ViewData["AuteurId"] = new SelectList(_context.Compte, "Id", "Courriel", billet.AuteurId);
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Id", billet.DepartementId);
            return View(billet);
        }

        // GET: Billet/Suppression/5
        public async Task<IActionResult> Suppression(int? id, String ordrePrecedent, int? pagePrecedente)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ordrePrecedent"] = ordrePrecedent;
            ViewData["pagePrecedente"] = pagePrecedente?? 1;
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

        // POST: Billet/Suppression/5
        [HttpPost, ActionName("Suppression")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuppressionConfirmee(int id)
        {
            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            _context.Billet.Remove(billet);
            await _context.SaveChangesAsync();

            var ordrePrecedent = HttpContext.Request.Form["ordrePrecedent"];
            var pagePrecedente = HttpContext.Request.Form["pagePrecedente"];

            return RedirectToAction("Index", new { @ordre = ordrePrecedent, @page = pagePrecedente });
        }

        private bool BilletExists(int id)
        {
            return _context.Billet.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Commentaire(int? id, String ordrePrecedent, int? pagePrecedente)
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
            var commentaire = new Commentaire();
            commentaire.Billet = billet;
            ViewData["ordrePrecedent"] = ordrePrecedent;
            ViewData["pagePrecedente"] = pagePrecedente ?? 1;
            return View(commentaire);
        }

        public async Task<IActionResult> AjouterCommentaire([Bind("Id,Texte")] Commentaire commentaire, int BilletId, String ordrePrecedent, int? pagePrecedente)
        {
            if (ModelState.IsValid)
            {
                commentaire.BilletId = BilletId;
                commentaire.AuteurId = HttpContext.Session.GetInt32("_Id");
                commentaire.DateCreation = DateTime.Now;
                _context.Add(commentaire);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { Id = BilletId, ordrePrecedent = ordrePrecedent, pagePrecedente = pagePrecedente });
            }
            ViewData["ordrePrecedent"] = ordrePrecedent;
            ViewData["pagePrecedente"] = pagePrecedente ?? 1;
            return View("Commentaire", commentaire);
        }


        public async Task<IActionResult> Accepter(int id, String ordrePrecedent, int? pagePrecedente)
        {

            Console.WriteLine("TESTTTTT");

            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            Console.WriteLine(billet.Description);
                if (billet == null)
                {
                    return NotFound();
                }

                billet.CompteId = HttpContext.Session.GetInt32("_Id");
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

        

            return RedirectToAction("Index", new { @ordre = ordrePrecedent, @page = pagePrecedente });
        }

    }
}
