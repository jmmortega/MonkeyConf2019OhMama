using ReactiveUI;
using ReactiveUI.XamForms;
using System;

namespace OhMama.Base
{
    public abstract class BaseReactiveContentPage<TViewModel> : ReactiveContentPage<TViewModel> where TViewModel : BaseReactiveViewModel
    {
        public BaseReactiveContentPage()
        {
            ViewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel));

            this.WhenActivated(registerDisposable => CreateBindings(registerDisposable));
        }

        public virtual void CreateBindings(Action<IDisposable> registerDisposable)
        {
        }

        protected override void OnAppearing() => ViewModel.OnAppearing();
        
    }
}
