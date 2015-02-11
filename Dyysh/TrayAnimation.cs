using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Dyysh
{
    class TrayAnimation
    {
        private static ImageSourceConverter imgSrcConverter = new ImageSourceConverter();
        private static List<ImageSource> iconList = new List<ImageSource>();
        private static bool _animated = false;

        public static bool Animated
        {
            get { return _animated; }
            set 
            { 
                _animated = value;
                if (value) DoAnimation();
            }
        }
        
        private const int iconAmount = 12;
        private const long timerInterval = 1000000;
        public static void Init()
        {
            for (int number = 0; number < iconAmount; number++)
            {
                var fileName = "tray" + number + ".ico";
                var file = new Uri(@"pack://application:,,,/Resources/icons/ico/" + fileName);
                iconList.Add( (ImageSource)imgSrcConverter.ConvertFrom(file) );
            }
        }
        public static void Start()
        {
            _animated = true;

            DoAnimation();
        }

        public static void Stop()
        {
            _animated = false;
        }

        private static void DoAnimation()
        {
            var mainWindow = Application.Current.Windows
               .Cast<Window>()
               .FirstOrDefault(window => window is MainWindow) as MainWindow;

                var currentIconNumber = 0;

                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = new TimeSpan(timerInterval);
                timer.IsEnabled = true;
                timer.Tick += delegate(object s, EventArgs e)
                {
                    if (currentIconNumber >= iconAmount) currentIconNumber = 0;

                    mainWindow.notifyIcon.IconSource = iconList[currentIconNumber];

                    if (!_animated && currentIconNumber == 0) { timer.Stop(); }

                    currentIconNumber++;
                };
        }

    }
}
