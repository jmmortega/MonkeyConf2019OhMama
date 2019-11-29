using Buttplug.Client;
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
        
        Task Vibrate(ButtplugClientDevice device);

        Task Stop(ButtplugClientDevice device);
    }
}
