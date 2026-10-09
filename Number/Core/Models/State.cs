namespace Number.Core.Models
{
    /// <summary>Các trạng thái nội bộ của DFA</summary>
    public enum State
    {
        q0,      // Khởi tạo
        q1,      // Đã đọc dấu (+/-)
        q2,      // Phần nguyên          [ACCEPT]
        q3,      // Dấu thập phân đứng đầu (chưa có số)
        q4,      // Dấu thập phân sau phần nguyên
        q5,      // Phần thập phân        [ACCEPT]
        q_err    // Trạng thái lỗi
    }
}
