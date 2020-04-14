using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace BlueReader
{
    public class BookModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string DocPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private string _imageUrl;

        [XmlIgnore]
        public BitmapImage Bitmap
        {
            get
            {
                var bm = new BitmapImage();
                bm.BeginInit();
                bm.CacheOption = BitmapCacheOption.OnLoad;
                bm.UriSource = new Uri(ImageSource);
                bm.EndInit();
                return bm;
            }
        }
        [XmlIgnore]
        public string ImageSource => Path.Combine(Directory.GetCurrentDirectory(), _imageUrl);

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; OnPropertyChanged("ImageUrl"); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}