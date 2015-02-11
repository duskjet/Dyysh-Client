using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Dyysh.Image
{

    class Capture
    {
        public static Screen[] allScreens = Screen.AllScreens;
        public static BitmapSource CaptureFullScreen()
        {
            return CaptureRegion(
                SystemInformation.VirtualScreen.Left,
                SystemInformation.VirtualScreen.Top,
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
        }
        public static BitmapSource CaptureFullScreen(int screenNumber)
        {
            return CaptureRegion(
                allScreens[screenNumber].Bounds.Left,
                allScreens[screenNumber].Bounds.Top,
                allScreens[screenNumber].Bounds.Width,
                allScreens[screenNumber].Bounds.Height);
        }
        public static BitmapSource CaptureRegion(int x, int y, int width, int height)
        {
            IntPtr sourceDC = IntPtr.Zero;
            IntPtr targetDC = IntPtr.Zero;
            IntPtr compatibleBitmapHandle = IntPtr.Zero;
            BitmapSource bitmap = null;

            try
            {
                // gets the main desktop and all open windows
                sourceDC = User32.GetDC(User32.GetDesktopWindow());
                //sourceDC = User32.GetDC(hWnd);
                targetDC = Gdi32.CreateCompatibleDC(sourceDC);

                // create a bitmap compatible with our target DC
                compatibleBitmapHandle = Gdi32.CreateCompatibleBitmap(sourceDC, width, height);

                // gets the bitmap into the target device context
                Gdi32.SelectObject(targetDC, compatibleBitmapHandle);

                // copy from source to destination
                Gdi32.BitBlt(targetDC, 0, 0, width, height, sourceDC, x, y, Gdi32.SRCCOPY);

                // Here's the WPF glue to make it all work. It converts from an 
                // hBitmap to a BitmapSource. Love the WPF interop functions
                bitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    compatibleBitmapHandle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                throw new ScreenCaptureException(string.Format("Error capturing region X:{0} Y:{1} W:{2} H:{3}", x, y, width, height), ex);
            }
            finally
            {
                Gdi32.DeleteObject(compatibleBitmapHandle);

                User32.ReleaseDC(IntPtr.Zero, sourceDC);
                User32.ReleaseDC(IntPtr.Zero, targetDC);
            }

            return bitmap;
        }
    }
}
