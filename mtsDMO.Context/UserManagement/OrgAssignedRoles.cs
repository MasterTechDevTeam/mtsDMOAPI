using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
    public class OrgAssignedRoles
    {
        public Guid Id { get; set; }
        public Guid OrgId { get; set; }
        public Guid RoleId { get; set; }
        public string DisplayName {get;set;}
    }
}
