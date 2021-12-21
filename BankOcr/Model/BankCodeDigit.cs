using Funcky;

namespace BankOcr.Model;

[DiscriminatedUnion]
public abstract partial record BankCodeDigit
{
    public sealed partial record Valid(int Value, string Source) : BankCodeDigit;

    public sealed partial record Invalid(string Source) : BankCodeDigit;
}
