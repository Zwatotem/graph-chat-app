# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
$sendInput = (gi $PSCommandPath).DirectoryName + '\..\sendInput.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
$testNo = [int]((gi $PSCommandPath).Directory.Name.Replace('.', ''))
# Prepare args for tested programs
$sArgs = ('127.0.0.1', (50000+$testNo).ToString(), '5')
$cArgs = ('127.0.0.1', (50000+$testNo).ToString())
# Prepare action templates
$loginUser1 =	gc((gi $PSCommandPath).DirectoryName + '\loginUser1.in')
$loginUser2 =	gc((gi $PSCommandPath).DirectoryName + '\loginUser2.in')
$createConversation =	gc((gi $PSCommandPath).DirectoryName + '\createConversation.in')
$enterConversation =	gc((gi $PSCommandPath).DirectoryName + '\enterConversation.in')
$sendMessage =	gc((gi $PSCommandPath).DirectoryName + '\sendMessage.in')
$receiveMessage =	gc((gi $PSCommandPath).DirectoryName + '\receiveMessage.in')
$end =	gc((gi $PSCommandPath).DirectoryName + '\end.in')
# Generate random message text
$message = [guid]::NewGuid().ToString()
# Create start info objects
$startInfoServer = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
	FileName               = $serverExe
	Arguments              = $sArgs
	UseShellExecute        = $false
	RedirectStandardInput  = $true
	RedirectStandardOutput	= $true
}
$server = [System.Diagnostics.Process]::Start($startInfoServer)
Write-Debug "Server started"
$startInfo = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
	FileName               = $clientExe
	Arguments              = $cArgs
	UseShellExecute        = $false
	RedirectStandardInput  = $true
	RedirectStandardOutput	= $true
}
$client1 = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "First instance started"
$client2 = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "Second instance started"
Write-Debug "Logging user 1"
& $sendInput $client1 $loginUser1
Start-Sleep 1
Write-Debug "Logging user 2"
& $sendInput $client2 $loginUser2
& $sendInput $client1 $createConversation.Replace('user', 'user2')
& $sendInput $client1 $enterConversation
Start-Sleep 1
Write-Debug "Sending message"
& $sendInput $client1 $sendMessage.Replace("messageText", $message)
Write-Debug "Sending self-reply"
& $sendInput $client1 $sendMessage.Replace('-1', '1').Replace("messageText", $message+'reply')
Start-Sleep 1
Write-Debug "Receiving message"
& $sendInput $client2 $receiveMessage
Write-Debug "Closing"
& $sendInput $client1 $end
$out = $client2.StandardOutput.ReadToEnd()
Stop-Process $server, $client1, $client2
return `
	$out.Contains($message) -and $out.Contains('reply: -1') -and `
	$out.Contains($message+'reply') -and $out.Contains('reply: 1')
