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
            var canvasWindow = new CanvasWindow(new Dyysh.Image.CaptureGDI());
            canvasWindow.Show();
            canvasWindow.Activate();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    } 
}
