-- Fase9: borrar SOLO la PO del catalogo (catProductos). Las cajas escaneadas (Inventario / InventarioDetalle)
-- no se tocan. Queda en BitacoraBajas con tipo 'CATALOGO PO'.
--   @soloConsultar = 1 -> no borra, solo regresa cuantos registros de catalogo y cajas activas hay.
-- Resultado: 1 = borrado, 2 = solo consulta, 0 = la PO no esta en el catalogo.

IF OBJECT_ID('dbo.BajaCatalogoPO') IS NULL
    EXEC('CREATE PROCEDURE dbo.BajaCatalogoPO AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.BajaCatalogoPO
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

    DECLARE @numProductos INT = (SELECT COUNT(*) FROM dbo.catProductos c WHERE c.po_varchar = @po OR c.po_numero = @po);
    DECLARE @numCajas INT = (SELECT COUNT(*) FROM dbo.Inventario i WHERE (i.po_varchar = @po OR i.po_numero = @po) AND i.id_status <> 4);

    DECLARE @resultado INT =
        CASE WHEN @numProductos = 0 THEN 0
             WHEN @soloConsultar = 1 THEN 2
             ELSE 1 END;

    IF @resultado = 1
    BEGIN
        BEGIN TRAN;

        DELETE FROM dbo.catProductos WHERE po_varchar = @po OR po_numero = @po;

        INSERT INTO dbo.BitacoraBajas (tipo, po, productos, cajas, id_usuario, usuario, maquina, ip, usuarioWindows)
        SELECT 'CATALOGO PO', @po, @numProductos, 0, @idUser,
               (SELECT TOP 1 u.usuario FROM dbo.usuarios u WHERE u.id = @idUser), @maquina, @ip, @usuarioWindows;

        COMMIT;
    END

    SELECT @resultado AS Resultado, @numProductos AS Productos, @numCajas AS Cajas;
END
GO
