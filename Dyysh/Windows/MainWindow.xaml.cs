using Dyysh.Image;
using Dyysh.Properties;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Interop;

namespace Dyysh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _mainWindow;
        public static MainWindow GetCurrentMainWindow
        {
            get { return _mainWindow; }
        }

        private static string imageUrl;
        public static string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
        public MainWindow()
        {
            InitializeComponent();

            _mainWindow = this;

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            TrayAnimation.Init();
        }

        #region ContextMenu Click Events
        private void PublishDesktop_Click(object sender, RoutedEventArgs e)
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0,0,1);
            dispatcherTimer.Start();

            dispatcherTimer.Tick += delegate(object s, EventArgs ev)
            {
                dispatcherTimer.Stop();
                var canvasWindow = new CanvasWindow();
                canvasWindow.Show();
                canvasWindow.Activate();
            };
        }

        private void PublishFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.ReadOnlyChecked = true;

            var result = openFileDialog.ShowDialog(Application.Current.MainWindow as MainWindow);
            if (result == true)
                {
                    filePath = openFileDialog.FileName;

                    BitmapSource image;

                    using (var bitmap = new Bitmap(filePath))
                        image = Conversion.BitmapToSource(bitmap);
                    //image = new BitmapImage( new Uri(filePath) );

                    // Open new designer window
                    var designer = new Dyysh.Windows.DesignerWindow(image);
                    designer.Show();
                }
            
        }

        private void PublishClipboard_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource image;

            if (Clipboard.ContainsImage())
            {
                image = Clipboard.GetImage();
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var fileDropList = Clipboard.GetFileDropList();
                using (var existingImage = new Bitmap(fileDropList[0]))
                    image = Conversion.BitmapToSource(existingImage);
            }
            else
            {
                MessageBox.Show(this, "There is no file or image in Clipboard.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Open new designer window
            var designer = new Dyysh.Windows.DesignerWindow(image);
            designer.Show();

        }

        private void ManageLibrary_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Settings.Default.ConnectionURL + "/Gallery");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dyysh.Properties.Settings.Default.Save();
        }

        static public void ShowBalloonTip()
        {
            string balloonMsg;
            if ( imageUrl.StartsWith("http:") )
                balloonMsg = "Image URL copied to clipboard: " + imageUrl;
            else
                balloonMsg = "Error: " + imageUrl;

            _mainWindow.notifyIcon.ShowBalloonTip
                ("Dyysh", balloonMsg, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        private void TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            if (imageUrl != string.Empty)
            {
                try
                {
                    System.Diagnostics.Process.Start(imageUrl);
                }
                catch(Exception) { }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.Username == string.Empty)
            {
                if (MessageBox.Show("To upload something you'll need to log in from installed app (Tray Icon -> Settings). Would you like to do it now?", 
                    "Don't forget to log in!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information,
                    MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    new SettingsWindow();
                }
            }
        }

    }
}
