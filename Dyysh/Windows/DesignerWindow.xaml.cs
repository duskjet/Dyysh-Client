using Dyysh.Image;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Dyysh.Windows
{
    /// <summary>
    /// Interaction logic for DesignerWindow.xaml
    /// </summary>
    public partial class DesignerWindow : Window
    {
        private BitmapSource _image;
        
        public DesignerWindow()
        {
            InitializeComponent();
        }

        public DesignerWindow(BitmapSource image)
        {
            InitializeComponent(); 

            _image = image;
            ImageFrame.Source = image;

            this.MaxHeight = SystemParameters.WorkArea.Height;
            this.MaxWidth = SystemParameters.WorkArea.Width;
        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(window => window is MainWindow) as MainWindow;

            if (_image != null)
                Http.UploadImage(_image);

            this.Close();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            // SaveFileDialog config
            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = "image";
            saveDialog.Filter = "PNG Images|*.png|JPEG Images|*.jpg";

            // Show save dialog box
            Nullable<bool> result = saveDialog.ShowDialog();

            // Process dialog box results 
            if (result == true)
            {
                // Save document 
                var fileName = saveDialog.FileName;
                var fileExt = fileName.Substring(fileName.Length - 3);
                var fileBytes = Conversion.GetBytesFromImage(_image, fileExt);

                File.WriteAllBytes(fileName, fileBytes);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CentralizeWindow();
        }

        private void CentralizeWindow()
        {
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            var windowWidth = this.Width;
            var windowHeight = this.Height;

            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
