using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class MilestoneUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Milestone Name")] // Fixed the display name
        public string MilestoneName { get; set; }

        [Required]
        public string Status { get; set; }

        public string Reason { get; set; }

        // This is crucial for proper redirection
        public Guid TaskEntityId { get; set; }


    }
}
