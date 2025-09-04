using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Services;

public class TeamController : Controller
{
    private readonly ProjectServices _projectServices;

    public TeamController(ProjectServices projectServices)
    {
        _projectServices = projectServices;
    }

    // GET: Team/AddTeamView
    [HttpGet]
    public IActionResult AddTeamView()
    {
        return View(new AddTeamViewModel());
    }

    // POST: Team/AddTeamView
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTeamView(AddTeamViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Map ViewModel to DTO used by the API
        var teamDto = new TeamDto
        {
            UserId = model.UserId,
            ProjectId = model.ProjectId,
            TeamName = model.TeamName,
            TeamCreationDate = model.TeamCreationDate
        };

        // You may want to get the token from session or user context
        string token = HttpContext.Session.GetString("JwtToken"); // example, adjust based on your setup

        var response = await _projectServices.CreateTeamAsync(teamDto, token);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Failed to create team. Please try again.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    // GET: Team/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        var teams = await _projectServices.GetAllTeamsAsync(token); // assuming this method exists
        return View(teams);
    }
}
