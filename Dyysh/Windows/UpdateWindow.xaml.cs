using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Dyysh.Windows
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private UpdateInfo _updateInfo;
        private string _localSetupPath;
        public UpdateWindow(UpdateInfo updateInfo)
        {
            InitializeComponent();

            _updateInfo = updateInfo;

            CentralizeWindow();

            // Fill window texts
            this.Title = "Dyysh v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Label_UpdateText.Text = string.Format("A newer version of Dyysh ({0}) is available for dowload. Do You want to update now?", updateInfo.LatestVersion);

            // Setup save location
            var fileName = Path.GetFileName(_updateInfo.Location);
            _localSetupPath = Path.Combine(Path.GetTempPath(), fileName);

            // Eventhandlers binding
            AppUpdate.WebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            AppUpdate.WebClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            System.Media.SystemSounds.Beep.Play();
        }

        void WebClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_localSetupPath != null)
            {
                System.Diagnostics.Process.Start(_localSetupPath);
                System.Environment.Exit(0);
            }

        }

        private void Button_UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = System.Windows.Visibility.Visible;

            var filePath = _updateInfo.Location;

            AppUpdate.DownloadUpdate(filePath, _localSetupPath);
        }

        private void Button_UpdateLater_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Button_Changelog_Click(object sender, RoutedEventArgs e)
        {

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
