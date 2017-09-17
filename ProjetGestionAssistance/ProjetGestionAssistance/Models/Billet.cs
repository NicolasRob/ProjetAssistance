using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Billet
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string Etat { get; set; }
        public string Image { get; set; }
        public string Commentaires { get; set; }

        public int AuteurId { get; set; }

        [ForeignKey("AuteurId")]
        public Compte Auteur { get; set; }

        public int? DepartementId { get; set; }

        [ForeignKey("DepartementId")]
        public Departement Departement { get; set; }
    }
}
