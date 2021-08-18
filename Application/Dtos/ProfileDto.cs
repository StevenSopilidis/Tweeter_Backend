using System.Collections.Generic;

namespace Application.Dtos
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public ICollection<PostDto> Posts { get; set; }
        public ICollection<RetweetDto> Retweets { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
    }
}