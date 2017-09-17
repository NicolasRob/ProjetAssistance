using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetGestionAssistance.Models;

namespace ProjetGestionAssistance.Models
{
    public class ProjetGestionAssistanceContext : DbContext
    {
        public ProjetGestionAssistanceContext (DbContextOptions<ProjetGestionAssistanceContext> options)
            : base(options)
        {
        }

        public DbSet<ProjetGestionAssistance.Models.Billet> Billet { get; set; }

        public DbSet<ProjetGestionAssistance.Models.Compte> Compte { get; set; }

        public DbSet<ProjetGestionAssistance.Models.Departement> Departement { get; set; }

        public DbSet<ProjetGestionAssistance.Models.Equipe> Equipe { get; set; }

    }
}
