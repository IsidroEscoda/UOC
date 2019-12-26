using DistraidaMente.Controllers;
using DistraidaMente.Helpers;
using DistraidaMente.Models;
using DistraidaMente.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DistraidaMente.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private Configuration _configuration;

        List<Challenge> _challenges;

        bool ignoreToogledEvent;
        FirebaseHelper firebaseHelper = new FirebaseHelper();

        private List<Challenge> _currentChallenges;
        private int _currentChallengeIndex;

        public HomePage()
        {
            _configuration = new Configuration();

            InitializeComponent();
            //LoadChallengesAsync();
        }

        private async Task LoadChallengesAsync()
        {
            var response = await firebaseHelper.GetAllChallenges();
            foreach (Challenge item in response)
            {
                try
                {
                   var challenge = new Challenge();
                    challenge.Id = item.Id;
                    challenge.TimeInSeconds = item.TimeInSeconds;
                    challenge.Statement = item.Statement;
                    challenge.Solution = item.Solution;
                    challenge.Hint = item.Hint;
                    challenge.PointsWithHint = item.PointsWithHint;
                    challenge.Type = item.Type;
                    _currentChallenges.Add(challenge);
                    /*_challenges.Add(new Challenge2() {
                        Statement = item.Statement
                    });*/
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Alert", ex.ToString(), "OK");
                    //"EXCEPTION WITH DICTIONARY MAP"
                }
            }
        }

        void OnToggled(object sender, ToggledEventArgs e)
        {
            if (ignoreToogledEvent) return;

            Switch cell = (sender as Switch);
            var classId = cell.ClassId;

            _configuration.SaveBoolProperty(classId, cell.IsToggled);

        }
        async void OpenChallengeCommand(object sender, EventArgs e)
        {
            //ShowChallengePage(0);
            await Navigation.PushAsync(new ChallengePage2());
            /*ChallengePage2 challengePage = new ChallengePage2();
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = challengePage; });*/
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

            //ChallengePage challengePage = new ChallengePage(_configuration, challenge, points, showMessageIfSkipChallenge, showActionButtons);

            //Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = challengePage; });

        }

        private List<Challenge> ChooseChallenges(ChallengeType challengeType)
        {
            List<Challenge> chosen = new List<Challenge>();

            var all = _challenges.ToList();
            //var completed = _challenges.Where(x => (x.Type == challengeType || x.Type == ChallengeType.CreateCustomChallenge) && x.Completed);
            //var notcompleted = _challenges.Where(x => (x.Type == challengeType || x.Type == ChallengeType.CreateCustomChallenge) && !x.Completed).ToList();

            //chosen.AddRange(notcompleted.Take(3));

            if (chosen.Count < 3)
            {
                //chosen.AddRange(completed.Take(3 - chosen.Count));
            }

            return chosen;
        }


        protected async override void OnAppearing()
        {

            base.OnAppearing();

            try
            {
                base.OnAppearing();
               
                var allChallenges = await firebaseHelper.GetAllChallenges();
                lstChallenges.ItemsSource = allChallenges;


                ignoreToogledEvent = true;

                switch10.IsToggled = _configuration.ReadBoolProperty("moverme");
                switch11.IsToggled = _configuration.ReadBoolProperty("relacionarme");

                ignoreToogledEvent = false;

                lstChallenges.ItemSelected += async (sender, e) => {

                    if (e.SelectedItem == null) return; // don't do anything if we just de-selected the row

                    await DisplayAlert("Tapped", (e.SelectedItem as Challenge).Statement, "OK");

                    ((ListView)sender).SelectedItem = null; // de-select the row
                };
            }

            catch (Exception ex)
            {
                throw new Exception("OnAppearing  Additional information..." + ex, ex);
            }
        }
    }
}