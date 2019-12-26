using System;
using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public class FrameButton : Frame
    {
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(FrameButton), 0.0);

        private double _fontSize;

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged();
            }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(FrameButton), Color.Black);

        private Color _textColor;

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }

        public FrameButton(string text, EventHandler onTapHandler = null)
        {
            Padding = new Thickness(5);
            CornerRadius = 5;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            HasShadow = true;

            Label itemLabel = new Label()
            {
                //FontSize = _style.FontSize,

                Text = text,
                //FontAttributes = FontAttributes.Bold,
                //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                //FontSize = fontSize < 0 ? Configuration.Theme.MediumFontSize : fontSize,
                //TextColor = _style.TextColor,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BindingContext = this,
            };

            itemLabel.SetBinding(Label.TextColorProperty, nameof(TextColor));
            itemLabel.SetBinding(Label.FontSizeProperty, nameof(FontSize));

            Content = itemLabel;

            if (onTapHandler != null)
            {
                //this.AddTapRecognizer(onTapHandler);
            }
        }
    }
}
