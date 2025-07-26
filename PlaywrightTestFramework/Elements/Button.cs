using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTestFramework.Elements
{
    public class Button : BaseElement
    {
        public Button(ILocator locator) : base(locator) { }

        public async Task DoubleClickAsync() => await Locator.DblClickAsync();

        public async Task ClickIfVisibleAsync()
        {
            if (await Locator.IsVisibleAsync())
                await Locator.ClickAsync();
        }

        public async Task ClickWithDelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
            await Locator.ClickAsync();
        }

        public async Task<bool> IsDisabledAsync() => !await Locator.IsEnabledAsync();

        public async Task ShouldBeEnabledAsync() => await Expect(Locator).ToBeEnabledAsync();

        public async Task ShouldBeDisabledAsync() => await Expect(Locator).ToBeDisabledAsync();

        public async Task<string?> GetButtonTypeAsync() => await Locator.GetAttributeAsync("type");
    }
}