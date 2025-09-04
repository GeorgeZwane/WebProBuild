using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebSpaceApp.Attributes;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceApp.Services;
using WebSpaceInnovatorsWebApp.Services;

namespace WebSpaceApp.Controllers
{
    public class MaterialInventoryController : Controller
    {
        private readonly MaterialService _projectServices;

        public MaterialInventoryController(MaterialService projectServices)
        {
            _projectServices = projectServices;
        }

        [HttpGet]
        public IActionResult AddMaterials(int? id = null, int? projectId = null) 
        {
           
            Console.WriteLine($"AddMaterials GET called with id: {id}, projectId: {projectId}");
            int? currentProjectId = id ?? projectId;
            if (!currentProjectId.HasValue)
            {
                string? projectIdString = HttpContext.Session.GetString("ProjectId");
                if (!string.IsNullOrEmpty(projectIdString) && int.TryParse(projectIdString, out int sessionProjectId))
                {
                    currentProjectId = sessionProjectId;
                }
                Console.WriteLine($"ProjectId from session: {currentProjectId}");
            }

            if (!currentProjectId.HasValue)
            {
                Console.WriteLine("No ProjectId found, redirecting to ProjectView");
                TempData["ErrorMessage"] = "Please select a project first before adding materials.";
                return RedirectToAction("ProjectView", "Project");
            }

            var model = new MaterialItemModel
            {
                ProjectId = currentProjectId.Value
            };
            ViewBag.ProjectId = currentProjectId.Value;

            Console.WriteLine($"Successfully created MaterialItemModel with ProjectId: {currentProjectId.Value}");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MaterialInventory()
        {
            string? token = HttpContext.Session.GetString("JwtToken");
            List<MaterialItemModel> viewModels = new List<MaterialItemModel>();

            Console.WriteLine("Starting MaterialInventory method");

            try
            {
                HttpResponseMessage response = await _projectServices.GetAllMaterialsAsync(token);
                Console.WriteLine($"API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Raw API Response: {content}");
                    var materialEntities = JsonConvert.DeserializeObject<List<MaterialDTO>>(content);

                    if (materialEntities != null && materialEntities.Any())
                    {
                        Console.WriteLine($"Found {materialEntities.Count} materials");

                        viewModels = materialEntities.Select(entity => new MaterialItemModel
                        {
                            ProjectId = entity.ProjectId,
                            Name = entity.Name,
                            Quantity = entity.Quantity,
                            MetricUnit = entity.MetricUnit
                        }).ToList();

                        Console.WriteLine($"Mapped {viewModels.Count} materials to view models");
                    }
                    else
                    {
                        Console.WriteLine("No materials found or deserialization failed");
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to load materials: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"API Error fetching materials: {response.StatusCode} - {errorContent}");
                }
            }
            catch (JsonException jsonEx)
            {
                TempData["ErrorMessage"] = $"Failed to parse materials data from API. Error: {jsonEx.Message}";
                Console.WriteLine($"JsonException in MaterialController.MaterialInventory: {jsonEx.Message}");
                Console.WriteLine($"JsonException StackTrace: {jsonEx.StackTrace}");
            }
            catch (HttpRequestException httpEx)
            {
                TempData["ErrorMessage"] = $"Could not connect to the material service. Please ensure the API is running. Error: {httpEx.Message}";
                Console.WriteLine($"HttpRequestException in MaterialController.MaterialInventory: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred while loading materials: {ex.Message}";
                Console.WriteLine($"General Exception in MaterialController.MaterialInventory: {ex.Message}");
                Console.WriteLine($"Exception StackTrace: {ex.StackTrace}");
            }

            Console.WriteLine($"Returning view with {viewModels?.Count ?? 0} materials");
            return View(viewModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMaterials(MaterialItemModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            if (model.ProjectId <= 0)
            {
               
                string? projId = HttpContext.Session.GetString("ProjectId");
                if (!string.IsNullOrEmpty(projId) && int.TryParse(projId, out int pId))
                {
                    model.ProjectId = pId;
                }
                else
                {
                    TempData["ErrorMessage"] = "Project ID is required to add materials.";
                    return RedirectToAction("ProjectView", "Project");
                }
            }

            
            var materialDto = new AddMaterialsProjectDTO  
            {
                Name = model.Name,
                Quantity = model.Quantity,
                MetricUnit = model.MetricUnit,
                ProjectId = model.ProjectId
            };

            try
            {
                string? token = HttpContext.Session.GetString("JwtToken");
                HttpResponseMessage apiResponse = await _projectServices.CreateMaterialAsync(materialDto, token);
                string jsonResponse = await apiResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Material added successfully!";
                    return RedirectToAction("MaterialInventory", "MaterialInventory");
                }
                else
                {
                    string errorContent = jsonResponse ?? "An unknown error occurred.";
                    ModelState.AddModelError(string.Empty, $"Material creation failed: {apiResponse.StatusCode}. Details: {errorContent}");
                    return View(model);
                }
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to the material creation service. Error: {httpEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
                return View(model);
            }
        }

    }

}
