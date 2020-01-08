using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace DistraidaMente.Model
{
    public static class PositiveThingsDatabase
    {
        private static SQLiteConnection database;

        public static void Initialize()
        {
            var db = DependencyService.Get<IPositiveThingsDatabase>();

            database = db.GetConnection();

            database.CreateTable<DistractionEventPlainData>();
            database.CreateTable<Challenge>();
            database.CreateTable<ChallengeCompleted>();
        }

        public static List<Challenge> GetCustomChallenges()
        {
            return database.Query<Challenge>("select * from Challenge");
        }

        public static void SaveCustomChallenge(Challenge challenge)
        {
            database.Insert(challenge);
        }

        public static long SaveDistractionEventPlainData(DistractionEventPlainData plainData)
        {
            if (plainData.Id == 0)
            {
                database.Insert(plainData);

                plainData.Id = database.ExecuteScalar<long>(@"select last_insert_rowid()");
            }
            else
            {
                database.Update(plainData);
            }

            return plainData.Id;
        }

        public static void SaveDistractionSessionData(DistractionEventData data)
        {
            var plainData = new DistractionEventPlainData(data);

            data.Id = SaveDistractionEventPlainData(plainData);
        }

        public static void SaveChallengeCompletedData(ChallengeCompleted data)
        {
            database.Insert(data);
        }

        public static IEnumerable<ChallengeCompleted> GetChallengesCompleted()
        {
            return database.Query<ChallengeCompleted>("select * from ChallengeCompleted");
        }

        public static IEnumerable<DistractionEventPlainData> GetDistractionEventPlainDataPendingToSynchronize()
        {
            return database.Query<DistractionEventPlainData>("select * from DistractionEventPlainData where Synchronized <> 1");
        }

        public static IEnumerable<DistractionEventPlainData> GetDistractionEventPlainData()
        {
            return database.Query<DistractionEventPlainData>("select * from DistractionEventPlainData");
        }

        internal static void SaveChallengeCompleted(ChallengeCompleted cc)
        {
            if (!GetChallengesCompleted().Any(x => x.ChallengeId == cc.ChallengeId))
            {
                database.Insert(cc);
            }
        }
    }
}