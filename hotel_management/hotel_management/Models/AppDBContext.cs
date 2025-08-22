using System.Data.Entity;

namespace hotel_management.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("HotelManagementConnection") { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserActivation> UserActivations { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomDiscount> RoomDiscounts { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasRequired(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .WillCascadeOnDelete(false); // tránh cascade vòng lặp

            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.FinalPrice)
                .HasPrecision(18, 2); // set column type for FinalPrice

            modelBuilder.Entity<RoomType>()
                .Property(r => r.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RoomDiscount>()
                .Property(r => r.DiscountValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.FinalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Review>()
                .HasRequired(rv => rv.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(rv => rv.UserId)
                .WillCascadeOnDelete(false); // tắt luôn cho chắc

            modelBuilder.Entity<Review>()
                .HasRequired(rv => rv.Reservation)
                .WithMany(r => r.Reviews)
                .HasForeignKey(rv => rv.ReservationId)
                .WillCascadeOnDelete(false);
        }

    }
}