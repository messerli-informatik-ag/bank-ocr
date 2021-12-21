#pragma warning disable SA1118

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BankOcr.Model;
using Funcky;
using Funcky.Extensions;
using Xunit;
using Xunit.Sdk;

namespace BankOcr.Test
{
    public class BankOcrTest
    {
        [Fact]
        public void ParseBankNumberScansCorrectly()
        {
        }

        [Theory]
        [InlineData("000000000", "zeroes.txt")]
        [InlineData("111111111", "ones.txt")]
        [InlineData("222222222", "twos.txt")]
        [InlineData("333333333", "threes.txt")]
        [InlineData("123123123", "123s.txt")]
        [InlineData("444444444", "fours.txt")]
        [InlineData("123456789", "all_digits.txt")]
        [InlineData("1234?678?", "illegal.txt")]
        public void FilesAreParsedCorrectly(string expected, string filename)
        {
            var input = File.ReadAllText(Path.Combine("TestData", filename));
            Assert.Equal(expected, BankCodeFormatter.Format(Parser.Parse(input)));
        }

        [Fact]
        public void NonMatrixInputsArePrevented()
        {
            var input = File.ReadAllText(Path.Combine("TestData", "twos.txt"));
            input = input.Take(27).Append(' ').Concat(input.Skip(27)).ToList().ConcatToString();
            Assert.Throws<ArgumentException>(() => Parser.Parse(input));
        }

        [Theory]
        [MemberData(nameof(ChunkTestData))]
        public void MissingWhitespacesGotPadded(string input, params string[] expected)
        {
            input = input.SplitLines().Select(l => l.TrimEnd(' ')).JoinToString("\n");
            Assert.Equal(expected, Parser.ToDigitChunks(input).Single());
        }

        [Theory]
        [InlineData(
            "0",
            " _ ",
            "| |",
            "|_|")]
        [InlineData(
            "1",
            "   ",
            "  |",
            "  |")]
        [InlineData(
            "2",
            " _ ",
            " _|",
            "|_ ")]
        [InlineData(
            "3",
            " _ ",
            " _|",
            " _|")]
        [InlineData(
            "4",
            "   ",
            "|_|",
            "  |")]
        [InlineData(
            "5",
            " _ ",
            "|_ ",
            " _|")]
        [InlineData(
            "6",
            " _ ",
            "|_ ",
            "|_|")]
        [InlineData(
            "7",
            " _ ",
            "  |",
            "  |")]
        [InlineData(
            "8",
            " _ ",
            "|_|",
            "|_|")]
        [InlineData(
            "9",
            " _ ",
            "|_|",
            " _|")]
        public void DigitIsParsedCorrectly(string expected, params string[] digit)
        {
            var result = Parser.ParseDigit(digit);
            Assert.Equal(expected, GetValidDigitOrThrow(result));
        }

        [Theory]
        [MemberData(nameof(ChunkTestData))]
        public void ChunksSingleOneCorrectly(string input, params string[] expected)
        {
            Assert.Equal(expected, Parser.ToDigitChunks(input).Single());
        }

        [Theory]
        [MemberData(nameof(FormatValidationInputs))]
        public void VerifyBankCodeValidation(BankCodeValidity validity, IEnumerable<BankCodeDigit> digits)
        {
            Assert.Equal(validity, BankCodeValidator.Validate(digits));
        }

        public static TheoryData<BankCodeValidity, IEnumerable<BankCodeDigit>> FormatValidationInputs()
            => new()
                {
                    {
                        BankCodeValidity.Valid,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty))
                    },
                    {
                        BankCodeValidity.Valid,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(2, string.Empty),
                            new BankCodeDigit.Valid(3, string.Empty),
                            new BankCodeDigit.Valid(4, string.Empty),
                            new BankCodeDigit.Valid(5, string.Empty),
                            new BankCodeDigit.Valid(6, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                    {
                        BankCodeValidity.Valid,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(4, string.Empty),
                            new BankCodeDigit.Valid(9, string.Empty),
                            new BankCodeDigit.Valid(0, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(6, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(5, string.Empty))
                    },
                    {
                        BankCodeValidity.Error,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                    {
                        BankCodeValidity.Error,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(4, string.Empty),
                            new BankCodeDigit.Valid(9, string.Empty),
                            new BankCodeDigit.Valid(0, string.Empty),
                            new BankCodeDigit.Valid(0, string.Empty),
                            new BankCodeDigit.Valid(6, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(5, string.Empty))
                    },
                    {
                        BankCodeValidity.Error,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(0, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(2, string.Empty),
                            new BankCodeDigit.Valid(3, string.Empty),
                            new BankCodeDigit.Valid(4, string.Empty),
                            new BankCodeDigit.Valid(5, string.Empty),
                            new BankCodeDigit.Valid(6, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                    {
                        BankCodeValidity.Illegal,
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(0, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(2, string.Empty),
                            new BankCodeDigit.Valid(3, string.Empty),
                            new BankCodeDigit.Invalid(string.Empty),
                            new BankCodeDigit.Valid(5, string.Empty),
                            new BankCodeDigit.Valid(6, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                };

        [Theory]
        [MemberData(nameof(FormatInputs))]
        public void IsFormattedCorrectly(string expected, IEnumerable<BankCodeDigit> digits)
        {
            Assert.Equal(expected, BankCodeFormatter.Format(digits));
        }

        public static TheoryData<string, IEnumerable<BankCodeDigit>> FormatInputs()
            => new()
                {
                    {
                        "711111111",
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty))
                    },
                    {
                        "0123?5678 ILL",
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(0, string.Empty),
                            new BankCodeDigit.Valid(1, string.Empty),
                            new BankCodeDigit.Valid(2, string.Empty),
                            new BankCodeDigit.Valid(3, string.Empty),
                            new BankCodeDigit.Invalid(string.Empty),
                            new BankCodeDigit.Valid(5, string.Empty),
                            new BankCodeDigit.Valid(6, string.Empty),
                            new BankCodeDigit.Valid(7, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                    {
                        "888888888 ERR",
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                    {
                        "??8888888 ILL",
                        Sequence.Return<BankCodeDigit>(
                            new BankCodeDigit.Invalid(string.Empty),
                            new BankCodeDigit.Invalid(string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty),
                            new BankCodeDigit.Valid(8, string.Empty))
                    },
                };

        // [Fact]
        // [InlineData("123456789", "illegal.txt")]
        // public void CanFixSingleInterpreterMistake(string expected, string inputFileName)
        // {
        //     Assert.Equal(expected, FixingParser.Parse(File.ReadAllText(inputFileName)));
        // }

        [Theory]
        [InlineData(
            0,
            "   " +
            "  |" +
            "  |",
            "   " +
            "  |" +
            "  |")]
        [InlineData(
            1,
            "   " +
            "  |" +
            "  |",
            " _ " +
            "  |" +
            "  |")]
        [InlineData(
            2,
            " _ " +
            " _|" +
            "|_ ",
            " _ " +
            "|_|" +
            "|_|")]
        public void CalculateCharacterDistanceCorrectly(int expected, string left, string right)
        {
            Assert.Equal(expected, DistanceCalculator.CalculateDistance(left, right));
        }

        [Theory]
        [InlineData(
            "   ",
            "___",
            "  |")]
        public void FindSingleMistakesCorrectly(params string[] input)
        {
            Assert.IsType<BankCodeDigit.Invalid>(Parser.ParseDigit(input));
        }

        [Theory]
        [InlineData(
            "   " +
            "   " +
            "  |",
            1)]
        [InlineData(
            "   " +
            "   " +
            "   ")]
        public void FindsCorrectCandidates(string input, params int[] candidates)
        {
            Assert.Equal(candidates, CandidateFinder.Find(input));
        }

        [Theory]
        [InlineData("000000051", "valid.txt")]
        [InlineData("711111111", "fixable.txt")]
        public void FixingParserWorksAsIntended(string expected, string fileName)
        {
            var fileContent = File.ReadAllText(Path.Combine("TestData", fileName));
            Assert.Equal(expected, BankCodeFormatter.Format(FixingParser.Parse(fileContent)));
        }

        public static TheoryData<string, string, string, string> ChunkTestData()
            => new()
            {
                {
                    "   \n" +
                    "  |\n" +
                    "  |\n",
                    "   ",
                    "  |",
                    "  |"
                },
            };

        private static string GetValidDigitOrThrow(BankCodeDigit digit)
        {
            return digit is BankCodeDigit.Valid valid ? valid.Value.ToString() : throw new XunitException("Invalid digit");
        }
    }
}
