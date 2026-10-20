# Instalación de Inventario V2

Inventario V2 se instala **junto** al Inventario actual (no lo reemplaza): tiene otro nombre (`InventarioV2`),
su propio acceso directo y su propia carpeta de actualizaciones. Si algo falla se sigue usando el viejo.

## 1. Base de datos (una vez, en SQL Server Management Studio)

Correr `Fase1_ReportesMarca.sql` (está en la raíz del repositorio) contra la base `Inventario`.

- Agrega la columna MARCA y el filtro por marca a `ConsultaInventario` y `ConsultaInventarioTallaSeparada`.
- Hace esos reportes ~8 veces más rápidos y corrige el reporte POTallasSeparadas (hoy falla con cartones de más de 5 dígitos).
- El Inventario viejo sigue funcionando igual (el parámetro nuevo es opcional).
- Si V2 se usa antes de correr el script, los reportes funcionan igual: la marca se calcula en la app.

## 2. Servidor (una vez, fuera de horario)

En el servidor `AIN-MSSRV` (192.168.128.245), como administrador:

```powershell
powershell -ExecutionPolicy Bypass -File ConfigurarServidorSQL.ps1
```

Fija el puerto de SQL Server en 58938 (hoy es dinámico y puede cambiar al reiniciar) y abre el firewall en todos
los perfiles de red. **Reinicia SQL Server** (unos segundos sin servicio).

## 3. Publicar

En Visual Studio: proyecto `Inventario` (en `C:\InventarioV2`) → Publicar. Ya está configurado para publicar en
`\\192.168.128.245\Sistemas\InventarioV2\`. Desde línea de comandos:

```
msbuild Inventario\Inventario.csproj -t:Publish -p:Configuration=Release -p:PublishDir=\\192.168.128.245\Sistemas\InventarioV2\
```

Cada publicación sube la versión; las máquinas se actualizan solas al abrir la app.

## 4. Instalar en cada máquina

Abrir `\\192.168.128.245\Sistemas\InventarioV2\setup.exe` (instala .NET 4.8 si falta) y aceptar.
Queda el acceso directo **Inventario V2** en el escritorio.

Las fuentes que usan algunas etiquetas (por ejemplo `Libre Barcode 39`) deben estar instaladas, igual que con el
Inventario viejo.

## Por qué ahora funciona por WiFi

El Inventario viejo se conecta por **nombre** (`AIN-MSSRV\SISTEMASAIN`): necesita que Windows resuelva el nombre y
que SQL Browser (UDP 1434) le diga el puerto. Por WiFi eso suele fallar (otra subred o red marcada como "Pública").
V2 se conecta directo a `192.168.128.245,58938` y reintenta solo si la señal se corta.

Si una máquina no conecta por WiFi, correr ahí:

```powershell
powershell -ExecutionPolicy Bypass -File DiagnosticoRed.ps1
```

Dice si el problema es la red WiFi (otra subred / red de invitados), el firewall del servidor o el perfil "Público"
de la máquina (`-HacerPrivada`, como administrador, lo corrige).
