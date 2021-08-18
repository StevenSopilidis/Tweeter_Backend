
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

namespace Application.Retweets
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid PostId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(_userAccessor.GetUserId());
                if(user == null) 
                    return Result<Unit>.Unauthorize();
                
                var post = await _context.Posts
                    .Include(p => p.Retweets)
                    .FirstOrDefaultAsync(p => p.Id == request.PostId);

                if(post == null) return null;

                var userHasAlreadyRetweetedPost = post.Retweets.FirstOrDefault(r => r.AppUserId.ToString() == user.Id);

                if(userHasAlreadyRetweetedPost != null)
                {
                    _context.Retweets.Remove(userHasAlreadyRetweetedPost);
                    post.Num_of_Retweets--;
                }else {
                    var retweet = new Retweet
                    {
                        AppUser= user,
                        AppUserId= new Guid(user.Id),
                        Post=post
                    };
                    post.Retweets.Add(retweet);
                    _context.Retweets.Add(retweet);
                    post.Num_of_Retweets++;
                }
                var result = await _context.SaveChangesAsync() > 0;
                if(result) return Result<Unit>.Success(Unit.Value);
                
                return Result<Unit>.Failed("Could not retweet"); 
            }
        }
    }
}