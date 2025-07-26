using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTestFramework.Elements
{
    public class BaseElement
    {
        protected readonly ILocator Locator;

        public BaseElement(ILocator locator)
        {
            Locator = locator;
        }

        public async Task ClickAsync()
        {
            await Locator.ClickAsync();
        }

        public async Task ShouldBeVisibleAsync()
        {
            await Locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        public async Task ShouldBeHiddenAsync()
        {
            await Locator.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        }

        public async Task ShouldHaveTextAsync(string expectedText)
        {
            await Expect(Locator).ToHaveTextAsync(expectedText);
        }

        public async Task<string> GetTextAsync()
        {
            return await Locator.InnerTextAsync();
        }

        public async Task<bool> IsVisibleAsync()
        {
            return await Locator.IsVisibleAsync();
        }

        public async Task<bool> IsEnabledAsync()
        {
            return await Locator.IsEnabledAsync();
        }

        public async Task HoverAsync()
        {
            await Locator.HoverAsync();
        }

        public async Task FocusAsync()
        {
            await Locator.FocusAsync();
        }

        public async Task WaitForAsync()
        {
            await Locator.WaitForAsync();
        }

        public async Task ScreenshotAsync(string path)
        {
            await Locator.ScreenshotAsync(new LocatorScreenshotOptions { Path = path });
        }

        public async Task<bool> ExistsAsync()
        {
            return await Locator.CountAsync() > 0;
        }
    }
}
