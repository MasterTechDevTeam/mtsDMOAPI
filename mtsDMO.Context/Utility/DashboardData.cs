using mtsDMO.Context.UserManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace mtsDMO.Context.Utility
{
    public class DashboardData
    {
        public DashboardData()
        {
            Friends = new List<UserFriendData>();
            Tasks = new List<SchedulerData>();
            Templates = new List<TemplateData>();
            Users = new List<UserProfile>();
            Roles = new List<OrgAssignedRoles>();
        }

        public List<UserFriendData> Friends { get; set; }
        public List<SchedulerData> Tasks { get; set; }
        public List<TemplateData> Templates { get; set; }
        public List<UserProfile> Users { get; set; }
        public List<OrgAssignedRoles> Roles { get; set; }
    }
}
