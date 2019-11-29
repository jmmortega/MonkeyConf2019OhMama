using Buttplug.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OhMama.Models
{
    public class ToyDevice
    {
        private ButtplugClientDevice _device;

        public ToyDevice(ButtplugClientDevice device)
        {
            _device = device;
        }

        public string Name => _device.Name;

        public Task SendVibration(double speed) => _device.SendVibrateCmd(speed);

        public Task StopVibration() => _device.StopDeviceCmd();
    }
}
