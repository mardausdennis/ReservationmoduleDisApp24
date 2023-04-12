using Microsoft.Maui.Controls;
using System;
using DisApp24.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace DisApp24
{
    public partial class LoginPage : ContentPage
    {

        private readonly IFirebaseAuthService _firebaseAuthService;



        public LoginPage(IFirebaseAuthService firebaseAuthService)
        {

            InitializeComponent();
            _firebaseAuthService = firebaseAuthService;
            
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            try
            {
                var result = await _firebaseAuthService.SignInWithEmailPasswordAsync(email, password);
                // Erfolgreiche Anmeldung, Navigation zur Hauptseite oder einer anderen Seite
                await Navigation.PopModalAsync(); // Hier schließen wir die LoginPage
            }
            catch (Exception ex)
            {
                // Fehler bei der Anmeldung, eine Fehlermeldung anzeigen
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
        }

        private async void OnGoogleLoginButtonClicked(object sender, EventArgs e)
        {
            string codeVerifier = GenerateCodeVerifier();
            string codeChallenge = GenerateCodeChallenge(codeVerifier);

            try
            {
                string clientId, redirectUri;
                GetPlatformSpecificValues(out clientId, out redirectUri);

                var authUrl = GenerateAuthUrl(clientId, redirectUri);
                var authResult = await WebAuthenticator.AuthenticateAsync(new Uri(authUrl), new Uri(redirectUri));

                if (authResult != null && authResult.Properties.ContainsKey("code"))
                {
                    var code = authResult.Properties["code"];
                    var tokens = await ExchangeCodeForTokens(clientId, code, redirectUri);

                    if (tokens.IdToken != null && tokens.AccessToken != null)
                    {
                        var userId = await _firebaseAuthService.SignInWithGoogleAsync(tokens.IdToken, tokens.AccessToken);

                        // Erfolgreiche Anmeldung, Navigation zur Hauptseite oder einer anderen Seite
                        await Navigation.PopModalAsync(); 
                    }
                    else
                    {
                        Console.WriteLine("Failed to exchange authorization code for tokens");
                    }
                }
                else
                {
                    Console.WriteLine("Authentication result is null or does not contain code");
                }
            }
            catch (Exception ex)
            {
                // Fehler bei der Anmeldung, eine Fehlermeldung anzeigen
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
        }

        private void GetPlatformSpecificValues(out string clientId, out string redirectUri)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    clientId = "952625355122-f88mo6erna3vc15ksnrfajsi7nm2jb85.apps.googleusercontent.com";
                    redirectUri = "com.companyname.disapp24:/callback";
                    break;
                case Device.iOS:
                    clientId = "952625355122-2kd62r51vevn41l02fl01sl6fjgkh1td.apps.googleusercontent.com";
                    redirectUri = "com.companyname.disapp24:/callback";
                    break;
                default:
                    throw new NotSupportedException("Unsupported platform");
            }
        }

        private string GenerateAuthUrl(string clientId, string redirectUri)
        {
            return $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&response_type=code&scope=openid%20email%20profile&redirect_uri={redirectUri}";
        }

        private async Task<(string IdToken, string AccessToken)> ExchangeCodeForTokens(string clientId, string code, string redirectUri)
        {
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            var requestBody = new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"code", code},
                {"grant_type", "authorization_code"},
                {"redirect_uri", redirectUri}
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));

 

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenResult = JsonConvert.DeserializeObject<JObject>(content);

                var idToken = tokenResult.Value<string>("id_token");
                var accessToken = tokenResult.Value<string>("access_token");
                return (idToken, accessToken);
            }
            else
            {
                Console.WriteLine($"Failed to exchange authorization code for tokens");
                return (null, null);
            }
        }


        private string GenerateCodeVerifier()
        {
            using var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "")
                .Substring(0, 43);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = new SHA256Managed();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            return Convert.ToBase64String(challengeBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }


    private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Navigation zur RegistrationPage
            await Navigation.PushModalAsync(new RegistrationPage(_firebaseAuthService));
        }


        private void OnGuestContinueButtonClicked(object sender, EventArgs e)
        {
            // Als Gast fortfahren
            Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            // Navigieren Sie zur RssPage, wenn die Hardware-Zurück-Taste gedrückt wird
            Dispatcher.DispatchAsync(async () => await NavigateBack());

            // Return true, um die Standard-Verarbeitung der Zurück-Taste zu verhindern
            return true;
        }

        private async Task NavigateBack()
        {
            await Shell.Current.GoToAsync("//rssPage"); ;
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await NavigateBack();
        }

    }
}
