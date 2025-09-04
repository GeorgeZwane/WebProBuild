namespace WebSpaceApp.DTOs
{
    public class AddTaskDTO
    {
        public required string TaskName { get; set; }
        public required string Description { get; set; }
        public required DateTime Startdate { get; set; }
        public required DateTime Enddate { get; set; }
        public required string Priority { get; set; }
        public required int ProjectId { get; set; }
        public int UserId { get; set; }
    }
}
