using DisApp24.Models;
using DisApp24.Services;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace DisApp24.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly HttpClient _httpClient;
        private readonly NavigationService _navigation;


        public string Email { get; set; }
        public string Password { get; set; }

        public Command LoginCommand { get; }
        public Command GoogleLoginCommand { get; }
        public Command RegisterCommand { get; }
        public Command GuestContinueCommand { get; }
        public Command BackCommand { get; }
        public Command BackButtonCommand { get; }


        public LoginPageViewModel()
        {
            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();
            _httpClient = ServiceHelper.GetService<HttpClient>();
            _navigation = ServiceHelper.GetService<NavigationService>();

            LoginCommand = new Command(async () => await OnLoginAsync());
            GoogleLoginCommand = new Command(async () => await OnGoogleLoginAsync());
            RegisterCommand = new Command(async () => await OnRegisterAsync());
        }


        private async Task OnLoginAsync()
        {
            try
            {
                var result = await _firebaseAuthService.SignInWithEmailPasswordAsync(Email, Password);
                // Successful login, navigate to the main page or another page
                await _navigation.GetNavigation().PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task OnGoogleLoginAsync()
        {
            string codeVerifier = GenerateCodeVerifier();
            string codeChallenge = GenerateCodeChallenge(codeVerifier);

            try
            {
                string clientId, redirectUri;
                GetPlatformSpecificValues(out clientId, out redirectUri);

                var authUrl = GenerateAuthUrl(clientId, redirectUri, codeChallenge);
                var authResult = await WebAuthenticator.AuthenticateAsync(new Uri(authUrl), new Uri(redirectUri));

                if (authResult != null && authResult.Properties.ContainsKey("code"))
                {
                    var code = authResult.Properties["code"];
                    var tokens = await ExchangeCodeForTokens(clientId, code, redirectUri, codeVerifier);

                    if (tokens.IdToken != null && tokens.AccessToken != null)
                    {
                        var userId = await _firebaseAuthService.SignInWithGoogleAsync(tokens.IdToken, tokens.AccessToken);

                        // Successful login, navigate to the appropriate page
                        await _navigation.GetNavigation().PopAsync();

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
                await Shell.Current.DisplayAlert("Fehler", ex.Message, "OK");
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

        private string GenerateAuthUrl(string clientId, string redirectUri, string codeChallenge)
        {
            return $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&response_type=code&scope=openid%20email%20profile&redirect_uri={redirectUri}&code_challenge={codeChallenge}&code_challenge_method=S256";
        }


        private async Task<(string IdToken, string AccessToken)> ExchangeCodeForTokens(string clientId, string code, string redirectUri, string codeVerifier)
        {
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            var requestBody = new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"code", code},
                {"grant_type", "authorization_code"},
                {"redirect_uri", redirectUri},
                {"code_verifier", codeVerifier}
            };

            
            var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestBody));
            System.Diagnostics.Debug.WriteLine($"Response status code: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");



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

        private async Task OnRegisterAsync()
        {
            // Navigate to the RegistrationPage
            await AppShell.Current.GoToAsync(nameof(RegistrationPage)); 
        }


    }
}
