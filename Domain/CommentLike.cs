using System;

namespace Domain
{
    public class CommentLike
    {
        public Guid Id { get; set; }
        public DateTime LikeOn { get; set; } = DateTime.Now;
        public AppUser AppUser { get; set; }
        public Guid UserId { get; set; }
    }
}