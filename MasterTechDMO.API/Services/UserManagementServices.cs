using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Helpers;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.UserManagement;
using MTSharedAccessToken.Model;
using MTSharedAccessToken.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MasterTechDMO.API.Services
{
    public class UserManagementServices
    {
        private IUserManagementRepo _userManagementRepo;
        public IConfiguration _configuration;
        private IIdentityRoleManagementRepo _identityRoleManagementRepo;
        public UserManagementServices(UserManager<DMOUsers> userManager,
            SignInManager<DMOUsers> signInManager,
            IConfiguration configuration,
             IServiceProvider serviceProvider,
              MTDMOContext context,
            ICipherService cipherService)
        {
            _userManagementRepo = new UserManagementRepo(userManager, signInManager, context, configuration, cipherService);
            _configuration = configuration;
            _identityRoleManagementRepo = new IdentityRoleManagementRepo(serviceProvider, context);

        }

        public async Task<APICallResponse<string>> RegisterUserAsync(UserRegistration user, string returnURL)
        {
            if (user != null)
            {
                if (user.UserType == Constants.BaseRole.OrgUser)
                {

                }

                var dmoUser = new DMOUsers
                {
                    Id = user.UserId.ToString(),
                    Email = user.EmailId,
                    UserName = user.EmailId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ContactNo = user.ContactNo,
                    DateofBirth = user.DateofBirth,
                    IsOrg = user.UserType == Constants.BaseRole.Org ? true : false,
                    OrgId = user.OrgId
                };
                var callResponse = await _userManagementRepo.RegisterUserAsync(dmoUser, user.Password);
                if (callResponse.IsSuccess)
                {
                    returnURL = HttpUtility.UrlDecode(returnURL);
                    returnURL = returnURL.Replace("<% CODE %>", callResponse.Respose);
                    if (SendUserVerificationMail(returnURL, string.Concat(string.Format($"{user.FirstName} {user.LastName}")), user.EmailId, user.UserType.ToString()))
                    {
                        callResponse.Message.Add("User registerd and mail send successfully.");
                    }
                    else
                    {
                        callResponse.Message.Add("User registerd but fail to send mail.");
                    }
                }

                return callResponse;
            }

            return new APICallResponse<string>()
            {
                IsSuccess = false,
                Message = new List<string>() { "User object is null." },
                Status = "Warning",
                Respose = string.Empty
            };

        }

        public async Task<APICallResponse<bool>> VerifyUserAsync(string emailId, string code)
        {
            if (code != string.Empty)
            {
                //code = HttpUtility.UrlDecode(code, Encoding.UTF8);
                //code = code.Replace(' ', '+');
                var callResponse = await _userManagementRepo.VerifyUserAsync(emailId, code);
                return callResponse;
            }

            return new APICallResponse<bool>()
            {
                IsSuccess = false,
                Message = new List<string>() { "Verification code is empty." },
                Status = "Warning",
                Respose = false
            };
        }

        public bool SendUserVerificationMail(string verificationLink, string username, string ToEmail, string UserType)
        {
            try
            {
                var smtpClient = GetSMTPSettings();

                string strMailBody = string.Empty;
                if (UserType == "OrganizationUser")
                {
                    //$"<a href='{verificationLink}'> Verify Account </a>" +
                    strMailBody = string.Format($"Hello {username}, " +
                            $"<br/> Thank you for registration. Your User Id will approve by your organizational admin." +
                            $"<br/>Thank You.<br/>");
                }
                else
                {
                    strMailBody = string.Format($"Hello {username}, " +
                            $"<br/> Thank you for registration. Please kindly click the button below to verify your account." +
                            $"<a href='{verificationLink}'> Verify Account </a>" +
                            $"<br/>Thank You.<br/>");
                }
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("sandboxmail@mastertechsolution.com"),
                    Subject = "Registration Confirmation",
                    Body = strMailBody,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(ToEmail);

                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception Ex)
            {
                string str = Ex.Message;
                return false;
            }
        }

        public async Task<APICallResponse<string>> LoginUserAsync(UserLogin user)
        {
            try
            {
                var loginResponse = new APICallResponse<string>();
                var response = await _userManagementRepo.LoginUserAsync(user);
                loginResponse.Message = response.Message;
                loginResponse.Status = response.Status;
                if (response.IsSuccess && response.Respose != null)
                {
                    var tokenSettings = new SharedAccessTokenSettings();
                    _configuration.Bind("JWTSettings", tokenSettings);
                    string token = MTSharedAccessTokenService.GenerateUserAccessToken(tokenSettings, response.Respose);
                    if (token == string.Empty)
                        loginResponse.Message.Add("Something went wrong while creating token.");
                    loginResponse.Respose = token;
                }
                loginResponse.IsSuccess = response.IsSuccess;
                return loginResponse;
            }
            catch (Exception Ex)
            {

                return new APICallResponse<string>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.InnerException.ToString() },
                    Status = "Error",
                    Respose = Ex.Message
                };
            }
        }

        public async Task<APICallResponse<UserProfile>> GetUserByEmailAsync(string EmailId)
        {
            try
            {
                APICallResponse<UserProfile> data = new APICallResponse<UserProfile>();
                var result = await _userManagementRepo.GetUserByEmailAsync(EmailId);

                data.IsSuccess = result.IsSuccess;
                data.Message = result.Message;
                data.Status = result.Status;
                if (result.Respose != null)
                {
                    string userType = string.Empty;
                    if (result.Respose.IsOrg)
                        userType = Constants.BaseRole.Org;
                    else if (result.Respose.OrgId != Guid.Empty)
                        userType = Constants.BaseRole.OrgUser;
                    else
                        userType = Constants.BaseRole.Indevidual;

                    string assignedRole = string.Empty;

                    var roleResult = await _identityRoleManagementRepo.GetAssignedRole(Guid.Parse(result.Respose.Id));
                    if (roleResult.IsSuccess && roleResult.Status == "Success")
                    {
                        assignedRole = roleResult.Respose;
                    }

                    UserProfile userDetails = new UserProfile
                    {
                        UserId = Guid.Parse(result.Respose.Id),
                        FirstName = result.Respose.FirstName,
                        LastName = result.Respose.LastName,
                        EmailId = result.Respose.Email,
                        Address = result.Respose.Address,
                        City = result.Respose.City,
                        UserType = userType,
                        ContactNo = result.Respose.ContactNo,
                        Country = result.Respose.Country,
                        DateofBirth = result.Respose.DateofBirth,
                        State = result.Respose.State,
                        Zipcode = result.Respose.Zipcode,
                        AssignedRole = assignedRole
                    };

                    data.Respose = userDetails;
                }

                return data;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<UserProfile>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.InnerException.ToString() },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<UserProfile>>> GetUsersAsync(Guid userId)
        {
            var callReponse = new APICallResponse<List<UserProfile>>();

            var userInRoleResponse = await _identityRoleManagementRepo.CheckUserInRole(userId);
            if (userInRoleResponse.IsSuccess && userInRoleResponse.Status == "Success")
            {
                var lstOrgUsers = await _userManagementRepo.GetUsersAsync(userId);
                if (lstOrgUsers != null)
                {
                    List<UserProfile> lstUsers = new List<UserProfile>();
                    foreach (var user in lstOrgUsers.Respose)
                    {
                        string assignedRole = string.Empty;
                        string userType = string.Empty;

                        var result = await _identityRoleManagementRepo.GetAssignedRole(Guid.Parse(user.Id));

                        if (result.IsSuccess && result.Status == "Success")
                        {
                            assignedRole = result.Respose;
                        }

                        if (user.IsOrg)
                            userType = Constants.BaseRole.Org;
                        else if (user.OrgId != null)
                            userType = Constants.BaseRole.OrgUser;
                        else
                            userType = Constants.BaseRole.Indevidual;

                        lstUsers.Add(
                            new UserProfile
                            {
                                Address = user.Address,
                                City = user.City,
                                ContactNo = user.ContactNo,
                                Country = user.Country,
                                DateofBirth = user.DateofBirth,
                                EmailId = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                State = user.State,
                                UserId = Guid.Parse(user.Id),
                                Zipcode = user.Zipcode,
                                AssignedRole = assignedRole,
                                UserType = userType
                            });
                    }
                    callReponse.Respose = lstUsers;
                    callReponse.IsSuccess = true;
                    callReponse.Message = lstOrgUsers.Message;
                    callReponse.Status = "Success";
                    return callReponse;
                }

                callReponse.Respose = null;
                callReponse.IsSuccess = lstOrgUsers.IsSuccess;
                callReponse.Message = lstOrgUsers.Message;
                callReponse.Status = "Warning";
                return callReponse;
            }
            else
            {
                return new APICallResponse<List<UserProfile>>
                {
                    IsSuccess = userInRoleResponse.IsSuccess,
                    Message = userInRoleResponse.Message,
                    Respose = null,
                    Status = userInRoleResponse.Status
                };
            }
        }

        public async Task<APICallResponse<bool>> UpdateUserDetailsAsync(UserProfile userDetails)
        {
            var callResponse = await _userManagementRepo.UpdateUserDetailsAsync(userDetails);

            if (!string.IsNullOrEmpty(userDetails.AssignedRole))
            {
                await _identityRoleManagementRepo.RemoveRoleFromUserAsync(userDetails.UserId);
                var roleCallResponse = await _identityRoleManagementRepo.AssignRoleToUserAsync(userDetails.UserId.ToString(), "", userDetails.AssignedRole);
                callResponse.IsSuccess = roleCallResponse.IsSuccess;
                callResponse.Status = roleCallResponse.Status;
                callResponse.Message.AddRange(roleCallResponse.Message);
            }

            return callResponse;

        }

        public async Task<APICallResponse<string>> GetResetPasswordTokenAsync(string returnUrl, string emailId)
        {
            try
            {
                var tokenCallResponse = await _userManagementRepo.GenerateForgetPasswordTokenAsync(emailId);
                if (!string.IsNullOrEmpty(tokenCallResponse.Respose))
                {
                    returnUrl = HttpUtility.UrlDecode(returnUrl, Encoding.UTF8);
                    returnUrl = returnUrl.Replace("<% CODE %>", tokenCallResponse.Respose);
                    SendResetPasswordMail(returnUrl, emailId);
                }
                return tokenCallResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<string>()
                {
                    IsSuccess = false,
                    Message = new List<string> { Ex.Message },
                    Status = "Error",
                    Respose = "Oops! something went wrong."
                };
            }
        }

        public bool SendResetPasswordMail(string verificationLink, string ToEmail)
        {
            try
            {
                var smtpClient = GetSMTPSettings();

                string strMailBody = string.Empty;
                strMailBody = string.Format($"Hello {ToEmail}, " +
                        $"<br/> Please click the link below to reset your password." +
                        $"<a href='{verificationLink}'> Reset Password </a>" +
                        $"<br/>Thank You.<br/>");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("sandboxmail@mastertechsolution.com"),
                    Subject = "Reset Password",
                    Body = strMailBody,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(ToEmail);

                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception Ex)
            {
                string str = Ex.Message;
                return false;
            }
        }

        public async Task<APICallResponse<bool>> ResetPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            return await _userManagementRepo.ResetPasswordAsync(forgotPasswordModel);
        }

        public async Task<APICallResponse<bool>> RemoveUserAsync(string username)
        {
            return await _userManagementRepo.RemoveUserAsync(username);

        }

        private SmtpClient GetSMTPSettings()
        {
            SMTPSettings smtpSettings = new SMTPSettings();

            _configuration.GetSection("SMTPSettings").Bind(smtpSettings);

            return new SmtpClient(smtpSettings.SMTP)
            {
                Port = smtpSettings.Port,
                Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
                EnableSsl = smtpSettings.EnableSsl,
            };
        }

        public async Task<APICallResponse<List<OrganizationsData>>> GetOrganizationAsync()
        {
            return await _userManagementRepo.GetOrganizationAsync();
        }

    }
}
