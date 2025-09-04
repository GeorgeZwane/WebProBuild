using Microsoft.AspNetCore.Mvc;
using WebSpaceApp.DTOs;
using WebSpaceApp.Models;
using WebSpaceApp.Services;
using Newtonsoft.Json;
using System.Net.Http;

namespace WebSpaceApp.Controllers
{
    public class MilestoneController : Controller
    {
        private readonly MilestoneService _milestoneServices;

        public MilestoneController(MilestoneService milestoneServices)
        {
            _milestoneServices = milestoneServices;
        }

        // Action to display all milestones for a given task.
        // It now takes a taskId to fetch the correct data from the API.
        [HttpGet]
        public async Task<IActionResult> MilestoneView(Guid Id)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole ?? "Unknown";
            // Call the service to get milestones for the specified taskId from the API.
            try
            {
                // This assumes your MilestoneService has a method to fetch milestones by taskId.
                // If not, you will need to add it there as well.
                var milestones = await _milestoneServices.GetMilestonesAsync(Id);

                if (milestones == null)
                {
                    // Handle the case where the API returns no data or a 404.
                    ViewBag.ErrorMessage = "No milestones found for this task.";
                    return View(new List<MilestoneViewModel>());
                }

                // Pass the retrieved list of milestones to the view.
                return View(milestones);
            }
            catch (HttpRequestException)
            {
                // Handle API errors, like the API server being down or a network issue.
                ViewBag.ErrorMessage = "Failed to load milestones. Please try again later.";
                return View(new List<MilestoneViewModel>());
            }
        }

        // This is your existing action for adding a milestone, which you should keep.
        [HttpGet]
        public IActionResult AddMilestone(Guid taskId)
        {
            var model = new MilestoneModel { TaskEntityId = taskId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MilestoneModel model)
        {
            if (model.TaskEntityId == Guid.Empty)
            {
                ModelState.AddModelError("", "Invalid task ID. Please try again from the task view.");
                return View("AddMilestone", model);
            }

            if (ModelState.IsValid)
            {
                var dto = new MilestoneDTO
                {
                    MilestoneName = model.MilestoneName,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    TaskEntityId = model.TaskEntityId
                };

                var response = await _milestoneServices.AddMilestoneAsync(dto);

                if (response.IsSuccessStatusCode)
                {
                   // FIXED: Use 'Id' instead of 'taskId' to match the MilestoneView parameter
                     return RedirectToAction("MilestoneView", "Milestone", new { Id = model.TaskEntityId });
                    
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create milestone.");
                }
            }
            return View("AddMilestone", model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateMilestone(Guid id)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
           
            try
            {
                var milestone = await _milestoneServices.GetMilestoneByIdAsync(id);

                if (milestone == null)
                {
                    ViewBag.ErrorMessage = "Milestone not found.";
                    return RedirectToAction("Error");
                }

                var model = new MilestoneUpdate
                {
                    Id = milestone.Id,
                    MilestoneName = milestone.MilestoneName,
                    Status = milestone.Status,
                    Reason = milestone.Reason,
                    TaskEntityId = milestone.TaskEntityId 
                };
                ViewBag.UserRole = userRole ?? "Unknown";
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error fetching milestone details: {ex.Message}";
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(MilestoneUpdate model)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateMilestone", model);
            }

            try
            {
                var updateDto = new UpdateMilestoneDTO
                {
                    Id = model.Id,
                    Status = model.Status,
                    Reason = model.Reason
                };

                var response = await _milestoneServices.UpdateMilestoneAsync(updateDto);

                if (response.IsSuccessStatusCode)
                {
                    // 🔁 Fetch the milestone again to get the correct TaskEntityId
                    var updatedMilestone = await _milestoneServices.GetMilestoneByIdAsync(model.Id);

                    if (updatedMilestone == null || updatedMilestone.TaskEntityId == Guid.Empty)
                    {
                        return RedirectToAction("Error");
                    }

                    return RedirectToAction("MilestoneView", "Milestone", new { Id = updatedMilestone.TaskEntityId });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update milestone.");
                    return View("UpdateMilestone", model);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"API error: {ex.Message}");
                return View("UpdateMilestone", model);
            }
        }

    }


}
