using System;
using System.Linq;
using Application.Dtos;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            string currentUserId = null;
            CreateMap<Post, Post>();

            CreateMap<AppUser, UserDto>();

            CreateMap<Post, PostDto>()
                .ForMember(d => d.Saved, opt => opt.MapFrom(s => s.Saves.Any(saves => 
                    saves.AppUserId == new Guid(currentUserId)
                )))
                .ForMember(d => d.Retweeted, opt => opt.MapFrom(s => s.Retweets.Any(r => 
                    r.AppUserId == new Guid(currentUserId)
                )))
                .ForMember(d => d.Liked, opt => opt.MapFrom(s => s.PostLikes.Any(r => 
                    r.AppUserId == new Guid(currentUserId)
                )));
            
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.Liked, opt => opt.MapFrom(s => s.CommentLikes.Any(cl => 
                    cl.UserId == new Guid(currentUserId)
                )));


            CreateMap<AppUser, ProfileDto>()
                .ForMember(d => d.FollowerCount, opt => opt.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingCount, opt => opt.MapFrom(s => s.Followings.Count));
            
            CreateMap<Retweet, RetweetDto>();

            CreateMap<Retweet, SavedOrRetweetedPostDto>()
                .ForMember(d => d.Content, opt => opt.MapFrom(r => r.Post.Content))                
                .ForMember(d => d.Public, opt => opt.MapFrom(r => r.Post.Public))                
                .ForMember(d => d.PostImage, opt => opt.MapFrom(r => r.Post.PostImage))                
                .ForMember(d => d.Num_of_Likes, opt => opt.MapFrom(r => r.Post.Num_of_Likes))
                .ForMember(d => d.Num_of_Comments, opt => opt.MapFrom(r => r.Post.Num_of_Comments))
                .ForMember(d => d.Num_of_Retweets, opt => opt.MapFrom(r => r.Post.Num_of_Retweets))
                .ForMember(d => d.Num_of_Saved, opt => opt.MapFrom(r => r.Post.Num_of_Saved))
                .ForMember(d => d.User, opt => opt.MapFrom(r => r.Post.User))
                .ForMember(d => d.Liked, opt => opt.MapFrom(sp => sp.Post.PostLikes.Any(l => l.AppUserId == new Guid(currentUserId))))
                .ForMember(d => d.Saved, opt => opt.MapFrom(sp => sp.Post.Saves.Any(l => l.AppUserId == new Guid(currentUserId))))
                .ForMember(d => d.Retweeted, opt => opt.MapFrom(sp => sp.Post.Retweets.Any(l => l.AppUserId == new Guid(currentUserId))));

            CreateMap<SavedPost, SavedOrRetweetedPostDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(sp => sp.Post.Id))
                .ForMember(d => d.Content, opt => opt.MapFrom(sp => sp.Post.Content))
                .ForMember(d => d.Public, opt => opt.MapFrom(r => r.Post.Public))                
                .ForMember(d => d.PostImage, opt => opt.MapFrom(r => r.Post.PostImage))                
                .ForMember(d => d.Num_of_Likes, opt => opt.MapFrom(sp => sp.Post.Num_of_Likes))
                .ForMember(d => d.Num_of_Comments, opt => opt.MapFrom(sp => sp.Post.Num_of_Comments))
                .ForMember(d => d.Num_of_Retweets, opt => opt.MapFrom(sp => sp.Post.Num_of_Retweets))
                .ForMember(d => d.Num_of_Saved, opt => opt.MapFrom(sp => sp.Post.Num_of_Saved))
                .ForMember(d => d.User, opt => opt.MapFrom(sp => sp.Post.User))
                .ForMember(d => d.Liked, opt => opt.MapFrom(sp => sp.Post.PostLikes.Any(l => l.AppUserId == new Guid(currentUserId))))
                .ForMember(d => d.Saved, opt => opt.MapFrom(sp => sp.Post.Saves.Any(l => l.AppUserId == new Guid(currentUserId))))
                .ForMember(d => d.Retweeted, opt => opt.MapFrom(sp => sp.Post.Retweets.Any(l => l.AppUserId == new Guid(currentUserId))));

            CreateMap<AppUser, Profile2Dto>()
                .ForMember(d => d.FollowerCount, opt => opt.MapFrom(u => u.Followers.Count))
                .ForMember(d => d.FollowingCount, opt => opt.MapFrom(u => u.Followings.Count));
        }
    }
}