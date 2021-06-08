# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
$sendInput = (gi $PSCommandPath).DirectoryName + '\..\sendInput.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
# Prepare args for tested programs
$sArgs = ('127.0.0.1', '50000', '5')
$cArgs = ('127.0.0.1', '50000')
# Prepare action templates
$clientOperations =	gc((gi $PSCommandPath).DirectoryName + '\clientOperations.in')
$serverOut =	(gi $PSCommandPath).DirectoryName + '\..\server.out'
# Create start info objects
$server = (Start-Process $serverExe -ArgumentList $sArgs -NoNewWindow -RedirectStandardOutput $serverOut -PassThru)
Start-Sleep 2
Write-Debug "Server started"
$startInfo = New-Object 'System.Diagnostics.ProcessStartInfo' -Property @{
	FileName               = $clientExe
	Arguments              = $cArgs
	UseShellExecute        = $false
	RedirectStandardInput  = $true
	RedirectStandardOutput	= $true
}
$endTime = Get-Date -Hour 20 -Minute 0 -Second 0
$userName = 3000
$result = $true
while ((Get-Date) -lt $endTime) {
	$userName += 1
	Write-Debug ("Starting client " + $userName.ToString())
	$client = [System.Diagnostics.Process]::Start($startInfo)
	& $sendInput $client $clientOperations.Replace("userName", $userName.ToString())
	Write-Debug "Reading output"
	$out = $client.StandardOutput.ReadToEnd()
	# Expecting two successes, for one it will evaluate to index -ne index; for zero, -1 -ne -1
	$result = $result -and ($out.IndexOf("Successfully") -ne $out.LastIndexOf("Successfully"))
	$result = $result -and ($client.ExitCode -eq 0)
	Write-Debug ("Intermediate result:  " + $result.ToString())
	Stop-Process $client
	Start-Sleep 300
}
$result