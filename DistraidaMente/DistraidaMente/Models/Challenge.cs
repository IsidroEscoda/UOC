using System;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Model
{
    public class Challenge
    {
        public string Id { get; set; }

        public string TypeCheck { get; set; }

        public ChallengeType Type { get; set; }
        public ChallengeSource Source { get; set; } = ChallengeSource.Original;

        public string Statement { get; set; }

        public long TimeInSeconds { get; set; }
        
        public string Hint { get; set; }
        public string Solution { get; set; }

        public string Photo { get; set; }

        public int Points { get; set; } = 10;
        public int PointsIfRepeat { get; set; } = 10;
        public int PointsWithHint { get; set; } = 5;
        public int PenaltyPoints { get; set; } = 0;

        public bool Completed { get; set; }
    }
}
