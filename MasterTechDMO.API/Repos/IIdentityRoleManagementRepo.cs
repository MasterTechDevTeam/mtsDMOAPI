using MasterTechDMO.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface IIdentityRoleManagementRepo
    {
        Task<KeyValuePair<bool, string>> CreateRolesAsync(string role);
        Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleName);
    }
}