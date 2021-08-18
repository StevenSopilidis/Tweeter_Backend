using System;
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

namespace Application.SavedPosts
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
                _context = context;
                _mapper=mapper;
            }
            public async Task<Result<PagedList<SavedOrRetweetedPostDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if(user == null) return null;

                var query = _context.SavedPosts
                    .Where(sp => sp.AppUserId == new Guid(request.UserId))
                    .ProjectTo<SavedOrRetweetedPostDto>(_mapper.ConfigurationProvider, new { currentUserId = request.UserId})
                    .AsQueryable();
                
                return Result<PagedList<SavedOrRetweetedPostDto>>.Success(
                    await PagedList<SavedOrRetweetedPostDto>.CreateAsync(query, request.PagingParams.PageNumber, request.PagingParams.PageSize)
                );
            }
        }
    }
}