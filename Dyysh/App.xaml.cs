using System;
using System.Diagnostics;
using System.Windows;

namespace Dyysh
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Register domain-wide exception notifying
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            #region Allow only one instance of the application
            // Get Reference to the current Process
            Process thisProc = Process.GetCurrentProcess();
            // Check how many total processes have the same name as the current one
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                // If ther is more than one, than it is already running.
                MessageBox.Show("Application is already running.");
                Application.Current.Shutdown();
                return;
            }
            #endregion

            #region Check for updates
            bool isUpdateAvailable;

            try
            {
                isUpdateAvailable = AppUpdate.CheckForUpdate
                    (Dyysh.Properties.Settings.Default.ConnectionURL + "/Client/UpdateInfo.xml");
            }
            catch(System.Net.WebException webEx)
            {
                isUpdateAvailable = false;
            }
            // Check for updates
            if (isUpdateAvailable)
            {
                var updateWindow = new Dyysh.Windows.UpdateWindow(AppUpdate.CurrentUpdateInfo);
                updateWindow.ShowDialog();
            }
            #endregion

            base.OnStartup(e);
            this.StartupUri = new Uri("Windows/MainWindow.xaml", UriKind.Relative);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            var errorMessage = String.Format(
                "An unhandled exception of type ({0}) occured: {1}.",
                ex.GetType(),
                ex.Message);

            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

