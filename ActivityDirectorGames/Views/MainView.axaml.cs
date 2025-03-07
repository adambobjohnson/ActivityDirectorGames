using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using System.Threading.Tasks;

namespace ActivityDirectorGames.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void ToggleTheme_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Application.Current is Application app)
        {
            if (app.ActualThemeVariant == ThemeVariant.Dark)
            {
                app.RequestedThemeVariant = ThemeVariant.Light;
            }
            else
            {
                app.RequestedThemeVariant = ThemeVariant.Dark;
            }
        }
    }

    private async void Play_Bingo(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WheelOfFortuneView.IsVisible =
        TombolaView.IsVisible =
        PokenoView.IsVisible =
        ChooseGame.IsVisible = false;

        BingoView.IsVisible =
        btnNewGame.IsVisible = true;
        // allow the screen to draw
        await Task.Delay(40);

        BingoView.WatchTextBlocks();
    }

    private async void Play_Pokeno(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WheelOfFortuneView.IsVisible =
        TombolaView.IsVisible =
        BingoView.IsVisible =
        ChooseGame.IsVisible = false;

        PokenoView.IsVisible = 
        btnNewGame.IsVisible = true;
        // allow the screen to draw
        await Task.Delay(40);

        PokenoView.WatchTextBlocks();
    }

    private async void Play_WheelOfFortune(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await Task.Yield();

        TombolaView.IsVisible =
        BingoView.IsVisible =
        PokenoView.IsVisible =
        ChooseGame.IsVisible = false;

        WheelOfFortuneView.IsVisible =
        btnNewGame.IsVisible = true;

        await Task.Delay(40);
        WheelOfFortuneView.SetupScreen();
    }

    private async void Play_Tombola(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WheelOfFortuneView.IsVisible =
        PokenoView.IsVisible =
        BingoView.IsVisible =
        ChooseGame.IsVisible = false;

        TombolaView.IsVisible =
        btnNewGame.IsVisible = true;
        // allow the screen to draw

        await Task.Delay(40);

        TombolaView.WatchTextBlocks();
    }

    private async void NewGame_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WheelOfFortuneView.IsVisible =
        btnNewGame.IsVisible =
        PokenoView.IsVisible =
        TombolaView.IsVisible =
        BingoView.IsVisible = false;
        ChooseGame.IsVisible = true;

        // allow the screen to draw
        await Task.Delay(40);
    }
}