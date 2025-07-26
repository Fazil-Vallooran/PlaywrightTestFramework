using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTestFramework.Pages
{
    public class BaseWorkspace
    {
        public BaseWorkspace(IPage page)
        {
            Page = page;
        }

        public IPage Page { get; }
    }
}
