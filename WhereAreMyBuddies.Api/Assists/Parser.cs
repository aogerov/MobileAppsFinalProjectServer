﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using WhereAreMyBuddies.Api.Models;
using WhereAreMyBuddies.Model;

namespace WhereAreMyBuddies.Api.Assists
{
    public class Parser
    {
        private const string Nickname = "NICKNAME";
        private const string Distance = "DISTANCE";
        private const string CoordinatesTimestamp = "COORDINATES_TIMESTAMP";
        private const string TimeDifferenceMoreThanDay = "more than 24 hours";
        private const string TimeDifferenceMoreThanYear = "more than one year ago";
        private const int DaysInYear = 365;
        private const int DaysInMonth = 30;
        private const int MetersInKilometer = 1000;
        private const int YardsInMile = 1760;
        private const double MetersToKilometer = 0.001;
        private const double MetersToYards = 1.0936133;
        private const double MetersToMiles = 0.000621371192;

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

        public static Coordinates CreateDefaultCoordinates(UserModel model)
        {
            return new Coordinates
            {
                Latitude = 0,
                Longitude = 0,
                Timestamp = DateTime.Now
            };
        }

        public static Coordinates CoordinatesModelToCoordinates(CoordinatesModel coordinatesModel)
        {
            return new Coordinates
            {
                Latitude =coordinatesModel.Latitude,
                Longitude = coordinatesModel.Longitude
            };
        }

        public static List<FriendModel> FriendsToFriendModels(User user, ICollection<User> friends, string orderBy)
        {
            var onlineFriends = new List<FriendModel>();
            var offlineFriends = new List<FriendModel>();
            // remove this after the public defence in Telerik!!! - start point
            var random = new Random();
            // remove this after the public defence in Telerik!!! - end point
            foreach (var friend in friends)
            {
                if (friend.Coordinates == null)
                {
                    continue;
                }

                var friendModel = Parser.FriendToFriendModel(friend);
                // remove this after the public defence in Telerik!!! - start point
                if (user.Nickname == "telerik" && friend.Nickname != "gercho" ||
                    user.Nickname == "gercho" && friend.Nickname != "telerik")
                {
                    double sofiaCenterLatitude = 42.697766;
                    double latitudeRandom = (double)random.Next(-10000, 10000) / 100000;
                    friendModel.Latitude = Math.Round(sofiaCenterLatitude + latitudeRandom, 6);

                    double sofiaCenterLongitude = 23.321311;
                    double longitudeRandom = (double)random.Next(-10000, 10000) / 100000;
                    friendModel.Longitude = Math.Round(sofiaCenterLongitude + longitudeRandom, 6);

                    //var time = user.Coordinates.Timestamp;
                    var time = DateTime.Now;
                    int timeRandom = random.Next(1, 180);
                    if (timeRandom > 120)
                    {
                        timeRandom = random.Next(1, 10);
                    }

                    time = time.AddMinutes(-timeRandom);
                    friendModel.CoordinatesTimestamp = time;
                    if (timeRandom <= 60)
                    {
                        friendModel.IsOnline = true;
                    }
                    else
                    {
                        friendModel.IsOnline = false;
                    }
                }
                // remove this after the public defence in Telerik!!! - end point
                if (friendModel.IsOnline)
                {
                    onlineFriends.Add(friendModel);
                }
                else
                {
                    offlineFriends.Add(friendModel);
                }
            }

            CalculateDistance(user, onlineFriends);
            CalculateDistance(user, offlineFriends);

            CalculateTimestampDifferences(onlineFriends);
            CalculateTimestampDifferences(offlineFriends);

            var sortedOnlineFriends = SortFriendLists(onlineFriends, orderBy);
            var sortedOfflineFriends = SortFriendLists(offlineFriends, orderBy);

            var friendModels = new List<FriendModel>();
            if (sortedOnlineFriends != null)
            {
                friendModels.AddRange(sortedOnlineFriends);
            }

            if (sortedOfflineFriends != null)
            {
                friendModels.AddRange(sortedOfflineFriends);
            }

            return friendModels;
        }

        public static FriendModel FriendToFriendModel(User friend)
        {
            var friendModel = new FriendModel
            {
                Id = friend.Id,
                Nickname = friend.Nickname,
                IsOnline = friend.IsOnline,
                Latitude = Math.Round(friend.Coordinates.Latitude, 6),
                Longitude = Math.Round(friend.Coordinates.Longitude, 6),
                CoordinatesTimestamp = friend.Coordinates.Timestamp
            };

            return friendModel;
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
                ThumbUrl = image.ThumbUrl,
                ImageDateAsString = image.ImageDateAsString,
                TimestampDifferenceWithCoordinates = image.TimestampDifferenceWithCoordinates,
                CoordinatesAccuracy = image.CoordinatesAccuracy,
                Latitude = image.Coordinates.Latitude,
                Longitude = image.Coordinates.Longitude
            };
        }

        public static Image ImageModelToImage(ImageModel imageModel)
        {
            return new Image
            {
                Url = imageModel.Url,
                ThumbUrl = imageModel.ThumbUrl,
                ImageDateAsString = imageModel.ImageDateAsString,
                TimestampDifferenceWithCoordinates = imageModel.TimestampDifferenceWithCoordinates,
                CoordinatesAccuracy = imageModel.CoordinatesAccuracy
            };
        }

        public static Coordinates ExtractCoordinatesFromImageModel(ImageModel imageModel)
        {
            return new Coordinates
            {
                Latitude = imageModel.Latitude,
                Longitude = imageModel.Longitude,
                Timestamp = DateTime.Now
            };
        }

        private static void CalculateTimestampDifferences(List<FriendModel> friends)
        {
            var timeNow = DateTime.Now;
            foreach (var friend in friends)
            {
                var timeDifference = timeNow.Subtract(friend.CoordinatesTimestamp);
                if (timeDifference.Days > 0)
                {
                    friend.CoordinatesTimestampDifference = TimeDifferenceMoreThanDay;
                    continue;
                }

                friend.CoordinatesTimestampDifference = GetTimeDifferenceAsString(timeDifference);
            }
        }

        private static string GetTimeDifferenceAsString(TimeSpan timeDifference)
        {
            StringBuilder difference = new StringBuilder();
            difference.Append(timeDifference.Hours + ":");
            if (timeDifference.Minutes < 10)
            {
                difference.Append("0");
            }

            difference.Append(timeDifference.Minutes + ":");
            if (timeDifference.Seconds < 10)
            {
                difference.Append("0");
            }

            difference.Append(timeDifference.Seconds);
            return difference.ToString();
        }

        private static void CalculateDistance(User user, List<FriendModel> friends)
        {
            if (user.Coordinates == null)
            {
                return;
            }

            var userGeoCoordinate = new GeoCoordinate(user.Coordinates.Latitude, user.Coordinates.Longitude);
            foreach (var friend in friends)
            {
                var friendGeoCoordinate = new GeoCoordinate(friend.Latitude, friend.Longitude);
                int meters = Convert.ToInt32(userGeoCoordinate.GetDistanceTo(friendGeoCoordinate));
                friend.DistanceInMeters = meters;

                double kilometers = meters * MetersToKilometer;
                int yards = Convert.ToInt32(meters * MetersToYards);
                double miles = meters * MetersToMiles;

                ParseDistancesToString(friend, meters, kilometers, yards, miles);
            }
        }

        private static void ParseDistancesToString(FriendModel friend, int meters, double kilometers, int yards, double miles)
        {
            if (meters < MetersInKilometer)
            {
                friend.DistanceInKilometersAsString = String.Format("{0} meters", meters);
            }
            else
            {
                friend.DistanceInKilometersAsString = String.Format("{0:F2} kilometers", kilometers);
            }

            if (yards < YardsInMile)
            {
                friend.DistanceInMilesAsString = String.Format("{0} yards", yards);
            }
            else
            {
                friend.DistanceInMilesAsString = String.Format("{0:F2} miles", miles);
            }
        }

        private static List<FriendModel> SortFriendLists(List<FriendModel> friends, string orderBy)
        {
            if (orderBy.ToLower() == Nickname.ToLower())
            {
                var sortedFriends = friends.OrderBy(f => f.Nickname);
                return sortedFriends.ToList();
            }

            if (orderBy.ToLower() == CoordinatesTimestamp.ToLower())
            {
                var sortedFriends = friends.OrderByDescending(f => f.CoordinatesTimestamp);
                return sortedFriends.ToList();
            }

            if (orderBy.ToLower() == Distance.ToLower())
            {
                var sortedFriends = friends.OrderBy(f => f.DistanceInMeters);
                return sortedFriends.ToList();
            }

            return null;
        }
    }
}