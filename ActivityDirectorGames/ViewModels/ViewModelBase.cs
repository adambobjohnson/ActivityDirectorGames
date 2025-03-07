using ReactiveUI;
using System.Reactive.Disposables;

namespace ActivityDirectorGames.ViewModels;


public class ViewModelBase : ReactiveObject, IActivatableViewModel
{
    public ViewModelBase()
    {
        this.WhenActivated(disposables =>
        {
            /* Handle activation */
            Disposable.Create(() => { /* Handle deactivation */ })
                .DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
}