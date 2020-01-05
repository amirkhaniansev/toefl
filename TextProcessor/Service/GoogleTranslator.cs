using System;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;

namespace Toefl.TextProcessor.Service
{
    public abstract class GoogleTranslator : ITranslator
    {
        protected readonly IHtmlParser parser;
        protected readonly string url;

        protected bool disposed;

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
            this.Dispose(true);
        }

        public async Task<TranslationResult> TranslateAsync(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentException(nameof(expression));

            var content = await this.GetContentAsync(expression);
            var result = this.parser.Parse(content);
            result.Expression = expression;

            return result;
        }

        protected abstract void Dispose(bool disposing);
        protected abstract Task<string> GetContentAsync(string expression); 
    }
}