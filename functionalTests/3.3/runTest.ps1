# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
$sendInput = (gi $PSCommandPath).DirectoryName + '\..\sendInput.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
$testNo = [int]((gi $PSCommandPath).Directory.Name.Replace('.', ''))
# Prepare args for tested programs
$sArgs = ('127.0.0.1', (50000+$testNo).ToString(), '5')
$cArgs = ('127.0.0.1', (50000+$testNo).ToString())
# Prepare action templates
$loginUser =	gc((gi $PSCommandPath).DirectoryName + '\loginUser.in')
$registerUser =	gc((gi $PSCommandPath).DirectoryName + '\registerUser.in')
$sendMessage =	gc((gi $PSCommandPath).DirectoryName + '\sendMessage.in')
$rejectConversation =	gc((gi $PSCommandPath).DirectoryName + '\rejectConversation.in')
$createConversation =	gc((gi $PSCommandPath).DirectoryName + '\createConversation.in')
$enterConversation =	gc((gi $PSCommandPath).DirectoryName + '\enterConversation.in')
$leaveConversation =	gc((gi $PSCommandPath).DirectoryName + '\leaveConversation.in')
$end =	gc((gi $PSCommandPath).DirectoryName + '\end.in')
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
Write-Debug "Creating conversation"
& $sendInput $client1 $createConversation.Replace('user', 'user2')
Start-Sleep 1
& $sendInput $client1 $enterConversation
Write-Debug "Sending message"
& $sendInput $client1 $sendMessage
$sentDate = Get-Date
Write-Debug "Leaving conversation"
& $sendInput $client1 $leaveConversation
& $sendInput $client1 $rejectConversation
Start-Sleep 1
& $sendInput $client2 $enterConversation
Write-Debug "Ending processes"
& $sendInput $client1 $end
& $sendInput $client2 $end
$out = $client2.StandardOutput.ReadToEnd()
Stop-Process $server, $client1, $client2
return `
	(
		$out.Contains($sentDate.ToString("dd.MM.yyyy HH:mm")) -or `
		$out.Contains($sentDate.AddMinutes(1).ToString("dd.MM.yyyy HH:mm")) -or `
		$out.Contains($sentDate.AddMinutes(-1).ToString("dd.MM.yyyy HH:mm"))
	) -and `
	(
		$out.LastIndexOf('user1') -gt $out.IndexOf('conversation you wish to display:')
	) -and `
	(
		$out.Contains('messageText')
	)