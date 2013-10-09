using System;
using System.Runtime.Serialization;

namespace WhereAreMyBuddies.Api.Models
{
    [DataContract]
    public class FriendRequestModel
    {
        [DataMember(Name = "fromUserId")]
        public int FromUserId { get; set; }

        [DataMember(Name = "fromUserNickname")]
        public string FromUserNickname { get; set; }

        [DataMember(Name = "isShowed")]
        public bool IsShowed { get; set; }
    }
}