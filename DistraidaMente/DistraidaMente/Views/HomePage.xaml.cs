using DeltaApps.CommonLibrary.Helpers;
using DeltaApps.CommonLibrary.Model;
using DeltaApps.PositiveApps.Common.Model;
using DeltaApps.PositiveApps.Common.Pages;
using DeltaApps.PositiveThings.Controllers;
using DeltaApps.PositiveThings.Helpers;
using DeltaApps.PositiveThings.Model;
using DeltaApps.PositiveThings.Pages;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using FirebaseHelper = DeltaApps.PositiveThings.Helpers.FirebaseHelper;

namespace DeltaApps.PositiveThings.Views
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
            _configuration = new DeltaApps.PositiveApps.Common.Model.Configuration();


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
                TabsPage tabbed = new TabsPage(_configuration);

                Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });

                SaveEventData(new DistractionEventData()
                {
                    EventType = DistractionEventType.SelectFirstEmotionalStatus,
                    Time = DateTime.UtcNow,
                    UserId = _configuration.UserId,
                    Data = emotionalStatus.ToString(),
                });

                //ShowSelectChallengeTypePage(emotionalStatus);
            };

            //selectEmotionalStatusPage.BackButtonPressed += () => { Xamarin.Forms.Application.Current.MainPage = new MainPage(); };

            selectEmotionalStatusPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = selectEmotionalStatusPage; });

            currentPage = selectEmotionalStatusPage;
        }

        protected async override void OnAppearing()
        {
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

            //var allChallenge = await firebaseHelper.GetAllChallengeFB();
            //lstChallenges.ItemsSource = allChallenge;

            //_challenges = JsonConvert.DeserializeObject<List<Challenge>>(content);

            //_challenges.AddRange(PositiveThingsDatabase.GetCustomChallenges());

            /*var completed = PositiveThingsDatabase.GetChallengesCompleted();

            foreach (Challenge c in _challenges)
            {
                c.Completed = completed.Any(x => x.ChallengeId == c.Id);
            }*/
        }


        async void OpenChallengeCommand(object sender, EventArgs e)
        {
            activity.IsEnabled = true;
            activity.IsRunning = true;
            activity.IsVisible = true;
            //StartProcess();
            //CheckboxesTypes();
            Random random = new Random();
            int num = random.Next(0, 3);

            ShowChallengePage();
            //ShowChallengePage(3);
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

            var challenge = _currentChallenges[_currentChallengeIndex];

            ShowChallengePage(challenge, startChallenge, showMessageIfSkipChallenge);
        }

        private void ShowChallengePage(Challenge challenge, bool startChallenge, bool showMessageIfSkipChallenge, bool showActionButtons = true)
        {
            int points = int.Parse(_configuration.ReadProperty("Points") ?? "0");

            SaveEventData(new DistractionEventData()
            {
                EventType = startChallenge ? DistractionEventType.StartChallenge : DistractionEventType.ChangeChallenge,
                Time = DateTime.UtcNow,
                UserId = _configuration.UserId,
                Data = $"Challenge type: { challenge.Type }, Challenge Id: { challenge.Id }",
            });

            ChallengePage challengePage = new ChallengePage(_configuration, challenge, points, showMessageIfSkipChallenge, showActionButtons);

            //selectChallengeTypePage.BackButtonPressed += () => { ShowSelectFirstEmotionalStatusPage(); };
            challengePage.EnableBackButton = false;
            //selectChallengeTypePage.NextButtonPressed += () => { ShowChallengePage(); };
            challengePage.EnableNextButton = false;

            challengePage.ShowNavigationBarOnEntry = false;
            challengePage.AnimateNavigationBarOnEntry = false;
            challengePage.AnimateNavigationBarOnBack = false;
            challengePage.AnimateNavigationBarOnNext = false;

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

            //var completed = _challenges.Where(x => (x.Type == challengeType) && x.Completed).Randomize();
            //var notcompleted = _challenges.Where(x => (x.Type == challengeType) && !x.Completed).Randomize();
            if (_configuration.ReadBoolProperty("personal"))
            {
                var personal = _challenges.Where(x => (x.TypeCheck == "1")).Randomize();
                chosen.AddRange(personal);
            }

            if (_configuration.ReadBoolProperty("acertijos"))
            {

                var acertijos = _challenges.Where(x => (x.TypeCheck == "2")).Randomize();
                chosen.AddRange(acertijos);
            }

            if (_configuration.ReadBoolProperty("enigmas"))
            {
                var enigmas = _challenges.Where(x => (x.TypeCheck == "3")).Randomize();
                chosen.AddRange(enigmas);
            }

            if (_configuration.ReadBoolProperty("diferencias"))
            {
                var diferencias = _challenges.Where(x => (x.TypeCheck == "4")).Randomize();
                chosen.AddRange(diferencias);
            }

            if (_configuration.ReadBoolProperty("sopas"))
            {

                var sopas = _challenges.Where(x => (x.TypeCheck == "5")).Randomize();
                chosen.AddRange(sopas);
            }

            if (_configuration.ReadBoolProperty("relacionarme"))
            {
                var relacionarme = _challenges.Where(x => (x.TypeCheck == "6")).Randomize();
                chosen.AddRange(relacionarme);
            }

            if (_configuration.ReadBoolProperty("moverme"))
            {
                var moverme = _challenges.Where(x => (x.TypeCheck.Contains("7"))).Randomize();
                chosen.AddRange(moverme);
            }

            if (_configuration.ReadBoolProperty("musica"))
            {
                var musica = _challenges.Where(x => (x.TypeCheck.Contains("8"))).Randomize();
                chosen.AddRange(musica);
            }

            if (_configuration.ReadBoolProperty("relax"))
            {
                var relax = _challenges.Where(x => (x.TypeCheck.Contains("9"))).Randomize();
                chosen.AddRange(relax);
            }

            if (!_configuration.ReadBoolProperty("personal") && !_configuration.ReadBoolProperty("acertijos") &&
                !_configuration.ReadBoolProperty("enigmas") && !_configuration.ReadBoolProperty("diferencias") &&
                !_configuration.ReadBoolProperty("sopas") && !_configuration.ReadBoolProperty("relacionarme") &&
                !_configuration.ReadBoolProperty("moverme") && !_configuration.ReadBoolProperty("musica") &&
                !_configuration.ReadBoolProperty("relax"))
            {
                chosen.AddRange(_challenges.Randomize());
                
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

                ShowSelectFirstEmotionalStatusPage();
            };


            endProcessSummaryPage.ExitPressed += () => {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            };

            endProcessSummaryPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = endProcessSummaryPage; });

            currentPage = null;
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

