using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using Avalonia.VisualTree;
using System.Linq;
using Avalonia.Styling;

namespace ActivityDirectorGames.Views;

public partial class BingoView : UserControl
{
    private IBrush? selectedBrush = new BrushConverter().ConvertFrom("#FF60FD59") as IBrush;
    private DispatcherTimer dispatcherTimer = new DispatcherTimer();
    public BingoView()
    {
        InitializeComponent();

        string str = "10"; 
        this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
        this.dispatcherTimer.Interval = TimeSpan.FromSeconds((double)Convert.ToInt32(str));
    }

    private void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        this.SelectedNumberPanel.IsVisible = false;
        this.dispatcherTimer.Stop();
    }

    private void BtnClear_Click(object? sender, RoutedEventArgs e)
    {
        if (gameBoard == null)
            return;

        foreach (Label child in gameBoard.GetVisualDescendants().OfType<Label>())
        {
            var labelLen = (child.Content?.ToString() ?? "").Length;
            if (labelLen == 2 &&  child.FindAncestorOfType<Border>() is Border childBorder)
            {
                childBorder.Background = Brushes.Transparent;
                if (child.FindAncestorOfType<ThemeVariantScope>() is ThemeVariantScope thm)
                {
                    thm.RequestedThemeVariant = ThemeVariant.Default;
                }
            }
        }
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

            var number = 0;
            var startingLetter = "";
            if (int.TryParse(child1.Content as string, out number))
            {
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