using System;
using System.Threading.Tasks;
using Toefl.TextProcessor.Service;
using Newtonsoft.Json;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var parser = new GoogleParser();
            var url = "https://translate.google.com/#view=home&op=translate&sl=en&tl=hy";
            using (var translator = new GoogleTranslator(parser, url))
            {
                while(true)
                {
                    Console.Write("Text : ");
                    var input = Console.ReadLine();
                    if (input == "exit")
                        break;

                    var result = await translator.TranslateAsync(input);

                    Console.WriteLine("Result : ");
                    Console.WriteLine($"Translation : {result.MainTranslation}");
                    Console.WriteLine($"Explanation : {result.Explanation}");

                    result.Synonyms.ForEach(s => Console.WriteLine($"Synonym : {s}"));
                }
            }
        }
    }
}
