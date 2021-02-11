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
        public UserManagementServices(UserManager<DMOUsers> userManager, SignInManager<DMOUsers> signInManager, IConfiguration configuration)
        {
            _userManagementRepo = new UserManagementRepo(userManager, signInManager);
            _configuration = configuration;
        }

        public async Task<APICallResponse<string>> RegisterUserAsync(UserRegistration user, string returnURL)
        {
            if (user != null)
            {
                var dmoUser = new DMOUsers
                {
                    Id = user.UserId.ToString(),

                    Email = user.EmailId,
                    UserName = user.EmailId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ContactNo = user.ContactNo,
                    DateofBirth = user.DateofBirth,
                    UserType = user.UserType,
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

        public async Task<APICallResponse<bool>> VerifyUserAsync(string userId, string code)
        {
            if (code != string.Empty)
            {
                code = HttpUtility.UrlDecode(code);
                var callResponse = await _userManagementRepo.VerifyUserAsync(userId, code);
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
                    strMailBody = string.Format($"Hello {username}, " +
                            $"<br/> Thank you for registration. Your User Id will approve by your organizational admin." +
                            //$"<a href='{verificationLink}'> Verify Account </a>" +
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
                if (response.IsSuccess && response.Respose != null)
                {
                    var tokenSettings = new SharedAccessTokenSettings();
                    _configuration.Bind("JWTSettings", tokenSettings);
                    string token = MTSharedAccessTokenService.GenerateUserAccessToken(tokenSettings,response.Respose);
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
    }
}
