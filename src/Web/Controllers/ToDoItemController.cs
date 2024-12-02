using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.TodoItems.Commands.CreateTodoItem;
using ToDoApp.Application.TodoItems.Commands.DeleteTodoItem;
using ToDoApp.Application.TodoItems.Commands.CompleteTodoItem;
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
        catch (ValidationException ex)
        {
            var allErrors = ex.Errors.SelectMany(e => e.Value).ToList(); 
            TempData["Errors"] = string.Join("; ", allErrors); 
        }

        return Redirect("/");
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromForm] List<int> id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Received IDs: " + string.Join(", ", id));
        bool allDeleted = true;

        foreach (var id_num in id)
        {
            var command = new DeleteTodoItemCommand(id_num);

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
            return BadRequest(new { message = "Some items not found." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Complete([FromForm] List<int> id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Received IDs: " + string.Join(", ", id));
        bool allCompleted = true;

        foreach (var id_num in id)
        {
            var command = new CompleteTodoItemCommand(id_num);

            try
            {
                await _sender.Send(command, cancellationToken);
            }
            catch (NotFoundException)
            {
                allCompleted = false;
            }
        }

        if (allCompleted)
        {
            return Redirect("/");
        }
        else
        {
            return BadRequest(new { message = "Some items not found." });
        }
    }

}

