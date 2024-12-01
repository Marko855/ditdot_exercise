using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.TodoItems.Commands.DeleteTodoItem
{
    public class DeleteTodoItemCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteTodoItemCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTodoItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);

            if (todoItem == null)
            {
                throw new NotFoundException(nameof(TodoItem), request.Id.ToString());
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync(cancellationToken);

            return true; 
        }
    }
}
