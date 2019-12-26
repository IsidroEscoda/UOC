using DistraidaMente.Common.Helpers;
using DistraidaMente.Common.Pages;
using DistraidaMente.Controllers;
using DistraidaMente.Controls;
using DistraidaMente.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DistraidaMente.Pages
{
    public delegate void ChallengeTypeSelectedHandler(ChallengeType challengeType);

    public class SelectChallengeTypePage : SummaryPage
    {
        EmotionalStatus? emotionalStatus;

        public static Size ChallengeTypeIconSize { get; set; }

        private Grid _challengeTypesIconsLayout;

        private class ChallengeTypeIcon
        {
            public ChallengeType ChallengeType { get; set; }
            public Image Image { get; set; }
        }

        Image emotionalIcon;

        private List<ChallengeTypeIcon> _challengeTypesIcons;


        public event ChallengeTypeSelectedHandler ChallengeTypeSelected;

        public SelectChallengeTypePage(Configuration configuration, EmotionalStatus? emotionalStatus, bool navigatingBack) : base(configuration)
        {
            this.emotionalStatus = emotionalStatus;

            NavigatingBack = navigatingBack;

            MiddleHeightProportion = 0.2;
            TopHeightProportion = 0.2;
            HideHasSelectedTitle = true;

            AnimateYouHaveChosenSectionOnEntry = navigatingBack;
            AnimateYouHaveChosenSectionOnBack = true;
            AnimateYouHaveChosenSectionOnNext = true;

            AnimateBottomContentOnEntry = true;
            AnimateBottomContentOnNext = true;
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
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            messageLabel.FormattedText = new FormattedString();

            messageLabel.FormattedText.Spans.Add(new Span() { Text = "Elige una prueba", FontAttributes = FontAttributes.Bold });

            messageGrid.Children.Add(messageLabel, 0, 0);

            return messageGrid;
        }

        protected override Layout SetupContentLayout()
        {
            Layout layout = base.SetupContentLayout();

            if (emotionalStatus != null)
            {
                emotionalIcon = FormsHelper.ConfigureImageButton($"{ emotionalStatus.ToString().ToLower() }.png", null, SelectEmotionalStatusPage.EmotionalIconSize); // new Size(85, 85));

                _mainLayout.Children.Add(emotionalIcon, new Rectangle(0.5, 130 - 50, SelectEmotionalStatusPage.EmotionalIconSize.Width, SelectEmotionalStatusPage.EmotionalIconSize.Height), AbsoluteLayoutFlags.XProportional);
            }

            ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = true };
            _mainLayout.Children.Add(activityIndicator, new Rectangle(0, 0, SelectEmotionalStatusPage.EmotionalIconSize.Width, 50), AbsoluteLayoutFlags.XProportional);


            _challengeTypesIconsLayout = new Grid()
            {
                Margin = new Thickness(0, 10),
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnSpacing = 10,
                //RowSpacing = 10,
            };

            _challengeTypesIconsLayout.ColumnDefinitions.Add(new ColumnDefinition());
            _challengeTypesIconsLayout.ColumnDefinitions.Add(new ColumnDefinition());
            _challengeTypesIconsLayout.RowDefinitions.Add(new RowDefinition());
            _challengeTypesIconsLayout.RowDefinitions.Add(new RowDefinition());

            _challengeTypesIcons = new List<ChallengeTypeIcon>();

            var icon = ConfigureOptionButton(ChallengeType.Social);
            icon.VerticalOptions = LayoutOptions.End;
            icon.HorizontalOptions = LayoutOptions.End;
            _challengeTypesIcons.Add(new ChallengeTypeIcon() { ChallengeType = ChallengeType.Social, Image = icon });
            _challengeTypesIconsLayout.Children.Add(icon, 0, 0);

            icon = ConfigureOptionButton(ChallengeType.Cognitive);
            icon.VerticalOptions = LayoutOptions.End;
            icon.HorizontalOptions = LayoutOptions.Start;
            _challengeTypesIcons.Add(new ChallengeTypeIcon() { ChallengeType = ChallengeType.Cognitive, Image = icon });
            _challengeTypesIconsLayout.Children.Add(icon, 1, 0);

            icon = ConfigureOptionButton(ChallengeType.Behavioral);
            icon.VerticalOptions = LayoutOptions.Start;
            icon.HorizontalOptions = LayoutOptions.End;
            _challengeTypesIcons.Add(new ChallengeTypeIcon() { ChallengeType = ChallengeType.Behavioral, Image = icon });
            _challengeTypesIconsLayout.Children.Add(icon, 0, 1);

            icon = ConfigureOptionButton(ChallengeType.CreateCustomChallenge);
            icon.VerticalOptions = LayoutOptions.Start;
            icon.HorizontalOptions = LayoutOptions.Start;
            _challengeTypesIcons.Add(new ChallengeTypeIcon() { ChallengeType = ChallengeType.CreateCustomChallenge, Image = icon });
            _challengeTypesIconsLayout.Children.Add(icon, 1, 1);

            _bottomContent.Children.Add(_challengeTypesIconsLayout, 0, 0);

            return layout;
        }

        private Image ConfigureOptionButton(ChallengeType challengeType, Size? size = null)
        {
            Image imageButton = FormsHelper.ConfigureImageButton($"{ challengeType.ToString().ToLower() }.png", (s, e) => { var t = OnChallengeTypeSelected(challengeType); }, size);

            //imageButton.WidthRequest = 100;
            imageButton.HorizontalOptions = LayoutOptions.Center;
            imageButton.VerticalOptions = LayoutOptions.FillAndExpand;
            imageButton.Aspect = Aspect.AspectFit;

            return imageButton;
        }

        //protected override void OnAppearing()
        //{
        //    emotionalIcon = FormsHelper.ConfigureImageButton($"{ emotionalStatus.ToString().ToLower() }.png", null, SelectEmotionalStatusPage.EmotionalIconSize); // new Size(85, 85));

        //    //emotionalIcon.VerticalOptions = LayoutOptions.Center;

        //    //emotionalIcon.Margin = new Thickness(0, 20, 0, 0);

        //    //_youHaveChosenContent.Children.Add(emotionalIcon);

        //    //_mainLayout.Children.Add(emotionalIcon, new Rectangle(0.5, 130, 87.7143, 87.7143), AbsoluteLayoutFlags.XProportional);

        //    _mainLayout.Children.Add(emotionalIcon, new Rectangle(0.5, 130, SelectEmotionalStatusPage.EmotionalIconSize.Width, SelectEmotionalStatusPage.EmotionalIconSize.Height), AbsoluteLayoutFlags.XProportional);

        //    base.OnAppearing();
        //}

        private async Task OnChallengeTypeSelected(ChallengeType selectedChallengeType)
        {
            foreach (ChallengeTypeIcon gti in _challengeTypesIcons)
            {
                gti.Image.IsEnabled = false;
            }

            await OnChallengeTypeSelectedAnimations(selectedChallengeType).ContinueWith(async (t) =>
            {
                await Task.Delay(1);

                ChallengeTypeSelected?.Invoke(selectedChallengeType);
            });
        }

        protected override async Task EntryAnimations()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(base.EntryAnimations());

            if (NavigatingBack)
            {
                //tasks.Add(emotionalIcon.AppearLeft(Configuration.AnimationSpeed));
            }

            await Task.WhenAll(tasks);
        }

        protected override async Task OnBackButtonPressedAnimations()
        {
            await Task.WhenAll
            (
                //base.OnBackButtonPressedAnimations(),
                //emotionalIcon.DisappearRight(Configuration.AnimationSpeed)
            );
        }

        //protected override async Task OnNextButtonPressedAnimations()
        //{
        //    await Task.WhenAll
        //    (
        //        base.OnNextButtonPressedAnimations(),
        //        emotionalIcon.DisappearLeft(Configuration.AnimationSpeed)
        //    );
        //}

        private const double _mainLayoutMargin = 20;

        private async Task OnChallengeTypeSelectedAnimations(ChallengeType selectedChallengeType)
        {
            //var notSelected = _challengeTypesIcons.Where(x => x.ChallengeType != selectedChallengeType);

            //var selected = _challengeTypesIcons.First(x => x.ChallengeType == selectedChallengeType);

            //await Task.WhenAll
            //(
            //    notSelected.Select(x => x.Image.FadeTo(0, Configuration.AnimationSpeed, Easing.SinIn))
            //);

            ////double imageY = (_challengeTypesIconsLayout.Parent.Parent as View).Y + (_challengeTypesIconsLayout.Parent as View).Y + _challengeTypesIconsLayout.Y + _mainLayoutMargin + ((selected.Image.Height + _challengeTypesIconsLayout.RowSpacing) * (int)selectedChallengeType);
            //double imageY = (_challengeTypesIconsLayout.Parent.Parent as View).Y + (_challengeTypesIconsLayout.Parent as View).Y + _challengeTypesIconsLayout.Y + ((selected.Image.Height + _challengeTypesIconsLayout.RowSpacing) * (int)selectedChallengeType);

            //int distanceToTop = -(int)Math.Ceiling(imageY - (130 - 50));

            //ChallengeTypeIconSize = new Size(selected.Image.Width, selected.Image.Height);

            await Task.WhenAll
            (
                //emotionalIcon.DisappearLeft(Configuration.AnimationSpeed),
                base.OnNextButtonPressedAnimations()
                //selected.Image.TranslateTo(0, distanceToTop, Configuration.AnimationSpeed, Easing.CubicInOut)
            );
        }
    }
}
