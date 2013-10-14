﻿using System;
using System.Runtime.Serialization;

namespace WhereAreMyBuddies.Api.Models
{
    [DataContract]
    public class CoordinatesModel
    {
        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }
    }
}