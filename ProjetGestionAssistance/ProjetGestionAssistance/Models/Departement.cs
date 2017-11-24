using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetGestionAssistance.Models
{
    public class Departement : IComparable
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public int CompareTo(object obj)
        {
            try
            {
                if (obj is null)
                    return 1;
                else
                {
                    Departement d2 = obj as Departement;
                    return this.Nom.CompareTo(d2.Nom);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
                return 1;
            }
        }
    }
}
