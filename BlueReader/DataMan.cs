using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace BlueReader
{
    public class DataMan
    {
        public static string ConfigPath = Path.Combine(DownMan.AppData(), "config.xml");
        public static FileSystemWatcher Watcher;
        public static Config StaticConfig;
        public static Config GetConfigFile()
        {
            if (!File.Exists(ConfigPath))
            {
                var genesis = new Config { OpenTime = DateTime.Now, Books = new List<BookModel>() };
                var serializer = new XmlSerializer(typeof(Config));

                using (var fstream = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var stream = new StreamWriter(fstream))
                {
                    serializer.Serialize(stream, genesis);
                    stream.Close();
                }
                StaticConfig = genesis;

                return genesis;
            }
            else
            {
                try
                {
                    while (IsFileLocked(new FileInfo(ConfigPath))) { }
                    using (var fstream = new FileStream(ConfigPath, FileMode.OpenOrCreate,
                                            FileAccess.Read, FileShare.None))
                    using (var reader = new StreamReader(fstream))
                    {
                        var serializer = new XmlSerializer(typeof(Config));
                        StaticConfig = (Config)serializer.Deserialize(reader);
                        reader.Close();
                    }

                }
                catch (Exception)
                {
                    File.Delete(ConfigPath);
                }

                return StaticConfig;
            }
        }

        public static void UpdateConfigFile()
        {
            while (IsFileLocked(new FileInfo(ConfigPath))) { }
            using (var fstream = new FileStream(ConfigPath, FileMode.Truncate, FileAccess.Write, FileShare.None))
            using (var stream = new StreamWriter(fstream))
            {
                var serializer = new XmlSerializer(typeof(Config));
                serializer.Serialize(stream, StaticConfig);
                stream.Close();
            }

        }

        public static void AddNewBook(BookModel book)
        {
            _ = GetConfigFile();
            if (book == null && StaticConfig.Books.Where(x => x.Id == book.Id).Count() > 1) return;
            StaticConfig.Books.Add(book);
            UpdateConfigFile();
        }

        public static void DeleteBook(BookModel book)
        {
            Watcher.EnableRaisingEvents = false;
            if (book == null && StaticConfig.Books.Where(x => x.Id == book.Id).Count() < 2) return;
            StaticConfig.Books.Remove(book);
            UpdateConfigFile();
            Watcher.EnableRaisingEvents = true;
        }

        protected static bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists) return false;
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}
