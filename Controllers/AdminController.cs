using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceInnovatorsWebApp.Services;

namespace WebSpaceApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProjectServices _projectServices;

        public AdminController(ProjectServices projectServices)
        {
            _projectServices = projectServices;
        }
        public async Task<IActionResult> AdminDashboard()
        {
            string? token = HttpContext.Session.GetString("JwtToken");
            string? userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to view projects.";
                return RedirectToAction("Login", "Account");
            }

            List<UserViewModel> users = new List<UserViewModel>();
            try
            {
                HttpResponseMessage apiResponse = await _projectServices.GetAllUsersAsync(userId, token);

                if (apiResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var projectDtos = JsonConvert.DeserializeObject<List<RegisterDto>>(jsonResponse);

                    if (projectDtos != null)
                    {
                        foreach (var dto in projectDtos)
                        {
                            users.Add(new UserViewModel
                            {

                                UserName = dto.Name,
                                UserSurname = dto.Surname,
                                Email = dto.Email,
                                Address = dto.Address,
                                Role = dto.UserRole

                            });
                        }
                    }
                }
                else
                {
                    string errorContent = await apiResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to load users: {apiResponse.StatusCode}. Details: {errorContent}";
                    Console.WriteLine($"Error fetching users: {apiResponse.StatusCode} - {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["ErrorMessage"] = $"Could not connect to the user service. Please try again later. Error: {httpEx.Message}";
                Console.WriteLine($"HttpRequestException in userController.userView: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                TempData["ErrorMessage"] = $"Failed to parse user data. Please try again later. Error: {jsonEx.Message}";
                Console.WriteLine($"JsonException in userController.userView: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred while loading user: {ex.Message}";
                Console.WriteLine($"General Exception in userController.userView: {ex.Message}");
            }


            return View("AdminDashboard", users);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserRole(AssignRoleDto dto)
        {
            // input check
            if (dto.UserId <= 0 || string.IsNullOrWhiteSpace(dto.NewRole))
            {
                TempData["ErrorMessage"] = "User ID and role are required.";
                return RedirectToAction("AdminDashboard", "Admin");
            }

            var assignRoleDto = new AssignRoleDto
            {
                UserId = dto.UserId,
                NewRole = dto.NewRole   
            };

            try
            {
                string? token = HttpContext.Session.GetString("JwtToken");

                HttpResponseMessage apiResponse = await _projectServices.AssignUserRoleAsync(assignRoleDto, token);

                if (apiResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Role '{dto.NewRole}' successfully assigned to user ID {dto.UserId}.";
                    Console.WriteLine($"Role '{dto.NewRole}' successfully assigned to user ID {dto.UserId}.");
                    return RedirectToAction("AdminDashboard", "Admin");

                }
                else
                {
                    string errorContent = "An error occurred assigning the role.";
                    try
                    {
                        errorContent = await apiResponse.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading API response content: {ex.Message}");
                    }

                    TempData["ErrorMessage"] = $"Assignment failed: {apiResponse.StatusCode}. Details: {errorContent}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                TempData["ErrorMessage"] = $"Connection error while assigning role: {httpEx.Message}";
                Console.WriteLine($"HttpRequestException in AdminController.AssignUserRole: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unexpected error assigning role: {ex.Message}";
                Console.WriteLine($"General Exception in AdminController.AssignUserRole: {ex.Message}");
            }

            return RedirectToAction("AdminDashboard", "Admin");
        }

        [HttpPost]
        public IActionResult DeleteUser(int UserId)
        {
            // TODO: Deleting user by UserId
            return RedirectToAction("Index");
        }
    }
}
