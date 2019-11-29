using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        }
    }
}