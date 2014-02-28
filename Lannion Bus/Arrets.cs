using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lannion_Bus
{
    public class Arret
    {
        public string arret { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string ligne { get; set; }
        public double distance { get; set; }
    }

    public class Ligne
    {
        public string ligne { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public List<Arret> arrets { get; set; }
    }

    public class Arrets
    {
        public  List<Ligne> lignes { get; set; }
    }
}
