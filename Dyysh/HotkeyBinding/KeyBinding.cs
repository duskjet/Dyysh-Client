using System;
using System.Windows.Input;

namespace Dyysh.HotkeyBinding
{
    /// <summary>
    /// Class for storing and retrieving key combinations of Key and ModifierKeys enumerations.
    /// </summary>
    public class KeyBinding
    {
        private static readonly KeyBinding empty = new KeyBinding(Key.None, ModifierKeys.None);
        public static KeyBinding Empty { get { return empty; } }
        /// <summary>
        /// Creates new key combination with "None" properties.
        /// </summary>
        public KeyBinding() 
        {
            
        }

        /// <summary>
        /// Creates new key combination.
        /// </summary>
        /// <param name="key">Key enumeration value.</param>
        /// <param name="modifierKeys">ModifierKeys enumeration value</param>
        public KeyBinding(Key key, ModifierKeys modifierKeys)
        {
            _key = key;
            _modkeys = modifierKeys;
        }

        /// <summary>
        /// Creates new key combination.
        /// </summary>
        /// <param name="keyCombination">String representation of hotkey, which was created by overrided ToString() class method.</param>
        public KeyBinding(string keyCombination)
        {
            // Assign default values if the string is empty
            if (keyCombination == string.Empty || keyCombination == null)
            {
                _key = Key.None;
                _modkeys = ModifierKeys.None;
                return;
            }

            // Split keys and remember last index - it is Key enum value
            var keyCollection = keyCombination.Split('+');
            var maxIndex = keyCollection.Length - 1;

            foreach (string key in keyCollection)
            {
                if (key != keyCollection[maxIndex])
                    _modkeys |= (ModifierKeys)Enum.Parse(typeof(ModifierKeys), key);
                else
                    _key = (Key)Enum.Parse(typeof(Key), key);
            }
        }

        /// <summary>
        /// Modifies key combination from string.
        /// </summary>
        /// <param name="keyCombination">String representation of hotkey, which was created by overrided ToString() class method.</param>
        public void FromString(string keyCombination)
        {
            // Assign default values if the string is empty
            if (keyCombination == string.Empty || keyCombination == null)
            {
                _key = Key.None;
                _modkeys = ModifierKeys.None;
                return;
            }

            // Split keys and remember last index - it is Key enum value
            var keyCollection = keyCombination.Split('+');
            var maxIndex = keyCollection.Length - 1;

            foreach (string key in keyCollection)
            {
                if (key != keyCollection[maxIndex])
                    _modkeys |= (ModifierKeys)Enum.Parse(typeof(ModifierKeys), key);
                else
                    _key = (Key)Enum.Parse(typeof(Key), key);
            }
        }

        /// <summary>
        /// Returns string representation of keys in format "Modifier+Modifier+Key"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_key == Key.None)
                return "";

            string key = _key.ToString();
            string modifiers = string.Empty;

            if (_modkeys.HasFlag(ModifierKeys.Alt))
                modifiers += "Alt+";
            if (_modkeys.HasFlag(ModifierKeys.Control))
                modifiers += "Control+";
            if (_modkeys.HasFlag(ModifierKeys.Shift))
                modifiers += "Shift+";
            if (_modkeys.HasFlag(ModifierKeys.Windows))
                modifiers += "Windows+";
            
            return modifiers + key;
        }

        /// <summary>
        /// Key enumeration value.
        /// </summary>
        public Key Key
        {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// ModifierKeys enumeration value. 
        /// </summary>
        public ModifierKeys ModifierKeys
        {
            get { return _modkeys; }
            set { _modkeys = value; }
        }

        private Key _key = Key.None;
        private ModifierKeys _modkeys = ModifierKeys.None;

    }
}