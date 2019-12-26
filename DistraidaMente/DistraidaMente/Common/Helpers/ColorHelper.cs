using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DeltaApps.CommonLibrary.Helpers
{
    public static class ColorHelper
    {
        private static Random _randomizer;

        private static Color[] _colors = new Color[]
        {
            Color.LightSkyBlue,
            Color.LightGreen,
            Color.LightYellow,
            Color.LightSalmon,
            Color.LightCoral,
        };

        public static Color RandomColor 
        {
            get
            {
                if (_randomizer == null)
                {
                    _randomizer = new Random();
                }

                int colorIndex = _randomizer.Next(0, _colors.Length);
                
                return _colors[colorIndex];
            }
        }
    }
}
