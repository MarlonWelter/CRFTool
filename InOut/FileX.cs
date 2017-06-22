using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase.InOut
{
    public static class FileX
    {
        public static string Find(string fileName, string directory)
        {
            var files = Directory.GetFiles(directory);
            if (files.Any(f => f.Contains(fileName)))
                return files.FirstOrDefault(f => f.Contains(fileName));
            else
            {
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    var file = Find(fileName, dir);
                    if (file.NotNullOrEmpty())
                    {
                        return file;
                    }
                }
            }
            return null;
        }
    }
}
