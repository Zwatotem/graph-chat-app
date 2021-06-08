# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
$sendInput = (gi $PSCommandPath).DirectoryName + '\..\sendInput.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
# Prepare args for tested programs
$sArgs = ('127.0.0.1', '50000', '5')
$cArgs = ('127.0.0.1', '50000')
# Prepare action templates
$loginUser =	gc((gi $PSCommandPath).DirectoryName + '\loginUser.in')
$registerUser =	gc((gi $PSCommandPath).DirectoryName + '\registerUser.in')
$sendMessage =	gc((gi $PSCommandPath).DirectoryName + '\sendMessage.in')
$receiveMessage =	gc((gi $PSCommandPath).DirectoryName + '\receiveMessage.in')
$enterConversation =	gc((gi $PSCommandPath).DirectoryName + '\enterConversation.in')
$createConversation =	gc((gi $PSCommandPath).DirectoryName + '\createConversation.in')
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
& $sendInput $client1 $registerUser.Replace('user', 'user1')
& $sendInput $client1 $registerUser.Replace('user', 'user2')
& $sendInput $client1 $loginUser.Replace('user', 'user1')
Start-Sleep 1
Write-Debug "Logging user 2"
& $sendInput $client2 $loginUser.Replace('user', 'user2')
& $sendInput $client1 $createConversation.Replace('user', 'user2')
& $sendInput $client1 $enterConversation
Start-Sleep 1
Write-Debug "Sending message"
& $sendInput $client1 $sendMessage.Replace("messageText", $message)
Start-Sleep 1
Write-Debug "Sending reply"
& $sendInput $client2 $enterConversation
& $sendInput $client2 $sendMessage.Replace('-1', '1').Replace("messageText", $message+'reply')
Start-Sleep 1
Write-Debug "Closing"
& $sendInput $client1 $end
$out = $client1.StandardOutput.ReadToEnd()
Stop-Process $server, $client1, $client2
return `
	$out.Contains($message) -and $out.Contains('reply: -1') -and `
	$out.Contains($message+'reply') -and $out.Contains('reply: 1')
