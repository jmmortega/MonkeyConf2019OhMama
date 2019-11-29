using Buttplug.Client;
using Buttplug.Core.Logging;
using Buttplug.Server;
using Buttplug.Server.Managers.XamarinBluetoothManager;
using OhMama.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OhMama.Services
{
    public class ToyService : IToyService
    {
        private ButtplugEmbeddedConnector _embeddedConnector;
        private ButtplugClient _client;

        public ButtplugClient Client => _client;

        public async Task Init()
        {
            _embeddedConnector = new ButtplugEmbeddedConnector("Oh mama Server");
            _embeddedConnector.Server.AddDeviceSubtypeManager<DeviceSubtypeManager>
                (alogger => new XamarinBluetoothManager(new ButtplugLogManager()));

            _client = new ButtplugClient("Oh mama client", _embeddedConnector);
            await _client.ConnectAsync();
        }
        
        public async Task Find()
            => await _client?.StartScanningAsync();


        public Task Vibrate(ToyDevice device)
            => device.SendVibration(0.5);

        public Task Stop(ToyDevice device)
            => device.SendVibration(0.0);
    }
}
