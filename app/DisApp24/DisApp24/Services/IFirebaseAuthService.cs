using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisApp24.Services
{
    public interface IFirebaseAuthService
    {
        Task<string> SignInWithEmailPasswordAsync(string email, string password);
        Task<string> SignUpWithEmailPasswordAsync(string email, string password);

        //Task<string> SignInWithGoogleAsync();
    }

}
