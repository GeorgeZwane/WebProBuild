namespace WebSpaceApp.Models
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Location { get; set; }

        public required DateTime Startdate { get; set; }

        public required DateTime Enddate { get; set; }

        public required string Progress { get; set; }
    }
}
