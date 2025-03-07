# Avalonia 11.0 Mobile browser keyboard workaround

In the Avalonia browser websites inside mobile devices is the touch keyboard non functional. You can get a somewhat working touch keyboard if you map the keys manually. The `keyboard.js` file contains the code for the browser to handle the keybindings. In my case i needed only german keybindings. I have only tested this on android mobile devices. I expected that it is also working with iOS devides. I don't know if this still works with the new `Avalonia 11.1` version.

## Create custom keybindings with `CollectKeyEvents.html`
You can create the required code for your supported keyboards with the helper file `CollectKeyEvents.html`. If you type inside the input element, the generated source code gets updated. Every line maps a character to the necessary keys for the input element. Those events are then processed by avalonia to insert the character inside the rendered view.

## Configure javascript part in browser project
Integrate the file `keyboard.js` and call the function `keyboard.initialize` with the `dotnetRuntime` as parameter:
```javascript
// Example: main.js

import { dotnet } from './_framework/dotnet.js'
import * as keyboard from './keyboard.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

dotnetRuntime.setModuleImports("main.js", {
    window: {
        location: {
            href: () => globalThis.window.location.href
        }
    },
});

keyboard.initialize(dotnetRuntime);

const mainAssemblyName = config.mainAssemblyName;
if (!mainAssemblyName)
    throw new Error("mainAssemblyName is undefined");

await dotnetRuntime.runMain(mainAssemblyName, [window.location.search]);
```

## Workaround for selection changes by Avalonia
Changes of the selection is not reported to the browser correctly. We workaround this by raising SurroundingTextChanged every time the selection of the `TextBox` changes. Here is the code of the helper file `TextBoxMobileKeyboardWorkaround.cs`.
```csharp
// TextBoxMobileKeyboardWorkaround.cs

static class TextBoxMobileKeyboardWorkaround
{
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

        if (e.Property == TextBox.SelectionStartProperty
            || e.Property == TextBox.SelectionEndProperty)
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
            // Treat changes to selection equally like SurroundingTextChanged handling.
            var methodInfo = typeof(TextInputMethodClient).GetMethod("RaiseSurroundingTextChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodInfo is null)
                return;

            methodInfo.Invoke(imClient, []);
        }
    }
}
```

### Helper attached property for easy setup
Now you need to call `TextBoxMobileKeyboardWorkaround.Register` on every instance of `TextBox`. You can create a helper attached property and set it via a style. Extend the class `TextBoxMobileKeyboardWorkaround` as followed:
```csharp
static class TextBoxMobileKeyboardWorkaround
{
    static TextBoxMobileKeyboardWorkaround()
    {
        ReportSelectionChangedToBrowserProperty.Changed.Subscribe(ReportSelectionChangedToBrowserChanged);
    }

    /// <summary>
    /// ReportSelectionChangedToBrowser AttachedProperty definition
    /// indicates automatic selection updates in browser native input control.
    /// </summary>
    public static readonly AttachedProperty<bool> ReportSelectionChangedToBrowserProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>("ReportSelectionChangedToBrowser", typeof(TextBoxBehavior), false);

    /// <summary>
    /// Accessor for Attached property <see cref="ReportSelectionChangedToBrowserProperty"/>.
    /// </summary>
    /// <param name="element">Target element</param>
    /// <param name="value">The value to set <see cref="ReportSelectionChangedToBrowserProperty"/>.</param>
    public static void SetReportSelectionChangedToBrowser(TextBox element, bool value) =>
        element.SetValue(ReportSelectionChangedToBrowserProperty, value);

    /// <summary>
    /// Accessor for Attached property <see cref="ReportSelectionChangedToBrowserProperty"/>.
    /// </summary>
    /// <param name="element">Target element</param>
    public static bool GetReportSelectionChangedToBrowser(TextBox element) =>
        element.GetValue(ReportSelectionChangedToBrowserProperty);

    private static void ReportSelectionChangedToBrowserChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.OldValue.GetValueOrDefault() == true)
        {
            if (args.Sender is TextBox textBox)
            {
                Unregister(textBox);
            }
        }
        if (args.NewValue.GetValueOrDefault().Equals(true))
        {
            if (args.Sender is TextBox textBox)
            {
                Register(textBox);
            }
        }
    }
}
```

Now you can activate the workaround via setting the `TextBoxMobileKeyboardWorkaround.ReportSelectionChangedToBrowser` attached property to `true`. You need to find a way to only set it to true if it is within the browser project.
