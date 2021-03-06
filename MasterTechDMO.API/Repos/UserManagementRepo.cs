using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using mtsDMO.Context.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Repos
{
    public class UserManagementRepo : IUserManagementRepo
    {
        private UserManager<DMOUsers> _userManager;
        private SignInManager<DMOUsers> _signInManager;

        public UserManagementRepo(UserManager<DMOUsers> userManager, SignInManager<DMOUsers> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<APICallResponse<string>> RegisterUserAsync(DMOUsers user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var tokenCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                tokenCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenCode));

                return new APICallResponse<string>
                {
                    IsSuccess = true,
                    Status = "Success",
                    Message = new List<string>() { "User Registered and Verification Token Generated" },
                    Respose = tokenCode
                };
            }
            else
            {
                List<string> iError = result.Errors.Select(x => x.Description).ToList();

                return new APICallResponse<string>
                {
                    IsSuccess = false,
                    Message = iError,
                    Status = "IdentityError",
                    Respose = string.Empty
                };
            }
        }

        public async Task<APICallResponse<bool>> VerifyUserAsync(string emailId, string code)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailId);
                if (user == null)
                {
                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Status = "Warning",
                        Message = new List<string>() { "No user found." },
                        Respose = false
                    };
                }

                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {

                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Status = "Success",
                        Message = new List<string>() { "User Verified Successfully" },
                        Respose = true
                    };

                }
                else
                {
                    List<string> iError = result.Errors.Select(x => x.Description).ToList();

                    return new APICallResponse<bool>
                    {
                        IsSuccess = false,
                        Message = iError,
                        Status = "IdentityError",
                        Respose = false
                    };
                }
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }

        public async Task<APICallResponse<IList<string>>> LoginUserAsync(UserLogin user)
        {
            var response = new APICallResponse<IList<string>>();
            try
            {
                var result = await _signInManager.PasswordSignInAsync(user.EmailId, user.Password, user.IsRemberMe, true);
                if (result.Succeeded)
                {
                    var claims = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(user.EmailId));
                    response.Message = new List<string>() { "User Logged In." };
                    response.Respose = claims;
                    response.Status = "Success";
                }
                else if (result.RequiresTwoFactor)
                {
                    response.Message = new List<string>() { "Required Two Factor Authentication." };
                    response.Respose = null;
                    response.Status = "Warning";
                }
                else if (result.IsLockedOut)
                {
                    response.Message = new List<string>() { "User account is locked out." };
                    response.Respose = null;
                    response.Status = "Warning";
                }
                else
                {
                    response.Message = new List<string>() { "EmailId or password is invalid." };
                    response.Respose = null;
                    response.Status = "Warning";
                }
                response.IsSuccess = true;
                return response;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<IList<string>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message, Ex.InnerException.ToString() },
                    Respose = null,
                    Status = "Error"
                };
            }

        }

        public async Task<APICallResponse<DMOUsers>> GetUserByEmailAsync(string EmailId)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(EmailId);
                if (user == null)
                {
                    return new APICallResponse<DMOUsers>
                    {
                        IsSuccess = true,
                        Status = "Warning",
                        Message = new List<string>() { "No user found." },
                        Respose = null
                    };
                }

                return new APICallResponse<DMOUsers>
                {
                    IsSuccess = true,
                    Status = "Success",
                    Message = new List<string>() { "User Found" },
                    Respose = user
                };

            }
            catch (Exception Ex)
            {

                return new APICallResponse<DMOUsers>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }
    }
}
