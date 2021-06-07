# Executing setup script
$setup = (gi $PSCommandPath).DirectoryName + '\..\setup.ps1'
($projectRoot, $serverPath, $clientPath, $serverExe, $clientExe) = (& $setup)
# Prepare args for tested programs
$sArgs = ('127.0.0.1', '50000', '5')
$cArgs = ('127.0.0.1', '50000')
# Prepare IO files
$serverout = (gi $PSCommandPath).DirectoryName + '\server.out'
$instance1intmpl = (gi $PSCommandPath).DirectoryName + '\instance1.in.template'
$instance1in = (gi $PSCommandPath).DirectoryName + '\instance1.in'
$instance1out = (gi $PSCommandPath).DirectoryName + '\instance1.out'
$instance2in = (gi $PSCommandPath).DirectoryName + '\instance2.in'
$instance2out = (gi $PSCommandPath).DirectoryName + '\instance2.out'
# Generate random message text
$message = [guid]::NewGuid().ToString()
Set-Content $instance1in (Get-Content $instance1intmpl).Replace("messageText", $message)
Start-Process -FilePath $serverExe -ArgumentList $sArgs -RedirectStandardOutput $serverout -NoNewWindow
Write-Debug "Server started"
Start-Process -FilePath $clientExe -ArgumentList $cArgs -RedirectStandardInput $instance1in -RedirectStandardOutput $instance1out -NoNewWindow -Wait
Write-Debug "First instance finished"
Start-Process -FilePath $clientExe -ArgumentList $cArgs -RedirectStandardInput $instance2in -RedirectStandardOutput $instance2out -NoNewWindow -Wait
Write-Debug "Second instance finished"
ps | ? { ('ChatClient', 'ChatServer') -contains $_.Name } | Stop-Process
(gc $instance2out).Contains($message)