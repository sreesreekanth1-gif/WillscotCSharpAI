using Framework.Configuration;
using Framework.Pages;
using Framework.Tests;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Framework.Tests.WebTests
{
    [TestFixture]
    public class InvalidLoginTests : BaseTest
    {
        private LoginPage _loginPage = null!;

        [SetUp]
        public new async Task SetUpAsync()
        {
            await base.SetUpAsync();
            _loginPage = new LoginPage(Page!);
        }

        /// <summary>
        /// TC-VAL-01: Empty Field Validation
        /// Leave email and password blank, click LOG IN.
        /// Expected: "Email can't be blank" and "Password can't be blank" errors appear.
        /// </summary>
        [Test]
        [Category("DryRun")]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_01_EmptyFieldValidation()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.ClickLoginAsync();

                var emailError    = await _loginPage.GetEmailErrorAsync();
                var passwordError = await _loginPage.GetPasswordErrorAsync();

                await StepWithScreenshotAsync("Assert email required error is shown", () =>
                    Assert.That(emailError, Does.Contain("blank").Or.Contain("required").IgnoreCase));

                await StepWithScreenshotAsync("Assert password required error is shown", () =>
                    Assert.That(passwordError, Does.Contain("blank").Or.Contain("required").IgnoreCase));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-02: Invalid Email Format (no @ symbol)
        /// Email: sreekanth.simhadri.com | Password: Admin@123
        /// Expected: "Please enter a valid email address" message.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_02_InvalidEmailFormat()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("sreekanth.simhadri.com");
                await _loginPage.EnterPasswordAsync("Admin@123");
                await _loginPage.ClickLoginAsync();

                var emailError = await _loginPage.GetEmailErrorAsync();

                await StepWithScreenshotAsync("Assert invalid email format error is shown", () =>
                    Assert.That(emailError, Does.Contain("invalid").Or.Contain("valid email").IgnoreCase));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-03: Missing Domain Extension
        /// Email: testuser@willscot | Password: Pass@123
        /// Expected: Validation error about valid email.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_03_MissingDomainExtension()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("testuser@willscot");
                await _loginPage.EnterPasswordAsync("Pass@123");
                await _loginPage.ClickLoginAsync();

                var emailErrorVisible  = await _loginPage.IsEmailErrorVisibleAsync();
                var globalErrorVisible = await _loginPage.IsGlobalErrorVisibleAsync();

                await StepWithScreenshotAsync("Assert validation error is triggered for missing domain extension", () =>
                    Assert.That(emailErrorVisible || globalErrorVisible, Is.True,
                        "Expected an error message for missing domain extension"));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-04: Unregistered Account
        /// Email: unknown_user_99@test.com | Password: AnyPass123
        /// Expected: "Wrong email or password" / "Invalid email or password" error.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_04_UnregisteredAccount()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("unknown_user_99@test.com");
                await _loginPage.EnterPasswordAsync("AnyPass123");
                await _loginPage.ClickLoginAsync();

                var globalError = await _loginPage.GetGlobalErrorAsync();

                await StepWithScreenshotAsync("Assert unregistered account error is shown", () =>
                    Assert.That(globalError, Does.Contain("wrong email or password").IgnoreCase
                        .Or.Contain("blocked").IgnoreCase));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-05: Incorrect Password
        /// Email: valid.user@willscot.com | Password: WrongPassword77
        /// Expected: "Invalid email or password" / "Wrong email or password" error.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_05_IncorrectPassword()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("valid.user@willscot.com");
                await _loginPage.EnterPasswordAsync("WrongPassword77");
                await _loginPage.ClickLoginAsync();

                var globalError = await _loginPage.GetGlobalErrorAsync();

                await StepWithScreenshotAsync("Assert incorrect password error is shown", () =>
                    Assert.That(globalError, Does.Contain("wrong email or password").IgnoreCase
                        .Or.Contain("blocked").IgnoreCase));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-06: Case Sensitivity (Password)
        /// Email: valid.user@willscot.com | Password: WELCOME123 (actual: welcome123)
        /// Expected: Login fails with invalid credentials error.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]    
        public async Task TC_VAL_06_CaseSensitivePassword()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("valid.user@willscot.com");
                await _loginPage.EnterPasswordAsync("WELCOME123");
                await _loginPage.ClickLoginAsync();

                var globalError = await _loginPage.GetGlobalErrorAsync();

                await StepWithScreenshotAsync("Assert login fails due to case-sensitive password mismatch", () =>
                    Assert.That(globalError, Does.Contain("wrong email or password").IgnoreCase
                        .Or.Contain("blocked").IgnoreCase));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-07: SQL Injection Attempt
        /// Email: ' OR 1=1 -- | Password: any
        /// Expected: System rejects input; no bypass or database error.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_07_SqlInjectionAttempt()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("' OR 1=1 --");
                await _loginPage.EnterPasswordAsync("any");
                await _loginPage.ClickLoginAsync();

                var emailErrorVisible  = await _loginPage.IsEmailErrorVisibleAsync();
                var globalErrorVisible = await _loginPage.IsGlobalErrorVisibleAsync();

                await StepWithScreenshotAsync("Assert SQL injection input is rejected and no bypass occurs", () =>
                    Assert.That(emailErrorVisible || globalErrorVisible, Is.True,
                        "Expected system to reject SQL injection input with an error message"));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-08: Leading/Trailing Spaces in Email
        /// Email: "  user@willscot.com  " | Password: ValidPass123
        /// Expected: System trims spaces and attempts login (shows credentials error), OR shows a format error.
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_08_LeadingTrailingSpacesInEmail()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("  user@willscot.com  ");
                await _loginPage.EnterPasswordAsync("ValidPass123");
                await _loginPage.ClickLoginAsync();

                // Auth0 trims leading/trailing spaces and attempts authentication;
                // expect either an inline format error or a credentials error banner.
                var emailErrorVisible  = await _loginPage.IsEmailErrorVisibleAsync();
                var globalErrorVisible = await _loginPage.IsGlobalErrorVisibleAsync();

                await StepWithScreenshotAsync("Assert system shows an error (format validation or wrong credentials after trimming)", () =>
                    Assert.That(emailErrorVisible || globalErrorVisible, Is.True,
                        "Expected Auth0 to show a format error or a wrong-credentials error for email with leading/trailing spaces"));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }

        /// <summary>
        /// TC-VAL-09: Special Characters in Email
        /// Email: user!#$%^&*()@domain.com | Password: Test123
        /// Expected: Validation error (format or wrong credentials — Auth0 may pass format check and reject on auth).
        /// </summary>
        [Test]
        [Category("Smoke")]
        [Category("Regression")]
        public async Task TC_VAL_09_SpecialCharactersInEmail()
        {
            try
            {
                await _loginPage.NavigateAndOpenLoginAsync(ConfigReader.BaseUrl);
                await _loginPage.EnterEmailAsync("user!#$%^&*()@domain.com");
                await _loginPage.EnterPasswordAsync("Test123");
                await _loginPage.ClickLoginAsync();

                // Auth0 may pass format check and reject on authentication;
                // expect either an inline email format error or a credentials error banner.
                var emailErrorVisible  = await _loginPage.IsEmailErrorVisibleAsync();
                var globalErrorVisible = await _loginPage.IsGlobalErrorVisibleAsync();

                await StepWithScreenshotAsync("Assert special characters in email result in a validation or credentials error", () =>
                    Assert.That(emailErrorVisible || globalErrorVisible, Is.True,
                        "Expected Auth0 to show an error for email with special characters"));
            }
            catch (Exception ex)
            {
                LogExceptionToAllure(ex);
                throw;
            }
        }
    }
}
