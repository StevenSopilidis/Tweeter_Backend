using Application.Dtos;
using FluentValidation;

namespace Application.Comments
{
    public class CommentValidator : AbstractValidator<CreateCommentDto>
    {
        public CommentValidator()
        {
            RuleFor(c => c.Content).NotEmpty().MinimumLength(4).MaximumLength(150);
            RuleFor(c => c.PostId).NotEmpty();

        }
    }
}