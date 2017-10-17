using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Compte
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vous devez entrer une adresse de courriel valide")]
        public string Courriel { get; set; }
        [Required(ErrorMessage = "Vous devez entrer un mot de passe")]
        public string MotPasse { get; set; }
        [Required(ErrorMessage = "Vous devez entrer votre nom")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Vous devez entrer votre prenom")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "Vous devez entrer un numéro de téléphone valide")]
        public string Telephone { get; set; }
        [Required]
        public int Type { get; set; }
        
        public bool Actif { get; set; }

        public int EquipeId { get; set; }

        [ForeignKey("EquipeId")]
        public Equipe Equipe { get; set; }

        //L'annotation NotMapped signifie que l'attribut n'est pas sauvegardé dans la base de données
        //L'annotation Compate("MotPasse") signifie que ConfirmationMotPasse doit être identique à MotPasse
        //pour que l'objet soit valide
        [NotMapped]
        [Compare("MotPasse", ErrorMessage = "La confirmation de mot de passe est différente du mot de passe")]
        public string ConfirmationMotPasse { get; set; }

    }
}
