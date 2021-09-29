using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Скобки
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Введите строку для определения корректной посследовательности скобок:");
            do
            {
                string str_input = Console.ReadLine();
                string str_CheckOut = ThatStringIsEmpty(str_input);


                if (str_CheckOut.IndexOf("Начинаю") >= 0)
                {
                    bool bool_result = CheckAll(str_input);
                    if (bool_result)
                    {
                        Console.WriteLine ("ШЕФ! Всё совпало!\nХотите повторить снова? y/n");
                    }
                    else
                    {
                        Console.WriteLine ("Несоответствие скобок!\nХотите повторить снова? y/n");
                    }
                }
                else
                {
                    Console.WriteLine(str_CheckOut);
                }

            }
            while (Console.ReadKey(true).Key == ConsoleKey.Y);
        }


        static bool CheckAll (string str_inputString)
        {
            Stack<char> stack_Chars = new Stack<char>();

            // Цикл проверка каждого символа во входящей строке
            foreach (char c in str_inputString)
            {
                switch(c)
                {
                    //Определяем есть ли открытая скобка
                    case '(':
                    case '[':
                    case '{':
                        // Если есть открытая - закидываем в стэк
                        stack_Chars.Push(c);
                        break;

                    case ')':
                        // Если стэк пустой или у нас нет открытой скобки, то False
                        if (stack_Chars.Count > 0 && stack_Chars.Peek() == '(')
                        {
                            stack_Chars.Pop();  //Удаляем из стэка элемент
                            break;
                        }
                        return false;


                    case ']':
                        // Если стэк пустой или у нас нет открытой скобки, то False
                        if (stack_Chars.Count > 0 || stack_Chars.Peek() == '[')
                        {
                            stack_Chars.Pop();  //Удаляем из стэка элемент
                            break;
                        }
                        return false;


                    case '}':
                        // Если стэк пустой или у нас нет открытой скобки, то False
                        if (stack_Chars.Count > 0 || stack_Chars.Peek() == '{')
                        {
                            stack_Chars.Pop();  //Удаляем из стэка элемент
                            break;
                        }
                        return false;
                    default:
                        break;
                }
            }
            //Возвращаем TRUE, если после обработки кол-во элементов в стэке = 0
            return stack_Chars.Count == 0;
        }

        static string ThatStringIsEmpty (string str_inputString)
        {

            if (str_inputString.Length <= 0) 
            {
                return "Вы ввели пустую строку!\nХотите повторить снова? y/n";
            }
            else
        
                if(str_inputString.IndexOf("(") >= 0 || str_inputString.IndexOf("[") >= 0  || str_inputString.IndexOf("{") >= 0 || str_inputString.IndexOf(")") >= 0 || str_inputString.IndexOf("]") >= 0 || str_inputString.IndexOf("}") >= 0)
                {
                    return "Начинаю обработку\n";

                }
                else
                {
                    return "Ни одной скобки не найдено! \nХотите повторить снова? y/n";
                }
            }
        }

    }

