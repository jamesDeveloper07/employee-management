using Employee.Application.DTOs;
using MediatR;

namespace Employee.Application.Queries;

public class GetEmployeeByIdQuery : IRequest<EmployeeDto?>
{
    public Guid Id { get; set; }

    public GetEmployeeByIdQuery(Guid id)
    {
        Id = id;
    }
}
