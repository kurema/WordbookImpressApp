using System;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Process();
            Console.ReadLine();

        }

        static async void Process()
        {
            var (wordbook, html, data) = await WordbookImpressLibrary.Models.Wordbook.Load(new Uri(@"https://impress.quizgenerator.net/impress/01impress/"), new WordbookImpressLibrary.Models.Authentication() { UserName = "impQG001", Password = "derugokuFE01" });
        }
    }
}
