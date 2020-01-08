using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace UOCApps.CommonLibrary.Model
{
    public class Theme
    {
        public Color TextColor { get; set; }

        public Color SelectedTextColor { get; set; }

        public Color BackgroundColor { get; set; }

        public Color SecondaryBackgroundColor { get; set; }

        public Color SelectedBackgroundColor { get; set; }

        public double SmallFontSize { get; set; }
        public double MediumFontSize { get; set; }
        public double BigFontSize { get; set; }
        public double ExtraBigFontSize { get; set; }
    }
}
