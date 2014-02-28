using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lannion_Bus
{
    public class Holliday
    {
        public string description { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }

    public class Hollidays
    {
        public string last_update { get; set; }
        public List<Holliday> hollidays { get; set; }
    }
  
}
