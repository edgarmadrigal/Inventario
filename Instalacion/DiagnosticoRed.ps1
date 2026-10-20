<#
.SYNOPSIS
  Se corre en la maquina (laptop/PC) que no conecta por WiFi. Dice por que y como arreglarlo.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File .\DiagnosticoRed.ps1
  powershell -ExecutionPolicy Bypass -File .\DiagnosticoRed.ps1 -HacerPrivada   (como administrador)
#>
param(
    [string]$Servidor = '192.168.128.245',
    [int]$Puerto = 58938,
    [switch]$HacerPrivada
)

Write-Host "`n=== Redes conectadas ===" -ForegroundColor Cyan
$perfiles = Get-NetConnectionProfile
$perfiles | Format-Table InterfaceAlias, Name, NetworkCategory -AutoSize
Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike 'Loopback*' -and $_.IPAddress -notlike '169.254*' } |
    Format-Table InterfaceAlias, IPAddress, PrefixLength -AutoSize

$problemas = @()

Write-Host "=== Pruebas al servidor $Servidor ===" -ForegroundColor Cyan
$ping = Test-Connection $Servidor -Count 2 -Quiet
Write-Host ("Ping al servidor ............ " + $(if ($ping) { 'OK' } else { 'NO responde' }))
$sql = Test-NetConnection $Servidor -Port $Puerto -WarningAction SilentlyContinue
Write-Host ("SQL Server (TCP $Puerto) ..... " + $(if ($sql.TcpTestSucceeded) { 'OK' } else { 'BLOQUEADO' }))
$smb = Test-NetConnection $Servidor -Port 445 -WarningAction SilentlyContinue
Write-Host ("Carpeta de instalacion (445)  " + $(if ($smb.TcpTestSucceeded) { 'OK' } else { 'BLOQUEADO' }))
$nombre = $null
try { $nombre = [System.Net.Dns]::GetHostAddresses('AIN-MSSRV') | Select-Object -First 1 } catch { }
Write-Host ("Nombre AIN-MSSRV ............ " + $(if ($nombre) { "OK ($nombre)  (solo lo usa el Inventario viejo)" } else { 'NO se resuelve (por eso el Inventario viejo no conecta por WiFi)' }))

if (-not $ping -and -not $sql.TcpTestSucceeded) {
    $problemas += "No hay ruta al servidor: la WiFi parece estar en otra red (p.ej. red de invitados). Conectar a la WiFi interna o pedir a Sistemas que la WiFi llegue a la red 192.168.128.x."
}
elseif (-not $sql.TcpTestSucceeded) {
    $problemas += "El servidor responde pero el puerto $Puerto esta cerrado: correr ConfigurarServidorSQL.ps1 en el servidor."
}
if ($perfiles | Where-Object { $_.NetworkCategory -eq 'Public' }) {
    $msg = "La red esta marcada como 'Publica' en esta maquina. Si algo falla, correr este script como administrador con -HacerPrivada."
    if ($sql.TcpTestSucceeded -and $smb.TcpTestSucceeded) { Write-Host "Aviso: $msg" -ForegroundColor DarkYellow } else { $problemas += $msg }
}

if ($HacerPrivada) {
    foreach ($p in $perfiles | Where-Object { $_.NetworkCategory -eq 'Public' }) {
        Set-NetConnectionProfile -InterfaceIndex $p.InterfaceIndex -NetworkCategory Private
        Write-Host "Red '$($p.Name)' cambiada a Privada" -ForegroundColor Green
    }
}

Write-Host "`n=== Resultado ===" -ForegroundColor Cyan
if ($problemas.Count -eq 0) { Write-Host "Todo bien: Inventario V2 puede conectar desde esta maquina." -ForegroundColor Green }
else { $problemas | ForEach-Object { Write-Host "- $_" -ForegroundColor Yellow } }
