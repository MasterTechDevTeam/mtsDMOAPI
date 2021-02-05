﻿using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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

        public UserManagementRepo(UserManager<DMOUsers> userManager)
        {
            _userManager = userManager;
        }

        public async Task<APICallResponse<string>> RegisterUserAsync(DMOUsers user,string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var tokenCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                tokenCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenCode));

                return new APICallResponse<string>
                {
                    IsSuccess = true,
                    Status="Success",
                    Message = new List<string>(){ "User Registered and Verification Token Generated" },
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

        public async Task<APICallResponse<bool>> VerifyUserAsync(string userId, string code)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new APICallResponse<bool>
                    {
                        IsSuccess = true,
                        Status  = "Warning",
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
                        Status = "Warning",
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
                        Respose =false
                    };
                }
            }
            catch (Exception Ex)
            {


                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() {Ex.Message},
                    Status = "Error",
                    Respose = false
                };
            }
        }
    }
}