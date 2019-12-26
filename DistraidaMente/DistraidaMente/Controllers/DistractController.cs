using DistraidaMente.Helpers;
using DistraidaMente.Views;
using DistraidaMente.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DistraidaMente.Common.Pages;
using System.Collections.ObjectModel;
using DistraidaMente.Common.Helpers;
using DistraidaMente.Models;
using VideoPage = DistraidaMente.Views.VideoPage;

namespace DistraidaMente.Controllers
{
    public class DistractController
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        private Configuration _configuration;
        bool FirstTime = true;

        private ObservableCollection<Challenge> getChallenge;
        static object lockObject = new object();
        private List<Challenge> _currentChallenges;
        private int _currentChallengeIndex;

        List<Challenge> _challenges;

        public static ObservableCollection<Challenge> Challenges { private set; get; }
        public DistractController()
        {
            _configuration = new Configuration();
            getChallenge = Challenges;
            LoadChallenges();
            LoadChallengesAsync();

        }

        private void LoadChallenges()
        {
            //string content = FilesHelper.ReadEmbeddedFileAsString("DeltaApps.PositiveThings.Text.challenges_" + _configuration.Language + ".json");
            string content = FilesHelper.ReadEmbeddedFileAsString("DistraidaMente.Text.challenges2_ES.json");
            _challenges = JsonConvert.DeserializeObject<List<Challenge>>(content);
        }
        private async Task LoadChallengesAsync()
        {
            var response = await firebaseHelper.GetAllChallenges();
            foreach (Challenge item in response)
            {
                try
                {
                    Challenge c = new Challenge()
                    {
                        Type = ChallengeType.CreateCustomChallenge,
                        Id = Guid.NewGuid().ToString(),
                        Statement = item.Statement,
                        TimeInSeconds = 300,
                        Source = ChallengeSource.Custom,
                    };
                    _challenges.Add(c);
                    /*_challenges.Add(new Challenge()
                    {
                        Statement = item.Name
                    });*/
                }
                catch (Exception ex)
                {
                    //"EXCEPTION WITH DICTIONARY MAP"
                }
            }
        }

        public void StartProcess()
        {
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

                //GetUserVideoAsync();


                //var response = await firebaseHelper.GetPersonDocId(docId);
                if (_configuration.VideoSaw == null)
                {
                    ShowVideo();
                    _configuration.VideoSaw = "true";
                }
                else
                {
                    ShowFirstEmoticonsPage();
                }
            }
        }

        private async Task GetUserVideoAsync()
        {
            var itemsonline = firebaseHelper.GetPersonDocId("DOC002P2");

            var retornodoservidor = await itemsonline;
            if (retornodoservidor.Video)
            {
                ShowFirstEmoticonsPage();
            }
            else
            {
                ShowVideo();
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
    }
}