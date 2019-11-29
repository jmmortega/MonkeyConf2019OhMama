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
using OhMama.Models;

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
            _connectCommand = ReactiveCommand.Create<ToyDevice>(PerformConnect);
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
                .Select(x => new ToyDevice(x.EventArgs.Device))
                .Where(x => !ToyDevices.Any(y => y.Name == x.Name))
                .Subscribe(x => ToyDevices.Add(x));
                
            Observable.FromEventPattern<EventHandler<DeviceRemovedEventArgs>, DeviceRemovedEventArgs>(
                h => _toyService.Client.DeviceRemoved += h,
                h => _toyService.Client.DeviceRemoved -= h)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(x => x.EventArgs.Device != null)
                .Select(x => x.EventArgs.Device)
                .Subscribe(x => ToyDevices.Remove(ToyDevices.FirstOrDefault(y => y.Name == x.Name)));                
        }

        private ToyDevice _deviceSelected;

        public ToyDevice DeviceSelected
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

        private ObservableCollection<ToyDevice> _toyDevices = new ObservableCollection<ToyDevice>();
        public ObservableCollection<ToyDevice> ToyDevices
        {
            get => _toyDevices;
            set => this.RaiseAndSetIfChanged(ref _toyDevices, value);
        }

        private ReactiveCommand<Unit,Unit> _findCommand;
        private ReactiveCommand<ToyDevice, Unit> _connectCommand;
        private ReactiveCommand<Unit,Unit> _vibrateCommand;
        private ReactiveCommand<Unit,Unit> _stopCommand;

        public ReactiveCommand<Unit, Unit> FindCommand => _findCommand;
        public ReactiveCommand<ToyDevice, Unit> ConnectCommand => _connectCommand;
        public ReactiveCommand<Unit, Unit> VibrateCommand => _vibrateCommand;
        public ReactiveCommand<Unit, Unit> StopCommand => _stopCommand;

        public IObservable<bool> CanOperate => this.WhenAnyValue(x => x.DeviceSelected, x => x.ToyDevices.Count,
                                                                    (device, count) => device != null && count > 0);
        private Task PerformFind()
            => _toyService.Find();

        private void PerformConnect(ToyDevice arg)
            => DeviceSelected = arg;

        private async Task PerformVibrate()
        {
            string song = "Never, Never Gonna Give ya up barry white";
            if(!string.IsNullOrEmpty(SearchQuery))
            {
                song = SearchQuery;
            }
            await _spotifyService.PlaySong(song);
            await Task.Delay(1000);
            await _toyService.Vibrate(DeviceSelected);            
        }
            

        private Task PerformStop()
        {
            _spotifyService.StopSong();
            _toyService.Stop(DeviceSelected);
            return Task.CompletedTask;
        }
            
    }
}

