using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_management.Models
{
    public class RoomDiscount
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }

        [Required]
        public byte DiscountType { get; set; } // 1=percent, 2=fixed

        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
