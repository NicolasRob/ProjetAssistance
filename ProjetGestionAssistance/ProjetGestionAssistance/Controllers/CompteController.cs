using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProjetGestionAssistance.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjetGestionAssistance.Controllers
{
    //Controlleur qui gère les logins et les créations de comptes
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
            if (ModelState.IsValid)
            {
                Compte tempCompte = _context.Compte.SingleOrDefault(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse);
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

                    HttpContext.Session.SetInt32(SessionId, tempCompte.Id);
                    HttpContext.Session.SetInt32(typeConnecte, tempCompte.Type);
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
            //Ce ViewData sera utilisé dans le formulaire de création pour afficher une liste des ID des équipes de la BD
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom");
            return View("Creation");
        }

        //Action qui s'éxécute quand on appuie sur le bouton Création du compte
        //Les champs du formulaire POST seront automatiquement utilisés pour créer un objet compte approprié
        //Cet objet sera ensuite ajouter à la BD
        //Attention: Il faut avoir créer au moins une équipe pour créer un utilisateur
        //Sinon, la liste des équipes sera vide et le formulaire ne sera jamais accepté
        public async Task<IActionResult> Creation([Bind("Id,Courriel,MotPasse,ConfirmationMotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
            compte.Type = 1;
            compte.Actif = true;
            if (_context.Compte.SingleOrDefault(cpt => cpt.Courriel == compte.Courriel) == null)
            {
                compte.Type = 1;
                compte.Actif = true;
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
    }
}