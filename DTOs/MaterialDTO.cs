using System.ComponentModel.DataAnnotations;

namespace WebSpaceApp.DTOs
{
    public class MaterialDTO
    {
      
        public int ProjectId { get; set; }
        public required string Name { get; set; }
        public required int Quantity { get; set; }
        public required string MetricUnit { get; set; }
    }
}
