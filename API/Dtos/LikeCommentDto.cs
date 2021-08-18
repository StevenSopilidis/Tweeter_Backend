using System;

namespace API.Dtos
{
    public class LikeCommentDto
    {
        public Guid commentId { get; set; }
        public Guid postId { get; set; }
    }
}