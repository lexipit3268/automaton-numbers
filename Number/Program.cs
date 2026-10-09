using System;
using Number.Core;

namespace Number
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== CHƯƠNG TRÌNH KIỂM TRA SỐ (DFA) ===");
            Console.WriteLine("Nhập chuỗi cần kiểm tra (hoặc 'exit' để thoát):");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                
                if (input == null || input.Trim().ToLower() == "exit") break;

                var result = NumberAutomaton.AnalyzeNumber(input);

                if (result.IsValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[Hợp lệ] '{input}' là {result.TypeInfo}.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Không hợp lệ] '{input}' không phải là số hợp lệ theo DFA.");
                }
                Console.ResetColor();
            }
        }
    }
}
