using System.ComponentModel.DataAnnotations;

namespace MyUni.Models
{
    public class UserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Img { get; set; }
        public DateTime? LastLogin { get; set; } // Track last login for coin logic
        public int Coin { get; set; }
        public int RemainingTime { get; set; } // In seconds
        public string Token { get; set; } // This should be in your model
    }
}
