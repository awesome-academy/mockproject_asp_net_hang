using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_management.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Room")]
        [Required]
        public int RoomId { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("RoomDiscount")]
        public int? DiscountId { get; set; }

        public decimal FinalPrice { get; set; }

        public byte Status { get; set; } // 1=Pending, 2=Confirmed...

        public byte PaymentMethod { get; set; } // 1=cash, 2=online
        public byte PaymentStatus { get; set; } // 1=paid, 2=unPaid

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
        public virtual RoomDiscount RoomDiscount { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
