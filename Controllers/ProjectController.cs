using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 
using System;
using System.Collections.Generic; 
using System.Net.Http; 
using System.Text;
using System.Threading.Tasks;
using WebSpaceApp.Attributes;
using WebSpaceApp.DTOs; 
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Services; 

namespace WebSpaceApp.Controllers
{
  
    public class ProjectController : Controller
    {
        private readonly ProjectServices _projectServices;


        public ProjectController(ProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        [HttpGet]
        [Route("api/webprojects")]
        public async Task<IActionResult> ProjectView()
        {
            string? token = HttpContext.Session.GetString("JwtToken");
            string? userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to view projects.";
                return RedirectToAction("Login", "Account");
            }

            List<CreateProjectViewModel> projects = new List<CreateProjectViewModel>();

            try
            {
                HttpResponseMessage apiResponse = await _projectServices.GetAllProjectsAsync(userId, token);

                if (apiResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var projectDtos = JsonConvert.DeserializeObject<List<ProjectDto>>(jsonResponse);

                    if (projectDtos != null)
                    {
                        foreach (var dto in projectDtos)
                        {
                            projects.Add(new CreateProjectViewModel
                            {
                                Id = dto.ProjectId,
                                Name = dto.Name,
                                Location = dto.Location,
                                Startdate = dto.Startdate,
                                Enddate = dto.Enddate,
                                Progress = dto.Progress,
                                Budget = dto.Budget,
                                Description = dto.Description, 
                                UserId = dto.UserId 
                            });
                        }
                    }
                }
                else
                {
                    string errorContent = await apiResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to load projects: {apiResponse.StatusCode}. Details: {errorContent}";
                    Console.WriteLine($"Error fetching projects: {apiResponse.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["ErrorMessage"] = $"Could not connect to the project service. Please try again later. Error: {httpEx.Message}";
                Console.WriteLine($"HttpRequestException in ProjectController.ProjectView: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                TempData["ErrorMessage"] = $"Failed to parse project data. Please try again later. Error: {jsonEx.Message}";
                Console.WriteLine($"JsonException in ProjectController.ProjectView: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred while loading projects: {ex.Message}";
                Console.WriteLine($"General Exception in ProjectController.ProjectView: {ex.Message}");
            }

            return View("ProjectView", projects);
        }

        public async Task<IActionResult> CompleteProjects()
        {
            string? token = HttpContext.Session.GetString("JwtToken");
            string? userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to view projects.";
                return RedirectToAction("Login", "Account");
            }

            List<CreateProjectViewModel> projects = new List<CreateProjectViewModel>();
            try
            {
                HttpResponseMessage apiResponse = await _projectServices.GetAllProjectViewAsync(userId, token);

                if (apiResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var projectDtos = JsonConvert.DeserializeObject<List<ProjectDto>>(jsonResponse);

                    if (projectDtos != null)
                    {
                        foreach (var dto in projectDtos)
                        {
                            projects.Add(new CreateProjectViewModel
                            {
                                Id = dto.ProjectId,
                                Name = dto.Name,
                                Location = dto.Location,
                                Startdate = dto.Startdate,
                                Enddate = dto.Enddate,
                                Progress = dto.Progress,
                                Budget = dto.Budget,
                                Status=dto.Status,
                                UserId = dto.UserId
                            });
                        }
                    }
                }
                else
                {
                    string errorContent = await apiResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to load projects: {apiResponse.StatusCode}. Details: {errorContent}";
                    Console.WriteLine($"Error fetching projects: {apiResponse.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["ErrorMessage"] = $"Could not connect to the project service. Please try again later. Error: {httpEx.Message}";
                Console.WriteLine($"HttpRequestException in ProjectController.ProjectView: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                TempData["ErrorMessage"] = $"Failed to parse project data. Please try again later. Error: {jsonEx.Message}";
                Console.WriteLine($"JsonException in ProjectController.ProjectView: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred while loading projects: {ex.Message}";
                Console.WriteLine($"General Exception in ProjectController.ProjectView: {ex.Message}");
            }

            return View("CompleteProjects", projects);
        }


        // GET:/Project/CreateProject (Displaying the form to create a new project)
        [HttpGet]
        public IActionResult CreateProject()
        {
            return View(); // I'm returning an empty view (Create Project)
        }

        // POST: /Project/CreateProject (Handles the form submission for creating a new project)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(CreateProjectViewModel model)
        {
            // Server-side validation 
            if (!ModelState.IsValid)
            {
                // If validation fails return the view And Display the error that occured
                return View(model);
            }

            //  Retrieving the JWT token
            string? userIdString = HttpContext.Session.GetString("UserId");
            string? userIdString2 = HttpContext.Session.GetString("UserRole");
            string? userIdString3 = HttpContext.Session.GetString("UserName");
            Console.WriteLine($"Session UserId: {userIdString}"); 
            Console.WriteLine($"Session UserRole: {userIdString2}"); 
            Console.WriteLine($"Session UserName: {userIdString3}"); 

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to create a project.";
                return RedirectToAction("Login", "Account"); // Redirecting to the user to login page if no token
            }

            // Mapping ProjectViewModel to ProjectDto
            // Ensuring that ProjectDto matches what the API expects for a new project.
            var projectDto = new ProjectDto
            {
                Name = model.Name,
                Location = model.Location,
                Startdate = model.Startdate,
                Enddate = model.Enddate,
                Progress = model.Progress,
                Budget = model.Budget,
                Description = model.Description,
                UserId = userId
            };

            try
            {
                HttpResponseMessage apiResponse = await _projectServices.CreateProjectAsync(projectDto, userIdString);
                string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                var projectResponse = JsonConvert.DeserializeObject<ProjectResponseDto>(jsonResponse);

                if (apiResponse.IsSuccessStatusCode)
                {
                   
                    if (projectResponse != null)
                    {
                        HttpContext.Session.SetString("ProjectId", projectResponse.ProjectId.ToString());
                    }
                    TempData["SuccessMessage"] = "Project created successfully!";
                    return RedirectToAction("ProjectView");
                }
                else
                {
                    string errorContent = "An unknown error occurred while creating the project.";
                    try
                    {
                        errorContent = await apiResponse.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading API create project error content: {ex.Message}");
                    }
                    ModelState.AddModelError(string.Empty, $"Project creation failed: {apiResponse.StatusCode}. Details: {errorContent}");
                    return View(model);
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to the project creation service. Please ensure the API is running. Error: {httpEx.Message}");
                Console.WriteLine($"HttpRequestException in ProjectController.CreateProject: {httpEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred during project creation: {ex.Message}");
                Console.WriteLine($"General Exception in ProjectController.CreateProject: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Edit(int id) => View();    
        public IActionResult Details(int id) => View();

        [HttpGet]
        public IActionResult SingleProjectView()
        {
            return View(); 
        }


        public async Task<IActionResult> Dashboard(int projectId)
        {
            HttpContext.Session.SetString("ProjectId", projectId.ToString());
            string? token = HttpContext.Session.GetString("JwtToken");

            // Get overview data
            var response = await _projectServices.GetTaskCountByPIDAsync(token, projectId); 

            if (response == null)
            {
                return NotFound(); 
            }
            

            OverviewModel overview = new OverviewModel();
        
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                overview = JsonConvert.DeserializeObject<OverviewModel>(json);
            }

            var viewModel = new DashboardViewModel
            {
                Greeting = "Welcome to ProBuild",
                RevenueLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                RevenueValues = new List<int> { 20000, 40000, 50000, 60000, 70000, 80000 },
                RecentActivities = new List<RecentItem> {
            new RecentItem { Identifier = "TSK-2025-001", Description = "Foundation Work - R15,750.00", TimeAgo = "2 hours ago" }
        },

                // Set metrics if needed
                Metrics = new List<MetricCard>
        {
            new MetricCard { Title = "Total Project Revenue", Value = "R328,750" },
            new MetricCard { Title = "Materials Cost", Value = "R15,425" }
        },

                // Map Overview values here
                TotalTasks = overview.TotalTasks,
                CompletedTasks = overview.CompleteTasks,
                InProgressTasks = overview.InProgressTasks,
                InWaitingTasks = overview.InWaitingTasks,

                ProjectCount = overview.ProjectCount,
                CompleteProjects = overview.CompleteProjects,
                InProgressProjects = overview.InProgressProjects,
                InWaitingProjects = overview.InWaitingProjects,
                TotalBudget = overview.TotalBudget,
                ProjectName = overview.ProjectName,
                TasksByStatus = overview.TasksByStatus

            };

            return View(viewModel);
        }

    }
}