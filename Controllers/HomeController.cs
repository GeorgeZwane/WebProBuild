using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebSpaceApp.Models; 
using WebSpaceInnovatorsWebApp.DTOs; 
using WebSpaceInnovatorsWebApp.Services; 
using System.Threading.Tasks;
using System; 
using Microsoft.Extensions.Logging;

namespace WebSpaceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjectServices _projectServices; 

       
        public HomeController(ILogger<HomeController> logger, ProjectServices projectServices)
        {
            _logger = logger;
            _projectServices = projectServices; 
        }

        
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        public IActionResult Home()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Home(LoginViewModel model) 
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

                    //good request API status is 200
                    TempData["SuccessMessage"] = "Login successful!";
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);
                    return RedirectToAction("Home", "Home"); //this code does not execute to debug later
                }
                else
                {
                    //Bad request API status is 400
                    string errorContent = "Invalid login credentials. Please try again."; 
                    try
                    {
                        errorContent = await apiResponse.Content.ReadAsStringAsync();
                       
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to read API login error content for user {Email}.", model.Email);
                     
                    }

                    ModelState.AddModelError(string.Empty, $"Login failed: {apiResponse.StatusCode}. Details: {errorContent}");
                    _logger.LogWarning("Login attempt failed for user {Email} with status {StatusCode}.", model.Email, apiResponse.StatusCode);
                    return View(model); 
                }
            }
            catch (HttpRequestException httpEx)
            {
               //Checking API connection
                ModelState.AddModelError(string.Empty, $"Could not connect to the login service. Please try again later. Error: {httpEx.Message}");
                _logger.LogError(httpEx, "HttpRequestException during login for user {Email}. API connection failed.", model.Email);
                return View(model);
            }
            catch (Exception ex)
            {
                // Catching any General exception that i have not specifically handled 
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred during login: {ex.Message}");
                _logger.LogError(ex, "Unexpected error during login for user {Email}.", model.Email);
                return View(model);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}