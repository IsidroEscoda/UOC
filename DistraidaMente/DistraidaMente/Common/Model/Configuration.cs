using DeltaApps.CommonLibrary.Model;
using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DistraidaMente.Common.Model
{
    public class Configuration
    {
        public string ServerUrl = "https://positiveserver.azurewebsites.net/api/";
        public string ServerUser = "positiveA4bxEs";
        public string ServerPassword = "dLmVNgYg72xdUnFW";

        public readonly uint AnimationSpeed = 1000;

        public Theme Theme { get; set; }

        public Configuration()
        {
            Language = Language; // this instruction changes Current thread language
        }

        public bool FirstLaunch
        {
            get { return ReadBoolProperty(nameof(FirstLaunch), true); }

            set { SaveBoolProperty(nameof(FirstLaunch), value); }
        }

        public string UserId
        {
            get { return ReadProperty(nameof(UserId)); }

            set { SaveProperty(nameof(UserId), value); }
        }

        public Language Language
        {
            get
            {
                var language = ReadEnumProperty<Language>(nameof(Language));

                if (language == null)
                {
                    CultureInfo ci = DependencyService.Get<ILocalize>()?.GetCurrentCultureInfo();
                    /*if (ci.Name == "ca-ES")
                    {
                        language = Language.CA;
                    }
                    else if (ci.Name.StartsWith("en"))
                    {
                        language = Language.EN;
                    }
                    else
                    {
                        language = Language.ES;
                    }*/
                    language = Language.ES;

                    SaveEnumProperty(nameof(Language), language.Value);
                }

                return language.Value;
            }

            set
            {
                SaveProperty(nameof(Language), value.ToString());

                //DependencyService.Get<ILocalize>().SetLocale(value);
            }
        }
                     
        public string ReadProperty(string propertyName)
        {
            return Application.Current.Properties.ContainsKey(propertyName) ? (string)Application.Current.Properties[propertyName] : null;
            // return SecureStorage.GetAsync(propertyName).Result;
        }

        public void SaveProperty(string propertyName, string propertyValue)
        {
            Application.Current.Properties[propertyName] = propertyValue;
            Application.Current.SavePropertiesAsync();
            //_ = SecureStorage.SetAsync(propertyName, propertyValue);
        }

        public bool ReadBoolProperty(string propertyName, bool defaultValue = false)
        {
            var b = ReadProperty(propertyName);

            if (bool.TryParse(b, out bool result))
            {
                return result;
            }

            return defaultValue;
        }

        public void SaveBoolProperty(string propertyName, bool propertyValue)
        {
            SaveProperty(propertyName, propertyValue.ToString());
        }

        public T? ReadEnumProperty<T>(string propertyName, T? defaultValue = null) where T : struct, IConvertible
        {
            var p = ReadProperty(propertyName);

            if (Enum.TryParse(p, out T result))
            {
                return result;
            }

            return defaultValue;
        }

        public void SaveEnumProperty<T>(string propertyName, T propertyValue) where T : struct, IConvertible
        {
            SaveProperty(propertyName, propertyValue.ToString());
        }

        public const string DateFormat = "yyyyMMdd - hh:mm:ss";

        public DateTime? ReadDateTimeProperty(string propertyName, DateTime? defaultValue = null)
        {
            var p = ReadProperty(propertyName);

            if (DateTime.TryParseExact(p, DateFormat, CultureInfo.InvariantCulture,DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return defaultValue;
        }

        public void SaveDateTimeProperty(string propertyName, DateTime propertyValue)
        {
            SaveProperty(propertyName, propertyValue.ToString(DateFormat));
        }

        //public T ReadProperty<T>(string propertyName)
        //{
        //    return Application.Current.Properties.ContainsKey(propertyName) ? (T)Application.Current.Properties[propertyName] : default(T);
        //}

        //public T ReadProperty<T>(string propertyName, T defaultValue)
        //{
        //    return Application.Current.Properties.ContainsKey(propertyName) ? (T)Application.Current.Properties[propertyName] : defaultValue;
        //}

        //public void SaveProperty<T>(string propertyName, T value)
        //{
        //    Application.Current.Properties[propertyName] = value;
        //    Application.Current.SavePropertiesAsync();
        //}
    }
}
