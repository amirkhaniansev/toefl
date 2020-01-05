using System;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;
using PuppeteerSharp;

namespace Toefl.TextProcessor.Service
{
    public class GooglePuppeteerTranslator : GoogleTranslator
    {
        private readonly BrowserFetcher browserFetcher;
        private readonly NavigationOptions navigationOptions;
        
        private RevisionInfo revisionInfo;
        private LaunchOptions launchOptions;
        private Browser browser;
        private Page page;
        
        public GooglePuppeteerTranslator(IHtmlParser parser, string url) : base(parser, url)
        {
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

        protected override async Task<string> GetContentAsync(string expression)
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
            var content  = await this.page.GetContentAsync();

            return content;
        }

        protected override void Dispose(bool disposing)
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