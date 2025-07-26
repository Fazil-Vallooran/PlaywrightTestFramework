using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTestFramework.Elements
{
    public class Dropdown : BaseElement
    {
        public Dropdown(ILocator locator) : base(locator) { }

        public async Task SelectByValueAsync(string value)
        {
            await Locator.SelectOptionAsync(new SelectOptionValue { Value = value });
        }

        public async Task SelectByLabelAsync(string label)
        {
            await Locator.SelectOptionAsync(new SelectOptionValue { Label = label });
        }

        public async Task<string[]> GetSelectedValuesAsync()
        {
            return await Locator.EvaluateAsync<string[]>("el => Array.from(el.selectedOptions).map(o => o.value)");
        }

        public async Task ShouldHaveSelectedValueAsync(string expected)
        {
            await Expect(Locator).ToHaveValueAsync(expected);
        }
    }
}
