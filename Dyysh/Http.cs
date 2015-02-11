using Dyysh.Image;
using Dyysh.Properties;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Dyysh
{
    internal class Http
    {
        private static string _requestKeyAction = "/api/RequestKey";
        private static string _authAction = "/api/Authorize";
        private static string _uploadAction = "/api/Upload";
        private static string _serverURL = Dyysh.Properties.Settings.Default.ConnectionURL;
        public static string ConnectionString
        {
            get { return _serverURL; }
            set { _serverURL = value; }
        }

        /// <summary>
        /// Receives server Rsa key (public only)
        /// </summary>
        /// <returns>server RSA key (public only)</returns>
        public static async Task<byte[]> RequestKey()
        {
            using (var client = new HttpClient())
            {
                var requestUri = _serverURL + _requestKeyAction;

                var response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var serverKey = await response.Content.ReadAsByteArrayAsync();
                    return serverKey;
                }
                else
                    throw new HttpRequestException("Server key acquiring failure: " + response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Sends user credentials to the server and receives GUID associated with user
        /// </summary>
        /// <param name="encryptedUsername">username encrypted with server key</param>
        /// <param name="encryptedPassword">password encrypted with server key</param>
        /// <param name="clientCspBlob">client RSA key (public only)</param>
        /// <returns>user GUID encrypted with his key</returns>
        public static async Task<byte[]> Authorize(byte[] encryptedUsername, byte[] encryptedPassword, byte[] clientCspBlob)
        {
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                // Encrypted credentials
                formData.Add(new ByteArrayContent(encryptedUsername), "username");
                formData.Add(new ByteArrayContent(encryptedPassword), "password");
                // Public client RSA key
                formData.Add(new ByteArrayContent(clientCspBlob), "key");

                var requestUri = _serverURL + _authAction;

                var response = await client.PostAsync(requestUri, formData);

                if (response.IsSuccessStatusCode)
                {
                    var guid = await response.Content.ReadAsByteArrayAsync();
                    return guid;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Uploads given image with thumbnail, using user GUID as an identifier
        /// </summary>
        /// <param name="image"></param>
        /// <param name="thumb"></param>
        /// <param name="encryptedGuid">GUID encrypted with server key</param>
        /// <param name="fileExt">extension of the image (.jpg or .png)</param>
        /// <returns>URL to open the uploaded image</returns>
        public static async Task<string> Upload(byte[] image, byte[] thumb, byte[] encryptedGuid, string fileExt)
        {
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new ByteArrayContent(image));
                formData.Add(new ByteArrayContent(thumb));
                formData.Add(new ByteArrayContent(encryptedGuid));
                formData.Add(new StringContent(fileExt.ToLower()));

                var requestUri = _serverURL + _uploadAction;

                var response = await client.PostAsync(requestUri, formData);

                if (response.IsSuccessStatusCode)
                {
                    var fileUrl = await response.Content.ReadAsStringAsync();
                    return fileUrl;
                }
                else
                {
                    return "Error: " + response.ReasonPhrase;
                }
            }
        }

        public static async void UploadImage(BitmapSource image)
        {
            var mainWindow = Application.Current.Windows
               .Cast<Window>()
               .FirstOrDefault(window => window is MainWindow) as MainWindow;

            // Start tray icon animation
            TrayAnimation.Start();

            // Get bytes from image
            var imageFormat = Settings.Default.ImageFormat;
            var imageBytes = Conversion.GetBytesFromImage(image, imageFormat);

            // Get bytes from thumbnail
            var thumbBytes = Conversion.GetBytesFromThumbnail(image, imageFormat, maxPixelLength: 160);

            // Send images with encrypted user GUID
            var guidBytes = System.Text.Encoding.Unicode.GetBytes(Settings.Default.UploadKey);
            var csp = new RSACryptoServiceProvider(2048);
            var serverKey = File.ReadAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dyysh\\ServerRsaKeyBlob");
            csp.ImportCspBlob(serverKey);
            var encryptedGuid = csp.Encrypt(guidBytes, true);

            // Get url or error code
            var imageUrl = await Http.Upload(imageBytes, thumbBytes, encryptedGuid, imageFormat);

            MainWindow.ImageUrl = Dyysh.Properties.Settings.Default.ConnectionURL + imageUrl;
            MainWindow.ShowBalloonTip();

            System.Windows.Clipboard.SetText(Settings.Default.ConnectionURL + imageUrl);

            // Stop tray icon animation
            TrayAnimation.Stop();

            // Save image if proper bool is in effect
            if (Settings.Default.SaveToDisk)
            {
                var fileName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
                var filePath = Settings.Default.SaveLocation + "\\" + fileName;

                File.WriteAllBytes(filePath, imageBytes);
            }
        }
    }
}
