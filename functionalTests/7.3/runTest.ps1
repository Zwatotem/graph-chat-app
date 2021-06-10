# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
$sendInput = (gi $PSCommandPath).DirectoryName + '\..\sendInput.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
$testNo = [int]((gi $PSCommandPath).Directory.Name.Replace('.', ''))
# Prepare args for tested programs
$sArgs = ('127.0.0.1', (50000+$testNo).ToString(), '5')
$cArgs = ('127.0.0.1', (50000+$testNo).ToString())
# Prepare action templates
$serverOut =	(gi $PSCommandPath).DirectoryName + '\server.out'
$loginUser1 =	gc((gi $PSCommandPath).DirectoryName + '\loginUser1.in')
$loginUser2 =	gc((gi $PSCommandPath).DirectoryName + '\loginUser2.in')
$sendMessage =	gc((gi $PSCommandPath).DirectoryName + '\sendMessage.in')
$end =	gc((gi $PSCommandPath).DirectoryName + '\end.in')
$server = (Start-Process $serverExe -ArgumentList $sArgs -NoNewWindow -RedirectStandardOutput $serverOut -PassThru)
Write-Debug "Server started"
$startInfo = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
	FileName               = $clientExe
	Arguments              = $cArgs
	UseShellExecute        = $false
	RedirectStandardInput  = $true
	RedirectStandardOutput	= $true
}
$endTime = (Get-Date).AddHours(2)
$client = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "First instance started"
Write-Debug "Logging user 1"
& $sendInput $client $loginUser1
Write-Debug "Sending message"
& $sendInput $client $sendMessage
& $sendInput $client $end
$client.StandardOutput.ReadToEnd()|Out-Null
Stop-Process $client
$result = $true
while ((Get-Date) -lt $endTime) {
	Start-Sleep 300
	Write-Debug ("Reentering "+(Get-Date).ToString('HH:mm:ss'))
	$client = [System.Diagnostics.Process]::Start($startInfo)
	& $sendInput $client $loginUser2
	& $sendInput $client $end
	$result = $result -and $client.StandardOutput.ReadToEnd().Contains('messageText')
	Stop-Process $client
}
Stop-Process $server
$result