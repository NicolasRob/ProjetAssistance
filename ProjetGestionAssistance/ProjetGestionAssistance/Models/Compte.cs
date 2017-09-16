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
        [Required]
        public string Courriel { get; set; }
        [Required]
        public string MotPasse { get; set; }
        [Required]
        public string Nom { get; set; }
        [Required]
        public string Prenom { get; set; }
        [Required]
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
        [Compare("MotPasse")]
        public string ConfirmationMotPasse { get; set; }

    }
}
