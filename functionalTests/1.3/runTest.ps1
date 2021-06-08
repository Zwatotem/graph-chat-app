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
Write-Debug 'Starting processes'
$clientSender = [System.Diagnostics.Process]::Start($startInfo)
$clientRecivers = (1..3 | foreach {[System.Diagnostics.Process]::Start($startInfo)})
Write-Debug 'Registering users'
& $sendInput $clientSender $registerUser.Replace('user', 'sender')
$clientRecivers | foreach {
	&$sendInput $_ $registerUser.Replace('user', 'receiver' + $_.Id)
}
Write-Debug 'Logging users in'
& $sendInput $clientSender $loginUser.Replace('user', 'sender')
$clientRecivers | foreach {
	&$sendInput $_ $loginUser.Replace('user', 'receiver' + $_.Id)
}
Start-Sleep 1
Write-Debug 'Creating conversation'
$userList = ''
$clientRecivers | foreach {
	$userList += 'receiver' + $_.Id + [System.Environment]::NewLine
}
&$sendInput $clientSender $createConversation.Replace('user', $userList)
Start-Sleep 1
Write-Debug 'Sending message'
&$sendInput $clientSender $enterConversation
&$sendInput $clientSender $sendMessage.Replace('messageText', $message)
Start-Sleep 1
Write-Debug 'Receiving message'
$result = $true
$clientRecivers | foreach {
	&$sendInput $_ $receiveMessage
	&$sendInput $_ $end
	$result = $result -and $_.StandardOutput.ReadToEnd().Contains($message)
}
$result