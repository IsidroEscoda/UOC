using DeltaApps.CommonLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using DistraidaMente.Controls;
using DistraidaMente.Controllers;
using DistraidaMente.Model;
using DistraidaMente.Helpers;
using DistraidaMente.Models;

namespace DistraidaMente.Common.Pages
{
    public class EndProcessSummaryPage : SummaryPage
    {
        EmotionalStatus _emotionalStatus;
        Image emotionalIcon;

        public event ButtonPressedHandler StartAgainPressed;
        public event ButtonPressedHandler ExitPressed;

        bool useSelectedColor;

        public EndProcessSummaryPage(Configuration configuration, EmotionalStatus emotionalStatus, bool useSelectedColor = false) : base(configuration)
        {
            _emotionalStatus = emotionalStatus;

            MiddleHeightProportion = 0.5; // 350;

            EnableBackButton = false;
            EnableNextButton = false;

            this.useSelectedColor = useSelectedColor;
        }

        protected override Layout SetupContentLayout()
        {
            //emotionalIcon = FormsHelper.ConfigureImageButton($"{ _emotionalStatus.ToString().ToLower() }.png", null, SelectEmotionalStatusPage.EmotionalIconSize); // new Size(85, 85));

            //_mainLayout.Children.Add(emotionalIcon, new Rectangle(0.5, 130, SelectEmotionalStatusPage.EmotionalIconSize.Width, SelectEmotionalStatusPage.EmotionalIconSize.Height), AbsoluteLayoutFlags.XProportional);

            return base.SetupContentLayout();
        }

        //protected override void OnAppearing()
        //{
        //    emotionalIcon = FormsHelper.ConfigureImageButton($"{ _emotionalStatus.ToString().ToLower() }.png", null, SelectEmotionalStatusPage.EmotionalIconSize); // new Size(85, 85));

        //    //emotionalIcon.VerticalOptions = LayoutOptions.Center;

        //    //emotionalIcon.Margin = new Thickness(0, 20, 0, 0);

        //    //_youHaveChosenContent.Children.Add(emotionalIcon);

        //    //_mainLayout.Children.Add(emotionalIcon, new Rectangle(0.5, 130, 87.7143, 87.7143), AbsoluteLayoutFlags.XProportional);

        //    _mainLayout.Children.Add(emotionalIcon, new Rectangle(0.5, 130, SelectEmotionalStatusPage.EmotionalIconSize.Width, SelectEmotionalStatusPage.EmotionalIconSize.Height), AbsoluteLayoutFlags.XProportional);

        //    base.OnAppearing();
        //}

        protected override Grid SetupMessageGrid()
        {
            Grid messageGrid = new Grid()
            {
                RowSpacing = 20,
            };

            messageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            messageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80, GridUnitType.Absolute) });

            AutoFontSizeFormattedTextLabel messageLabel = new AutoFontSizeFormattedTextLabel()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                //TextColor = Configuration.Theme.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            messageLabel.FormattedText = new FormattedString();

            if (_emotionalStatus == EmotionalStatus.Happy || _emotionalStatus == EmotionalStatus.VeryHappy)
            {
                messageLabel.FormattedText.Spans.Add(new Span() { Text = "Genial, ", FontAttributes = FontAttributes.Bold });
                messageLabel.FormattedText.Spans.Add(new Span() { Text = "nos alegramos de que te sientas bien." + Environment.NewLine + Environment.NewLine + "Vuelve a ejecutar el proceso cuando lo necesites." });
            }
            else
            {
                messageLabel.FormattedText.Spans.Add(new Span() { Text = "Te recomendamos que vuelvas a intentar el proceso a ver si puedes sentirte un poquito mejor" });
            }

            messageGrid.Children.Add(messageLabel, 0, 0);
                       
            Frame itemFrame = new Frame()
            {
                CornerRadius = 10,
                //BackgroundColor = useSelectedColor ? Configuration.Theme.SelectedBackgroundColor : Configuration.Theme.SecondaryBackgroundColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = true,
                Margin = new Thickness(10, 0),
            };

            Label itemLabel = new Label()
            {
                //FontSize = Configuration.Theme.MediumFontSize,
                Text = "Volver a empezar",
                //TextColor = useSelectedColor ? Configuration.Theme.SelectedTextColor : Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
                        
            itemFrame.Content = itemLabel;

            itemFrame.AddTapRecognizer((e, s) => { StartAgainPressed?.Invoke(); });

            Frame itemFrame2 = new Frame()
            {
                CornerRadius = 10,
                //BackgroundColor = useSelectedColor ? Configuration.Theme.SelectedBackgroundColor : Configuration.Theme.SecondaryBackgroundColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = true,
                Margin = new Thickness(10, 0),
            };

            Label itemLabel2 = new Label()
            {
                //FontSize = Configuration.Theme.MediumFontSize,
                Text = "Salir",
                //TextColor = useSelectedColor ? Configuration.Theme.SelectedTextColor : Configuration.Theme.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            itemFrame2.Content = itemLabel2;

            itemFrame2.AddTapRecognizer((e, s) => { ExitPressed?.Invoke(); });

            messageGrid.Children.Add(itemFrame, 0, 1);
            messageGrid.Children.Add(itemFrame2, 0, 2);

            return messageGrid;
        }
    }
}