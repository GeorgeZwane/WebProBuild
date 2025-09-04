using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class AddTeamViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProjectId { get; set; }

      
        public string? TeamName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TeamCreationDate { get; set; }
    }
}
