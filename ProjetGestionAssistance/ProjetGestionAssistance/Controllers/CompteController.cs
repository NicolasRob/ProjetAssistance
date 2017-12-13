using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjetGestionAssistance.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetGestionAssistance.Models.Services;
using System.Reflection;

namespace ProjetGestionAssistance.Controllers
{
    //Controlleur qui gére les logins et les créations de comptes
    public class CompteController : Controller
    {
        //Initialisation de la variable _context pour intéragir avec la base de données
        private readonly ProjetGestionAssistanceContext _context;

        public CompteController(ProjetGestionAssistanceContext context)
        {
            _context = context;
        }

        const string SessionId = "_Id";

        //Redirige vers la vue Compte/Login.cshtml
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("_Id") != null) {
                return RedirectToAction("Index","Home");
            }
            return View();                
        }

        //Francis Paré 2017-09-27
        //Action appelé quand on appuie sur le bouton connection
        //Si la connection fonctionne, On set le SessionId avec son Id d'utilisateur, il sera maintenant considéré connecté
        //Puis, On le redige vers l'action Index dans le controlleur HomeController()
        //**** A noté que l'utilisateur devra etre rediriger vers sa liste de billet lorsqu'elle sera implémenter ****
        [HttpPost]
        public IActionResult Connection([Bind("Courriel")]string Courriel, [Bind("MotPasse")]string MotPasse)
        {
            if (HttpContext.Session.GetInt32("_Id") != null) {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                //Compte tempCompte = _context.Compte.SingleOrDefault(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse);
                Compte tempCompte = _context.Compte.Include(cpt => cpt.Equipe).Where(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse).AsEnumerable().SingleOrDefault(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse);
                if (tempCompte != null)
                {
                    // On vérifie si le compte de l'utilisateur est actif
                    if (!tempCompte.Actif)
                    {
                        // Le compte n'est pas actif, on le redirige vers le vue de login.
                        // Puis, on affiche un message à l'utilisateur lui indiquant que son compte n'est pas actif
                        ViewData["MessageErreurConnection"] = "Votre compte n'est pas activé. Veuillez contacter un supérieur pour plus d'information.";
                        return View("Login");
                    }
                    //Joel Lutumba 2017-10-01 -ajout
                    //typeUtilisateur : variable dont la valeur est stockée dans la session pour avoir le type de l'utilisateur connecté
                    string typeConnecte = "_Type";
                    string departementConnecte = "_Dep";

                    HttpContext.Session.SetInt32(SessionId, tempCompte.Id);
                    HttpContext.Session.SetInt32(typeConnecte, tempCompte.Type);
                    HttpContext.Session.SetInt32(departementConnecte, tempCompte.Equipe.DepartementId);
                    ViewData["connection"] = tempCompte.Id;
                    return RedirectToAction("Index", "Home");
                }
                // On affiche ce message d'erreur quand le courriel utilisateur ou le mot de passe saisie est incorrect.
                ViewData["MessageErreurConnection"] = "Le courriel ou le mot de passe entré est incorrect";
            }
            return View("Login");
        }

        //Action qui affiche la page de création de compte
        public IActionResult AffichageCreation()
        {
            if (HttpContext.Session.GetInt32("_Id") != null) {
                return RedirectToAction("Index", "Home");
            }
            //Ce ViewData sera utilisé dans le formulaire de création pour afficher une liste des ID des équipes de la BD
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom");
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom");
            return View("Creation");
        }

        //Action qui s'éxécute quand on appuie sur le bouton Création du compte
        //Les champs du formulaire POST seront automatiquement utilisés pour créer un objet compte approprié
        //Cet objet sera ensuite ajouter à la BD
        //Attention: Il faut avoir créer au moins une équipe pour créer un utilisateur
        //Sinon, la liste des équipes sera vide et le formulaire ne sera jamais accepté
        public async Task<IActionResult> Creation([Bind("Id,Courriel,MotPasse,ConfirmationMotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {

            if (HttpContext.Session.GetInt32("_Id") != null) {
                return RedirectToAction("Index", "Home");
            }

            if (_context.Compte.SingleOrDefault(cpt => cpt.Courriel == compte.Courriel) == null)
            {
                compte.Type = 1;
                compte.Actif = false;
                if (ModelState.IsValid)
                {
                    //_contexte représente la BD, .Add est une méthode de DAO qui a été généré automatiquement
                    //et compte est l'objet créé par le formulaire
                    _context.Add(compte);
                    await _context.SaveChangesAsync();
                    //On redirige ensuite l'utilisateur vers la page de login
                    //On utilise RedirectToAction plutôt que View pour éviter les doubles submit
                    return RedirectToAction("Login");
                }
            }
            else
                ViewData["CourrielErreur"] = "Le courriel entré est déja en utilisation";
            //Si la création échoue, on redirige l'utilisateur vers la vue de Creation
            //Cependant, il faut recréer la liste des Id des équipes puisqu'on a perdu le ViewData précédent
            //lorsqu'on a appuyé sur le bouton de soumission.
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            return View();
        }
        //Francis Paré : 2017-10-07
        // Action qui déconnecte l'utilisateur de la session
        // On clear l'élément session, puis on redirige l'utilisateur à la page de connection
        public IActionResult Deconnection()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login","Compte");
        }

        public async Task<IActionResult> AfficherGestionCompte(String ordre, int? page)
        {

            if (HttpContext.Session.GetInt32("_Id") == null || HttpContext.Session.GetInt32("_Type") < 3) {
                return RedirectToAction("Login", "Compte");
            }

            //nombre de billet par page
            int nbElementParPage = 5;

            IOrderedQueryable<Compte> listeCompte;
            switch (ordre)
            {
                case "IdUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Id);
                    break;
                case "IdDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Id);
                    break;
                case "PrenomUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Prenom);
                    break;
                case "PrenomDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Prenom);
                    break;
                case "NomUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Nom);
                    break;
                case "NomDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Nom);
                    break;
                case "CourrielUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Courriel);
                    break;
                case "CourrielDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Courriel);
                    break;
                case "EquipeUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Equipe.Nom);
                    break;
                case "EquipeDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Equipe.Nom);
                    break;
                case "TypeUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Type);
                    break;
                case "TypeDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Type);
                    break;
                case "EtatUp":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Actif);
                    break;
                case "EtatDown":
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Actif);
                    break;
                default:
                    ordre = "IdDown";
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderByDescending(c => c.Id);
                    break;
            }
            ViewData["ordre"] = ordre;
            return View("GestionCompte", await ListePaginee<Compte>.CreateAsync(listeCompte, page ?? 1, nbElementParPage));

        }


        public async Task<IActionResult> AfficherModificationCompte(int? id, string ordre, int? page)
        {
            if (HttpContext.Session.GetInt32("_Id") == null || HttpContext.Session.GetInt32("_Type") < 3) {
                return RedirectToAction("Login", "Compte");
            }

            if (id == null)
            {
                return NotFound();
            }

            var compte = await _context.Compte.Include(m => m.Equipe).SingleOrDefaultAsync(m => m.Id == id);
            if (compte == null)
            {
                return NotFound();
            }
            ViewData["ordre"] = ordre;
            ViewData["page"] = page;
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            ViewData["DepartementId"] = new SelectList(_context.Departement, "Id", "Nom", compte.Equipe.DepartementId);
            return View("ModificationCompte", compte);
        }

        public async Task<IActionResult> ModifierCompte(int id, [Bind("Id,Courriel,MotPasse,ConfirmationMotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
            if (HttpContext.Session.GetInt32("_Id") == null || HttpContext.Session.GetInt32("_Type") < 3) {
                return RedirectToAction("Login", "Compte");
            }

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
            }
            ViewData["MessageModificationCompte"] = "Les changements ont &eacute;t&eacute; enregistr&eacute;s avec succ&egrave;s !";
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            return View("ModificationCompte", compte);
        }

        public async Task<IActionResult> ModifierEtatCompte(int? id, string ordre, int? page)
        {

            if (HttpContext.Session.GetInt32("_Id") == null || HttpContext.Session.GetInt32("_Type") < 3) {
                return RedirectToAction("Login", "Compte");
            }

            Compte compte = await _context.Compte.SingleOrDefaultAsync(c => c.Id == id);
            compte.Actif = !compte.Actif;
            //compte.ConfirmationMotPasse = compte.MotPasse;
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
            }
            return RedirectToAction("AfficherGestionCompte", new { ordre = ordre, page = page });
        }

        public JsonResult ListeEquipeParDepartementId(int Id)
        {
            List<Equipe> listEquipe = new List<Equipe>(_context.Equipe.Where(e => e.DepartementId == Id));
            return Json(listEquipe);
        }

        private bool CompteExists(int id)
        {
            return _context.Compte.Any(e => e.Id == id);
        }
    }
}