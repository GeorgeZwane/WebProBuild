using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class CreateTaskViewModel
    {
        [Required(ErrorMessage = "Task name is required")]
        public string? TaskName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.DateTime)]
        public DateTime Startdate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.DateTime)]
        public DateTime Enddate { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string? Priority { get; set; }

        public double? Progress { get; set; }

        [Required(ErrorMessage = "Project ID is required")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Please select a foreman")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid foreman")]
        public int AssignedTo { get; set; }

        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}