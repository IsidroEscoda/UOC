using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DistraidaMente.Common.Helpers
{
    public static class FilesHelper
    {
        public static string ReadEmbeddedFileAsString(string path)
        {
            string result = "";

            try
            {
                var assembly = Assembly.GetCallingAssembly(); // typeof(FilesHelper).GetTypeInfo().Assembly;

                StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(path));

                result = sr.ReadToEnd();
            }
            catch
            {
            }

            return result;
        }
    }
}
