using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Posts
{
    //for getting the latest posts
    public class Latest
    {
        public class Query : IRequest<Result<PagedList<PostDto>>>
        {
            public PagingParams PagingParams { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<PostDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<PagedList<PostDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Posts
                    .OrderByDescending(p => p.Posted_On)                    
                    .ProjectTo<PostDto>(_mapper.ConfigurationProvider, new
                    {
                        currentUserId = _userAccessor.GetUserId()
                    })
                    .AsQueryable();

                var posts = await PagedList<PostDto>.CreateAsync(query, request.PagingParams.PageNumber, request.PagingParams.PageSize);

                return Result<PagedList<PostDto>>.Success(posts);
            }
        }
    }
}