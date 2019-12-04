using Toefl.TextProcessor.Models;

namespace Toefl.TextProcessor.Service
{
    public interface IHtmlParser
    {
        TranslationResult Parse(string html);
    }
}