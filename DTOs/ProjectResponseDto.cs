using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.DTOs
{
    public class ProjectResponseDto
    {

        [Key]
        public int ProjectId { get; set; }

        public string Token { get; set; } = string.Empty;
    }
}
