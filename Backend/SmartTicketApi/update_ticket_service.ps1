$filePath = "c:\Users\hp\SmartTicketSystem\Backend\SmartTicketApi\Services\Tickets\TicketService.cs"
$content = Get-Content $filePath -Raw

# Replace pattern 1: AssignedTo = t.AssignedTo != null ? t.AssignedTo.Name : null
$pattern1 = '(?m)(^\s+AssignedTo = t\.AssignedTo != null \? t\.AssignedTo\.Name : null)\r?\n(\s+\}\))'
$replacement1 = '$1,' + "`r`n" + '$2IsEscalated = t.IsEscalated' + "`r`n" + '$2'
$content = $content -replace $pattern1, $replacement1

# Replace pattern 2: AssignedTo = t.AssignedTo!.Name  
$pattern2 = '(?m)(^\s+AssignedTo = t\.AssignedTo!\.Name)\r?\n(\s+\}\))'
$replacement2 = '$1,' + "`r`n" + '$2IsEscalated = t.IsEscalated' + "`r`n" + '$2'
$content = $content -replace $pattern2, $replacement2

Set-Content $filePath -Value $content -NoNewline
Write-Host "File updated successfully"
