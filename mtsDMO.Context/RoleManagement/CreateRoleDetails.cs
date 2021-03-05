using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.RoleManagement
{
	public class CreateRoleDetails
	{
		public Guid UserId { get; set; }
		public string UserType { get; set; }
		public string RoleName { get; set; }
	}
}
