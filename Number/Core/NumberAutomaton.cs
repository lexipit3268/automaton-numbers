using System.Collections.Generic;

namespace Number.Core
{
    public class NumberAutomaton
    {
        private static readonly Dictionary<(State, InputType), State> TransitionTable = new Dictionary<(State, InputType), State>
        {
            { (State.q0, InputType.Sign), State.q1 },
            { (State.q0, InputType.Digit), State.q2 },
            { (State.q0, InputType.Dot), State.q3 },
            
            { (State.q1, InputType.Digit), State.q2 },
            { (State.q1, InputType.Dot), State.q3 },

            { (State.q2, InputType.Digit), State.q2 },
            { (State.q2, InputType.Dot), State.q4 },

            { (State.q3, InputType.Digit), State.q5 },
            { (State.q4, InputType.Digit), State.q5 },
            { (State.q5, InputType.Digit), State.q5 }
        };

        private static InputType GetInputType(char c)
        {
            if (char.IsDigit(c)) return InputType.Digit;
            if (c == '.') return InputType.Dot;
            if (c == '+' || c == '-') return InputType.Sign;
            return InputType.Invalid;
        }

        // Trả về Tuple gồm: trạng thái hợp lệ (bool) và thông tin loại số (string)
        public static (bool IsValid, string TypeInfo) AnalyzeNumber(string input)
        {
            if (string.IsNullOrEmpty(input)) return (false, "Không hợp lệ");

            State currentState = State.q0;

            foreach (char c in input)
            {
                InputType type = GetInputType(c);

                if (TransitionTable.TryGetValue((currentState, type), out State nextState))
                {
                    currentState = nextState;
                }
                else
                {
                    currentState = State.q_err;
                    break;
                }
            }

            if (currentState == State.q2 || currentState == State.q5)
            {
                // Phân tích chi tiết dựa vào chuỗi và trạng thái cuối
                bool isNegative = input.StartsWith("-");
                string signText = isNegative ? "âm" : "dương";
                
                // q2 là số nguyên, q5 là số thực
                string numberType = (currentState == State.q2) ? "Số nguyên" : "Số thực (hệ thập phân)";

                return (true, $"{numberType} {signText}");
            }

            return (false, "Không hợp lệ");
        }
    }
}
