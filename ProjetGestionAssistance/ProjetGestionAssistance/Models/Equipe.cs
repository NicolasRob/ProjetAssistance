using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public int DepartementId { get; set; }

        [ForeignKey("DepartementId")]
        public Departement Departement { get; set; }
    }
}
