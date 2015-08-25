using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Dyysh.Image
{
    public interface ICaptureProvider
    {
        BitmapSource CaptureFullScreen();

        BitmapSource CaptureArea(System.Windows.Int32Rect rect);
    }
}
