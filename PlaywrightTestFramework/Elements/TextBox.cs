using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTestFramework.Elements
{
    public class TextBox : BaseElement
    {
        public TextBox(ILocator locator) : base(locator) { }

        public async Task SetTextAsync(string text)
        {
            await Locator.FillAsync(text);
        }

        public new async Task<string> GetTextAsync()
        {
            return await Locator.InputValueAsync();
        }

        public async Task ClearAsync()
        {
            await Locator.FillAsync("");
        }

        public async Task AppendTextAsync(string text)
        {
            var current = await Locator.InputValueAsync();
            await Locator.FillAsync(current + text);
        }

        public async Task ShouldHavePlaceholderAsync(string expected)
        {
            await Expect(Locator).ToHaveAttributeAsync("placeholder", expected);
        }

        public async Task ShouldBeReadOnlyAsync()
        {
            await Expect(Locator).ToHaveAttributeAsync("readonly", "");
        }
    }
}
