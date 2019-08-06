using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;

namespace Selenium.Axe.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private const string TargetTestUrl = "https://www.google.ca/";

        [TestCleanup]
        public virtual void TearDown()
        {
            _webDriver?.Quit();
            _webDriver?.Dispose();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DataRow("Chrome")]
        //[DataRow("Firefox")]
        public void TestAnalyzeTarget(string browser)
        {
            this.InitDriver(browser);
            _webDriver.Navigate().GoToUrl(TargetTestUrl);
            // wait for email input box is found
            _wait.Until(drv => drv.FindElement(By.XPath("//input[@title='Search']")));
            AxeResult results = _webDriver.Analyze();
            results.Should().NotBeNull(nameof(results));
        }

        private void InitDriver(string browser)
        {
            switch (browser.ToUpper())
            {
                case "CHROME":
                    ChromeOptions options = new ChromeOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };
                    options.AddArgument("no-sandbox");
                    options.AddArgument("--log-level=3");
                    options.AddArgument("--silent");

                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
                    service.SuppressInitialDiagnosticInformation = true;
                    var chromeDriverDirectory = Environment.GetEnvironmentVariable("chromedriver");
                    _webDriver = new ChromeDriver(chromeDriverDirectory ?? Environment.CurrentDirectory, options);
                    
                    break;

                case "FIREFOX":
                    var geckoDriverDirectory = Environment.GetEnvironmentVariable("geckodriver");
                    _webDriver = new FirefoxDriver(geckoDriverDirectory ?? Environment.CurrentDirectory);
                    break;

                default:
                    throw new ArgumentException($"Remote browser type '{browser}' is not supported");

            }

            _wait = new WebDriverWait(_webDriver, TimeSpan.FromMinutes(4));
            _webDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(3);
            _webDriver.Manage().Window.Maximize();
        }
    }
}
