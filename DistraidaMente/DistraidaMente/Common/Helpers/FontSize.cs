using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DeltaApps.CommonLibrary.Helpers
{
    public static class FontSize
    {
        public static double Micro
        {
            get
            {
                //return 12;
                return Device.GetNamedSize(NamedSize.Micro, typeof(Label));
            }
        }

        public static double Small
        {
            get
            {
                //return 15;
                return Device.GetNamedSize(NamedSize.Small, typeof(Label));
            }
        }

        public static double Default
        {
            get
            {
                //return 17;
                return Device.GetNamedSize(NamedSize.Default, typeof(Label));
            }
        }

        public static double Medium
        {
            get
            {
                //return 19;
                return Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            }
        }

        public static double Large
        {
            get
            {
                //return 24;
                return Device.GetNamedSize(NamedSize.Large, typeof(Label));
            }
        }

        public static double ExtraLarge
        {
            get
            {
                //return 26;
                return Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 2;
            }
        }

        public static double Huge
        {
            get
            {
                //return 28; 
                return Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 4;
            }
        }
    }
}
