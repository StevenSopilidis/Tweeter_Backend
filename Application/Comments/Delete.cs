using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid CommentId { get; set; }
            public Guid PostId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(_userAccessor.GetUserId());
                if(user == null) return Result<Unit>.Unauthorize();

                var post = await _context.Posts
                    .Include(p => p.Comments)
                    .FirstOrDefaultAsync(p => p.Id == request.PostId);
                if(post == null) return null;
                var comment = post.Comments.SingleOrDefault(c => c.Id == request.CommentId);
                if(comment == null) return null;
                if(comment.UserId.ToString() != user.Id) return Result<Unit>.Unauthorize();
                comment = null;
                post.Num_of_Comments--;
                if(await _context.SaveChangesAsync() > 0) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failed("Could not remove comment");
            }
        }
    }
}