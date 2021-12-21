using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BankOcr.Model;
using Funcky.Extensions;
using static BankOcr.DigitsMapping;

namespace BankOcr;

public static class Parser
{
    private const int DigitWidth = 3;
    private const int DigitHeight = 3;

    public static IEnumerable<BankCodeDigit> Parse(string input)
        => ToDigitChunks(input).Select(ParseDigit);

    public static IEnumerable<IReadOnlyCollection<string>> ToDigitChunks(string input)
    {
        var lines = input.SplitLines().Take(DigitHeight).ToImmutableArray();
        var maxWidth = lines.Max(l => l.Length);
        return lines
            .Select(l => l.PadRight(maxWidth, ' '))
            .Inspect(EnsureLineHasExpectedLength)
            .Transpose()
            .Chunk(DigitWidth)
            .Select(x => (IReadOnlyCollection<string>)x.Transpose().Select(l => l.ConcatToString()).ToImmutableArray());
    }

    public static BankCodeDigit ParseDigit(IReadOnlyCollection<string> digit)
        => Digits.GetValueOrNone(digit.ConcatToString()).Match<BankCodeDigit>(
            some: parsed => new BankCodeDigit.Valid(parsed, digit.ConcatToString()),
            none: () => new BankCodeDigit.Invalid(digit.ConcatToString()));

    private static void EnsureLineHasExpectedLength(string line)
    {
        if (line.Length % DigitWidth != 0)
        {
            throw new ArgumentException($"Invalid line '{line}'");
        }
    }
}
