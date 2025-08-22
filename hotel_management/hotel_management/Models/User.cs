using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_management.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [ForeignKey("Role")]
        [Required]
        public int RoleId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<UserActivation> UserActivations { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
