using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhereAreMyBuddies.Api.Assists;
using WhereAreMyBuddies.Api.Models;
using WhereAreMyBuddies.Data;

namespace WhereAreMyBuddies.Api.Controllers
{
    public class FriendsController : BaseApiController
    {
        // api/friends/all?orderBy={orderBy}&sessionKey={sessionKey}
        [HttpGet]
        [ActionName("all")]
        public HttpResponseMessage GetAllFriends([FromUri]string orderBy, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExeptions(() =>
            {
                using (var context = new WhereAreMyBuddiesContext())
                {
                    var user = Validator.ValidateSessionKey(context, sessionKey);

                    foreach (var friend in user.Friends)
                    {
                        if (friend.Coordinates == null)
                        {
                            continue;
                        }
                        
                        if (friend.Coordinates.Timestamp.AddHours(-2) > DateTime.Now)
                        {
                            friend.IsOnline = false;
                        }
                    }                
                                        
                    context.SaveChanges();

                    // remove this after the public defence in Telerik!!! - start point
                    if (user.Nickname == "telerik")
                    {
                        var teleriksCoordinates = user.Coordinates;
                        var random = new Random();
                        
                        foreach (var friend in user.Friends)
                        {
                            int onlineRandom = random.Next(1, 10);
                            if (onlineRandom > 3)
                            {
                                friend.IsOnline = true;
                            }
                            else
                            {
                                friend.IsOnline = false;
                            }

                            double sofiaCenterLatitude = 42.697766;
                            double sofiaCenterLongitude = 23.321311;
                            double coordinatesRandom = random.Next(-14000, 14000) / 1000000;
                            friend.Coordinates.Latitude = sofiaCenterLatitude + coordinatesRandom;
                            friend.Coordinates.Longitude = sofiaCenterLongitude + coordinatesRandom;
                        }
                    }
                    // remove this after the public defence in Telerik!!! - end point

                    var friendModels = Parser.FriendsToFriendModels(user, user.Friends, orderBy);

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, friendModels);
                    return response;
                }
            });

            return responseMsg;
        }

        // api/friends/find?friendNickname={friendNickname}&sessionKey={sessionKey}
        [HttpGet]
        [ActionName("find")]
        public HttpResponseMessage GetFindFriend([FromUri]string friendNickname, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExeptions(() =>
            {
                using (var context = new WhereAreMyBuddiesContext())
                {
                    string friendNicknameToLower = friendNickname.Trim().ToLower();
                    Validator.ValidateNickname(friendNicknameToLower);
                    var user = Validator.ValidateSessionKey(context, sessionKey);

                    if (user.Nickname.ToLower() == friendNicknameToLower ||
                        user.Friends.FirstOrDefault(f => f.Nickname.ToLower() == friendNicknameToLower) != null)
                    {
                        return this.Request.CreateResponse(HttpStatusCode.OK);
                    }

                    var friendFound = context.Users.FirstOrDefault(u => u.Nickname == friendNicknameToLower);
                    var friendFoundModel = Parser.UserToFriendFoundModel(friendFound);

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, friendFoundModel);
                    return response;
                }
            });

            return responseMsg;
        }

        // api/friends/remove?sessionKey={sessionKey}
        [HttpPost]
        [ActionName("remove")]
        public HttpResponseMessage PostAddFriendRequest([FromBody]FriendModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExeptions(() =>
            {
                using (var context = new WhereAreMyBuddiesContext())
                {
                    var user = Validator.ValidateSessionKey(context, sessionKey);
                    var friend = Validator.ValidateFriendInDb(context, model.Id, model.Nickname);
                    bool isErrorInRemoveAppeared = false;

                    if (user.Friends.Contains(friend))
                    {
                        user.Friends.Remove(friend);
                    }
                    else
                    {
                        isErrorInRemoveAppeared = true;
                    }

                    if (friend.Friends.Contains(user))
                    {
                        friend.Friends.Remove(user);
                    }
                    else
                    {
                        isErrorInRemoveAppeared = true;
                    }

                    context.SaveChanges();
                    if (isErrorInRemoveAppeared)
                    {
                        throw new ArgumentException("Error in friends removing appeared");
                    }

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            });

            return responseMsg;
        }
    }
}
