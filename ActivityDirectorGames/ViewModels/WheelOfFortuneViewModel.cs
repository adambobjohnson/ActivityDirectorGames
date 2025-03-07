using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityDirectorGames.ViewModels
{
    internal class WheelOfFortuneViewModel : ViewModelBase
    {

        public WheelOfFortuneViewModel()
        {
            // We can use this to add some items for the designer. 
            // You can also use a DesignTime-ViewModel
            if (Design.IsDesignMode)
            {
                Rounds = new ObservableCollectionExtended<WheelOfFortuneRoundViewModel>(new[]
                {
                new WheelOfFortuneRoundViewModel() { Title = "Person, Place or Thing", RoundNumber = 1},
                });
            }
            else
            {
                Rounds = new ObservableCollectionExtended<WheelOfFortuneRoundViewModel>(new[]
                {
                new WheelOfFortuneRoundViewModel() { Title = "", RoundNumber = 1},
                });
            }

            this.CurrentRound = Rounds.First();

            AddNewRoundCommand = ReactiveCommand.Create(AddNewRound);
            RemoveRoundCommand = ReactiveCommand.Create<WheelOfFortuneRoundViewModel>(RemoveRound);

            Rounds.CollectionChanged += Rounds_CollectionChanged;

            Rounds.ToObservableChangeSet()
                .AutoRefresh(model => model.IsSolved) // Subscribe only to IsValid property changes
                .ToCollection()                      // Get the new collection of items
                .Subscribe(x => CheckCanGoNext());

            this.WhenAnyValue(x => x.CurrentRound)
                .Subscribe(x => CheckCanGoNext());

            //( x, y) => x != null && x.IsSolved && x != this.Rounds.LastOrDefault());
            //canGoNext.ToPropertyEx(this, x => x.CanGoNext);

            var roundsCanStart = Rounds
                .ToObservableChangeSet()
                .AutoRefresh(model => model.CanStartGame) // Subscribe only to IsValid property changes
                .ToCollection()                      // Get the new collection of items
                .Select(x => x.All(y => y.CanStartGame));

            roundsCanStart.ToPropertyEx(this, x => x.CanStartGame);

            this.WhenAnyValue(x => x.CurrentRound, (x) => x?.RoundNumber > 1)
                .ToPropertyEx(this, x => x.CanGoBack);
        }

        private object CheckCanGoNext()
        {
            this.CanGoNext = CurrentRound != null
                && CurrentRound.IsSolved
                && CurrentRound != this.Rounds.LastOrDefault();

            return this.CanGoNext;
        }


        [Reactive]
        public bool ShowBoardSetup { get; set; } = true;

        [ObservableAsProperty]
        public bool CanStartGame { get; }

        [ObservableAsProperty]
        public bool CanGoBack { get; } = false;

        private void Rounds_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var round in Rounds.Where(r => r.RoundNumber == 0))
            {
                round.RoundNumber = Rounds.IndexOf(round) + 1;
            }
        }

        [Reactive]
        public bool CanGoNext { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="ToDoItem"/> which allows adding and removing items
        /// </summary>
        public ObservableCollectionExtended<WheelOfFortuneRoundViewModel> Rounds { get; } = new ObservableCollectionExtended<WheelOfFortuneRoundViewModel>();
        public ReactiveCommand<Unit, Unit> AddNewRoundCommand { get; }
        public ReactiveCommand<WheelOfFortuneRoundViewModel, Unit> RemoveRoundCommand { get; }

        [Reactive]
        public WheelOfFortuneRoundViewModel? CurrentRound { get; set; }

        internal void AddNewRound()
        {
            var newRound = new WheelOfFortuneRoundViewModel() { Title = NewRoundTitle };
            // Add a new item to the list
            Rounds.Add(newRound);
            CurrentRound = newRound;
            ShowBoardSetup = CurrentRound != null;

            // reset the NewItemContent
            NewRoundTitle = null;

        }

        [Reactive]
        [MaxLength(50)]
        public string? NewRoundTitle { get; set; } = "";


        private void RemoveRound(WheelOfFortuneRoundViewModel item)
        {
            // Remove the given item from the list
            Rounds.Remove(item);

            CurrentRound = Rounds.LastOrDefault();
            ShowBoardSetup = CurrentRound != null;
        }

    }
}
