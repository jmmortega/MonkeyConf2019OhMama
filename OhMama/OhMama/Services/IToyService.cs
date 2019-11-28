using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OhMama.Services
{
    public interface IToyService
    {
        Task Find();

        Task Connect();

        Task Vibrate();

        Task Stop();
    }
}
