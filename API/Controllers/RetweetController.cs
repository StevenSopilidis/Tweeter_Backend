using System;
using System.Threading.Tasks;
using Application.Core;
using Application.Retweets;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class RetweetController : BaseController
    {
        [HttpPost("{postId}")]
        public async Task<IActionResult> CreateRetweet(Guid postId)
        {
            return HandleResult(await Mediator.Send(new Create.Command{PostId= postId}));
        }

        //get retweets of a certain user
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUsersRetweets([FromQuery]PagingParams pagingParams, string userId)
        {
            return HandleResult(await Mediator.Send(new List.Query{PagingParams=pagingParams, UserId=userId}));
        }
    }
}