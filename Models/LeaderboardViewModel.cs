namespace WebSpaceApp.Models
{
    public class LeaderboardViewModel
    {
        public string FullName { get; set; }
        public int TotalTasks { get; set; }

        public int TotalMilestones { get; set; }
        public int CompletedTasks { get; set; }
        public int IncompletedTasks { get; set; }
        public int CompletedMilestone { get; set; }
        public int IncompletedMilestone { get; set; }

        public double AverageTaskCompletionRate { get; set; }
        public double AverageMilestoneCompletionRate { get; set; }

    }
}
