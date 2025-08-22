using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_management.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("RoomType")]
        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public byte Status { get; set; } // 1=Available, 2=Occupied...

        public string Floor { get; set; }

        public string ImageUrl { get; set; }

        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
