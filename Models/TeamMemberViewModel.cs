namespace WebSpaceApp.Models
{
    public class TeamMemberViewModel
    {
        public List<TeamMember> Members { get; set; }
        public string SearchQuery { get; set; }
        public string SelectedRole { get; set; }

        public List<string> Roles => new List<string>
        {
        "Project manager", "Site Foreman", "Plumber", "Electrician", "General Worker", "Artisan"
        };
    }
}
