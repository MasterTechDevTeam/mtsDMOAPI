using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MasterTechDMO.API.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the DMOUsers class
    public class DMOUsers : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ContactNo { get; set; }
        public string DateofBirth { get; set; }
        //public string UserType { get; set; }
        public Guid? OrgId { get; set; }
        public bool IsOrg { get; set; }
        public bool IsDeactive { get; set; }

    }
}
