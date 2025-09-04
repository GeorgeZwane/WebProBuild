using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class CreateTaskViewModel
    {

        public string? Description { get; set; }
        public string? TaskName { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Startdate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Enddate { get; set; }
        public string? Priority { get; set; }
        public double? Progress { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
    
}
