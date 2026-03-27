$file = 'D:\OneDrive - OTSI\AI_Learning\WillscotCSharpAI\Reports\index.html'
$content = [System.IO.File]::ReadAllText($file)

Write-Host "File size: $($content.Length) chars"
Write-Host "WillScot count: $([regex]::Matches($content, 'WillScot').Count)"
Write-Host "Allure count (any case): $([regex]::Matches($content, '(?i)allure report').Count)"

# Find reportName in JSON blobs
$rn = [regex]::Matches($content, '"reportName"\s*:\s*"([^"]*)"')
Write-Host "reportName JSON hits: $($rn.Count)"
foreach ($m in $rn) { Write-Host "  -> $($m.Value)" }

# Find title-like patterns in JS data
$t = [regex]::Matches($content, '"title"\s*:\s*"([^"]{1,60})"')
Write-Host "title JSON hits (first 5):"
$t | Select-Object -First 5 | ForEach-Object { Write-Host "  -> $($_.Value)" }
