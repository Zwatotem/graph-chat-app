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
$receiveMessage =	gc((gi $PSCommandPath).DirectoryName + '\receiveMessage.in')
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
$client = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "Client started"
& $sendInput $client $registerUser
& $sendInput $client $loginUser
& $sendInput $client $createConversation.Replace('user', '')
& $sendInput $client $enterConversation
& $sendInput $client $sendMessage
$sentDate = Get-Date
& $sendInput $client $end
$out = $client.StandardOutput.ReadToEnd()
Stop-Process $server, $client
return `
	(
		$out.Contains($sentDate.ToString("dd.MM.yyyy HH:mm")) -or `
		$out.Contains($sentDate.AddMinutes(1).ToString("dd.MM.yyyy HH:mm")) -or `
		$out.Contains($sentDate.AddMinutes(-1).ToString("dd.MM.yyyy HH:mm"))
	) -and `
	(
		$out.LastIndexOf('user') -gt $out.IndexOf('Successfully sent message')
	)