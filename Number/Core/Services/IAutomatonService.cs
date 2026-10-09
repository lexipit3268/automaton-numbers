using Number.Core.Models;

namespace Number.Core.Services
{
    /// <summary>Giao diện hành vi của máy kiểm tra số — dễ mock khi Unit Test</summary>
    public interface IAutomatonService
    {
        AutomatonResult Analyze(string input);
        string GenerateRandom();
    }
}
