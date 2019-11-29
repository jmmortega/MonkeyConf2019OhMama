using Buttplug.Client;
using OhMama.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OhMama.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView 
    {
        public MainView()
        {
            InitializeComponent();
        }

        public override void CreateBindings(Action<IDisposable> d)
        {
            base.CreateBindings(d);

            d(this.Bind(ViewModel, vm => vm.SearchQuery, v => v.EntrySearchQuerySong.Text));            
            d(this.BindCommand(ViewModel, vm => vm.VibrateCommand, v => v.ButtonVibrate));
            d(this.BindCommand(ViewModel, vm => vm.StopCommand, v => v.ButtonStop));
            d(this.OneWayBind(ViewModel, vm => vm.ToyDevices, v => v.ListDevices.ItemsSource));
            
            var itemSelectedObservable = Observable.FromEventPattern<EventHandler<ItemTappedEventArgs>, ItemTappedEventArgs>(
                                        h => ListDevices.ItemTapped += h,
                                        h => ListDevices.ItemTapped -= h)
                .Select(x => x.EventArgs)
                .Where(x => x.Item != null)
                .Select(x => x.Item as ToyDevice)
                .InvokeCommand(ViewModel.ConnectCommand);


            this.ViewModel.CanOperate.Subscribe(canOperate =>
            {
                if (canOperate)
                {
                    AnimateAppearing();
                }
                else
                {
                    AnimateDissapearing();
                }
            });
                
        }

        private async void AnimateAppearing()
        {
            await FrameVibration.TranslateTo(0, 0, 400, Easing.SinIn).ContinueWith(async (t) =>
                await Task.Delay(250).ContinueWith(async (tt) => 
                await FrameStop.TranslateTo(0, 0, 400, Easing.SinIn)));            
        }

        private async void AnimateDissapearing()
        {
            await FrameVibration.TranslateTo(0, 200, 250, Easing.SpringIn);
            await FrameStop.TranslateTo(0, 200, 250, Easing.SpringIn);
        }
    }
}
 