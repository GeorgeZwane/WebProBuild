namespace WebSpaceApp.Models
{
    public class BudgetReport
    {
        public decimal TotalBudget { get; set; }
        public decimal SpentSoFar { get; set; }
        public decimal RemainingBudget => TotalBudget - SpentSoFar;
        public decimal OverbudgetItems { get; set; }
        public List<BudgetItem> BudgetDetails { get; set; } 
    } 
}
