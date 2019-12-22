using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DistraidaMente.Controllers
{
    public class Configuration
    {
        public readonly uint AnimationSpeed = 1000;

        public Configuration()
        {
        }

        public bool FirstLaunch
        {
            get { return ReadBoolProperty(nameof(FirstLaunch), true); }

            set { SaveBoolProperty(nameof(FirstLaunch), value); }
        }

        public string VideoSaw
        {
            get { return ReadProperty(nameof(VideoSaw)); }

            set { SaveProperty(nameof(VideoSaw), value); }
        }

        public string UserId
        {
            get { return ReadProperty(nameof(UserId)); }

            set { SaveProperty(nameof(UserId), value); }
        }

        public string ReadProperty(string propertyName)
        {
            return Application.Current.Properties.ContainsKey(propertyName) ? (string)Application.Current.Properties[propertyName] : null;
        }

        public void SaveProperty(string propertyName, string propertyValue)
        {
            Application.Current.Properties[propertyName] = propertyValue;
            Application.Current.SavePropertiesAsync();
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

        public void SaveDateTimeProperty(string propertyName, DateTime propertyValue)
        {
            SaveProperty(propertyName, propertyValue.ToString(DateFormat));
        }
    }
}
