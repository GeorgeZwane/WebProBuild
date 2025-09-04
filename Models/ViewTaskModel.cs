using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class ViewTaskModel
    {

        public Guid Id { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; } = string.Empty;

        [Display(Name = "Task Description")]
        public string Description { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public string? Priority { get; set; }
        public required string AssignedTo { get; set; }
        public double? Progress { get; set; } 
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
    }
}
