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

namespace MasterTechDMO.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserManagement : ControllerBase
	{
		//private readonly SignInManager<DMOUsers> _signInManager;
		//private readonly UserManager<DMOUsers> _userManager;

        private UserManagementServices _userManagementServices;
        private IConfiguration _confifuraton;
        public UserManagement(UserManager<DMOUsers> userManager,
             IConfiguration confifuraton)
		{
            _userManagementServices = new UserManagementServices(userManager, confifuraton);

        }


        [HttpPost]
        [Route("registerUser")]
        public async Task<IActionResult> RegisterUserAsync(UserRegistration user)
        {
            try
            {
                string returnURL = string.Empty;
                if (Request.Headers.ContainsKey("returnUrl"))
                {
                    returnURL = Request.Headers.Single(x => x.Key == "returnUrl").Value;
                }
                return Ok(await _userManagementServices.RegisterUserAsync(user, returnURL));
            }
            catch (Exception Ex)
            {
                return StatusCode(500, Ex.Message);
            }
        }

        [HttpGet]
        [Route("verifyUser")]
        public async Task<IActionResult> VerifyUserAsync(string code)
        {
            try
            {
                string userId = string.Empty;
                if (Request.Headers.ContainsKey("userId"))
                {
                    userId = Request.Headers.Single(x => x.Key == "userId").Value;
                }
                return Ok(await _userManagementServices.VerifyUserAsync(userId, code));
            }
            catch (Exception Ex)
            {
                return StatusCode(500, Ex.Message);
            }
        }
    }
}
