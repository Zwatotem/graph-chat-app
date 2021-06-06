$projectRoot = (Get-Item $PSCommandPath).Directory.Parent
$clientPath = $projectRoot.GetDirectories() |
	? Name -eq 'ChatClient' |
	% { $_.GetDirectories() } |
	? Name -eq 'bin' |
	% { $_.GetDirectories() } |
	? Name -eq 'Release'|
	% { $_.GetDirectories() } |
	? Name -eq 'net5.0-windows'
$serverPath = $projectRoot.GetDirectories() |
	? Name -eq 'ChatServer' |
	% { $_.GetDirectories() } |
	? Name -eq 'bin' |
	% { $_.GetDirectories() } | 
	? Name -eq 'Release'|
	% { $_.GetDirectories() } | 
	? Name -eq 'net5.0-windows'
$serverExe = $serverPath.GetFiles()|? Extension -eq '.exe'|select -First 1
$clientExe = $clientPath.GetFiles()|? Extension -eq '.exe'|select -First 1
($projectRoot.FullName, $serverPath.FullName, $clientPath.FullName, $serverExe.FullName, $clientExe.FullName)