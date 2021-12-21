using System.Collections.Generic;
using System.Linq;
using BankOcr.Model;

namespace BankOcr;

public static class BankCodeValidator
{
    public static BankCodeValidity Validate(IEnumerable<BankCodeDigit> code)
        => code.Any(d => d is BankCodeDigit.Invalid)
            ? BankCodeValidity.Illegal
            : Validate(code.Cast<BankCodeDigit.Valid>().Select(d => d.Value))
                ? BankCodeValidity.Valid
                : BankCodeValidity.Error;

    private static bool Validate(IEnumerable<int> digits)
    {
        var sum = digits.Reverse().Select((value, index) => value * (index + 1)).Sum();
        return sum % 11 == 0;
    }
}
