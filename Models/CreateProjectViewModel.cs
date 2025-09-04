using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class CreateProjectViewModel
    {
        [JsonProperty("projectId")]
        public int Id { get; set; } 

        public required string Name { get; set; }

        public required string Location { get; set; }

        public required DateTime Startdate { get; set; }

        public required DateTime Enddate { get; set; }

        public double? Progress { get; set; }
        public required double Budget { get; set; }
        public string? Description { get; set; }
        public int UserId {  get; set; }

        public string? Status { get; set; }

        // public RegisterViewModel UserSession { get; set; }
    }
}
