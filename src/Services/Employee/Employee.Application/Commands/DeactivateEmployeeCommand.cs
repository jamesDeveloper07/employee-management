using MediatR;

namespace Employee.Application.Commands;

public class DeactivateEmployeeCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeactivateEmployeeCommand(Guid id)
    {
        Id = id;
    }
}
