using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.TextInput;
using System;
using System.Reflection;

namespace ActivityDirectorGames.Utilities // Adjust namespace as needed
{
    public static class TextBoxMobileKeyboardWorkaround
    {
        static TextBoxMobileKeyboardWorkaround()
        {
            ReportSelectionChangedToBrowserProperty.Changed.Subscribe(ReportSelectionChangedToBrowserChanged);
        }

        public static readonly AttachedProperty<bool> ReportSelectionChangedToBrowserProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>("ReportSelectionChangedToBrowser", typeof(TextBoxMobileKeyboardWorkaround), false);

        public static void SetReportSelectionChangedToBrowser(TextBox element, bool value) =>
            element.SetValue(ReportSelectionChangedToBrowserProperty, value);

        public static bool GetReportSelectionChangedToBrowser(TextBox element) =>
            element.GetValue(ReportSelectionChangedToBrowserProperty);

        public static void Register(TextBox textBox)
        {
            textBox.PropertyChanged += ReportSelectionChangedToBrowser_TextBoxPropertyChanged;
        }

        public static void Unregister(TextBox textBox)
        {
            textBox.PropertyChanged -= ReportSelectionChangedToBrowser_TextBoxPropertyChanged;
        }

        private static void ReportSelectionChangedToBrowser_TextBoxPropertyChanged(object? o, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is not TextBox textBox)
                return;

            if (e.Property == TextBox.SelectionStartProperty || e.Property == TextBox.SelectionEndProperty)
            {
                RaiseSurroundingTextChanged(textBox);
            }
        }

        private static void RaiseSurroundingTextChanged(TextBox textBox)
        {
            var imClientFieldInfo = typeof(TextBox).GetField("_imClient", BindingFlags.Instance | BindingFlags.NonPublic);
            var imClientRaw = imClientFieldInfo?.GetValue(textBox);
            if (imClientRaw is TextInputMethodClient imClient)
            {
                var methodInfo = typeof(TextInputMethodClient).GetMethod("RaiseSurroundingTextChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                methodInfo?.Invoke(imClient, []);
            }
        }

        private static void ReportSelectionChangedToBrowserChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            if (args.OldValue.GetValueOrDefault() && args.Sender is TextBox textBoxOld)
            {
                Unregister(textBoxOld);
            }
            if (args.NewValue.GetValueOrDefault() && args.Sender is TextBox textBoxNew)
            {
                Register(textBoxNew);
            }
        }
    }
}