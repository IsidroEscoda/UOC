using DistraidaMente.Model;
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
        //FirebaseClient firebaseC = new FirebaseClient("https://uocapps.firebaseio.com/");
        private int count;

        public async Task<List<InfoClassModel>> GetInfo()
        {

            return (await firebase
              .Child("DistraidaMente/Info")
              .OnceAsync<InfoClassModel>()).Select(item => new InfoClassModel
              {
                  Icon = item.Object.Icon,
                  Name = item.Object.Name,
                  Description = item.Object.Description
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
                  DocId = item.Object.DocId,
                  PruRea = item.Object.PruRea,
                  PisSol = item.Object.PisSol,
                  PruSal = item.Object.PruSal,
                  ResAce = item.Object.ResAce,
                  ResErr = item.Object.ResErr,
                  //CheckBoxes = item.Object.CheckBoxes
                  //CBR = item.Object.CBR
                  Adivinanzas = item.Object.Adivinanzas,
                  Sopas = item.Object.Sopas,
                  Personales = item.Object.Personales,
                  Enigmas = item.Object.Enigmas,
                  Diferencias = item.Object.Diferencias,
                  Sociales = item.Object.Sociales,
                  Musica = item.Object.Musica,
                  Relax = item.Object.Relax,
                  Accion = item.Object.Accion,
              }).ToList();
        }

        public async Task<List<Challenge>> GetAllChallenges()
        {
            //return JsonConvert.DeserializeObject<List<DomainInfo>>(await response.Content.ReadAsStringAsync());
            return (await firebase
              .Child("DistraidaMente/Challenges")
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


        public async Task<List<Challenge>> GetAllChallengeFB()
        {
            //return JsonConvert.DeserializeObject<List<DomainInfo>>(await response.Content.ReadAsStringAsync());
            var response = (await firebase
              .Child("DistraidaMente/Challenges")
              .OnceAsync<Challenge>()).Select(item => new Challenge
              {
                  Id = item.Object.Id,
                  TimeInSeconds = item.Object.TimeInSeconds,
                  Statement = item.Object.Statement,
                  Solution = item.Object.Solution,
                  Photo = item.Object.Photo,
                  TypeCheck = item.Object.TypeCheck,
                  Hint = item.Object.Hint
              }).ToList();
            return response;

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

        public async Task<List<CheckBoxResume>> GetResume(string docId)
        {
            return (await firebase
              .Child("DistraidaMente")
              .Child("CheckBoxes")
              .Child(docId)
              .OnceAsync<CheckBoxResume>()).Select(item => new CheckBoxResume
              {
                  /*Adivinanzas = item.Object.Adivinanzas,
                  Sopas = item.Object.Sopas,
                  Personales = item.Object.Personales,
                  Diferencias = item.Object.Diferencias,
                  Enigmas = item.Object.Enigmas,
                  Sociales = item.Object.Sociales,
                  Musica = item.Object.Musica,
                  Relax = item.Object.Relax,
                  Accion = item.Object.Accion*/
              }).ToList();
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

        public async Task<List<CheckBoxResume>> GetPersonResume2(string docId)
        {

            return (await firebase
               .Child("DistraidaMente")
               .Child("CheckBoxes")
               .Child(docId)
               .OnceAsync<CheckBoxResume>()).Select(item => new CheckBoxResume
               {
                   //TypeName = item.Object.TypeName,
                   //Value = item.Object.Value
               }).ToList();

        }

        public async Task<CheckBoxResume> GetPersonResume(string docId)
        {
            var toUpdatePerson = (await firebase
                 .Child("Persons")
                 .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

           return (DistraidaMente.Model.CheckBoxResume)await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("CBR")
              .OnceAsync<CheckBoxResume>();
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
        public async Task UpdatePersonCBR(string typeName, string docId)
        {
            int newvalue = 0;
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            switch (typeName)
            {
                case "Sopas":
                    count = toUpdatePerson.Object.Sopas + +1;
                    break;
                case "Adivinanzas":
                    count = toUpdatePerson.Object.Adivinanzas + +1;
                    break;
                case "Personales":
                    count = toUpdatePerson.Object.Personales + +1;
                    break;
                case "Enigmas":
                    count = toUpdatePerson.Object.Enigmas + +1;
                    break;
                case "Diferencias":
                    count = toUpdatePerson.Object.Diferencias + +1;
                    break;
                case "Sociales":
                    count = toUpdatePerson.Object.Sociales + +1;
                    break;
                case "Accion":
                    count = toUpdatePerson.Object.Accion + +1;
                    break;
                case "Musica":
                    count = toUpdatePerson.Object.Musica + +1;
                    break;
                case "Relax":
                    count = toUpdatePerson.Object.Relax + +1;
                    break;
            }
            

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child(typeName)
              .PutAsync(count);
            /*var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();*/

            /*await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("CBR")
              .PostAsync(new CheckBoxResume() {TypeName = typeName, Value = 1 });*/

            /*var idTypeName = (await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("CBR")
              .OnceAsync<CheckBoxResume>()).Where(x => x.Object.TypeName == typeName).FirstOrDefault();

            int newvalue = toUpdatePerson.Object.CBR[0].Value + +1;*/
        }

        public async Task UpdatePersonPruRea(string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            int newvalue = toUpdatePerson.Object.PruRea+ + 1;

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("PruRea")
              .PutAsync(newvalue);
        }

        public async Task UpdatePersonPisSol(int value, string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            int newvalue = toUpdatePerson.Object.PisSol + 1;

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("PisSol")
              .PutAsync(newvalue);
        }

        public async Task UpdatePersonPruSal(int value, string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            int newvalue = toUpdatePerson.Object.PruSal + 1;

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("PruSal")
              .PutAsync(newvalue);
        }

        public async Task UpdatePersonResAce(string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            int newvalue = toUpdatePerson.Object.ResAce + 1;

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("ResAce")
              .PutAsync(newvalue);
        }
        public async Task UpdatePersonResErr(string docId)
        {
            var toUpdatePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.DocId == docId).FirstOrDefault();

            int newvalue = toUpdatePerson.Object.ResErr + 1;

            await firebase
              .Child("Persons")
              .Child(toUpdatePerson.Key)
              .Child("ResErr")
              .PutAsync(newvalue);
        }

        public async Task DeletePerson(int personId)
        {
            var toDeletePerson = (await firebase
              .Child("Persons")
              .OnceAsync<Person>()).Where(a => a.Object.PersonId == personId).FirstOrDefault();
            await firebase.Child("Persons").Child(toDeletePerson.Key).DeleteAsync();

        }

        public async Task AddEventThing(DistractionEventData data)
        {
            await firebase
              .Child("DistraidaMente/Events")
              .PostAsync(new DistractionEventData()
              {
                  UserId = data.UserId.ToString(),
                  Time = data.Time,
                  Data = data.Data,
                  Synchronized = data.Synchronized,
                  EventType = data.EventType,
              });
        }
    }
}

