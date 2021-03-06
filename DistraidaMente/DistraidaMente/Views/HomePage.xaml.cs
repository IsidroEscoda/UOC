﻿using UOCApps.CommonLibrary.Helpers;
using UOCApps.CommonLibrary.Model;
using UOCApps.Common.Model;
using UOCApps.Common.Pages;
using DistraidaMente.Controllers;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using DistraidaMente.Pages;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using FirebaseHelper = DistraidaMente.Helpers.FirebaseHelper;

namespace DistraidaMente.Views
{
    public partial class HomePage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        BasePage currentPage;
        private Configuration _configuration;

        char emoticonSet;
        string emoticonMessage;
        IEnumerable<EmotionalStatus> emoticonOrder = Enum.GetValues(typeof(EmotionalStatus)).Cast<EmotionalStatus>().ToList();

        List<Challenge> _challenges;
        List<Challenge2> _challengesFB;

        private bool FirstTime = true;

        static object lockObject = new object();
        private List<Challenge> _currentChallenges;
        private int _currentChallengeIndex;

        bool ignoreToogledEvent;

        public HomePage()
        {
            _configuration = new UOCApps.Common.Model.Configuration();


            InitializeComponent();
            try
            {

                //CheckConnectivity();
                var isConnected = CrossConnectivity.Current.IsConnected;

                if (isConnected == true)
                {
                    LoadChallengesAsync();
                }
                else
                {
                    DisplayAlert("No Internet", "Para poder ver el ranking se necesita una conexión a Internet!!", "Ok");
                }
            }
            catch (System.Exception ex)
            {
                DisplayAlert("No Internet", "Para poder ver el ranking se necesita una conexión a Internet!!", "Ok");
            }


            _configuration = new Configuration()
            {
                Theme = new Theme()
                {
                    BackgroundColor = Color.FromRgb(255, 255, 255),
                    //TextColor = Color.FromRgb(64, 64, 65),
                    TextColor = Color.FromRgb(64, 64, 65),
                    //SelectedTextColor = Color.FromRgb(247, 234, 204),
                    SelectedTextColor = Color.FromRgb(255, 255, 255),
                    //SecondaryBackgroundColor = Color.FromRgb(246, 230, 199),
                    SecondaryBackgroundColor = Color.FromRgb(115, 237, 255),
                    //SelectedBackgroundColor = Color.FromRgb(45, 62, 80),
                    SelectedBackgroundColor = Color.FromRgb(0, 0, 120),

                    SmallFontSize = 18,
                    MediumFontSize = 21,
                    BigFontSize = 28,
                    ExtraBigFontSize = 36,
                }
            };

        }

        

        internal void ShowSelectFirstEmotionalStatusPage()
        {
            if (emoticonOrder == null)
            {
                emoticonOrder = Enum.GetValues(typeof(EmotionalStatus)).Cast<EmotionalStatus>().ToList(); // typeof(EmotionalStatus).Randomize<EmotionalStatus>();
                emoticonMessage = "¿Cómo te encuentras hoy?"; //PositiveThingsConfiguration.CurrentConfigurationMessages.InitialEmotionalState.RandomItem();
                emoticonSet = 'z'; // "abcdefghijkl".ToCharArray().RandomItem();
            }
            else
            {
                emoticonMessage = "¿Cómo te encuentras hoy?"; //PositiveThingsConfiguration.CurrentConfigurationMessages.InitialEmotionalState.RandomItem();
                emoticonSet = 'z'; // "abcdefghijkl".ToCharArray().RandomItem();
            }

            SelectEmotionalStatusPage selectEmotionalStatusPage = new SelectEmotionalStatusPage(_configuration, emoticonMessage, emoticonSet, emoticonOrder, true);

            selectEmotionalStatusPage.EmoticonOffset = -50;
            selectEmotionalStatusPage.EmotionalStatusSelected += (emotionalStatus) =>
            {
                ShowChallengePage();
                SaveEventData(new DistractionEventData()
                {
                    EventType = DistractionEventType.SelectFirstEmotionalStatus,
                    Time = DateTime.UtcNow,
                    UserId = _configuration.UserId,
                    Data = emotionalStatus.ToString(),
                });

                //ShowSelectChallengeTypePage(emotionalStatus);
            };
            _configuration.FirstEmoticons = "false";
            //selectEmotionalStatusPage.BackButtonPressed += () => { Xamarin.Forms.Application.Current.MainPage = new MainPage(); };

            selectEmotionalStatusPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = selectEmotionalStatusPage; });

            currentPage = selectEmotionalStatusPage;
        }

        protected async override void OnAppearing()
        {
            _configuration.FirstEmoticons = "true";
            base.OnAppearing();
            // Do your thing
            try
            {
               ignoreToogledEvent = true;

                switch10.IsToggled = _configuration.ReadBoolProperty("moverme");
                switch11.IsToggled = _configuration.ReadBoolProperty("relacionarme");

                ignoreToogledEvent = false;
            }
            catch (Exception ex)
            {
                throw new Exception("OnAppearing Additional information..." + ex, ex);
            }
        }

        void OnToggled(object sender, ToggledEventArgs e)
        {
            if (ignoreToogledEvent) return;

            Switch cell = (sender as Switch);
            var classId = cell.ClassId;

            _configuration.SaveBoolProperty(classId, cell.IsToggled);

            //DisplayAlert("Success", String.Format("Switch is now {0} {1}", classId, e.Value), "OK");
        }

        private async Task LoadChallengesAsync()
        {
            _challenges = await firebaseHelper.GetAllChallengeFB();

            var completed = PositiveThingsDatabase.GetChallengesCompleted();

            foreach (Challenge c in _challenges)
            {
                c.Completed = completed.Any(x => x.ChallengeId == c.Id);
            }
        }


        async void OpenChallengeCommand(object sender, EventArgs e)
        {
            activity.IsVisible = true;
            activity.IsEnabled = true;
            activity.IsRunning = true;

            openChallengeBtn.IsEnabled = false;
            openChallengeBtn.IsVisible = false;

            if(_configuration.FirstEmoticons == "true")
            {
                ShowSelectFirstEmotionalStatusPage();
            }
            else
            {
                ShowChallengePage();
            }

            //await SetCheckBoxResumeAsync();
        }

        private async Task SetCheckBoxResumeAsync()
        {
            if (_configuration.ReadBoolProperty("personal"))
            {
                await firebaseHelper.UpdatePersonCBR("Personal", _configuration.ReadProperty("docId"));
            }
            if (_configuration.ReadBoolProperty("acertijos"))
            {
                await firebaseHelper.UpdatePersonCBR("Adivinanzas", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("enigmas"))
            {
                await firebaseHelper.UpdatePersonCBR("Enigmas", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("diferencias"))
            {
                await firebaseHelper.UpdatePersonCBR("Diferencias", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("sopas"))
            {
                await firebaseHelper.UpdatePersonCBR("Sopas", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("relacionarme"))
            {
                await firebaseHelper.UpdatePersonCBR("Sociales", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("moverme"))
            {
                await firebaseHelper.UpdatePersonCBR("Accion", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("musica"))
            {
                await firebaseHelper.UpdatePersonCBR("Musica", _configuration.ReadProperty("docId"));
            }

            if (_configuration.ReadBoolProperty("relax"))
            {
                await firebaseHelper.UpdatePersonCBR("Relax", _configuration.ReadProperty("docId"));
            }
        }

        private void ShowChallengePage()
        {

            bool startChallenge = _currentChallenges == null;

            if (_currentChallenges == null)
            {
                _currentChallenges = ChooseChallenges();
                _currentChallengeIndex = 0;
            }
            else
            {
                _currentChallengeIndex = (_currentChallengeIndex + 1) % 3;
            }

            bool showMessageIfSkipChallenge = _currentChallengeIndex == 2;

            bool showNoMoreChallenges = _currentChallenges.Count() <= 2;
            bool showNoMoreChallenges2 = _currentChallengeIndex == _currentChallenges.Count();


            if(_currentChallenges.Count() != 0)
            {
                if (_currentChallengeIndex <= _currentChallenges.Count()-1)
                {
                    var challenge = _currentChallenges[_currentChallengeIndex];
                    ShowChallengePage(challenge, false, startChallenge, showMessageIfSkipChallenge);
                }
                else
                {
                    _currentChallengeIndex = 0;
                       var challenge = _currentChallenges[_currentChallengeIndex];
                    ShowChallengePage(challenge, false, startChallenge, showMessageIfSkipChallenge);
                }
            }
            else
            {
                var challenge = new Challenge();
                ShowChallengePage(challenge, true, false, false);
                //DisplayAlert("Alert", "You have been alerted", "OK");
            }

            /*var challenge = _currentChallenges[_currentChallengeIndex];
            ShowChallengePage(challenge, false, startChallenge, showMessageIfSkipChallenge);*/

        }

        private void ShowChallengePage(Challenge challenge, bool noMoreChallenges, bool startChallenge, bool showMessageIfSkipChallenge, bool showActionButtons = true)
        {
            int points = int.Parse(_configuration.ReadProperty("Points") ?? "0");

            SaveEventData(new DistractionEventData()
            {
                EventType = startChallenge ? DistractionEventType.StartChallenge : DistractionEventType.ChangeChallenge,
                Time = DateTime.UtcNow,
                UserId = _configuration.UserId,
                Data = $"Challenge type: { challenge.Type }, Challenge Id: { challenge.Id }",
            });

            ChallengePage challengePage = new ChallengePage(_configuration, challenge, noMoreChallenges, points, showMessageIfSkipChallenge, showActionButtons);

            //selectChallengeTypePage.BackButtonPressed += () => { ShowSelectFirstEmotionalStatusPage(); };
            challengePage.EnableBackButton = false;
            //selectChallengeTypePage.NextButtonPressed += () => { ShowChallengePage(); };
            challengePage.EnableNextButton = false;

            challengePage.ShowNavigationBarOnEntry = false;
            challengePage.AnimateNavigationBarOnEntry = false;
            challengePage.AnimateNavigationBarOnBack = false;
            challengePage.AnimateNavigationBarOnNext = false;

            challengePage.ChallengeNoMore += () => {

                var completedDB = PositiveThingsDatabase.GetChallengesCompleted();
                foreach (Challenge ch in _challenges)
                {
                    if (ch.Completed)
                    {
                        if (_configuration.ReadBoolProperty("personal") && ch.TypeCheck.Contains("1"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("acertijos") && ch.TypeCheck.Contains("2"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("enigmas") && ch.TypeCheck.Contains("3"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("diferencias") && ch.TypeCheck.Contains("4"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("sopas") && ch.TypeCheck.Contains("5"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("relacionarme") && ch.TypeCheck.Contains("6"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("moverme") && ch.TypeCheck.Contains("7"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("musica") && ch.TypeCheck.Contains("8"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }

                        if (_configuration.ReadBoolProperty("relax") && ch.TypeCheck.Contains("9"))
                        {
                            PositiveThingsDatabase.RemoveChallengeCompletedId(ch.Id);
                            ch.Completed = false;
                            continue;
                        }
                    }
                }
                var completedDB2 = PositiveThingsDatabase.GetChallengesCompleted();
                //Task.Run(() => ShowChallengePage());
                //Device.BeginInvokeOnMainThread(() => ShowChallengePage());
                _currentChallenges = null;
                ShowChallengePage();
            };

            challengePage.ChallengeSkipped += () => { Task.Run(() => /*ShowChallengePage(challenge.Type)*/ShowChallengePage()); };

            challengePage.ChallengeEnd += (challengeResult) =>
            {
                _currentChallenges = null;
                _currentChallengeIndex = 0;

                var cc = new ChallengeCompleted() { ChallengeId = challengeResult.ChallengeId };

                PositiveThingsDatabase.SaveChallengeCompleted(cc);

                var ch = _challenges.First(x => x.Id == cc.ChallengeId);

                ch.Completed = true;

                int totaPoints = Math.Max(0, int.Parse(_configuration.ReadProperty("Points") ?? "0") + challengeResult.ChallengePoints);

                _configuration.SaveProperty("Points", totaPoints.ToString());

                SaveEventDataSync(new DistractionEventData()
                {
                    EventType = DistractionEventType.EndChallenge,
                    Time = DateTime.UtcNow,
                    UserId = _configuration.UserId,
                    Data = challengeResult.ToString(),
                });

                return _configuration.ReadProperty("ranking");
            };

            challengePage.ChallengeExit += (continuePlaying) =>
            {
                if (continuePlaying)
                {
                    //ShowSelectChallengeTypePage();
                    ShowChallengePage();
                    /*TabbedPage1 tabbed = new TabbedPage1(_configuration);

                    Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });*/
                }
                else
                {
                    ShowSelectEndEmotionalStatusPage();
                }
            };

            challengePage.Initialize();

            //await currentPage.Navigation.PushModalAsync(selectNegativeTagFirstLevelSummaryPage, false);

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = challengePage; });

            //await Task.Delay(1000);

            //Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Task.Delay(1000); });

            //await Task.Delay(1000);

            currentPage = challengePage;
        }
        internal void ShowSelectEndEmotionalStatusPage() //string positiveThought)
        {
            //_sessionData.PositiveThoughtSelected = positiveThought;
            //_sessionData.PositiveThoughtSelectedDate = DateTime.UtcNow;

            // SaveData(_sessionData);

            SelectEmotionalStatusPage selectEmotionalStatusPage = new SelectEmotionalStatusPage(_configuration, "¿Y cómo te sientes ahora?", emoticonSet, emoticonOrder, true);

            selectEmotionalStatusPage.EmotionalStatusSelected += EndProcess;
            //selectEmotionalStatusPage.BackButtonPressed += () => { ShowSelectFirstEmotionalStatusPage(); };
            selectEmotionalStatusPage.EnableBackButton = false;

            selectEmotionalStatusPage.ShowNavigationBarOnEntry = false;
            selectEmotionalStatusPage.AnimateNavigationBarOnEntry = false;
            selectEmotionalStatusPage.AnimateNavigationBarOnBack = false;
            selectEmotionalStatusPage.AnimateNavigationBarOnNext = false;

            selectEmotionalStatusPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = selectEmotionalStatusPage; });

            currentPage = selectEmotionalStatusPage;
        }

        private List<Challenge> ChooseChallenges()
        {
            List<Challenge> chosen = new List<Challenge>();
            foreach (Challenge ch in _challenges)
            {
                if (!ch.Completed)
                {
                    if (_configuration.ReadBoolProperty("personal") && ch.TypeCheck.Contains("1"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("acertijos") && ch.TypeCheck.Contains("2"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("enigmas") && ch.TypeCheck.Contains("3"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("diferencias") && ch.TypeCheck.Contains("4"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("sopas") && ch.TypeCheck.Contains("5"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("relacionarme") && ch.TypeCheck.Contains("6"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("moverme") && ch.TypeCheck.Contains("7"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("musica") && ch.TypeCheck.Contains("8"))
                    {
                        chosen.Add(ch);
                        continue;
                    }

                    if (_configuration.ReadBoolProperty("relax") && ch.TypeCheck.Contains("9"))
                    {
                        chosen.Add(ch);
                        continue;
                    }
                }
            }
            //var completed = _challenges.Where(x => (x.Type == challengeType) && x.Completed).Randomize();
            //var notcompleted = _challenges.Where(x => (x.Type == challengeType) && !x.Completed).Randomize();
            /*if (_configuration.ReadBoolProperty("personal"))
            {
                var personal = _challenges.Where(x => (x.TypeCheck.Contains("1")) && !x.Completed).Randomize();
                chosen.AddRange(personal);
            }

            if (_configuration.ReadBoolProperty("acertijos"))
            {

                var acertijos = _challenges.Where(x => (x.TypeCheck.Contains("2")) && !x.Completed).Randomize();
                chosen.AddRange(acertijos);
            }

            if (_configuration.ReadBoolProperty("enigmas"))
            {
                var enigmas = _challenges.Where(x => (x.TypeCheck.Contains("3")) && !x.Completed).Randomize();
                chosen.AddRange(enigmas);
            }

            if (_configuration.ReadBoolProperty("diferencias"))
            {
                var diferencias = _challenges.Where(x => (x.TypeCheck.Contains("4")) && !x.Completed).Randomize();
                chosen.AddRange(diferencias);
            }

            if (_configuration.ReadBoolProperty("sopas"))
            {

                var sopas = _challenges.Where(x => (x.TypeCheck.Contains("5")) && !x.Completed).Randomize();
                chosen.AddRange(sopas);
            }

            if (_configuration.ReadBoolProperty("relacionarme"))
            {
                var relacionarme = _challenges.Where(x => (x.TypeCheck.Contains("6")) && !x.Completed).Randomize();
                chosen.AddRange(relacionarme);
            }

            if (_configuration.ReadBoolProperty("moverme"))
            {
                var moverme = _challenges.Where(x => (x.TypeCheck.Contains("7")) && !x.Completed).Randomize();
                chosen.AddRange(moverme);
            }

            if (_configuration.ReadBoolProperty("musica"))
            {
                var musica = _challenges.Where(x => (x.TypeCheck.Contains("8")) && !x.Completed).Randomize();
                chosen.AddRange(musica);
            }

            if (_configuration.ReadBoolProperty("relax"))
            {
                var relax = _challenges.Where(x => (x.TypeCheck.Contains("9")) && !x.Completed).Randomize();
                chosen.AddRange(relax);
            }*/

            if (!_configuration.ReadBoolProperty("personal") && !_configuration.ReadBoolProperty("acertijos") &&
                !_configuration.ReadBoolProperty("enigmas") && !_configuration.ReadBoolProperty("diferencias") &&
                !_configuration.ReadBoolProperty("sopas") && !_configuration.ReadBoolProperty("relacionarme") &&
                !_configuration.ReadBoolProperty("moverme") && !_configuration.ReadBoolProperty("musica") &&
                !_configuration.ReadBoolProperty("relax"))
            {
                chosen.AddRange(_challenges.Where(x => !x.Completed).Randomize());
                
            }

                return chosen;
        }


        internal void EndProcess(EmotionalStatus emotionalStatus)
        {
            SaveEventData(new DistractionEventData()
            {
                EventType = DistractionEventType.SelectEndEmotionalStatus,
                Time = DateTime.UtcNow,
                UserId = _configuration.UserId,
                Data = emotionalStatus.ToString(),
            });

            SaveEventData(new DistractionEventData()
            {
                EventType = DistractionEventType.EndSession,
                Time = DateTime.UtcNow,
                UserId = _configuration.UserId,
            });

            EndProcessSummaryPage endProcessSummaryPage = new EndProcessSummaryPage(_configuration, emotionalStatus, true);

            endProcessSummaryPage.StartAgainPressed += () => {
                //StartProcess();

                //ShowSelectFirstEmotionalStatusPage();

                _configuration.FirstEmoticons = "true";
                ShowSelectChallengeTypePage(null);
            };


            endProcessSummaryPage.ExitPressed += () => {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            };

            endProcessSummaryPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = endProcessSummaryPage; });

            currentPage = null;
        }

        internal void ShowSelectChallengeTypePage(EmotionalStatus? emotionalStatus = null)
        {
            try
            {
                TabPageCS selectChallengeTypePage = new TabPageCS(_configuration);
                Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = selectChallengeTypePage; });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal void SaveEventDataSync(DistractionEventData data)
        {
            PositiveThingsDatabase.SaveDistractionSessionData(data);

            var plainData = new DistractionEventPlainData(data);

            SynchronizeData(plainData);
        }

        internal void SaveEventData(DistractionEventData data)
        {
            PositiveThingsDatabase.SaveDistractionSessionData(data);

            SynchronizeData();
        }

        internal void CancelProcess()
        {

        }

        public async void SynchronizeData()
        {
            await Task.Delay(1);

            lock (lockObject)
            {
                var data = PositiveThingsDatabase.GetDistractionEventPlainDataPendingToSynchronize();

                foreach (DistractionEventPlainData d in data)
                {
                    SynchronizeData(d);
                }
            }
        }

        private void SynchronizeData(DistractionEventPlainData plainData)
        {

            firebaseHelper.AddEventThing(new DistractionEventData()
            {
                EventType = DistractionEventType.StartSession,
                Time = plainData.Time,
                UserId = plainData.UserId,
            });

            bool sent = RESTHelper.Post(_configuration.ServerUrl + "PositiveThingsData", plainData, _configuration.ServerUser, _configuration.ServerPassword);

            if (sent)
            {
                plainData.Synchronized = true;

                PositiveThingsDatabase.SaveDistractionEventPlainData(plainData);
            }
        }

    }
}

