using Dyysh.Image;
using Dyysh.Properties;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Interop;
using NHotkey.Wpf;
using Dyysh.HotkeyBinding;

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
            PublishDesktop();
        }

        public void PublishDesktop()
        {
            var canvasWindow = new CanvasWindow(new CaptureGDI());
            canvasWindow.Show();
            canvasWindow.Activate();
        }

        private void PublishFile_Click(object sender, RoutedEventArgs e)
        {
            PublishFile();
            
        }

        public void PublishFile()
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
            PublishClipboard();

        }

        public void PublishClipboard()
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

        public void PublishDesktopHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            if (!KeyBindingProvider.IsRecording)
            {
                PublishDesktop();
                e.Handled = true;
            }
        }

        public void PublishFileHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            if (!KeyBindingProvider.IsRecording)
            {
                PublishFile();
                e.Handled = true;
            }
        }

        public void PublishClipHotkey(object sender, NHotkey.HotkeyEventArgs e)
        {
            if (!KeyBindingProvider.IsRecording)
            {
                PublishClipboard();
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load and assign saved hotkeys
            var keyBind = new KeyBinding();
            string setting;
            bool gotHotkeyException = false;
            
            // Very crappy check for every hotkey
            // TODO: refactor and add friendly exception handling
            try
            {
                setting = Settings.Default.Hotkey_CaptureArea;
                if (setting != null && setting != string.Empty)
                {
                    keyBind.FromString(setting);
                    HotkeyManager.Current.AddOrReplace("CaptureArea", keyBind.Key, keyBind.ModifierKeys, PublishDesktopHotkey);
                }
            }
            catch (NHotkey.HotkeyAlreadyRegisteredException exception)
            {
                HotkeyManager.Current.Remove(exception.Name);
                gotHotkeyException = true;
            }

            try
            {
                setting = Settings.Default.Hotkey_PublishClip;
                if (setting != null && setting != string.Empty)
                {
                    keyBind.FromString(setting);
                    HotkeyManager.Current.AddOrReplace("PublishClip", keyBind.Key, keyBind.ModifierKeys, PublishClipHotkey);
                }
            }
            catch (NHotkey.HotkeyAlreadyRegisteredException exception)
            {
                HotkeyManager.Current.Remove(exception.Name);
                gotHotkeyException = true;
            }

            try
            {
                setting = Settings.Default.Hotkey_PublishFile;
                if (setting != null && setting != string.Empty)
                {
                    keyBind.FromString(setting);
                    HotkeyManager.Current.AddOrReplace("PublishFile", keyBind.Key, keyBind.ModifierKeys, PublishClipHotkey);
                }
            }
            catch (NHotkey.HotkeyAlreadyRegisteredException exception)
            {
                HotkeyManager.Current.Remove(exception.Name);
                gotHotkeyException = true;
            }
            
            // Prompt message box to alert about hotkeys
            if (gotHotkeyException)
            {
                if (MessageBox.Show("Some of the hotkeys are disabled because they are already registered somewhere else. Do you want to change them now? ('Yes' opens Settings window)",
                    "Hotkey already registered.",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error,
                    MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    new SettingsWindow();
                }
            }

            // Suggest user to log in
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
