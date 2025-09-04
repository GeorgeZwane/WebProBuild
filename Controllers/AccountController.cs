using Microsoft.AspNetCore.Mvc;
using System; 
using System.Text;
using System.Threading.Tasks; 
using WebSpaceApp.DTOs; 
using WebSpaceApp.Models; 
using WebSpaceInnovatorsWebApp.DTOs; 
using WebSpaceInnovatorsWebApp.Services; 
using Newtonsoft.Json; 
using Microsoft.AspNetCore.Http; 

namespace WebSpaceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ProjectServices _projectServices;

        public AccountController(ProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerDto = new RegisterDto
            {
                Name = model.Name,
                Surname = model.Surname,
                Password = model.Password,
                Email = model.Email,
                Address = model.Address,
                Contact = model.ContactNumber,
                UserRole = model.Role
            };

            try
            {
                HttpResponseMessage apiResponse = await _projectServices.RegisterAsync(registerDto);

                if (apiResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    string errorContent = "An unknown error occurred during registration.";
                    try
                    {
                        errorContent = await apiResponse.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading API response content: {ex.Message}");
                    }
                    ModelState.AddModelError(string.Empty, $"Registration failed: {apiResponse.StatusCode}. Details: {errorContent}");
                    return View(model);
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to the registration service. Please ensure the API is running. Error: {httpEx.Message}");
                Console.WriteLine($"HttpRequestException in AccountController.Register: {httpEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"General Exception in AccountController.Register: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            Response.Headers.Append("Pragma", "no-cache");
            Response.Headers.Append("Expires", "0");

            TempData["SuccessMessage"] = "You have been successfully logged out.";
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginRequestDto = new LoginRequestDto
            {
                Email = model.Email,
                Password = model.Password
            };

            try
            {
                HttpResponseMessage apiResponse = await _projectServices.LoginAsync(loginRequestDto);

                if (apiResponse.IsSuccessStatusCode)
                {
                   
                    string jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(jsonResponse);

                    if (loginResponse != null)
                    {
                        //HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                        TempData["SuccessMessage"] = "Login successful!";
                        HttpContext.Session.SetString("UserId", loginResponse.UserId.ToString());
                        HttpContext.Session.SetString("UserRole", loginResponse.UserRole.ToString());
                        HttpContext.Session.SetString("UserName", loginResponse.UserName.ToString());
                        //HttpContext.Session.SetString("JwtToken", loginResponse.Token.ToString());
                        //Console.WriteLine($"JWT Token stored in session: {loginResponse.Token.Substring(0, Math.Min(loginResponse.Token.Length, 30))}..."); // Log a partial token for verification
                    }
                    else
                    {
                        
                        ModelState.AddModelError(string.Empty, "Login successful, but no token received. Please contact support.");
                        Console.WriteLine("API returned success status but no JWT token found in response.");
                        return View(model);
                    }

                    switch (loginResponse.UserRole)
                    {
                        case "Admin":
                            return RedirectToAction("AdminDashboard", "Admin");
                        case "Project Manager":
                            return RedirectToAction("ProjectView", "Project");
                        case "Director":
                            return RedirectToAction("ProjectOverview", "ProjectOverview");
                        case "Foreman":
                            return RedirectToAction("ViewTaskMilestone", "Task");

                        default:
                            return RedirectToAction("Index", "Home");
                    }

                   
                }
                else
                {
                    string errorContent = "Invalid login credentials.";
                    try
                    {
                        errorContent = await apiResponse.Content.ReadAsStringAsync();
                      
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading API login response content: {ex.Message}");
                    }

                    ModelState.AddModelError(string.Empty, $"Login failed: {apiResponse.StatusCode}. Details: {errorContent}");
                    return View(model);
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to the login service. Please try again later. Error: {httpEx.Message}");
                Console.WriteLine($"HttpRequestException in AccountController.Login: {httpEx.Message}");
                return View(model);
            }
            catch (JsonException jsonEx) 
            {
                ModelState.AddModelError(string.Empty, $"Failed to parse login response. Please contact support. Error: {jsonEx.Message}");
                Console.WriteLine($"JsonException during login response deserialization: {jsonEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred during login: {ex.Message}");
                Console.WriteLine($"General Exception in AccountController.Login: {ex.Message}");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult UpdateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUser(RegisterUpdate model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save user to database or service
                TempData["SuccessMessage"] = "User registered successfully!";
                return RedirectToAction("Register");
            }

            return View(model);
        }
    }
}