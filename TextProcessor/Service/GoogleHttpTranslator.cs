using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Toefl.TextProcessor.Service
{
    public class GoogleHttpTranslator : GoogleTranslator
    {
        private readonly HttpClient client;
        private readonly StringBuilder builder;

        public GoogleHttpTranslator(IHtmlParser parser, string url) : base(parser, url)
        {
            this.client = new HttpClient();
            this.builder = new StringBuilder();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
                this.client.Dispose();

            this.disposed = true;
        }

        protected override async Task<string> GetContentAsync(string expression)
        {
            if (this.builder.Length != 0)
                this.builder.Clear();

            var finalUrl = this.builder
                    .Append(this.url)
                    .Append("&text=")
                    .Append(expression)
                    .ToString();

            var response = await this.client.GetAsync(finalUrl);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception();

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}