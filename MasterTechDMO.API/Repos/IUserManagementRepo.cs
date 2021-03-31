using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using mtsDMO.Context.UserManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
	public interface IUserManagementRepo
    {
        Task<APICallResponse<string>> RegisterUserAsync(DMOUsers user,string password);

        Task<APICallResponse<bool>> VerifyUserAsync(string emailId, string code);

        Task<APICallResponse<IList<string>>> LoginUserAsync(UserLogin user);

        Task<APICallResponse<DMOUsers>> GetUserByEmailAsync(string EmailId);

        Task<APICallResponse<List<DMOUsers>>> GetUsersAsync(Guid orgId);

        Task<APICallResponse<bool>> UpdateUserDetailsAsync(UserProfile userDetails);

        Task<APICallResponse<string>> GenerateForgetPasswordTokenAsync(string EmailId);

        Task<APICallResponse<bool>> ResetPasswordAsync(ForgotPasswordModel forgotPasswordModel);

        Task<APICallResponse<bool>> RemoveUserAsync(string username);

        Task<APICallResponse<List<OrganizationsData>>> GetOrganizationAsync();
    }
}
