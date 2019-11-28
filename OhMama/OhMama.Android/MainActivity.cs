using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Spotify.Android.Appremote.Api;
using Java.Lang;
using OhMama.Droid.Model;
using Android.Content;
using Com.Spotify.Sdk.Android.Authentication;
using Xamarin.Forms;
using OhMama.Droid.Services;

namespace OhMama.Droid
{
    [Activity(Label = "OhMama", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IConnectorConnectionListener
    {
        public event SpotifyAuthenticatedHandler OnSpotifyAuthenticated;
        public event SpotifyConnectedHandler OnSpotifyConnected;

        public static MainActivity CurrentActivity { get; private set; }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            CurrentActivity = this;
            DependencyService.Register<SpotifyService>();
            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnConnected(SpotifyAppRemote p0)
        {
            OnSpotifyConnected?.Invoke(this, new SpotifyConnectedArgs(p0));
        }

        public void OnFailure(Throwable p0)
        {
            throw new ArgumentException(p0.Message);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if(requestCode == Settings.SpotifyAuthenticationRequestCode)
            {
                var response = AuthenticationClient.GetResponse((int)resultCode, data);

                if (response.GetType() == AuthenticationResponse.Type.Token)
                {
                    OnSpotifyAuthenticated?.Invoke(this, new SpotifyAuthenticationArgs(response.AccessToken));
                }
            }
        }
    }
}