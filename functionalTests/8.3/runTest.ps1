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
$displayUsers =	gc((gi $PSCommandPath).DirectoryName + '\displayUsers.in')
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
Write-Debug 'Starting processes'
$clients = (1..5 | foreach {[System.Diagnostics.Process]::Start($startInfo)})
Write-Debug 'Registering users'
$clients | foreach {
	&$sendInput $_ $registerUser.Replace('user', 'user' + $_.Id)
}
Write-Debug 'Logging users in'
$clients | foreach {
	&$sendInput $_ $loginUser.Replace('user', 'user' + $_.Id)
	Start-Sleep 1
}
Write-Debug 'Creating conversation'
$userList = ''
$clients | foreach {
	$userList += 'user' + $_.Id + [System.Environment]::NewLine
}
&$sendInput $clients[0] $createConversation.Replace('user', $userList)
Start-Sleep 1
Write-Debug 'Entering conversation'
$clients | foreach {
	&$sendInput $_ $enterConversation
}
Write-Debug 'Displaing users'
$clients | foreach {
	&$sendInput $_ $displayUsers
}
Start-Sleep 2
Write-Debug 'Leaving'
$result = $true
$clients | foreach {
	&$sendInput $_ $end
	$out = $_.StandardOutput.ReadToEnd()
	$clients | foreach {
		$result = $result -and $out.Contains('user' + $_.Id)
	}
}
Stop-Process $server
Stop-Process $clients
$result