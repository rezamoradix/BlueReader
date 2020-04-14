using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace BlueReader
{
    /// <summary>
    /// Interaction logic for BookControl.xaml
    /// </summary>
    public partial class BookControl : UserControl
    {
        public MainWindow MainWindow { get; }

        public BookControl(BookModel model, MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = model;
            MainWindow = mainWindow;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Path.Combine(DownMan.AppData(), "sumatra.exe")))
            {
                var zip = Ionic.Zip.ZipFile.Read(Path.Combine(DownMan.AppData(), "sumatra.zip"));
                zip.ExtractAll(DownMan.AppData());
            }

            if (!File.Exists(((BookModel)DataContext).DocPath))
            {
                MessageBox.Show("Error: No File");
                return;
            }

            Process.Start(Path.Combine(DownMan.AppData(), "sumatra.exe"), Path.Combine(DownMan.AppData(), ((BookModel)DataContext).DocPath));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var book = (BookModel)DataContext;
                DataMan.DeleteBook(book);
                File.Delete(Path.Combine(DownMan.AppData(), book.DocPath));
                File.Delete(Path.Combine(DownMan.AppData(), book.ImageUrl));
                ((WrapPanel)this.Parent).Children.Remove(this);
            }
            catch (Exception e3)
            {
                MessageBox.Show("Error | " + e3.Message);
            }
        }
    }

}
