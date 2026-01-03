using Common.Domain;

namespace Employee.Domain.Events;

public class EmployeeDeactivatedEvent : DomainEvent
{
    public Guid EmployeeId { get; }

    public EmployeeDeactivatedEvent(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}
