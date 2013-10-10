using System;
using System.Collections.Generic;
using System.Linq;
using WhereAreMyBuddies.Api.Models;
using WhereAreMyBuddies.Model;

namespace WhereAreMyBuddies.Api.Assists
{
    public class Parser
    {
        public static User UserModelToUser(UserModel userModel)
        {
            return new User
            {
                Username = userModel.Username.Trim().ToLower(),
                Nickname = userModel.Nickname.Trim(),
                AuthCode = userModel.AuthCode.Trim()
            };
        }

        public static UserLoggedModel UserToUserLoggedModel(User user)
        {
            return new UserLoggedModel
            {
                Nickname = user.Nickname,
                SessionKey = user.SessionKey
            };
        }

        public static Coordinates CoordinatesModelToCoordinates(CoordinatesModel coordinatesModel)
        {
            return new Coordinates
            {
                Latitude = coordinatesModel.Latitude,
                Longitude = coordinatesModel.Longitude
            };
        }

        public static IEnumerable<FriendModel> FriendsToFriendModels(IOrderedEnumerable<User> friendEntities)
        {
            var friendModels =
                (from friendEntity in friendEntities
                 select new FriendModel
                 {
                     Id = friendEntity.Id,
                     Nickname = friendEntity.Nickname,
                     IsOnline = friendEntity.IsOnline,
                     Latitude = friendEntity.Coordinates.Latitude,
                     Longitude = friendEntity.Coordinates.Longitude
                 });

            return friendModels;
        }

        public static FriendModel UserToFriendFoundModel(User friendFound)
        {
            if (friendFound == null)
            {
                return null;
            }

            return new FriendModel
            {
                Id = friendFound.Id,
                Nickname = friendFound.Nickname,
                IsOnline = friendFound.IsOnline
            };
        }

        public static FriendRequest CreateFriendRequest(User userThatMakesRequest)
        {
            return new FriendRequest
            {
                FromUserId = userThatMakesRequest.Id,
                FromUserNickname = userThatMakesRequest.Nickname
            };
        }

        public static IEnumerable<FriendRequestModel> FriendRequestsToFriendRequestModels(
            IOrderedEnumerable<FriendRequest> friendRequestsEntities)
        {
            var friendRequestModels =
                (from friendRequestEntity in friendRequestsEntities
                 select new FriendRequestModel
                 {
                     FromUserId = friendRequestEntity.FromUserId,
                     FromUserNickname = friendRequestEntity.FromUserNickname,
                     IsShowed = friendRequestEntity.IsShowed
                 });

            return friendRequestModels;
        }

        public static ImageModel ImageToImageModel(Image image)
        {
            return new ImageModel
            {
                Url = image.Url,
                DateTimeAtCapturing = image.DateTimeAtCapturing,
                LatitudeAtCapturing = image.Coordinates.Latitude,
                LongitudeAtCapturing = image.Coordinates.Longitude
            };
        }

        public static Image ImageModelToImage(ImageModel imageModel)
        {
            return new Image
            {
                Url = imageModel.Url,
                DateTimeAtCapturing = imageModel.DateTimeAtCapturing,
                Coordinates = new Coordinates
                {
                    Latitude = imageModel.LatitudeAtCapturing,
                    Longitude = imageModel.LongitudeAtCapturing
                }
            };
        }
    }
}