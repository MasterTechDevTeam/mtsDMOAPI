using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using MasterTechDMO.API.Services;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.UserManagement;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MasterTechDMO.API.Helpers;

namespace MasterTechDMO.API.Controllers
{
    /// <summary>
    /// API for User Management like Login, Registration
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagement : ControllerBase
    {

        private UserManagementServices _userManagementServices;
        private IdentityRoleService _identityRoleService;
        public UserManagement(UserManager<DMOUsers> userManager,
            SignInManager<DMOUsers> signInManager,
            IServiceProvider serviceProvider,
             IConfiguration confifuraton,
             MTDMOContext context,
             ICipherService cipherService)
        {
            _identityRoleService = new IdentityRoleService(serviceProvider, context);
            _userManagementServices = new UserManagementServices(userManager, signInManager, confifuraton, serviceProvider, context, cipherService);
        }

        /// <summary>
        /// Register new user 
        /// </summary>
        /// <param name="user">Object of UserRegistration type</param>
        /// <param name="returnUrl">returnUrl of dmo application</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("registerUser")]
        public async Task<IActionResult> RegisterUserAsync(UserRegistration user,[FromHeader] string returnUrl)
        {
            try
            {
                //string returnURL = string.Empty;
                //if (Request.Headers.ContainsKey("returnUrl"))
                //{
                //    returnURL = Request.Headers.Single(x => x.Key == "returnUrl").Value;
                //}

                var userRegisterReponse = await _userManagementServices.RegisterUserAsync(user, returnUrl);
                if (userRegisterReponse.IsSuccess && userRegisterReponse.Status == "Success")
                {
                    if (user.UserType == Constants.BaseRole.Org)
                        await _identityRoleService.AssignRoleToUserAsync(user.UserId.ToString(), Constants.BaseRole.Org);
                }
                return Ok(userRegisterReponse);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, Ex.Message);
            }
        }

        /// <summary>
        /// Register user as admin or as an organization after login.
        /// </summary>
        /// <param name="user">Object of UserRegistration type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("registerUserAsAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RegisterUserAsAdminAsync(UserRegistration user)
        {
            try
            {
                string returnURL = string.Empty;

                var userRegisterReponse = await _userManagementServices.RegisterUserAsAdminAsync(user);
                return Ok(userRegisterReponse);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, Ex.Message);
            }
        }

        /// <summary>
        /// Will verify the verification send to the user in email
        /// </summary>
        /// <param name="verificationCode">Verificaion Code from email</param>
        /// <param name="emailId">emailId to verify user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("verifyUser")]
        public async Task<IActionResult> VerifyUserAsync(string verificationCode,[FromHeader] string emailId)
        {
            try
            {
                //string emailId = string.Empty;
                //if (Request.Headers.ContainsKey("emailId"))
                //{
                //    emailId = Request.Headers.Single(x => x.Key == "emailId").Value;
                //}
                return Ok(await _userManagementServices.VerifyUserAsync(emailId, verificationCode));
            }
            catch (Exception Ex)
            {
                return StatusCode(500, Ex.Message);
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="user">Object of UserLogin type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("loginUser")]
        public async Task<IActionResult> LoginUserAsync(UserLogin user)
        {
            return Ok(await _userManagementServices.LoginUserAsync(user));
        }

        /// <summary>
        /// Return the details of requested user 
        /// </summary>
        /// <param name="EmailId">EmailId of User</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getUserByEmail/{EmailId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserByEmailAsync(string EmailId)
        {
            return Ok(await _userManagementServices.GetUserByEmailAsync(EmailId));
        }

        /// <summary>
        /// Return the user details of requested user
        /// </summary>
        /// <param name="userId">userId of User</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getUsers/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersAsync(Guid userId)
        {
            return Ok(await _userManagementServices.GetUsersAsync(userId));
        }

        /// <summary>
        /// Return the fellow employee list if user is an organization user
        /// </summary>
        /// <param name="userId">userId of user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getFellowEmployees/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> getFellowEmployeesAsync(Guid userId)
        {
            return Ok(await _userManagementServices.GetFellowEmployeesAsync(userId));
        }

        /// <summary>
        /// Verify the user when user is created either by admin  or by organization.
        /// </summary>
        /// <param name="emailId">emailId of user.</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("verifyUserAsAdmin/{emailId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyUserAsAdminAsync(string emailId)
        {
            return Ok(await _userManagementServices.VerifyUserAsAdminAsync(emailId));
        }

        /// <summary>
        /// Will update the user profile of user
        /// </summary>
        /// <param name="userDetails">Object of UserProfile type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("updateUserDetails")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserDetailsAsync(UserProfile userDetails)
        {
            return Ok(await _userManagementServices.UpdateUserDetailsAsync(userDetails));
        }

        /// <summary>
        /// Will send the reset password mail to the requested user
        /// </summary>
        /// <param name="EmailId">EmailId of user</param>
        /// <param name="returnUrl">returnUrl of application</param>
        /// <returns></returns>
        [HttpGet]
        [Route("sendResetPasswordMail/{EmailId}")]
        public async Task<IActionResult> SendResetPasswordMailAsync(string EmailId,[FromHeader] string returnUrl)
        {
            //string returnURL = string.Empty;
            //if (Request.Headers.ContainsKey("returnUrl"))
            //{
            //    returnURL = Request.Headers.Single(x => x.Key == "returnUrl").Value;
            //}
            return Ok(await _userManagementServices.GetResetPasswordTokenAsync(returnUrl, EmailId));
        }

        /// <summary>
        /// Will reset the password for the user
        /// </summary>
        /// <param name="forgotPasswordModel">Object of forgotPasswordModel type</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpPost]
        [Route("resetPasword")]
        public async Task<IActionResult> ResetPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            //string returnURL = string.Empty;
            //if (Request.Headers.ContainsKey("returnUrl"))
            //{
            //    returnURL = Request.Headers.Single(x => x.Key == "returnUrl").Value;
            //}
            return Ok(await _userManagementServices.ResetPasswordAsync(forgotPasswordModel));
        }


        /// <summary>
        /// Will Remove user if requestd by either admin or organization
        /// </summary>
        /// <param name="username">emailId of the user</param>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("removeUser/{username}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RemoveUserAsync(string username)
        {
            return Ok(await _userManagementServices.RemoveUserAsync(username));
        }

        /// <summary>
        /// Return the list of  Organization
        /// </summary>
        /// <returns>Return object of Ok(200)</returns>
        [HttpGet]
        [Route("getOrganization")]
        public async Task<IActionResult> GetOrganizationAsync()
        {
            return Ok(await _userManagementServices.GetOrganizationAsync());
        }



    }
}
