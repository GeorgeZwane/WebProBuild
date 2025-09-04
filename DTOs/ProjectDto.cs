using WebSpaceApp.Models;

namespace WebSpaceApp.DTOs
{
    public class ProjectDto
    {
      
        public required string Name { get; set; }

        public required string Location { get; set; }

        public required DateTime Startdate { get; set; }

        public required DateTime Enddate { get; set; }

        public double? Progress { get; set; }
        public required double Budget { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }
        public int ProjectId { get; set; }
        
    }
}