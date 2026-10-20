-- Fase7: bitacora de bajas y opcion de borrar tambien lo escaneado al dar de baja una talla.
--  - Tabla BitacoraBajas: quien, cuando, desde que equipo (nombre, IP, usuario de Windows) y que se borro.
--  - RegistrarBitacoraBaja: la usa la app para la baja de PO completa.
--  - BajaTallaEstilo: nuevo @borrarCajas. Con 1, ademas de borrar la talla del catalogo da de baja (id_status = 4)
--    sus cajas activas y, en cajas mixtas, solo los renglones de detalle de esa talla. Todo queda en la bitacora.
-- Resultado de BajaTallaEstilo: 1 = borrado, 3 = borrado junto con cajas, 2 = solo consulta,
--                               0 = no se encontro la talla, -1 = tiene cajas activas y no se pidio borrarlas.

IF OBJECT_ID('dbo.BitacoraBajas') IS NULL
BEGIN
    CREATE TABLE dbo.BitacoraBajas
    (
        id             INT IDENTITY(1,1) PRIMARY KEY,
        fecha          DATETIME      NOT NULL DEFAULT GETDATE(),
        tipo           VARCHAR(30)   NOT NULL,   -- TALLA, TALLA + CAJAS, PO COMPLETA
        po             VARCHAR(150)  NULL,
        poItem         VARCHAR(150)  NULL,
        estilo         VARCHAR(150)  NULL,
        talla          VARCHAR(150)  NULL,
        productos      INT           NULL,
        cajas          INT           NULL,
        idsCajas       VARCHAR(MAX)  NULL,
        id_usuario     INT           NULL,
        usuario        VARCHAR(250)  NULL,
        maquina        VARCHAR(250)  NULL,
        ip             VARCHAR(250)  NULL,
        usuarioWindows VARCHAR(250)  NULL,
        hostSql        VARCHAR(250)  NULL DEFAULT HOST_NAME()
    );
END
GO

IF OBJECT_ID('dbo.RegistrarBitacoraBaja') IS NULL
    EXEC('CREATE PROCEDURE dbo.RegistrarBitacoraBaja AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.RegistrarBitacoraBaja
    @tipo           VARCHAR(30),
    @po             VARCHAR(150),
    @poItem         VARCHAR(150) = NULL,
    @estilo         VARCHAR(150) = NULL,
    @talla          VARCHAR(150) = NULL,
    @productos      INT = NULL,
    @cajas          INT = NULL,
    @idsCajas       VARCHAR(MAX) = NULL,
    @idUser         INT = NULL,
    @maquina        VARCHAR(250) = NULL,
    @ip             VARCHAR(250) = NULL,
    @usuarioWindows VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.BitacoraBajas (tipo, po, poItem, estilo, talla, productos, cajas, idsCajas, id_usuario, usuario, maquina, ip, usuarioWindows)
    SELECT @tipo, @po, @poItem, @estilo, @talla, @productos, @cajas, @idsCajas, @idUser,
           (SELECT TOP 1 u.usuario FROM dbo.usuarios u WHERE u.id = @idUser), @maquina, @ip, @usuarioWindows;
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS id;
END
GO

ALTER PROCEDURE dbo.BajaTallaEstilo
    @po             VARCHAR(150),
    @poItem         VARCHAR(150),
    @estilo         VARCHAR(150),
    @talla          VARCHAR(150),
    @idUser         INT,
    @soloConsultar  BIT = 0,
    @borrarCajas    BIT = 0,
    @maquina        VARCHAR(250) = NULL,
    @ip             VARCHAR(250) = NULL,
    @usuarioWindows VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @productos TABLE (id INT PRIMARY KEY, upc VARCHAR(150), po_numero VARCHAR(150), po_varchar VARCHAR(150));

    INSERT INTO @productos (id, upc, po_numero, po_varchar)
    SELECT cp.id, cp.upc, cp.po_numero, cp.po_varchar
    FROM dbo.catProductos cp
    WHERE (cp.po_varchar = @po OR cp.po_numero = @po)
      AND ((cp.marca = 'IFR' AND cp.estilo = @estilo) OR (cp.po_item = @poItem AND cp.prod_cd = @estilo))
      AND @talla = CASE
                     WHEN cp.marca = 'IFR' OR cp.po_item IN ('77', '88') THEN cp.size_izquierdo
                     WHEN cp.size_derecho IS NULL OR cp.size_derecho = '' OR LTRIM(RTRIM(cp.size_derecho)) = 'x' THEN cp.size_izquierdo
                     ELSE cp.size_izquierdo + ' x ' + cp.size_derecho
                   END;

    -- Cajas normales de la talla. El encabezado MIXTO queda ligado a la primera talla de la caja mixta,
    -- asi que se excluye: de las mixtas solo se dan de baja los renglones de detalle de esta talla.
    DECLARE @cajas TABLE (id INT PRIMARY KEY);
    INSERT INTO @cajas (id)
    SELECT i.id FROM dbo.Inventario i
    WHERE i.id_CatProductos IN (SELECT id FROM @productos) AND i.id_status <> 4
      AND ISNULL(i.size_izquierdo, '') <> 'MIXTO';

    -- Renglones de cajas mixtas con esa talla
    DECLARE @detalle TABLE (id INT PRIMARY KEY, id_Inventario INT);
    INSERT INTO @detalle (id, id_Inventario)
    SELECT d.id, d.id_Inventario FROM dbo.InventarioDetalle d
    WHERE d.upc IN (SELECT upc FROM @productos WHERE upc IS NOT NULL AND upc <> '')
      AND (d.po_numero = @po
           OR d.po_numero IN (SELECT po_numero FROM @productos)
           OR d.po_numero IN (SELECT po_varchar FROM @productos))
      AND d.id_status <> 4;

    DECLARE @numProductos INT = (SELECT COUNT(*) FROM @productos);
    DECLARE @numCajas INT = (SELECT COUNT(*) FROM @cajas)
                          + (SELECT COUNT(DISTINCT id_Inventario) FROM @detalle WHERE id_Inventario NOT IN (SELECT id FROM @cajas));

    DECLARE @resultado INT =
        CASE WHEN @numProductos = 0 THEN 0
             WHEN @soloConsultar = 1 THEN 2
             WHEN @numCajas > 0 AND @borrarCajas = 0 THEN -1
             WHEN @numCajas > 0 THEN 3
             ELSE 1 END;

    IF @resultado IN (1, 3)
    BEGIN
        DECLARE @ids VARCHAR(MAX) = STUFF((
            SELECT ',' + CAST(x.id AS VARCHAR(20)) FROM
              (SELECT id FROM @cajas UNION SELECT id_Inventario FROM @detalle) x
            ORDER BY x.id FOR XML PATH('')), 1, 1, '');

        BEGIN TRAN;

        IF @resultado = 3
        BEGIN
            UPDATE i SET i.id_status = 4, i.update_dtm = GETDATE(), i.update_iduser = @idUser
            FROM dbo.Inventario i WHERE i.id IN (SELECT id FROM @cajas);

            UPDATE d SET d.id_status = 4
            FROM dbo.InventarioDetalle d WHERE d.id IN (SELECT id FROM @detalle);
        END

        DELETE FROM dbo.catProductos WHERE id IN (SELECT id FROM @productos);

        INSERT INTO dbo.BitacoraBajas (tipo, po, poItem, estilo, talla, productos, cajas, idsCajas, id_usuario, usuario, maquina, ip, usuarioWindows)
        SELECT CASE WHEN @resultado = 3 THEN 'TALLA + CAJAS' ELSE 'TALLA' END,
               @po, @poItem, @estilo, @talla, @numProductos, @numCajas, @ids, @idUser,
               (SELECT TOP 1 u.usuario FROM dbo.usuarios u WHERE u.id = @idUser), @maquina, @ip, @usuarioWindows;

        COMMIT;
    END

    SELECT @resultado AS Resultado, @numProductos AS Productos, @numCajas AS Cajas;
END
GO
