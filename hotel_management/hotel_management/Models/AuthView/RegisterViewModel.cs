using System.ComponentModel.DataAnnotations;

namespace hotel_management.Models.AuthView
{
    public class RegisterViewModel
    {

        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
