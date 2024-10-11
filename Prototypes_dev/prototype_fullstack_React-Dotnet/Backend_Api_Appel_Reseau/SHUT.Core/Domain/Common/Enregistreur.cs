using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Domain.Common
{
    public enum Liaison
    {
        Tcp,
        Ftp
    }


    public class Enregistreur
    {
        public List<Station> stations { get; set; } = new List<Station>();
        public string? IPAdress { get; set; }
        public int Port { get; set; }
        public Liaison Liasion { get; set; }
    }


}
