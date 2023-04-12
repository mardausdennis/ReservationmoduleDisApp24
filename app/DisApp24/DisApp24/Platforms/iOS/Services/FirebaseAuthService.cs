using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;
using Foundation;

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
        public async Task<string> SignInWithGoogleAsync(string idToken, string accessToken)
        {
            var authProvider = Auth.DefaultInstance;
            var credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            var result = await authProvider.SignInWithCredentialAsync(credential);
            return result.User.Uid;
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
