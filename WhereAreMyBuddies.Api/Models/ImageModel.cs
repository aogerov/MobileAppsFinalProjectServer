using System;
using System.Runtime.Serialization;

namespace WhereAreMyBuddies.Api.Models
{
    [DataContract]
    public class ImageModel
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "dateTimeAtCapturing")]
        public DateTime DateTimeAtCapturing { get; set; }

        [DataMember(Name = "latitudeAtCapturing")]
        public string LatitudeAtCapturing { get; set; }

        [DataMember(Name = "longitudeAtCapturing")]
        public string LongitudeAtCapturing { get; set; }
    }
}