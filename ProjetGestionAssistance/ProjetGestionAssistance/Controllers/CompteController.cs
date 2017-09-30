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
    //Controlleur qui g�re les logins et les cr�ations de comptes
    public class CompteController : Controller
    {
        //Initialisation de la variable _context pour int�ragir avec la base de donn�es
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

        //Francis Par� 2017-09-27
        //Action appel� quand on appuie sur le bouton connection
        //Si la connection fonctionne, On set le SessionId avec son Id d'utilisateur, il sera maintenant consid�r� connect�
        //Puis, On le redige vers l'action Index dans le controlleur HomeController()
        //**** A not� que l'utilisateur devra etre rediriger vers sa liste de billet lorsqu'elle sera impl�menter ****
        [HttpPost]
        public IActionResult Connection([Bind("Courriel")]string Courriel, [Bind("MotPasse")]string MotPasse)
        {
            if (ModelState.IsValid)
            {
                Compte tempCompte = _context.Compte.SingleOrDefault(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse);
                if (tempCompte != null)
                {
                    HttpContext.Session.SetInt32(SessionId, tempCompte.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Compte");
        }

        //Action qui affiche la page de cr�ation de compte
        public IActionResult AffichageCreation()
        {
            //Ce ViewData sera utilis� dans le formulaire de cr�ation pour afficher une liste des ID des �quipes de la BD
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom");
            return View("Creation");
        }

        //Action qui s'�x�cute quand on appuie sur le bouton Cr�ation du compte
        //Les champs du formulaire POST seront automatiquement utilis�s pour cr�er un objet compte appropri�
        //Cet objet sera ensuite ajouter � la BD
        //Attention: Il faut avoir cr�er au moins une �quipe pour cr�er un utilisateur
        //Sinon, la liste des �quipes sera vide et le formulaire ne sera jamais accept�
        public async Task<IActionResult> Creation([Bind("Id,Courriel,MotPasse,ConfirmationMotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
            compte.Type = 1;
            compte.Actif = true;
            if (ModelState.IsValid)
            {
                //_contexte repr�sente la BD, .Add est une m�thode de DAO qui a �t� g�n�r� automatiquement
                //et compte est l'objet cr�� par le formulaire
                _context.Add(compte);
                await _context.SaveChangesAsync();
                //On redirige ensuite l'utilisateur vers la page de login
                //On utilise RedirectToAction plut�t que View pour �viter les doubles submit
                return RedirectToAction("Login");
            }
            //Si la cr�ation �choue, on redirige l'utilisateur vers la vue de Creation
            //Cependant, il faut recr�er la liste des Id des �quipes puisqu'on a perdu le ViewData pr�c�dent
            //lorsqu'on a appuy� sur le bouton de soumission.
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            return View();
        }
    }
}