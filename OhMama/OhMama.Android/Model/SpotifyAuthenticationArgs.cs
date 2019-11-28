using System;

namespace OhMama.Droid.Model
{
    public class SpotifyAuthenticationArgs : EventArgs
    {
        public SpotifyAuthenticationArgs(string accessToken)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; private set; }
    }
}