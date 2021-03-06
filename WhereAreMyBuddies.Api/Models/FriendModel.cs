﻿using System;
using System.Runtime.Serialization;

namespace WhereAreMyBuddies.Api.Models
{
    [DataContract]
    public class FriendModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }

        [DataMember(Name = "isOnline")]
        public bool IsOnline { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "coordinatesTimestamp")]
        public DateTime CoordinatesTimestamp { get; set; }

        [DataMember(Name = "coordinatesTimestampDifference")]
        public string CoordinatesTimestampDifference { get; set; }

        [DataMember(Name = "distanceInMeters")]
        public int DistanceInMeters { get; set; }

        [DataMember(Name = "distanceInKilometersAsString")]
        public string DistanceInKilometersAsString { get; set; }

        [DataMember(Name = "distanceInMilesAsString")]
        public string DistanceInMilesAsString { get; set; }
    }
}