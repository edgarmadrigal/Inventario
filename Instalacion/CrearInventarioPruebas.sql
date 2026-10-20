/* =====================================================================================
   CrearInventarioPruebas.sql
   Crea la base InventarioPruebas para el "Modo de prueba" de Inventario V2.

   - Se corre UNA vez en SSMS, conectado a la instancia AIN-MSSRV\SISTEMASAIN
     (tcp:192.168.128.245,58938) con el usuario sa.
   - NO modifica la base Inventario: solo lee su estructura y crea todo en InventarioPruebas.
   - Copia los catalogos completos (tallas, usuarios, clientes, ...) y solo la ESTRUCTURA de las
     tablas de movimientos (cajas, POs, bajas, almacen): la base de pruebas arranca sin cajas.
   - Copia procedimientos, vistas, funciones y triggers cambiando las referencias
     [Inventario].[dbo]. / Inventario.dbo.  ->  [InventarioPruebas].[dbo]. / InventarioPruebas.dbo.
     para que nada de lo que se haga en pruebas escriba en produccion.
   - Los ID de cajas y productos de prueba empiezan en 900,000,000 (no se confunden con los reales).
   - Deja dada de alta la PO de prueba TYNDALE 991017 (K176T LBL: SMRG, MDRG, LGRG; 3 prendas por caja, 6 por talla).
   ===================================================================================== */
SET NOCOUNT ON;
GO
IF DB_ID('InventarioPruebas') IS NOT NULL
BEGIN
    RAISERROR('La base InventarioPruebas ya existe. Si quieres crearla de nuevo, primero borrala a mano (DROP DATABASE InventarioPruebas).', 16, 1);
    SET NOEXEC ON;   -- no ejecuta el resto del script
END
GO
CREATE DATABASE InventarioPruebas;
GO
ALTER DATABASE InventarioPruebas SET RECOVERY SIMPLE;
GO
USE Inventario;   -- solo para LEER la estructura de produccion
GO
DECLARE @sql nvarchar(max), @tabla sysname, @errores int = 0;

/* ---------- 1. Tablas ---------- */
DECLARE @soloEstructura TABLE (nombre sysname PRIMARY KEY);
INSERT @soloEstructura VALUES ('Inventario'), ('InventarioDetalle'), ('catProductos'), ('idBajaPO'),
                              ('ubicacionDetalle'), ('Prepack'), ('PrepackDetalle');

DECLARE cTablas CURSOR LOCAL FAST_FORWARD FOR
    SELECT name FROM sys.tables
    WHERE is_ms_shipped = 0 AND name <> 'sysdiagrams' AND name NOT LIKE 'Respaldo%'
    ORDER BY name;
OPEN cTablas; FETCH cTablas INTO @tabla;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'SELECT * INTO [InventarioPruebas].[dbo].' + QUOTENAME(@tabla) +
               N' FROM [Inventario].[dbo].' + QUOTENAME(@tabla) +
               CASE WHEN EXISTS (SELECT 1 FROM @soloEstructura WHERE nombre = @tabla) THEN N' WHERE 1 = 0' ELSE N'' END;
    BEGIN TRY EXEC (@sql); PRINT 'Tabla ' + @tabla; END TRY
    BEGIN CATCH SET @errores += 1; PRINT 'ERROR tabla ' + @tabla + ': ' + ERROR_MESSAGE(); END CATCH
    FETCH cTablas INTO @tabla;
END
CLOSE cTablas; DEALLOCATE cTablas;

/* ---------- 2. Valores por default y llaves primarias ---------- */
DECLARE cDef CURSOR LOCAL FAST_FORWARD FOR
    SELECT N'ALTER TABLE dbo.' + QUOTENAME(t.name) + N' ADD DEFAULT ' + dc.definition + N' FOR ' + QUOTENAME(c.name)
    FROM sys.default_constraints dc
    JOIN sys.tables t ON t.object_id = dc.parent_object_id
    JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
    WHERE t.name <> 'sysdiagrams' AND t.name NOT LIKE 'Respaldo%'
    UNION ALL
    SELECT N'ALTER TABLE dbo.' + QUOTENAME(t.name) + N' ADD PRIMARY KEY (' +
           STUFF((SELECT N',' + QUOTENAME(c.name) FROM sys.index_columns ic
                  JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                  WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id ORDER BY ic.key_ordinal
                  FOR XML PATH('')), 1, 1, N'') + N')'
    FROM sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id
    WHERE i.is_primary_key = 1 AND t.name <> 'sysdiagrams' AND t.name NOT LIKE 'Respaldo%';
OPEN cDef; FETCH cDef INTO @sql;
WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY EXEC InventarioPruebas.sys.sp_executesql @sql; END TRY
    BEGIN CATCH SET @errores += 1; PRINT 'ERROR ' + @sql + ': ' + ERROR_MESSAGE(); END CATCH
    FETCH cDef INTO @sql;
END
CLOSE cDef; DEALLOCATE cDef;

/* ---------- 3. Vistas, funciones, procedimientos y triggers ---------- */
DECLARE @modulos TABLE (orden int, nombre sysname, def nvarchar(max), listo bit DEFAULT 0, error nvarchar(4000));
INSERT @modulos (orden, nombre, def)
SELECT CASE o.type WHEN 'FN' THEN 1 WHEN 'IF' THEN 1 WHEN 'TF' THEN 1 WHEN 'V' THEN 2 WHEN 'P' THEN 3 ELSE 4 END,
       o.name,
       REPLACE(REPLACE(m.definition, N'[Inventario].[dbo].', N'[InventarioPruebas].[dbo].'),
                                     N'Inventario.dbo.',     N'InventarioPruebas.dbo.')
FROM sys.sql_modules m JOIN sys.objects o ON o.object_id = m.object_id
WHERE o.is_ms_shipped = 0 AND o.name NOT LIKE 'sp_%diagram%' AND o.name NOT LIKE 'fn_diagram%';

-- Varias vueltas por si una vista o funcion depende de otra que todavia no existe
DECLARE @vuelta int = 1, @nombre sysname, @def nvarchar(max);
WHILE @vuelta <= 3 AND EXISTS (SELECT 1 FROM @modulos WHERE listo = 0)
BEGIN
    DECLARE cMod CURSOR LOCAL FAST_FORWARD FOR SELECT nombre, def FROM @modulos WHERE listo = 0 ORDER BY orden, nombre;
    OPEN cMod; FETCH cMod INTO @nombre, @def;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            EXEC InventarioPruebas.sys.sp_executesql @def;
            UPDATE @modulos SET listo = 1, error = NULL WHERE nombre = @nombre;
        END TRY
        BEGIN CATCH
            UPDATE @modulos SET error = ERROR_MESSAGE() WHERE nombre = @nombre;
        END CATCH
        FETCH cMod INTO @nombre, @def;
    END
    CLOSE cMod; DEALLOCATE cMod;
    SET @vuelta += 1;
END
SELECT @errores += COUNT(*) FROM @modulos WHERE listo = 0;
SELECT nombre AS modulo_con_error, error FROM @modulos WHERE listo = 0;

/* ---------- 4. ID de prueba arriba de 900,000,000 ---------- */
DECLARE cIdent CURSOR LOCAL FAST_FORWARD FOR
    SELECT N'DBCC CHECKIDENT (''dbo.' + t.name + N''', RESEED, 900000000) WITH NO_INFOMSGS'
    FROM InventarioPruebas.sys.tables t
    WHERE t.name IN (SELECT nombre FROM @soloEstructura)
      AND EXISTS (SELECT 1 FROM InventarioPruebas.sys.identity_columns ic WHERE ic.object_id = t.object_id);
OPEN cIdent; FETCH cIdent INTO @sql;
WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY EXEC InventarioPruebas.sys.sp_executesql @sql; END TRY
    BEGIN CATCH SET @errores += 1; PRINT 'ERROR ' + @sql + ': ' + ERROR_MESSAGE(); END CATCH
    FETCH cIdent INTO @sql;
END
CLOSE cIdent; DEALLOCATE cIdent;

/* ---------- 5. PO de prueba TYNDALE 991017 ---------- */
DECLARE @idUsuario int = (SELECT TOP 1 id FROM [InventarioPruebas].[dbo].[usuarios] ORDER BY id);
DECLARE @t1 int = (SELECT TOP 1 id FROM [InventarioPruebas].[dbo].[tallas] WHERE tipo = 'TYNDALE' AND estilo = 'K176T' AND color = 'LBL' AND size_izquierdo = 'SMRG');
DECLARE @t2 int = (SELECT TOP 1 id FROM [InventarioPruebas].[dbo].[tallas] WHERE tipo = 'TYNDALE' AND estilo = 'K176T' AND color = 'LBL' AND size_izquierdo = 'MDRG');
DECLARE @t3 int = (SELECT TOP 1 id FROM [InventarioPruebas].[dbo].[tallas] WHERE tipo = 'TYNDALE' AND estilo = 'K176T' AND color = 'LBL' AND size_izquierdo = 'LGRG');
DECLARE @r TABLE (respuesta int);
INSERT @r EXEC [InventarioPruebas].[dbo].[GuardarTYNDALE] '991017', 3, @t1, @idUsuario, '840160731860', 'LBL', 'K176T', 6;
INSERT @r EXEC [InventarioPruebas].[dbo].[GuardarTYNDALE] '991017', 3, @t2, @idUsuario, '840160731853', 'LBL', 'K176T', 6;
INSERT @r EXEC [InventarioPruebas].[dbo].[GuardarTYNDALE] '991017', 3, @t3, @idUsuario, '840160731839', 'LBL', 'K176T', 6;

/* ---------- 6. Resumen ---------- */
SELECT 'Tablas' AS que, COUNT(*) AS cuantas FROM InventarioPruebas.sys.tables
UNION ALL SELECT 'Procedimientos/vistas/funciones/triggers', COUNT(*) FROM InventarioPruebas.sys.sql_modules
UNION ALL SELECT 'Tallas de la PO 991017', COUNT(*) FROM [InventarioPruebas].[dbo].[catProductos] WHERE po_numero = '991017'
UNION ALL SELECT 'Cajas (debe ser 0)', COUNT(*) FROM [InventarioPruebas].[dbo].[Inventario]
UNION ALL SELECT 'Referencias que quedaron apuntando a produccion (debe ser 0)', COUNT(*)
          FROM InventarioPruebas.sys.sql_modules
          WHERE definition LIKE '%[[]Inventario].%' OR definition LIKE '%Inventario.dbo.%';
PRINT CASE WHEN @errores = 0 THEN 'LISTO: InventarioPruebas creada sin errores.'
           ELSE 'Terminado con ' + CAST(@errores AS varchar(10)) + ' error(es): revisa los mensajes de arriba.' END;
GO
SET NOEXEC OFF;
GO
