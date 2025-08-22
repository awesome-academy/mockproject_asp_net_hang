using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_management.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } // standard, deluxe...

        public byte View { get; set; } // 1=sea, 2=mountain

        [Required]
        public byte Type { get; set; } // 1=single, 2=double...

        public int Capacity { get; set; }

        public decimal Price { get; set; } // original price

        public string Description { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<RoomDiscount> RoomDiscounts { get; set; }
    }
}
