using UOCApps.CommonLibrary.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using UOCApps.Common.Model;
using UOCApps.CommonLibrary.Controls;

namespace UOCApps.Common.Pages
{
    public abstract class BasePage : ContentPage, ISwipeCallBack
    {
        public Configuration Configuration { get; set; }

        public BasePage(Configuration configuration)
        {
            Configuration = configuration;
        }

        public delegate void ButtonPressedHandler();

        private readonly object interactionLock = new object();
        private bool interactionEnabled = true;

        protected bool LockInteraction()
        {
            lock (interactionLock)
            {
                if (interactionEnabled)
                {
                    interactionEnabled = false;

                    return true;
                }
            }

            return false;
        }

        protected void ReleaseInteraction()
        {
            lock (interactionLock)
            {
                interactionEnabled = true;
            }
        }

        public event ButtonPressedHandler BackButtonPressed;
        public event ButtonPressedHandler NextButtonPressed;

        private SwipeListener swipeListener;
        protected AbsoluteLayout _mainLayout;
        //protected StackLayout _contentLayout;
        protected Grid _navigationBar;

        //protected string TitleText { get; set; }

        // TODO : borrar????
        //protected bool ShowTitle { get; set; }

        public bool ShowNavigationBarOnEntry { get; set; }
        public bool AnimateNavigationBarOnEntry { get; set; }
        public bool AnimateNavigationBarOnBack { get; set; }
        public bool AnimateNavigationBarOnNext { get; set; }

        public bool EnableBackButton { get; set; }

        public bool EnableNextButton { get; set; }

        protected Image BackButton { get; set; }

        protected Image NextButton { get; set; }

        protected Label NavigationBarMessage { get; set; }

        public bool NavigatingBack { get; set; }

        public static string BackgroundImageName
        {
            set
            {
                BackgroundImage = FormsHelper.ConfigureImageButton(value);

                BackgroundImage.Aspect = Aspect.Fill;
            }
        }

        protected static Image BackgroundImage { get; set; }

        public void Initialize()
        {
            SetupPage();
        }

        protected override void OnAppearing()
        {
            if (BackgroundImage != null)
            {
                _mainLayout.Children.Add(BackgroundImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

                _mainLayout.LowerChild(BackgroundImage);
            }

            //CrossAnalytics.Current.TrackScreen(GetType().Name);

            //AnalyticsTracker.TrackView(this.GetType().Name);
            //AnalyticsTracker.TrackEvent(this.GetType().Name, this.GetType().Name, this.GetType().Name, 5);

            // SetupPage();
            // Task.Delay(1000);

            var task = EntryAnimations();
        }

        private void SetupPage()
        {
            BackgroundColor = Configuration.Theme.BackgroundColor;

            _mainLayout = new AbsoluteLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            _navigationBar = SetupNavigationBar();

            _mainLayout.Children.Add(SetupContentLayout(), new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            _mainLayout.Children.Add(_navigationBar, new Rectangle(0, 1, 1, 50), AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
                                          
            Content = _mainLayout;

            //_mainLayout.IsVisible = false;

            swipeListener = new SwipeListener(_mainLayout, this);
        }

        protected abstract Layout SetupContentLayout();
        //{
        //    StackLayout _contentLayout = new StackLayout()
        //    {
        //        HorizontalOptions = LayoutOptions.FillAndExpand,
        //        VerticalOptions = LayoutOptions.FillAndExpand,
        //    };

        //    return _contentLayout;
        //}

        private Grid SetupNavigationBar()
        {
            Grid navigationBarLayout = new Grid()
            {
                BackgroundColor = Configuration.Theme.BackgroundColor,
                IsVisible = ShowNavigationBarOnEntry,
                HeightRequest = 50,
            };

            navigationBarLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Absolute) });
            navigationBarLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            navigationBarLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Absolute) });

            BackButton = SetupNavigationButton("back", EnableBackButton, OnBackButtonPressed);
            navigationBarLayout.Children.Add(BackButton, 0, 0);

            NavigationBarMessage = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Configuration.Theme.SelectedTextColor,
                FontSize = Configuration.Theme.MediumFontSize,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            navigationBarLayout.Children.Add(NavigationBarMessage, 1, 0);

            NextButton = SetupNavigationButton("next_", EnableNextButton, OnNextButtonPressed);
            navigationBarLayout.Children.Add(NextButton, 2, 0);

            return navigationBarLayout;
        }

        private Image SetupNavigationButton(string type, bool enable, Func<bool> action)
        {
            //Image navigationButton = FormsHelper.ConfigureImageButton($"UOCApps.PositiveThinking.Images.{ type }.png", (e, s) => { action(); }, new Size(40, 40), false);
            Image navigationButton = FormsHelper.ConfigureImageButton($"{ type }.png", (e, s) => { action(); }, new Size(40, 40));

            navigationButton.IsEnabled = enable;

            navigationButton.VerticalOptions = LayoutOptions.End;
            navigationButton.HorizontalOptions = LayoutOptions.Start;

            navigationButton.Margin = new Thickness(10);

            return navigationButton;
        }

        protected override bool OnBackButtonPressed()
        {
            if (EnableBackButton && LockInteraction())
            {
                OnBackButtonPressedAnimations().ContinueWith((t) => { BackButtonPressed?.Invoke(); ReleaseInteraction(); });
            }

            return BackButtonPressed != null || base.OnBackButtonPressed();
        }


        protected virtual bool OnNextButtonPressed()
        {
            if (EnableNextButton && LockInteraction())
            {
                OnNextButtonPressedAnimations().ContinueWith((t) => { NextButtonPressed?.Invoke(); ReleaseInteraction(); });
            }

            return true;
        }

        public async Task ShowNavigationBar()
        {
            await _navigationBar.AppearBottom(Configuration.AnimationSpeed);
        }

        public async Task HideNavigationBar()
        {
            await _navigationBar.DisappearBottom(Configuration.AnimationSpeed);

            _navigationBar.IsVisible = false;
        }

        protected virtual async Task EntryAnimations()
        {
            //_mainLayout.IsVisible = true;

            List<Task> tasks = new List<Task>();

            if (AnimateNavigationBarOnEntry)
            {
                await ShowNavigationBar();
            }

            await Task.WhenAll(tasks);
        }

        protected virtual async Task OnBackButtonPressedAnimations()
        {
            if (AnimateNavigationBarOnBack)
            {
                await HideNavigationBar();
            }
        }

        protected virtual async Task OnNextButtonPressedAnimations()
        {
            if (AnimateNavigationBarOnNext)
            {
                await HideNavigationBar();
            }
        }

        public void OnLeftSwipe(View view)
        {
            OnNextButtonPressed();
        }

        public void OnRightSwipe(View view)
        {
            OnBackButtonPressed();
        }

        public void OnTopSwipe(View view)
        {
        }

        public void OnBottomSwipe(View view)
        {
        }

        public void OnNothingSwiped(View view)
        {
        }
    }
}
