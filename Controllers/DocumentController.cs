using Microsoft.AspNetCore.Mvc;
using WebSpaceApp.Models;

namespace WebSpaceApp.Controllers
{
    public class DocumentController : Controller
    {
        [HttpGet]
        public ActionResult DocumentUpload()
        {
            //TODO: Implementing logic to retrieve task documents from the database
            var documents = new List<TaskDocument>
        {
            new TaskDocument { Id = 1, TaskName = "Task 1", Title = "Final Architectural Plan.pdf", FileType = "pdf" },
            new TaskDocument { Id = 2, TaskName = "Task 2", Title = "User Authentication Specs.docx", FileType = "docx" }
        };

            return View(documents);
        }

        [HttpPost]
        public ActionResult Upload(TaskDocument doc)
        {
            // TODO: Handling file upload logic 
            // TODO: Save to database 
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult DocumentView()
        {
            var documents = new List<TaskDocument>
        {
            new TaskDocument { Id = 1, Title = "Final Architectural Plan.pdf", TaskName = "Task 1", FileType = "pdf" },
            new TaskDocument { Id = 2, Title = "User Authentication Specs.docx", TaskName = "Task 2", FileType = "word" }
        };

            var updates = new List<DocumentUpdate>
        {
            new DocumentUpdate {
                Id = 1,
                Author = "Jane Doe",
                Date = new DateTime(2024, 8, 7),
                DocumentTitle = "Final Architectural Plan.pdf",
                Comment = "Final review of the plan is complete. Please review for sign-off."
            },
            new DocumentUpdate {
                Id = 2,
                Author = "John Smith",
                Date = new DateTime(2024, 8, 6),
                DocumentTitle = "User Authentication Specs.docx",
                Comment = "Added new specifications for two-factor authentication based on client feedback."
            }
        };

            ViewBag.Updates = updates;
            return View(documents);
        }
    }
}
