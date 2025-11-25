using COMP2139_ICE.Models;
using COMP2139_ICE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_ICE.Areas.ProjectManagement.Controllers
{
    [Area("ProjectManagement")]
    [Route("[area]/[controller]/[action]")]
    public class ProjectTaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectTaskController(ApplicationDbContext context) : base()
        {
            _context = context;
        }

        // GET: Tasks
        [HttpGet("Index/{projectId:int}")] //Lab6-Part2#1
        public async Task<IActionResult> Index(int projectId) //Lab9
        {
            var tasks = await _context.ProjectTasks //Lab9
                .Where(t => t.ProjectId == projectId)
                .ToListAsync(); //Lab9

            ViewBag.ProjectId = projectId; // Store projectId in ViewBag

            return View(tasks);
        }

        // GET: Tasks/Details/5
        [HttpGet("Details/{id:int}")] //Lab6-Part2#1
        public async Task<IActionResult> Details(int id) //Lab9
        {
            var task = await _context.ProjectTasks //Lab9
                .Include(t => t.Project) // Include related project data
                .FirstOrDefaultAsync(t => t.ProjectTaskId == id); //Lab9

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpGet("Create/{projectId:int}")] //Lab6-Part2#1
        public async Task<IActionResult> Create(int projectId) //Lab9
        {
            var project = await _context.Projects.FindAsync(projectId); //Lab9
            if (project == null)
            {
                return NotFound(); // Or handle appropriately if project doesn't exist
            }

            var task = new ProjectTask
            {
                ProjectId = projectId,
                Title = "",
                Description = ""
            };

            return View(task);
        }

        [HttpPost("Create/{projectId:int}")] //Lab6-Part2#1
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title", "Description", "ProjectId")] ProjectTask task) //Lab9
        {
            if (ModelState.IsValid)
            {
                await _context.ProjectTasks.AddAsync(task); //Lab9
                await _context.SaveChangesAsync(); //Lab9

                // Redirect to the Index action with the projectId of the created task
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            // Lab 9
            var projects = await _context.Projects.ToListAsync();

            // Repopulate the Projects SelectList if returning to the form
            ViewBag.Projects = new SelectList(projects, "ProjectId", "Name", task.ProjectId); //Lab9
            return View(task);
        }

        [HttpGet("Edit/{id:int}")] //Lab6-Part2#1
        public async Task<IActionResult> Edit(int id) //Lab9
        {
            var task = await _context.ProjectTasks //Lab9
                .Include(t => t.Project) // Include related project data
                .FirstOrDefaultAsync(t => t.ProjectTaskId == id); //Lab9

            if (task == null)
            {
                return NotFound();
            }

            // Async call to retrieve projects from SelectList
            // Lab 9
            var projects = await _context.Projects.ToListAsync();

            ViewBag.Projects = new SelectList(projects, "ProjectId", "Name", task.ProjectId);
            return View(task);
        }

        [HttpPost("Edit/{id:int}")] //Lab6-Part2#1
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectTaskId", "Title", "Description", "ProjectId")] ProjectTask task) //Lab9
        {
            if (id != task.ProjectTaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.ProjectTasks.Update(task);
                await _context.SaveChangesAsync(); //Lab9
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            ViewBag.Projects = new SelectList(_context.Projects, "ProjectId", "Name", task.ProjectId);
            return View(task);
        }

        [HttpGet("Delete/{id:int}")] //Lab6-Part2#1
        public async Task<IActionResult> Delete(int id) //Lab9
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project) // Include related project data
                .FirstOrDefaultAsync(t => t.ProjectTaskId == id); //Lab9

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost("DeleteConfirmed/{id:int}")] //Lab6-Part2#1
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int projectTaskId) //Lab9
        {
            var task = await _context.ProjectTasks.FindAsync(projectTaskId); //Lab9
            if (task != null)
            {
                _context.ProjectTasks.Remove(task);
                await _context.SaveChangesAsync(); //Lab9
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            return NotFound();
        }

        // Lab 6 - Search ProjectTasks
        // GET: ProjectTasks/Search/{projectId?}/{searchString?}
        [HttpGet("Search")]
        public async Task<IActionResult> Search(int? projectId, string searchString)
        {
            // Fetch all tasks as an IQueryable (deferred execution)
            var taskQuery = _context.ProjectTasks.AsQueryable();

            // Track whether a search was performed
            bool searchPerformed = !string.IsNullOrWhiteSpace(searchString);

            // If a projectId is provided, filter by project
            if (projectId.HasValue)
            {
                taskQuery = taskQuery.Where(t => t.ProjectId == projectId.Value);
            }

            // FIXED: Apply search filter when searchString is provided
            if (searchPerformed)
            {
                searchString = searchString.ToLower(); // Case-insensitive search

                // Ensure null-safe search on nullable Description
                taskQuery = taskQuery.Where(t =>
                    t.Title.ToLower().Contains(searchString) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchString))
                );
            }

            var tasks = await taskQuery.ToListAsync();

            // Pass search metadata to the view
            ViewData["SearchPerformed"] = searchPerformed;
            ViewData["SearchString"] = searchString;

            return View("Index", tasks);
        }
    }
}