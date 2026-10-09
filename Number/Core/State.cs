namespace Number.Core
{
    public enum State
    {
        q0,      // Khởi tạo
        q1,      // Có dấu (+, -)
        q2,      // Phần nguyên (Chấp nhận)
        q3,      // Chấm thập phân đầu tiên chưa có số
        q4,      // Chấm thập phân sau phần nguyên
        q5,      // Phần thập phân (Chấp nhận)
        q_err    // Lỗi
    }
}
