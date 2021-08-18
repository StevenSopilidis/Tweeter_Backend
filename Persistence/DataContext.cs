using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext: IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<UserFollowing>(e => {
                e.HasKey(f => new {f.FollowingId, f.FollowerId});

                e.HasOne(f => f.Follower)
                    .WithMany(f => f.Followings)
                    .HasForeignKey(f => f.FollowingId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(f => f.Following)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Retweet> Retweets { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<UserFollowing> UserFollowings { get; set; }
        public DbSet<UserProfileImage> UserProfileImages { get; set; }
    }
}