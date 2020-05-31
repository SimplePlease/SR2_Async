using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SR2_VakhitovaDiana
{
    public class BookWork
    {
        /// <summary>
        /// Словарь для замены букв.
        /// </summary>
        static Dictionary<char, string> transliteration = new Dictionary<char, string>
        { {'A', "А"}, {'B',"Б" }, {'V',"В" }, {'G',"Г" }, {'D',"Д" }, {'E',"Е" }, {'J',"Ж" },
          {'Z', "З"}, {'I',"И" }, {'K',"К" }, {'L',"Л" }, {'M',"М" }, {'N',"Н" }, {'O',"О" },
          {'P', "П"}, {'R',"Р" }, {'S',"С" }, {'T',"Т" }, {'U',"У" }, {'F',"Ф" }, {'H',"Х" },
          {'C', "Ц"}, {'Q',"КУ" }, {'W',"У" }, {'X',"КС" }, {'Y',"Ы"},

          {'a', "а"}, {'b',"б" }, {'v',"в" }, {'g',"г" }, {'d',"д" }, {'e',"е" }, {'j',"ж" },
          {'z', "з"}, {'i',"и" }, {'k',"к" }, {'l',"л" }, {'m',"м" }, {'n',"н" }, {'o',"о" },
          {'p', "п"}, {'r',"р" }, {'s',"с" }, {'t',"т" }, {'u',"у" }, {'f',"ф" }, {'h',"х" },
          {'c', "ц"}, {'q',"ку" }, {'w',"у" }, {'x',"кс" }, {'y',"ы"},
        };

        /// <summary>
        /// Метод, который заменяет буквы в книге, добытой по ссылке.
        /// </summary>
        /// <param name="content"> Содержимое страницы. </param>
        public static void ChangeBookWeb(HttpResponseMessage content)
        {
            // Создание ттаймера, финальной строки и получение строки содержимого страницы.
            Stopwatch timer = new Stopwatch();
            StringBuilder result = new StringBuilder();
            string readBook = content.Content.ReadAsStringAsync().Result;

            timer.Start();  // Запуск таймера, изменение текста.
            for (int i = 0; i < readBook.Length; i++)
            {
                result.Append((transliteration.ContainsKey(readBook[i])) ? transliteration[readBook[i]]
                          : (Char.IsLetter(readBook[i])) ? "" : readBook[i].ToString());
            }
            timer.Stop(); // Останавливаем таймер.
            TimeSpan ts = timer.Elapsed;

            // Вывод информации об изменении книги на консоль.
            lock (Console.Out)
            {
                string elapsedTime = String.Format("{0:00}.{1:000}", ts.Seconds, ts.Milliseconds);

                Console.WriteLine($"{Program.webAsync.Split('/').Last()}: количество символов до {readBook.Length}," +
                $" количество символов после {result.Length}. Заняло: {elapsedTime} "  + "секунд.");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Конец.");
            Console.ForegroundColor = ConsoleColor.White;

            // Запись итогового текста в файл.
            using (StreamWriter writer = new StreamWriter(Program.webAsync, false))
            {
                writer.Write(result);
            }
        }

        /// <summary>
        /// Метод, который изменяет содержимое пути и сохраняет. Если метод синхронный, то сохраняет в одну папку, если вызвано и асинхронного метода, то в другую.
        /// </summary>
        /// <param name="path"> Путь, где лежит книга. </param>
        /// <param name="sync"> Определяет тип метода, из которого вызывается. </param>
        public static void ChangeBook(string path, bool sync)
        {
            // Создание ттаймера, финальной строки и получение строки содержимого страницы.
            StringBuilder result = new StringBuilder();
            string readBook;
            Stopwatch timer = new Stopwatch();

            timer.Start();  // Запуск таймера.

            // Чтение книиги.
            using (StreamReader reader = new StreamReader(path))
            {
                readBook = reader.ReadToEnd();
            }

            // Изменение содержимого книги.
            for (int i = 0; i < readBook.Length; i++)
            {
                result.Append((transliteration.ContainsKey(readBook[i])) ? transliteration[readBook[i]]
                          : (Char.IsLetter(readBook[i])) ? "" : readBook[i].ToString());
            }

            // Запись получившейся книги. 
            using (StreamWriter writer = new StreamWriter(NewBookName(path, sync), false))
            {
                writer.Write(result);
            }

            timer.Stop();   // Остановка таймера.
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}.{1:000}", ts.Seconds, ts.Milliseconds);

            // Вывод информации о книге.
            Console.WriteLine($"{path.Split('\\').Last()}: количество символов до {readBook.Length}," +
                $" количество символов после {result.Length}. Заняло: {elapsedTime} " + "секунд.");
        }

        /// <summary>
        /// Метод, который переименовывает НАЗВАНИЕ ФАЙЛА. В зависимости от того, 
        /// синхронный или асинхронный метод обрабатывает файлы, сохраняет в разные папки.
        /// </summary>
        /// <param name="oldName"> Старое название файла. </param>
        /// <param name="sync"> Определяет, в какой каталог сохранить.</param>
        /// <returns></returns>
        private static string NewBookName(string oldName, bool sync)
        {
            return sync ? $"{Program.directorySync}/new_" + oldName.Split('\\').Last() : $"{Program.directoryAsync}/new_" + oldName.Split('\\').Last();
        }
    }
}
