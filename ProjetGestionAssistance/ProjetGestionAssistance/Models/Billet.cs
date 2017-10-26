using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Billet
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Le titre du billet est obligatoire")]
        public string Titre { get; set; }
        [Required(ErrorMessage = "La description du billet est obligatoire")]
        public string Description { get; set; }
        public string Etat { get; set; }
        public string Image { get; set; }
        public string Commentaires { get; set; }

        public int AuteurId { get; set; }

        [ForeignKey("AuteurId")]
        public Compte Auteur { get; set; }

        public int? CompteId { get; set; }

        [ForeignKey("CompteId")]
        public Compte Compte { get; set; }

        public int? DepartementId { get; set; }

        [ForeignKey("DepartementId")]
        public Departement Departement { get; set; }

        public int? EquipeId { get; set; }

        [ForeignKey("EquipeId")]
        public Departement Equipe { get; set; }
    }
}
