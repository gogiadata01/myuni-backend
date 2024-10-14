using System.ComponentModel.DataAnnotations;

namespace MyUni.Models
{
public class UserRecoverPasswordDto
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
}
}

