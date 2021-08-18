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

namespace Application.SavedPosts
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string PostId { get; set; }
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
                    .Include(p => p.Saves)
                    .FirstOrDefaultAsync(p => p.Id == new Guid(request.PostId));
                if(post == null) return null;

                //dont allow the posts author to save his own post
                if(post.UserId.ToString() == user.Id) 
                    return Result<Unit>.Failed("Cannot save your own post");            

                var userSavedPost = post.Saves.FirstOrDefault(s => s.AppUserId.ToString() == user.Id);
                if(userSavedPost == null)
                {
                    var saved = new SavedPost
                    {
                        AppUser= user,
                        AppUserId= new Guid(user.Id),
                        Post= post
                    };
                    post.Num_of_Saved++;
                    post.Saves.Add(saved);
                    _context.SavedPosts.Add(saved);
                }else {
                    _context.SavedPosts.Remove(userSavedPost);
                    post.Num_of_Saved--;
                }
                
                var result = await _context.SaveChangesAsync() > 0;
                if(result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failed("Could not save post");
            }
        }
    }
}