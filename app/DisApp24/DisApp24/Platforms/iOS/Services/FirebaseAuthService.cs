using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;



namespace DisApp24.Services
{ 
    public class FirebaseAuthService : IFirebaseAuthService
    {
        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = Auth.DefaultInstance;
            var result = await authProvider.SignInWithPasswordAsync(email, password);
            return result.User.Uid;
        }

        public async Task<string> SignUpWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = Auth.DefaultInstance;
            var result = await authProvider.CreateUserAsync(email, password);
            return result.User.Uid;
        }
    }
}
