using DistraidaMente.Model;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistraidaMente.Helpers
{
    public class FirebaseHelper2
    {
        FirebaseClient firebase = new FirebaseClient("https://positiveapps-efbc0.firebaseio.com/");

        public async Task<List<Challenge>> GetAllChallenges()
        {
            return (await firebase
              .Child("challenges")
              .OnceAsync<Challenge>()).Select(item => new Challenge
              {
                  Id = item.Object.Id,
                  TimeInSeconds = item.Object.TimeInSeconds,
                  Statement = item.Object.Statement,
                  Solution = item.Object.Solution,
                  Hint = item.Object.Hint,
                  PointsWithHint = item.Object.PointsWithHint,
                  Type = item.Object.Type
              }).ToList();

        }

        public async Task<List<Person>> GetAllPersons()
        {

            return (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Select(item => new Person
              {
                  Name = item.Object.Name,
                  PersonId = item.Object.PersonId,
                  Video = item.Object.Video,
                  DocId = item.Object.DocId
              }).ToList();
        }

        public async Task AddVideoSkip(string docId)
        {
            var toUpdatePerson = (await firebase
                 .Child("Persons")
                 .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("Video")
              .PutAsync(true);
        }

        public async Task<Person> GetPersonVideo(string docId)
        {   var allPersons = await GetAllPersons();
            await firebase
              .Child("Persons")
              .OnceAsync<Person>();
            return allPersons.Where(a => a.DocId == docId && a.Video == true).FirstOrDefault();
        }

        public async Task AddPerson2(int personId, string name, string docId)
        {

            await firebase
              .Child("Persons")
              .PostAsync(new Person() { PersonId = personId, Name = name, DocId = docId });
        }
        public async Task<List<Ranking>> GetRankings()
        {

            return (await firebase
              .Child("DistraidaMente/Ranking")
              .OnceAsync<Ranking>()).Select(item => new Ranking
              {
                  Name = item.Object.Name,
                  Position = item.Object.Position,
                  Points = item.Object.Points,
                  DocId = item.Object.DocId
              }).OrderByDescending(w => w.Points).ToList();
        }

        public async Task AddRankingThing(int position, int points, string docId, string name)
        {
            var toUpdatePerson = (await firebase
              .Child("DistraidaMente/Ranking")
              .OnceAsync<Ranking>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            if(toUpdatePerson != null)
            {
                await firebase
              .Child("DistraidaMente/Ranking")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Ranking() { Points = points, Position = position, Name = name, DocId = docId });
            }
            else
            {
                await firebase
                  .Child("DistraidaMente/Ranking")
                  .PostAsync(new Ranking() { Points = points, Position = position, Name = name, DocId = docId });
            }
        }

        public async Task AddPerson(int personId, string name, string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            if (toUpdatePerson != null)
            {
                await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Person() { PersonId = personId, Name = name, DocId = docId });
            }
            else
            {
                await firebase
              .Child("Persons")
              .PostAsync(new Person() { PersonId = personId, Name = name, DocId = docId });
            }
        }

        public async Task<Person> GetPerson(int personId)
        {
            var allPersons = await GetAllPersons();
            await firebase
              .Child("Persons")
              .OnceAsync<Person>();
            return allPersons.Where(a => a.PersonId == personId).FirstOrDefault();
        }

        public async Task UpdatePerson(int personId, string name, string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.PersonId == personId).FirstOrDefault();

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Person() { PersonId = personId, Name = name, DocId = docId });
        }

        public async Task DeletePerson(int personId)
        {
            var toDeletePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.PersonId == personId).FirstOrDefault();
            await firebase.Child("Persons").Child(toDeletePerson.Key).DeleteAsync();

        }

        public class Ranking
        {
            public string Name { get; set; }
            public string DocId { get; set; }
            public int Position { get; set; }
            public int Points { get; set; }
        }
    }
}

