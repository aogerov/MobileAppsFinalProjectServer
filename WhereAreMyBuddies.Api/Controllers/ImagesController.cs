using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhereAreMyBuddies.Api.Assists;
using WhereAreMyBuddies.Api.Models;
using WhereAreMyBuddies.Data;

namespace WhereAreMyBuddies.Api.Controllers
{
    public class ImagesController : BaseApiController
    {
        // api/images/get?sessionKey={sessionKey}
        [HttpPost]
        [ActionName("get")]
        public HttpResponseMessage PostResponseToFriendRequest(
            [FromBody]FriendModel friendModel, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExeptions(() =>
            {
                using (var context = new WhereAreMyBuddiesContext())
                {
                    var user = Validator.ValidateSessionKey(context, sessionKey);
                    var friend = Validator.ValidateFriendInDb(context, friendModel.Id, friendModel.Nickname);

                    if (!user.Friends.Contains(friend))
                    {
                        return this.Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    var imageModel = Parser.ImageToImageModel(friend.Image);
                    var response = this.Request.CreateResponse(HttpStatusCode.OK, imageModel);
                    return response;
                }
            });

            return responseMsg;
        }
    }
}
