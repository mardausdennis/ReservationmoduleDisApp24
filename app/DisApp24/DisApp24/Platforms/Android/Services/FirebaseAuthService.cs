using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;
using Firebase.Database;
using Firebase.Database.Query;


namespace DisApp24.Services{ 
    public class FirebaseAuthService: IFirebaseAuthService
    {

        private readonly FirebaseClient _firebaseClient;

        public FirebaseAuthService()
        {
            _firebaseClient = new FirebaseClient("https://disapp24-reservation-module-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = FirebaseAuth.Instance;
            var result = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
            return result.User.Uid;
        }

        public async Task<string> SignUpWithEmailPasswordAsync(string email, string password, string firstName, string lastName, string phoneNumber)
        {
            var authProvider = FirebaseAuth.Instance;
            var result = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

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
            var authProvider = FirebaseAuth.Instance;
            var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            var result = await authProvider.SignInWithCredentialAsync(credential);
            var userId = result.User.Uid;

            // �berpr�fen, ob der Benutzer bereits in der Firebase Realtime Database vorhanden ist
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



        public async Task<IDictionary<string, object>> GetUserProfileAsync(string userId)
        {
            var userProfile = await _firebaseClient.Child($"users/{userId}").OnceSingleAsync<IDictionary<string, object>>();
            return userProfile;
        }

        public Task<AppUser> GetCurrentUserAsync()
        {
            var firebaseUser = FirebaseAuth.Instance.CurrentUser;
            if (firebaseUser != null)
            {
                return Task.FromResult(new AppUser
                {
                    Uid = firebaseUser.Uid,
                    DisplayName = firebaseUser.DisplayName,
                    Email = firebaseUser.Email
                });
            }
            return Task.FromResult<AppUser>(null);
        }





        public bool IsSignedIn()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }

        public void SignOutAsync()
        {
            FirebaseAuth.Instance.SignOut();
        }

    }
}