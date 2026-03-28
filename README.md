# Playwright + NUnit Test Automation Framework (WillScot)

## Overview
C# .NET latest automation framework using Playwright with NUnit, Allure reporting, Excel data, self-healing locators, and notifications.

## Prerequisites
- .NET 6 SDK or later
- Visual Studio Code + C# extension
- Playwright CLI: `dotnet tool install --global Microsoft.Playwright.CLI`
- Allure CLI installed (`choco install allure` or `scoop install allure`)

## Setup
1. `dotnet restore`
2. `dotnet tool restore`
3. `pwsh .\packages\Microsoft.Playwright.Tools\tools\playwright.ps1 install` (or `playwright install`)
4. Configure `Configuration/appsettings.json` (secure secrets via env vars)

## Run tests
`dotnet test`

## Generate Allure report
`allure generate Reports --clean -o Reports/generated`
`allure open Reports/generated`

## Folder structure
- `Configuration/`: `appsettings.json`, `ConfigReader`, enums
- `Utilities/`: `ExcelReader`, `NotificationService`, `SelfHealingLocator`, `TestContextManager`
- `Pages/`: `BasePage`, `LandingPage`
- `Tests/`: `BaseTest`, `WebTests`, `ApiTests`
- `TestData/`: Excel test data (e.g. `LoginData.xlsx`)
- `Reports/`: Allure results + screenshots

## Features
- Parallel execution with `[Parallelizable]` and `AsyncLocal` for per-thread context
- Self-healing locator fallback strategies
- Post-run email/Slack/Teams notifications from `NotificationService`
- Excel data loader with `ClosedXML`
- Allure support via `Allure.NUnit`

## Notes
- Do not commit real passwords; use environment variables.
- Add extra pages and test cases as needed.
- For CI, use `.github/workflows/ci.yml`.
