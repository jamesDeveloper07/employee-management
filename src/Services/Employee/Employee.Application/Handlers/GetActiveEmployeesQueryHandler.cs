using AutoMapper;
using Employee.Application.DTOs;
using Employee.Application.Queries;
using Employee.Domain.Repositories;
using MediatR;

namespace Employee.Application.Handlers;

public class GetActiveEmployeesQueryHandler : IRequestHandler<GetActiveEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetActiveEmployeesQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> Handle(GetActiveEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetActiveEmployeesAsync(cancellationToken);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }
}
