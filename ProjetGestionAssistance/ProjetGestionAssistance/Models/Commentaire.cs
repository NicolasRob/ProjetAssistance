using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Commentaire
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Le texte du commentaire ne peut être vide.")]
        public string Texte { get; set; }
        public DateTime DateCreation { get; set; }

        public int? AuteurId { get; set; }

        [ForeignKey("AuteurId")]
        public Compte Auteur { get; set; }

        public int? BilletId { get; set; }

        [ForeignKey("BilletId")]
        public Billet Billet { get; set; }


    }
}
