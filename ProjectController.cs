using COMP2139_ICE.Areas.ProjectManagement.Models;
using COMP2139_ICE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_ICE.Areas.ProjectManagement.Controllers
{
    [Area("ProjectManagement")]
    [Route("[area]/[controller]/[action]")]
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")] //Lab6-Part2#1
        public async Task<IActionResult> Index() //Lab9
        {
            var projects = await _context.Projects.ToListAsync(); //Lab9
            return View(projects);
        }

        [HttpGet("Create")] //Lab6-Part2#1
        public IActionResult Create()
        {
            return View();
        }

        //Lab4 - Part2 - #1
        [HttpPost("Create")] //Lab6-Part2#1
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project) //Lab9
        {
            if (ModelState.IsValid)
            {
                // Convert to UTC before saving
                project.StartDate = ToUtc(project.StartDate);
                project.EndDate = ToUtc(project.EndDate);

                await _context.Projects.AddAsync(project); //Lab9
                await _context.SaveChangesAsync(); //Lab9
                return RedirectToAction("Index");
            }

            return View(project);
        }

        private DateTime ToUtc(DateTime input)
        {
            if (input.Kind == DateTimeKind.Utc)
                return input;
            if (input.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(input, DateTimeKind.Utc); // assume local is already UTC
            return input.ToUniversalTime();
        }

        //Lab4 - Part3 - #1
        [HttpGet("Edit/{id:int?}")] //Lab6-Part2#1
        public async Task<IActionResult> Edit(int id) //Lab9
        {
            var project = await _context.Projects.FindAsync(id); //Lab9
            if (project == null)
            {
                return NotFound(); // Returns a 404 error if the project doesn't exist.
            }
            return View(project);
        }

        //Lab4 - Part3 - #2
        [HttpPost("Edit/{id:int?}")] //Lab6-Part2#1
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name,Description")] Project project) //Lab9
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Projects.Update(project);
                    await _context.SaveChangesAsync(); //Lab9
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(project.ProjectId)) //Lab9
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(project);
        }

        private async Task<bool> ProjectExists(int id) //Lab9
        {
            return await _context.Projects.AnyAsync(e => e.ProjectId == id);
        }

        //Lab4 - Part3 - #2
        [HttpGet("Details/{id:int?}")] //Lab6-Part2#1
        public async Task<IActionResult> Details(int id) //Lab9
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id); //Lab9
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        //Lab4 - Part4 - #1
        [HttpGet("Delete/{id:int?}")] //Lab6-Part2#1
        public async Task<IActionResult> Delete(int id) //Lab9
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id); //Lab9
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        //Lab4 - Part4 - #2
        [HttpPost("DeleteConfirmed/{id:int?}")] //Lab6-Part2#1
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) //Lab9
        {
            var project = await _context.Projects.FindAsync(id); //Lab9
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync(); //Lab9
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // Lab 6 - Project Search Functionality
        // Custom route for search functionality
        // Accessible at /Projects/Search/{searchString?}
        [HttpGet("Search/{searchString?}")]
        public async Task<IActionResult> Search(string searchString)
        {
            // Fetch all projects from the database as an IQueryable collection
            // IQueryable allows us to apply filters before executing the database query
            var projectsQuery = _context.Projects.AsQueryable();

            // Check if a search string was provided (avoids null or empty search issues)
            bool searchPerformed = !string.IsNullOrWhiteSpace(searchString);

            if (searchPerformed)
            {
                // Convert searchString to lowercase to make the search case-insensitive
                searchString = searchString.ToLower();

                // Apply filtering: Match project name or description
                // Query ensures p.Name is checked first before calling ToLower() to prevent NullReferenceException
                projectsQuery = projectsQuery.Where(p =>
                    p.Name.ToLower().Contains(searchString) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchString)));
            }

            // Execute the query asynchronously using `ToListAsync()`
            var projects = await projectsQuery.ToListAsync();

            // Store search metadata for the view
            ViewData["SearchPerformed"] = searchPerformed;
            ViewData["SearchString"] = searchString;

            // Return the filtered list to the Index view (reusing existing UI)
            return View("Index", projects);
        }
    }
}