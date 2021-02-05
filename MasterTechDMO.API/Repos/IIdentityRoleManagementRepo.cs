using MasterTechDMO.API.Models;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public interface IIdentityRoleManagementRepo
    {
        Task CreateRolesAsync(string role);
        Task<APICallResponse<bool>> AssignRoleToUserAsync(string userId, string roleName);
    }
}