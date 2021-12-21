using System.Collections.Generic;
using System.Linq;
using BankOcr.Model;
using Funcky.Extensions;
using static BankOcr.BankCodeValidity;

namespace BankOcr;

public static class BankCodeFormatter
{
    public static string Format(IEnumerable<BankCodeDigit> bankCode)
        => BankCodeValidator.Validate(bankCode) switch
        {
            Error => $"{FormatBankCode(bankCode)} ERR",
            Illegal => $"{FormatBankCode(bankCode)} ILL",
            _ => FormatBankCode(bankCode),
        };

    private static string FormatBankCode(IEnumerable<BankCodeDigit> code)
        => code.Select(Format).ConcatToString();

    private static string Format(BankCodeDigit digit)
        => digit.Match(
            valid => valid.Value.ToString(),
            _ => "?");
}
