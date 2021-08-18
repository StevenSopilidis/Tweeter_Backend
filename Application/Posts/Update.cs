using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Posts
{
    public class Update
    {
        public class Command : IRequest<Result<Unit>>
        {
            public CreatePostDto PostDto { get; set; }
            public Guid Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.PostDto).SetValidator(new PostsValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ICloudinaryServices _cloudinaryService;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly IAuthorizeService _authorizeService;
            public Handler(DataContext context, IAuthorizeService authorizeService,
            ICloudinaryServices cloudinaryService, IMapper mapper, IUserAccessor userAccessor)
            {
                _authorizeService = authorizeService;
                _cloudinaryService = cloudinaryService;
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var post = await _context.Posts
                    .FirstOrDefaultAsync(p => p.Id == request.Id);

                if (post == null) return null;
                if (await _authorizeService.OwnsPost(post, _context, _userAccessor) == false)
                    return Result<Unit>.Unauthorize();

                //if user has alread uploaded an image, 
                //and has provided new image delete it from cloudinary and use the new one
                if (post.PostImage != null && request.PostDto.Photo != null)
                {
                    var uploadResult = await _cloudinaryService.UploadPhoto(request.PostDto.Photo);
                    if (uploadResult.Error != null) return Result<Unit>.Failed(uploadResult.Error.Message);

                    await _cloudinaryService.DeleteImage(post.PostImage.PublicId);
                    post.PostImage.Url = uploadResult.Url.AbsolutePath;
                    post.PostImage.PublicId = uploadResult.PublicId;
                }
                //if user was to upload image to the post but he has not previously uplaoded image
                if (post.PostImage == null && request.PostDto.Photo != null)
                {
                    var uploadResult = await _cloudinaryService.UploadPhoto(request.PostDto.Photo);
                    if (uploadResult.Error != null) return Result<Unit>.Failed(uploadResult.Error.Message);
                    post.PostImage = new PostImage
                    {
                        Url = uploadResult.Url.AbsolutePath,
                        PublicId = uploadResult.PublicId
                    };
                }
                post.Content = request.PostDto.Content;
                post.Public = request.PostDto.Public;
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failed("Post could not be updated");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}