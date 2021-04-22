using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Helpers
{
    public class RepoHelpers
    {

        public static bool IsOrgUser(Guid orgId, MTDMOContext context)
        {
            try
            {
                var orgUserData = context.Users.Where(x => x.Id == orgId.ToString() && x.IsOrg == true).FirstOrDefault();
                if (orgUserData != null)
                {
                    var userInRole = context.UserRoles.Where(x => x.UserId == orgId.ToString()).FirstOrDefault();
                    if (userInRole != null)
                    {
                        var roleDetails = context.Roles.Where(x => x.Id == userInRole.RoleId).FirstOrDefault();
                        if (roleDetails.Name == Constants.BaseRole.Org)
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        public static bool IsMTAdmin(Guid userId, MTDMOContext context)
        {
            try
            {
                var userData = context.Users.Where(x => x.Id == userId.ToString()).FirstOrDefault();
                if (userData != null)
                {
                    var userInRole = context.UserRoles.Where(x => x.UserId == userId.ToString()).FirstOrDefault();
                    if (userInRole != null)
                    {
                        var roleDetails = context.Roles.Where(x => x.Id == userInRole.RoleId).FirstOrDefault();
                        if (roleDetails.Name == Constants.BaseRole.MTAdmin)
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        public static bool IsOrgEmploee(Guid userId, MTDMOContext context)
        {
            try
            {
                var userData = context.Users.Where(x => x.Id == userId.ToString() && x.OrgId != Guid.Empty).FirstOrDefault();
                if (userData != null)
                {
                    return  true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        public static DateTime ConvertDateTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}
