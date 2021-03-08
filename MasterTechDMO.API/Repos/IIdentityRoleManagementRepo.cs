using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface IIdentityRoleManagementRepo
    {
        Task<KeyValuePair<bool, string>> CreateRolesAsync(Guid orgId,string roleName);
        Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleName);
        Task<APICallResponse<List<DMOOrgRoles>>> GetRolesAsync(Guid orgId);

        Task<APICallResponse<bool>> CheckUserInRole(Guid id);

        Task<KeyValuePair<bool, string>> CreateBaseRoleAsync(string roleName);
    }
}