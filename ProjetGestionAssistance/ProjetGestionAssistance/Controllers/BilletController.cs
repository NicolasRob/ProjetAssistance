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
using System.Linq.Expressions;
using System.Reflection;

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
        public async Task<IActionResult> Index(String ordre, int? page, string sort, string direction)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }
            //Compte connecté
            Compte compte = _context.Compte.SingleOrDefault(cpt => cpt.Id == HttpContext.Session.GetInt32("_Id"));
            
            //nombre de billet par page
            int nbElementParPage = 5;

            string param;

            switch (sort)
            {
                case "Titre":
                    param = "Titre";
                    break;
                case "Description":
                    param = "Description";
                    break;
                case "Etat":
                    param = "Etat";
                    break;
                case "Auteur":
                    param = "Auteur";
                    break;
                case "Departement":
                    param = "Departement";
                    break;
                default:
                    param = "Id";
                    direction = "Down";
                    break;
            }

            var propertyInfo = typeof(Billet).GetProperty(param);

            // renvoie la vue billet seulement avec les billets que l'utiilisateur connecté à composé
            if (ordre == "compose" && compte.Actif)
            {
                ViewData["NomListeBillet"] = "composés";
                ViewData["ordre"] = "compose";
                ViewData["direction"] = direction;
                ViewData["sort"] = sort;
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.AuteurId == compte.Id);
                if (direction == "Down")
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderByDescending(b => propertyInfo.GetValue(b));
                else
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderBy(b => propertyInfo.GetValue(b));
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            
            // vérifie si l'utilisateur est minimalement un employé de service   => mise en place pour l'ajout d'assignation
            else if (ordre == "assigne" && compte.Type >= 1 && compte.Actif)
            {
                ViewData["NomListeBillet"] = "assignés";
                ViewData["ordre"] = "assigne";
                ViewData["direction"] = direction;
                ViewData["sort"] = sort;
                //var equipe = _context.Compte.SingleOrDefault(e => e.Id == compte.EquipeId);
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.EquipeId == compte.EquipeId).Where(b => b.CompteId == compte.Id);
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }

            else if (ordre == "equipe" && compte.Type >= 1 && compte.Actif)
            {
                ViewData["NomListeBillet"] = "équipe";
                ViewData["ordre"] = "equipe";
                //var equipe = _context.Compte.SingleOrDefault(e => e.Id == compte.EquipeId);
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.EquipeId == compte.EquipeId);
                if (direction == "Down")
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderByDescending(b => propertyInfo.GetValue(b));
                else
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderBy(b => propertyInfo.GetValue(b));
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }


            // vérifie si l'utilisateur est minimalement un gestionnaire
            else if (ordre == "departement" && compte.Type >= 2 && compte.Actif)
            {
                ViewData["NomListeBillet"] = "du département";
                ViewData["ordre"] = "departement";
                ViewData["direction"] = direction;
                ViewData["sort"] = sort;
                //Joel Lutumba - 2017-10-03
                /*Impossible d'acceder à l'id du département avec compte.Equipe.DepartementId sans
                  chercher l'équipe dont fait partie l'utilisateur connecté donc*/
                var equipe = _context.Equipe.SingleOrDefault(e => e.Id == compte.EquipeId);

                // option 1 - cherche les billets en comparant l'id de son déparment et l'id du département de l'équipe de l'utilisateur connecté
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.DepartementId == equipe.DepartementId);
                
                // option 2 - vu que c'est maintenant possible on peut aussi faire
                //var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.DepartementId == compte.Equipe.DepartementId);

                if (direction == "Down")
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderByDescending(b => propertyInfo.GetValue(b));
                else
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderBy(b => propertyInfo.GetValue(b));

                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            
            // revoie la vue billet avec tous les billets crées
            else if (ordre == "entreprise" && compte.Type >= 3 && compte.Actif)
            {
                ViewData["ordre"] = "entreprise";
                ViewData["NomListeBillet"] = "de tous les départements";
                ViewData["direction"] = direction;
                ViewData["sort"] = sort;
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Select(b => b);
                if (direction == "Down")
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderByDescending(b => propertyInfo.GetValue(b));
                else
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderBy(b => propertyInfo.GetValue(b));
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));
            }
            //si l'utilisateur n'est qu'un demandeur alors sa vue par defaut sera billets composés sinon ce sera la vue billets assignés
            else
            {
                //Pour l'instant lorsque le paramètre n'est pas définit on renvoie la vue des billets composés
                ViewData["ordre"] = "compose";
                ViewData["direction"] = direction;
                ViewData["sort"] = sort;
                var projetGestionAssistanceContext = _context.Billet.Include(b => b.Auteur).Include(b => b.Departement).Where(b => b.AuteurId == HttpContext.Session.GetInt32("_Id"));
                //return View(await projetGestionAssistanceContext.ToListAsync());
                if (direction == "Down")
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderByDescending(b => propertyInfo.GetValue(b));
                else
                    projetGestionAssistanceContext = projetGestionAssistanceContext.OrderBy(b => propertyInfo.GetValue(b));
                return View(await ListePaginee<Billet>.CreateAsync(projetGestionAssistanceContext, page ?? 1, nbElementParPage));

            }

        }

        // GET: Billet/Details/5
        public async Task<IActionResult> Details(int? id, string sort, string direction, String ordrePrecedent, int? pagePrecedente)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

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
                .OrderByDescending(c => c.DateCreation)
                .ToList();
            ViewData["sort"] = sort;
            ViewData["direction"] = direction;
            return View(billet);
        }

        // GET: Billet/Modification/5
        public async Task<IActionResult> Modification(int? id, string sort, string direction, String ordrePrecedent, int? pagePrecedente)
        {
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

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
            // Création d'une selectList avec les équipes qui font partie du département
            List<SelectListItem> equipe = new SelectList(_context.Set<Equipe>().Where(e => e.DepartementId == billet.DepartementId), "Id", "Nom", billet.EquipeId).ToList();
            // On insere une valeur 'aucune équipe' qui vaut pour NULL dans la base de donnée
            equipe.Insert(0, (new SelectListItem { Text = "Aucune équipe", Value = DBNull.Value.ToString() }));
            ViewData["EquipeId"] = equipe;
            
            //création d'un objet personnalisé pour permettre d'afficher le nom et le prenom, et les billets en cours des employés dans le SelectList
            var listeCompteBillet = (from cpt in _context.Compte
                            join b in _context.Billet on cpt.Id equals b.CompteId
                               where(b.Etat != "Nouveau" && b.Etat != "Fermé") 
                            group cpt by new { cpt.Id, cpt.Prenom, cpt.Nom, } into g
                            orderby g.Count() ascending
                            select new { g.Key.Id, g.Key.Prenom, g.Key.Nom, Count = g.Count() }
                            ).ToList();

            var listeCompteTout = (from cpt in _context.Compte
                               select new { cpt.Id, cpt.Prenom, cpt.Nom });

            

            var listeComptePersonnalisee =
              listeCompteBillet

                .Select(c => new
                {
                    compteID = c.Id,
                    Description = $"{c.Prenom} {c.Nom} | {c.Count} " + ((c.Count < 2) ? " billet" : " billets") + " en cours",
                })
                .ToList();

            listeComptePersonnalisee.Insert(0, new
            {
                compteID = -1,
                Description = "Sélectionnez un employé...",
            });

            List<int> listeID = new List<int>();
            foreach(var item in listeCompteBillet)
            {
                listeID.Add(item.Id);
            }

      
            int j = 1;


 
            foreach (var item in listeCompteTout)

            {
                if (listeID.Contains(item.Id))
                { }

                else
                {
                    listeComptePersonnalisee.Insert(j, new
                    {
                        compteID = item.Id,
                        Description = $"{item.Prenom} {item.Nom}",
                    });
                    j++;
                }

         


            }

            ViewData["CompteId"] = new SelectList(listeComptePersonnalisee, "compteID", "Description");
  

            //Liste des États du billets
            List < String > listeEtat = new List<string>(new string[] { "Nouveau", "En traitement", "Fermé" });
            ViewData["Etat"] = listeEtat.Select(x => new SelectListItem()
            {
                Text = x.ToString()
            });

            ViewData["sort"] = sort;
            ViewData["direction"] = direction;

            return View(billet);
        }

        // POST: Billet/Modification/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modification(int id, int compteId, IFormFile fichierPhoto, [Bind("Id,Titre,Description,Image,Etat,Commentaires,AuteurId,DepartementId,EquipeId")] Billet billet)
        {

            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

            if (id != billet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (compteId > 0)
                    {
                        billet.CompteId = compteId;
                        billet.Etat = "En traitement";
                    }
                    else
                    {
                        billet.CompteId = null;
                    }


                    if (fichierPhoto != null)
                    {

                        string filePath = "./images/billet" + billet.AuteurId + "-" + billet.Id;
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
                        billet.Image = "TEST TEST";


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
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

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
                    var filePath = "/images/billets/billet" + billet.AuteurId+"-"+idBilletTemp+".jpg";
                    try
                    {
                        //Copie du fichierPhoto dans notre dossier local
                        using (var stream = new FileStream("./wwwroot/" + filePath, FileMode.Create))
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
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

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
            if (HttpContext.Session.GetInt32("_Id") == null) {
                return RedirectToAction("Login", "Compte");
            }

            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            var cheminImage = "./wwwroot/" + billet.Image; //chemin à partir de la racine de l'application
            if (System.IO.File.Exists(cheminImage)) {
                System.IO.File.Delete(cheminImage);
            }
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

        /*public async Task<IActionResult> Commentaire(int? id, String ordrePrecedent, int? pagePrecedente)
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
        }*/

        public async Task<IActionResult> AjouterCommentaire(string Texte, int BilletId, String ordrePrecedent, int? pagePrecedente)
        {
            Billet billet = _context.Billet.SingleOrDefault(b => b.Id == BilletId);
            int? sessionId = HttpContext.Session.GetInt32("_Id");
            int? sessionType = HttpContext.Session.GetInt32("_Type");
            Compte compte = null;
            if (sessionId != null)
                compte = _context.Compte.Include(c => c.Equipe).SingleOrDefault(c => c.Id == sessionId);
            if (billet != null && compte != null 
                && (compte.Id == billet.CompteId 
                    || (compte.Type == 2 && compte.Equipe.DepartementId == billet.DepartementId)
                    || (compte.Type >= 3)))
            {
                if (string.IsNullOrWhiteSpace(Texte))
                {
                    ViewData["TexteVide"] = "Le commentaire ne peut pas être vide.";
                }
                else if (ModelState.IsValid)
                {
                    Commentaire commentaire = new Commentaire();
                    commentaire.BilletId = BilletId;
                    commentaire.Texte = Texte;
                    commentaire.AuteurId = HttpContext.Session.GetInt32("_Id");
                    commentaire.DateCreation = DateTime.Now;
                    _context.Add(commentaire);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { Id = BilletId, ordrePrecedent = ordrePrecedent, pagePrecedente = pagePrecedente });
                }
                ViewData["commentaires"] = _context.Commentaire
                    .Include(c => c.Auteur)
                    .Include(c => c.Billet)
                    .Where(c => c.BilletId == BilletId)
                    .OrderByDescending(c => c.DateCreation)
                    .ToList();
                ViewData["ordrePrecedent"] = ordrePrecedent;
                ViewData["pagePrecedente"] = pagePrecedente ?? 1;
                return View("Details", billet);
            }
            else
                return View("/Views/Home/Index.cshtml");
        }


        public async Task<IActionResult> Accepter(int id, String ordrePrecedent, int? pagePrecedente)
        {


            var billet = await _context.Billet.SingleOrDefaultAsync(m => m.Id == id);
            Console.WriteLine(billet.Description);
                if (billet == null)
                {
                    return NotFound();
                }

                billet.CompteId = HttpContext.Session.GetInt32("_Id");
                billet.Etat = "EN TRAITEMENT";
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

        // Fonction créé par Francis Paré : 11-17-2017
        // Elle retourne la liste de toute les équipes faisant 
        // partie du département donné en paramètre. 
        public JsonResult ListeEquipeParDepartementId(int Id)
        {
            List<Equipe> listEquipe = new List<Equipe>(_context.Equipe.Where(e => e.DepartementId == Id));
            return Json(listEquipe);
        }

    }

}
