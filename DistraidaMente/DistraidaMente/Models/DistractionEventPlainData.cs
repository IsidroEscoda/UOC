using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistraidaMente.Model
{
    public class DistractionEventPlainData
    {
        [AutoIncrement]
        [PrimaryKey]
        public long Id { get; set; }

        public string UserId { get; set; }

        public DateTime Time { get; set; }

        public string EventType { get; set; }

        public string Data { get; set; }

        public bool Synchronized { get; set; }

        public DistractionEventPlainData()
        {
        }

        public DistractionEventPlainData(DistractionEventData data)
        {
            Id = data.Id;

            UserId = data.UserId.ToString();

            Time = data.Time;

            EventType = data.EventType.ToString();

            Data = data.Data;

            Synchronized = data.Synchronized;
        }
    }
}
