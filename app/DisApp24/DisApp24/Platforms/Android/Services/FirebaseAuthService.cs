using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;
using Firebase.Database;
using Firebase.Database.Query;
using DisApp24.Models;
using CommunityToolkit.Mvvm.Messaging;



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



        public async Task<IDictionary<string, object>> GetUserProfileAsync(string userId)
        {
            var userProfile = await _firebaseClient.Child($"users/{userId}").OnceSingleAsync<IDictionary<string, object>>();
            return userProfile;
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


        public async Task<List<Appointment>> GetReservationsAsync()
        {
            var firebaseAppointments = await _firebaseClient
        .Child("reservations")
        .OnceAsync<Appointment>();

            var appointments = new List<Appointment>();
            foreach (var item in firebaseAppointments)
            {
                var appointment = item.Object;
                appointment.Key = item.Key;
                appointments.Add(appointment);
            }

            return appointments;


        }

        public async Task<bool> IsUserAccountValid(string userId)
        {
            var userExists = await _firebaseClient
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<AppUser>();

            return userExists != null;
        }



        public bool IsSignedIn()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }

        public void SignOutAsync()
        {
            FirebaseAuth.Instance.SignOut();

            WeakReferenceMessenger.Default.Send(new UserChangedMessage(null));
        }

    }
}
