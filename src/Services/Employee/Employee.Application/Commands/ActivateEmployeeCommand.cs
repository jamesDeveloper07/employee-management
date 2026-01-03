using MediatR;

namespace Employee.Application.Commands;

public class ActivateEmployeeCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public ActivateEmployeeCommand(Guid id)
    {
        Id = id;
    }
}
