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
    }
}