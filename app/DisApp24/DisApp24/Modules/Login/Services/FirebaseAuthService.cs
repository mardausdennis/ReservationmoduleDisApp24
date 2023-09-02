using System.Threading.Tasks;
using DisApp24.Services;
using Firebase.Database;
using Firebase.Database.Query;
using DisApp24.Models;
using CommunityToolkit.Mvvm.Messaging;

#if __ANDROID__
using Firebase.Auth;
using DisApp24;
#endif

#if __IOS__
using Firebase.Auth;
using Foundation;
using System.IdentityModel.Tokens.Jwt;
using PhoneNumbers;
using Contacts;
using Microsoft.Maui.ApplicationModel.Communication;
#endif

namespace DisApp24.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly AppConfig _config;

        public FirebaseAuthService()
        {
            _config = ServiceHelper.GetService<AppConfig>();
            _firebaseClient = new FirebaseClient(_config.FirebaseUrl);
        }

#if ANDROID
        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = FirebaseAuth.Instance;
            var result = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

            var appUser = await GetCurrentUserAsync();
            WeakReferenceMessenger.Default.Send(new UserChangedMessage(appUser));

            return result.User.Uid;
        }

        public async Task<AuthResult> SignUpWithEmailPasswordAsync(string email, string password, string firstName, string lastName, string phoneNumber)
        {
            var authProvider = FirebaseAuth.Instance;
            var result = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

            var userId = result.User.Uid;

            // Save the user information in the Firebase Realtime Database
            var userProfile = new
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };
            await _firebaseClient.Child($"users/{userId}").PutAsync(userProfile);

            var appUser = new AppUser
            {
                Uid = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            WeakReferenceMessenger.Default.Send(new UserChangedMessage(appUser));

            return new AuthResult { User = appUser };
        }

        public async Task<string> SignInWithGoogleAsync(string idToken, string accessToken)
        {
            var authProvider = FirebaseAuth.Instance;
            var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            var result = await authProvider.SignInWithCredentialAsync(credential);
            var userId = result.User.Uid;

            var appUser = await GetCurrentUserAsync();
            WeakReferenceMessenger.Default.Send(new UserChangedMessage(appUser));

            // Überprüfen, ob der Benutzer bereits in der Firebase Realtime Database vorhanden ist
            var existingProfile = await GetUserProfileAsync(userId);

            if (existingProfile == null)
            {
                // Benutzerinformationen in der Firebase Realtime Database speichern
                var userProfile = new
                {
                    FirstName = result.AdditionalUserInfo.Profile.ContainsKey("given_name") ? result.AdditionalUserInfo.Profile["given_name"].ToString() : "",
                    LastName = result.AdditionalUserInfo.Profile.ContainsKey("family_name") ? result.AdditionalUserInfo.Profile["family_name"].ToString() : ""
                };
                await _firebaseClient.Child($"users/{userId}").PutAsync(userProfile);
            }

            return userId;
        }


        public async Task<AppUser> GetCurrentUserAsync()
        {
            var authProvider = FirebaseAuth.Instance;
            var user = authProvider.CurrentUser;
            if (user != null)
            {
                // Benutzerprofil aus der Firebase Realtime Database abrufen
                var userProfile = await GetUserProfileAsync(user.Uid);

                // Verwende TryGetValue, um Werte aus dem Benutzerprofil abzurufen
                userProfile.TryGetValue("FirstName", out object firstName);
                userProfile.TryGetValue("LastName", out object lastName);

                // Erstelle ein AppUser-Objekt mit den Informationen aus dem Benutzerprofil
                return new AppUser
                {
                    Uid = user.Uid,
                    FirstName = firstName as string,
                    LastName = lastName as string,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
            }
            return null;
        }

        public bool IsSignedIn()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }

        public async Task SignOutAsync()
        {
            FirebaseAuth.Instance.SignOut();

            WeakReferenceMessenger.Default.Send(new UserChangedMessage(null));
            await Task.CompletedTask;
        }
#endif
#if IOS    
        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = Auth.DefaultInstance;
            var result = await authProvider.SignInWithPasswordAsync(email, password);
            return result.User.Uid;
        }

        public async Task<AuthResult> SignUpWithEmailPasswordAsync(string email, string password, string firstName, string lastName, string phoneNumber)
        {
            var authProvider = Auth.DefaultInstance;
            var result = await authProvider.CreateUserAsync(email, password);

            var userId = result.User.Uid;

            // Speichern der Benutzerinformationen in der Firebase Realtime Database
            var userProfile = new
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };
            await _firebaseClient.Child($"users/{userId}").PutAsync(userProfile);

            var appUser = new AppUser
            {
                Uid = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            return new AuthResult { User = appUser };
        }

        public async Task<string> SignInWithGoogleAsync(string idToken, string accessToken)
        {
            var authProvider = Auth.DefaultInstance;
            var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            var result = await authProvider.SignInWithCredentialAsync(credential);
            var userId = result.User.Uid;

            // Benutzerinformationen aus dem IdToken extrahieren
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);
            var givenName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "";
            var familyName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "";

            // Überprüfen, ob der Benutzer bereits in der Firebase Realtime Database vorhanden ist
            var existingProfile = await GetUserProfileAsync(userId);

            if (existingProfile == null)
            {
                // Benutzerinformationen in der Firebase Realtime Database speichern
                var userProfile = new
                {
                    FirstName = givenName,
                    LastName = familyName
                };
                await _firebaseClient.Child($"users/{userId}").PutAsync(userProfile);
            }

            return userId;
        }

         public async Task<AppUser> GetCurrentUserAsync()
        {
            var authProvider = Auth.DefaultInstance;
            var user = authProvider.CurrentUser;
            if (user != null)
            {
                // Benutzerprofil aus der Firebase Realtime Database abrufen
                var userProfile = await GetUserProfileAsync(user.Uid);

                // Verwende TryGetValue, um Werte aus dem Benutzerprofil abzurufen
                userProfile.TryGetValue("FirstName", out object firstName);
                userProfile.TryGetValue("LastName", out object lastName);


                // Erstelle ein AppUser-Objekt mit den Informationen aus dem Benutzerprofil
                return new AppUser
                {
                    Uid = user.Uid,
                    FirstName = firstName as string,
                    LastName = lastName as string,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
            }
            return null;
        }

        public bool IsSignedIn()
        {
            var authProvider = Auth.DefaultInstance;
            var user = authProvider.CurrentUser;
            return user != null;
        }

        public async Task SignOutAsync()
        {
            Auth.DefaultInstance.SignOut(out NSError error);
            await Task.CompletedTask;
        }

#endif

        public async Task<IDictionary<string, object>> GetUserProfileAsync(string userId)
        {
            var userProfile = await _firebaseClient.Child($"users/{userId}").OnceSingleAsync<IDictionary<string, object>>();
            return userProfile;
        }

        public async Task<bool> IsUserAccountValid(string userId)
        {
            var userExists = await _firebaseClient
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<AppUser>();

            return userExists != null;
        }
    }
}
