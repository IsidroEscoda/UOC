using System.Collections.Generic;
using Xamarin.Forms;

namespace DistraidaMente.Controls
{
    public class AutoFontSizeFormattedTextLabel : Label
    {
        public double MinFontSize { get; set; } = 1;
        public double MaxFontSize { get; set; } = 80;


        public AutoFontSizeFormattedTextLabel()
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
            double minFontSize = MinFontSize;
            double maxFontSize = MaxFontSize;
            double fontSize = 0;

            LineBreakMode = LineBreakMode.WordWrap;

            List<string> text = new List<string>();

            foreach (Span s in FormattedText.Spans)
            {
                text.Add(s.Text);

                s.Text = s.Text + "...";
            }
            
            double height = 0;

            while (maxFontSize - minFontSize > 1) // && height <= Height)
            {
                fontSize = (maxFontSize + minFontSize) / 2;

                FontSize = fontSize;

                foreach (Span s in FormattedText.Spans)
                {
                    s.FontSize = fontSize;
                }

                SizeRequest sizeRequestFixedWidth = Measure(Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);

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

            int index = 0;

            foreach (Span s in FormattedText.Spans)
            {
                s.FontSize = fontSize - 2;
                s.Text = text[index];
                index++;
            }
        }

        public void AutoSizeFontAdjust_OLD()
        {
            //double minFontSize = 1;
            double maxFontSize = 80;
            double fontSize = 1;

            LineBreakMode = LineBreakMode.WordWrap;

            List<string> text = new List<string>();

            foreach (Span s in FormattedText.Spans)
            {
                text.Add(s.Text);

                s.Text = s.Text + "...";
            }

            double height = 0;

            while (fontSize <= maxFontSize && height <= Height)
            {
                FontSize = fontSize;

                foreach (Span s in FormattedText.Spans)
                {
                    s.FontSize = fontSize;
                }

                SizeRequest sizeRequestFixedWidth = Measure(Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);

                height = sizeRequestFixedWidth.Request.Height;

                fontSize++;
            }

            int index = 0;

            foreach (Span s in FormattedText.Spans)
            {
                s.FontSize = fontSize - 2;
                s.Text = text[index];
                index++;
            }
        }
    }
}