using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ChatLe.ViewModels
{
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }

        public UpdatePasswordViewModel Passwords { get; set; }
    }
}
