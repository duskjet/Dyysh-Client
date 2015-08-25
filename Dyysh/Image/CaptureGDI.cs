using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Dyysh.Image
{
    public class CaptureGDI : ICaptureProvider
    {
        public System.Windows.Media.Imaging.BitmapSource CaptureFullScreen()
        {
            return CaptureArea(new Int32Rect(
                SystemInformation.VirtualScreen.Left,
                SystemInformation.VirtualScreen.Top,
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height));
        }

        public System.Windows.Media.Imaging.BitmapSource CaptureArea(System.Windows.Int32Rect rect)
        {
            // Initialize bitmap and pointers
            System.Windows.Media.Imaging.BitmapSource bitmapsource;

            Bitmap bmp = null;
            IntPtr hDesk = IntPtr.Zero;
            IntPtr hSrce = IntPtr.Zero;
            IntPtr hDest = IntPtr.Zero;
            IntPtr hBmp = IntPtr.Zero;

            try
            {
                hDesk = GetDesktopWindow();
                hSrce = GetWindowDC(hDesk);
                hDest = CreateCompatibleDC(hSrce);
                hBmp = CreateCompatibleBitmap(hSrce, rect.Width, rect.Height);
                IntPtr hOldBmp = SelectObject(hDest, hBmp);
                bool b = BitBlt(hDest, 0, 0, rect.Width, rect.Height, hSrce, rect.X, rect.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
                bmp = Bitmap.FromHbitmap(hBmp);

                // Create Bitmapsource from bitmap handle
                bitmapsource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()
                );

                SelectObject(hDest, hOldBmp);
            }
            finally
            {
                DeleteObject(hBmp);
                DeleteDC(hDest);
                ReleaseDC(hDesk, hSrce);
                bmp.Dispose();
            }

            return bitmapsource;
        }

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);
    }
}
