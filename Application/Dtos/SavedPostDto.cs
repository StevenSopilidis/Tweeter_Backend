using System;
using System.Collections.Generic;
using Domain;

namespace Application.Dtos
{
    public class SavedOrRetweetedPostDto
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
        //wether the currentUser has saved this post
        public bool Saved { get; set; }
        //wether the currentUser has retweeted this post
        public bool Retweeted { get; set; }
        //wether the current user has liked this sasved post
        public bool Liked { get; set; }
    }
}