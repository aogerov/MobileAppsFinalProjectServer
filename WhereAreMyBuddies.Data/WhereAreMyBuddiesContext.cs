using System;
using System.Data.Entity;
using WhereAreMyBuddies.Model;

namespace WhereAreMyBuddies.Data
{
    public class WhereAreMyBuddiesContext : DbContext
    {
        public WhereAreMyBuddiesContext()
            : base("WhereAreMyBuddiesDb")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Coordinates> Coordinates { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Friends)
                .WithMany();

            modelBuilder.Entity<User>()
                .Property(u => u.SessionKey)
                .IsFixedLength()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.AuthCode)
                .IsFixedLength()
                .HasMaxLength(40);

            base.OnModelCreating(modelBuilder);
        }
    }
}
