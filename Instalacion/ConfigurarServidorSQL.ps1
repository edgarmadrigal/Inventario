#Requires -RunAsAdministrator
<#
.SYNOPSIS
  Se corre UNA vez en el servidor de SQL (AIN-MSSRV, 192.168.128.245) para que Inventario V2 conecte por WiFi.

.DESCRIPTION
  1. Fija el puerto TCP de la instancia (hoy es dinamico: 58938 y puede cambiar al reiniciar SQL Server).
  2. Abre el firewall de Windows para ese puerto, para SQL Browser (lo usa el Inventario viejo) y para
     "Compartir archivos" (la instalacion/actualizacion ClickOnce sale de \\192.168.128.245\Sistemas),
     en TODOS los perfiles de red (Dominio, Privado y Publico).
  3. Reinicia el servicio de SQL Server: la app deja de responder unos segundos. Correrlo fuera de horario.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File .\ConfigurarServidorSQL.ps1
#>
param(
    [string]$Instancia = 'SISTEMASAIN',
    [int]$Puerto = 58938
)
$ErrorActionPreference = 'Stop'

$id = (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL').$Instancia
if (-not $id) { throw "No se encontro la instancia $Instancia en este servidor." }
$ipAll = "HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\$id\MSSQLServer\SuperSocketNetLib\Tcp\IPAll"
$actual = Get-ItemProperty $ipAll
Write-Host "Instancia $Instancia ($id). Puerto actual: TcpPort='$($actual.TcpPort)' TcpDynamicPorts='$($actual.TcpDynamicPorts)'"

# 1) puerto fijo (se quita el dinamico)
Set-ItemProperty $ipAll -Name TcpDynamicPorts -Value ''
Set-ItemProperty $ipAll -Name TcpPort -Value "$Puerto"
Write-Host "Puerto fijo configurado: $Puerto"

# 2) firewall en todos los perfiles
function Regla($nombre, $protocolo, $puerto) {
    Get-NetFirewallRule -DisplayName $nombre -ErrorAction SilentlyContinue | Remove-NetFirewallRule
    New-NetFirewallRule -DisplayName $nombre -Direction Inbound -Protocol $protocolo -LocalPort $puerto -Action Allow -Profile Any | Out-Null
    Write-Host "Firewall: $nombre"
}
Regla "Inventario - SQL Server $Instancia (TCP $Puerto)" TCP $Puerto
Regla "Inventario - SQL Browser (UDP 1434)" UDP 1434
# Compartir archivos e impresoras (grupo independiente del idioma de Windows)
Get-NetFirewallRule -Group '@FirewallAPI.dll,-28502' | Set-NetFirewallRule -Enabled True -Profile Any
Write-Host "Firewall: Compartir archivos e impresoras habilitado en todos los perfiles"

# 3) reiniciar SQL Server para aplicar el puerto
$servicio = "MSSQL`$$Instancia"
Write-Host "Reiniciando $servicio ..."
Restart-Service -Name $servicio -Force
Start-Sleep -Seconds 5
$escucha = Get-NetTCPConnection -State Listen -LocalPort $Puerto -ErrorAction SilentlyContinue
if ($escucha) { Write-Host "OK: SQL Server escucha en el puerto $Puerto" -ForegroundColor Green }
else { Write-Warning "SQL Server no aparece escuchando en $Puerto. Revisar SQL Server Configuration Manager > Protocolos > TCP/IP (Habilitado)." }
