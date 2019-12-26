using DistraidaMente.Helpers;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public delegate void ClosedHandler ();

    public class Dialog : Grid
    {
        public event ClosedHandler Closed;

        public static readonly BindableProperty DialogBackgroundColorProperty = BindableProperty.Create(nameof(DialogBackgroundColor), typeof(Color), typeof(Dialog), Color.White);

        private Color _dialogBackgroundColor;

        public Color DialogBackgroundColor
        {
            get { return _dialogBackgroundColor; }
            set
            {
                _dialogBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(DialogSize), typeof(Size), typeof(Dialog), Size.Zero);

        private Size _dialogSize;

        public Size DialogSize
        {
            get { return _dialogSize; }
            set
            {
                _dialogSize = value;
                OnPropertyChanged();
            }
        }

        public static readonly BindableProperty DialogHeightProperty = BindableProperty.Create(nameof(DialogHeight), typeof(double), typeof(Dialog), 0.0);

        private double _dialogHeight;

        public double DialogHeight
        {
            get { return _dialogHeight; }
            set
            {
                _dialogHeight = value;
                OnPropertyChanged();
            }
        }

        public static readonly BindableProperty EnableCloseButtonProperty = BindableProperty.Create(nameof(EnableCloseButton), typeof(bool), typeof(Dialog), true);

        private bool _enableCloseButton = true;

        public bool EnableCloseButton
        {
            get { return _enableCloseButton; }
            set
            {
                _enableCloseButton = value;
                OnPropertyChanged();
            }
        }

        public static readonly BindableProperty ShowOverlayProperty = BindableProperty.Create(nameof(ShowOverlay), typeof(bool), typeof(Dialog), true);

        private bool _showOverlay = true;

        public bool ShowOverlay
        {
            get { return _showOverlay; }
            set
            {
                _showOverlay = value;
                OnPropertyChanged();
            }
        }

        public static readonly BindableProperty DialogVerticalAlignmentProperty = BindableProperty.Create(nameof(DialogVerticalAlignment), typeof(LayoutOptions), typeof(Dialog), LayoutOptions.CenterAndExpand);

        private LayoutOptions _dialogVerticalAlignment = LayoutOptions.CenterAndExpand;

        public LayoutOptions DialogVerticalAlignment
        {
            get { return _dialogVerticalAlignment; }
            set
            {
                _dialogVerticalAlignment = value;
                OnPropertyChanged();
            }
        }


        public static readonly BindableProperty DialogMarginProperty = BindableProperty.Create(nameof(DialogMargin), typeof(Thickness), typeof(Dialog));

        private Thickness _dialogMargin;

        public Thickness DialogMargin
        {
            get { return _dialogMargin; }
            set
            {
                _dialogMargin = value;
                OnPropertyChanged();
            }
        }
               
        Grid DialogContent { get; set; }

        public View Content
        {
            set
            {
                DialogContent.Children.Add(value);
            }
        }

        public int AutoCloseTime { get; set; }

        public Dialog(Size size)
        {
            BackgroundColor = Color.Transparent;

            BoxView overlay = new BoxView()
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BindingContext = this,
            };

            overlay.SetBinding(BoxView.IsVisibleProperty, nameof(ShowOverlay));

            Children.Add(overlay);

            StackLayout content = new StackLayout()
            {
                Padding = new Thickness(40, 0, 40, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,

                WidthRequest = size.Width,
                HeightRequest = size.Height,
                BindingContext = this,
            };

            content.SetBinding(StackLayout.VerticalOptionsProperty, nameof(DialogVerticalAlignment));
            content.SetBinding(StackLayout.MarginProperty, nameof(DialogMargin));

            //content.SetBinding(StackLayout.WidthRequestProperty, nameof(DialogSize.Width));
            //content.SetBinding(StackLayout.HeightRequestProperty, nameof(DialogHeight));

            Frame dialogFrame = new Frame()
            {
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = true,
                BindingContext = this,
            };

            dialogFrame.SetBinding(Frame.BackgroundColorProperty, nameof(DialogBackgroundColor));

            AbsoluteLayout dialogContentLayout = new AbsoluteLayout()
            {
            };

            DialogContent = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            dialogContentLayout.Children.Add(DialogContent, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            //Image itemClose = FormsHelper.ConfigureImageButton("close.png", (e, s) => { Close(); }, new Size(20, 20), true, Assembly.GetCallingAssembly());

            //itemClose.SetBinding(Image.IsVisibleProperty, nameof(EnableCloseButton));
            //itemClose.BindingContext = this;

            //dialogContentLayout.Children.Add(itemClose, new Rectangle(1, 0, 20, 20), AbsoluteLayoutFlags.PositionProportional);

            dialogFrame.Content = dialogContentLayout;

            content.Children.Add(dialogFrame);

            Children.Add(content);

            IsVisible = false;
        }

        public async Task Show()
        {
            IsVisible = true;
            (Parent as Layout)?.RaiseChild(this);

            if (AutoCloseTime > 0)
            {
                await Task.Delay(AutoCloseTime).ContinueWith((t) => Hide());
            }
        }

        public void Close()
        {
            Hide();

            Closed?.Invoke();
        }

        public void Hide()
        {
            Device.BeginInvokeOnMainThread(() => { IsVisible = false; });            
        }
    }
}
