using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public static class ReadFromFile<Type>
    {
        public static Type Do(string file, string parametername, Func<string, Type> parser, bool logResult = false)
        {
            var paramstring = "";
            try
            {
                using (var reader = new StreamReader(file))
                {
                    var line = "";
                    while (!((line = reader.ReadLine()) == null))
                    {
                        if (line.StartsWith(parametername))
                        {
                            paramstring = line.Split(' ')[1];
                        }
                    }
                }
            }
            catch
            {
                Log.Post("Couldn't read parameter " + parametername + ".", LogCategory.Critical);
            }
            if (logResult)
                Log.Post(parametername + ": " + paramstring);
            return parser(paramstring);
        }
    }
    public static class ReadFromFile
    {
        public static string Do(string file, string parametername, bool logResult = false)
        {
            var paramstring = "";
            try
            {
                using (var reader = new StreamReader(file))
                {
                    var line = "";
                    while (!((line = reader.ReadLine()) == null))
                    {
                        if (line.StartsWith(parametername))
                        {
                            paramstring = line.Split(' ')[1];
                        }
                    }
                }
            }
            catch
            {
                Log.Post("Couldn't read parameter " + parametername + ".", LogCategory.Critical);
            }
            if (logResult)
                Log.Post(parametername + ": " + paramstring);
            return paramstring;
        }
    }

    public static class ReadFileLines
    {
        public static LinkedList<string> Do(string file, bool logResult = false)
        {
            var ll = new LinkedList<string>();
            //try
            {
                using (var reader = new StreamReader(file))
                {
                    var line = "";
                    while (!((line = reader.ReadLine()) == null))
                    {
                        ll.AddLast(line);
                    }
                }
            }
            return ll;
        }
    }
}
