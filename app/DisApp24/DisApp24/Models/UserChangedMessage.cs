
using CommunityToolkit.Mvvm.Messaging.Messages;


namespace DisApp24.Models
{
    public class UserChangedMessage : ValueChangedMessage<AppUser>
    {
        public UserChangedMessage(AppUser user) : base(user)
        {
        }
    }
}
