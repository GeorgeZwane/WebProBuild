using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class RegisterViewModel
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Role { get; set; }


        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
