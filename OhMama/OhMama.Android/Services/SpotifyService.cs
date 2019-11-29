using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Spotify.Android.Appremote.Api;
using Com.Spotify.Sdk.Android.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OhMama.Droid.Model;
using OhMama.Services;

namespace OhMama.Droid.Services
{
    public class SpotifyService : ISpotifyService
    {                
        private IDisposable _spotifyConnectObservable;
        private IDisposable _spotifyAuthenticationObservable;

        public async Task PlaySong(string searchQuery)
        {
            _spotifyAuthenticationObservable = Observable.FromEventPattern<EventHandler<SpotifyAuthenticationArgs>, SpotifyAuthenticationArgs>(
                h => MainActivity.CurrentActivity.OnSpotifyAuthenticated += h,
                h => MainActivity.CurrentActivity.OnSpotifyAuthenticated -= h)                    
                    .Where(x => !string.IsNullOrEmpty(x.EventArgs.AccessToken))
                    .Select(x => x.EventArgs.AccessToken)
                    .Subscribe(x =>
                    {
                        SubscribeSpotifyAppRemote(searchQuery, x);
                    });

            Authenticate();
        }

        private void SubscribeSpotifyAppRemote(string searchQuery, string accessToken)
        {
            _spotifyConnectObservable = Observable.FromEventPattern<EventHandler<SpotifyConnectedArgs>, SpotifyConnectedArgs>(
                            h => MainActivity.CurrentActivity.OnSpotifyConnected += h,
                            h => MainActivity.CurrentActivity.OnSpotifyConnected -= h)
                                .Where(x => x.EventArgs.AppRemote != null)
                                .Select(x => x.EventArgs.AppRemote)
                                .Subscribe(async x =>
                                {                                    
                                    _spotifyAuthenticationObservable.Dispose();                                                                
                                    var songs = await Search(searchQuery, accessToken);
                                    x.PlayerApi.Play(songs.OrderBy(y => y.Popularity).First().Uri);
                                });

            Connect();
        }

        private void Connect()
        {
            var connectionParamsBuilder = new ConnectionParams.Builder(Settings.SpotifyClientId);
            connectionParamsBuilder.SetRedirectUri(Settings.SpotifyRedirectUri);
            connectionParamsBuilder.ShowAuthView(true);

            var connectionParams = connectionParamsBuilder.Build();

            SpotifyAppRemote.Connect(MainActivity.CurrentActivity, connectionParams, MainActivity.CurrentActivity);
        }

        private void Authenticate()
        {
            var builder = new AuthenticationRequest.Builder(Settings.SpotifyClientId, AuthenticationResponse.Type.Token, 
                Settings.SpotifyRedirectUri);

            builder.SetScopes(new string[] { "streaming", "app-remote-control" });
            var request = builder.Build();

            AuthenticationClient.OpenLoginActivity(MainActivity.CurrentActivity, Settings.SpotifyAuthenticationRequestCode, request);
        }

        private async Task<List<Track>> Search(string songQuery, string accessToken)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            //TODO: Encode query
            var response = await client.GetStringAsync($"https://api.spotify.com/v1/search?q={songQuery}&type=track&market=ES");
            return JObject.Parse(response).SelectToken("tracks").SelectToken("items").ToObject<List<Track>>();                        
        }
    }
}