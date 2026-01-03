using Employee.Application.DTOs;
using MediatR;

namespace Employee.Application.Queries;

public class GetEmployeesByDepartmentQuery : IRequest<IEnumerable<EmployeeDto>>
{
    public Guid DepartmentId { get; set; }

    public GetEmployeesByDepartmentQuery(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
}
