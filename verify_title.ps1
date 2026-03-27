$content = [System.IO.File]::ReadAllText('D:\OneDrive - OTSI\AI_Learning\WillscotCSharpAI\Reports\index.html')
$hits = [regex]::Matches($content, '.{0,40}WillScot.{0,40}')
foreach ($h in $hits) { Write-Host $h.Value }
