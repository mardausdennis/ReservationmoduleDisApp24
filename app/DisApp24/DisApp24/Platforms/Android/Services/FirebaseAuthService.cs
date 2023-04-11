using System.Threading.Tasks;
using Firebase.Auth;
using DisApp24;
using DisApp24.Services;



namespace DisApp24.Services{ 
    public class FirebaseAuthService: IFirebaseAuthService
    {
        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = FirebaseAuth.Instance;
            var result = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
            return result.User.Uid;
        }
        public async Task<string> SignUpWithEmailPasswordAsync(string email, string password)
        {
            var authProvider = FirebaseAuth.Instance;
            var result = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
            return result.User.Uid;
        }
    }
}
