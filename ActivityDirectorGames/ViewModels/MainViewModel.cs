using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace ActivityDirectorGames.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        ChangeTextCommand = ReactiveCommand.Create(ChangeText);

        this.WhenAnyValue(x => x.WindowWidth, y => y.WindowHeight, (x, y) => $"Your window size is {x} X {y}.")
            .ToPropertyEx(this, x => x.WindowSizeDescription);
    }

    public ReactiveCommand<Unit, Task> ChangeTextCommand { get; }

    [Reactive]
    public string Greeting { get; private set; } = "Welcome to Avalonia!";

    [Reactive]
    public double WindowHeight { get; set; }

    [ObservableAsProperty]
    public string? WindowSizeDescription { get; }

    [Reactive]
    public double WindowWidth { get; set; }

    internal void SizeChanged(double width, double height)
    {
        WindowWidth = width;
        WindowHeight = height;
    }

    private async Task ChangeText()
    {
        var deviceTypeName = "";
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
        {
            deviceTypeName = "mobile";
        }
        else if (OperatingSystem.IsWindows())
        {
            deviceTypeName = "Windows";
        }
        else if (OperatingSystem.IsBrowser() || OperatingSystem.IsWasi())
        {
            deviceTypeName = "browser";
        }
        else if (OperatingSystem.IsLinux())
        {
            deviceTypeName = "Linux";
        }
        else if (OperatingSystem.IsFreeBSD())
        {
            deviceTypeName = "FreeBSD";
        }
        else if (OperatingSystem.IsTvOS())
        {
            deviceTypeName = "TvOS";
        }
        else
        {
            deviceTypeName = "Mac";
        }

        await Task.Delay(4000);
        if (Greeting?.StartsWith("Welcome") == true)
        {
            Greeting = $"Hi from Avalonia! You're running on a {deviceTypeName} device.";
        }
        else
        {
            Greeting = $"Welcome to Avalonia! You're running on a {deviceTypeName} device.";
        }
    }
}