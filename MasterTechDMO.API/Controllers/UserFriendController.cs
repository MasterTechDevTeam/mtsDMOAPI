using System;
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
using mtsDMO.Context.Utility;

namespace MasterTechDMO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class UserFriendController : ControllerBase
    {
        private IConfiguration _confifuraton;
        private FriendListServices _friendSerivce;
        public UserFriendController(
             MTDMOContext context)
        {
            _friendSerivce = new FriendListServices(context);
        }


        [HttpGet]
        [Route("getFriendList/{userId}")]
        public async Task<IActionResult> GetFriendListAsync(Guid userId)
        {
            return Ok(await _friendSerivce.GetFriendListAsync(userId));
        }

        [HttpGet]
        [Route("getFriendData/{userId}/{friendEmailId}")]
        public async Task<IActionResult> GetFriendDataByEmailAsync(Guid userId,string friendEmailId)
        {
            return Ok(await _friendSerivce.GetFriendDataByEmailAsync(userId,friendEmailId));
        }

        [HttpPost]
        [Route("updateFriendData")]
        public async Task<IActionResult> UpdateFriendDataAsync(UserFriendData friendData)
        {
            return Ok(await _friendSerivce.AddOrUpdateFriendDataAsync(friendData));
        }

        [HttpPost]
        [Route("addFriendData")]
        public async Task<IActionResult> AddFriendDataAsync(UserFriendData friendData)
        {
            return Ok(await _friendSerivce.AddOrUpdateFriendDataAsync(friendData));
        }

        [HttpGet]
        [Route("removeFriend/{userId}/{username}")]
        public async Task<IActionResult> RemoveFriendAsync(Guid userId,string username)
        {
            return Ok(await _friendSerivce.RemoveFriendAsync(userId,username));
        }
    }
}
