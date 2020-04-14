using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

namespace BlueReader
{
    public class DownMan
    {
        public enum FileType
        {
            File, Image
        }

        public static string Download(string url, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.File:
                    return Download(url, "files");
                case FileType.Image:
                    return Download(url, "images");
                default: return null;
            }
        }

        private static string Download(string url, string dest)
        {
            var req = WebRequest.Create(url);
            var name = Path.GetFileName(url);
            var apppath = CheckDir(dest);
            var path = Path.Combine(apppath, name);

            using (var output = File.OpenWrite(path))
            using (var res = req.GetResponse().GetResponseStream())
            {
                // check if file is exe
                byte[] buf = new byte[1024];
                int len = res.Read(buf, 0, buf.Length);
                var fileHead = Encoding.ASCII.GetString(buf);
                if (fileHead.Contains("MZ"))
                {
                    System.Windows.MessageBox.Show("Error | file is wrong");
                    System.Windows.Application.Current.Shutdown();
                }

                res.CopyTo(output);
            }
            return path;
        }

        private static string CheckDir(string path)
        {
            string br = AppData();
            string dest = Path.Combine(br, path);
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);

            return dest;
        }

        public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string AppData()
        {
            string br = Path.Combine(appdata, "bluereader");
            if (!Directory.Exists(br)) Directory.CreateDirectory(br);
            return br;
        }
    }
}
