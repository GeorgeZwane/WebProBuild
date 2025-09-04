namespace WebSpaceApp.Models
{
    internal class ForemanProgressViewModel
    {
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public int PendingMilestones { get; set; }
        public double CompletionRate { get; set; }
        public string Message { get; set; }
        public string PerformanceLevel { get; set; }
    }
}