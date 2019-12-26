using System;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Models
{
    public class ChallengeResult
    {
        public ChallengeType ChallengeType { get; set; }

        public int ChallengePoints { get; set; }

        public int TotalPoints { get; set; }

        public long ElapsedTime { get; set; }

        public bool TimeOver { get; set; }

        public bool ChallengeSuccess { get; set; }

        public bool HintUsed { get; set; }

        public bool Repeating { get; set; }
        public string ChallengeId { get; internal set; }

        public override string ToString()
        {
            return $"id: { ChallengeId }, type: {ChallengeType}, points { ChallengePoints }, totalpoints: { TotalPoints }, time: { ElapsedTime }, timeover? : { TimeOver }, success?: {  ChallengeSuccess }, hint used?: { HintUsed }, repeating?: { Repeating }";
        }
    }
}
