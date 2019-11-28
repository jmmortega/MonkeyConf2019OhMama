using ReactiveUI;
using OhMama.Base;
using OhMama.Services;
using Xamarin.Forms;
using System.Windows.Input;
using System;
using System.Reactive;
using System.Threading.Tasks;

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
            _vibrateCommand = ReactiveCommand.CreateFromTask(PerformVibrate, CanVibrate);
            _stopCommand = ReactiveCommand.CreateFromTask(PerformStop, CanStop);
            
        }        
        private string _searchQuery;

        public string SearchQuery
        {
            get => _searchQuery;
            set => this.RaiseAndSetIfChanged(ref _searchQuery, value);
        }
        
        private ReactiveCommand<Unit,Unit> _findCommand;
        private ReactiveCommand<Unit,Unit> _vibrateCommand;
        private ReactiveCommand<Unit,Unit> _stopCommand;

        public ReactiveCommand<Unit, Unit> FindCommand => _findCommand;
        public ReactiveCommand<Unit, Unit> VibrateCommand => _vibrateCommand;
        public ReactiveCommand<Unit, Unit> StopCommand => _stopCommand;

        public IObservable<bool> CanVibrate { get; private set; }
        public IObservable<bool> CanStop { get; private set; }

        private Task<Unit> PerformFind()
        {
            throw new NotImplementedException();
        }

        private Task<Unit> PerformVibrate()
        {
            throw new NotImplementedException();
        }

        private Task<Unit> PerformStop()
        {
            throw new NotImplementedException();
        }        
    }
}

