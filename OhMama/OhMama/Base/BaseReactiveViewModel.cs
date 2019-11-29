using ReactiveUI;

namespace OhMama.Base
{
    public abstract class BaseReactiveViewModel : ReactiveObject
    {
        public virtual void OnAppearing()
        { }
    }
}
