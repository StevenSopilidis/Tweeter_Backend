using Application.Dtos;
using Domain;
using FluentValidation;

namespace Application.Posts
{
    public class PostsValidator : AbstractValidator<CreatePostDto>
    {
        public PostsValidator()
        {
            RuleFor(p => p.Content).NotEmpty().MinimumLength(3).MaximumLength(400);
            RuleFor(p => p.Public).NotEmpty();
        }
    }
}