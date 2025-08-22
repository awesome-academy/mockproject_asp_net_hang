using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_management.Models
{
    public class UserActivation
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }


        [Required]
        public string Token { get; set; }

        [Required]
        public byte TokenType { get; set; } // 1=EmailVerification, 2=PasswordReset

        public DateTime ExpiredAt { get; set; }

        public byte IsUsed { get; set; } // 1=used, 2=unused

        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
