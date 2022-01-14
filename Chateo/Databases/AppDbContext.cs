using Chateo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Databases
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Friendship>(b =>
            {
                b.HasKey(x => new { x.UserId, x.UserFriendId });

                b.HasOne(x => x.User)
                    .WithMany(x => x.Friends)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.UserFriend)
                    .WithMany(x => x.FriendsOf)
                    .HasForeignKey(x => x.UserFriendId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<FriendRequest>(b =>
            {
                b.HasKey(x => new { x.UserFromId, x.UserToId });

                b.HasOne(x => x.UserFrom)
                    .WithMany(x => x.FriendRequests)
                    .HasForeignKey(x => x.UserFromId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.UserTo)
                    .WithMany(x => x.FriendRequestsOf)
                    .HasForeignKey(x => x.UserToId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
