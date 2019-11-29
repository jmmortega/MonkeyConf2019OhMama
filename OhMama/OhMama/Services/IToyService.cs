using Buttplug.Client;
using OhMama.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OhMama.Services
{
    public interface IToyService
    {
        ButtplugClient Client {get;}
        Task Init();

        Task Find();
        
        Task Vibrate(ToyDevice device);

        Task Stop(ToyDevice device);
    }
}
