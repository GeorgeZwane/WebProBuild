using WebSpaceApp.DTOs;

namespace WebSpaceApp.Models
{
    public class MetricCard
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }

    public class RecentItem
    {
        public string Identifier { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
    }

    public class DashboardViewModel
    {
        public string Greeting { get; set; }
        public List<MetricCard> Metrics { get; set; }
        public List<RecentItem> RecentActivities { get; set; }
        public List<string> RevenueLabels { get; set; }
        public List<int> RevenueValues { get; set; }

        //===============================================
        //Mr Georges approach to displaying in metrics
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int InWaitingTasks { get; set; }

        public int ProjectCount { get; set; }
        public int CompleteProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int InWaitingProjects { get; set; }
        
            public string ProjectName { get; set; }
        // Tasks grouped by status, key = status, value = list of tasks
        public Dictionary<string, List<TaskDTO>> TasksByStatus { get; set; } = new();
        public double TotalBudget { get; set; }
    }

    

}
