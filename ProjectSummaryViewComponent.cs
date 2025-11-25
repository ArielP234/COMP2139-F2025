using COMP2139_ICE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_ICE.Areas.ProjectManagement.Components.ProjectSummary;

public class ProjectSummaryViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public ProjectSummaryViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int projectId)
    {
        // Query the Projects table asynchronously to retrieve the project with the specified ID.
        // Includes the related 'Tasks' navigation property to load associated tasks with the project.
        var project = await _context.Projects
            .Include(p => p.Tasks) // Load related tasks for the project (eager loading).
            .FirstOrDefaultAsync(p => p.ProjectId == projectId); // Retrieve the project with the matching ID.

        // Check if the project is null (not found in the database).
        if (project == null)
        {
            // Return a plain text response indicating the project was not found.
            return Content("Project not found");
        }

        // Return the default view for this ViewComponent, passing the project as the model.
        return View(project);
    }

}

//--Key takeaway--
//Task - Represents asynchronous execution
//<IViewComponentResult> - The type of result the task will produce
//Task<IViewComponentResult> - “An async method that eventually returns a renderable component result”
//await - Used to asynchronously wait for the completion of the task
//async - Indicates that the method can perform asynchronous operations

//DbContext - Represents a session with the database, allowing querying and saving data
//FirstOrDefaultAsync - Asynchronously retrieves the first element of a sequence or a default value
//Include - Eagerly loads related entities (in this case, tasks associated with the project)
//Content - Returns a content result with the specified string
//View - Returns a view result with the specified model (in this case, the project)
//InvokeAsync - The method that gets called when the ViewComponent is invoked, allowing for asynchronous execution and returning a result that can be rendered in a view.
//ProjectSummaryViewComponent - A ViewComponent that retrieves and displays project summary information
//ApplicationDbContext - The database context used to interact with the database, typically containing DbSet



//File name - Default.cshtml
@model COMP2139_ICE.Areas.ProjectManagement.Models.Project
<!--Week 10 - Lab 8-->
<div class="project-summary">
    <p>@Html.DisplayFor(model => model.Name, "ProjectName")</p>
    <p>@Html.DisplayFor(model => model.Description, "ProjectDescription")</p>
    <p><b>Start Date: </b>@Html.DisplayFor(model => model.StartDate, "DateTime")</p>
    <p><b>End Date: </b>@Html.DisplayFor(model => model.EndDate, "DateTime")</p>
    <p><b>Status: </b>@Html.DisplayFor(model => model.Status, "ProjectName")</p>

    <p><b>Tasks</b></p>
    <ul>
        @foreach (var task in Model.Tasks)
        {
            <li>
                @Html.DisplayFor(model => task.Title, "ProjectTaskTitle") -
                @Html.DisplayFor(model => task.Description, "ProjectDescription")
            </li>
        }
    </ul>
</div>

//--Key takeaway--
//@model - Specifies the type of the model that the view will use, allowing access to its properties
//@Html.DisplayFor - Renders the specified property of the model using the default display template
//@foreach - Iterates over a collection (in this case, the tasks associated with the


//File name /Areas/ProjectManagement/Projects/Index.cshtml
//Position just below <td>@project.ProjectId</td>

                    <!--Week 10 - Lab 8-->
                    <td>@Html.DisplayFor(modelItem => project.Name, "ProjectName")</td> <!-- Uses ProjectName.cshtml -->
                    <td>@Html.DisplayFor(modelItem => project.Description, "ProjectDescription") <!-- Uses ProjectDescription.cshtml --></td>
                   
                    <!--Week 10 - Lab 8 -- ViewComponent-->
                    <td>@await Component.InvokeAsync("ProjectSummary", new {projectId = project.ProjectId})</td>

//--Key takeaway--
//@await Component.InvokeAsync - Invokes a ViewComponent asynchronously, allowing it to render its content within the view
//new { projectId = project.ProjectId } - Passes the project ID as a parameter to the ViewComponent
//"ProjectSummary" - The name of the ViewComponent to be invoked, which corresponds to the ProjectSummaryViewComponent class
//@Html.DisplayFor - Renders the specified property of the model using the default display template
//"ProjectName" - A custom display template for rendering the project name
//"ProjectDescription" - A custom display template for rendering the project description
