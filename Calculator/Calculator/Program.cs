using System;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                int x = GetNumber("Пожалуйста, введите первое число");
                int y = GetNumber("Пожалуйста, введите второе число");
                Console.WriteLine("Пожалуйста, выберите команду: +, -, *, /, min, max");
                String cmd = Console.ReadLine();
                Console.WriteLine(GetResult(x, y, cmd));
                Console.WriteLine("Хотите ли вы продолжить? да/нет");
                string cont = Console.ReadLine();
                if (cont.ToLower().Contains("нет"))
                    break;
            } while (true);
        }

        static int GetMax(int a, int b) {
            int max = 0;
            if (a > b)
                max = a;
            else
                max = b;
            return max;
        }
        static int GetMin(int a, int b)
        {
            int min = 0;
            if (a > b)
                min = b;
            else
                min = a;
            return min;
        }
        static int GetNumber(string text) {
            Console.WriteLine(text);
            while (true) {
                string str = Console.ReadLine();
                int result;
                if (Int32.TryParse(str, out result))
                    return result;
                else
                    Console.WriteLine("Неправильный формат числа. Попробуйте ещё раз.");
            }
        }
        private static double GetResult(int x, int y, string cmd) {
            double result;
            switch (cmd) {
                case "+": 
                    result = x + y;
                    break;
                case "-": 
                    result = x - y;
                    break;
                case "*": 
                    result = x * y;
                    break;
                case "/":
                    result = (double) x / y;
                    break;
                case "min":
                    result = GetMin(x, y);
                    break;
                case "max":
                    result = GetMax(x, y);
                    break;
                default:
                    result = 0;
                    break;
            }
            return result;
        }
    }
}
