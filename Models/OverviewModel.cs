using WebSpaceApp.DTOs;

namespace WebSpaceApp.Models
{
    public class OverviewModel
    {
        public string TaskName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalTasks { get; set; }
        public int CompleteTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int InWaitingTasks { get; set; }

        public string ProjectName { get; set; }
        public int ProjectCount { get; set; }
        public int CompleteProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int InWaitingProjects { get; set; }
        public Dictionary<string, List<TaskDTO>> TasksByStatus { get; set; } = new();
        public double TotalBudget { get; set; }
    }
}
