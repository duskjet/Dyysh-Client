using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dyysh.ContextMenuCommands
{
    public class CaptureArea : ICommand
    {
        public void Execute(object parameter)
        {
            //var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer.Tick += delegate(object s, EventArgs ev)
            //{
            //    dispatcherTimer.Stop();
            //    var canvasWindow = new CanvasWindow();
            //    canvasWindow.Show();
            //};
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0);
            //dispatcherTimer.Start();
            var canvasWindow = new CanvasWindow();
            canvasWindow.Show();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    } 
}
