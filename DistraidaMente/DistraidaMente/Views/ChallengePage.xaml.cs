using DistraidaMente.Controllers;
using DistraidaMente.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DistraidaMente.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChallengePage : ContentPage
    {
        private Timer _timer;
        Configuration _configuration;

        private Challenge _challenge;
        ChallengeResult _challengeResult;
        private long _countDown;

        bool _showMessageIfSkipChallenge;
        bool _showActionButtons;

        public ChallengePage()
        {
            _configuration = new Configuration();
            InitializeComponent();
        }
        public ChallengePage(Configuration configuration, Challenge challenge, int totalPoints, bool showMessageIfSkipChallenge, bool showActionButtons)
        {
            _challengeResult = new ChallengeResult()
            {
                TotalPoints = totalPoints,
                ChallengeType = challenge.Type,
                Repeating = challenge.Completed,
                ChallengeId = challenge.Id,
            };

            _showMessageIfSkipChallenge = showMessageIfSkipChallenge;
            _showActionButtons = showActionButtons;

            this._challenge = challenge;

            _configuration = configuration;

            _countDown = challenge.TimeInSeconds;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetupTimer();
        }

        private void SetupTimer()
        {
            _timer = new Timer(1000);

            // when event fires, update Label
            _timer.Elapsed += (sender, e) =>
            {
                if (_countDown == 0)
                {
                    TimeElapsed();
                    _timer.Stop();
                }
                else
                {
                    _countDown--;
                    UpdateClock();
                };
            };

            // start the timer
            _timer.Start();
        }

        private void UpdateClock()
        {
            Device.BeginInvokeOnMainThread(() => _clockLabel.Text = $"{_countDown / 60:00}:{_countDown % 60:00}");
        }

        private void TimeElapsed()
        {
            //ShowResultSummary(true, false);
        }
    }
}