-- Fase10: corregir datos de una PO (descripcion, color, estilo, vendor, ShipTo, UPC) en el catalogo
-- y en lo ya escaneado.
--  - ConsultaProductosCorreccion: renglones de catProductos de la PO con sus cajas activas.
--  - ModificarProductoPO: cambia un renglon de catProductos y lo propaga a Inventario (id_CatProductos)
--    y a InventarioDetalle (cajas mixtas, por UPC anterior + PO). Cada campo cambiado va a BitacoraCambios.

IF OBJECT_ID('dbo.BitacoraCambios') IS NULL
BEGIN
    CREATE TABLE dbo.BitacoraCambios
    (
        id              INT IDENTITY(1,1) PRIMARY KEY,
        fecha           DATETIME      NOT NULL DEFAULT GETDATE(),
        id_catProductos INT           NULL,
        po              VARCHAR(350)  NULL,
        talla           VARCHAR(500)  NULL,
        campo           VARCHAR(50)   NOT NULL,
        valorAnterior   VARCHAR(MAX)  NULL,
        valorNuevo      VARCHAR(MAX)  NULL,
        cajasAfectadas  INT           NULL,
        id_usuario      INT           NULL,
        usuario         VARCHAR(250)  NULL,
        maquina         VARCHAR(250)  NULL,
        ip              VARCHAR(250)  NULL,
        usuarioWindows  VARCHAR(250)  NULL
    );
END
GO

IF OBJECT_ID('dbo.ConsultaProductosCorreccion') IS NULL
    EXEC('CREATE PROCEDURE dbo.ConsultaProductosCorreccion AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.ConsultaProductosCorreccion
    @po VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.id,
           ISNULL(c.po_item, '') AS po_item,
           ISNULL(c.marca, '') AS marca,
           CASE WHEN c.size_derecho IS NULL OR c.size_derecho = '' OR LTRIM(RTRIM(c.size_derecho)) = 'x'
                THEN ISNULL(c.size_izquierdo, '') ELSE c.size_izquierdo + ' x ' + c.size_derecho END AS talla,
           ISNULL(c.prod_cd, '') AS estilo,
           ISNULL(c.color, '') AS color,
           ISNULL(c.itemDescription, '') AS descripcion,
           ISNULL(c.vendor, '') AS vendor,
           ISNULL(c.ShipTo, '') AS shipTo,
           ISNULL(c.upc, '') AS upc,
           CAST(ISNULL(c.cantidad, 0) AS INT) AS cantidad,
           (SELECT COUNT(*) FROM dbo.Inventario i WHERE i.id_CatProductos = c.id AND i.id_status <> 4) AS cajas
    FROM dbo.catProductos c
    WHERE c.po_varchar = @po OR c.po_numero = @po
    ORDER BY c.po_item, c.prod_cd, c.id;
END
GO

IF OBJECT_ID('dbo.ModificarProductoPO') IS NULL
    EXEC('CREATE PROCEDURE dbo.ModificarProductoPO AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.ModificarProductoPO
    @id             INT,
    @estilo         VARCHAR(150),
    @color          VARCHAR(250),
    @descripcion    VARCHAR(250),
    @vendor         VARCHAR(MAX),
    @shipTo         VARCHAR(MAX),
    @upc            VARCHAR(150),
    @idUser         INT,
    @maquina        VARCHAR(250) = NULL,
    @ip             VARCHAR(250) = NULL,
    @usuarioWindows VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @poNum VARCHAR(MAX), @poVar VARCHAR(350), @talla VARCHAR(500),
            @aEstilo VARCHAR(150), @aColor VARCHAR(250), @aDesc VARCHAR(250), @aVendor VARCHAR(MAX), @aShip VARCHAR(MAX), @aUpc VARCHAR(150);

    SELECT @poNum = po_numero, @poVar = po_varchar,
           @talla = ISNULL(size_izquierdo, '') + ISNULL(' x ' + NULLIF(LTRIM(RTRIM(size_derecho)), ''), ''),
           @aEstilo = ISNULL(prod_cd, ''), @aColor = ISNULL(color, ''), @aDesc = ISNULL(itemDescription, ''),
           @aVendor = ISNULL(vendor, ''), @aShip = ISNULL(ShipTo, ''), @aUpc = ISNULL(upc, '')
    FROM dbo.catProductos WHERE id = @id;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Resultado, 0 AS Cajas;
        RETURN;
    END

    DECLARE @cajas INT = (SELECT COUNT(*) FROM dbo.Inventario WHERE id_CatProductos = @id);
    DECLARE @usuario VARCHAR(250) = (SELECT TOP 1 u.usuario FROM dbo.usuarios u WHERE u.id = @idUser);

    BEGIN TRAN;

    -- Catalogo (prod_cd y estilo son el mismo dato segun la marca: se mantienen iguales si ya lo estaban)
    UPDATE dbo.catProductos
    SET prod_cd = @estilo,
        estilo = CASE WHEN ISNULL(estilo, '') = @aEstilo THEN @estilo ELSE estilo END,
        color = @color, itemDescription = @descripcion, vendor = @vendor, ShipTo = @shipTo, upc = @upc,
        update_iduser = @idUser, update_dtm = GETDATE()
    WHERE id = @id;

    -- Lo escaneado de ese renglon
    UPDATE dbo.Inventario
    SET prod_cd = @estilo,
        estilo = CASE WHEN ISNULL(estilo, '') = @aEstilo THEN @estilo ELSE estilo END,
        color = @color, itemDescription = @descripcion, vendor = @vendor, ShipTo = @shipTo, upc = @upc,
        update_iduser = @idUser, update_dtm = GETDATE()
    WHERE id_CatProductos = @id;

    -- Detalle de cajas mixtas: se identifica por el UPC anterior y la PO
    IF @aUpc <> ''
        UPDATE dbo.InventarioDetalle
        SET upc = @upc,
            Estilo = CASE WHEN ISNULL(Estilo, '') = @aEstilo THEN @estilo ELSE Estilo END
        WHERE upc = @aUpc AND (po_numero = @poNum OR po_numero = @poVar);

    INSERT INTO dbo.BitacoraCambios (id_catProductos, po, talla, campo, valorAnterior, valorNuevo, cajasAfectadas, id_usuario, usuario, maquina, ip, usuarioWindows)
    SELECT @id, ISNULL(@poVar, @poNum), @talla, x.campo, x.antes, x.despues, @cajas, @idUser, @usuario, @maquina, @ip, @usuarioWindows
    FROM (SELECT 'ESTILO' campo, @aEstilo antes, @estilo despues
          UNION ALL SELECT 'COLOR', @aColor, @color
          UNION ALL SELECT 'DESCRIPCION', @aDesc, @descripcion
          UNION ALL SELECT 'VENDOR', @aVendor, @vendor
          UNION ALL SELECT 'SHIPTO', @aShip, @shipTo
          UNION ALL SELECT 'UPC', @aUpc, @upc) x
    WHERE x.antes <> x.despues;

    COMMIT;

    SELECT 1 AS Resultado, @cajas AS Cajas;
END
GO
