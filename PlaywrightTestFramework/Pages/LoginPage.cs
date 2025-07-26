using Microsoft.Playwright;
using PlaywrightTestFramework.Elements;

namespace PlaywrightTestFramework.Pages
{
    public class LoginPage : BaseWorkspace
    {
        private readonly IPage _page;
        public LoginPage(IPage page) : base(page)
        {
            _page = page;
        }

        // Extracted selectors
        private const string FormSelector = "form";
        private const string OtpInputSelector = ".r4vIwl";
        private const string SubmitButtonSelector = "._1wE2Px";

        private static readonly (AriaRole role, string name) LoginButton = (AriaRole.Button, "Login");
        private static readonly (AriaRole role, string name) RequestOtpButton = (AriaRole.Button, "Request OTP");

        private Button GetLoginButton() => new Button(_page.GetByRole(LoginButton.role, new() { Name = LoginButton.name }));
        private Button GetRequestOtpButton() => new Button(_page.GetByRole(RequestOtpButton.role, new() { Name = RequestOtpButton.name }));

        public async Task LoginAsync(string username)
        {
            await _page.GotoAsync("https://www.flipkart.com/");
            var loginButton = GetLoginButton();
            await loginButton.ShouldBeVisibleAsync();
            await loginButton.ClickAsync();
            var form = _page.Locator(FormSelector).Filter(new() { HasText = "Enter Email/Mobile numberBy" });
            await form.GetByRole(AriaRole.Textbox).ClickAsync();
            await form.GetByRole(AriaRole.Textbox).FillAsync(username);
            await form.GetByRole(AriaRole.Textbox).PressAsync("Enter");
            var requestOtpButton = GetRequestOtpButton();
            await requestOtpButton.ShouldBeVisibleAsync();
            await requestOtpButton.ClickAsync();
        }

        public async Task EnterOtpAsync(string[] otpDigits)
        {
            await _page.Locator(OtpInputSelector).First.FillAsync(otpDigits[0]);
            for (int i = 1; i < otpDigits.Length; i++)
            {
                await _page.Locator($"div:nth-child({i + 1}) > {OtpInputSelector}").FillAsync(otpDigits[i]);
            }
            await _page.Locator(SubmitButtonSelector).ClickAsync();
        }
    }
}
