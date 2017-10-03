using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjetGestionAssistance.Models.Services
{
    public class ListePaginee<T> : List<T>
    {
        public int IndexDePage { get; private set; }
        public int NbPagesTotal { get; private set; }

        public ListePaginee(List<T> listeElements, int nbElementTotal, int indexDePage, int nbElementParPage)
        {
            IndexDePage = indexDePage;
            NbPagesTotal = (int)Math.Ceiling(nbElementTotal / (double)nbElementParPage); //Arrondit vers le haut pour permettre un page de plus.

            this.AddRange(listeElements);
        }

        public bool APagePrecedente
        {
            get
            {
                return (IndexDePage > 1);
            }
        }

        public bool APageSuivante
        {
            get
            {
                return (IndexDePage < NbPagesTotal);
            }
        }

        public bool PageCourante(int page)
        {
            return IndexDePage == page;
        }


        //Elle reçoit une liste de données, la page présente et, le nombre d'élément par page et retourne une version de liste en format PaginatedList Async
        public static async Task<ListePaginee<T>> CreateAsync(IQueryable<T> source, int indexDePage, int nbElementParPage)
        {
            var nbElementTotal = await source.CountAsync();
            var listeElements = await source.Skip((indexDePage - 1) * nbElementParPage).Take(nbElementParPage).ToListAsync();
            return new ListePaginee<T>(listeElements, nbElementTotal, indexDePage, nbElementParPage);
        }
    }
}
