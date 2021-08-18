using System;
using System.Threading.Tasks;
using API.Dtos;
using Application.Comments;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CommentsController: BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto)
        {
            return HandleResult(await Mediator.Send(new Create.Command{CreateCommentDto= createCommentDto}));
        }

        [HttpDelete("{commentId}/post/{postId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId, Guid postId)
        {
            return HandleResult(await Mediator.Send(new Delete.Command{CommentId=commentId,PostId=postId}));
        }
        
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody]string updatedContent)
        {
            return HandleResult(await Mediator.Send(new Update.Command{CommentId=commentId,UpdatedContent=updatedContent}));
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikeComment(LikeCommentDto likeCommentDto)
        {
            return HandleResult(await Mediator.Send(new LikeComment.Command{
                CommentId=likeCommentDto.commentId,
                PostId=likeCommentDto.postId
            }));
        }
    }
}