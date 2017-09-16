using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Compte
    {
        public int Id { get; set; }
        public string Courriel { get; set; }
        public string MotPasse { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public int Type { get; set; }
        public bool Actif { get; set; }

        public int EquipeId { get; set; }

        [ForeignKey("EquipeId")]
        public Equipe Equipe { get; set; }

    }
}
