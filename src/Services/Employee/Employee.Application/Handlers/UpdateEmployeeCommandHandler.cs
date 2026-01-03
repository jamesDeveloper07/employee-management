using AutoMapper;
using Employee.Application.Commands;
using Employee.Application.DTOs;
using Employee.Domain.Repositories;
using Employee.Domain.Resources;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Employee.Application.Handlers;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<DomainExceptions> _localizer;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IMapper mapper,
        IStringLocalizer<DomainExceptions> localizer)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
            throw new InvalidOperationException(_localizer["EmployeeNotFound", request.Id]);

        employee.UpdatePersonalInfo(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber
        );

        employee.UpdateAddress(
            request.Street,
            request.Number,
            request.Complement,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode
        );

        employee.UpdateJobInfo(
            request.Position,
            request.Salary,
            request.DepartmentId
        );

        _employeeRepository.Update(employee);
        await _employeeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return _mapper.Map<EmployeeDto>(employee);
    }
}
