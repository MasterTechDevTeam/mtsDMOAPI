using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface IIdentityRoleManagementRepo
    {
        Task<KeyValuePair<bool, string>> CreateRolesAsync(Guid orgId, string roleName);
        Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleName, string roleId = "");
        Task<APICallResponse<List<DMOOrgRoles>>> GetRolesAsync(Guid orgId);

        Task<APICallResponse<bool>> CheckUserInRole(Guid id);

        Task<KeyValuePair<bool, string>> CreateBaseRoleAsync(string roleName);

        Task<APICallResponse<string>> GetAssignedRole(Guid userId);

        Task<APICallResponse<bool>> RemoveRoleFromUserAsync(Guid userId);

        Task<APICallResponse<DMOOrgRoles>> FindRoleByIdAsync(Guid roleId);

        Task<APICallResponse<bool>> UpdateRoleByIdAsync(DMOOrgRoles orgRole);

        Task<APICallResponse<bool>> RemoveRoleAsync(string roleName, Guid orgId);
    }
}