using DistraidaMente.Controllers;
using DistraidaMente.Views;
using DistraidaMente.Controls;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using DistraidaMente.ViewModels;
using System.Timers;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DistraidaMente.Models;

namespace DistraidaMente.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChallengePage2 : ContentPage
    {
        int counter = 30;
        int mins = 0;
        int isTimerCancel = 0;

        private long _countDown;

        private Timer _timer;

        ChallengeViewModel viewModel;

        public ChallengePage2()
        {
            InitializeComponent();

            var item = new Challenge { Statement = "¿Cuántas personas trabajan en tu departamento o empresa?", TimeInSeconds = 60};
            viewModel = new ChallengeViewModel(item);
            BindingContext = viewModel;
            StartTimer(mins, counter);
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

        public void StartTimer(int m, int sec)
        {
            mins = m;
            counter = sec;
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                if (isTimerCancel == 1)
                {
                    return false;
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        counter = counter - 1;
                        if (counter < 0)
                        {
                            counter = 59;
                            mins = mins - 1;
                            if (mins < 0)
                            {
                                mins = 59;
                            }
                        }

                        _clockLabel.Text = string.Format("{0:00}:{1:00}", mins, counter);
                    });
                    if (mins == 0 && counter == 0)
                    {
                        SetupResultSummaryDialog();
                        //Code to call next form
                        /*Navigation.PushAsync(new HomePage());
                        DisplayAlert("Exam", "Exam Time Over", "Close");*/
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            });

        }

        private Dialog SetupResultSummaryDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, 410))
            {
                DialogBackgroundColor = Color.Red,
                EnableCloseButton = false,
            };

            Grid dialogLayout = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 10,
            };

            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Absolute) });

            int row = 0;

            Label anotherChallenge = new Label()
            {
                Text = "¿Qué quieres hacer?",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
            };

            dialogLayout.Children.Add(anotherChallenge, 0, row);
            row++;


            dialog.Content = dialogLayout;

            return dialog;
        }

        void StopClock(object sender, EventArgs e)
        {
            _clockLabel.Text = "STOP";
        }
        private async void Hint(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new PopUp());
        }
        void NextChallenge(object sender, EventArgs e)
        {
            _clockLabel.Text = "NEXT";
        }
    }
}