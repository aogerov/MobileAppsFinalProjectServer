using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhereAreMyBuddies.Api.Assists;
using WhereAreMyBuddies.Api.Models;
using WhereAreMyBuddies.Data;
using WhereAreMyBuddies.Model;

namespace WhereAreMyBuddies.Api.Controllers
{
    public class ImagesController : BaseApiController
    {
        // api/images/set?sessionKey={sessionKey}
        [HttpPost]
        [ActionName("set")]
        public HttpResponseMessage PostSetNewImage(
            [FromBody]ImageModel imageModel, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExeptions(() =>
            {
                using (var context = new WhereAreMyBuddiesContext())
                {
                    if (imageModel == null)
                    {
                        return this.Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    var user = Validator.ValidateSessionKey(context, sessionKey);
                    var image = Parser.ImageModelToImage(imageModel);
                    user.Images.Add(image);

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, imageModel);
                    return response;
                }
            });

            return responseMsg;
        }

        // api/images/get?imagesCount={imagesCount}&sessionKey={sessionKey}
        [HttpPost]
        [ActionName("get")]
        public HttpResponseMessage PostGetFriendsImage(
            [FromBody]FriendModel friendModel, [FromUri]int imagesCount, [FromUri]string sessionKey)
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

                    var imageModels = new List<ImageModel>();
                    if (friend.Images.Count > 0 && imagesCount > 0)
                    {
                        var images = new List<Image>(friend.Images);
                        for (int i = friend.Images.Count - 1; i >= 0 || imagesCount > 0; i--, imagesCount--)
                        {
                            var imageModel = Parser.ImageToImageModel(images[i]);
                            imageModels.Add(imageModel);
                        }
                    }

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, imageModels);
                    return response;
                }
            });

            return responseMsg;
        }
    }
}
