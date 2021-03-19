using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Areas.Identity.Data
{
    public class DMOUserFriendList : CommonField
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string EmailId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsRemoved { get; set; }
    }
}
