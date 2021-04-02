using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Helpers;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.UserManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MasterTechDMO.API.Repos
{
    public class UserManagementRepo : IUserManagementRepo
    {
        private UserManager<DMOUsers> _userManager;
        private SignInManager<DMOUsers> _signInManager;
        private MTDMOContext _context;
        private IConfiguration _configuration;
        private ICipherService _cipherService;
        public UserManagementRepo(UserManager<DMOUsers> userManager,
            SignInManager<DMOUsers> signInManager,
            MTDMOContext context,
            IConfiguration configuration,
            ICipherService cipherService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _cipherService = cipherService;
        }

        public async Task<APICallResponse<string>> RegisterUserAsync(DMOUsers user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (user.EmailConfirmed)
                {
                    return new APICallResponse<string>
                    {
                        IsSuccess = true,
                        Status = "Success",
                        Message = new List<string>() { "User Registered." },
                        Respose = ""
                    };
                }

                //var tokenCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var tokenCode = GenerateToken(user.Email, Constants.TokenType.Registration);
                tokenCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenCode));
                //tokenCode = HttpUtility.UrlEncode(tokenCode, Encoding.UTF8);

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
                var result = VerifiyToken(code);
                //var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.ToLower() == "success")
                {

                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Status = "Success",
                        Message = new List<string>() { "User Verified Successfully" },
                        Respose = true
                    };

                }
                else if (result.ToLower() == "verified")
                {
                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Status = "Success",
                        Message = new List<string>() { "User Already Verified." },
                        Respose = true
                    };
                }
                else
                {
                    //List<string> iError = result.Errors.Select(x => x.Description).ToList();

                    return new APICallResponse<bool>
                    {
                        IsSuccess = false,
                        Message = new List<string> { result },
                        Status = "Error",
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

        public async Task<APICallResponse<List<DMOUsers>>> GetUsersAsync(Guid orgId)
        {
            try
            {
                var lstOrgUsers = new APICallResponse<List<DMOUsers>>();
                if (orgId != null && RepoHelpers.IsOrgUser(orgId, _context))
                {
                    lstOrgUsers.Respose = _context.Users.Where(x => x.OrgId == orgId && x.IsDeactive == false).ToList();
                    lstOrgUsers.Message = new List<string> { $"{lstOrgUsers.Respose.Count} users founds." };
                    lstOrgUsers.Status = "Success";
                }
                else if (orgId != null && RepoHelpers.IsMTAdmin(orgId, _context))
                {
                    lstOrgUsers.Respose = _context.Users.ToList();
                    lstOrgUsers.Message = new List<string> { $"{lstOrgUsers.Respose.Count} users founds." };
                    lstOrgUsers.Status = "Success";
                }
                else
                {
                    lstOrgUsers.Respose = null;
                    lstOrgUsers.Message = new List<string> { "Either user not found or user is missing permission." };
                    lstOrgUsers.Status = "Warning";
                }

                lstOrgUsers.IsSuccess = true;
                return lstOrgUsers;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<DMOUsers>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<bool>> UpdateUserDetailsAsync(UserProfile userDetails)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbUserDetails = _context.Users.Where(x => x.Id == userDetails.UserId.ToString()).FirstOrDefault();
                if (dbUserDetails != null)
                {
                    dbUserDetails.FirstName = userDetails.FirstName;
                    dbUserDetails.LastName = userDetails.LastName;
                    dbUserDetails.ContactNo = userDetails.ContactNo;
                    dbUserDetails.Address = userDetails.Address;
                    dbUserDetails.Zipcode = userDetails.Zipcode;
                    dbUserDetails.State = userDetails.State;
                    dbUserDetails.Country = userDetails.Country;
                    dbUserDetails.City = userDetails.City;
                }
                _context.SaveChanges();
                callResponse.IsSuccess = true;
                callResponse.Respose = true;
                callResponse.Status = "Success";
                callResponse.Message = new List<string> { $"User {dbUserDetails.UserName} is update successfully" };
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.InnerException.ToString() },
                    Status = "Error",
                    Respose = false
                };
            }

        }

        public async Task<APICallResponse<string>> GenerateForgetPasswordTokenAsync(string EmailId)
        {
            try
            {
                var callResponse = new APICallResponse<string>();
                var dbUser = await _userManager.FindByNameAsync(EmailId);
                if (dbUser != null)
                {
                    //var token = await _userManager.GeneratePasswordResetTokenAsync(dbUser);
                    var token = GenerateToken(EmailId, Constants.TokenType.ResetPassword);
                    if (!string.IsNullOrEmpty(token))
                    {
                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                        //token = HttpUtility.UrlEncode(token, Encoding.UTF8);
                        callResponse.Respose = token;
                        callResponse.Status = "Success";
                        callResponse.Message = new List<string> { "Token generated successfully." };
                    }
                    else
                    {
                        callResponse.Respose = string.Empty;
                        callResponse.Status = "Warning";
                        callResponse.Message = new List<string> { "Failed to generate token." };
                    }
                }
                else
                {
                    callResponse.Respose = string.Empty;
                    callResponse.Status = "Warning";
                    callResponse.Message = new List<string> { "User not found." };
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<string>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.InnerException.ToString() },
                    Status = "Error",
                    Respose = "Oops! Something went wrong"
                };
            }
        }

        public async Task<APICallResponse<bool>> ResetPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();

                var dbUser = _signInManager.UserManager.FindByEmailAsync(forgotPasswordModel.EmailId).Result;

                if (dbUser != null)
                {
                    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(forgotPasswordModel.Code));
                    var result = VerifiyToken(code);

                    //var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(forgotPasswordModel.Code));
                    if (result.ToLower() == "success")
                    {
                        var resetHashedPassword = _userManager.PasswordHasher.HashPassword(dbUser, forgotPasswordModel.ConfirmPassword);
                        if (resetHashedPassword != null)
                        {
                            dbUser.PasswordHash = resetHashedPassword;
                            _context.Entry(dbUser).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            callResponse.IsSuccess = true;
                            callResponse.Respose = true;
                            callResponse.Message = new List<string> { "Password Chaanged successfully." };
                            callResponse.Status = "Success";
                        }
                        else
                        {
                            callResponse.IsSuccess = true;
                            callResponse.Status = "Warning";
                            callResponse.Respose = false;
                            callResponse.Message = new List<string> { "Oops, Something went wrong. Try again." };
                        }

                    }
                    else if (result.ToLower() == "verified")
                    {
                        return new APICallResponse<bool>
                        {
                            IsSuccess = true,
                            Status = "Success",
                            Message = new List<string>() { "User Already Verified." },
                            Respose = true
                        };
                    }
                    else
                    {
                        //List<string> iError = result.Errors.Select(x => x.Description).ToList();

                        return new APICallResponse<bool>
                        {
                            IsSuccess = false,
                            Message = new List<string> { result },
                            Status = "Error",
                            Respose = false
                        };
                    }
                    //var changePasswordResult = await _userManager.ResetPasswordAsync(dbUser, code, forgotPasswordModel.Password);
                    //if (changePasswordResult.Succeeded)
                    //{
                    //    callResponse.IsSuccess = true;
                    //    callResponse.Respose = true;
                    //    callResponse.Message = new List<string> { "Password Chaanged successfully." };
                    //    callResponse.Status = "Success";
                    //}
                    //else
                    //{
                    //    callResponse.IsSuccess = true;
                    //    callResponse.Status = "Warning";
                    //    callResponse.Respose = false;
                    //    callResponse.Message = new List<string> { "Oops, Something went wrong. Try again." };
                    //}
                }
                else
                {
                    callResponse.IsSuccess = true;
                    callResponse.Respose = false;
                    callResponse.Message = new List<string> { "User not found." };
                    callResponse.Status = "Warning";
                }

                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.InnerException.ToString() },
                    Status = "Error",
                    Respose = false
                };
            }
        }

        public async Task<APICallResponse<bool>> RemoveUserAsync(string username)
        {
            try
            {
                var dbUser = _context.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (dbUser != null)
                {
                    dbUser.IsDeactive = true;
                    _context.SaveChanges();
                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Message = new List<string> { "User removed." },
                        Respose = true,
                        Status = "Success"
                    };
                }
                return new APICallResponse<bool>
                {
                    IsSuccess = true,
                    Message = new List<string> { "User not found." },
                    Respose = false,
                    Status = "Warning"
                };
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string> { Ex.Message },
                    Respose = false,
                    Status = "Error"
                };
            }
        }

        private string GenerateToken(string emailId, string TokenType)
        {
            try
            {
                string textToEnc = JsonConvert.SerializeObject(new TokenModel { EmailId = emailId, Type = TokenType, Time = DateTime.Now });
                return _cipherService.Encrypt(textToEnc, _configuration["AppSettings:EncDecKey"]);
            }
            catch (Exception Ex)
            {
                return string.Empty;
            }
        }

        private string VerifiyToken(string encText)
        {
            try
            {
                var decText = _cipherService.Decrypt(encText, _configuration["AppSettings:EncDecKey"]);
                var tokenData = JsonConvert.DeserializeObject<TokenModel>(decText);
                if (tokenData != null)
                {
                    if (tokenData.Time <= tokenData.Time.AddHours(Convert.ToInt32(_configuration["AppSettings:TokenValidateLife"])))
                    {
                        var user = _userManager.FindByEmailAsync(tokenData.EmailId).Result;
                        if (user?.EmailConfirmed == false && tokenData.Type == Constants.TokenType.Registration)
                        {
                            user.EmailConfirmed = true;
                            _context.SaveChanges();
                            return "Success";
                        }
                        else if (tokenData.Type == Constants.TokenType.ResetPassword) return "Success";
                        else
                            return "Verified";
                    }
                    else
                        return "Token expired.";
                }
                return string.Empty;
            }
            catch (Exception Ex)
            {
                return string.Empty;
            }
        }

        public async Task<APICallResponse<List<OrganizationsData>>> GetOrganizationAsync()
        {
            try
            {
                var orgList = _context.Users.Where(x => x.IsOrg == true).Select(x => new OrganizationsData(Guid.Parse(x.Id), x.FirstName, x.LastName)).ToList();
                if (orgList != null)
                {
                    return new APICallResponse<List<OrganizationsData>>
                    {
                        IsSuccess = true,
                        Message = new List<string> { $"{orgList.Count} organization found." },
                        Respose = orgList,
                        Status = "Success"
                    };
                }
                else
                    return new APICallResponse<List<OrganizationsData>>
                    {
                        IsSuccess = true,
                        Message = new List<string> { $"No organization found." },
                        Respose = orgList,
                        Status = "Warning"
                    };

            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<OrganizationsData>>
                {
                    IsSuccess = false,
                    Message = new List<string> { Ex.Message },
                    Respose = null,
                    Status = "Error"
                };
            }
        }

        public async Task<APICallResponse<bool>> VerifyUserAsAdminAsync(string emailId)
        {
            try
            {
                var dbUser = _context.Users.Where(x => x.Email == emailId).FirstOrDefault();
                if (dbUser?.EmailConfirmed == false)
                {
                    dbUser.EmailConfirmed = true;
                    _context.Entry(dbUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return new APICallResponse<bool>
                    { 
                        IsSuccess = true,
                        Message = new List<string> { $"{emailId} verified" },
                        Respose = true,
                        Status = "Success"
                    };
                }
                return new APICallResponse<bool>
                {
                    IsSuccess = true,
                    Message = new List<string> { $"{emailId} not found" },
                    Respose = true,
                    Status = "Success"
                };
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string> { Ex.Message },
                    Respose = false,
                    Status = "Error"
                };
            }
        }
    }
}
