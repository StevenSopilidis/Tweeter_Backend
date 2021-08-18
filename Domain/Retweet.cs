using System;

namespace Domain
{
    public class Retweet
    {
        public Guid Id { get; set; }
        public AppUser AppUser { get; set; }
        public Guid AppUserId { get; set; }
        public Post Post { get; set; }
        public Guid PostId { get; set; }    
    }
}