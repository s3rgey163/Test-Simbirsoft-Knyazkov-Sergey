using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();
            bool exit = false;
            while (!exit)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        DownloadPage();
                        Menu();
                        break;
                    case ConsoleKey.D2:
                        Unique();
                        Menu();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
            
        }


        private static void Unique()
        {
            Console.Clear();
            Console.Write("Введите адрес HTML страницы: ");
          
            string url = Console.ReadLine();
            if (url.StartsWith("www"))
                url = url.Insert(0, "https://");
            Console.WriteLine();

            HtmlProc html = new HtmlProc();
            string[] words = null;
            bool tryToGet = true;
            while (tryToGet && !html.GetAllWords(url,out words))
            {
                Console.WriteLine("Ошибка: '{0}' {1}", url, Logs.GetLastError);
                Console.WriteLine("Для повтора, нажмите Y. Для того, чтобы выйти в меню, любую другую клавишу.");

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    Console.Write("Введите адрес HTML страницы: ");
                    url = Console.ReadLine();
                    Console.Clear();
                }
                else
                {
                    tryToGet = false;
                }

            }
            if (tryToGet)
            {
                Console.WriteLine("Слова успешно загружены.");
                Dictionary<string, int> pairs = html.GetPairs(words);
                PrintPairs(pairs);
                Console.ReadKey();
            }
        }

        private static void DownloadPage()
        {
            Console.Clear();
            Console.Write("Введите адрес HTML страницы: ");
            string url = Console.ReadLine();
            if (url.StartsWith("www"))
                url = url.Insert(0, "https://");
            Console.WriteLine();

            HtmlProc html = new HtmlProc();
            bool tryDownload = true;
            string page = null;
            while (tryDownload && !html.GetHtmlPage(url,out page))
            {
                Console.WriteLine("Ошибка: '{0}' {1}", url, Logs.GetLastError);
                Console.WriteLine("Для повтора, нажмите Y. Для того, чтобы выйти в меню, любую другую клавишу.");

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    Console.Write("Введите адрес HTML страницы: ");
                    url = Console.ReadLine();
                    Console.Clear();
                }
                else
                {
                    tryDownload = false;
                }

            }
            if (tryDownload)
            {
                FileManager fPage = new FileManager(HtmlProc.GetName(url) + ".html");
                fPage.WriteFile(page, FileManager.WriteFileOptions.REWRITE_ALWAYS);
                Console.WriteLine("HTML страница успешна скачана.\n{0}",fPage.GetPath);
                Console.ReadKey();
            }
        }

        private static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Выберите пункт: \n 1 - Скачать HTML страницу.\n " +
                "2 - Показать статистику по количеству уникальных слов. \n Для выхода нажмите ESC.");
        }

        private static void PrintPairs(Dictionary<string, int> pairs)
        {
            for(int i = 0; i < pairs.Count;i++)
            {
                Console.WriteLine("{0}) {1} - {2}", i + 1, pairs.ElementAt(i).Key, pairs.ElementAt(i).Value);
            }
        }
    }
}
