using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.Models
{
    public class SendNotificationViewModel
    {
        [Required]
        public int SenderId { get; set; }

        [Required]
        public int RecipientId { get; set; }


        [Required]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; }

        public List<SelectListItem> Foreman { get; set; } = new();
    }
}
