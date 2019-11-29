using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OhMama.Services
{
    public interface ISpotifyService
    {
        Task PlaySong(string searchQuery);

        Task StopSong();
    }
}
