using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
    public class UserDetails
    {
        public Guid UserId { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string DateofBirth { get; set; }

        [Required]
        public string UserType { get; set; }

        public string AssignedRole { get; set; }
        public Guid OrgId { get; set; }
        public bool IsVerified { get; set; }
    }
}
