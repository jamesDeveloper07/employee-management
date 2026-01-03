using Common.Domain;
using System.Text.RegularExpressions;

namespace Employee.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        var cleanPhone = Regex.Replace(phoneNumber, @"[^\d]", "");

        if (cleanPhone.Length < 10 || cleanPhone.Length > 11)
            throw new ArgumentException("Phone number must have 10 or 11 digits", nameof(phoneNumber));

        return new PhoneNumber(cleanPhone);
    }

    public string GetFormatted()
    {
        if (Value.Length == 11)
            return $"({Value.Substring(0, 2)}) {Value.Substring(2, 5)}-{Value.Substring(7, 4)}";

        return $"({Value.Substring(0, 2)}) {Value.Substring(2, 4)}-{Value.Substring(6, 4)}";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => GetFormatted();
}
