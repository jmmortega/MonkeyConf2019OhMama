using System;
using Com.Spotify.Android.Appremote.Api;

namespace OhMama.Droid.Model
{
    public class SpotifyConnectedArgs : EventArgs
    {
        public SpotifyConnectedArgs(SpotifyAppRemote appRemote)
        {
            AppRemote = appRemote;
        }

        public SpotifyAppRemote AppRemote { get; private set; }
    }
}