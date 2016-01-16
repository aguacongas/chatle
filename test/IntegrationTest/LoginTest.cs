using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest
{
	[Collection(ChatLeCollectionFixture.Definition)]
    public class LoginTest
    {
		public IWebDriver GetDriver(string browser)
		{
			switch(browser)
			{
				case "ie":
					return new InternetExplorerDriver("../../packages/Selenium.WebDriver.IEDriver/driver");
				case "firefox":
					return new FirefoxDriver(new FirefoxBinary(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"), new FirefoxProfile());
				case "chrome":
					return new ChromeDriver("../../packages/Selenium.WebDriver.ChromeDriver/driver");
			}

			throw new ArgumentOutOfRangeException("unknow browser");
		}

		[Theory]
		[InlineData("ie")]
		[InlineData("firefox")]
		[InlineData("chrome")]
		public void DefaultPage_should_redirect_to_login(string browser)
		{
			using (var driver = GetDriver(browser))
			{
				driver.Navigate().GoToUrl("http://localhost:5000");
				Assert.Equal(driver.Url, "http://localhost:5000/Account");
			}
		}

		[Theory]
		[InlineData("ie")]
		[InlineData("firefox")]
		[InlineData("chrome")]
		public void loginPage_should_redirect_to_Home(string browser)
		{
			using (var driver = GetDriver(browser))
			{
				driver.Navigate().GoToUrl("http://localhost:5000/Account");
				var forms = driver.FindElements(By.XPath("//form"));
				foreach(var form in forms)
				{
					if (form.GetAttribute("action").EndsWith("/Account/Guess"))
					{
						var inputUserName = form.FindElement(By.Id("UserName"));
						inputUserName.SendKeys("test" + browser);
						form.Submit();
						Assert.Equal(driver.Url, "http://localhost:5000/");
					}
				}
			}
		}
	}
}
