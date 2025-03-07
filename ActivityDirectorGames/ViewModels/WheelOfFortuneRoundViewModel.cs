using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityDirectorGames.ViewModels
{
    public class WheelOfFortuneRoundViewModel: ViewModelBase
    {
        [Reactive]
        public bool NeedsNewGame { get; set; } = true;


        [ObservableAsProperty]
        public bool CanStartGame { get; }

        [ObservableAsProperty]
        public bool ShowWarning
        {
            get;
        }

        [Reactive]
        public bool IsSolved { get; set; } = false;

        [Reactive]
        public bool GameHasStarted
        {
            get; set;
        } = false;
        public WheelOfFortuneRoundViewModel()
        {
            this.WhenAnyValue(x => x.RowOneText)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(x => this.RowOneText = (x ?? "").ToUpper());

            this.WhenAnyValue(x => x.RowTwoText)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(x =>
                {
                    this.RowTwoText = (x ?? "").ToUpper();
                });

            this.WhenAnyValue(x => x.RowThreeText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(x => this.RowThreeText = (x ?? "").ToUpper());

            this.WhenAnyValue(x => x.RowFourText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(x => this.RowFourText = (x ?? "").ToUpper());

            this.WhenAnyValue(x => x.Title)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(x => this.Title = (Title ?? "").ToUpper());

            this.WhenAnyValue(x => x.RowOneText, (x) =>
                FormatRowText(0, x))
                .ToPropertyEx(this, x => x.RowOneTextFormatted);

            this.WhenAnyValue(x => x.RowTwoText, (x) =>
                FormatRowText(1, x))
                .ToPropertyEx(this, x => x.RowTwoTextFormatted);

            this.WhenAnyValue(x => x.RowThreeText, (x) =>
                FormatRowText(2, x))
                .ToPropertyEx(this, x => x.RowThreeTextFormatted);

            this.WhenAnyValue(x => x.RowFourText, (x) =>
                FormatRowText(3, x))
                .ToPropertyEx(this, x => x.RowFourTextFormatted);

            this.WhenAnyValue(a => a.RowOneText, b => b.RowTwoText, c => c.RowThreeText, d => d.RowFourText, e => e.Title)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe((x) => NeedsNewGame = true);

            this.WhenAnyValue(a => a.RowOneText, b => b.RowTwoText, c => c.RowThreeText, d => d.RowFourText, e => e.Title, f => f.NeedsNewGame
                , (a, b, c, d, title, needsGame) => needsGame && (title ?? "").Length > 0 && (a + b + c + d).Length > 0)
            .ToPropertyEx(this, x => x.CanStartGame);

            this.WhenAnyValue(e => e.NeedsNewGame, f => f.GameHasStarted
                , (needsGame, gameStarted) => gameStarted && !needsGame)
            .ToPropertyEx(this, x => x.ShowWarning);
        }

        [Reactive]
        public string PlayedLetters { get; set; } = "";

        private string FormatRowText(int rowNumber, string text)
        {
            if (text == null)
                text = "";

            var totalLength = 14;
            if (rowNumber == 0 || rowNumber == 3)
                totalLength = 12;

            if (text.Length < totalLength)
            {
                var appendToEnd = (totalLength - text.Length) / 2;
                var appendToStart = appendToEnd;

                if ((totalLength - text.Length) % 2 > 0)
                    appendToStart += 1;

                text = $"{new string('_', appendToStart)}{text}{new string('_', appendToEnd)}";
            }

            return text.Replace(' ', '_');
        }

        [MaxLength(12)]
        [Reactive]
        public string RowOneText { get; set; } = "";

        [MaxLength(14)]
        [Reactive]
        public string RowTwoText { get; set; } = "";

        [MaxLength(14)]
        [Reactive]
        public string RowThreeText { get; set; } = "";

        [MaxLength(12)]
        [Reactive]
        public string RowFourText { get; set; } = "";

        [Reactive]
        public int RoundNumber { get; set; }

        [ObservableAsProperty]
        public string? RowOneTextFormatted { get; }

        [ObservableAsProperty]
        public string? RowTwoTextFormatted { get; }
        [ObservableAsProperty]
        public string? RowThreeTextFormatted { get; }
        [ObservableAsProperty]
        public string? RowFourTextFormatted { get; }

        [Reactive]
        [MaxLength(50)]
        public string? Title { get; set; } = "";
    }
}
