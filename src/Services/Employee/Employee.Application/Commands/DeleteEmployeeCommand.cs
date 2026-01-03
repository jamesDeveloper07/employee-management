using MediatR;

namespace Employee.Application.Commands;

public class DeleteEmployeeCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteEmployeeCommand(Guid id)
    {
        Id = id;
    }
}
