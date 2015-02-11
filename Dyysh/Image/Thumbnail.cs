using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dyysh.Image
{
    partial class Conversion
    {
        static public byte[] GetBytesFromThumbnail(BitmapSource imageSource, string imageFormat, int maxPixelLength, int jpegQuality = 80)
        {
            var scale = new ScaleTransform();

            if (imageSource.Width > imageSource.Height)
            { scale.ScaleX = scale.ScaleY = (double)maxPixelLength / (double)imageSource.PixelWidth; }
            else
            { scale.ScaleX = scale.ScaleY = (double)maxPixelLength / (double)imageSource.PixelHeight; }

            var transformedBitmap = new TransformedBitmap(imageSource, scale);

            BitmapEncoder encoder;
            var extension = imageFormat.ToLower();

            if (extension == "png")
                encoder = new PngBitmapEncoder();
            else if (extension == "jpg")
                encoder = new JpegBitmapEncoder();
            else
                throw new FileFormatException("Unknown file format");

            encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                return memoryStream.GetBuffer();
            }
        }
    }
}
