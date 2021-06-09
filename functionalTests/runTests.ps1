$excludedTests = ('4.1', '4.2', '4.3', '7.1', '7.2', '7.3', '1.3', '6.2', '8.3')
$result = $true
write "Running tests"
Get-ChildItem ($PSCommandPath+'\..') -Directory | 
where {-not ($excludedTests -contains $_.Name)} |
select Name, @{Name='File';Expression={$_.GetFiles() | where Name -eq 'runTest.ps1' | select -First 1}} |
foreach {
    $intermediateResult = 
    (Powershell.exe -executionpolicy remotesigned -File $_.File)
    if ($intermediateResult -eq 'True') {
        Write-Host -Object ($_.Name+' passed successfully') -ForegroundColor 'Green'
    }
    else {
        Write-Host -Object ($_.Name+' failed') -ForegroundColor 'Red'
    }
    $result = $result -and $intermediateResult
}

if ($Result -eq 'True') {
    Write-Host -Object ('*All tests passed successfully*') -ForegroundColor 'Green'
}
else {
    Write-Host -Object ('*Some tests failed*') -ForegroundColor 'Red'
}