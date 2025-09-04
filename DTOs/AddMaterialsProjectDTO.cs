namespace WebSpaceApp.DTOs
{
    public class AddMaterialsProjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }  
        public string MetricUnit { get; set; } = string.Empty;
        public int ProjectId { get; set; }

    }
}
