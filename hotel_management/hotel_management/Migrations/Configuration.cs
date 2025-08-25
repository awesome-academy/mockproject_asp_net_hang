namespace hotel_management.Migrations
{
    using hotel_management.Models;
    using hotel_management.Services.Auth;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<hotel_management.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(hotel_management.Models.AppDbContext context)
        {
            context.Roles.AddOrUpdate(
                r => r.Id,
                new Role { Id = 1, Name = "Admin", Description = "Quản trị hệ thống" },
                new Role { Id = 2, Name = "Staff", Description = "Nhân viên khách sạn" },
                new Role { Id = 3, Name = "Customer", Description = "Khách hàng" }
            );

            context.SaveChanges();

            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            var staffRole = context.Roles.FirstOrDefault(r => r.Name == "Staff");
            var customerRole = context.Roles.FirstOrDefault(r => r.Name == "Customer");

            context.Users.AddOrUpdate(
                u => u.Email,
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = PasswordHasher.HashPassword("Admin@123"),
                    RoleId = 1, // Admin
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "staff1",
                    Email = "staff@example.com",
                    Password = PasswordHasher.HashPassword("Staff@123"),
                    RoleId = 2, // Staff
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "customer1",
                    Email = "customer@example.com",
                    Password = PasswordHasher.HashPassword("Customer@123"),
                    RoleId = 3, // Customer
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }
    }
}
