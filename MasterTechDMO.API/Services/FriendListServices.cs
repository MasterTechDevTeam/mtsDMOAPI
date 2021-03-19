using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using MasterTechDMO.API.Repos;
using mtsDMO.Context.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterTechDMO.API.Services
{
    public class FriendListServices
    {
        private IFriendListRepo _friendListRepo;

        public FriendListServices(MTDMOContext context)
        {
            _friendListRepo = new FriendListRepo(context);
        }

        public async Task<APICallResponse<List<UserFriendData>>> GetFriendListAsync(Guid userId)
        {
            var callResponse = new APICallResponse<List<UserFriendData>>();
            var dbFriendListCallResponse = await _friendListRepo.GetFriendListAsync(userId);

            if (dbFriendListCallResponse.Respose != null)
            {
                callResponse.Respose = new List<UserFriendData>();
                foreach (var dbFriend in dbFriendListCallResponse.Respose)
                {
                    callResponse.Respose.Add(new UserFriendData
                    {
                        EmailId = dbFriend.EmailId,
                        Id = dbFriend.Id,
                        Name = dbFriend.Name,
                        PhoneNumber = dbFriend.PhoneNumber,
                        UserId = dbFriend.UserId
                    });
                }
            }
            callResponse.IsSuccess = dbFriendListCallResponse.IsSuccess;
            callResponse.Message = dbFriendListCallResponse.Message;
            callResponse.Status = dbFriendListCallResponse.Status;
            return callResponse;
        }

        public async Task<APICallResponse<UserFriendData>> GetFriendDataByEmailAsync(Guid userId, string friendEmailId)
        {
            var callResponse = new APICallResponse<UserFriendData>();
            var dbFriendCallResponse = await _friendListRepo.GetFriendDataByEmailAsync(userId, friendEmailId);
            if (dbFriendCallResponse.Respose != null)
            {
                callResponse.Respose = new UserFriendData
                {
                    EmailId = dbFriendCallResponse.Respose.EmailId,
                    Id = dbFriendCallResponse.Respose.Id,
                    Name = dbFriendCallResponse.Respose.Name,
                    PhoneNumber = dbFriendCallResponse.Respose.PhoneNumber,
                    UserId = dbFriendCallResponse.Respose.UserId
                };
            }
            callResponse.IsSuccess = dbFriendCallResponse.IsSuccess;
            callResponse.Message = dbFriendCallResponse.Message;
            callResponse.Status = dbFriendCallResponse.Status;
            return callResponse;
        }

        public async Task<APICallResponse<bool>> AddOrUpdateFriendDataAsync(UserFriendData friendData)
        {
            var dbFriendData = new DMOUserFriendList
            {
                Id = friendData.Id,
                EmailId = friendData.EmailId,
                Name = friendData.Name,
                PhoneNumber = friendData.PhoneNumber,
                UserId = friendData.UserId
            };

            return await _friendListRepo.AddOrUpdateFriendDataAsync(dbFriendData);
        }

        public async Task<APICallResponse<bool>> RemoveFriendAsync(Guid userId,string username)
        {
            return await _friendListRepo.RemoveFriendAsync(userId, username);
        }

    }
}
