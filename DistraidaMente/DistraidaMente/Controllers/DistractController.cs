using DistraidaMente.Helpers;
using DistraidaMente.Model;
using DistraidaMente.Models;
using DistraidaMente.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DistraidaMente.Controllers
{
    public class DistractController
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        private Configuration _configuration;
        List<Challenge> _challenges;
        bool FirstTime = true;

        static object lockObject = new object();
        private List<Challenge> _currentChallenges;
        private int _currentChallengeIndex;

        public  DistractController()
        {
            _configuration = new Configuration();
            LoadChallengesAsync();
        }

        private async Task LoadChallengesAsync()
        {
            var response = await firebaseHelper.GetAllChallenges();
            foreach (Challenge item in response)
            {
                try
                {
                    _challenges.Add(item); // item.value is a Java.Lang.Object
                }
                catch (Exception ex)
                {
                    //"EXCEPTION WITH DICTIONARY MAP"
                }
            }
        }

        public void StartProcess()
        {
            /*if (_configuration.FirstLaunch)
            {
                _configuration.FirstLaunch = false;
            }*/

            PrepareSessionData();
        }

        internal void PrepareSessionData()
        {
            if (_configuration.UserId == null)
            {
                LoginPage loginPage = new LoginPage();
                loginPage.ReadCode += (code) =>
                {
                    code = code?.ToUpper() ?? string.Empty;
                    if (code.Length >= 8)
                    {
                        if (code.StartsWith("DOC"))
                        {
                            _configuration.UserId = code;

                            Task.Run(() => PrepareSessionData());

                            return true;
                        }
                    }

                    return false;
                };
                Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = loginPage; });
            }
            else
            {
                    var person = firebaseHelper.GetPersonDocId(_configuration.UserId);
             
                   if (_configuration.VideoSaw == null)
                    {
                        _configuration.VideoSaw = "false";
                    
                        ShowVideo();
                    }
                    else
                    {
                        ShowFirstEmoticonsPage();
                    }
            }
        }

        private void ShowVideo()
        {
            VideoPage videoPage = new VideoPage();
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = videoPage; });
            //Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = new VideoPage("tutorial.mp4", VideoType.Resource, () => { ShowFirstEmoticonsPage(); }, Assembly.GetExecutingAssembly(), true, true); });
        }

        internal void ShowFirstEmoticonsPage()
        {
            MainPage emoticonsPage = new MainPage();
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = emoticonsPage; });
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

            ChallengePage challengePage = new ChallengePage(_configuration, challenge, points, showMessageIfSkipChallenge, showActionButtons);

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = challengePage; });

        }

        private List<Challenge> ChooseChallenges(ChallengeType challengeType)
        {
            List<Challenge> chosen = new List<Challenge>();

            /*var completed = _challenges.Where(x => (x.Type == challengeType || x.Type == ChallengeType.CreateCustomChallenge) && x.Completed).Randomize();
            var notcompleted = _challenges.Where(x => (x.Type == challengeType || x.Type == ChallengeType.CreateCustomChallenge) && !x.Completed).Randomize();

            chosen.AddRange(notcompleted.Take(3));

            if (chosen.Count < 3)
            {
                chosen.AddRange(completed.Take(3 - chosen.Count));
            }*/

            return chosen;
        }

    }
}
