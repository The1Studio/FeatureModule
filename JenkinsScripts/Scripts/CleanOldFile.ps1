Param ([string] $WorkspaceDir)
if (!$WorkspaceDir) {Write-Host "Path null"}
if ($WorkspaceDir -and $PWD.Path.Contains("workspace")) {
    Get-ChildItem -Path $WorkspaceDir\ -Directory | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-3)} | Remove-Item -Recurse -Force -Verbose
    Get-ChildItem -Path $WorkspaceDir\Android -Directory | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-3)} | Remove-Item -Recurse -Force -Verbose
    Get-ChildItem -Path $WorkspaceDir\IOS -Directory | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-3)} | Remove-Item -Recurse -Force -Verbose
    Get-ChildItem -Path $WorkspaceDir\WebGL -Directory | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-3)} | Remove-Item -Recurse -Force -Verbose
    Get-ChildItem -Path $WorkspaceDir\Windows -Directory | Where-Object {$_.LastWriteTime -lt (Get-Date).AddDays(-3)} | Remove-Item -Recurse -Force -Verbose
}
