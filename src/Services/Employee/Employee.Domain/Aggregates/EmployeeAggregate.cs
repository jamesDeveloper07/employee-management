using Common.Domain;
using Employee.Domain.Entities;
using Employee.Domain.Events;
using Employee.Domain.ValueObjects;

namespace Employee.Domain.Aggregates;

public class EmployeeAggregate : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public CPF CPF { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Address Address { get; private set; }
    public DateTime BirthDate { get; private set; }
    public DateTime HireDate { get; private set; }
    public decimal Salary { get; private set; }
    public string Position { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core constructor
    private EmployeeAggregate()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        CPF = null!;
        Email = null!;
        PhoneNumber = null!;
        Address = null!;
        Position = string.Empty;
    }

    private EmployeeAggregate(
        string firstName,
        string lastName,
        CPF cpf,
        Email email,
        PhoneNumber phoneNumber,
        Address address,
        DateTime birthDate,
        DateTime hireDate,
        decimal salary,
        string position,
        Guid departmentId)
    {
        FirstName = firstName;
        LastName = lastName;
        CPF = cpf;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        BirthDate = birthDate;
        HireDate = hireDate;
        Salary = salary;
        Position = position;
        DepartmentId = departmentId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static EmployeeAggregate Create(
        string firstName,
        string lastName,
        string cpf,
        string email,
        string phoneNumber,
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        DateTime birthDate,
        DateTime hireDate,
        decimal salary,
        string position,
        Guid departmentId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (salary <= 0)
            throw new ArgumentException("Salary must be greater than zero", nameof(salary));

        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Position cannot be empty", nameof(position));

        if (birthDate >= DateTime.UtcNow)
            throw new ArgumentException("Birth date must be in the past", nameof(birthDate));

        if (hireDate > DateTime.UtcNow)
            throw new ArgumentException("Hire date cannot be in the future", nameof(hireDate));

        var cpfVO = CPF.Create(cpf);
        var emailVO = Email.Create(email);
        var phoneVO = PhoneNumber.Create(phoneNumber);
        var addressVO = Address.Create(street, number, complement, neighborhood, city, state, zipCode);

        var employee = new EmployeeAggregate(
            firstName.Trim(),
            lastName.Trim(),
            cpfVO,
            emailVO,
            phoneVO,
            addressVO,
            birthDate,
            hireDate,
            salary,
            position.Trim(),
            departmentId
        );

        employee.AddDomainEvent(new EmployeeCreatedEvent(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email.Value,
            employee.CPF.Value,
            employee.DepartmentId
        ));

        return employee;
    }

    public void UpdatePersonalInfo(
        string firstName,
        string lastName,
        string email,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = Email.Create(email);
        PhoneNumber = PhoneNumber.Create(phoneNumber);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EmployeeUpdatedEvent(
            Id,
            FirstName,
            LastName,
            Email.Value
        ));
    }

    public void UpdateAddress(
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        string zipCode)
    {
        Address = Address.Create(street, number, complement, neighborhood, city, state, zipCode);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateJobInfo(string position, decimal salary, Guid departmentId)
    {
        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Position cannot be empty", nameof(position));

        if (salary <= 0)
            throw new ArgumentException("Salary must be greater than zero", nameof(salary));

        Position = position.Trim();
        Salary = salary;
        DepartmentId = departmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EmployeeActivatedEvent(Id));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EmployeeDeactivatedEvent(Id));
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public int GetAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - BirthDate.Year;
        if (BirthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    public int GetYearsOfService()
    {
        var today = DateTime.UtcNow;
        var years = today.Year - HireDate.Year;
        if (HireDate.Date > today.AddYears(-years)) years--;
        return years;
    }
}
