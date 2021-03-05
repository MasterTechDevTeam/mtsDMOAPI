﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mtsDMO.Context.RoleManagement;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class RoleManagementController : ControllerBase
    {

		private IdentityRoleService _identityRoleService;
		private IConfiguration _confifuraton;
		public RoleManagementController(
			IServiceProvider serviceProvider,
			 IConfiguration confifuraton,
			 MTDMOContext context)
		{
			_identityRoleService = new IdentityRoleService(serviceProvider,context);
		}

		[HttpPost]
		[Route("createRole")]
		public async Task<IActionResult> CreateRoleAsync(CreateRoleDetails roleDetails)
		{
			return Ok(await _identityRoleService.CreateRolesAsync(roleDetails));
		}

		[HttpPost]
		[Route("getRole")]
		public async Task<IActionResult> GetRoles()
		{
			string userId = string.Empty;
			if (Request.Headers.ContainsKey("userId"))
			{
				userId = Request.Headers.Single(x => x.Key == "userId").Value;
			}
			return Ok(await _identityRoleService.GetRoleAsync(userId));

		}
	}
}
