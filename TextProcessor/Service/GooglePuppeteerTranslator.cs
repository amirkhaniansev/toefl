using System;
using System.Text;
using System.Threading.Tasks;
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
                this.page = await this.browser.NewPageAsync();

            var finalUrl = new StringBuilder()
                    .Append(this.url)
                    .Append("&text=")
                    .Append(expression)
                    .ToString();

            var response = await this.page.GoToAsync(finalUrl, this.navigationOptions);
            if (!response.Ok)
                throw new Exception();

            var jsHandle = await this.page.WaitForSelectorAsync(".result-shield-container");            
            var content  = await this.page.GetContentAsync();
            var back = await this.page.GoBackAsync();

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