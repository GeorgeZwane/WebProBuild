namespace WebSpaceApp.Models
{
    public class TeamMember
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public string Avatar => string.IsNullOrEmpty(FullName) ? "?" : FullName.Substring(0, 1).ToUpper();
    }
}
