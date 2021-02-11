using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter Username/EmailId")]
        [Display(Name = "Username")]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemberMe { get; set; }
    }
}
