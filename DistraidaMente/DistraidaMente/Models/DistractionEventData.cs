using DistraidaMente.Common.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistraidaMente.Model
{
    public enum DistractionEventType
    {
        StartSession,
        SelectFirstEmotionalStatus,
        StartChallenge,
        ChangeChallenge,
        EndChallenge,
        SelectEndEmotionalStatus,
        EndSession,
    }

    public class DistractionEventData
    {
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        public string UserId { get; set; }

        public DateTime Time { get; set; }

        public DistractionEventType EventType { get; set; }

        public string Data { get; set; }
        
        public bool Synchronized { get; set; }
    }
}
