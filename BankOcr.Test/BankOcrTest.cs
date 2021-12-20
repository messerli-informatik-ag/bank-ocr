using System.Linq;
using Xunit;

namespace BankOcr.Test
{
    public class BankOcrTest
    {
        [Fact]
        public void ParseBankNumberScansCorrectly()
        {
        }

        ////[Theory]
        ////[InlineData("711111111", true)]
        ////[InlineData("123456789", true)]
        ////[InlineData("490867715", true)]
        ////[InlineData("888888888", false)]
        ////[InlineData("490067715", false)]
        ////[InlineData("012345678", false)]
        ////public void BankCodeIsValidReturnsTrueOnAValidCode(string code, bool valid)
        ////{
        ////}

        [Fact]
        public void WriteReportFileCorrectly()
        {
        }

        [Fact]
        public void FindSingleMistakesCorrectly()
        {
        }
    }
}
