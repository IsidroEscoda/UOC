using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using DistraidaMente.Model;
using DistraidaMente.Controllers;
using DistraidaMente.Controls;
using DistraidaMente.Helpers;
using DistraidaMente.Common.Helpers;
using DistraidaMente.Models;

namespace DistraidaMente.Common.Pages
{
    public delegate void EmotionalStatusSelectedHandler(EmotionalStatus emotionalStatus);

    public class SelectEmotionalStatusPage : BasePage
    {
        public double EmoticonOffset { get; set; }

        public static Size EmotionalIconSize { get; set; }

        private class EmotionalStateIcon
        {
            public EmotionalStatus EmotionalStatus { get; set; }
            public Image Image { get; set; }
        }

        public event EmotionalStatusSelectedHandler EmotionalStatusSelected;

        private char set;

        private IEnumerable<EmotionalStatus> order;

        private Grid _titleLayout;

        private Grid _emotionalIconsLayout;

        private List<EmotionalStateIcon> _emotionalStatesIcons;

        private string _titleText;

        private bool _moveSelectedStatusToTop;

        public SelectEmotionalStatusPage(Configuration configuration, string message, char set, IEnumerable<EmotionalStatus> order, bool moveSelectedStatusToTop = false) : base(configuration)
        {
            this.set = set;
            this.order = order;

            _titleText = message;

            _moveSelectedStatusToTop = moveSelectedStatusToTop;
        }

        private const double _mainLayoutMargin = 20;

        protected override Layout SetupContentLayout()
        {
            StackLayout layout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(_mainLayoutMargin),
                Spacing = 10,
            };

            _titleLayout = SetupTitleLayout();

            layout.Children.Add(_titleLayout);

            _emotionalIconsLayout = SetupEmoticonsLayout();

            layout.Children.Add(_emotionalIconsLayout);

            _titleLayout.IsVisible = false;
            _emotionalIconsLayout.IsVisible = false;

            return layout;
        }

        private Grid SetupTitleLayout()
        {
            Grid titleLayout = new Grid()
            {
                HeightRequest = 130,
            };

            titleLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            titleLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            titleLayout.Children.Add(new AutoFontSizeLabel()
            {
                //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                //FontSize = Configuration.Theme.BigFontSize,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                //TextColor = Configuration.Theme.TextColor,
                Text = _titleText,
            }, 0, 0);

            AutoFontSizeFormattedTextLabel subtitle = new AutoFontSizeFormattedTextLabel()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            subtitle.FormattedText = new FormattedString();

            subtitle.FormattedText.Spans.Add(new Span()
            {
                Text = "Selecciona",
                //TextColor = Configuration.Theme.TextColor,
                FontAttributes = FontAttributes.Bold,
            });

            subtitle.FormattedText.Spans.Add(new Span()
            {
                Text = " la opción que más se acerca a tu estado de ánimo",
                //TextColor = Configuration.Theme.TextColor,
            });

            titleLayout.Children.Add(subtitle, 0, 1);

            //titleLayout.Children.Add(new HtmlLabel()
            //{
            //    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            //    //FontSize = Configuration.Theme.MediumFontSize,
            //    HorizontalTextAlignment = TextAlignment.Center,
            //    TextColor = Configuration.Theme.TextColor,
            //    Text = "<b>Selecciona</b> la opción que<br/>más se acerca a tu estado de ánimo",
            //}, 0, 1);

            return titleLayout;
        }

        private Grid SetupEmoticonsLayout()
        {
            _emotionalStatesIcons = new List<EmotionalStateIcon>();

            Grid iconsLayout = new Grid()
            {
                Margin = new Thickness(0, 10),
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            iconsLayout.ColumnDefinitions.Add(new ColumnDefinition());

            int row = 0;

            foreach (EmotionalStatus es in order)
            {
                iconsLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                Image icon = ConfigureEmotionalStatusButton(es, set);

                iconsLayout.Children.Add(icon, 0, row);

                _emotionalStatesIcons.Add(new EmotionalStateIcon() { EmotionalStatus = es, Image = icon });

                row++;
            }

            return iconsLayout;
        }

        private Image ConfigureEmotionalStatusButton(EmotionalStatus emotionalStatus, char set, Size? size = null)
        {
            Image imageButton = FormsHelper.ConfigureImageButton($"{ emotionalStatus.ToString().ToLower() }.png", (s, e) => { var t = OnEmotionalStatusSelected(emotionalStatus); }, size);

            imageButton.WidthRequest = 100;
            imageButton.HorizontalOptions = LayoutOptions.Center;
            imageButton.VerticalOptions = LayoutOptions.FillAndExpand;
            imageButton.Aspect = Aspect.AspectFit;

            return imageButton;
        }

        private async Task OnEmotionalStatusSelected(EmotionalStatus selectedEmotionalStatus)
        {
            //await OnEmotionalStatusSelectedAnimations(selectedEmotionalStatus).ContinueWith((t) => { Device.BeginInvokeOnMainThread(() => { EmotionalStatusSelected?.Invoke(selectedEmotionalStatus); }); });

            foreach (EmotionalStateIcon esi in _emotionalStatesIcons)
            {
                esi.Image.IsEnabled = false;
            }

            await OnEmotionalStatusSelectedAnimations(selectedEmotionalStatus).ContinueWith((t) => 
            {
                //await Task.Delay(1);

                Task.Run(() => Device.BeginInvokeOnMainThread(() => { EmotionalStatusSelected?.Invoke(selectedEmotionalStatus); }));
                
                //Device.BeginInvokeOnMainThread(() => { EmotionalStatusSelected?.Invoke(selectedEmotionalStatus); });
            });
        }

        private async Task OnEmotionalStatusSelectedAnimations(EmotionalStatus selectedEmotionalStatus)
        {
            if (_moveSelectedStatusToTop)
            {
                var notSelected = _emotionalStatesIcons.Where(x => x.EmotionalStatus != selectedEmotionalStatus);

                var selected = _emotionalStatesIcons.First(x => x.EmotionalStatus == selectedEmotionalStatus);

                await Task.WhenAll
                (
                    notSelected.Select(x => x.Image.FadeTo(0, Configuration.AnimationSpeed, Easing.SinIn))
                );

                double imageY = _emotionalIconsLayout.Y + _mainLayoutMargin + ((selected.Image.Height + _emotionalIconsLayout.RowSpacing) * (int)selectedEmotionalStatus);

                int distanceToTop = -(int)Math.Ceiling(imageY - (130 + EmoticonOffset));

                EmotionalIconSize = new Size(selected.Image.Width, selected.Image.Height);

                await Task.WhenAll
                (
                    _titleLayout.DisappearLeft(Configuration.AnimationSpeed),
                    selected.Image.TranslateTo(0, distanceToTop, Configuration.AnimationSpeed, Easing.CubicInOut)
                );
            }
            else
            {
                await Task.WhenAll
                (
                    _titleLayout.DisappearLeft(Configuration.AnimationSpeed),
                    _emotionalIconsLayout.DisappearLeft(Configuration.AnimationSpeed)
                );
            }
        }

        protected override async Task EntryAnimations()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(base.EntryAnimations());

            if (NavigatingBack)
            {
                tasks.Add(_titleLayout.AppearLeft(Configuration.AnimationSpeed));
            }
            else
            {
                tasks.Add(_titleLayout.AppearRight(Configuration.AnimationSpeed));
            }

            tasks.Add(_emotionalIconsLayout.AppearBottom(Configuration.AnimationSpeed));

            await Task.WhenAll(tasks);
        }

        protected override async Task OnBackButtonPressedAnimations()
        {
            await Task.WhenAll
            (
                _titleLayout.DisappearRight(Configuration.AnimationSpeed),
                _emotionalIconsLayout.DisappearBottom(Configuration.AnimationSpeed)
            );
        }
    }
}