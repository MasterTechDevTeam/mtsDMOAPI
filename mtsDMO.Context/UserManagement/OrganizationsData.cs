using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.UserManagement
{
    public class OrganizationsData
    {
        public OrganizationsData(Guid orgId,string firstname,string lastname)
        {
            Name = $"{firstname} {lastname}";
            OrgId = orgId;
        }

        public Guid OrgId { get; set; }
        public string Name { get; set; }
    }
}
