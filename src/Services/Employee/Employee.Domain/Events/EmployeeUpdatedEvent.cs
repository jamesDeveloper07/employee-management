using Common.Domain;

namespace Employee.Domain.Events;

public class EmployeeUpdatedEvent : DomainEvent
{
    public Guid EmployeeId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public EmployeeUpdatedEvent(
        Guid employeeId,
        string firstName,
        string lastName,
        string email)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}
