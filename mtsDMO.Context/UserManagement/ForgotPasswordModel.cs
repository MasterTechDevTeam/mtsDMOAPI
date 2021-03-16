using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
    public class ForgotPasswordModel
    {
        [TempData]
        public string EmailId { get; set;}

        [TempData]
        public string Code { get; set; }

        [Required(ErrorMessage ="Please enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pleae enter Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
