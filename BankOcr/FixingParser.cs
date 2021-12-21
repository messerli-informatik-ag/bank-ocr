using System.Collections.Generic;
using BankOcr.Model;

namespace BankOcr;

public static class FixingParser
{
    public static IEnumerable<BankCodeDigit> Parse(string input)
    {
        var bankCode = Parser.Parse(input);
        return BankCodeValidator.Validate(bankCode) switch
        {
            BankCodeValidity.Valid => bankCode,
            BankCodeValidity.Error => TryFixBankCodeWithChecksumError(bankCode, input),
        };
    }

    private static IEnumerable<BankCodeDigit> TryFixBankCodeWithChecksumError(IEnumerable<BankCodeDigit> bankCode, string input)
    {
        var candidates = CandidateFinder.Find(input);

        return bankCode;
    }
}
