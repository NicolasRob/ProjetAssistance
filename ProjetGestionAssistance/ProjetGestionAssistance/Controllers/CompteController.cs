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

        //Implémentation temporaire
        //Action appelé quand on appuie sur le bouton connection
        //On set le SessionId à 1, l'utilisateur sera maintenant considéré connecté
        //On le redige ensuite vers l'action Index dans le controlleur HomeController
        public IActionResult Connection()
        {
            HttpContext.Session.SetInt32(SessionId, 1);
            return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> Creation([Bind("Id,Courriel,MotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
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
            //Si la création échoue, on redirige l'utilisateur vers la vue de Creation
            //Cependant, il faut recréer la liste des Id des équipes puisqu'on a perdu le ViewData précédent
            //lorsqu'on a appuyé sur le bouton de soumission.
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            return View();
        }
    }
}