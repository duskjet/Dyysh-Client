using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Dyysh.HotkeyBinding
{
    class KeyBindingToStringCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var keyBindingValue = (KeyBinding)value;
            var key = keyBindingValue.Key;
            var string_key = key.ToString();

            if (key >= Key.D0 && key <= Key.D9)
            {
                string_key = string_key.Trim('D');
            }

            switch (key)
            {
                case Key.Oem1:
                    string_key = ";"; break;
                case Key.Oem2:
                    string_key = "/"; break;
                case Key.Oem3:
                    string_key = "`"; break;
                case Key.Oem4:
                    string_key = "["; break;
                case Key.Oem5:
                    string_key = "\\"; break;
                case Key.Oem6:
                    string_key = "]"; break;
                case Key.Oem7:
                    string_key = "'"; break;
                case Key.Oem8:
                case Key.OemBackslash:
                    string_key = "\\"; break;
                case Key.OemPeriod:
                    string_key = "."; break;
                case Key.OemComma:
                    string_key = ","; break;
                case Key.Back:
                    string_key = "Backspace"; break;
                case Key.Capital:
                    string_key = "CapsLock"; break;
                case Key.OemPlus:
                    string_key = "+"; break;
                case Key.OemMinus:
                    string_key = "-"; break;
                case Key.Divide:
                    string_key = "NumPad/"; break;
                case Key.Multiply:
                    string_key = "NumPad*"; break;
                case Key.Subtract:
                    string_key = "NumPad-"; break;
                case Key.Add:
                    string_key = "NumPad+"; break;
                case Key.Decimal:
                    string_key = "NumPad."; break;
                default: break;
            }

            var modifierKeys = keyBindingValue.ModifierKeys;
            var string_modifierKeys = string.Empty;
            if (modifierKeys.HasFlag(ModifierKeys.Alt))
                string_modifierKeys += "Alt + ";

            if (modifierKeys.HasFlag(ModifierKeys.Control))
                string_modifierKeys += "Ctrl + ";

            if (modifierKeys.HasFlag(ModifierKeys.Shift))
                string_modifierKeys += "Shift + ";

            if (modifierKeys.HasFlag(ModifierKeys.Windows))
                string_modifierKeys += "Win + ";

            return string_modifierKeys + string_key;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
