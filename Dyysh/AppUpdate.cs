using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;


namespace Dyysh
{
    /// <summary>
    /// Provides common methods for updating client application.
    /// </summary>
    class AppUpdate
    {
        private static WebClient _webClient = new WebClient();
        private static UpdateInfo _updateInfo;

        /// <summary>
        /// Gets WebClient object which can be used to bind download event handlers.
        /// </summary>
        public static WebClient WebClient
        {
            get { return _webClient; }
        }

        /// <summary>
        /// Gets UpdateInfo object which was obtained by CheckForUpdate method.
        /// </summary>
        public static UpdateInfo CurrentUpdateInfo
        {
            get { return _updateInfo; }
        }
        
        /// <summary>
        /// Downloads setup file from server to local machine
        /// </summary>
        /// <param name="sourcePath">File path on web</param>
        /// <param name="destPath">Local file path</param>
        public static void DownloadUpdate(string sourcePath, string destPath)
        {
            _webClient.DownloadFileAsync(new Uri(sourcePath), destPath);
        }

        /// <summary>
        /// Compares versions of current assembly and version specified in UpdateInfo.xml
        /// </summary>
        /// <param name="filePath">UpdateInfo.xml file on web server</param>
        /// <returns>True if update is available, False if not.</returns>
        public static bool CheckForUpdate(string filePath)
        {
            _updateInfo = XmlDeserialize(filePath);

            var latestVer = new Version(_updateInfo.LatestVersion);
            var currentVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var latestIsNewer = latestVer.CompareTo(currentVer);
            if (latestIsNewer > 0)
                return true;
            else
                return false;
        }

        private static UpdateInfo XmlDeserialize(string filePath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(UpdateInfo));

            var stream = _webClient.OpenRead(new Uri(filePath));

            var XmlData = (UpdateInfo)deserializer.Deserialize(stream);

            stream.Close();

            return XmlData;
        }

    }
    public class UpdateInfo
    {
        public string LatestVersion { get; set; }
        public string Location { get; set; }
    }
}
