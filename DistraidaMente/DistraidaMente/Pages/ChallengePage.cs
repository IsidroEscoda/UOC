using DistraidaMente.Common.Pages;
using DistraidaMente.Controllers;
using DistraidaMente.Controls;
using DistraidaMente.Common.Model;
using DistraidaMente.Models;
using System;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace DistraidaMente.Pages
{
    public delegate string ChallengeEndHandler(ChallengeResult result);
    public delegate void ChallengeExitHandler(bool continuePlaying);
    public delegate void ChallengeSkippedHandler();

    public class ChallengePage : SummaryPage
    {
        public event ChallengeEndHandler ChallengeEnd;
        public event ChallengeExitHandler ChallengeExit;
        public event ChallengeSkippedHandler ChallengeSkipped;

        private Challenge _challenge;

        //private Image challengeTypeIcon;

        private long _countDown;

        private Label _clockLabel;

        private Timer _timer;

        private Dialog _youveGotItDialog;
        private Dialog _resultSummaryDialog;
        private Dialog _finalResultSummaryDialog;

        private Dialog _hintDialog;
        private Dialog _skipChallengeLimitDialog;

        private Image summaryIcon;

        private Label _resultSummaryDialogTitle;
        private Label _resultSummaryDialogPoints;
        private Label _finalResultSummaryDialogPoints;
        private Label _finalResultSummaryDialogRanking;

        private Image _stopButton;
        private FrameButton _skipButton;
        private FrameButton _hintButton;

        Configuration _configuration;

        ChallengeResult _challengeResult;

        bool _showMessageIfSkipChallenge;
        bool _showActionButtons;
        private Challenge challenge;
        private int points;
        private bool showMessageIfSkipChallenge;
        private bool showActionButtons;

            public ChallengePage(Configuration configuration, Challenge challenge, int totalPoints, bool showMessageIfSkipChallenge, bool showActionButtons) : base(configuration)
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

            NavigatingBack = false;

            AnimateBottomContentOnEntry = true;
            AnimateBottomContentOnNext = true;

            TopHeightProportion = 0.05;
            MiddleHeightProportion = 0.5;

            HideHasSelectedTitle = true;

            _countDown = challenge.TimeInSeconds;

            //AnimateYouHaveChosenSectionOnEntry = navigatingBack;
            //AnimateYouHaveChosenSectionOnBack = false;
            //AnimateYouHaveChosenSectionOnNext = true;
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

        protected override Grid SetupMessageGrid()
        {
            Grid messageGrid = new Grid()
            {
                RowSpacing = 20,
            };

            messageGrid.RowDefinitions.Add(new RowDefinition() { });

            AutoFontSizeFormattedTextLabel messageLabel = new AutoFontSizeFormattedTextLabel()
            {
                MinFontSize = 12,
                MaxFontSize = 26,
                //FontSize = 100,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            messageLabel.FormattedText = new FormattedString();

            messageLabel.FormattedText.Spans.Add(new Span() { Text = _challenge.Statement });

            messageGrid.Children.Add(messageLabel, 0, 0);

            return messageGrid;
        }

        protected override Layout SetupContentLayout()
        {
            Layout layout = base.SetupContentLayout();

            //challengeTypeIcon = FormsHelper.ConfigureImageButton($"{ challenge.Type.ToString().ToLower() }.png", null, SelectChallengeTypePage.ChallengeTypeIconSize);

            //_mainLayout.Children.Add(challengeTypeIcon, new Rectangle(0.5, 130 - 50, SelectChallengeTypePage.ChallengeTypeIconSize.Width, SelectChallengeTypePage.ChallengeTypeIconSize.Height), AbsoluteLayoutFlags.XProportional);

            SetupHintDialog();

            SetupSkipChallengeLimitDialog();

            Grid optionsLayout = new Grid()
            {
                Margin = new Thickness(0, 10),
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 15,
            };

            optionsLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            optionsLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            if (_showActionButtons)
            {
                optionsLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            var row = 0;

            _clockLabel = new AutoFontSizeLabel()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            optionsLayout.Children.Add(_clockLabel, 0, row);
            row++;

            UpdateClock();

            //_stopButton = FormsHelper.ConfigureImageButton("stop.png", (s, e) => { StopChallenge(); });

            //_stopButton.WidthRequest = 100;
            //_stopButton.HorizontalOptions = LayoutOptions.Center;
            //_stopButton.VerticalOptions = LayoutOptions.FillAndExpand;
            //_stopButton.Aspect = Aspect.AspectFit;

            //optionsLayout.Children.Add(_stopButton, 0, row);
            row++;

            if (_showActionButtons)
            {
                Grid actionButtons = new Grid()
                {
                    VerticalOptions = LayoutOptions.Center,
                    ColumnSpacing = 10,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                };

                _hintButton = new FrameButton("Pista", (e, s) => ShowHint())
                {
                    //BackgroundColor = Configuration.Theme.SelectedBackgroundColor,
                    //TextColor = Configuration.Theme.SelectedTextColor,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) - 2,
                    //Margin = new Thickness(10, 0),
                };

                actionButtons.Children.Add(_hintButton, 0, 0);

                _skipButton = new FrameButton("Saltar prueba", (e, s) => SkipChallenge())
                {
                    //BackgroundColor = Configuration.Theme.SelectedBackgroundColor,
                    //TextColor = Configuration.Theme.SelectedTextColor,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) - 2,
                    //Margin = new Thickness(10, 0),
                };

                actionButtons.Children.Add(_skipButton, 1, 0);

                optionsLayout.Children.Add(actionButtons, 0, row);
                row++;
            }

            _bottomContent.Children.Add(optionsLayout);

            ImageButton imageButton = new ImageButton
            {
                Padding= 20,
                BackgroundColor= Color.Transparent,
                Source = "back.png"
            };
            imageButton.Clicked += OnImageButtonClicked;
            _mainLayout.Children.Add(imageButton);

            _bottomContent.Margin = new Thickness(0);

            _youveGotItDialog = SetupYouveGotItDialog();
            _resultSummaryDialog = SetupResultSummaryDialog();
            _finalResultSummaryDialog = SetupFinalResultSummaryDialog();

            _mainLayout.Children.Add(_youveGotItDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_resultSummaryDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_finalResultSummaryDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            return layout;
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            
        }

        private void SetupHintDialog()
        {
            _hintDialog = new Dialog(new Size(App.ScreenWidth - 30, 150))
            {
                //DialogBackgroundColor = Configuration.Theme.BackgroundColor,
                ShowOverlay = false,
                AutoCloseTime = 5000,
            };

            Grid hintLayout = new Grid();

            hintLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Absolute) });
            hintLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            //hintLayout.Children.Add(FormsHelper.ConfigureImageButton("hint.png"), 0, 0);

            hintLayout.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 2,
                Text = _challenge.Hint ?? "¡Sólo tienes que ir más rápido para acabar la prueba!",
                //TextColor = Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            }, 1, 0);

            _hintDialog.Content = hintLayout;

            _hintDialog.Closed += () => Device.BeginInvokeOnMainThread(() => EnableButtons(true));

            _mainLayout.Children.Add(_hintDialog, new Rectangle(0, 250, 1, 150), AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.WidthProportional);
        }

        private void SetupSkipChallengeLimitDialog()
        {
            _skipChallengeLimitDialog = new Dialog(new Size(App.ScreenWidth - 30, 150))
            {
                //DialogBackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                ShowOverlay = false,
                AutoCloseTime = 3000,
            };

            Grid skipChallengeLimitLayout = new Grid();

            skipChallengeLimitLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Absolute) });
            skipChallengeLimitLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            //skipChallengeLimitLayout.Children.Add(FormsHelper.ConfigureImageButton("hint.png"), 0, 0);

            skipChallengeLimitLayout.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 2,
                Text = "Sólo puedes saltar de prueba tres veces.",
                //TextColor = Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            }, 1, 0);

            _skipChallengeLimitDialog.Content = skipChallengeLimitLayout;

            _mainLayout.Children.Add(_skipChallengeLimitDialog, new Rectangle(0, 250, 1, 150), AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.WidthProportional);
        }

        private void SkipChallenge()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _timer.Stop();

                _hintDialog.Hide();

                if (_showMessageIfSkipChallenge)
                {
                    EnableButtons(false);

                    await _skipChallengeLimitDialog.Show();
                }

                await OnNextButtonPressedAnimations().ContinueWith(async (t) =>
                {
                    await Task.Delay(1);

                    Device.BeginInvokeOnMainThread(() => ChallengeSkipped?.Invoke());
                });
            });
        }

        private void ShowHint()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _challengeResult.HintUsed = true;

                EnableButtons(false);

                await _hintDialog.Show();
            });
        }

        private Dialog SetupYouveGotItDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, _challenge.Solution != null ? 430 : 340))
            {
                //DialogBackgroundColor = _configuration.Theme.BackgroundColor,
                EnableCloseButton = false,
            };

            Grid dialogLayout = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 20,
            };

            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Absolute) });

            if (_challenge.Solution != null)
            {
                dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Absolute) });

            int row = 0;

            //var summaryIcon = FormsHelper.ConfigureImageButton("timestopped.png");

            //dialogLayout.Children.Add(summaryIcon, 0, row);
            row++;

            if (_challenge.Solution != null)
            {
                dialogLayout.Children.Add(new AutoFontSizeLabel() { Text = $"Solución: { _challenge.Solution }", HorizontalTextAlignment = TextAlignment.Center }, 0, row);
                row++;
            }

            dialogLayout.Children.Add(new Label()
            {
                Text = $"¿Lo has conseguido?",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
            }, 0, row);
            row++;

            dialogLayout.Children.Add(new FrameButton("SÍ", (e, s) => { ShowResultSummary(false, true); })
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                //BackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                //TextColor = _configuration.Theme.SelectedTextColor,
                Margin = new Thickness(35, 0),
            }, 0, row);
            row++;

            dialogLayout.Children.Add(new FrameButton("NO", (e, s) => { ShowResultSummary(false, false); })
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                //BackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                //TextColor = _configuration.Theme.SelectedTextColor,
                Margin = new Thickness(35, 0),
            }, 0, row);
            row++;

            dialog.Content = dialogLayout;

            return dialog;
        }

        private Dialog SetupResultSummaryDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, 410))
            {
                //DialogBackgroundColor = _configuration.Theme.BackgroundColor,
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

            summaryIcon = new Image()
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            dialogLayout.Children.Add(summaryIcon, 0, row);
            row++;

            _resultSummaryDialogTitle = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 4,
            };

            dialogLayout.Children.Add(_resultSummaryDialogTitle, 0, row);
            row++;

            _resultSummaryDialogPoints = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Red,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 4,
            };

            dialogLayout.Children.Add(_resultSummaryDialogPoints, 0, row);
            row++;

            Label anotherChallenge = new Label()
            {
                Text = "¿Qué quieres hacer?",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
            };

            dialogLayout.Children.Add(anotherChallenge, 0, row);
            row++;

            //Grid buttons = new Grid()
            //{
            //    Margin = new Thickness(20, 0),
            //    ColumnSpacing = 10,
            //    RowSpacing = 10,
            //};

            //var icon = ConfigureOptionButton(ChallengeType.Social);
            //icon.HorizontalOptions = LayoutOptions.EndAndExpand;
            //icon.VerticalOptions = LayoutOptions.EndAndExpand;
            //buttons.Children.Add(icon, 0, 0);

            //icon = ConfigureOptionButton(ChallengeType.Cognitive);
            //icon.HorizontalOptions = LayoutOptions.StartAndExpand;
            //icon.VerticalOptions = LayoutOptions.EndAndExpand;
            //buttons.Children.Add(icon, 1, 0);

            //icon = ConfigureOptionButton(ChallengeType.Behavioral);
            //icon.HorizontalOptions = LayoutOptions.EndAndExpand;
            //icon.VerticalOptions = LayoutOptions.StartAndExpand;
            //buttons.Children.Add(icon, 0, 1);

            //icon = ConfigureOptionButton(ChallengeType.CreateCustomChallenge);
            //icon.HorizontalOptions = LayoutOptions.StartAndExpand;
            //icon.VerticalOptions = LayoutOptions.StartAndExpand;
            //buttons.Children.Add(icon, 1, 1);

            //dialogLayout.Children.Add(buttons, 0, row);
            //row++;

            var continueButton = new FrameButton("Seguir jugando", (e, s) => { OnChallengeEnd(true); })
            {
                //BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                //TextColor = Configuration.Theme.SelectedTextColor,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(35, 0),
            };

            dialogLayout.Children.Add(continueButton, 0, row);
            row++;

            var exitButton = new FrameButton("Salir", (e, s) => { OnChallengeEnd(false); })
            {
                //BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                //TextColor = Configuration.Theme.SelectedTextColor,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(35, 0),
            };

            dialogLayout.Children.Add(exitButton, 0, row);
            row++;

            dialog.Content = dialogLayout;

            return dialog;
        }

        private Dialog SetupFinalResultSummaryDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, 540))
            {
                //DialogBackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                EnableCloseButton = false,
            };

            Grid dialogLayout = new Grid()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                RowSpacing = 10,
                Padding = new Thickness(15),
            };

            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Absolute) });

            int row = 0;

            //var prizeIcon = FormsHelper.ConfigureImageButton("ivegotit.png");

            //dialogLayout.Children.Add(prizeIcon, 0, row);
            row++;

            var title = new Label()
            {
                Text = "Has acumulado",
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 4,
            };

            dialogLayout.Children.Add(title, 0, row);
            row++;

            _finalResultSummaryDialogPoints = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Red,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 4,
            };

            dialogLayout.Children.Add(_finalResultSummaryDialogPoints, 0, row);
            row++;

            _finalResultSummaryDialogRanking = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                //TextColor = Color.Red,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            };

            dialogLayout.Children.Add(_finalResultSummaryDialogRanking, 0, row);
            row++;

            dialog.Content = dialogLayout;

            return dialog;
        }

        //private async void ShowRankingDialog(RankingPosition currentPosition, )
        //{
        //    Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, 500))
        //    {
        //        DialogBackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
        //        EnableCloseButton = false,
        //    };

        //    Grid dialogLayout = new Grid()
        //    {
        //        VerticalOptions = LayoutOptions.CenterAndExpand,
        //        HorizontalOptions = LayoutOptions.CenterAndExpand,
        //        RowSpacing = 10,
        //        Padding = new Thickness(15),
        //    };

        //    dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Absolute) });
        //    dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
        //    dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
        //    dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Absolute) });

        //    int row = 0;

        //    dialogLayout.Children.Add(new Label()
        //    {
        //        HorizontalTextAlignment = TextAlignment.Center,
        //        FontAttributes = FontAttributes.Bold,
        //        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 4,
        //        Text = "Ranking",
        //    }, 0, row);
        //    row++;

        //    var prizeIcon = FormsHelper.ConfigureImageButton("ivegotit.png");

        //    dialogLayout.Children.Add(prizeIcon, 0, row);
        //    row++;

        //    var title = new Label()
        //    {
        //        Text = "Has acumulado",
        //        HorizontalTextAlignment = TextAlignment.Center,
        //        FontAttributes = FontAttributes.Bold,
        //        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 4,
        //    };

        //    dialogLayout.Children.Add(title, 0, row);
        //    row++;

        //    _finalResultSummaryDialogPoints = new Label()
        //    {
        //        HorizontalTextAlignment = TextAlignment.Center,
        //        TextColor = Color.Red,
        //        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 4,
        //    };

        //    dialogLayout.Children.Add(_finalResultSummaryDialogPoints, 0, row);
        //    row++;

        //    _finalResultSummaryDialogRanking = new Label()
        //    {
        //        HorizontalTextAlignment = TextAlignment.Center,
        //        TextColor = Color.Red,
        //        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
        //    };

        //    dialogLayout.Children.Add(_finalResultSummaryDialogRanking, 0, row);
        //    row++;

        //    dialog.Content = dialogLayout;

        //    await dialog.Show();
        //}

        //private Image ConfigureOptionButton(ChallengeType challengeType, Size? size = null)
        //{
        //    Image imageButton = FormsHelper.ConfigureImageButton($"{ challengeType.ToString().ToLower() }.png", (s, e) => { OnChallengeEnd(challengeType); }, size);

        //    imageButton.WidthRequest = 100;
        //    imageButton.HorizontalOptions = LayoutOptions.Center;
        //    imageButton.VerticalOptions = LayoutOptions.FillAndExpand;
        //    imageButton.Aspect = Aspect.AspectFit;

        //    return imageButton;
        //}

        private void StopChallenge()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _hintDialog.Hide();

                _timer.Stop();

                await _youveGotItDialog.Show();
            });
        }

        private void ShowResultSummary(bool timeOver, bool iveGotIt)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _hintDialog.Hide();

                _youveGotItDialog.Hide();

                var iconName = timeOver ? "timeover.png" : iveGotIt ? "ivegotit.png" : "youlose.png";

                summaryIcon.Source = ImageSource.FromFile(iconName);

                _challengeResult.TimeOver = timeOver;
                _challengeResult.ChallengeSuccess = iveGotIt;
                _challengeResult.ElapsedTime = _challenge.TimeInSeconds - _countDown;

                _challengeResult.ChallengePoints = iveGotIt ? (_challenge.Completed ? _challenge.PointsIfRepeat : (_challengeResult.HintUsed ? _challenge.PointsWithHint : _challenge.Points)) : _challenge.PenaltyPoints;

                _challengeResult.TotalPoints = Math.Max(0, _challengeResult.TotalPoints + _challengeResult.ChallengePoints);

                _resultSummaryDialogTitle.Text = timeOver ? $"¡Se acabó el tiempo!" : iveGotIt ? $"¡Prueba superada!" : $"Prueba no superada.";

                _resultSummaryDialogPoints.Text = $"{ _challengeResult.ChallengePoints } puntos";
                //_resultSummaryDialogPoints.TextColor = _configuration.Theme.SelectedBackgroundColor;

                await _resultSummaryDialog.Show();
            });
        }

        private void UpdateClock()
        {
            Device.BeginInvokeOnMainThread(() => _clockLabel.Text = $"{_countDown / 60:00}:{_countDown % 60:00}");
        }

        private void TimeElapsed()
        {
            ShowResultSummary(true, false);
        }

        //protected override async Task EntryAnimations()
        //{
        //    List<Task> tasks = new List<Task>();

        //    tasks.Add(base.EntryAnimations());

        //    if (NavigatingBack)
        //    {
        //        tasks.Add(emotionalIcon.AppearLeft(Configuration.AnimationSpeed));
        //    }

        //    await Task.WhenAll(tasks);
        //}

        ////protected override async Task OnBackButtonPressedAnimations()
        ////{
        ////    await Task.WhenAll
        ////    (
        ////        base.OnBackButtonPressedAnimations(),
        ////        emotionalIcon.DisappearRight (Configuration.AnimationFastSpeed)
        ////    );
        ////}

        //protected override async Task OnNextButtonPressedAnimations()
        //{
        //    await Task.WhenAll
        //    (
        //        base.OnNextButtonPressedAnimations(),
        //        emotionalIcon.DisappearLeft(Configuration.AnimationSpeed)
        //    );
        //}

        private async void OnChallengeEnd(bool continuePlaying)
        {
            string rankingResponse = ChallengeEnd?.Invoke(_challengeResult);

            string[] parts = rankingResponse.Split(';');

            string ranking = parts[0];
            string leftPoints = parts[1];
            //string ranking = "0";
            //string leftPoints = "0";

            _resultSummaryDialog.Hide();

            if (!continuePlaying)
            {
                _finalResultSummaryDialogPoints.Text = $"{ _challengeResult.TotalPoints } puntos";
                //_finalResultSummaryDialogPoints.Text = $"0 puntos";

                string leftPointsMessage = leftPoints != "0" ? $"Te faltan { leftPoints } puntos para alcanzar la primera posición." : String.Empty;

                _finalResultSummaryDialogRanking.Text = $"Estás en la posición nº { ranking } del ranking. { leftPointsMessage }";

                //FirebaseHelper firebaseHelper = new FirebaseHelper();
                //firebaseHelper.AddRankingThing(Convert.ToInt32(ranking),  _challengeResult.TotalPoints, _configuration.UserId.ToString(), _configuration.ReadProperty("name"));

                await _finalResultSummaryDialog.Show();

                await Task.Delay(3000);

                _finalResultSummaryDialog.Hide();
            }
            
            await OnNextButtonPressedAnimations().ContinueWith((t) =>
            {
                Device.BeginInvokeOnMainThread(() => ChallengeExit?.Invoke(continuePlaying));
            });
        }

        private void EnableButtons(bool enable)
        {
            _hintButton.IsEnabled = enable;
            _skipButton.IsEnabled = enable;
            _stopButton.IsEnabled = enable;
        }
    }
}
