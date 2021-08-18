using System;
using System.Collections.Generic;
using Domain;

namespace Application.Dtos
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Num_of_Likes { get; set; } = 0;
        public int Num_of_Comments { get; set; } = 0;
        public int Num_of_Retweets { get; set; } = 0;
        public int Num_of_Saved { get; set; } = 0;
        public DateTime Posted_On { get; set; } = DateTime.Now;
        public bool Public { get; set; }
        public PostImage PostImage { get; set; }
        public UserDto User { get; set; }
        public string UserId { get ; set;}
        public ICollection<CommentDto> Comments { get; set; }
        public ICollection<Retweet> Retweets { get; set; }
        public ICollection<SavedPost> Saves { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
        // whether the current user has saved the post
        public bool Saved { get; set; }
        //wheter the current user has retweeted the post
        public bool Retweeted { get; set; }
        //wheter the current user has liked the post
        public bool Liked { get; set; }
    }
}