﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace MyUni.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Img { get; set; }
        public int Coin { get; set; }
        public string Token { get; set; } // Token for password reset
        public DateTime? ResetTokenExpiry { get; set; } // Token expiration time
        public DateTime? LastQuizAttempt { get; set; } // Nullable for users who haven't attempted a quiz
        public int RemainingTime { get; set; } // In seconds

    }

}
