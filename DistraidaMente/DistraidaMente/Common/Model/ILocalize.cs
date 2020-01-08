using DistraidaMente.Common.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UOCApps.CommonLibrary.Model
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale();

        void SetLocale(Language language);
    }
}
