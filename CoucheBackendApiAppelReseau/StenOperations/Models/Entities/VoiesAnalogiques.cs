using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Models.Entities
{
    public class VoiesAnalogiques
    {
        /// <summary>
        /// Primary key in the datatable, used in postgres database
        /// </summary>
        public int Id { get; set; }
        public int Num { get; set; }
        public string Libel { get; set; } = "";
        public int Ve { get; set; }
        public int Defaut { get; set; }
        public int Virgule { get; set; }
        public bool Aga { get; set; }
        public int Seuil { get; set; }
        public int Delta { get; set; }
        public int AdrBES { get; set; }
        public int Info { get; set; }
        public float SeuilB { get; set; }
        public float SeuilH { get; set; }
        public float Vdelta { get; set; }
        public int Groupe { get; set; }
        public float Zero { get; set; }
        public float Cent { get; set; }
    }
}
