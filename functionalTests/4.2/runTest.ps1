# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
$sendInput = (gi $PSCommandPath).DirectoryName + '\..\sendInput.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
$testNo = [int]((gi $PSCommandPath).Directory.Name.Replace('.', ''))
$serverOut =	(gi $PSCommandPath).DirectoryName + '\server.out'
# Prepare args for tested programs
$sArgs = ('127.0.0.1', (50000+$testNo).ToString(), '5')
$cArgs = ('127.0.0.1', (50000+$testNo).ToString())
# Prepare action templates
$loginUser1 =	gc((gi $PSCommandPath).DirectoryName + '\loginUser1.in')
$loginUser2 =	gc((gi $PSCommandPath).DirectoryName + '\loginUser2.in')
$sendMessage =	gc((gi $PSCommandPath).DirectoryName + '\sendMessage.in')
$end =	gc((gi $PSCommandPath).DirectoryName + '\end.in')
# Create start info objects
$startInfoServer = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
	FileName               = $serverExe
	Arguments              = $sArgs
	UseShellExecute        = $false
	RedirectStandardInput  = $true
	RedirectStandardOutput	= $true
}
$server = (Start-Process $serverExe -ArgumentList $sArgs -NoNewWindow -RedirectStandardOutput $serverOut -PassThru)
Write-Debug "Server started"
$startInfo = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
	FileName               = $clientExe
	Arguments              = $cArgs
	UseShellExecute        = $false
	RedirectStandardInput  = $true
	RedirectStandardOutput	= $true
}
$endTime = Get-Date -Hour 20 -Minute 0 -Second 0
$client1 = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "First instance started"
$client2 = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "Second instance started"
Write-Debug "Logging user 1"
& $sendInput $client1 $loginUser1
Start-Sleep 1
Write-Debug "Logging user 2"
& $sendInput $client2 $loginUser2
Start-Sleep 120
$message = 3000
while ((Get-Date) -lt $endTime) {
	$message += 1
	Write-Debug ("Sending message "+$message.ToString())
	& $sendInput $client1 $sendMessage.Replace("messageText", $message.ToString())
	Start-Sleep 60
	$message += 1
	Write-Debug ("Resending message "+$message.ToString())
	& $sendInput $client2 $sendMessage.Replace("messageText", $message.ToString())
	Start-Sleep 240
}
Write-Debug "Ending processes"
& $sendInput $client1 $end
& $sendInput $client2 $end
# Reading output
$out = $client1.StandardOutput.ReadToEnd()
$out += $client2.StandardOutput.ReadToEnd()
Stop-Process $server, $client1, $client2
$result = $true
3001..$message | foreach { $result = $result -and ($out.Contains($_.ToString())) }
$result