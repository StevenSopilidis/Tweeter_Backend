using System;

namespace Domain
{
    public class UserFollowing
    {
        public string FollowerId { get; set; }
        public AppUser Follower { get; set; }
        public string FollowingId { get; set; }
        public AppUser Following { get; set; }
    }
}