using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateTodoItemCommand> _validator;


    public CreateTodoItemCommandHandler(IApplicationDbContext context, IValidator<CreateTodoItemCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {   
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var todoItem = new TodoItem
        {
            Title = request.Title,
            ListId = request.ListId,
            Done = false 
        };

        _context.TodoItems.Add(todoItem);

        await _context.SaveChangesAsync(cancellationToken);

        return todoItem.Id;
    }
}
