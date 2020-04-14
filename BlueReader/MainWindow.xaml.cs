using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BlueReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Task.Factory.StartNew(() =>
            //{
            //    _ = DataMan.GetConfigFile();
            //    Dispatcher.Invoke(UpdateUI);
            //});
            _ = DataMan.GetConfigFile();
            UpdateUI();
            WatchfulWatcher();
        }

        public void UpdateUI()
        {
            mainGrid.Children.Clear();
            if (DataMan.StaticConfig.Books != null)
                DataMan.StaticConfig.Books.ForEach(b => mainGrid.Children.Add(new BookControl(b, this)));
            loaderStackPanel.Visibility = Visibility.Hidden;
        }

        public async void WatchfulWatcher()
        {
            await Task.Run(() =>
            {
                DataMan.Watcher = new FileSystemWatcher(DownMan.AppData());
                DataMan.Watcher.BeginInit();
                DataMan.Watcher.EnableRaisingEvents = true;
                DataMan.Watcher.NotifyFilter = NotifyFilters.LastWrite;
                DataMan.Watcher.Filter = "*.xml";
                DataMan.Watcher.Changed += FileSystemWatcher_Changed;
                DataMan.Watcher.EndInit();
            });
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //Thread.Sleep(2000);
            DataMan.GetConfigFile();
            Dispatcher.Invoke(() =>
            {
                UpdateUI();
                Activate();
            });
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://bluepaper.ir");
        }
    }
}
