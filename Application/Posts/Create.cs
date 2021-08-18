using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper.QueryableExtensions;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence;

namespace Application.Posts
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            //data for creating a post
            public CreatePostDto CreatePostDto { get; set; }
        }

        public class CommandValidator: AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.CreatePostDto).SetValidator(new PostsValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ICloudinaryServices _cloudinaryServices;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, ICloudinaryServices cloudinaryServices,IUserAccessor userAccessor)
            {
                _context = context;
                _cloudinaryServices = cloudinaryServices;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            { 
                var user = await _context.Users.FindAsync(_userAccessor.GetUserId());
                if(user == null)
                    return Result<Unit>.Unauthorize();
                Post post;
                //if photo is not null upload it to cloudinary
                if(request.CreatePostDto.Photo != null)
                {
                    var cloudinary_result = await _cloudinaryServices.UploadPhoto(request.CreatePostDto.Photo);
                    //check if the photo was uploaded successfully
                    if(cloudinary_result.Error != null)
                        return Result<Unit>.Failed(cloudinary_result.Error.Message);

                    // if the photo was uploaded successfully
                    post = new Post
                    {
                        Content= request.CreatePostDto.Content,
                        Public= request.CreatePostDto.Public,
                        User=user,
                        PostImage= new PostImage
                        {
                            Url=cloudinary_result.SecureUrl.AbsoluteUri,
                            PublicId= cloudinary_result.PublicId,
                        }                        
                    };
                }
                else{
                    post = new Post{
                        Content= request.CreatePostDto.Content,
                        Public= request.CreatePostDto.Public,
                        User=user,
                        UserId= new Guid(user.Id)
                    };
                }
                _context.Add(post);
                //check if post saved in the database successfully
                var result = await _context.SaveChangesAsync() > 0;
                if(!result && post.PostImage != null)
                {
                    //delete the image from the db
                    await _cloudinaryServices.DeleteImage(post.PostImage.PublicId);
                    return Result<Unit>.Failed("Post could not be saved to the database");
                }
                //if post was saved to db successfully
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}