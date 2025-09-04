using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class MilestoneModel
    {
        [Required(ErrorMessage = "Milestone name is required")]
        [Display(Name = "Milestone Name")]
        public string? MilestoneName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        [Required]
        public Guid TaskEntityId { get; set; }
    }
}
