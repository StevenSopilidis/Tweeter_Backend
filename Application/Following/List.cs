using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Following
{
    public class List
    {
        public class Query : IRequest<Result<List<Profile2Dto>>>
        {
            public string UserId { get; set; }

            //determins wether we return the followers of followings of a user
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Profile2Dto>>>
        {
            private readonly IUserAccessor _userAcccessor;
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAcccessor, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
                _userAcccessor = userAcccessor;
            }

            public async Task<Result<List<Profile2Dto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profiles = new List<Profile2Dto>();

                switch (request.Predicate)
                {
                    //by whom the user is followed
                    case "followers":
                        profiles = await _context.UserFollowings.Where(u => u.Following.Id == request.UserId)
                            .Select(u => u.Follower)
                            .ProjectTo<Profile2Dto>(_mapper.ConfigurationProvider, new {
                                currentUserId = _userAcccessor.GetUserId()
                            })
                            .ToListAsync();
                        break;
                    case "followings":
                        //who the user follows
                        profiles = await _context.UserFollowings.Where(u => u.Follower.Id == request.UserId)
                        .Select(u => u.Following)
                        .ProjectTo<Profile2Dto>(_mapper.ConfigurationProvider, new {
                            currentUserId = _userAcccessor.GetUserId()
                        })
                        .ToListAsync();
                        break;
                }
                
                return Result<List<Profile2Dto>>.Success(profiles);
            }
        }
    }
}