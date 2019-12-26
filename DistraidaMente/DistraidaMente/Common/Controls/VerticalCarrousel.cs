using DistraidaMente.Controls;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public class VerticalCarrousel : ScrollView
    {
        Dictionary<string, Frame> buttons;
        Dictionary<string, Label> labels;

        private List<TextItem> _selectedItems;

        public delegate void SelectedCountChangedHandler(int count);

        public event SelectedCountChangedHandler SelectedCountChanged;

        private Model _model;
        private Style _style;
                
        public class Model
        {
            public List<TextItem> Items { get; set; }
            public Action<TextItem> Action { get; set; }
            public bool MultiSelect { get; set; }
            public List<TextItem> SelectedItems { get; set; }
            public SelectedCountChangedHandler SelectedCountChangedHandler { get; set; }
        }

        public class Style
        {
            public Color TextColor { get; set; }
            public Color BackgroundColor { get; set; }
            public Color SelectedTextColor { get; set; }
            public Color SelectedBackgroundColor { get; set; }
            public double Height { get; set; } = -1;
            public double FontSize { get; set; } = -1;
        }

        public VerticalCarrousel(Model model, Style style)
        {
            _model = model;
            _style = style;

            StackLayout itemsLayout = new StackLayout()
            {
                Spacing = 10,
                Padding = new Thickness(10),
            };

            buttons = new Dictionary<string, Frame>();
            labels = new Dictionary<string, Label>();

            foreach (TextItem ti in _model.Items)
            {
                itemsLayout.Children.Add(SetupItemButton(ti));
            }

            Content = itemsLayout;

            if (_model.SelectedCountChangedHandler != null)
            {
                SelectedCountChanged = _model.SelectedCountChangedHandler;
            }

            _selectedItems = new List<TextItem>();

            if (_model.SelectedItems != null)
            {
                foreach (TextItem ti in _model.SelectedItems)
                {
                    ToggleItem(ti);
                }
            }
        }

        private View SetupItemButton(TextItem ti)
        {
            //Forms9Patch.Frame itemFrame = new Forms9Patch.Frame()
            Frame itemFrame = new Frame()
            {
                CornerRadius = 10,
                BackgroundColor = _style.BackgroundColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = true,
            };

            Label itemLabel = null;

            if (_style.Height > 0)
            {
                itemFrame.HeightRequest = _style.Height;
                itemLabel = new AutoFontSizeLabel();
            }
            else if (_style.FontSize > 0)
            {
                itemLabel = new Label() { FontSize = _style.FontSize };
            }
            else
            {
                itemLabel = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            }

            itemLabel.Text = ti.Text;
            //FontAttributes = FontAttributes.Bold,
            //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            //FontSize = fontSize < 0 ? Configuration.Theme.MediumFontSize : fontSize,
            itemLabel.TextColor = _style.TextColor;
            itemLabel.VerticalTextAlignment = TextAlignment.Center;
            itemLabel.HorizontalTextAlignment = TextAlignment.Center;
            itemLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
            itemLabel.VerticalOptions = LayoutOptions.FillAndExpand;

            itemFrame.Content = itemLabel;

            //var effect = Effect.Resolve("PositiveThinking.ViewShadowEffect");

            //itemFrame.Effects.Add(new ShadowEffect
            //{
            //    Radius = 2,
            //    Color = Color.Red,
            //    DistanceX = 2,
            //    DistanceY = 2
            //});

            buttons.Add(ti.Text, itemFrame);
            labels.Add(ti.Text, itemLabel);

            if (_model.Action != null) { itemFrame.AddTapRecognizer((s, e) => { ToggleItem(ti); _model.Action(ti); }); }

            if (_model.MultiSelect) { itemFrame.AddTapRecognizer((s, e) => { ToggleItem(ti); }); }

            return itemFrame;
        }

        private void ToggleItem(TextItem ti)
        {
            Frame b = buttons[ti.Text];
            Label l = labels[ti.Text];

            if (_selectedItems.Contains(ti))
            {
                _selectedItems.Remove(ti);

                l.TextColor = _style.TextColor;
                b.BackgroundColor = _style.BackgroundColor;
            }
            else
            {
                _selectedItems.Add(ti);

                l.TextColor = _style.SelectedTextColor;
                b.BackgroundColor = _style.SelectedBackgroundColor;
            }

            SelectedCountChanged?.Invoke(_selectedItems.Count);
        }

        public List<TextItem> SelectedItems { get { return _selectedItems; } }
    }
}