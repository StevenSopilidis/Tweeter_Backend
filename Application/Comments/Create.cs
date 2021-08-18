using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public CreateCommentDto CreateCommentDto { get; set; }
        }
        
        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.CreateCommentDto).SetValidator(new CommentValidator());
            }
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

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //check if the postId the user provided can be converted to Gui
                Guid postId;
                try
                {
                    postId = new Guid(request.CreateCommentDto.PostId);
                }
                catch (System.Exception ex)
                {
                    return Result<Unit>.Failed("The post you are trying to comment does not exist");
                }

                var user = await _context.Users.FindAsync(_userAccessor.GetUserId());
                if (user == null)
                    return Result<Unit>.Failed("The account you are trying to create a post with does not exist");
                var post = await _context.Posts.Include(p => p.Comments)
                    .SingleOrDefaultAsync(p => p.Id == postId);
                if (post == null)
                    return Result<Unit>.Failed("The post you are trying to comment does not exist");
                var comment = new Comment
                {
                    Content= request.CreateCommentDto.Content,
                    AppUser= user,
                    UserId= new Guid(user.Id)
                };
                post.Comments.Add(comment);
                post.Num_of_Comments++;
                if(await _context.SaveChangesAsync() > 0)
                    return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failed("Could not create comment");
            } 
        }
    }
}