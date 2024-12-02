using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.TodoItems.Commands.CompleteTodoItem
{
    public class CompleteTodoItemCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public CompleteTodoItemCommand(int id)
        {
            Id = id;
        }
    }

    public class CompleteTodoItemCommandHandler : IRequestHandler<CompleteTodoItemCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public CompleteTodoItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CompleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(new object[] { request.Id }, cancellationToken);

            if (todoItem == null)
            {
                throw new NotFoundException(nameof(TodoItem), request.Id.ToString());
            }

            todoItem.Done = true;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
