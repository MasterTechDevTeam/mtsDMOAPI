using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.Utility
{
    public class UserFriendData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string EmailId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
