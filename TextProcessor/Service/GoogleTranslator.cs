using System;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;

namespace Toefl.TextProcessor.Service
{
    public class GoogleTranslator : ITranslator
    {
        private readonly IHtmlParser parser;
        private readonly string url;

        public GoogleTranslator(IHtmlParser parser, string url)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException(nameof(url));

            this.parser = parser;
            this.url = url;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<TranslationResult> TranslateAsync(string expression)
        {
            throw new NotImplementedException();
        }
    }
}