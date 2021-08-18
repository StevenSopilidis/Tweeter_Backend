using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Posts
{
    public class LikePost
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string PostId { get; set; }
        }


        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccssor;
            public Handler(DataContext context, IUserAccessor userAccssor)
            {
                _userAccssor = userAccssor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(_userAccssor.GetUserId());
                if(user == null)
                    return Result<Unit>.Unauthorize();

                var post = await _context.Posts.Include(p => p.PostLikes).FirstOrDefaultAsync(p => p.Id == new Guid(request.PostId));                 
                if(post == null) return null;

                var userPostLike = post.PostLikes.FirstOrDefault(pl => pl.AppUserId.ToString() == user.Id);
                if(userPostLike == null)
                {
                    userPostLike = new PostLike
                    {
                        AppUser=user,
                        AppUserId= new Guid(user.Id),
                        Post=post
                    };
                    _context.PostLikes.Add(userPostLike);
                    post.PostLikes.Add(userPostLike);
                    post.Num_of_Likes++;
                } else{
                    post.Num_of_Likes--;
                    post.PostLikes.Remove(userPostLike);
                    _context.PostLikes.Remove(userPostLike);
                }
                var result = await _context.SaveChangesAsync() > 0;
                if(result)
                    return Result<Unit>.Success(Unit.Value);
                
                return Result<Unit>.Failed("Could not like post");
            }
        }
    }
}