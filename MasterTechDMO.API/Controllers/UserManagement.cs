﻿using System;
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

namespace MasterTechDMO.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class UserManagement : ControllerBase
	{
		//private readonly SignInManager<DMOUsers> _signInManager;
		//private readonly UserManager<DMOUsers> _userManager;

		private UserManagementServices _userManagementServices;
		private IdentityRoleService _identityRoleService;
		private IConfiguration _confifuraton;
		public UserManagement(UserManager<DMOUsers> userManager,
			SignInManager<DMOUsers> signInManager,
			IServiceProvider serviceProvider,
			 IConfiguration confifuraton,
			 MTDMOContext context)
		{
			_identityRoleService = new IdentityRoleService(serviceProvider, context); 
			_userManagementServices = new UserManagementServices(userManager, signInManager, confifuraton);
		}


		[HttpPost]
		[Route("registerUser")]
		[AllowAnonymous]
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
		[AllowAnonymous]
		public async Task<IActionResult> VerifyUserAsync(string verificationCode)
		{
			try
			{
				string emailId = string.Empty;
				if (Request.Headers.ContainsKey("emailId"))
				{
					emailId = Request.Headers.Single(x => x.Key == "emailId").Value;
				}
				return Ok(await _userManagementServices.VerifyUserAsync(emailId, verificationCode));
			}
			catch (Exception Ex)
			{
				return StatusCode(500, Ex.Message);
			}
		}

		[HttpPost]
		[Route("loginUser")]
		[AllowAnonymous]
		public async Task<IActionResult> LoginUserAsync(UserLogin user)
		{
			return Ok(await _userManagementServices.LoginUserAsync(user));
		}

		[HttpGet]
		[Route("getUserByEmail/{EmailId}")]
		public async Task<IActionResult> GetUserByEmailAsync(string EmailId)
		{
			return Ok(await _userManagementServices.GetUserByEmailAsync(EmailId));
		}


	}
}
