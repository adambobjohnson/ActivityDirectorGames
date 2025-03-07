using ActivityDirectorGames.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DynamicData.Aggregation;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActivityDirectorGames.Views;

public partial class WheelOfFortuneView : UserControl
{
    private string firstRow = new string('_', 12);
    private string secondRow = new string('_', 14);
    private string thirdRow = new string('_', 14);
    private string fourthRow = new string('_', 12);
    private string playedLetters = "";
    private string lastLetter = string.Empty;
    private DispatcherTimer dispatcherTimer = new DispatcherTimer();
    bool isSolved = false;

    private Regex lettersRegEx = new Regex("[A-Z]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private WheelOfFortuneViewModel model;
    public WheelOfFortuneView()
    {
        this.DataContext = model  = new WheelOfFortuneViewModel();
        InitializeComponent();
        this.dispatcherTimer.Tick += DispatcherTimer_Tick;
        this.dispatcherTimer.Interval = TimeSpan.FromSeconds(3);

        model.Rounds.CollectionChanged += Rounds_CollectionChanged;
    }

    private void Rounds_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            FirstRowTextBox.Focus();
        }
    }

    private async void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        this.LetterResultsPanel.IsVisible = false;
        this.dispatcherTimer.Stop();
        await LayoutBoard();
        if (model?.CurrentRound != null) 
            model.CurrentRound.IsSolved = isSolved;
    }

    private async Task ShowWheelOfFortuneBoard()
    {
        if (model?.CurrentRound == null)
            return;

        if (this.GameTab.IsVisible == true)
        {
            isSolved = model.CurrentRound.IsSolved;
            playedLetters = model.CurrentRound.PlayedLetters;

            WatchTextBlocks(model.CurrentRound.NeedsNewGame);

            if (model.CurrentRound.NeedsNewGame)
            {
                model.CurrentRound.IsSolved = isSolved = false;
                model.CurrentRound.PlayedLetters = playedLetters = "";
                model.CurrentRound.NeedsNewGame = false;
                lastLetter = "";
                await LayoutBoard();
            }

            await LayoutBoard();
        }
    }

    private string FormatRowText(string? rowText)
    {
        if (rowText == null)
            rowText = string.Empty;

        var regex = $"[^_/\\'!-.?{playedLetters}0-9]";
        
        var newString = Regex.Replace(rowText, regex, " ", RegexOptions.IgnoreCase);

        return newString;
    }

    private async Task LayoutBoard()
    {
        int counter = 0;
        var combinedString = GetCombinedFormattedPuzzleString();

        foreach (Label label in this.GetVisualDescendants().OfType<Label>().Where(l => l.Classes.Contains("Tile")))
        {
            var currentLetter = combinedString.Substring(counter, 1);
            counter += 1;

            label.Classes.Clear();
            label.Classes.Add("Tile");

            
            if (currentLetter == "_")
                label.Classes.Add("NotUsed");
            else
            {
                if (lettersRegEx.IsMatch(currentLetter.ToUpper()) && lastLetter.IndexOf(currentLetter.ToUpper(), StringComparison.OrdinalIgnoreCase) >=0 )
                {
                    label.Classes.Add("JustFound");
                }
                else
                {
                    label.Classes.Add("Used");
                }
                label.Content = currentLetter.ToUpper(); 
            }
        }

        // now reveal the shown tiles...
        await RevealLetters();

        // if the puzzes is solved, show the message...
        if (PuzzleIsSolved()
            && lastLetter.Length == 1) 
        {
            await ShowSolvedMessage();
        }

        lastLetter = string.Empty;
    }

    private async Task ShowSolvedMessage(bool avoidDelay = false)
    {
        isSolved = true;
        if (!avoidDelay)
            await Task.Delay(2000);

        LetterResultsBorder.Classes.Clear();
        LetterResultsBorder.Classes.Add("LetterFound");
        this.LetterResultsMessage.Content = $"Solved!  Congratulations!";
        this.LetterResultsPanel.IsVisible = true;
        dispatcherTimer.Start();
    }

    private bool PuzzleIsSolved()
    {
        return (GetCombinedRawPuzzleString()).Length > 0
            && Regex.Matches(GetCombinedFormattedPuzzleString(), " ").Count == 0;
    }

    private string GetCombinedFormattedPuzzleString()
    {
        return FormatRowText(model.CurrentRound?.RowOneTextFormatted) + FormatRowText(model.CurrentRound?.RowTwoTextFormatted) + FormatRowText(model.CurrentRound?.RowThreeTextFormatted) + FormatRowText(model.CurrentRound?.RowFourTextFormatted);
    }

    internal string GetCombinedRawPuzzleString()
    {
        return (model.CurrentRound?.RowOneText + model.CurrentRound?.RowTwoText + model.CurrentRound?.RowThreeText + model.CurrentRound?.RowFourText) ?? "";
    }

    private async Task RevealLetters()
    {
        foreach (Label label in this.GetVisualDescendants().OfType<Label>().Where(l => l.Classes.Contains("Tile") && l.Classes.Contains("JustFound")))
        {
            await Task.Delay(1000);
            label.Classes.Remove("JustFound");
            label.Classes.Add("Used");
        }
    } 


    internal void WatchTextBlocks(bool resetClasses = false)
    {
        foreach (Label label in this.GetVisualDescendants().OfType<Label>().Where(l => l.Classes.Contains("Alphabet")))
        {
            label.PointerPressed -= AlphabetLetterClicked;
            label.PointerPressed += AlphabetLetterClicked;

            if (resetClasses)
            {
                label.Classes.Clear();
                label.Classes.AddRange(new[] { "Alphabet", "Enabled"});
            }
            else
            {
                // highlight any characters that were played already...
                var letter = label.Content as string;
                if (letter == null) return;

                letter = letter.ToUpper();

                label.Classes.Clear();
                label.Classes.AddRange(new[] { "Alphabet", "Enabled" });

                if (playedLetters.Contains(letter))
                {
                    label.Classes.Clear();
                    label.Classes.AddRange(new[] { "Alphabet", "Disabled" });

                    var numberFound = Regex.Matches(model.CurrentRound?.RowOneText + model.CurrentRound?.RowTwoText + model.CurrentRound?.RowThreeText + model.CurrentRound?.RowFourText, letter, RegexOptions.IgnoreCase).Count;

                    LetterResultsBorder.Classes.Clear();
                    if (numberFound == 0)
                    {
                        label.Classes.Add("Fail");
                    }
                    else if (numberFound >= 1)
                    {
                        label.Classes.Add("Success");
                    }
                }
            }
        }
    }

    private void AlphabetLetterClicked(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (isSolved || model?.CurrentRound == null)
            return;

        var label = sender as Label;
        if (label == null) return;


        var letter = label.Content as string;
        if (letter == null) return;

        if (!label.Classes.Contains("Enabled"))
            return;

        letter = letter.ToUpper();

        label.Classes.Clear();
        label.Classes.AddRange(new[] { "Alphabet", "Disabled" });

        model.CurrentRound.PlayedLetters = playedLetters += letter;
        lastLetter = letter;

        var numberFound = Regex.Matches(model.CurrentRound?.RowOneText + model.CurrentRound?.RowTwoText + model.CurrentRound?.RowThreeText + model.CurrentRound?.RowFourText, letter, RegexOptions.IgnoreCase).Count;

        LetterResultsBorder.Classes.Clear();
        if (numberFound == 0)
        {
            LetterResultsBorder.Classes.Add("LetterNotFound");
            label.Classes.Add("Fail");
            this.LetterResultsMessage.Content = $"No '{lastLetter}'s Found";
        }
        else if (numberFound == 1) {
            label.Classes.Add("Success");
            LetterResultsBorder.Classes.Add("LetterFound");
            this.LetterResultsMessage.Content = $"One '{lastLetter}' Found";
        }
        else
        {
            label.Classes.Add("Success");
            LetterResultsBorder.Classes.Add("LetterFound");
            this.LetterResultsMessage.Content = $"{numberFound} '{lastLetter}'s Found";
        }
        this.LetterResultsPanel.IsVisible = true;
        dispatcherTimer.Start();
    }

    private async void PlayerSolved_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!isSolved && model?.CurrentRound!= null)
        {
            var regex = $"[^_/\\'!-.?{playedLetters}0-9]";

            var notUsed = Regex.Matches(GetCombinedRawPuzzleString(), regex, RegexOptions.IgnoreCase).
                Select(m => m.Groups[0].Value).Distinct();

            lastLetter = string.Join("",notUsed);
            model.CurrentRound.PlayedLetters = playedLetters += lastLetter;

            await ShowSolvedMessage(true);
        }
    }

    private void ClearFields_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (model?.CurrentRound == null)
            return;

        model.CurrentRound.RowOneText =
            model.CurrentRound.RowTwoText =
            model.CurrentRound.RowThreeText =
            model.CurrentRound.RowFourText =
            model.CurrentRound.Title = string.Empty;
    }

    private async void StartRound_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (model == null)
            return;

        model.CurrentRound = model.Rounds.FirstOrDefault(r => !r.IsSolved);
        if (model.CurrentRound == null) return;

        model.CurrentRound.GameHasStarted = true;
        this.GameTab.IsVisible = true;
        this.SetupTab.IsVisible = false;

        await Task.Delay(50);

        await ShowWheelOfFortuneBoard();
    }

    private async void NextRound_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (model == null)
            return;

        if (model.CurrentRound == null)
        {
            model.CurrentRound = model.Rounds.FirstOrDefault(r => !r.IsSolved);
        }
        else
        {
            model.CurrentRound = model.Rounds.FirstOrDefault(r => r.RoundNumber == model.CurrentRound.RoundNumber + 1);
        }

        if (model.CurrentRound == null) return;

        model.CurrentRound.GameHasStarted = true;
        this.GameTab.IsVisible = true;
        this.SetupTab.IsVisible = false;

        await Task.Delay(50);

        await ShowWheelOfFortuneBoard();
    }

    private async void PreviousRound_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (model == null)
            return;

        if (model.CurrentRound == null)
        {
            model.CurrentRound = model.Rounds.FirstOrDefault();
        }
        else
        {
            model.CurrentRound = model.Rounds.FirstOrDefault(r => r.RoundNumber == model.CurrentRound.RoundNumber - 1);
        }

        if (model.CurrentRound == null) return;

        model.CurrentRound.GameHasStarted = true;
        this.GameTab.IsVisible = true;
        this.SetupTab.IsVisible = false;

        await Task.Delay(50);

        await ShowWheelOfFortuneBoard();
    }

    private void GotToSteup_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.GameTab.IsVisible = false;
        this.SetupTab.IsVisible = true;
    }

    internal void SetupScreen()
    {
        if(model != null && model.ShowBoardSetup)
        {
            FirstRowTextBox.Focus();
        }
    }
}