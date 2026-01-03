using Common.Domain;

namespace Employee.Domain.Events;

public class EmployeeActivatedEvent : DomainEvent
{
    public Guid EmployeeId { get; }

    public EmployeeActivatedEvent(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}
