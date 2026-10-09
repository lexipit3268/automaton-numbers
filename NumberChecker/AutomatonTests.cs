using Xunit;
using Number.Core;

namespace NumberChecker
{
    public class AutomatonTests
    {
        [Theory]
        [InlineData("0", "Số nguyên dương")]
        [InlineData("123", "Số nguyên dương")]
        [InlineData("+123", "Số nguyên dương")]
        [InlineData("-456", "Số nguyên âm")]
        [InlineData("-0", "Số nguyên âm")]
        public void AnalyzeNumber_Integers_ReturnsCorrectType(string input, string expectedInfo)
        {
            var result = NumberAutomaton.AnalyzeNumber(input);
            Assert.True(result.IsValid);
            Assert.Equal(expectedInfo, result.TypeInfo);
        }

        [Theory]
        [InlineData("1.23", "Số thực (hệ thập phân) dương")]
        [InlineData("0.5", "Số thực (hệ thập phân) dương")]
        [InlineData(".5", "Số thực (hệ thập phân) dương")]
        [InlineData("+.5", "Số thực (hệ thập phân) dương")]
        [InlineData("-.5", "Số thực (hệ thập phân) âm")]
        [InlineData("+1.23", "Số thực (hệ thập phân) dương")]
        [InlineData("-9.99", "Số thực (hệ thập phân) âm")]
        public void AnalyzeNumber_RealNumbers_ReturnsCorrectType(string input, string expectedInfo)
        {
            var result = NumberAutomaton.AnalyzeNumber(input);
            Assert.True(result.IsValid);
            Assert.Equal(expectedInfo, result.TypeInfo);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("1..23")]
        [InlineData("1.2.3")]
        [InlineData("++1")]
        [InlineData("1+")]
        [InlineData(".")]
        [InlineData("-.")]
        [InlineData("-1.")]
        public void AnalyzeNumber_InvalidInputs_ReturnsFalse(string input)
        {
            var result = NumberAutomaton.AnalyzeNumber(input ?? string.Empty);
            Assert.False(result.IsValid);
        }
    }
}
