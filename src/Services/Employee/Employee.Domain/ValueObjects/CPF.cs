using Common.Domain;
using System.Text.RegularExpressions;

namespace Employee.Domain.ValueObjects;

public class CPF : ValueObject
{
    public string Value { get; private set; }

    private CPF(string value)
    {
        Value = value;
    }

    public static CPF Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF cannot be empty", nameof(cpf));

        var cleanCpf = Regex.Replace(cpf, @"[^\d]", "");

        if (cleanCpf.Length != 11)
            throw new ArgumentException("CPF must have 11 digits", nameof(cpf));

        if (!IsValid(cleanCpf))
            throw new ArgumentException("Invalid CPF", nameof(cpf));

        return new CPF(cleanCpf);
    }

    private static bool IsValid(string cpf)
    {
        if (cpf.All(c => c == cpf[0]))
            return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != firstDigit)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == secondDigit;
    }

    public string GetFormatted()
    {
        return $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9, 2)}";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => GetFormatted();
}
