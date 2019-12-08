using System;
using System.Text;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;
using PuppeteerSharp;

namespace Toefl.TextProcessor.Service
{
    public class GoogleTranslator : ITranslator
    {
        private readonly IHtmlParser parser;
        private readonly BrowserFetcher browserFetcher;
        private readonly NavigationOptions navigationOptions;
        private readonly string url;

        private RevisionInfo revisionInfo;
        private LaunchOptions launchOptions;
        private Browser browser;
        private bool disposed;

        public GoogleTranslator(IHtmlParser parser, string url)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException(nameof(url));

            this.parser = parser;
            this.url = url;
            this.browserFetcher = new BrowserFetcher();
            this.navigationOptions = new NavigationOptions
            {
                WaitUntil = new []
                {
                    WaitUntilNavigation.Networkidle0,
                    WaitUntilNavigation.Networkidle2
                }
            };
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public async Task<TranslationResult> TranslateAsync(string expression)
        {
            if (this.revisionInfo == null)
                this.revisionInfo = await this.browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

            if (this.launchOptions == null)
                this.launchOptions = new LaunchOptions();

            if (this.browser == null)
                this.browser = await Puppeteer.LaunchAsync(this.launchOptions);

            var page = await this.browser.NewPageAsync();
            var finalUrl = new StringBuilder()
                    .Append(this.url)
                    .Append("&text=")
                    .Append(expression)
                    .ToString();

            var response = await page.GoToAsync(finalUrl, this.navigationOptions);
            var jsHandle = await page.WaitForSelectorAsync(".result-shield-container");
            if (!response.Ok)
                throw new Exception();
           
            var content = await page.GetContentAsync();

            await page.CloseAsync();

            var result = this.parser.Parse(content);
            result.Expression = expression;

            return result;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if(disposing)
            {
                this.browser.Dispose();
            }

            this.disposed = true;
        }
    }
}