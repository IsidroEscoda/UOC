
using DeltaApps.CommonLibrary.Helpers;
using DistraidaMente.Controllers;
using DistraidaMente.Controls;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DistraidaMente.Common.Pages
{
    public abstract class SummaryPage : BasePage
    {
        protected class InstructionMessage
        {
            public string Message { get; set; }

            public double FontSize { get; set; }
        }

        protected StackLayout _youHaveChosenContent { get; set; }
        protected Grid _youHaveChosenContentParent { get; set; }
        protected Grid _instructionsContent { get; set; }
        protected Grid _bottomContent { get; set; }
        //protected StackLayout actionContent { get; set; }

        protected string NextActionText { get; set; }

        protected bool HideHasSelectedTitle { get; set; }

        protected bool AnimateYouHaveChosenSectionOnEntry { get; set; }
        protected bool AnimateYouHaveChosenSectionOnBack { get; set; }
        protected bool AnimateYouHaveChosenSectionOnNext { get; set; }

        protected bool AnimateBottomContentOnEntry { get; set; }
        protected bool AnimateBottomContentOnBack { get; set; }
        protected bool AnimateBottomContentOnNext { get; set; }


        //private List<Image> _instructionBullets;

        //protected int CurrentMessage { get; set; }

        //private HtmlLabel _currentMessageLabel { get; set; }
        //private HtmlLabel _tempMessageLabel { get; set; }
        //private Label _messageLabel { get; set; }
        //private Label _tempMessageLabel { get; set; }

        protected double TopHeightProportion { get; set; } = 0.25;
        protected double MiddleHeightProportion { get; set; }
        //protected double MiddleHeightProportion { get; set; }

        private double TopHeight { get; set; }
        private double MiddleHeight { get; set; }
        private double BottomHeight { get; set; }

        public static double ScreenWidth;
        public static double ScreenHeight;

        public SummaryPage(Configuration configuration) : base (configuration)
        {
            Configuration = configuration;

            AnimateYouHaveChosenSectionOnEntry = true;
            AnimateYouHaveChosenSectionOnBack = true;
            AnimateYouHaveChosenSectionOnNext = true;
        }

        protected void CalculateHeights()
        {
            double navigationBarHeight = 100;

            //MiddleHeightProportion = 0.5;

            TopHeight = ScreenHeight * TopHeightProportion;
            MiddleHeight = ScreenHeight * MiddleHeightProportion;
            BottomHeight = ScreenHeight - TopHeight - MiddleHeight;

            if (BottomHeight < navigationBarHeight)
            //if (TopHeight < TopMinHeight || MiddleHeight < MiddleMinHeight)
            {
                TopHeight = (ScreenHeight - navigationBarHeight) * TopHeightProportion;
                MiddleHeight = (ScreenHeight - navigationBarHeight) * MiddleHeightProportion;
                BottomHeight = navigationBarHeight;
            }

            //double totalPortions = TopHeightProportion + MiddleMinHeight + BottomHeightProportion;

            //double portionHeight = App.ScreenHeight / totalPortions;

            //double topHeight = portionHeight * TopHeightProportion;

            //double middleHeight = portionHeight * MiddleMinHeight;

            //if (topHeight < 130 || middleHeight < 500)
            //{
            //    int freePortions = BottomHeightProportion - 2;

            //    BottomHeightProportion = BottomHeightProportion - 2;



            //}
            
            //if (App.ScreenHeight < TopHeightProportion + MiddleHeightProportion)
            //{
            //    TopHeightProportion = App.ScreenHeight * 0.4;
            //    MiddleHeightProportion = App.ScreenHeight * 0.5;
            //}
        }

        protected override Layout SetupContentLayout()
        {
            Grid layout = new Grid()
            {
                Margin = new Thickness(40),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            CalculateHeights();

            layout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(TopHeight, GridUnitType.Absolute) });
            layout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(MiddleHeight, GridUnitType.Absolute) });
            layout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            _youHaveChosenContentParent = SetupYouHaveChoosenContentParentLayout();
                       
            layout.Children.Add(_youHaveChosenContentParent, 0, 0);

            _instructionsContent = SetupInstructionsContentLayout();

            layout.Children.Add(_instructionsContent, 0, 1);

            if (AnimateYouHaveChosenSectionOnEntry)
            {
                _youHaveChosenContentParent.IsVisible = false;
            }

            _instructionsContent.IsVisible = false;

            _bottomContent = SetupBottomContentLayout();

            layout.Children.Add(_bottomContent, 0, 2);

            _bottomContent.IsVisible = false;

            return layout;
        }

        private Grid SetupYouHaveChoosenContentParentLayout()
        {
            Grid youHaveChoosenContentParent = new Grid();

            int index = 0;

            if (!HideHasSelectedTitle)
            {
                youHaveChoosenContentParent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(45, GridUnitType.Absolute) });

                youHaveChoosenContentParent.Children.Add(new AutoFontSizeLabel()
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "Has escogido",
                    Margin = new Thickness(0, 10, 0, 0),
                }, 0, index);

                index++;
            }

            youHaveChoosenContentParent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            _youHaveChosenContent = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            youHaveChoosenContentParent.Children.Add(_youHaveChosenContent, 0, index);
            
            return youHaveChoosenContentParent;
        }
        
        private Grid SetupInstructionsContentLayout()
        {
            Grid instructionsContentLayout = new Grid()
            {
                IsVisible = false,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 10),
                RowSpacing = 10,
            };

            instructionsContentLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Absolute) });
            instructionsContentLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            instructionsContentLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Absolute) });
            //instructionsContentLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });

            instructionsContentLayout.Children.Add(new BoxView()
            {
                HeightRequest = 1,
            }, 0, 0);

            instructionsContentLayout.Children.Add(SetupMessageGrid(), 0, 1);

            instructionsContentLayout.Children.Add(new BoxView()
            {
                HeightRequest = 1,
            }, 0, 2);

            //instructionsContentLayout.Children.Add(SetupBullets(), 0, 3);

            return instructionsContentLayout;
        }

        private Grid SetupBottomContentLayout()
        {
            Grid bottomContentLayout = new Grid()
            {
                IsVisible = false,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                //Margin = new Thickness(0, 20),
                RowSpacing = 20,
            };

            return bottomContentLayout;
        }

        protected abstract Grid SetupMessageGrid();

        //private StackLayout SetupBullets()
        //{
        //    StackLayout bulletsLayout = new StackLayout()
        //    {
        //        Orientation = StackOrientation.Horizontal,
        //        HorizontalOptions = LayoutOptions.Center,
        //        Margin = new Thickness(0, 5),
        //    };

        //    _instructionBullets = new List<Image>();

        //    for (int i = 0; i <= InstructionMessages.Count; i++)
        //    {
        //        string status = i == 0 ? "on" : "off";

        //        Image bullet = FormsHelper.ConfigureImageButton($"bullet{status}.png", (e, s) => { OnBackButtonPressed(); }, new Size(12, 12));

        //        _instructionBullets.Add(bullet);

        //        bulletsLayout.Children.Add(bullet);
        //    }

        //    return bulletsLayout;
        //}

        //private AbsoluteLayout SetupMessageLayout()
        //{
        //    AbsoluteLayout messageLayout = new AbsoluteLayout();

        //    messageLayout.Children.Add(MessageLabel, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

        //    return messageLayout;
        //}

        protected override async Task EntryAnimations()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(base.EntryAnimations());

            if (NavigatingBack)
            {
                tasks.Add(_instructionsContent.AppearLeft(Configuration.AnimationSpeed));
            }
            else
            {
                tasks.Add(_instructionsContent.AppearRight(Configuration.AnimationSpeed));
            }

            if (AnimateBottomContentOnEntry)
            {
                if (NavigatingBack)
                {
                    tasks.Add(_bottomContent.AppearLeft(Configuration.AnimationSpeed));
                }
                else
                {
                    tasks.Add(_bottomContent.AppearRight(Configuration.AnimationSpeed));
                }
            }

            if (AnimateYouHaveChosenSectionOnEntry)
            {
                if (NavigatingBack)
                {
                    tasks.Add(_youHaveChosenContentParent.AppearLeft(Configuration.AnimationSpeed));
                }
                else
                {
                    tasks.Add(_youHaveChosenContentParent.AppearRight(Configuration.AnimationSpeed));
                }
            }

            await Task.WhenAll(tasks);
        }

        //private async Task ConfigureMessage(Func<int, int> incrementFunction, Task[] animations)
        //{
        //    //ConfigureCurrentMessageBullet(false);

        //    CurrentMessage = incrementFunction(CurrentMessage);

        //    //ConfigureCurrentMessageBullet(true);

        //    _tempMessageLabel.FontSize = InstructionMessages[CurrentMessage].FontSize;
        //    _tempMessageLabel.Text = InstructionMessages[CurrentMessage].Message;

        //    _tempMessageLabel.IsVisible = true;

        //    await Task.WhenAll
        //    (
        //        animations
        //    );

        //    _messageLabel.FontSize = InstructionMessages[CurrentMessage].FontSize;
        //    _messageLabel.Text = InstructionMessages[CurrentMessage].Message;

        //    await _messageLabel.TranslateTo(0, 0, 0);

        //    _tempMessageLabel.IsVisible = false;
        //}

        //private async Task ConfigurePreviousMessage()
        //{
        //    Task[] animations = new Task[]
        //    {
        //        _tempMessageLabel.AppearLeft(Configuration.AnimationFastSpeed),
        //        _messageLabel.DisappearRight(Configuration.AnimationFastSpeed)
        //    };

        //    await ConfigureMessage((m) => { return m - 1; }, animations);
        //}

        //private async Task ConfigureNextMessage()
        //{
        //    Task[] animations = new Task[]
        //    {
        //        _tempMessageLabel.AppearRight(Configuration.AnimationFastSpeed),
        //        _messageLabel.DisappearLeft(Configuration.AnimationFastSpeed)
        //    };

        //    await ConfigureMessage((m) => { return m + 1; }, animations);
        //}

        //private void ConfigureCurrentMessageBullet(bool on)
        //{
        //    string status = on ? "on" : "off";

        //    _instructionBullets[CurrentMessage].Source = ImageSource.FromResource($"PositiveThinking.Images.bullet{status}.png");
        //}

        //protected override bool OnBackButtonPressed()
        //{
        //    if (CurrentMessage == 0)
        //    {
        //        return base.OnBackButtonPressed();
        //    }
        //    else
        //    {
        //        if (LockInteraction())
        //        {
        //            ConfigurePreviousMessage().ContinueWith((t) => { ReleaseInteraction(); });
        //        }
        //    }

        //    return true;
        //}

        protected override async Task OnBackButtonPressedAnimations()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(base.OnBackButtonPressedAnimations());
            tasks.Add(_instructionsContent.DisappearRight(Configuration.AnimationSpeed));

            if (AnimateBottomContentOnBack)
            {
                tasks.Add(_bottomContent.DisappearRight(Configuration.AnimationSpeed));
            }

            if (AnimateYouHaveChosenSectionOnBack)
            {
                tasks.Add(_youHaveChosenContentParent.DisappearRight(Configuration.AnimationSpeed));
            }

            await Task.WhenAll(tasks);
        }

        //protected override bool OnNextButtonPressed()
        //{
        //    if (CurrentMessage == InstructionMessages.Count - 1)
        //    {
        //        return base.OnNextButtonPressed();
        //    }
        //    else
        //    {
        //        if (LockInteraction())
        //        {
        //            ConfigureNextMessage().ContinueWith((t) => { ReleaseInteraction(); });
        //        }
        //    }

        //    return true;
        //}

        protected override async Task OnNextButtonPressedAnimations()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(base.OnNextButtonPressedAnimations());
            tasks.Add(_instructionsContent.DisappearLeft(Configuration.AnimationSpeed));

            if (AnimateBottomContentOnNext)
            {
                tasks.Add(_bottomContent.DisappearLeft(Configuration.AnimationSpeed));
            }

            if (AnimateYouHaveChosenSectionOnNext)
            {
                tasks.Add(_youHaveChosenContentParent.DisappearLeft(Configuration.AnimationSpeed));
            }

            await Task.WhenAll(tasks);
        }
    }
}

