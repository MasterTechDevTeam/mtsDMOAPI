using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Areas.Identity.Data
{
    public class DMOOrgRoles
    {
        public Guid Id { get; set; }
        public Guid OrgId { get; set; }
        public Guid RoleId { get; set; }
        public string DisplayName { get; set; }
        public bool IsDeactive { get; set; }
    }
}
