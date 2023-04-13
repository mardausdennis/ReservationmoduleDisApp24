using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;
using Foundation;
using Firebase.Database;
using Firebase.Database.Query;
using System.IdentityModel.Tokens.Jwt;

namespace DisApp24.Services
{ 
    public class FirebaseAuthService : IFirebaseAuthService
    {

        private readonly FirebaseClient _firebaseClient;

        public FirebaseAuthService()
        {
            _firebaseClient = new FirebaseClient("https://disapp24-reservation-module-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = Auth.DefaultInstance;
            var result = await authProvider.SignInWithPasswordAsync(email, password);
            return result.User.Uid;
        }

        public async Task<string> SignUpWithEmailPasswordAsync(string email, string password, string firstName, string lastName, string phoneNumber)
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

            return userId;
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


        public Task<AppUser> GetCurrentUserAsync()
        {
            var authProvider = Auth.DefaultInstance;
            var user = authProvider.CurrentUser;
            if (user != null)
            {
                return Task.FromResult(new AppUser
                {
                    Uid = user.Uid,
                    DisplayName = user.DisplayName,
                    Email = user.Email
                });
            }
            return Task.FromResult<AppUser>(null);
        }




        public async Task<IDictionary<string, object>> GetUserProfileAsync(string userId)
        {
            var userProfile = await _firebaseClient.Child($"users/{userId}").OnceSingleAsync<IDictionary<string, object>>();
            return userProfile;
        }


        public bool IsSignedIn()
        {
            var authProvider = Auth.DefaultInstance;
            var user = authProvider.CurrentUser;
            return user != null;
        }

        public void SignOutAsync()
        {
            Auth.DefaultInstance.SignOut(out NSError error);
        }


        }
}
