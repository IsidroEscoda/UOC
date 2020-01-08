using System;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Model
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
        public int ResErr { get; set; }
        //public  CheckBoxResume[] CBR { get; set; }
        //public  int[] CheckBoxes { get; set; }
        public int Adivinanzas { get; set; }
        public int Sopas { get; set; }
        public int Personales { get; set; }
        public int Enigmas { get; set; }
        public int Diferencias { get; set; }
        public int Sociales { get; set; }
        public int Musica { get; set; }
        public int Relax { get; set; }
        public int Accion { get; set; }
    }
}
