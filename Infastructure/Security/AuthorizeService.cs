using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using Persistence;

namespace Application.Services
{
    //class for checkinng wether a user own a certain entity
    public class AuthorizeService : IAuthorizeService
    {
        public async Task<bool> OwnsPost(Post post, DataContext dbContext, IUserAccessor userAccessor)
        {
            var userId = userAccessor.GetUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if(user == null) return false;
            return userId == post.UserId.ToString();
        }
    }
}