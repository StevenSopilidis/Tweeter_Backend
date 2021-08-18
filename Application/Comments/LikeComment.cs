using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class LikeComment
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid PostId { get; set; }
            public Guid CommentId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            //if user hasnt liked the comment it  will like it 
            //else if the user has liked the comment it will remove
            //the like from the comment
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(_userAccessor.GetUserId());
                if (user == null)
                    return Result<Unit>.Unauthorize();
                
                var post = await _context.Posts
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.CommentLikes)
                    .FirstOrDefaultAsync(p => p.Id == request.PostId);
                if(post == null) return null;
                
                var comment = post.Comments.FirstOrDefault(c => c.Id == request.CommentId);
                if(comment == null) return null;

                var commentLike = comment.CommentLikes.SingleOrDefault(cl => cl.UserId.ToString() == user.Id);
                if(commentLike == null)
                {
                    var newCommentLike = new CommentLike
                    {
                        AppUser= user,
                        UserId= new Guid(user.Id)
                    };
                    comment.CommentLikes.Add(newCommentLike);
                    comment.CommentLikeCount++;
                }
                else {
                    comment.CommentLikes.Remove(commentLike);
                    comment.CommentLikeCount--;
                };
                
                if(await _context.SaveChangesAsync() > 0) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failed("Could not like comment");
            }
        }
    }
}