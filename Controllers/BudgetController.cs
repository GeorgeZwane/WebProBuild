using Microsoft.AspNetCore.Mvc;
using WebSpaceApp.Models;

public class BudgetController : Controller
{
    [HttpGet]
    public IActionResult BudgetEntry()
    {
        ViewBag.UserRole = User.IsInRole("Director") ? "Director" :
        User.IsInRole("ProjectManager") ? "ProjectManager" : "User";
        return View(new BudgetEntryModel { Date = DateTime.Today });
    }
    [HttpGet]
    public IActionResult Finance()
    {
        return View();
    }
    private static List<BudgetEntryModel> _dummyBudgetEntries = new List<BudgetEntryModel>
        {
            new BudgetEntryModel { Category = "Labor", Amount = 15000.00m, Description = "Initial labor costs", Date = DateTime.Today.AddDays(-10) },
            new BudgetEntryModel { Category = "Materials", Amount = 8500.50m, Description = "Concrete and steel purchase", Date = DateTime.Today.AddDays(-5) },
            new BudgetEntryModel { Category = "Equipment", Amount = 4200.00m, Description = "Tool rentals", Date = DateTime.Today.AddDays(-2) }
        };

    [HttpGet]
    public IActionResult BudgetList()
    {
        var viewModel = new BudgetListViewModel
        {
            BudgetEntries = _dummyBudgetEntries
        };

        return View(viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddBudgetEntry(BudgetEntryModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["SuccessMessage"] = "Budget entry added successfully!";
            return RedirectToAction("BudgetEntry");
        }

        return View("BudgetEntry", model);
    }

    public ActionResult BudgetReport()
    {
        var report = new BudgetReport
        {
            TotalBudget = 500000,
            SpentSoFar = 350000,
            OverbudgetItems = 25000,
            BudgetDetails = new List<BudgetItem>
            {
                new BudgetItem { Category = "Materials", BudgetedAmount = 200000, ActualSpend = 225000 },
                new BudgetItem { Category = "Labor", BudgetedAmount = 150000, ActualSpend = 100000 },
                new BudgetItem { Category = "Equipment", BudgetedAmount = 100000, ActualSpend = 25000 },
                new BudgetItem { Category = "Miscellaneous", BudgetedAmount = 50000, ActualSpend = 20000 }
            }
        };

        return View(report);
    }
}
