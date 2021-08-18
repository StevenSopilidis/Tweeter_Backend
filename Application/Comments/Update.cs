using Application.Core;
using MediatR;
using Domain;
using FluentValidation;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Persistence;
using System;

namespace Application.Comments
{
    public class Update
    {
        public class Command: IRequest<Result<Unit>>
        {
            public string UpdatedContent { get; set; }
            public Guid CommentId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.UpdatedContent).NotEmpty().MinimumLength(4).MaximumLength(150);
            }
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
                
                var comment = await _context.Comments.FindAsync(request.CommentId);
                if(comment == null) return null;
                if(comment.UserId.ToString() != _userAccessor.GetUserId()) return Result<Unit>.Unauthorize();

                comment.Content = request.UpdatedContent;
                if(await _context.SaveChangesAsync() > 0 ) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failed("Could not update comment");
            }
        }
    }
}