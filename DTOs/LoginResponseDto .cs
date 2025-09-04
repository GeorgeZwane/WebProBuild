using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.DTOs
{
    public class LoginResponseDto
    {
        [Key]
        public int UserId { get; set; }
        public required string UserRole { get; set; }
        public required string UserName { get; set; }

        public string Token { get; set; } = string.Empty;
    }
}
