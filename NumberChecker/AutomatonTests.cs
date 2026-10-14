using Xunit;
using Number.Core.Models;
using Number.Core.Services;

namespace NumberChecker;

public class AutomatonTests
{
    private readonly IAutomatonService _engine = new DfaNumberEngine();

    // ── Số nguyên hợp lệ ──────────────────────────────────────
    [Theory]
    [InlineData("0",    "Số nguyên bằng không")]
    [InlineData("123",  "Số nguyên dương")]
    [InlineData("+99",  "Số nguyên dương")]
    [InlineData("-456", "Số nguyên âm")]
    [InlineData("-0",   "Số nguyên bằng không")]
    [InlineData("+0",   "Số nguyên bằng không")]
    public void Integers_AreValid_WithCorrectType(string input, string expected)
    {
        var r = _engine.Analyze(input);
        Assert.True(r.IsValid);
        Assert.Equal(expected, r.NumberType);
    }

    // ── Số thực hợp lệ ────────────────────────────────────────
    [Theory]
    [InlineData("1.23",  "Số thực dương")]
    [InlineData("0.5",   "Số thực dương")]
    [InlineData(".5",    "Số thực dương")]
    [InlineData("+.5",   "Số thực dương")]
    [InlineData("-.5",   "Số thực âm")]
    [InlineData("-9.99", "Số thực âm")]
    [InlineData("0.0",   "Số thực bằng không")]
    [InlineData("-0.0",  "Số thực bằng không")]
    [InlineData("+.0",   "Số thực bằng không")]
    public void RealNumbers_AreValid_WithCorrectType(string input, string expected)
    {
        var r = _engine.Analyze(input);
        Assert.True(r.IsValid);
        Assert.Equal(expected, r.NumberType);
    }

    // ── Chuỗi không hợp lệ ───────────────────────────────────
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1.2.3")]
    [InlineData("1..2")]
    [InlineData("++1")]
    [InlineData("1+")]
    [InlineData(".")]
    [InlineData("-.")]
    [InlineData("-1.")]
    [InlineData("1e5")]
    public void Invalid_Inputs_ReturnFalse(string input)
    {
        var r = _engine.Analyze(input);
        Assert.False(r.IsValid);
    }

    // ── Lộ trình trạng thái ───────────────────────────────────
    [Fact]
    public void ExecutionPath_ForInteger_IsCorrect()
    {
        var r = _engine.Analyze("12");
        // q0 -> q2 -> q2
        Assert.Equal(new[] { State.q0, State.q2, State.q2 }, r.ExecutionPath);
    }

    [Fact]
    public void ExecutionPath_ForSignedReal_IsCorrect()
    {
        var r = _engine.Analyze("-0.5");
        // q0 -> q1 -> q2 -> q4 -> q5
        Assert.Equal(new[] { State.q0, State.q1, State.q2, State.q4, State.q5 }, r.ExecutionPath);
    }

    [Fact]
    public void ExecutionPath_ForInvalid_EndsAtQErr()
    {
        var r = _engine.Analyze("1a");
        Assert.Equal(State.q_err, r.ExecutionPath.Last());
    }

    // ── Sinh ngẫu nhiên luôn hợp lệ ──────────────────────────
    [Fact]
    public void GenerateRandom_AlwaysProducesValidNumber()
    {
        for (int i = 0; i < 200; i++)
        {
            string rnd = _engine.GenerateRandom();
            var r = _engine.Analyze(rnd);
            Assert.True(r.IsValid, $"Random '{rnd}' failed");
        }
    }
}
