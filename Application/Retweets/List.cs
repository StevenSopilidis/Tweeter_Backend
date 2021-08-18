using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Retweets
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<SavedOrRetweetedPostDto>>>
        {
            public PagingParams PagingParams { get; set; }
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<SavedOrRetweetedPostDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

            }
            public async Task<Result<PagedList<SavedOrRetweetedPostDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if(user == null) return null; 

                var query = _context.Retweets
                    .Where(r => r.AppUserId == new Guid(request.UserId))
                    .ProjectTo<SavedOrRetweetedPostDto>(_mapper.ConfigurationProvider, new { currentUserId = request.UserId})
                    .AsQueryable();
                
                return Result<PagedList<SavedOrRetweetedPostDto>>.Success(
                    await PagedList<SavedOrRetweetedPostDto>.CreateAsync(query, request.PagingParams.PageNumber, request.PagingParams.PageSize)
                );
            }
        }
    }
}