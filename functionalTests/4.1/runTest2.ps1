# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
$testNo = [int]((gi $PSCommandPath).Directory.Name.Replace('.', ''))
# Prepare args for tested programs
$sArgs = ('127.0.0.1', (50000+$testNo).ToString(), '5')
$cArgs = ('127.0.0.1', (50000+$testNo).ToString())
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
    StandardOutputEncoding = [System.Text.Encoding]::UTF8
    RedirectStandardInput = $true
    RedirectStandardOutput = $true
}
$result = $true
$expectedDisplay = Get-Content ((gi $PSCommandPath).DirectoryName + '\expectedDisplay.txt')
$endTime = Get-Date -Hour 21 -Minute 6 -Second 0
$currTime = Get-Date
#while ($currTime -lt $endTime) {
for ($i = 0; $i -lt 1; $i++) {
    $client = [System.Diagnostics.Process]::Start($startInfoClient)
    $client.StandardInput.WriteLine("0") 
    $stdout = $client.StandardOutput.ReadToEnd()
    $stdout > "f.out"
    $objects = @{
    ReferenceObject = $expectedDisplay
    DifferenceObject = (Get-Content "f.out")
    }
    Compare-Object @objects | foreach { If ($_.SideIndicator.Equals("<=")) {$result = false} }
    Start-Sleep -Seconds 1
    $currTime = Get-Date
}
ps | ? { ('ChatClient', 'ChatServer') -contains $_.Name } | Stop-Process
$result 