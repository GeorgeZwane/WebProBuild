using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.DTOs
{
    public class TaskDTO
    {
        public Guid Id { get; set; }
        public required string Description { get; set; }
        public required string TaskName { get; set; }

        [DataType(DataType.DateTime)]
        public required DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }
        public required string Priority { get; set; }

        public double? Progress { get; set; }

        public required int ProjectId { get; set; }
        public required string AssignedTo { get; set; }

        public int UserId { get; set; }
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public int PendingMilestones { get; set; }
        public double CompletionRate { get; set; }
        public string Message { get; set; }
        public string PerformanceLevel { get; set; }

    }
}
