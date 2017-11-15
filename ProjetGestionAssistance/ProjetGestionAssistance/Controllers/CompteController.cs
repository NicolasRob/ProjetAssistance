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
                //Compte tempCompte = _context.Compte.SingleOrDefault(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse);
                Compte tempCompte = _context.Compte.Include(cpt => cpt.Equipe).Where(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse).AsEnumerable().SingleOrDefault(cpt => cpt.Courriel == Courriel & cpt.MotPasse == MotPasse);
                if (tempCompte != null)
                {
                    // On v�rifie si le compte de l'utilisateur est actif
                    if (!tempCompte.Actif)
                    {
                        // Le compte n'est pas actif, on le redirige vers le vue de login.
                        // Puis, on affiche un message � l'utilisateur lui indiquant que son compte n'est pas actif
                        ViewData["MessageErreurConnection"] = "Votre compte n'est pas activ�. Veuillez contacter un sup�rieur pour plus d'information.";
                        return View("Login");
                    }
                    //Joel Lutumba 2017-10-01 -ajout
                    //typeUtilisateur : variable dont la valeur est stock�e dans la session pour avoir le type de l'utilisateur connect�
                    string typeConnecte = "_Type";
                    string departementConnecte = "_Dep";

                    HttpContext.Session.SetInt32(SessionId, tempCompte.Id);
                    HttpContext.Session.SetInt32(typeConnecte, tempCompte.Type);
                    HttpContext.Session.SetInt32(departementConnecte, tempCompte.Equipe.DepartementId);
                    ViewData["connection"] = tempCompte.Id;
                    return RedirectToAction("Index", "Home");
                }
                // On affiche ce message d'erreur quand le courriel utilisateur ou le mot de passe saisie est incorrect.
                ViewData["MessageErreurConnection"] = "Le courriel ou le mot de passe entr� est incorrect";
            }
            return View("Login");
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
            if (_context.Compte.SingleOrDefault(cpt => cpt.Courriel == compte.Courriel) == null)
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
            }
            else
                ViewData["CourrielErreur"] = "Le courriel entr� est d�ja en utilisation";
            //Si la cr�ation �choue, on redirige l'utilisateur vers la vue de Creation
            //Cependant, il faut recr�er la liste des Id des �quipes puisqu'on a perdu le ViewData pr�c�dent
            //lorsqu'on a appuy� sur le bouton de soumission.
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            return View();
        }
        //Francis Par� : 2017-10-07
        // Action qui d�connecte l'utilisateur de la session
        // On clear l'�l�ment session, puis on redirige l'utilisateur � la page de connection
        public IActionResult Deconnection()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login","Compte");
        }

        public async Task<IActionResult> AfficherGestionCompte(String ordre, int? page)
        {
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
                    ordre = "IdUp";
                    listeCompte = _context.Compte.Include(b => b.Equipe).OrderBy(c => c.Id);
                    break;
            }
            ViewData["ordre"] = ordre;
            return View("GestionCompte", await ListePaginee<Compte>.CreateAsync(listeCompte, page ?? 1, nbElementParPage));

        }


        public async Task<IActionResult> AfficherModificationCompte(int? id, string ordre, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compte = await _context.Compte.SingleOrDefaultAsync(m => m.Id == id);
            if (compte == null)
            {
                return NotFound();
            }
            ViewData["ordre"] = ordre;
            ViewData["page"] = page;
            ViewData["EquipeId"] = new SelectList(_context.Set<Equipe>(), "Id", "Nom", compte.EquipeId);
            return View("ModificationCompte", compte);
        }

        public async Task<IActionResult> ModifierCompte(int id, [Bind("Id,Courriel,MotPasse,ConfirmationMotPasse,Nom,Prenom,Telephone,Type,Actif,EquipeId")] Compte compte)
        {
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

        private bool CompteExists(int id)
        {
            return _context.Compte.Any(e => e.Id == id);
        }
    }
}