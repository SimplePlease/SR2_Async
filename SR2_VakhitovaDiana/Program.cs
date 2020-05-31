using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
/// <summary>
/// Самостоятельная работа 2, асинхронность, Вахитова Диана БПИ 199.
/// </summary>
namespace SR2_VakhitovaDiana
{
    class Program
    {
      
        /// <summary>
        /// Константы.
        /// </summary>
        public const string directoryName = "../../../Книги";   // Папка, откуда берутся файлы для обработки.
        public const string directorySync = "../../../NewSync"; // Папка, куда записываются файлы после выполнения синхронного метода.
        public const string directoryAsync = "../../../NewAsync";   // Папка, в которую записываются файлы после выполнения асинхронного метода. 
        public const string webAsync = "../../../new_book_from_web.txt";    // Путь, по которому сохраняется книга из сети.
        public static string link = "https://www.gutenberg.org/files/1342/1342-0.txt";  // Ссылка, откуда скачиваем книгу.

        /// <summary>
        /// Чтение, изменение, запись книг из папки синхронно.
        /// </summary>
        static void SyncPart()
        {
            // Удаление директории, если она была создана. Создание дииректории.
            try
            {
                if (Directory.Exists(directorySync))
                {
                    Directory.Delete(directorySync, true);
                }
                Directory.CreateDirectory(directorySync);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Работа синхронного метода.");
            Console.ForegroundColor = ConsoleColor.White;

            // Запуск таймера, изменение содержимого книг.
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                foreach (String path in Directory.GetFiles(directoryName))
                {
                    BookWork.ChangeBook(path, true);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            timer.Stop();

            // Вывод инфы о проделанной операции.
            string syncTime = String.Format("{0:00}.{1:000}", timer.Elapsed.Seconds, timer.Elapsed.Milliseconds);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Не асинхронная обработка заняла: " + syncTime + "секунд.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Чтение, изменение, запись книг из папки асинхронно.
        /// </summary>
        public static async Task AsyncPart()
        {
            // Удаление директории, если она была создана. Создание дииректории.
            try
            {
                if (Directory.Exists(directoryAsync))
                {
                    Directory.Delete(directoryAsync, true);
                }
                Directory.CreateDirectory(directoryAsync);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Stopwatch timer = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Работа асинхронного метода.");
            Console.ForegroundColor = ConsoleColor.White;

            // Запуск таймера, изменение содержимого книг.
            timer.Start();
            await Task.WhenAll(Directory.GetFiles(directoryName).Select(file => Task.Run(() => BookWork.ChangeBook(file, false))));
            timer.Stop();

            // Вывод инфы о проделанной операции.
            Console.ForegroundColor = ConsoleColor.Yellow;
            string asyncTime = String.Format("{0:0}.{1:000}", timer.Elapsed.Seconds, timer.Elapsed.Milliseconds);
            Console.WriteLine("Асинхронная обработка заняла: " + asyncTime + " секунд.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        
        /// <summary>
        /// Перезапись книги по ссылке. 
        /// </summary>
        /// <returns></returns>
        public static async Task AsyncWebPart()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Bеб-часть.");
            Console.ForegroundColor = ConsoleColor.White;

            /// Получение контента по ссылке.
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage bookText = await client.GetAsync(link))
                {
                    /// Всё тот же алгоритм, что и для обработки обычного текста.
                    if (bookText.IsSuccessStatusCode)
                    {
                        BookWork.ChangeBookWeb(bookText);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при чтении инфы по ссылке.");
                    }
                }
            }
            
        }

        static async Task Main(string[] args)
        {
            // Синхронный метод перезаписи.
            SyncPart();
            Console.WriteLine(Environment.NewLine);

            // асинхронный метод перезаписи.
            await AsyncPart();
            Console.WriteLine(Environment.NewLine);

            // Асинхронный метод перезаписи по ссылке.
            await AsyncWebPart();
            Console.Read();
        }
    }
}
