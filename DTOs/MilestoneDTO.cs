using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.DTOs
{
    public class MilestoneDTO
    {
        public required string MilestoneName { get; set; }
        public required string Description { get; set; }
        public required DateTime DueDate { get; set; }

        // Link to Task
        public Guid TaskEntityId { get; set; }
    }
}
