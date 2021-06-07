# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
# Prepare args for tested programs
$sArgs = ('127.0.0.1', '50000', '5')
$cArgs = ('127.0.0.1', '50000')
$startInfoServer = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
    FileName = $serverExe
    Arguments = $sArgs
    UseShellExecute = $false
    RedirectStandardInput = $true
    RedirectStandardOutput = $true
}
$server = [System.Diagnostics.Process]::Start($startInfoServer)
$startInfoClient = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
    FileName = $clientExe
    Arguments = $cArgs
    UseShellExecute = $false
    RedirectStandardInput = $true
    RedirectStandardOutput = $true
}
$result = $true
$expectedDisplay = Get-Content ((gi $PSCommandPath).DirectoryName + '\expectedDisplay.txt')
$endTime = Get-Date -Hour 20 -Minute 30 -Second 0
$currTime = Get-Date
while ($currTime -lt $endTime) {
    $client = [System.Diagnostics.Process]::Start($startInfoClient)
    $client.StandardInput.WriteLine("0") 
    $stdout = $client.StandardOutput.ReadToEnd()
    If ($stdout.IndexOf($expectedDisplay) -eq -1) {$result = $false}
    Start-Sleep -Seconds 300
    $currTime = Get-Date
}
ps | ? { ('ChatClient', 'ChatServer') -contains $_.Name } | Stop-Process
Write-Output $result 