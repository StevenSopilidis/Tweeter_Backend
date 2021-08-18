using System;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Posts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PostController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery]PagingParams pagingParams)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query{PagingParams=pagingParams}));
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestPosts([FromQuery]PagingParams pagingParams)
        {
            return HandlePagedResult(await Mediator.Send(new Latest.Query{PagingParams= pagingParams}));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query{Id= id}));

        }
        
        [HttpGet("saved/{userId}")]
        public async Task<IActionResult> GetSavedPosts([FromQuery]PagingParams pagingParams, string userId)
        {
            return HandlePagedResult(await Mediator.Send(new Application.SavedPosts.List.Query{
                PagingParams= pagingParams,UserId=userId
            }));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm]CreatePostDto post)
        {
            return HandleResult(await Mediator.Send(new Create.Command{CreatePostDto= post}));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromForm]CreatePostDto updatedPost)
        {
            return HandleResult(await Mediator.Send(new Update.Command{PostDto=updatedPost, Id=id}));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command{Id=id} ));
        }

        [HttpPost("save/{postId}")]
        public async Task<IActionResult> SavePost(string postId)
        {
            return HandleResult(await Mediator.Send(new Application.SavedPosts.Create.Command{PostId=postId}));
        }

        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(string postId)
        {
            return HandleResult(await Mediator.Send(new LikePost.Command{PostId=postId}));
        }

    }
}