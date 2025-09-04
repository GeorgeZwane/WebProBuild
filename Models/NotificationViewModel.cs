namespace WebSpaceApp.Models
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
