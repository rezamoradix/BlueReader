using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Threading;
using System.Diagnostics;

namespace BlueReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // if app is runnnig but user adds new book
            

            //var inu = "bluereader://https://bayanbox.ir/download/1304514819017851126/A-Confederate-General-From-Big-Sur-Richard-Brautigan.epub|http://bayanbox.ir/view/7398172316998520139/a-confederate-general-richrad-brautigan.jpg|Richard Brautiagn a condeferate genrakl from big sur";
            if (e.Args.Length > 0)
            //if (true)
            {
                var call = WebUtility.UrlDecode(e.Args[0]);
                //var call = WebUtility.UrlDecode(inu);
                var data = call.ToLower().Replace("bluereader://", "").Split(new[] { '|' });
                if (data.Length < 2 || data[0] == null || data[1] == null || data[2] == null)
                {                
                    System.Windows.MessageBox.Show("Error | Link is broken");
                    return;
                }
                //System.Windows.MessageBox.Show(data[1]);
                var pg = new ProgressWindow();
                pg.Show();
                Thread th = new Thread(new ThreadStart(() =>
                {
                }));
                    th.Start();
                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var docPath = DownMan.Download(data[0], DownMan.FileType.File);
                        var imgPath = DownMan.Download(data[1], DownMan.FileType.Image);

                        var config = DataMan.GetConfigFile();
                        DataMan.AddNewBook(new BookModel()
                        {
                            Id = (config.Books?.Count + 1) ?? 0,
                            Title = data[2],
                            ImageUrl = imgPath,
                            DocPath = docPath
                        });

                        

                        Dispatcher.Invoke(() =>
                        {
                            Process p = Process.GetCurrentProcess();
                            int c = Process.GetProcesses().Where(x => x.ProcessName == p.ProcessName).Count();
                            if (c > 1)
                                Current.Shutdown();

                            var mw = new MainWindow();
                            pg.Close();
                            mw.ShowDialog();
                        });
                    }
                    catch (Exception ee)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Error. please try again | " + ee.Message);
                            Current.Shutdown();
                        });
                    }
                });
            }
            else
            {
                Process p = Process.GetCurrentProcess();
                int c = Process.GetProcesses().Where(x => x.ProcessName == p.ProcessName).Count();
                if (c > 1)
                {
                    MessageBox.Show("an instance is running...");
                    Current.Shutdown();
                }
                var mw = new MainWindow();
                mw.ShowDialog();
            }
        }
    }
}
