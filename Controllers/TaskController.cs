using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;
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
                            await _projectServices.UpdateTaskAsync(token, dto.ProjectId);
                            Console.WriteLine($"Updating task using {dto.ProjectId}");
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
    return View("ViewTask", tasks);
}

        [HttpGet]
        public async Task<IActionResult> ViewTaskMilestone(int? userid, string? progressFilter)
        {

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
                HttpResponseMessage apiResponse = await _taskServices.GetAllTaskByUserIDAsync(token, userId);

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

                        if (!string.IsNullOrEmpty(progressFilter))
                        {
                            var parts = progressFilter.Split('-');
                            if (parts.Length == 2 &&
                                double.TryParse(parts[0], out double min) &&
                                double.TryParse(parts[1], out double max))
                            {
                                tasks = tasks.Where(p => (p.Progress ?? 0) >= min && (p.Progress ?? 0) <= max).ToList();
                            }
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
            return View("ViewTaskMilestone", tasks);
        }

        [HttpGet]
        public async Task<IActionResult> ViewForeManProgress()
        {
            string? token = HttpContext.Session.GetString("JwtToken");
            string? userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to view tasks.";
                return RedirectToAction("Login", "Account");
            }

            HttpResponseMessage apiResponse = await _taskServices.GetAllTaskByUserIDAsync(token, userId);
            List<TaskDTO> tasks = new();

            if (apiResponse.IsSuccessStatusCode)
            {
                string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                tasks = JsonConvert.DeserializeObject<List<TaskDTO>>(jsonResponse) ?? new List<TaskDTO>();
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to load your tasks.";
                return RedirectToAction("Dashboard", "Home");
            }

            // Treat each task as a milestone
            int total = tasks.Count;
            int completed = tasks.Count(t => t.Progress == 100);
            int pending = total - completed;
            double completionRate = total == 0 ? 0 : (double)completed / total * 100;

            string performanceMessage;
            string performanceLevel;

            if (completionRate >= 80)
            {
                performanceMessage = "Excellent work! You're crushing it! 🚀";
                performanceLevel = "high";
            }
            else if (completionRate >= 50)
            {
                performanceMessage = "You're doing good, but there's room to push harder! 💪";
                performanceLevel = "medium";
            }
            else
            {
                performanceMessage = "Let's step it up! You’ve got what it takes. 👊";
                performanceLevel = "low";
            }

            var viewModel = new ForemanProgressViewModel
            {
                TotalMilestones = total,
                CompletedMilestones = completed,
                PendingMilestones = pending,
                CompletionRate = completionRate,
                Message = performanceMessage,
                PerformanceLevel = performanceLevel
            };

            return View(viewModel);
        }
       
        [HttpGet]
        public async Task<IActionResult> LeaderboardView()
        {
            string? token = HttpContext.Session.GetString("JwtToken");

            HttpResponseMessage response = await _taskServices.GetLeaderboardAsync(token);
            List<LeaderboardViewModel> leaderboard = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                leaderboard = JsonConvert.DeserializeObject<List<LeaderboardViewModel>>(json) ?? new List<LeaderboardViewModel>();
            }
            else
            {
                TempData["ErrorMessage"] = "Could not load leaderboard data.";
            }

            return View(leaderboard);
        }

        [HttpGet]
        public async Task<IActionResult> LeaderboardViewAnalysis()
        {
            string? token = HttpContext.Session.GetString("JwtToken");

            HttpResponseMessage response = await _taskServices.GetLeaderboardAsync(token);
            List<LeaderboardViewModel> leaderboard = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                leaderboard = JsonConvert.DeserializeObject<List<LeaderboardViewModel>>(json) ?? new List<LeaderboardViewModel>();
            }
            else
            {
                TempData["ErrorMessage"] = "Could not load leaderboard data.";
            }

            return View(leaderboard);
        }


        [HttpGet]
      

        [HttpGet]
        public async Task<IActionResult> NotificationView()
        {
            string? token = HttpContext.Session.GetString("JwtToken");
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login", "Account");

            HttpResponseMessage response = await _taskServices.GetNotificationsAsync(token, userId);
            List<NotificationViewModel> notifications = new();

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                notifications = JsonConvert.DeserializeObject<List<NotificationViewModel>>(json) ?? new();
            }

            int unread = notifications.Count(n => !n.IsRead);
            ViewBag.UnreadCount = unread;

            return View(notifications);
        }

      

        [HttpGet]
        public async Task<IActionResult> SendNotificationView()
        {
            string? token = HttpContext.Session.GetString("JwtToken");

            HttpResponseMessage response = await _taskServices.GetAllForemenAsync(token);
            List<UserDto> foremen = new();

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                foremen = JsonConvert.DeserializeObject<List<UserDto>>(json) ?? new();
            }

            var viewModel = new SendNotificationViewModel
            {
                Foreman = foremen.Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(SendNotificationViewModel model)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (!ModelState.IsValid)
            {
                // Step 1: Call API to get HttpResponseMessage
                HttpResponseMessage response = await _taskServices.GetAllForemenAsync(token);

                List<UserDto> foremen = new List<UserDto>();

                // Step 2: Deserialize JSON content
                if (response.IsSuccessStatusCode )
                {
                    string json = await response.Content.ReadAsStringAsync();
                    foremen = JsonConvert.DeserializeObject<List<UserDto>>(json) ?? new List<UserDto>();
                }

                // Step 3: Map to SelectListItem
                model.Foreman = foremen.Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                }).ToList();

               
                return View(model);
            }
            int.TryParse(userIdStr, out int userId);
            // Prepare DTO for sending notification
            var dto = new SendNotificationDTO
            {
                SenderId = userId,
                RecipientId = model.RecipientId,
                Message = model.Message
            };

            // Send notification
            var sendResponse = await _taskServices.SendNotificationAsync(token, dto);

            if (sendResponse.IsSuccessStatusCode)
            {
                TempData["Success"] = "Notification sent successfully!";
                return RedirectToAction("SendNotification");
            }

            string errorDetails = await sendResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"SendNotification failed. Status: {sendResponse.StatusCode}, Details: {errorDetails}");

            TempData["Error"] = $"Failed to send message to {model.RecipientId} by {userId}. " +
                    $"Status: {sendResponse.StatusCode}, Reason: {sendResponse.ReasonPhrase}, " +
                    $"Details: {errorDetails}";

            // Reload foremen list if sending fails and return view
            HttpResponseMessage retryResponse = await _taskServices.GetAllForemenAsync(token);
            List<UserDto> retryForemen = new List<UserDto>();
            if (retryResponse.IsSuccessStatusCode)
            {
                string retryJson = await retryResponse.Content.ReadAsStringAsync();
                retryForemen = JsonConvert.DeserializeObject<List<UserDto>>(retryJson) ?? new List<UserDto>();
            }
            model.Foreman = retryForemen.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name
            }).ToList();

            return View(model);
        }

        // GET: /Task/CreateTask
        // Display the form to create a new task
        [HttpGet]
        public async Task<IActionResult> CreateTask(int? projectId = null)
        {
            // Plan A: From Route
            int? currentProjectId = projectId;

            // Plan B: From Session
            if (!currentProjectId.HasValue)
            {
                string? projectIdString = HttpContext.Session.GetString("ProjectId");
                if (!string.IsNullOrEmpty(projectIdString) && int.TryParse(projectIdString, out int sessionProjectId))
                {
                    currentProjectId = sessionProjectId;
                }
            }

            // Plan C: Redirect if no project ID
            if (!currentProjectId.HasValue)
            {
                TempData["ErrorMessage"] = "Please create or select a project first before creating tasks.";
                return RedirectToAction("ProjectView", "Project");
            }

           
            string? token = HttpContext.Session.GetString("JwtToken");
            //if (string.IsNullOrEmpty(token))
            //{
            //    TempData["ErrorMessage"] = "You are not authorized.";
            //    return RedirectToAction("Login", "Account"); // Or wherever you handle auth
            //}

            // 👇 Call your service to fetch users
            var userDropdown = await _taskServices.GetUsersForDropdownAsync(token);

            // ✅ Build ViewModel
            var model = new CreateTaskViewModel
            {
                ProjectId = currentProjectId.Value,
                Users = userDropdown
            };

            ViewBag.ProjectId = currentProjectId.Value;

            return View(model);
        }


        // POST: /Task/CreateTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(CreateTaskViewModel model)
        {

            if (!ModelState.IsValid)
            {
                
                var users = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Jane Doe" },
                    new SelectListItem { Value = "2", Text = "John Smith" },
                    new SelectListItem { Value = "3", Text = "Mike Johnson" },
                    new SelectListItem { Value = "4", Text = "Sarah Lee" }
                };
                model.Users = users;
                return View(model);
            }


            string? token = HttpContext.Session.GetString("JwtToken");
            string? userIdString = HttpContext.Session.GetString("UserId");

            //if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            //{
            //    TempData["ErrorMessage"] = "You need to be logged in to create a task.";
            //    return RedirectToAction("Login", "Account");
            //}


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
            

            var taskDto = new AddTaskDTO
            {
                TaskName = model.TaskName,
                Description = model.Description,
                Startdate = model.Startdate,
                Enddate = model.Enddate,
                Priority = model.Priority,
                ProjectId = model.ProjectId,
                UserId = model.UserId,
            };

            try
            {

                HttpResponseMessage apiResponse = await _taskServices.CreateTaskAsync(taskDto, token);

                if (apiResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Task created successfully!";
                    return RedirectToAction("ViewTask", "Task");
                }
                else
                {
                    string errorContent = await apiResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Task creation failed: {apiResponse.StatusCode}. Details: {errorContent}");

                   
                    var users = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "Mabuza Junior" },
                        new SelectListItem { Value = "2", Text = "Maite Selowa" },
                        new SelectListItem { Value = "3", Text = "George Zwane" },
                        new SelectListItem { Value = "4", Text = "L Phiri" }
                    };
                    model.Users = users;

                    return View(model);
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to the task creation service. Error: {httpEx.Message}");

               
                var users = new List<SelectListItem>
                {
                        new SelectListItem { Value = "1", Text = "Mabuza Junior" },
                        new SelectListItem { Value = "2", Text = "Maite Selowa" },
                        new SelectListItem { Value = "3", Text = "George Zwane" },
                        new SelectListItem { Value = "4", Text = "L Phiri" }
                };
                model.Users = users;

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");

               
                var users = new List<SelectListItem>
                {
                        new SelectListItem { Value = "1", Text = "Mabuza Junior" },
                        new SelectListItem { Value = "2", Text = "Maite Selowa" },
                        new SelectListItem { Value = "3", Text = "George Zwane" },
                        new SelectListItem { Value = "4", Text = "L Phiri" }
                };
                model.Users = users;

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