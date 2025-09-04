namespace WebSpaceApp.Models
{
    public class BudgetItem
    {
        public string Category { get; set; }
        public decimal BudgetedAmount { get; set; }
        public decimal ActualSpend { get; set; }
        public decimal Variance => ActualSpend - BudgetedAmount;
        public string Status => Variance > 0 ? "Over Budget" : "Under Budget";
    }
}
