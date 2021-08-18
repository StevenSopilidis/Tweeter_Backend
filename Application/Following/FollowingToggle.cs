using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Following
{
    public class FollowingToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string TargetId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FindAsync(_userAccessor.GetUserId());
                if(user == null)
                    return Result<Unit>.Unauthorize();
                var target = await _context.Users.FindAsync(request.TargetId);
                if(target == null) return null;

                var following = await _context.UserFollowings.FindAsync(user.Id, target.Id);
                if(following != null)
                {
                    _context.UserFollowings.Remove(following);
                    System.Console.WriteLine("Removed following");
                }else {
                    _context.UserFollowings.Add(new UserFollowing{
                        Follower= user,
                        FollowerId= user.Id,
                        Following= target,
                        FollowingId= target.Id
                    });
                    System.Console.WriteLine("Added following");
                }

                var result = await _context.SaveChangesAsync() > 0;
                
                if(result)
                    return Result<Unit>.Success(Unit.Value);

                
                return Result<Unit>.Failed("Could not follow user");
            }
        }
    }
}