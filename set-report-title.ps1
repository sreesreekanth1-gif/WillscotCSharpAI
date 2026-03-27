param(
    [string]$AppSettingsFile,
    [string]$AllureConfigRoot,
    [string]$AllureConfigOut,
    [string]$AllureResultsDir,
    [string]$ReportHtml
)

$settings = Get-Content $AppSettingsFile -Raw | ConvertFrom-Json
$title = $settings.ReportTitle

# Update allureConfig.json in all three locations so allure generate picks up reportName
$configs = @($AllureConfigRoot, $AllureConfigOut, "$AllureResultsDir\allureConfig.json")
foreach ($path in $configs) {
    if (Test-Path $path) {
        $cfg = Get-Content $path -Raw | ConvertFrom-Json
    } else {
        $cfg = [PSCustomObject]@{ allure = [PSCustomObject]@{ directory = "allure-results"; links = @() } }
    }
    $cfg.allure | Add-Member -NotePropertyName reportName -NotePropertyValue $title -Force
    $cfg | ConvertTo-Json -Depth 5 | Set-Content $path
}
Write-Host "Set reportName='$title' in allureConfig.json files"

# Patch the generated index.html if it exists
if (-not (Test-Path $ReportHtml)) {
    Write-Host "index.html not found yet, skipping HTML patch"
    exit 0
}

$html = [System.IO.File]::ReadAllText($ReportHtml)

# Replace <title> tag
$html = [regex]::Replace($html, '<title>[^<]*</title>', "<title>$title</title>")

# Inject a visible title banner after <body> tag
$banner = @"
<style>
  #report-custom-title {
    position: fixed; top: 0; left: 0; right: 0; z-index: 99999;
    background: #d33d37; color: #fff;
    font-family: Arial, sans-serif; font-size: 16px; font-weight: bold;
    padding: 8px 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.3);
  }
  body { padding-top: 36px !important; }
</style>
<div id="report-custom-title">$title &mdash; Test Report</div>
"@

if ($html -notmatch 'report-custom-title') {
    $html = $html -replace '<body>', "<body>`n$banner"
}

[System.IO.File]::WriteAllText($ReportHtml, $html, [System.Text.Encoding]::UTF8)
Write-Host "Patched index.html with title '$title'"
