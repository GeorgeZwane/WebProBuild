using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class MilestoneUpdate
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Task Name")]
        public string MilestoneName { get; set; }

        [Required]
        public string Status { get; set; }

        public string Reason { get; set; }
    }
}
