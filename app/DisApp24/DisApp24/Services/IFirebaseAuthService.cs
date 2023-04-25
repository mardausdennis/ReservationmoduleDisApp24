using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;

namespace DisApp24.Services
{
    public interface IFirebaseAuthService
    {
        Task<string> SignInWithEmailPasswordAsync(string email, string password);
        Task<string> SignUpWithEmailPasswordAsync(string email, string password, string firstName, string lastName, string phoneNumber);
        Task<string> SignInWithGoogleAsync(string idToken, string accessToken);
        Task<IDictionary<string, object>> GetUserProfileAsync(string userId);
        Task<AppUser> GetCurrentUserAsync();
        Task<List<Appointment>> GetReservationsAsync();

        void SignOutAsync();
        bool IsSignedIn();
    }

}
