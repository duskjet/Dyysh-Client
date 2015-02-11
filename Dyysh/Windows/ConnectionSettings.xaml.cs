using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Dyysh.Windows
{
    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    public partial class ConnectionSettings : Window
    {
        private ConnectionSettings _instance = null;
        public ConnectionSettings()
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
            url_TextBox_Address.Text = Dyysh.Properties.Settings.Default.ConnectionURL;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = url_TextBox_Address.Text;

            // Format the server url
            var formattedUrl = FormatUrl(text);

            // Save URL
            Dyysh.Properties.Settings.Default.ConnectionURL =
                Http.ConnectionString = formattedUrl;

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string FormatUrl(string text)
        {
            var formattedString = text;

            if (text.LastIndexOf('/') == text.Length - 1)
                formattedString = text.Remove(text.Length - 1);

            if (!text.Contains("http://"))
                formattedString = "http://" + formattedString;

            return formattedString;
        }

    }
}
