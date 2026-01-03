using AutoMapper;
using Employee.Application.Commands;
using Employee.Application.DTOs;
using Employee.Domain.Aggregates;
using Employee.Domain.Repositories;
using Employee.Domain.Resources;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Employee.Application.Handlers;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<DomainExceptions> _localizer;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        IStringLocalizer<DomainExceptions> localizer)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existingEmployee = await _employeeRepository.GetByCPFAsync(request.CPF, cancellationToken);
        if (existingEmployee != null)
            throw new InvalidOperationException(_localizer["EmployeeWithCPFAlreadyExists", request.CPF]);

        var employee = EmployeeAggregate.Create(
            request.FirstName,
            request.LastName,
            request.CPF,
            request.Email,
            request.PhoneNumber,
            request.Street,
            request.Number,
            request.Complement,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode,
            request.BirthDate,
            request.HireDate,
            request.Salary,
            request.Position,
            request.DepartmentId
        );

        _employeeRepository.Add(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return _mapper.Map<EmployeeDto>(employee);
    }
}
