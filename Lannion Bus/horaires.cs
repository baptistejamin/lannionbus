using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lannion_Bus
{
    public class Horaires
    {
        public string arret { get; set; }
        public int horaire { get; set; }
        public string id { get; set; }
    }

    public class Tournee
    {
        public bool velo { get; set; }
        public List<Horaires> horaires { get; set; }
    }

    public class Horaire
    {
        public string ligne { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public int monday { get; set; }
        public int tuesday { get; set; }
        public int wednesday { get; set; }
        public int thursday { get; set; }
        public int friday { get; set; }
        public int saturday { get; set; }
        public int sunday { get; set; }
        public List<Tournee> tournees { get; set; }
    }

    public class RootObject
    {
        public string last_update { get; set; }
        public List<Horaire> horaires { get; set; }
    }
}
