using System.Collections.Generic;
using System.Linq;
using static BankOcr.DigitsMapping;
using static BankOcr.DistanceCalculator;

namespace BankOcr;

public static class CandidateFinder
{
    public static IEnumerable<int> Find(string input)
        => Digits
            .Where(d => CalculateDistance(d.Key, input) == 1)
            .Select(d => d.Value);
}
