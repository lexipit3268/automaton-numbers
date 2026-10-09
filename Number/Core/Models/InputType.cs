namespace Number.Core.Models
{
    /// <summary>Phân loại ký tự đầu vào để tra bảng chuyển trạng thái</summary>
    public enum InputType
    {
        Digit,   // 0-9
        Dot,     // .
        Sign,    // + hoặc -
        Invalid  // Các ký tự khác
    }
}
