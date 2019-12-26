using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public class AutoFontSizeLabel : Label
    {
        public double MinFontSize { get; set; } = 1;
        public double MaxFontSize { get; set; } = 80;

        public AutoFontSizeLabel()
        {
            //BackgroundColor = new Color(RandomHelper.RandomInt(0, 255), RandomHelper.RandomInt(0, 255), RandomHelper.RandomInt(0, 255));
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            AutoSizeFontAdjust();
        }

        public void AutoSizeFontAdjust()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return;
            }

            double minFontSize = MinFontSize;
            double maxFontSize = MaxFontSize;
            double fontSize = 0;

            LineBreakMode = LineBreakMode.WordWrap;

            string text = Text;

            Text = text + "....";

            double height = 0;

            while (maxFontSize - minFontSize > 1) // && height <= Height)
            {
                fontSize = (maxFontSize + minFontSize) / 2;

                FontSize = fontSize;

                SizeRequest sizeRequestFixedWidth = Measure(Width, double.PositiveInfinity);

                height = sizeRequestFixedWidth.Request.Height;

                if (height >= Height)
                {
                    maxFontSize = fontSize;
                }
                else
                {
                    minFontSize = fontSize;
                }

                //fontSize++;
            }

            Text = text;

            FontSize = fontSize - 1;
        }

        public void AutoSizeFontAdjust_OLD()
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return;
            }

            //double minFontSize = 1;
            double maxFontSize = 80;
            double fontSize = 1;

            LineBreakMode = LineBreakMode.WordWrap;

            string text = Text;

            Text = text + "....";

            double height = 0;

            while (fontSize <= maxFontSize && height <= Height)
            {
                FontSize = fontSize;

                SizeRequest sizeRequestFixedWidth = Measure(Width, double.PositiveInfinity);

                height = sizeRequestFixedWidth.Request.Height;

                fontSize++;
            }

            Text = text;

            FontSize = fontSize - 1;
        }
    }
}