using System.Threading.Tasks;
using Domain;
using Persistence;

namespace Application.Interfaces
{
    public interface IAuthorizeService
    {
        Task<bool> OwnsPost(Post post, DataContext dbContext, IUserAccessor userAccessor);
    }
}