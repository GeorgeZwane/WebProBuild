using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Services;

namespace WebSpaceApp.Controllers
{
    public class ProjectOverviewController : Controller
    {
        private readonly ProjectServices _projectServices;

        public ProjectOverviewController(ProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        [HttpGet]
        public async Task<IActionResult> ProjectOverview()
        {
            string? token = HttpContext.Session.GetString("JwtToken");

            HttpResponseMessage response = await _projectServices.GetProjectAndTaskCountsAsync(token);
            if (!response.IsSuccessStatusCode)
            {
                // default empty view
                return View("ProjectOverview", new OverviewModel());
            }

            string json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OverviewModel>(json);

            Console.WriteLine($"Tasks => Total: {result.TotalTasks}, Complete: {result.CompleteTasks}, In Progress: {result.InProgressTasks}, Incomplete: {result.InWaitingTasks}");
            Console.WriteLine($"Projects => Total: {result.ProjectCount}, Complete: {result.CompleteProjects}, In Progress: {result.InProgressProjects}, Incomplete: {result.InWaitingProjects}");
            Console.WriteLine($"Budget Spent: {result.TotalBudget}");

            return View("ProjectOverview", result);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
