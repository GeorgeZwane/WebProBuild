namespace WebSpaceApp.Models
{
    public class DocumentUpdate
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string DocumentTitle { get; set; }
        public string Comment { get; set; }
    }
}
