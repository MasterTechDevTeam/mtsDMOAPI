using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
    public class UserVM
    {
        public  UserVM()
        {
            UserAddressDetails = new UserProfile();
            UserRegistration = new UserRegistration();
        }

        public UserRegistration UserRegistration { get; set; }
        public UserProfile UserAddressDetails { get; set; }
    }
}
