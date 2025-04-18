using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System.Configuration;
using System;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System.Linq;

namespace ActivityDirectorGames.Views;

public partial class TombolaView : UserControl
{
    public TombolaView()
    {
        InitializeComponent();
        //string familyName = ConfigurationManager.AppSettings["font-family"].ToString();
        this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
        this.dispatcherTimer.Interval = TimeSpan.FromSeconds(8);
        //this.FontFamily = new FontFamily(familyName);
        this.Round = 1;
        this.btnPrevRound.Click += BtnPrevRound_Click;
        this.btnNext.Click += BtnNext_Click;
        this.SetupRound();
    }

    private async void BtnNext_Click(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("Change Round?", string.Format("Are you sure you want to move to Round {0}?", (object)(this.Round + 1)),
                ButtonEnum.YesNo);

        this.GameGrid.IsVisible = !IsBrowser;

        var result = await box.ShowAsync();

        this.GameGrid.IsVisible = true;

        if (result != ButtonResult.Yes)
            return;

        ++this.Round;
        this.SetupRound();
    }

    public bool IsBrowser
    {
        get
        {
            return OperatingSystem.IsBrowser() || OperatingSystem.IsWasi();
        }
    }

    private async void BtnPrevRound_Click(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager
            .GetMessageBoxStandard("Change Round?", string.Format("Are you sure you want to move to Round {0}?", (object)(this.Round - 1)),
                ButtonEnum.YesNo);

        this.GameGrid.IsVisible = !IsBrowser;

        var result = await box.ShowAsync();

        this.GameGrid.IsVisible = true;

        if (result != ButtonResult.Yes)
            return;

        --this.Round;
        this.SetupRound();
    }

    private async void BtnClear_Click(object? sender, RoutedEventArgs e)
    {

        var box = MessageBoxManager
            .GetMessageBoxStandard("Start New Game?", "Are you sure you want to start a New Game?",
                ButtonEnum.YesNo);

        this.GameGrid.IsVisible = !IsBrowser;

        var result = await box.ShowAsync();

        this.GameGrid.IsVisible = true;

        if (result != ButtonResult.Yes)
            return;

        this.Round = 1;
        this.SetupRound();
    }

    private DispatcherTimer dispatcherTimer = new DispatcherTimer();
    private IBrush? selectedBrush = new BrushConverter().ConvertFrom((object)"#FF60FD59") as IBrush;


    private void SetupRound()
    {
        this.ClearHighlightedCells();
        this.btnPrevRound.IsEnabled = this.Round > 1;
        this.btnNext.IsEnabled = this.Round < 5;
        this.tbRoundNumber.Text = this.Round.ToString();
        if (this.Round < 5)
        {
            switch (this.Round)
            {
                case 1:
                    this.tbNumInRow.Text = "Two";
                    break;
                case 2:
                    this.tbNumInRow.Text = "Three";
                    break;
                case 3:
                    this.tbNumInRow.Text = "Four";
                    break;
                case 4:
                    this.tbNumInRow.Text = "Five";
                    break;
            }

            this.tbNumInRow.FontStyle = FontStyle.Italic;
            this.tbNumInRow.FontWeight = FontWeight.DemiBold;
            this.tbNumDescription.FontWeight = FontWeight.Normal;
            this.tbNumDescription.Text = "In A Row";
        }
        else
        {
            this.tbNumInRow.FontStyle = FontStyle.Normal;
            this.tbNumInRow.FontWeight = FontWeight.UltraBold;
            this.tbNumInRow.Text = "BLACK";
            this.tbNumDescription.FontWeight = FontWeight.Bold;
            this.tbNumDescription.Text = "OUT";
        }
    }

    private void ClearHighlightedCells()
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

    public int Round { get; set; }

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
                if (label != this.SelectedNumber && label.Content is string c && c.Length > 0
                    && c.Length < 3)
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

            this.SelectedNumber.Content = child1.Content;
            this.SelectedNumber.Styles.Clear();
            this.SelectedNumber.Styles.AddRange(child1.Styles);
            this.SelectedNumber.Classes.Clear();
            this.SelectedNumber.Classes.AddRange(child1.Classes.Where(c => !c.StartsWith(":")));
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