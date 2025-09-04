namespace WebSpaceApp.Models
{
    public class SingleProjectViewModel
    {
        public int ProjectId { get; set; } 
        public string ProjectName { get; set; }
        public string Location { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public int Progress { get; set; }

      
        public List<ViewTaskModel> Tasks { get; set; }
        public List<MaterialItemModel> Materials { get; set; }
       // public List<TeamMemberViewModel> TeamMembers { get; set; }
    }
}
