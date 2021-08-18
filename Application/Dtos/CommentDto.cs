using System;
using System.Collections.Generic;
using Domain;

namespace Application.Dtos
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Content { get; set; }
        public int CommentLikeCount { get; set; }
        public ICollection<CommentLike> CommentLikes { get; set; }

        //wether the currently loggedIn User has liked the comment
        public bool Liked { get; set; }
    }
}