using System.Linq;
using Funcky.Extensions;

namespace BankOcr;

public static class DistanceCalculator
{
    public static int CalculateDistance(string first, string second)
        => first.ConcatToString()
            .Zip(second.ConcatToString())
            .Count(pair => pair.First != pair.Second);
}
