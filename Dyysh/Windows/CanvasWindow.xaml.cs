using Dyysh.Image;
using Dyysh.Properties;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Dyysh
{
    /// <summary>
    /// Interaction logic for CanvasWindow.xaml
    /// </summary>
    public partial class CanvasWindow : Window
    {
        private SolidColorBrush _whiteBrush = new SolidColorBrush(Colors.White);
        private SolidColorBrush _blackBrush = new SolidColorBrush(Colors.Black);
        private System.Windows.Point _startPos;
		private System.Windows.Point _currentPos;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private bool _isMouseDown = false;
        private object _mouseLock = new object();

        private ICaptureProvider captureProvider;

        public CanvasWindow(ICaptureProvider captureProvider)
		{
			InitializeComponent();

            this.captureProvider = captureProvider;

            ImageBrush brush = new ImageBrush(captureProvider.CaptureFullScreen());
            brush.Freeze();
            this.Background = brush;

            this.Topmost = true;

            this.Left = 0;
            this.Top = 0;

            this.Width = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
            this.Height = System.Windows.Forms.SystemInformation.VirtualScreen.Height;
		}

		private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (this._isMouseDown)
			{
				_currentPos = e.GetPosition(null);

                // Value for drawing rectangles outside of capture area
                var rectOffset = 1;

                var backRect = new System.Windows.Shapes.Rectangle();
                var rect = new System.Windows.Shapes.Rectangle();

                _whiteBrush.Freeze();
                backRect.Stroke = _whiteBrush;
                backRect.StrokeDashArray = new DoubleCollection() { 5, 0 };

                _blackBrush.Freeze();
                rect.Stroke = _blackBrush;
                rect.StrokeDashArray = new DoubleCollection() { 5, 5 };

				_x = Math.Min(_currentPos.X, _startPos.X);
				_y = Math.Min(_currentPos.Y, _startPos.Y);

				_width = Math.Max(_currentPos.X, _startPos.X) - _x + 1;
                backRect.Width = rect.Width = _width + rectOffset * 2;

                _height = Math.Max(_currentPos.Y, _startPos.Y) - _y + 1;
                backRect.Height = rect.Height = _height + rectOffset * 2;

				canvas.Children.Clear();
                canvas.Children.Add(backRect);
				canvas.Children.Add(rect);

                Canvas.SetLeft(backRect, _x - rectOffset);
                Canvas.SetTop(backRect, _y - rectOffset);
                Canvas.SetLeft(rect, _x - rectOffset);
                Canvas.SetTop(rect, _y - rectOffset);

				if (e.LeftButton == MouseButtonState.Released)
				{
					lock (_mouseLock) { FinishDrawing(); }
				}
			}
		}

		private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_isMouseDown = true;
			_startPos = e.GetPosition(null);
		}

		private void Window_MouseUp(object sender, MouseButtonEventArgs e)
		{
			lock (_mouseLock) { FinishDrawing(); }
		}

		private void FinishDrawing()
		{
            if (_width * _height >= 9)
            {
                this._isMouseDown = false;

                _x += this.Left;

                BitmapSource image = captureProvider.CaptureArea(new Int32Rect((int)_x, (int)_y, (int)_width, (int)_height));

                if (Settings.Default.UseEditor)
                {
                    // Open new designer window
                    var designer = new Dyysh.Windows.DesignerWindow(image);
                    designer.Show();
                }
                else { Http.UploadImage(image); }
            }

            this.Close();
            
		}

		/*void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			var image = Capture.CopyArea(startPos, currentPos, new System.Windows.Size(width, height));

            //using (var fileStream = new FileStream("C:/Users/duskjet/Desktop/goomba.png", FileMode.Create))
            //{
            //    Stream stream = Upload(data);
            //}

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            //TODO: make selection between png or jpg for image, thumbnail always jpg
            var imageData = Conversion.GetBytesFromPng(image);
            var thumbData = Conversion.CreateThumbnail(image, maxPixelLength: 160);

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                imageData = ms.ToArray();
            }

            var nvc = new NameValueCollection();
            nvc.Add("filename", String.Empty);

            var fileSize = imageData.Length + thumbData.Length;
            nvc.Add("filesize", fileSize.ToString());

            string answer = HttpWeb.UploadImage(imageData, thumbData, nvc);

            MessageBox.Show(answer);
            
            this.Close();
		}*/

        /*public static void PublishImage(string filePath = "", bool isInClipboard = false)
        {
            BitmapSource image = null;

            // If file path is present - make bitmapsource from file
            if (filePath != string.Empty)
                using (var existingImage = new Bitmap(filePath))
                {
                    image = BitmapToSource(existingImage);
                }
            // Clipboard have file or bitmap - make bitmapsource from one of them
            else if (isInClipboard == true)
            {
                if (Clipboard.ContainsImage())
                    image = Clipboard.GetImage();
                else if (Clipboard.ContainsFileDropList())
                {
                    var fileDropList = Clipboard.GetFileDropList();
                    using (var existingImage = new Bitmap(fileDropList[0]))
                    image = BitmapToSource(existingImage);
                }
                else
                {
                    MessageBox.Show("There is no file nor image in the Clipboard.");
                    return;
                }
            }
            // No parameters - capture area for image
            else
                image = Capture.CopyArea(_startPos, new System.Windows.Size(_width, _height));

            var designer = new Dyysh.Windows.DesignerWindow(image);
            designer.ImageFrame.Source = image;
            designer.Show();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            /*
            // get bytes from image
            var imageFormat = Settings.Default.ImageFormat;
            var imageBytes = Conversion.GetBytesFromImage(image, imageFormat);

            // get bytes from thumbnail
            var thumbBytes = Conversion.GetBytesFromThumbnail(image, imageFormat, maxPixelLength: 160);

            // send images with encrypted user GUID
            var guidBytes = System.Text.Encoding.Unicode.GetBytes(Settings.Default.UploadKey);
            var csp = new RSACryptoServiceProvider(2048);
            var serverKey = File.ReadAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dyysh\\ServerRsaKeyBlob");
            csp.ImportCspBlob(serverKey);
            var encryptedGuid = csp.Encrypt(guidBytes, true);

            // get url or error code
            var imageUrl = await Http.Upload(imageBytes, thumbBytes, encryptedGuid, imageFormat);

            var mainWindow = Application.Current.Windows
                .Cast<Window>()
                .FirstOrDefault(window => window is MainWindow) as MainWindow;

            mainWindow.imageUrl = Dyysh.Properties.Settings.Default.ConnectionURL + imageUrl;

            System.Windows.Clipboard.SetText(mainWindow.imageUrl);
            mainWindow.notifyIcon.ShowBalloonTip("ShareMage+", "Image URL copied to clipboard: " + mainWindow.imageUrl, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
            //mainWindow.notifyIcon.HideBalloonTip();
            // save copy to disk?
            // TODO: make saving to disk if proper bool is in effect
            
        }
        */

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;

            window.WindowStyle = System.Windows.WindowStyle.None;
            window.WindowState = System.Windows.WindowState.Normal;
            window.ResizeMode = System.Windows.ResizeMode.NoResize;

            window.Activate();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close(); 
        }

	}
}
