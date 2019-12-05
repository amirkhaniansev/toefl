using System.Threading.Tasks;
using Toefl.TextProcessor.Service;

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
                var result = await translator.TranslateAsync("great");
            }
        }
    }
}
