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
        Task<APICallResponse<bool>> GetRoleAsync(string userId);

        Task<KeyValuePair<bool, string>> CreateBaseRoleAsync(string roleName);
    }
}