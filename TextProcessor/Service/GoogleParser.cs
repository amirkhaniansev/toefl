using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;

namespace Toefl.TextProcessor.Service
{
    public class GoogleParser : IHtmlParser
    {
        public TranslationResult Parse(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var node = document.DocumentNode.Descendants()
                .Where(n => n.HasClass("tlid - translation translation"))
                .FirstOrDefault();

            return new TranslationResult();
        }
    }
}
