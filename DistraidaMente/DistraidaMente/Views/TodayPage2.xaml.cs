using UOCApps.CommonLibrary.Helpers;
using UOCApps.CommonLibrary.Model;
using DistraidaMente.Common.Model;
using DistraidaMente.Common.Pages;
using DistraidaMente.Controllers;
using DistraidaMente.Model;
using DistraidaMente.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DistraidaMente.Views
{
	public partial class TodayPage2 : ContentPage
	{
        BasePage currentPage;
        private Configuration _configuration;

        char emoticonSet;
        string emoticonMessage;
        IEnumerable<EmotionalStatus> emoticonOrder = Enum.GetValues(typeof(EmotionalStatus)).Cast<EmotionalStatus>().ToList();

        List<Challenge> _challenges;

        private bool FirstTime = true;

        static object lockObject = new object();
        private List<Challenge> _currentChallenges;
        private int _currentChallengeIndex;

        bool ignoreToogledEvent;

        public TodayPage2 ()
		{
            _configuration = new DistraidaMente.Common.Model.Configuration();


            //InitializeComponent();

            ignoreToogledEvent = true;

            //switch10.IsToggled = _configuration.ReadBoolProperty("moverme");
            //switch11.IsToggled = _configuration.ReadBoolProperty("relacionarme");

            ignoreToogledEvent = false;

            LoadChallenges();

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

        void OnToggled(object sender, ToggledEventArgs e)
        {
            if (ignoreToogledEvent) return;

            Switch cell = (sender as Switch);
            var classId = cell.ClassId;

            _configuration.SaveBoolProperty(classId, cell.IsToggled);

            DisplayAlert("Success", String.Format("Switch is now {0} {1}", classId, e.Value), "OK");
        }


        public void StartProcess()
        {
            if (_configuration.FirstLaunch == null)
            {
                _configuration.FirstLaunch = "true";
            }

            PrepareSessionData();
        }

        internal void PrepareSessionData()
        {
            if (_configuration.UserId == null)
            {
                _configuration.SaveProperty("ranking", "1;0");

                var readInvitationCodePage = new ReadInvitationCodePage(_configuration);

                readInvitationCodePage.Initialize();

                readInvitationCodePage.ReadCode += (code) =>
                {
                    code = code?.ToUpper() ?? string.Empty;

                    if (code.Length >= 7)
                    {
                        bool wrongCodes = false;

                        /*if (code.StartsWith("T1G1S"))
                        {
                            int subjectId;

                            if (Int32.TryParse(code.Substring(5), out subjectId))
                            {
                                wrongCodes = subjectId >= 46 && subjectId <= 62;
                            }
                        }*/

                        if (code.StartsWith("DOC"))
                        //if (code.StartsWith("T1G2S") || code.StartsWith("T1G3S") || wrongCodes)
                        {
                            _configuration.UserId = code;

                            Task.Run(() => PrepareSessionData());

                            return true;
                        }
                    }

                    return false;
                };

                Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = readInvitationCodePage; });
            }
            else
            {
                SaveEventData(new DistractionEventData()
                {
                    EventType = DistractionEventType.StartSession,
                    Time = DateTime.UtcNow,
                    UserId = _configuration.UserId,
                });

                if (FirstTime)
                {
                    FirstTime = false;

                    //ShowTutorialVideo();
                }
                else
                {
                    ShowSelectFirstEmotionalStatusPage();
                }

            }
        }



        internal void ShowSelectFirstEmotionalStatusPage()
        {
            if (emoticonOrder == null)
            {
                emoticonOrder = Enum.GetValues(typeof(EmotionalStatus)).Cast<EmotionalStatus>().ToList(); // typeof(EmotionalStatus).Randomize<EmotionalStatus>();
                emoticonMessage = "¿Cómo te encuentras hoy?"; //PositiveThingsConfiguration.CurrentConfigurationMessages.InitialEmotionalState.RandomItem();
                emoticonSet = 'z'; // "abcdefghijkl".ToCharArray().RandomItem();
            }

            SelectEmotionalStatusPage selectEmotionalStatusPage = new SelectEmotionalStatusPage(_configuration, emoticonMessage, emoticonSet, emoticonOrder, true);

            selectEmotionalStatusPage.EmoticonOffset = -50;
            selectEmotionalStatusPage.EmotionalStatusSelected += (emotionalStatus) =>
            {
                SaveEventData(new DistractionEventData()
                {
                    EventType = DistractionEventType.SelectFirstEmotionalStatus,
                    Time = DateTime.UtcNow,
                    UserId = _configuration.UserId,
                    Data = emotionalStatus.ToString(),
                });

                //ShowSelectChallengeTypePage(emotionalStatus);

                TabsPage tabbed = new TabsPage(_configuration);

                Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });

            };

            //selectEmotionalStatusPage.BackButtonPressed += () => { Xamarin.Forms.Application.Current.MainPage = new MainPage(); };

            selectEmotionalStatusPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = selectEmotionalStatusPage; });

            currentPage = selectEmotionalStatusPage;
        }


        private void LoadChallenges()
        {
            //string content = FilesHelper.ReadEmbeddedFileAsString("DistraidaMente.Text.challenges_" + _configuration.Language + ".json");
            string content = FilesHelper.ReadEmbeddedFileAsString("DistraidaMente.Text.challenges2_ES.json");
            _challenges = JsonConvert.DeserializeObject<List<Challenge>>(content);

            //_challenges.AddRange(PositiveThingsDatabase.GetCustomChallenges());

            var completed = PositiveThingsDatabase.GetChallengesCompleted();

            foreach (Challenge c in _challenges)
            {
                c.Completed = completed.Any(x => x.ChallengeId == c.Id);
            }
        }

        async void OpenChallengeCommand(object sender, EventArgs e)
        {
            //StartProcess();
            ShowChallengePage(0);
        }
        private void ShowCustomChallengePage()
        {
            /*SaveEventData(new DistractionEventData()
            {
                EventType = DistractionEventType.StartChallenge,
                Time = DateTime.UtcNow,
                UserId = _configuration.UserId,
                Data = ChallengeType.CreateCustomChallenge.ToString(),
            });*/

            CustomChallengePage customChallengePage = new CustomChallengePage(_configuration);

            //selectChallengeTypePage.BackButtonPressed += () => { ShowSelectFirstEmotionalStatusPage(); };
            customChallengePage.EnableBackButton = false;
            //selectChallengeTypePage.NextButtonPressed += () => { ShowChallengePage(); };
            customChallengePage.EnableNextButton = false;

            customChallengePage.ShowNavigationBarOnEntry = false;
            customChallengePage.AnimateNavigationBarOnEntry = false;
            customChallengePage.AnimateNavigationBarOnBack = false;
            customChallengePage.AnimateNavigationBarOnNext = false;

            customChallengePage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = customChallengePage; });

            currentPage = customChallengePage;
        }


        private void ShowChallengePage(ChallengeType challengeType)
        {
            bool startChallenge = _currentChallenges == null;

            if (_currentChallenges == null)
            {
                _currentChallenges = ChooseChallenges(challengeType);
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

            /*SaveEventData(new DistractionEventData()
            {
                EventType = startChallenge ? DistractionEventType.StartChallenge : DistractionEventType.ChangeChallenge,
                Time = DateTime.UtcNow,
                UserId = _configuration.UserId,
                Data = $"Challenge type: { challenge.Type }, Challenge Id: { challenge.Id }",
            });*/

            ChallengePage challengePage = new ChallengePage(_configuration, challenge, points, showMessageIfSkipChallenge, showActionButtons);

            //selectChallengeTypePage.BackButtonPressed += () => { ShowSelectFirstEmotionalStatusPage(); };
            challengePage.EnableBackButton = false;
            //selectChallengeTypePage.NextButtonPressed += () => { ShowChallengePage(); };
            challengePage.EnableNextButton = false;

            challengePage.ShowNavigationBarOnEntry = false;
            challengePage.AnimateNavigationBarOnEntry = false;
            challengePage.AnimateNavigationBarOnBack = false;
            challengePage.AnimateNavigationBarOnNext = false;

            challengePage.ChallengeSkipped += () => { Task.Run(() => ShowChallengePage(challenge.Type)); };

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

                /*SaveEventDataSync(new DistractionEventData()
                {
                    EventType = DistractionEventType.EndChallenge,
                    Time = DateTime.UtcNow,
                    UserId = _configuration.UserId,
                    Data = challengeResult.ToString(),
                });*/

                string ranking = RESTHelper.Get(_configuration.ServerUrl + "PositiveThingsData?userId=" + _configuration.UserId, _configuration.ServerUser, _configuration.ServerPassword);

                if (ranking != null && ranking.Length > 2)
                {
                    ranking = ranking.Substring(1, ranking.Length - 2);

                    _configuration.SaveProperty("ranking", ranking);
                }

                return _configuration.ReadProperty("ranking");
            };

            challengePage.ChallengeExit += (continuePlaying) =>
            {
                if (continuePlaying)
                {
                    //ShowSelectChallengeTypePage();
                    TabsPage tabbed = new TabsPage(_configuration);

                    Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });
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

        private List<Challenge> ChooseChallenges(ChallengeType challengeType)
        {
            List<Challenge> chosen = new List<Challenge>();

            var completed = _challenges.Where(x => (x.Type == challengeType || x.Type == ChallengeType.CreateCustomChallenge) && x.Completed).Randomize();
            var notcompleted = _challenges.Where(x => (x.Type == challengeType || x.Type == ChallengeType.CreateCustomChallenge) && !x.Completed).Randomize();

            chosen.AddRange(notcompleted.Take(9));

            if (chosen.Count < 9)
            {
                chosen.AddRange(completed.Take(9 - chosen.Count));
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
                StartProcess(); 
            };

            endProcessSummaryPage.Initialize();

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = endProcessSummaryPage; });

            currentPage = null;
        }

        internal void SaveEventDataSync(DistractionEventData data)
        {
            PositiveThingsDatabase.SaveDistractionSessionData(data);

            var plainData = new DistractionEventPlainData(data);

            //SynchronizeData(plainData);
        }

        internal void SaveEventData(DistractionEventData data)
        {
            PositiveThingsDatabase.SaveDistractionSessionData(data);

            //SynchronizeData();
        }

        internal void CancelProcess()
        {

        }

    }
}

