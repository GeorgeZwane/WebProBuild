using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Services;

namespace WebSpaceApp.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskService _taskServices;
        private readonly ProjectServices _projectServices;

        public TaskController(TaskService taskServices, ProjectServices projectServices)
        {
            _taskServices = taskServices;
            _projectServices = projectServices;
        }



[HttpGet]
[Route("api/webtask")]
public async Task<IActionResult> ViewTask(int? projectId)
{
            string? userRole = HttpContext.Session.GetString("UserRole");
            // Step 1: Handle missing projectId (try session fallback)
            if (!projectId.HasValue)
    {
        int? sessionProjectId = HttpContext.Session.GetInt32("ProjectId");
       
        if (!sessionProjectId.HasValue)
        {
            TempData["ErrorMessage"] = "Project ID is missing. Please select a project.";
            return RedirectToAction("ProjectView", "Project");
        }

        projectId = sessionProjectId;
    }

    int projectid = projectId.Value;

    // Step 2: Get user ID and token from session
    string? token = HttpContext.Session.GetString("JwtToken");
    string? userIdString = HttpContext.Session.GetString("UserId");

    if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
    {
        TempData["ErrorMessage"] = "You need to be logged in to view tasks.";
        return RedirectToAction("Login", "Account");
    }

    // Step 3: Initialize view model list
    List<ViewTaskModel> tasks = new List<ViewTaskModel>();

    try
    {
        // Step 4: Call service to get tasks for the project
        HttpResponseMessage apiResponse = await _taskServices.GetAllTaskByIDAsync(token, projectid);

        if (apiResponse.IsSuccessStatusCode)
        {
            string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
            var taskDtos = JsonConvert.DeserializeObject<List<TaskDTO>>(jsonResponse);

            if (taskDtos != null)
            {
                foreach (var dto in taskDtos)
                {
                    tasks.Add(new ViewTaskModel
                    {
                        Id = dto.Id,
                        TaskName = dto.TaskName,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate,
                        Progress = dto.Progress,
                        AssignedTo = dto.AssignedTo,
                        Priority = dto.Priority,
                        Description = dto.Description,
                        ProjectId = dto.ProjectId
                    });
                }
            }
        }
        else
        {
            string errorContent = await apiResponse.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Failed to load tasks: {apiResponse.StatusCode}. Details: {errorContent}";
            Console.WriteLine($"Error fetching tasks: {apiResponse.StatusCode} - {errorContent}");
        }
    }
    catch (HttpRequestException httpEx)
    {
        TempData["ErrorMessage"] = $"Could not connect to the task service. Please try again later. Error: {httpEx.Message}";
        Console.WriteLine($"HttpRequestException in TaskController.ViewTask: {httpEx.Message}");
    }
    catch (JsonException jsonEx)
    {
        TempData["ErrorMessage"] = $"Failed to parse task data. Please try again later. Error: {jsonEx.Message}";
        Console.WriteLine($"JsonException in TaskController.ViewTask: {jsonEx.Message}");
    }
    catch (Exception ex)
     {
        TempData["ErrorMessage"] = $"An unexpected error occurred while loading tasks: {ex.Message}";
        Console.WriteLine($"General Exception in TaskController.ViewTask: {ex.Message}");
     }

            // Step 5: Return the view with tasks
            ViewBag.UserRole = userRole ?? "Unknown";
            return View("ViewTask", tasks);
}

        // GET: /Task/CreateTask
        // Display the form to create a new task
        // GET: /Task/CreateTask
        // Display the form to create a new task
        [HttpGet]
        public async Task<IActionResult> CreateTask(int? projectId = null)
        {
            int? currentProjectId = projectId ?? HttpContext.Session.GetInt32("ProjectId");

            if (!currentProjectId.HasValue)
            {
                TempData["ErrorMessage"] = "Please create or select a project first before creating tasks.";
                return RedirectToAction("ProjectView", "Project");
            }

            string? token = HttpContext.Session.GetString("AccessToken");

            var foremenList = await _taskServices.GetUsersForDropdownAsync(token);

            // Debug: Log the foremen list
            Console.WriteLine($"Retrieved {foremenList.Count} foremen for dropdown:");
            foreach (var foreman in foremenList)
            {
                Console.WriteLine($"  ID: {foreman.Value}, Name: {foreman.Text}");
            }

            var model = new CreateTaskViewModel
            {
                ProjectId = currentProjectId.Value,
                Users = foremenList
            };

            ViewBag.ProjectId = currentProjectId.Value;

            return View(model);
        }



        // POST: /Task/CreateTask
        // POST: /Task/CreateTask
        // POST: /Task/CreateTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(CreateTaskViewModel model)
        {
            // Debug: Log everything received
            Console.WriteLine("=== DEBUGGING FORM SUBMISSION ===");
            Console.WriteLine($"TaskName: {model.TaskName}");
            Console.WriteLine($"Description: {model.Description}");
            Console.WriteLine($"ProjectId: {model.ProjectId}");
            Console.WriteLine($"AssignedTo: {model.AssignedTo}");
            Console.WriteLine($"Priority: {model.Priority}");

            // Debug: Log all form values from Request.Form
            Console.WriteLine("\n=== RAW FORM VALUES ===");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"{key}: {Request.Form[key]}");
            }

            // Debug: Check model state
            Console.WriteLine("\n=== MODEL STATE ===");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Key: {kvp.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            // For now, let's skip validation and just try to create the task
            // We'll add validation back once we confirm the basic flow works

            string? jwtToken = HttpContext.Session.GetString("JwtToken");

            if (model.ProjectId <= 0)
            {
                string? projectIdString = HttpContext.Session.GetString("ProjectId");
                if (!string.IsNullOrEmpty(projectIdString) && int.TryParse(projectIdString, out int sessionProjectId))
                {
                    model.ProjectId = sessionProjectId;
                }
                else
                {
                    TempData["ErrorMessage"] = "Project ID is required to create a task.";
                    return RedirectToAction("CreateProject", "Project");
                }
            }

            // Debug: Check if AssignedTo has a valid value
            Console.WriteLine($"\n=== FINAL VALUES BEFORE API CALL ===");
            Console.WriteLine($"Final AssignedTo value: {model.AssignedTo}");

            // Create DTO regardless of validation for debugging
            var taskDto = new AddTaskDTO
            {
                TaskName = model.TaskName ?? "Test Task",
                Description = model.Description ?? "Test Description",
                Startdate = model.Startdate != default ? model.Startdate : DateTime.Now,
                Enddate = model.Enddate != default ? model.Enddate : DateTime.Now.AddDays(1),
                Priority = model.Priority ?? "Low",
                ProjectId = model.ProjectId,
                AssignedTo = model.AssignedTo.ToString() // Convert int to string here
            };

            Console.WriteLine($"TaskDTO AssignedTo: {taskDto.AssignedTo}");

            try
            {
                HttpResponseMessage apiResponse = await _taskServices.CreateTaskAsync(taskDto, jwtToken);

                if (apiResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Task created successfully!";
                    return RedirectToAction("ViewTask", "Task");
                }
                else
                {
                    string errorContent = await apiResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {apiResponse.StatusCode} - {errorContent}");
                    TempData["ErrorMessage"] = $"Task creation failed: {apiResponse.StatusCode}. Details: {errorContent}";

                    // Reload dropdown on error
                    string? token = HttpContext.Session.GetString("AccessToken");
                    model.Users = await _taskServices.GetUsersForDropdownAsync(token);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";

                // Reload dropdown on error
                string? token = HttpContext.Session.GetString("AccessToken");
                model.Users = await _taskServices.GetUsersForDropdownAsync(token);
                return View(model);
            }
        }

        public async Task<IActionResult> TaskReport()
        {
           // HttpContext.Session.SetString("ProjectId");
            string? token = HttpContext.Session.GetString("JwtToken");

            // Get overview data
            var response = await _projectServices.GetProjectAndTaskCountsAsync(token);
            OverviewModel overview = new OverviewModel();
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                overview = JsonConvert.DeserializeObject<OverviewModel>(json);
            }


            // Step 2: Map the overview values into TaskReportViewModel
            var model = new TaskReportViewModel
            {
                Metrics = new List<TaskMetric>
        {
            new TaskMetric { Title = "Total Tasks", Value = overview.TotalTasks.ToString(), IsOverdue = false },
            new TaskMetric { Title = "Completed", Value = overview.CompleteTasks.ToString(), Note = GetPercentage(overview.CompleteTasks, overview.TotalTasks), IsOverdue = false },
            new TaskMetric { Title = "Overdue", Value = "5", IsOverdue = true }, // <-- Replace with real value later
            new TaskMetric { Title = "In Progress", Value = overview.InProgressTasks.ToString(), IsOverdue = false }
        },

                OverdueTasks = new List<TaskDetail>
        {
            new TaskDetail { TaskName = "Finalize design mockups", AssignedTo = "Jane Doe", DueDate = DateTime.Parse("2025-07-28"), OverdueBy = "8 days" },
            new TaskDetail { TaskName = "Implement user authentication", AssignedTo = "John Smith", DueDate = DateTime.Parse("2025-07-30"), OverdueBy = "6 days" },
            new TaskDetail { TaskName = "Write API documentation", AssignedTo = "Mike Johnson", DueDate = DateTime.Parse("2025-08-01"), OverdueBy = "4 days" },
            new TaskDetail { TaskName = "Perform load testing", AssignedTo = "Jane Doe", DueDate = DateTime.Parse("2025-08-04"), OverdueBy = "1 day" },
            new TaskDetail { TaskName = "Database schema review", AssignedTo = "Sarah Lee", DueDate = DateTime.Parse("2025-07-25"), OverdueBy = "11 days" }
        },

                ChartLabels = new List<string> { "Completed", "In Progress", "Overdue", "Not Started" },
                ChartValues = new List<int>
        {
                    overview.TotalTasks,
            overview.CompleteTasks,
            overview.InProgressTasks,
            5, // Placeholder for Overdue count — replace when dynamic
            overview.InWaitingTasks
        },

                TotalTasks = overview.TotalTasks,
                CompleteTasks = overview.CompleteTasks,
                InProgressTasks = overview.InProgressTasks,
                IncompleteTasks = overview.InWaitingTasks
            };

            return View(model);
        }

        // Optional helper for calculating % as a string
        private string GetPercentage(int part, int total)
        {
            if (total == 0) return "0%";
            return ((double)part / total * 100).ToString("0.#") + "%";
        }

    }
}