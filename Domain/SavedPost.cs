using System;

namespace Domain
{
    public class SavedPost
    {
        public Guid SavedPostId { get; set; }
        public AppUser AppUser { get; set; }
        public Guid AppUserId { get; set; }
        public Post Post { get; set; }
        public Guid PostId { get; set; }
    }
}