-- Fase8: borrar todas las cajas escaneadas de una PO SIN borrar la PO (catalogo, estilos y tallas se quedan
-- para volver a escanear).
--  - Da de baja (id_status = 4) todas las cajas activas de la PO, incluidas las cajas mixtas y su detalle.
--  - Regresa a 0 el contador catProductos.escaneado de la PO (es el conteo de cajas activas por talla).
--  - Queda en BitacoraBajas con tipo 'CAJAS PO' y los ids de las cajas.
--   @soloConsultar = 1 -> no borra, solo regresa cuantas cajas hay.
-- Resultado: 1 = borrado, 2 = solo consulta, 0 = la PO no tiene cajas activas.

IF OBJECT_ID('dbo.BajaCajasPO') IS NULL
    EXEC('CREATE PROCEDURE dbo.BajaCajasPO AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.BajaCajasPO
    @po             VARCHAR(150),
    @idUser         INT,
    @soloConsultar  BIT = 0,
    @maquina        VARCHAR(250) = NULL,
    @ip             VARCHAR(250) = NULL,
    @usuarioWindows VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @cajas TABLE (id INT PRIMARY KEY);
    INSERT INTO @cajas (id)
    SELECT i.id FROM dbo.Inventario i
    WHERE (i.po_varchar = @po OR i.po_numero = @po) AND i.id_status <> 4;

    DECLARE @numCajas INT = (SELECT COUNT(*) FROM @cajas);
    DECLARE @numPrendas INT = ISNULL((SELECT SUM(i.cantidad) FROM dbo.Inventario i WHERE i.id IN (SELECT id FROM @cajas) AND ISNULL(i.mixto, 0) = 0), 0)
                            + ISNULL((SELECT SUM(d.cantidad) FROM dbo.InventarioDetalle d WHERE d.id_Inventario IN (SELECT id FROM @cajas) AND d.id_status <> 4), 0);

    DECLARE @resultado INT =
        CASE WHEN @numCajas = 0 THEN 0
             WHEN @soloConsultar = 1 THEN 2
             ELSE 1 END;

    IF @resultado = 1
    BEGIN
        DECLARE @ids VARCHAR(MAX) = STUFF((SELECT ',' + CAST(id AS VARCHAR(20)) FROM @cajas ORDER BY id FOR XML PATH('')), 1, 1, '');

        BEGIN TRAN;

        UPDATE i SET i.id_status = 4, i.update_dtm = GETDATE(), i.update_iduser = @idUser
        FROM dbo.Inventario i WHERE i.id IN (SELECT id FROM @cajas);

        UPDATE d SET d.id_status = 4
        FROM dbo.InventarioDetalle d WHERE d.id_Inventario IN (SELECT id FROM @cajas) AND d.id_status <> 4;

        UPDATE c SET c.escaneado = 0
        FROM dbo.catProductos c WHERE c.po_varchar = @po OR c.po_numero = @po;

        INSERT INTO dbo.BitacoraBajas (tipo, po, productos, cajas, idsCajas, id_usuario, usuario, maquina, ip, usuarioWindows)
        SELECT 'CAJAS PO', @po, NULL, @numCajas, @ids, @idUser,
               (SELECT TOP 1 u.usuario FROM dbo.usuarios u WHERE u.id = @idUser), @maquina, @ip, @usuarioWindows;

        COMMIT;
    END

    SELECT @resultado AS Resultado, @numCajas AS Cajas, @numPrendas AS Prendas;
END
GO
