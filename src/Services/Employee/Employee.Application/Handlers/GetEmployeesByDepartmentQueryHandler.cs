using AutoMapper;
using Employee.Application.DTOs;
using Employee.Application.Queries;
using Employee.Domain.Repositories;
using MediatR;

namespace Employee.Application.Handlers;

public class GetEmployeesByDepartmentQueryHandler : IRequestHandler<GetEmployeesByDepartmentQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeesByDepartmentQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetByDepartmentIdAsync(request.DepartmentId, cancellationToken);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }
}
