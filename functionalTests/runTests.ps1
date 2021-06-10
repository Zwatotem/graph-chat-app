$notRun = ('4.1', '4.2', '4.3', '7.1', '7.2', '7.3')
ls ($PSCommandPath+'\..') -Recurse 'runTest.ps1' |
where { -not ($notRun -contains $_.Directory.Name) } |
foreach { write ($_.Directory.Name + ':'); Invoke-Expression $_ }
