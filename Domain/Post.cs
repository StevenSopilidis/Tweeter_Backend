using System;
using System.Collections.Generic;

namespace Domain
{
    //class that represents the Post entity 
    public class Post
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Num_of_Comments { get; set; } = 0;
        public int Num_of_Likes { get; set; } = 0;
        public int Num_of_Retweets { get; set; } = 0;
        public int Num_of_Saved { get; set; } = 0;
        public DateTime Posted_On { get; set; } = DateTime.Now;
        public bool Public { get; set; }
        public PostImage PostImage { get; set; }
        public AppUser User { get; set; }
        public Guid UserId { get ; set; } 
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Retweet> Retweets { get; set; }
        public ICollection<SavedPost> Saves { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
    }
}