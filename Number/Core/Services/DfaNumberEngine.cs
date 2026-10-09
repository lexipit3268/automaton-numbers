using Number.Core.Models;
using State = Number.Core.Models.State;

namespace Number.Core.Services
{
    /// <summary>
    /// Triển khai DFA bằng Table-Driven Pattern.
    /// TransitionTable là ánh xạ trực tiếp của Bảng chuyển trạng thái trong lý thuyết Automata.
    /// </summary>
    public class DfaNumberEngine : IAutomatonService
    {
        // ===== BẢNG CHUYỂN TRẠNG THÁI (State Transition Table) =====
        private static readonly Dictionary<(State, InputType), State> TransitionTable = new()
        {
            // Từ q0 (Bắt đầu)
            { (State.q0, InputType.Sign),  State.q1 },
            { (State.q0, InputType.Digit), State.q2 },
            { (State.q0, InputType.Dot),   State.q3 },

            // Từ q1 (Đã đọc dấu)
            { (State.q1, InputType.Digit), State.q2 },
            { (State.q1, InputType.Dot),   State.q3 },

            // Từ q2 (Phần nguyên) [ACCEPT]
            { (State.q2, InputType.Digit), State.q2 },
            { (State.q2, InputType.Dot),   State.q4 },

            // Từ q3 (Dấu chấm đứng đầu)
            { (State.q3, InputType.Digit), State.q5 },

            // Từ q4 (Dấu chấm sau nguyên)
            { (State.q4, InputType.Digit), State.q5 },

            // Từ q5 (Phần thập phân) [ACCEPT]
            { (State.q5, InputType.Digit), State.q5 },
        };

        // ===== PHÂN LOẠI KÝ TỰ =====
        private static InputType Classify(char c) => c switch
        {
            >= '0' and <= '9' => InputType.Digit,
            '.'               => InputType.Dot,
            '+' or '-'        => InputType.Sign,
            _                 => InputType.Invalid
        };

        // ===== PHÂN TÍCH CHUỖI =====
        public AutomatonResult Analyze(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new AutomatonResult { IsValid = false, Input = input ?? "", NumberType = "Chuỗi rỗng" };

            var path = new List<State>();
            State current = State.q0;
            path.Add(current);

            foreach (char c in input)
            {
                InputType type = Classify(c);
                if (TransitionTable.TryGetValue((current, type), out State next))
                {
                    current = next;
                }
                else
                {
                    current = State.q_err;
                }
                path.Add(current);
                if (current == State.q_err) break;
            }

            bool isValid = current is State.q2 or State.q5;
            string numberType = BuildTypeDescription(input, current, isValid);

            return new AutomatonResult
            {
                IsValid = isValid,
                NumberType = numberType,
                ExecutionPath = path,
                Input = input
            };
        }

        private static string BuildTypeDescription(string input, State finalState, bool isValid)
        {
            if (!isValid) return "Không hợp lệ";

            string sign = input.StartsWith('-') ? "âm" : "dương";
            string kind = finalState == State.q2 ? "Số nguyên" : "Số thực (thập phân)";
            return $"{kind} {sign}";
        }

        // ===== SINH SỐ NGẪU NHIÊN =====
        public string GenerateRandom()
        {
            var rng = Random.Shared;
            int kind = rng.Next(0, 4); // 0=nguyên dương, 1=nguyên âm, 2=thực dương, 3=thực âm

            string sign = (kind == 1 || kind == 3) ? "-" : "";
            int intPart = rng.Next(0, 10000);

            if (kind < 2) // số nguyên
                return $"{sign}{intPart}";

            // số thực
            int decPart = rng.Next(1, 10000);
            // đôi khi bỏ phần nguyên (dạng .5)
            bool noIntPart = rng.Next(0, 5) == 0;
            return noIntPart
                ? $"{sign}.{decPart}"
                : $"{sign}{intPart}.{decPart}";
        }
    }
}
