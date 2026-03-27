using Microsoft.Playwright;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    public static class SelfHealingLocator
    {
        public static async Task<ILocator> ResolveAsync(IPage page, string selector, string? textOrAlt = null)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentException("Selector cannot be empty", nameof(selector));

            var fallbackText = textOrAlt;
            var candidates = fallbackText != null
                ? new[]
                {
                    selector,
                    $"text={fallbackText}",
                    $"[aria-label=\"{fallbackText}\"]",
                    $"xpath=//*[contains(text(), '{fallbackText}') ]"
                }
                : new[]
                {
                    selector,
                    $"text={selector}",
                    $"[aria-label=\"{selector}\"]"
                };

            var diagnostics = new StringBuilder();

            foreach (var candidate in candidates.Distinct())
            {
                diagnostics.AppendLine($"Trying selector: {candidate}");
                var locator = page.Locator(candidate);
                if (await locator.CountAsync() > 0)
                {
                    return locator;
                }
            }

            throw new Exception($"Self-healing locator failed for '{selector}'. Tried:\n{diagnostics}");
        }
    }
}
