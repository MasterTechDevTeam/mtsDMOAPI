using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
	public interface IUserManagementRepo
    {
        Task<APICallResponse<string>> RegisterUserAsync(DMOUsers user,string password);

        Task<APICallResponse<bool>> VerifyUserAsync(string userId, string code);
    }
}
