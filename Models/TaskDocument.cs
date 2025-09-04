namespace WebSpaceApp.Models
{
    public class TaskDocument
    {
        public int Id { get; set; }
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string Comment { get; set; }
        public DateTime UploadDate { get; set; }
        public string Author { get; set; }
    }
}
