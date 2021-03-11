using MasterTechDMO.API.Areas.Identity.Data;
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
              MTDMOContext context)
        {
            _userManagementRepo = new UserManagementRepo(userManager, signInManager, context);
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
                    OrgId = Guid.Empty,
                    Address = user.Address,
                    City = user.City,
                    Zipcode = user.Zipcode,
                    State = user.State,
                    Country = user.Country
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
                code = HttpUtility.UrlDecode(code);
                code = code.Replace(' ', '+');
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
                SMTPSettings smtpSettings = new SMTPSettings();

                _configuration.GetSection("SMTPSettings").Bind(smtpSettings);

                var smtpClient = new SmtpClient(smtpSettings.SMTP)
                {
                    Port = smtpSettings.Port,
                    Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl,
                };
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
                    From = new MailAddress(smtpSettings.Username),
                    Subject = "Registration Confirmation",
                    Body = strMailBody,
                    IsBodyHtml = true,
                };

                //var mailMessage = new MailMessage
                //{
                //	From = new MailAddress(smtpSettings.Username),
                //	Subject = "Registration Confirmation",
                //	Body = string.Format($"Hello {username}, " +
                //			$"<br/> Thank you for registration. Please kindly click the button below to verify your account." +
                //			$"<a href='{verificationLink}'> Verify Account </a>" +
                //			$"<br/>Thank You.<br/>"),
                //	IsBodyHtml = true,
                //};

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

        public async Task<APICallResponse<UserDetails>> GetUserByEmailAsync(string EmailId)
        {
            try
            {
                APICallResponse<UserDetails> data = new APICallResponse<UserDetails>();
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

                    UserDetails userDetails = new UserDetails
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
                        Zipcode = result.Respose.Zipcode
                    };

                    data.Respose = userDetails;
                }

                return data;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<UserDetails>()
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.InnerException.ToString() },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<UserDetails>>> GetUsersAsync(Guid userId)
        {
            var callReponse = new APICallResponse<List<UserDetails>>();

            var userInRoleResponse = await _identityRoleManagementRepo.CheckUserInRole(userId);
            if (userInRoleResponse.IsSuccess && userInRoleResponse.Status == "Success")
            {
                var lstOrgUsers = await _userManagementRepo.GetUsersAsync(userId);
                if (lstOrgUsers != null)
                {
                    List<UserDetails> lstUsers = new List<UserDetails>();
                    foreach (var user in lstOrgUsers.Respose)
                    {
                        lstUsers.Add(
                            new UserDetails {
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
                return new APICallResponse<List<UserDetails>>
                {
                    IsSuccess = userInRoleResponse.IsSuccess,
                    Message = userInRoleResponse.Message,
                    Respose = null,
                    Status = userInRoleResponse.Status
                };
            }
        }
    }
}
