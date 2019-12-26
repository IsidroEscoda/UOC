using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaApps.CommonLibrary.Helpers
{ 
    public static class FontFamily
    {
        public static string OpenSansBold
        {
            get
            {
#if __ANDROID__
                return "OpenSans-Bold.ttf#OpenSans-Bold";
#elif WINDOWS_UWP
                return "/Assets/OpenSans-Bold.ttf#OpenSans-Bold";
#else // __IOS__
                return "OpenSans-Bold";
#endif
            }
        }

        public static string OpenSansItalic
        {
            get
            {
#if __ANDROID__
                return "OpenSans-Italic.ttf#OpenSans-Italic";
#elif WINDOWS_UWP
                return "/Assets/OpenSans-Italic.ttf#OpenSans-Italic";
#else // __IOS__
                return "OpenSans-Italic";
#endif
            }
        }

        public static string OpenSansRegular
        {
            get
            {
#if __ANDROID__
                return "OpenSans-Regular.ttf#OpenSans-Regular";
#elif WINDOWS_UWP
                return "/Assets/OpenSans-Regular.ttf#OpenSans-Regular";
#else // __IOS__
                return "OpenSans-Regular";
#endif
            }
        }
    }
}
