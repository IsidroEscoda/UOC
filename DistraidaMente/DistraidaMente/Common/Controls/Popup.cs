using DistraidaMente.Helpers;
using DistraidaMente.Model;
using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public class Popup : Grid
    {
        public class Style
        {
            public double FontSize { get; set; }
            public Color BackgroundColor { get; set; }
            public Color TextColor { get; set; }
        }

        public Popup(string message, Style configuration)
        {
            BoxView overlay = new BoxView()
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            Children.Add(overlay);

            StackLayout content = new StackLayout()
            {
                Padding = new Thickness(40, 100, 40, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            Frame itemFrame = new Frame()
            {
                CornerRadius = 10,
                BackgroundColor = configuration.BackgroundColor, 
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = true,
            };

            Grid itemGrid = new Grid()
            {

            };

            itemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Absolute) });
            
            Label itemLabel = new Label()
            {
                FontSize = configuration.FontSize, 
                Text = message,
                TextColor = configuration.TextColor, 
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            itemGrid.Children.Add(itemLabel, 0, 0);

            //Image itemClose = FormsHelper.ConfigureImageButton("close", (e, s) => { this.IsVisible = false; }, new Size(16, 16));

            //itemClose.VerticalOptions = LayoutOptions.Start;

            //itemGrid.Children.Add(itemClose, 1, 0);

            itemFrame.Content = itemGrid;

            content.Children.Add(itemFrame);

            Children.Add(content);

            IsVisible = false;
        }

        public void Show()
        {
            IsVisible = true;
            (Parent as Layout)?.RaiseChild(this);
        }
    }
}
