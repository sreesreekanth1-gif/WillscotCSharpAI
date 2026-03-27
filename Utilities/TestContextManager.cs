using Microsoft.Playwright;
using System;
using System.Threading;

namespace Framework.Utilities
{
    public static class TestContextManager
    {
        private static readonly AsyncLocal<IPage?> _currentPage = new();
        private static readonly AsyncLocal<IBrowserContext?> _currentContext = new();

        public static bool HasPage => _currentPage.Value != null;
        public static bool HasBrowserContext => _currentContext.Value != null;

        public static IPage Page
        {
            get => _currentPage.Value ?? throw new InvalidOperationException("Page is not set for this context.");
            set => _currentPage.Value = value;
        }

        public static IBrowserContext BrowserContext
        {
            get => _currentContext.Value ?? throw new InvalidOperationException("BrowserContext is not set for this context.");
            set => _currentContext.Value = value;
        }

        public static void Clear()
        {
            _currentPage.Value = null;
            _currentContext.Value = null;
        }
    }
}