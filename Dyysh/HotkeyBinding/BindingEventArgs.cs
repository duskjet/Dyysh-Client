using System;

namespace Dyysh.HotkeyBinding
{
    public delegate void BindingEventHandler(object source, EventArgs e);
    public class BindingEventArgs : EventArgs
    {
        public BindingEventArgs()
        {

        }
    }
}
