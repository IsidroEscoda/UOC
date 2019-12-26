using DistraidaMente.Model;
using DistraidaMente.Models;
using DistraidaMente.ViewModels;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistraidaMente.Helpers
{
    public class FirebaseHelper
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

        public async Task<List<InfoClassModel>> GetInfo()
        {
            return (await firebase
              .Child("PositiveThings/Info")
              .OnceAsync<InfoClassModel>()).Select(item => new InfoClassModel
              {
                  Icon = item.Object.Icon,
                  Name = item.Object.Name,
                  Description = item.Object.Description
              }).ToList();

        }

        public async Task<List<Ranking>> GetRankings()
        {

            return (await firebase
              .Child("PositiveThings/Ranking")
              .OnceAsync<Ranking>()).Select(item => new Ranking
              {
                  Name = item.Object.Name,
                  Position = item.Object.Position,
                  Points = item.Object.Points,
                  DocId = item.Object.DocId
              }).OrderByDescending(w => w.Points).ToList();
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Select(item => new Person
              {
                  DocId = item.Object.DocId,
                  Video = item.Object.Video,
                  Name = item.Object.Name,
                  PersonId = item.Object.PersonId,
                  PruRea = item.Object.PruRea,
                  PisSol = item.Object.PisSol,
                  PruSal = item.Object.PruSal,
                  ResAce = item.Object.ResAce,
                  ResErr = item.Object.ResErr
              }).ToList();
        }

        public async Task AddPerson(int personId, string name)
        {

            await firebase
              .Child("Persons")
              .PostAsync(new Person() { PersonId = personId, Name = name });
        }


        public async Task AddPerson(int personId, string name, string docId, bool sawIt)
        {
            var toUpdatePerson = (await firebase
               .Child("Persons")
               .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            if (toUpdatePerson != null)
            {
                await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Person() { PersonId = personId, Name = name, DocId = docId, Video = sawIt });
            }
            else
            {
                await firebase
              .Child("Persons")
              .PostAsync(new Person() { PersonId = personId, Name = name, DocId = docId, Video = sawIt });
            }

        }


        public async Task<Person> GetPersonDocId(string docId)
        {
            var allPersons = await GetAllPersons();
            await firebase
              .Child("Persons")
              .OnceAsync<Person>();
            return allPersons.Where(a => a.DocId == docId).FirstOrDefault();
            
        }

        public async Task<Person> GetPerson(int personId)
        {
            var allPersons = await GetAllPersons();
            await firebase
              .Child("Persons")
              .OnceAsync<Person>();
            return allPersons.Where(a => a.PersonId == personId).FirstOrDefault();
        }

        public async Task UpdatePerson(int personId, string name)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.PersonId == personId).FirstOrDefault();

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Person() { PersonId = personId, Name = name });
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