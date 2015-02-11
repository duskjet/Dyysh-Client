using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Threading;

namespace Dyysh.Image
{
    partial class Conversion
    {
        static public byte[] GetBytesFromImage(BitmapSource image, string imageFormat)
        {
            BitmapEncoder encoder;
            var extension = imageFormat.ToLower();

            if (extension == "png")
                encoder = new PngBitmapEncoder();
            else if (extension == "jpg")
                encoder = new JpegBitmapEncoder();
            else
                throw new FileFormatException("Unknown file format");

            using (var memoryStream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);

                return memoryStream.ToArray();
            }
        }

        //static public BitmapSource BitmapToSource(Bitmap source)
        //{
        //    var rect = new Rectangle(0, 0, source.Width, source.Height);
        //    var bitmap_data = source.LockBits(rect, ImageLockMode.ReadOnly, source.PixelFormat);

        //    try
        //    {
        //        BitmapPalette palette = null;

        //        if (source.Palette.Entries.Length > 0)
        //        {
        //            var palette_colors = source.Palette.Entries.Select(entry => System.Windows.Media.Color.FromArgb(entry.A, entry.R, entry.G, entry.B)).ToList();
        //            palette = new BitmapPalette(palette_colors);
        //        }

        //        return BitmapSource.Create(
        //            source.Width,
        //            source.Height,
        //            // TODO: Not every device using 96 dpi ...
        //            96, // source.HorizontalResolution,
        //            96, // source.VerticalResolution,
        //            ConvertPixelFormat(source.PixelFormat),
        //            palette,
        //            bitmap_data.Scan0,
        //            bitmap_data.Stride * source.Height,
        //            bitmap_data.Stride
        //        );
        //    }
        //    finally
        //    {
        //        source.UnlockBits(bitmap_data);
        //    }
        //}

        static public BitmapSource BitmapToSource(Bitmap source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (Application.Current.Dispatcher == null)
                return null; // Is it possible?

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // You need to specify the image format to fill the stream. 
                    // I'm assuming it is PNG
                    source.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Make sure to create the bitmap in the UI thread
                    if (InvokeRequired)
                        return (BitmapSource)Application.Current.Dispatcher.Invoke(
                            new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                            DispatcherPriority.Normal,
                            memoryStream);

                    return CreateBitmapSourceFromBitmap(memoryStream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        private static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            // This will disconnect the stream from the image completely...
            WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            return writable;
        }

        private static System.Windows.Media.PixelFormat ConvertPixelFormat(PixelFormat sourceFormat)
        {
            switch (sourceFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return System.Windows.Media.PixelFormats.Bgr24;

                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return System.Windows.Media.PixelFormats.Bgra32;

                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return System.Windows.Media.PixelFormats.Bgr32;
            }

            return new System.Windows.Media.PixelFormat();
        }
    }
}
