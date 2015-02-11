using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dyysh
{
    class ImageCapture
    {
        static public BitmapSource CopyScreen()
        {
            using (var screenBmp = new Bitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        static public BitmapSource CaptureArea(System.Windows.Point startPos, System.Windows.Point endPos, System.Windows.Size size)
        {
            using (var screenBmp = new Bitmap(
                (int)size.Width,
                (int)size.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen((int)startPos.X, (int)startPos.Y, 0, 0, screenBmp.Size, CopyPixelOperation.SourceCopy);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        static public byte[] GetBytesFromPng(BitmapSource image)
        {
            using (var memoryStream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);

                return memoryStream.GetBuffer();
            }
        }

        static public byte[] GetBytesFromJpeg(BitmapSource image)
        {
            using (var memoryStream = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);

                return memoryStream.GetBuffer();
            }
        }

        static public byte[] CreateThumbnail(BitmapSource imageSource, int maxPixelLength, int quality)
        {
            var scale = new ScaleTransform();

            if (imageSource.Width > imageSource.Height)
            { scale.ScaleX = scale.ScaleY = (double)maxPixelLength / (double)imageSource.PixelWidth; }
            else
            { scale.ScaleX = scale.ScaleY = (double)maxPixelLength / (double)imageSource.PixelHeight; }

            var transformedBitmap = new TransformedBitmap(imageSource, scale);

            var encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = quality;
            encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                return memoryStream.GetBuffer();
            }
        }
    }
}
