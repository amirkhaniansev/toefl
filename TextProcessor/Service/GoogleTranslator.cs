using System;
using System.Text;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;
using PuppeteerSharp;
using PuppeteerSharp.Input;

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
        private Page page;
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

            if (this.page == null)
            {
                this.page = await this.browser.NewPageAsync();
                var response = await this.page.GoToAsync(this.url, this.navigationOptions);
                if (!response.Ok)
                    throw new Exception();
            }

            await this.page.Keyboard.DownAsync("Control");
            await this.page.Keyboard.PressAsync("KeyA");
            await this.page.Keyboard.UpAsync("Control");
            await this.page.Keyboard.DownAsync("Backspace");
            await this.page.TypeAsync("textarea[id=source]", expression);
            
            var jsHandle = await page.WaitForSelectorAsync(".result-shield-container");
            var content = await this.page.GetContentAsync();

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
                if (this.page != null)
                    this.page.Dispose();

                if (this.browser != null)
                    this.browser.Dispose();
            }

            this.disposed = true;
        }
    }
}