using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Models
{
    public static class Constants
    {
        public struct BaseRole
        {
            public const string Org = "Organization";
            public const string OrgUser = "OrganizationUser";
            public const string Indevidual = "SingleUser";
            public const string MTAdmin = "MTAdmin";
        }

        public struct TokenType
        {
            public const string Registration = "EmailConfirmation";
            public const string ResetPassword = "ResetPassword";
        }
    }
}
