using System;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Model
{
    public class Challenge2
    {
        public string ID { get; set; }
        public string TYPE_CHECK { get; set; }
        public string STATEMENT { get; set; }
        public string TIMEINSECONDS { get; set; }
        public string PHOTO { get; set; }
        public string HINT { get; set; }
        public string SOLUCION { get; set; }

        /*public int Points { get; set; } = 10;
        public int PointsIfRepeat { get; set; } = 10;
        public int PointsWithHint { get; set; } = 5;
        public int PenaltyPoints { get; set; } = 0;

        public bool Completed { get; set; }*/
    }
}
