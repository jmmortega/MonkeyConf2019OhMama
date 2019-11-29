using ReactiveUI;
using OhMama.Base;
using OhMama.Services;
using Xamarin.Forms;
using System.Windows.Input;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Buttplug.Client;
using System.Reactive.Linq;
using System.Collections.ObjectModel;

namespace OhMama.ViewModels
{
    public class MainViewModel : BaseReactiveViewModel
    {
        private readonly ISpotifyService _spotifyService;
        private readonly IToyService _toyService;
        
        public MainViewModel()
        {
            _spotifyService = DependencyService.Get<ISpotifyService>();
            _toyService = DependencyService.Get<IToyService>();
            InitCommands();
        }

        private void InitCommands()
        {
            _findCommand = ReactiveCommand.CreateFromTask(PerformFind);
            _connectCommand = ReactiveCommand.Create<ButtplugClientDevice>(PerformConnect);
            _vibrateCommand = ReactiveCommand.CreateFromTask(PerformVibrate, CanOperate);            
            _stopCommand = ReactiveCommand.CreateFromTask(PerformStop, CanOperate);            
        }
        
        public async override void OnAppearing()
        {
            base.OnAppearing();
            await _toyService.Init();
            SubscribeToyEvents();
            await FindCommand.Execute();
        }

        private void SubscribeToyEvents()
        {
            Observable.FromEventPattern<EventHandler<DeviceAddedEventArgs>, DeviceAddedEventArgs>(
                h => _toyService.Client.DeviceAdded += h,
                h => _toyService.Client.DeviceAdded -= h)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(x => x.EventArgs.Device != null)
                .Select(x => x.EventArgs.Device)
                .Subscribe(x =>
                {
                    ButtPlugDevices.Add(x);
                    DeviceSelected = x;                    
                });
                

            Observable.FromEventPattern<EventHandler<DeviceRemovedEventArgs>, DeviceRemovedEventArgs>(
                h => _toyService.Client.DeviceRemoved += h,
                h => _toyService.Client.DeviceRemoved -= h)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(x => x.EventArgs.Device != null)
                .Select(x => x.EventArgs.Device)
                //.Subscribe(x => ButtPlugDevices.Remove(ButtPlugDevices.FirstOrDefault(y => y.Name == x.Name)));
                .Subscribe(x => DeviceSelected = null);
        }

        private ButtplugClientDevice _deviceSelected;

        private ButtplugClientDevice DeviceSelected
        {
            get => _deviceSelected;
            set => this.RaiseAndSetIfChanged(ref _deviceSelected, value);
        }

        private string _searchQuery;

        public string SearchQuery
        {
            get => _searchQuery;
            set => this.RaiseAndSetIfChanged(ref _searchQuery, value);
        }

        private ObservableCollection<ButtplugClientDevice> _buttPlugDevices = new ObservableCollection<ButtplugClientDevice>();
        public ObservableCollection<ButtplugClientDevice> ButtPlugDevices
        {
            get => _buttPlugDevices;
            set => this.RaiseAndSetIfChanged(ref _buttPlugDevices, value);
        }

        private ReactiveCommand<Unit,Unit> _findCommand;
        private ReactiveCommand<ButtplugClientDevice, Unit> _connectCommand;
        private ReactiveCommand<Unit,Unit> _vibrateCommand;
        private ReactiveCommand<Unit,Unit> _stopCommand;

        public ReactiveCommand<Unit, Unit> FindCommand => _findCommand;
        public ReactiveCommand<ButtplugClientDevice, Unit> ConnectCommand => _connectCommand;
        public ReactiveCommand<Unit, Unit> VibrateCommand => _vibrateCommand;
        public ReactiveCommand<Unit, Unit> StopCommand => _stopCommand;

        public IObservable<bool> CanOperate => this.WhenAnyValue(x => x.DeviceSelected, x => x.ButtPlugDevices.Count,
                                                                    (device, count) => device != null && count > 0);
        private Task PerformFind()
            => _toyService.Find();

        private void PerformConnect(ButtplugClientDevice arg)
            => DeviceSelected = arg;

        private async Task PerformVibrate()
        {
            await _toyService.Vibrate(DeviceSelected);
            await _spotifyService.PlaySong("can't get enough of your love");
        }
            

        private Task PerformStop()
            => _toyService.Stop(DeviceSelected);        
    }
}

