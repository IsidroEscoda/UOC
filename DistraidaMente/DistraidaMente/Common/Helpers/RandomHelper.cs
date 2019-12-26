using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaApps.CommonLibrary.Helpers
{
    public static class RandomHelper
    {
        private static Random _randomizer;

        private static Random Randomizer
        {
            get
            {
                if (_randomizer == null)
                {
                    _randomizer = new Random();
                }

                return _randomizer;
            }
        }
        
        public static T RandomItem<T> (this IEnumerable<T> list)
        {
            if (list.Any())
            {
                int index = Randomizer.Next(0, list.Count());

                return list.ElementAt(index);
            }

            return default;
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(x=> Randomizer.Next());
        }
       
        public static int RandomInt(int start, int end)
        {
            return Randomizer.Next(start, end);
        }

        public static bool RandomBool()
        {
            return RandomInt(0, 2) == 1;
        }

        public static T RandomEnum<T>(this Type enumType) where T : Enum
        {
            var list = Enum.GetValues(enumType);

            int index = Randomizer.Next(list.Length);

            return (T)list.GetValue (index);
        }

        public static IEnumerable<T> Randomize<T>(this Type enumType) where T : Enum
        {
            var list = Enum.GetValues(enumType).Cast<T>().ToList();

            return list.OrderBy(x => Randomizer.Next());
        }
    }
}
