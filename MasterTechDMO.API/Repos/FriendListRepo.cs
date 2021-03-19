using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterTechDMO.API.Areas.Identity.Data;
using MasterTechDMO.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mtsDMO.Context.Utility;

namespace MasterTechDMO.API.Repos
{
    public class FriendListRepo : IFriendListRepo
    {
        private MTDMOContext _context;
        public FriendListRepo(MTDMOContext context) => _context = context;

        public async Task<APICallResponse<bool>> AddOrUpdateFriendDataAsync(DMOUserFriendList friendData)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbFriend = _context.DMOUserFriendList.Find(friendData.Id);
                if (dbFriend != null)
                {
                    dbFriend.EmailId = friendData.EmailId;
                    dbFriend.Name = friendData.Name;
                    dbFriend.PhoneNumber = friendData.PhoneNumber;
                    dbFriend.UpdDT = DateTime.Now;
                    dbFriend.UpdUser = friendData.UserId;
                    callResponse.Respose = true;
                    callResponse.Message = new List<string> { "Friend data update." };
                    callResponse.Status = "Success";
                }
                else
                {
                    friendData.InsDT = DateTime.Now;
                    friendData.InsUser = friendData.UserId;
                    _context.DMOUserFriendList.Add(friendData);
                    callResponse.Respose = true;
                    callResponse.Message = new List<string> { "Friend data added." };
                    callResponse.Status = "Success";
                }
                _context.SaveChanges();
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {

                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }

        public async Task<APICallResponse<DMOUserFriendList>> GetFriendDataByEmailAsync(Guid userId, string friendEmailId)
        {
            try
            {
                var callResponse = new APICallResponse<DMOUserFriendList>();
                var dbFriend = _context.DMOUserFriendList.Where(x => x.UserId == userId && x.EmailId == friendEmailId).FirstOrDefault();
                if (dbFriend != null)
                {
                    callResponse.Respose = dbFriend;
                    callResponse.Message = new List<string> { "Friend found." };
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Respose = null;
                    callResponse.Message = new List<string> { "Oops! Friend not found." };
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<DMOUserFriendList>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<List<DMOUserFriendList>>> GetFriendListAsync(Guid userId)
        {
            try
            {
                var callResponse = new APICallResponse<List<DMOUserFriendList>>();
                var dbFriendList = _context.DMOUserFriendList.Where(x => x.UserId == userId).ToList();
                if (dbFriendList != null)
                {
                    callResponse.Message = new List<string>() { $"{dbFriendList.Count} frined found." };
                    callResponse.Respose = dbFriendList;
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Message = new List<string>() { "Friend List is empty." };
                    callResponse.Status = "Warning";
                    callResponse.Respose = null;
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<List<DMOUserFriendList>>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = null
                };
            }
        }

        public async Task<APICallResponse<bool>> RemoveFriendAsync(Guid userId, string username)
        {
            try
            {
                var callResponse = new APICallResponse<bool>();
                var dbFriend = _context.DMOUserFriendList.Where(x => x.UserId == userId && x.EmailId == username).FirstOrDefault();
                if (dbFriend != null)
                {
                    _context.DMOUserFriendList.Remove(dbFriend);
                    _context.SaveChanges();
                    callResponse.Respose = true;
                    callResponse.Message = new List<string> { "Friend removed." };
                    callResponse.Status = "Success";
                }
                else
                {
                    callResponse.Respose = true;
                    callResponse.Message = new List<string> { "Oops! Friend not found." };
                    callResponse.Status = "Warning";
                }
                callResponse.IsSuccess = true;
                return callResponse;
            }
            catch (Exception Ex)
            {
                return new APICallResponse<bool>
                {
                    IsSuccess = false,
                    Message = new List<string>() { Ex.Message },
                    Status = "Error",
                    Respose = false
                };
            }
        }
    }
}
