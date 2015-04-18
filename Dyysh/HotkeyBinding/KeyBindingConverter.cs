using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Dyysh.HotkeyBinding
{
    class KeyBindingConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(
    ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                // "Shift+D1"

                //string[] parts = ((string)value).Split(new char[] { ',' });
                //Room room = new Room();
                //room.RoomNumber = Convert.ToInt32(parts[0]);
                //room.Location = parts.Length > 1 ? parts[1] : null;
                //return room;
                var keybindString = value as string;
                var splittedKeys = keybindString.Split('+');

                ModifierKeys modkeys = ModifierKeys.None;
                Key key = Key.None;

                foreach (string keyString in splittedKeys)
                {
                    if (keyString != splittedKeys[splittedKeys.Length - 1])
                        modkeys |= (ModifierKeys) Enum.Parse(typeof(ModifierKeys), keyString);
                    else
                        key = (Key) Enum.Parse(typeof(Key), keyString);
                }

                return new KeyBinding(key, modkeys);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
    ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
    object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                //Room room = value as Room;
                //return string.Format("{0},{1}", room.RoomNumber, room.Location);
                var keybind = value as KeyBinding;

                var modkeys = keybind.ModifierKeys;
                var formattedString = string.Empty;

                if (modkeys.HasFlag(ModifierKeys.Alt))
                    formattedString += "Alt+";

                if (modkeys.HasFlag(ModifierKeys.Control))
                    formattedString += "Ctrl+";

                if (modkeys.HasFlag(ModifierKeys.Shift))
                    formattedString += "Shift+";

                if (modkeys.HasFlag(ModifierKeys.Windows))
                    formattedString += "Win+";

                return formattedString + keybind.Key.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
