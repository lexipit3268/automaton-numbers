namespace Number.Core.Models
{
    /// <summary>
    /// Kết quả phân tích chuỗi sau khi DFA chạy xong.
    /// Đây là "Model" đại diện cho output — tách khỏi logic xử lý và UI.
    /// </summary>
    public class AutomatonResult
    {
        /// <summary>Chuỗi hợp lệ hay không</summary>
        public bool IsValid { get; init; }

        /// <summary>Mô tả loại số: "Số nguyên dương", "Số thực âm", v.v.</summary>
        public string NumberType { get; init; } = string.Empty;

        /// <summary>Lịch sử các trạng thái DFA đã đi qua (để hiển thị trực quan)</summary>
        public List<State> ExecutionPath { get; init; } = new();

        /// <summary>Chuỗi đầu vào gốc</summary>
        public string Input { get; init; } = string.Empty;
    }
}
