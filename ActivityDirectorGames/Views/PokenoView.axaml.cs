using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System.Configuration;
using System;
using Avalonia.VisualTree;
using System.Linq;
using Avalonia.Styling;

namespace ActivityDirectorGames.Views;

public partial class PokenoView : UserControl
{
    public PokenoView()
    {
        InitializeComponent();

        this.Loaded += PokenoView_Loaded;
        this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
        this.dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
    }

    private void PokenoView_Loaded(object? sender, RoutedEventArgs e)
    {
        this.btnClear.Click += BtnClear_Click;
    }

    private void BtnClear_Click(object? sender, RoutedEventArgs e)
    {
        foreach (Label child in this.GetVisualDescendants().OfType<Label>())
        {
            if (child.FindAncestorOfType<Border>() is Border childBorder)
            {
                childBorder.Background = Brushes.Transparent;
                if (child.FindAncestorOfType<ThemeVariantScope>() is ThemeVariantScope thm)
                {
                    thm.RequestedThemeVariant = ThemeVariant.Default;
                }
            }
        }
    }

    private IBrush? selectedBrush = new BrushConverter().ConvertFrom("#FF60FD59") as IBrush;
    private DispatcherTimer dispatcherTimer = new DispatcherTimer();


    private void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        this.SelectedNumberPanel.IsVisible = false;
        this.dispatcherTimer.Stop();
    }


    internal void WatchTextBlocks()
    {
        foreach (Border border in this.GetVisualDescendants().OfType<Border>())
        {
            foreach (Label label in border.GetVisualDescendants().OfType<Label>())
            {
                if (label != this.SelectedCard && label.Content is string c && c.Length > 0)
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
                if (child2 != this.SelectedCard)
                {
                    if (child2.FindAncestorOfType<Border>() is Border parent2 && parent2.Background == Brushes.Yellow)
                    {
                        parent2.Background = this.selectedBrush;
                        parent2.UpdateLayout();
                    }
                }
            }

            this.SelectedCard.Content = child1.Content;
            this.SelectedCard.Styles.Clear();
            this.SelectedCard.Styles.AddRange(child1.Styles);
            this.SelectedCard.Classes.Clear();
            this.SelectedCard.Classes.AddRange(child1.Classes.Where(c => !c.StartsWith(":")));
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