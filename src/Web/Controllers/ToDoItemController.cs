using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.TodoItems.Commands.CreateTodoItem;
using ToDoApp.Application.TodoItems.Commands.DeleteTodoItem;
using ToDoApp.Application.Common.Exceptions;
using System.Collections.Generic;
using System.Threading;

namespace ToDoApp.Web.Controllers;

public class ToDoItemController : Controller
{
    private readonly ISender _sender;

    public ToDoItemController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoItemCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(command, cancellationToken);
        }
        catch (ValidationException)
        {
            //TempData["Errors"] = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
        }

        return Redirect("/");
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody]List<int> id_list, CancellationToken cancellationToken)
    {   

        Console.WriteLine("Received IDs: " + string.Join(", ", id_list));
        bool allDeleted = true;

        foreach (var id in id_list)
        {
            var command = new DeleteTodoItemCommand(id);

            try
            {
                await _sender.Send(command, cancellationToken);
            }
            catch (NotFoundException)
            {
                allDeleted = false;
            }
        }

        if (allDeleted)
        {
            return Redirect("/");
        }
        else
        {
            return BadRequest(new { message = "Some items were not found." });
        }
    }

}

