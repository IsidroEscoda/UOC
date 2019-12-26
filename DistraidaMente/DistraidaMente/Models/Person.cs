using System;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Models
{
    public class Person
    {
        public bool Video { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string DocId { get; set; }
        public int PruRea { get; set; }
        public int PisSol { get; set; }
        public int PruSal { get; set; }
        public int ResAce { get; set; }
        public int ResErr{ get; set; }
    }
}
