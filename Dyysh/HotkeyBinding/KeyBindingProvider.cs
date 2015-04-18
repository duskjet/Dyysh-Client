using System;
using System.Windows.Input;

namespace Dyysh.HotkeyBinding
{
    public class KeyBindingProvider
    {
        public static bool IsRecording { get; private set; }
        public KeyBinding CurrentKeyBinding { get; private set; }

        public event BindingEventHandler OnBindingFinished;

        public void KeyDown_ExtEventHandler(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (isRecording)
            {
                Key pressedKey = (e.Key == Key.System ? e.SystemKey : e.Key);

                // If key is modifier, logically assign it to modkeys flag collection
                if (IsModifierKey(pressedKey))
                {
                    var modifier = KeyToModifierkey(pressedKey);
                    _modKeys |= modifier;
                }

                // If key is ordinary key, assign it to field key
                else
                    _key = pressedKey;
            }
        }

        private void ClearHotkey()
        {
            _key = Key.None;
            _modKeys = ModifierKeys.None;

            CurrentKeyBinding = new KeyBinding(_key, _modKeys);

            isRecording = false;
            IsRecording = false;

            OnBindingFinished(this, EventArgs.Empty);
        }

        public void KeyUp_ExtEventHandler(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (isRecording)
            {
                Key pressedKey = (e.Key == Key.System ? e.SystemKey : e.Key);

                // If key is Backspace, clear the hotkey
                if (pressedKey == Key.Back)
                {
                    ClearHotkey();
                    return;
                }

                if (pressedKey == _key)
                {
                    CurrentKeyBinding = new KeyBinding(_key, _modKeys);

                    isRecording = false;
                    IsRecording = false;

                    OnBindingFinished(this, EventArgs.Empty);
                }
            }
        }

        public void StartRecording()
        {
            _key = Key.None; ;
            _modKeys = ModifierKeys.None;
            CurrentKeyBinding = null;

            isRecording = true;
            IsRecording = true;
        }

        private Key _key;
        private ModifierKeys _modKeys;
        private bool isRecording = false;

        private bool IsModifierKey(Key key)
        {
            if (key >= Key.LeftShift && key <= Key.RightAlt)
                return true;

            if (key == Key.LWin || key == Key.RWin)
                return true;

            // Return false if key is not Shift, Ctrl, Alt or Win
            return false;
        }

        private ModifierKeys KeyToModifierkey(Key key)
        {
            switch (key)
            {
                case Key.LWin:
                case Key.RWin:
                    return ModifierKeys.Windows;

                case Key.LeftShift:
                case Key.RightShift:
                    return ModifierKeys.Shift;

                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return ModifierKeys.Control;

                case Key.LeftAlt:
                case Key.RightAlt:
                    return ModifierKeys.Alt;

                default:
                    return ModifierKeys.None;
            }
        }
    }
}
