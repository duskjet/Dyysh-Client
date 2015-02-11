using Dyysh.Properties;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Dyysh
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private static SettingsWindow _instance = null;
        private string currentRootPath;
        private string appDataFolder;

        public AuthorizeState AuthorizeState
        {
            get { return (AuthorizeState)GetValue(AuthorizeStateProperty); }
            set { SetValue(AuthorizeStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AuthorizeState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AuthorizeStateProperty =
            DependencyProperty.Register("AuthorizeState", typeof(AuthorizeState), typeof(SettingsWindow), new PropertyMetadata(AuthorizeState.Initial));

        public SettingsWindow()
        {
            if (_instance == null)
            {
                InitializeComponent();
                _instance = this;
                _instance.Show();
            }
            else
            {
                var windowstate = _instance.WindowState;
                _instance.Activate();
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _instance = null;
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            //Saving settings only if Done was pressed
            Settings.Default.Username = account_TextBox_UsernameField.Text;
            Settings.Default.UseEditor = (bool)CheckBox_UseEditor.IsChecked;
            Settings.Default.SaveToDisk = (bool)savecopy_CheckBox_SavetoDisk.IsChecked;
            Settings.Default.SaveLocation = savecopy_TextBox_CurrentPathField.Text;
            Settings.Default.ImageFormat = ComboBox_ImageFormat.Text;

            Settings.Default.Save();

            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            //Restore window state from minimized
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            currentRootPath = System.AppDomain.CurrentDomain.BaseDirectory;
            appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dyysh";

            #region Getting values from Settings class
            if (Settings.Default.SaveToDisk == true)
            {
                saveCopyIsEnabled(true);
            }

            ComboBox_ImageFormat.Text = Settings.Default.ImageFormat;

            if (Settings.Default.LastAuthorizeSuccess == true)
            {
                AuthorizeState = AuthorizeState.Granted;
                account_Label_UploadAs.Content = Settings.Default.Username;
            }
            #endregion
        }

        private void savecopy_CheckBox_SavetoDisk_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsInitialized == true)
            {
                saveCopyIsEnabled(true);
            }
            
        }
        private void savecopy_CheckBox_SavetoDisk_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsInitialized == true)
            {
                saveCopyIsEnabled(false);
            }
        }

        private void savecopy_Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowDialog();
            savecopy_TextBox_CurrentPathField.Text = dialog.SelectedPath;
        }

        private void savecopy_Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            savecopy_TextBox_CurrentPathField.Text = currentRootPath;
        }

        #region Helper Methods
        private void saveCopyIsEnabled(bool status)
        {
            savecopy_TextBox_CurrentPathField.IsEnabled = status;
            savecopy_Button_Browse.IsEnabled = status;
            savecopy_Button_Reset.IsEnabled = status;
        }

        private void AuthorizationInProgress(bool status)
        {
            if (status)
            {
                account_ProgressBar.Visibility = System.Windows.Visibility.Visible;
                account_Button_Authorize.IsEnabled = false;
                account_Button_ConnectionSettings.IsEnabled = false;
            }
            else
            {
                account_ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                account_Button_Authorize.IsEnabled = true;
                account_Button_ConnectionSettings.IsEnabled = true;
            }
        }

        #endregion

        private async void account_Button_Authorize_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationInProgress(true);

            try
            { 
            // Get key from the server
            var serverKey = await Http.RequestKey();

            if (serverKey == null)
            {
                MessageBox.Show("Could not receive server data. Check connection URL or try again later.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Directory.CreateDirectory(appDataFolder);
            System.IO.File.WriteAllBytes(appDataFolder + "\\ServerRsaKeyBlob", serverKey);

            using (var serverRsa = new RSACryptoServiceProvider(2048))
            {
                serverRsa.ImportCspBlob(serverKey);

                // Cryptography provider code
                var cspParams = new CspParameters
                {
                    KeyContainerName = "userContainer",
                    Flags = CspProviderFlags.NoPrompt | CspProviderFlags.UseArchivableKey | CspProviderFlags.UseMachineKeyStore
                };

                using (var clientRsa = new RSACryptoServiceProvider(2048, cspParams))
                {
                    var clientKeyBlob = clientRsa.ExportCspBlob(false);

                    // Prepare all the neccessary params for auth
                    var username = account_TextBox_UsernameField.Text;
                    var password = account_PasswordBox_PasswordField.Password;

                    var usernameBytes = Encoding.Unicode.GetBytes(username);
                    var passwordBytes = Encoding.Unicode.GetBytes(password);

                    var encryptedUsername = serverRsa.Encrypt(usernameBytes, true);
                    var encryptedPassword = serverRsa.Encrypt(passwordBytes, true);

                    // Get GUID for uploading
                    var encryptedGuid = await Http.Authorize(encryptedUsername, encryptedPassword, clientKeyBlob);

                    if (encryptedGuid != null)
                    {
                        var guid = clientRsa.Decrypt(encryptedGuid, true);
                        Settings.Default.UploadKey = Encoding.Unicode.GetString(guid);

                        AuthorizeState = AuthorizeState.Granted;
                        Settings.Default.AuthorizeSuccess = true;
                        Settings.Default.LastAuthorizeSuccess = true;
                        account_Label_UploadAs.Content = account_TextBox_UsernameField.Text;

                        AuthorizationInProgress(false);
                    }
                    else
                    {
                        AuthorizeState = AuthorizeState.Denied;
                        Settings.Default.AuthorizeSuccess = false;
                        Settings.Default.LastAuthorizeSuccess = false;
                        account_Label_UploadAs.Content = "N/A";

                        AuthorizationInProgress(false);
                    }
                }
            }
            }
            catch(Exception ex)
            {
                AuthorizeState = AuthorizeState.Denied;
                Settings.Default.AuthorizeSuccess = false;
                Settings.Default.LastAuthorizeSuccess = false;
                account_Label_UploadAs.Content = "N/A";

                AuthorizationInProgress(false);

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void account_Button_ConnectionSettings_Click(object sender, RoutedEventArgs e)
        {
            var connectionSettings = new Dyysh.Windows.ConnectionSettings();
        }
      
    }

}
