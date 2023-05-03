using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;
using Foundation;
using Firebase.Database;
using Firebase.Database.Query;
using System.IdentityModel.Tokens.Jwt;
using DisApp24.Models;

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
                    Email = user.Email
                };
            }
            return null;
        }

        public async Task<IDictionary<string, object>> GetUserProfileAsync(string userId)
        {
            var userProfile = await _firebaseClient.Child($"users/{userId}").OnceSingleAsync<IDictionary<string, object>>();
            return userProfile;
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
