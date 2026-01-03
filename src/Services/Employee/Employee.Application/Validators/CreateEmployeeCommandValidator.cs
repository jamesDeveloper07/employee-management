using Employee.Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Employee.Application.Validators;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(localizer["FirstName_Required"])
            .MaximumLength(100).WithMessage(localizer["FirstName_MaxLength"]);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(localizer["LastName_Required"])
            .MaximumLength(100).WithMessage(localizer["LastName_MaxLength"]);

        RuleFor(x => x.CPF)
            .NotEmpty().WithMessage(localizer["CPF_Required"])
            .Matches(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$")
            .WithMessage(localizer["CPF_InvalidFormat"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["Email_Required"])
            .EmailAddress().WithMessage(localizer["Email_InvalidFormat"]);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(localizer["PhoneNumber_Required"])
            .Matches(@"^\d{10,11}$|^\(\d{2}\)\s?\d{4,5}-\d{4}$")
            .WithMessage(localizer["PhoneNumber_InvalidFormat"]);

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage(localizer["Street_Required"])
            .MaximumLength(200).WithMessage(localizer["Street_MaxLength"]);

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage(localizer["Number_Required"])
            .MaximumLength(20).WithMessage(localizer["Number_MaxLength"]);

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage(localizer["Neighborhood_Required"])
            .MaximumLength(100).WithMessage(localizer["Neighborhood_MaxLength"]);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage(localizer["City_Required"])
            .MaximumLength(100).WithMessage(localizer["City_MaxLength"]);

        RuleFor(x => x.State)
            .NotEmpty().WithMessage(localizer["State_Required"])
            .Length(2).WithMessage(localizer["State_ExactLength"]);

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage(localizer["ZipCode_Required"])
            .Matches(@"^\d{8}$|^\d{5}-\d{3}$")
            .WithMessage(localizer["ZipCode_InvalidFormat"]);

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage(localizer["BirthDate_Required"])
            .LessThan(DateTime.UtcNow).WithMessage(localizer["BirthDate_MustBeInPast"]);

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage(localizer["HireDate_Required"])
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(localizer["HireDate_CannotBeInFuture"]);

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage(localizer["Salary_GreaterThanZero"]);

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage(localizer["Position_Required"])
            .MaximumLength(100).WithMessage(localizer["Position_MaxLength"]);

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage(localizer["DepartmentId_Required"]);
    }
}

// Dummy class for resource file generation
public class ValidationMessages { }
