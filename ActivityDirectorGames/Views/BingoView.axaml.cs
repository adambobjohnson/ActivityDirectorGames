using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using Avalonia.VisualTree;
using System.Linq;
using Avalonia.Styling;

namespace ActivityDirectorGames.Views;

public enum GameMode
{
    Normal,
    Blackout,
    Odds,
    Evens
}

public partial class BingoView : UserControl
{
    private IBrush? selectedBrush = new BrushConverter().ConvertFrom("#FF60FD59") as IBrush;
    private DispatcherTimer dispatcherTimer = new DispatcherTimer();

    public BingoView()
    {
        InitializeComponent();

        // Set up the ModeSelector ComboBox
        ModeSelector.ItemsSource = Enum.GetValues(typeof(GameMode));
        ModeSelector.SelectedIndex = 0; // Default to Normal mode
        UpdateLetterBorders(GameMode.Normal); // Set initial letter border styles
        MarkNumbersBasedOnMode(GameMode.Normal); // Mark numbers based on initial mode

        string str = "10";
        this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
        this.dispatcherTimer.Interval = TimeSpan.FromSeconds((double)Convert.ToInt32(str));
    }

    private void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        this.SelectedNumberPanel.IsVisible = false;
        this.dispatcherTimer.Stop();
    }

    private void ModeSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedMode = (GameMode)ModeSelector.SelectedItem;
        BlackoutStack.IsVisible = selectedMode == GameMode.Blackout;
        OddsStack.IsVisible = selectedMode == GameMode.Odds;
        EvensStack.IsVisible = selectedMode == GameMode.Evens;
        UpdateLetterBorders(selectedMode);
        MarkNumbersBasedOnMode(selectedMode); // Mark numbers when mode changes
    }

    private void UpdateLetterBorders(GameMode mode)
    {
        var borders = new[] { BorderB, BorderI, BorderN, BorderG, BorderO };
        foreach (var border in borders)
        {
            border.Classes.Clear();
            if (mode == GameMode.Blackout)
            {
                border.Classes.Add("BingoIsBlackout");
            }
            else // Normal, Odds, Evens use the same letter styling as Normal
            {
                var letter = (border.Child as Label)?.Content as string;
                if (letter == "B" || letter == "N" || letter == "O")
                {
                    border.Classes.Add("BingoRedLetter");
                }
                else if (letter == "I" || letter == "G")
                {
                    border.Classes.Add("BingoBlueLetter");
                }
            }
        }
    }

    private void MarkNumbersBasedOnMode(GameMode mode)
    {
        foreach (Label child in gameBoard.GetVisualDescendants().OfType<Label>())
        {
            var labelContent = child.Content?.ToString() ?? "";
            if (labelContent.Length == 2 && child.FindAncestorOfType<Border>() is Border childBorder)
            {
                if (int.TryParse(labelContent, out int number))
                {
                    bool isOdd = number % 2 != 0;
                    bool shouldMark = (mode == GameMode.Odds && isOdd) || (mode == GameMode.Evens && !isOdd);
                    childBorder.Background = shouldMark ? selectedBrush : Brushes.Transparent;
                }
            }
        }
    }

    private void BtnClear_Click(object? sender, RoutedEventArgs e)
    {
        if (gameBoard == null)
            return;

        var selectedMode = (GameMode)ModeSelector.SelectedItem;
        MarkNumbersBasedOnMode(selectedMode); // Reset board while preserving mode-specific markings
    }

    internal void WatchTextBlocks()
    {
        foreach (Border border in this.GetVisualDescendants().OfType<Border>())
        {
            foreach (Label label in border.GetVisualDescendants().OfType<Label>())
            {
                if (label != this.SelectedNumber && label.Content is string c && c.Length > 1)
                {
                    border.PointerPressed -= Item_PointerPressed;
                    border.PointerPressed += Item_PointerPressed;
                }
            }
        }
    }

    private void Item_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var border = sender as Border;
        var child1 = sender as Label;

        if (e.ClickCount != 1 || (border == null && child1 == null))
            return;

        if (child1 == null)
        {
            if (border.FindDescendantOfType<Label>() is not Label child)
                return;

            child1 = child;
        }
        else
        {
            if (child1.FindAncestorOfType<Border>() is not Border parent)
                return;

            border = parent;
        }

        if (border == null || child1 == null)
            return;

        // Parse the number
        if (!int.TryParse(child1.Content as string, out int number))
            return; // If not a number, do nothing

        // Check if the number is pre-marked in the current mode
        var selectedMode = (GameMode)ModeSelector.SelectedItem;
        bool isOdd = number % 2 != 0;
        bool shouldBeMarked = (selectedMode == GameMode.Odds && isOdd) || (selectedMode == GameMode.Evens && !isOdd);
        if (shouldBeMarked)
        {
            return; // Ignore the click if the number is pre-marked
        }

        // Proceed with existing logic for non-pre-marked numbers
        if (border.Background != this.selectedBrush && border.Background != Brushes.Yellow)
        {
            foreach (Label child2 in this.GetVisualDescendants().OfType<Label>())
            {
                if (child2 != this.SelectedNumber)
                {
                    if (child2.FindAncestorOfType<Border>() is Border parent2 && parent2.Background == Brushes.Yellow)
                    {
                        parent2.Background = this.selectedBrush;
                        parent2.UpdateLayout();
                    }
                }
            }

            var startingLetter = "";
            if (number <= 15)
            {
                startingLetter = "B - ";
            }
            else if (number <= 30)
            {
                startingLetter = "I - ";
            }
            else if (number <= 45)
            {
                startingLetter = "N - ";
            }
            else if (number <= 60)
            {
                startingLetter = "G - ";
            }
            else
            {
                startingLetter = "O - ";
            }

            this.SelectedNumber.Content = startingLetter + child1.Content as string;
            this.SelectedNumber.Styles.Clear();
            this.SelectedNumber.Styles.AddRange(child1.Styles);
            this.SelectedNumberPanel.IsVisible = true;
            border.Background = Brushes.Yellow;

            if (child1.FindAncestorOfType<ThemeVariantScope>() is ThemeVariantScope thm)
            {
                thm.RequestedThemeVariant = ThemeVariant.Light;
            }

            this.dispatcherTimer.Start();
        }
        else
        {
            border.Background = Brushes.Transparent;
            if (child1.FindAncestorOfType<ThemeVariantScope>() is ThemeVariantScope thm)
            {
                thm.RequestedThemeVariant = ThemeVariant.Default;
            }
        }
    }
}