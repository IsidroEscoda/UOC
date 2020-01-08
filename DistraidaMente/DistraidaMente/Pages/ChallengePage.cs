using UOCApps.CommonLibrary.Controls;
using UOCApps.CommonLibrary.Helpers;
using DistraidaMente.Common.Model;
using DistraidaMente.Common.Pages;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using DistraidaMente.Views;
using System;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using FirebaseHelper = DistraidaMente.Helpers.FirebaseHelper;

namespace DistraidaMente.Pages
{
    public delegate string ChallengeEndHandler(ChallengeResult result);
    public delegate void ChallengeExitHandler(bool continuePlaying);
    public delegate void ChallengeSkippedHandler();

    public class ChallengePage : SummaryPage
    {
        int count;

        public event ChallengeEndHandler ChallengeEnd;
        public event ChallengeExitHandler ChallengeExit;
        public event ChallengeSkippedHandler ChallengeSkipped;

        private Challenge _challenge;

        //private Image challengeTypeIcon;

        private long _countDown;

        private Label _clockLabel;

        private Timer _timer;

        private Dialog _skip3Dialog;
        private Dialog _youveGotItDialog;
        private Dialog _resultSummaryDialog;
        private Dialog _finalResultSummaryDialog;

        private Dialog _noMoreChallengesDialog;

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

        bool _noMoreChallenges;

        public ChallengePage(Configuration configuration, Challenge challenge,bool noMoreChallenges, int totalPoints, bool showMessageIfSkipChallenge, bool showActionButtons) : base(configuration)
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

            _noMoreChallenges = noMoreChallenges;

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

            if (!_noMoreChallenges)
            {
                SetupTimer();
            }
            else
            {
                _noMoreChallengesDialog.Show();
            }
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
                TextColor = Configuration.Theme.TextColor,
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
            //SetupSkip3Dialog();

            SetupSkipChallengeLimitDialog();

            Grid optionsLayout = new Grid()
            {
                Margin = new Thickness(0, 10),
                //BackgroundColor = Color.Yellow,
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

            _stopButton = FormsHelper.ConfigureImageButton("stop.png", (s, e) => { StopChallenge(); });

            _stopButton.WidthRequest = 100;
            _stopButton.HorizontalOptions = LayoutOptions.Center;
            _stopButton.VerticalOptions = LayoutOptions.FillAndExpand;
            _stopButton.Aspect = Aspect.AspectFit;

            optionsLayout.Children.Add(_stopButton, 0, row);
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
                    BackgroundColor = Configuration.Theme.SelectedBackgroundColor,
                    TextColor = Configuration.Theme.SelectedTextColor,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) - 2,
                    //Margin = new Thickness(10, 0),
                };

                if(_challenge.Hint != "")
                {
                    actionButtons.Children.Add(_hintButton, 0, 0);
                }
               
                _skipButton = new FrameButton("Saltar prueba", (e, s) => SkipChallenge())
                {
                    BackgroundColor = Configuration.Theme.SelectedBackgroundColor,
                    TextColor = Configuration.Theme.SelectedTextColor,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) - 2,
                    //Margin = new Thickness(10, 0),
                };

                actionButtons.Children.Add(_skipButton, 1, 0);

                optionsLayout.Children.Add(actionButtons, 0, row);
                row++;
            }

            _bottomContent.Children.Add(optionsLayout);

            /*var grid = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0, 0, 0),
                BackgroundColor = Color.Transparent,
            };

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });


            grid.Children.Add(new BoxView { Color = Color.Blue }, 0, 0);
            grid.Children.Add(layout);

            _mainLayout.Children.Add(grid, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);*/

            _bottomContent.Margin = new Thickness(0);

            _youveGotItDialog = SetupYouveGotItDialog();
            _resultSummaryDialog = SetupResultSummaryDialog();
            _finalResultSummaryDialog = SetupFinalResultSummaryDialog();
            _skip3Dialog = SetupSkip3Dialog();
            _noMoreChallengesDialog = SetupnoMoreChallengesDialog();

            _mainLayout.Children.Add(_youveGotItDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_resultSummaryDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_finalResultSummaryDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_skip3Dialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_noMoreChallengesDialog, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            var grid = new Grid
            {
                BackgroundColor = Color.Transparent,
                RowSpacing = 30
            };


            var gridButton = new Button { Text = "1!" };
            gridButton.VerticalOptions = LayoutOptions.End;
            gridButton.Clicked += OnImageButtonClicked;

            var gridButton2 = new Button { Text = "2" };
            gridButton2.VerticalOptions = LayoutOptions.End;
            gridButton2.Clicked += OnImageButtonClicked;

            var imageButton = new ImageButton
            {
                BackgroundColor = Color.Transparent,
                Padding = 10,
                Source = "home.png",
                //HorizontalOptions = LayoutOptions.End,
                //VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            imageButton.Clicked += OnImageButtonClicked;

            var imageButton2 = new ImageButton
            {
                BackgroundColor = Color.Transparent,
              
                Padding = 10,
                Source = "info.png",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            imageButton2.Clicked += OnImageButtonClicked2;

            var imageButton3 = new ImageButton
            {
                BackgroundColor = Color.Transparent,
                Padding = 10,
                Source = "list.png",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            imageButton3.Clicked += OnImageButtonClicked3;

            var imageButton4 = new ImageButton
            {
                BackgroundColor = Color.Transparent,
                Padding = 10,
                Source = "person.png",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            imageButton4.Clicked += OnImageButtonClicked4;

            grid.Children.Add(imageButton, 0, 0);
            grid.Children.Add(imageButton2, 1, 0);
            grid.Children.Add(imageButton3, 2, 0);
            grid.Children.Add(imageButton4, 3, 0);

            grid.VerticalOptions = LayoutOptions.End;

            _mainLayout.Children.Add(grid, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            return layout;
        }

        private Dialog SetupSkip3Dialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, _challenge.Solution != null ? 430 : 340))
            {
                DialogBackgroundColor = _configuration.Theme.BackgroundColor,
                EnableCloseButton = false,
            };

            Grid dialogLayout = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 20,
            };

            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60, GridUnitType.Absolute) });

            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Absolute) });
            dialogLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Absolute) });

            int row = 0;

            var infoIcon = FormsHelper.ConfigureImageButton("info.png");

            dialogLayout.Children.Add(infoIcon, 0, row);
            row++;

            dialogLayout.Children.Add(new Label()
            {
                Text = $"Sólo puedes saltar de prueba tres veces.",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
            }, 0, row);
            row++;

            dialogLayout.Children.Add(new FrameButton("VOLVER", (e, s) => { dialog.Close(); EnableButtonsAfter3Times(true); })
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                BackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                TextColor = _configuration.Theme.SelectedTextColor,
                Margin = new Thickness(35, 0),
            }, 0, row);
            row++;

            dialogLayout.Children.Add(new FrameButton("PASAR", (e, s) => { ShowResultSummary(false, false); })
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                BackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                TextColor = _configuration.Theme.SelectedTextColor,
                Margin = new Thickness(35, 0),
            }, 0, row);
            row++;

            dialog.Content = dialogLayout;

            //_mainLayout.Children.Add(dialog, new Rectangle(0, 250, 1, 350), AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.WidthProportional);
            return dialog;
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            ImageButton ib = (sender as ImageButton);
            /*var source = ib.Id;

            if (source == "back")*/

            TabsPage tabbed = new TabsPage(_configuration);

            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });
        }

        void OnImageButtonClicked2(object sender, EventArgs e)
        {
            TabsPage tabbed = new TabsPage(_configuration);
            tabbed.CurrentPage = tabbed.Children[1];
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });
        }
        void OnImageButtonClicked3(object sender, EventArgs e)
        {
            TabsPage tabbed = new TabsPage(_configuration);
            tabbed.CurrentPage = tabbed.Children[2];
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });
        }
        void OnImageButtonClicked4(object sender, EventArgs e)
        {
            TabsPage tabbed = new TabsPage(_configuration);
            tabbed.CurrentPage = tabbed.Children[3];
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });
        }

        private void SetupHintDialog()
        {
            _hintDialog = new Dialog(new Size(App.ScreenWidth - 30, _challenge.Hint != null ? 530 : 440))
            {
                DialogBackgroundColor = Configuration.Theme.BackgroundColor,
                DialogVerticalAlignment = LayoutOptions.FillAndExpand,
                //DialogMargin = new Thickness (0,250,0,0),
                //EnableCloseButton = false,
                ShowOverlay = false,
                //AutoCloseTime = 5000,
            };

            Grid hintLayout = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 20,
            };

            hintLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Absolute) });
            hintLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            hintLayout.Children.Add(FormsHelper.ConfigureImageButton("hint.png"), 0, 0);

            hintLayout.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 2,
                Text = _challenge.Hint ?? "¡Sólo tienes que ir más rápido para acabar la prueba!",
                TextColor = Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            }, 1, 0);
            
            _hintDialog.Content = hintLayout;
            
            _hintDialog.Closed += () => Device.BeginInvokeOnMainThread(() => EnableButtons(true));

            _mainLayout.Children.Add(_hintDialog, new Rectangle(0, 250, 1, 350), AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.WidthProportional);
        }

        private void SetupSkipChallengeLimitDialog()
        {
            _skipChallengeLimitDialog = new Dialog(new Size(App.ScreenWidth - 30, 250))
            {
                DialogBackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                DialogVerticalAlignment = LayoutOptions.StartAndExpand,
                //DialogMargin = new Thickness (0,250,0,0),
                //EnableCloseButton = false,
                ShowOverlay = false,
                AutoCloseTime = 6000,
            };

            Grid skipChallengeLimitLayout = new Grid();

            skipChallengeLimitLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Absolute) });
            skipChallengeLimitLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            skipChallengeLimitLayout.Children.Add(FormsHelper.ConfigureImageButton("hint.png"), 0, 0);

            skipChallengeLimitLayout.Children.Add(new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 2,
                Text = "Sólo puedes saltar de prueba tres veces.",
                TextColor = Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            }, 1, 0);

            _skipChallengeLimitDialog.Content = skipChallengeLimitLayout;

            _mainLayout.Children.Add(_skipChallengeLimitDialog, new Rectangle(0, 250, 1, 250), AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.WidthProportional);

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

                    //await _skipChallengeLimitDialog.Show();
                    await _skip3Dialog.Show();
                    _timer.Stop();

                }
                else
                {

                    await OnNextButtonPressedAnimations().ContinueWith(async (t) =>
                    {
                        await Task.Delay(1);

                        Device.BeginInvokeOnMainThread(() => ChallengeSkipped?.Invoke());
                    });
                }
                FirebaseHelper firebaseHelper = new FirebaseHelper();
                await firebaseHelper.UpdatePersonPruSal(8, _configuration.ReadProperty("docId"));
            });
        }

        private void ShowHint()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _challengeResult.HintUsed = true;

                EnableButtons(false);
                _timer.Stop();

                await _hintDialog.Show();

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                await firebaseHelper.UpdatePersonPisSol(8, _configuration.ReadProperty("docId"));
            });
        }

        private Dialog SetupYouveGotItDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, _challenge.Solution != null ? 430 : 340))
            {
                DialogBackgroundColor = _configuration.Theme.BackgroundColor,
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

            var summaryIcon = FormsHelper.ConfigureImageButton("timestopped.png");

            dialogLayout.Children.Add(summaryIcon, 0, row);
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
                BackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                TextColor = Color.FromHex("#000078"),
                Margin = new Thickness(35, 0),
            }, 0, row);
            row++;

            dialogLayout.Children.Add(new FrameButton("NO", (e, s) => { ShowResultSummary(false, false); })
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                BackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
                TextColor = Color.FromHex("#000078"),
                Margin = new Thickness(35, 0),
            }, 0, row);
            row++;

            dialog.Content = dialogLayout;

            return dialog;
        }

        private Dialog SetupnoMoreChallengesDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, 300))
            {
                DialogBackgroundColor = _configuration.Theme.BackgroundColor,
                EnableCloseButton = false,
            };

            Grid dialogLayout = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 20,
            };

            int row = 0;

            var summaryIcon = FormsHelper.ConfigureImageButton("info.png");

            dialogLayout.Children.Add(summaryIcon, 0, row);
            row++;

            Label anotherChallenge = new Label()
            {
                Text = "Se han acabado las pruebas de esta categoria...ves al Inicio o al apartado de Filtros que se encuentra en Perfil.",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
            };

            dialogLayout.Children.Add(anotherChallenge, 0, row);
            row++;

            Button buttonI = new Button
            {
                Text = "Volver a Inicio",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                TextColor = Color.FromHex("#000078"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(35, 0),
            };

            ImageButton imageButton = new ImageButton
            {
                Source = "info.png",
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 60,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            buttonI.Clicked += OnImageButtonClicked;

            var continueButton = new FrameButton("Ir a Inicio", (e, s) => { OnChallengeEnd(true); })
            {
                BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                TextColor = Color.FromHex("#000078"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(35, 0),
            };

            dialogLayout.Children.Add(buttonI, 0, row);
            row++;

            dialog.Content = dialogLayout;
            return dialog;
        }

        private Dialog SetupResultSummaryDialog()
        {
            Dialog dialog = new Dialog(new Size(App.ScreenWidth - 30, 410))
            {
                DialogBackgroundColor = _configuration.Theme.BackgroundColor,
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

            var continueButton = new FrameButton("Seguir jugando", (e, s) => { OnChallengeEnd(true); })
            {
                BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                TextColor = Color.FromHex("#000078"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(35, 0),
            };

            dialogLayout.Children.Add(continueButton, 0, row);
            row++;

            var exitButton = new FrameButton("Salir", (e, s) => { OnChallengeEnd(false); })
            {
                BackgroundColor = Configuration.Theme.SecondaryBackgroundColor,
                TextColor = Color.FromHex("#000078"),
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
                DialogBackgroundColor = _configuration.Theme.SecondaryBackgroundColor,
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

            var prizeIcon = FormsHelper.ConfigureImageButton("ivegotit.png");

            dialogLayout.Children.Add(prizeIcon, 0, row);
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

                //_resultSummaryDialogPoints.Text = iveGotIt ? $"10 puntos" : $"0 puntos";
                _resultSummaryDialogPoints.Text = $"{ _challengeResult.ChallengePoints } puntos";
                _resultSummaryDialogPoints.TextColor = _configuration.Theme.SelectedBackgroundColor;

                await _resultSummaryDialog.Show();

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                if (iveGotIt)
                {
                    await firebaseHelper.UpdatePersonResAce(_configuration.ReadProperty("docId"));
                }
                else
                {
                    await firebaseHelper.UpdatePersonResErr(_configuration.ReadProperty("docId"));
                }
                await firebaseHelper.UpdatePersonPruRea(_configuration.ReadProperty("docId"));
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

                FirebaseHelper firebaseHelper = new FirebaseHelper();
                await firebaseHelper.AddRankingThing(Convert.ToInt32(ranking),  _challengeResult.TotalPoints, _configuration.UserId.ToString(), _configuration.ReadProperty("name"));

                await _finalResultSummaryDialog.Show();

                await Task.Delay(1000);

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
            _timer.Start();
        }
        private void EnableButtonsAfter3Times(bool enable)
        {
            _hintButton.IsEnabled = enable;
            _skipButton.IsEnabled = !enable;
            _stopButton.IsEnabled = enable;
            _timer.Start();
        }
    }
}
