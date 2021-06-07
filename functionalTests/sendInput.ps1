($process, $text) = $args
$text = $text.Split([System.Environment]::NewLine)
$text | foreach { $process.StandardInput.WriteLine($_) }