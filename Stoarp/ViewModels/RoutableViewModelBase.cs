using ReactiveUI;

namespace Stoarp.ViewModels;

public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    public abstract string UrlPathSegment { get; }
    public IScreen HostScreen { get; }

    protected RoutableViewModelBase(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }
}
