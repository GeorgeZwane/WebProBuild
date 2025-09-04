using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class RegisterUpdate
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Cell Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit number")]
        public string CellNo { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
