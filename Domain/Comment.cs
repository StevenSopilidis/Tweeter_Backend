using System;
using System.Collections.Generic;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int CommentLikeCount { get; set; } = 0;
        public ICollection<CommentLike> CommentLikes { get; set; }
        public AppUser AppUser { get; set; }
        public Guid UserId { get; set; }
    }
}