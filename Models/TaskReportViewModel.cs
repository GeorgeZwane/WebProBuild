namespace WebSpaceApp.Models
{
    public class TaskMetric
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Note { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class TaskDetail
    {
        public string TaskName { get; set; }
        public string AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public string OverdueBy { get; set; }
    }

    public class TaskReportViewModel
    {
        public List<TaskMetric> Metrics { get; set; }
        public List<TaskDetail> OverdueTasks { get; set; }
        public List<string> ChartLabels { get; set; }
        public List<int> ChartValues { get; set; }

        //===============================================
        //Mr Georges approach to displaying in metrics
        public int TotalTasks { get; set; }
        public int CompleteTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int IncompleteTasks { get; set; }
    }

}
