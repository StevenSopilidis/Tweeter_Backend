using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Following;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class FollowController : BaseController
    {
        [HttpPost("{userId}")]
        public async Task<IActionResult> FollowUser(string userId)
        {
            return HandleResult(await Mediator.Send(new FollowingToggle.Command { TargetId = userId }));
        }

        //for getting the followings and followers of a certain user
        [HttpGet("{userId}/{predicate}")]
        public async Task<IActionResult> GetUserFollowings(string userId, string predicate)
        {
            return HandleResult(await Mediator.Send(new List.Query { UserId=userId, Predicate=predicate }));
        }

    }
}