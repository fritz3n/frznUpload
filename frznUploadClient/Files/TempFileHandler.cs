using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Files
{
    static class TempFileHandler
    {
        private static string Tmp => Path.GetTempPath() + "\\frznUpload\\";
        private static List<string> RegisteredFiles = new List<string>();

        public static string RegisterFile()
        {
            if (!Directory.Exists(Tmp))
                Directory.CreateDirectory(Tmp);

            var p = GetUniqueFilename();
            RegisteredFiles.Add(p);
            return Tmp + p;
        }

        public static void FreeFile(string path)
        {
            if (Path.GetDirectoryName(path) != Tmp)
                return;

            if (File.Exists(path))
                File.Delete(path);

            if (RegisteredFiles.Contains(Path.GetFileName(path)))
                RegisteredFiles.Remove(Path.GetFileName(path));
        }

        private static string GetUniqueFilename()
        {
            string name = "";

            do
            {
                name = GetFilename();
            } while (RegisteredFiles.Contains(name));

            return name;
        }

        private static string GetFilename()
        {
            string space = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string name = "";
            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                name += space[random.Next(space.Length)];
            }

            return name;
        }

        public static void FreeAll()
        {
            foreach(string f in RegisteredFiles)
            {
                FreeFile(Tmp + f);
            }
        }
    }
}
