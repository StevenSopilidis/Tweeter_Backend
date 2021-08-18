using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Posts
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ICloudinaryServices _cloudinary;
            private readonly IUserAccessor _userAccessor;
            private readonly IAuthorizeService _authorizeService;
            public Handler(DataContext context, ICloudinaryServices cloudinary,
            IUserAccessor userAccessor, IAuthorizeService authorizeService)
            {
                _authorizeService = authorizeService;
                _userAccessor = userAccessor;
                _cloudinary = cloudinary;
                _context = context;
            }


        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var post = await _context.Posts.FindAsync(request.Id);
            if (post == null) return null;
            if (await _authorizeService.OwnsPost(post, _context, _userAccessor) == false)
                return Result<Unit>.Unauthorize();
            if (post.PostImage != null)
            {
                await _cloudinary.DeleteImage(post.PostImage.PublicId);
            }
            _context.Remove(post);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return Result<Unit>.Failed("Could not delete post");
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
}