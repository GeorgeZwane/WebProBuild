
namespace WebSpaceApp.Models
{
    public class BudgetBreakdownViewModel
    {
        public CreateProjectViewModel TopProject { get; set; }
        public double TotalBudget => Projects.Sum(p => p.Project.Budget);
        public double AvgProgress => Projects.Any() ? Projects.Average(p => p.Project.Progress ?? 0) : 0;

        public int TotalTasksAcrossAll { get; set; }
        public int TotalMilestonesAcrossAll { get; set; }

        public List<ProjectStats> Projects { get; set; } = new();

        public class ProjectStats
        {
            public CreateProjectViewModel Project { get; set; }
            public int TaskCount { get; set; }
            public int MilestoneCount { get; set; }
            public List<TaskInfo> Tasks { get; set; } = new();

            public class TaskInfo
            {
                public string TaskName { get; set; }
                public int Milestones { get; set; }
                public string AssignedUser { get; set; }
            }
        }
    }
}
