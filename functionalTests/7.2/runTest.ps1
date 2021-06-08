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
$createConversation =	gc((gi $PSCommandPath).DirectoryName + '\createConversation.in')
$enterConversation =	gc((gi $PSCommandPath).DirectoryName + '\enterConversation.in')
$leaveConversation =	gc((gi $PSCommandPath).DirectoryName + '\leaveConversation.in')
$rejectConversation =	gc((gi $PSCommandPath).DirectoryName + '\rejectConversation.in')
$expandConversation =	gc((gi $PSCommandPath).DirectoryName + '\expandConversation.in')
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
& $sendInput $client1 $createConversation.Replace('user', 'user2').Replace('conv', 'conv1')
& $sendInput $client1 $createConversation.Replace('user', 'user2').Replace('conv', 'conv2')
Write-Debug "Sending message"
& $sendInput $client1 $enterConversation
& $sendInput $client1 $sendMessage.Replace("messageText", "message1")
Start-Sleep 1
Write-Debug "Resending message"
& $sendInput $client2 $enterConversation
& $sendInput $client2 $sendMessage.Replace("messageText", "message2")
Write-Debug "Changing conversations"
& $sendInput $client1 $leaveConversation
& $sendInput $client2 $leaveConversation
& $sendInput $client1 $enterConversation.Replace('1', '2')
& $sendInput $client2 $enterConversation.Replace('1', '2')
Write-Debug "Sending message"
& $sendInput $client1 $enterConversation
& $sendInput $client1 $sendMessage.Replace("messageText", "message1")
Start-Sleep 1
Write-Debug "Resending message"
& $sendInput $client2 $enterConversation
& $sendInput $client2 $sendMessage.Replace("messageText", "message2")
Write-Debug "Ending processes"
& $sendInput $client1 $end
& $sendInput $client2 $end
Start-Sleep (115 * 60)
# Reading output
$client1 = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "First instance woke"
$client2 = [System.Diagnostics.Process]::Start($startInfo)
Write-Debug "Second instance woke"
& $sendInput $client1 $loginUser.Replace('user', 'user1')
& $sendInput $client2 $loginUser.Replace('user', 'user2')
& $sendInput $client1 $receiveMessage
& $sendInput $client2 $receiveMessage
Write-Debug "Changing conversations"
& $sendInput $client1 $leaveConversation
& $sendInput $client2 $leaveConversation
& $sendInput $client1 $receiveMessage.Replace('1', '2')
& $sendInput $client2 $receiveMessage.Replace('1', '2')
Write-Debug "Ending processes"
& $sendInput $client1 $end
& $sendInput $client2 $end
$out1 = $client1.StandardOutput.ReadToEnd()
$out2 = $client2.StandardOutput.ReadToEnd()
Stop-Process $server, $client1, $client2
$out1.Contains("message1") -and $out1.Contains("message2") -and $out2.Contains("message1") -and $out2.Contains("message2") 