using Common.Domain;

namespace Employee.Domain.Events;

public class EmployeeCreatedEvent : DomainEvent
{
    public Guid EmployeeId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string CPF { get; }
    public Guid DepartmentId { get; }

    public EmployeeCreatedEvent(
        Guid employeeId,
        string firstName,
        string lastName,
        string email,
        string cpf,
        Guid departmentId)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CPF = cpf;
        DepartmentId = departmentId;
    }
}
