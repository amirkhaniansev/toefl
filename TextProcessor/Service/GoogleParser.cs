using System.Linq;
using Toefl.TextProcessor.Models;
using HtmlAgilityPack;

namespace Toefl.TextProcessor.Service
{
    public class GoogleParser : IHtmlParser
    {
        public TranslationResult Parse(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var result = new TranslationResult();
            
            var translationNode = document.DocumentNode.Descendants()
                .Where(n => n.Name == "span" && n.HasClass("tlid-translation"))
                .FirstOrDefault();
            if (translationNode != null)
                result.MainTranslation = translationNode.InnerText;

            var explanationNode = document.DocumentNode.Descendants()
                .Where(n => n.Name == "div" && n.HasClass("gt-def-row"))
                .FirstOrDefault();
            if (explanationNode != null)
                result.Explanation = explanationNode.InnerText;

            var synonymNodes = document.DocumentNode.Descendants()
                .Where(n => n.Name == "span" && n.HasClass("gt-cd-cl"))
                .ToList();
            if (synonymNodes != null && synonymNodes.Count > 0)
                result.Synonyms = synonymNodes.Select(n => n.InnerText).Distinct().ToList();

            return result;
        }
    }
}