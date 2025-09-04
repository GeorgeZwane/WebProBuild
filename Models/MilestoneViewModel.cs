namespace WebSpaceApp.Models
{
    public class MilestoneViewModel
    {
        public Guid Id { get; set; }
        public string MilestoneName { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
