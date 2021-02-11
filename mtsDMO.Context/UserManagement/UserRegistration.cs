using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
	public class UserRegistration
	{
		public Guid UserId { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string EmailId { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		public string ConfirmPassword { get; set; }

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

		[Required]
		public string Address { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string Zipcode { get; set; }

		[Required]
		public string State { get; set; }

		[Required]
		public string Country { get; set; }
	}
}
