-- Fase6: baja de talla por estilo.
-- BajaTallaPO solo recibia PO y talla, asi que borraba esa talla de TODOS los estilos de la PO.
-- BajaTallaEstilo filtra por PO + POItem + estilo + talla (misma regla de tallas que ConsultaSizes)
-- y no deja dar de baja si la talla tiene cajas activas (Inventario o detalle de cajas mixtas).
--   @soloConsultar = 1 -> no borra, solo regresa cuantos productos y cajas hay.
-- Resultado: 1 = borrado, 2 = solo consulta, 0 = no se encontro la talla, -1 = bloqueado por cajas activas.
-- BajaTallaPO se deja igual por compatibilidad.

IF OBJECT_ID('dbo.BajaTallaEstilo') IS NULL
    EXEC('CREATE PROCEDURE dbo.BajaTallaEstilo AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.BajaTallaEstilo
    @po            VARCHAR(150),
    @poItem        VARCHAR(150),
    @estilo        VARCHAR(150),
    @talla         VARCHAR(150),
    @idUser        INT,
    @soloConsultar BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

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

    DECLARE @numProductos INT = (SELECT COUNT(*) FROM @productos);

    DECLARE @numCajas INT =
        (SELECT COUNT(*) FROM dbo.Inventario i
         WHERE i.id_CatProductos IN (SELECT id FROM @productos) AND i.id_status <> 4)
      + (SELECT COUNT(DISTINCT d.id_Inventario) FROM dbo.InventarioDetalle d
         WHERE d.upc IN (SELECT upc FROM @productos WHERE upc IS NOT NULL AND upc <> '')
           AND (d.po_numero = @po
                OR d.po_numero IN (SELECT po_numero FROM @productos)
                OR d.po_numero IN (SELECT po_varchar FROM @productos))
           AND d.id_status <> 4);

    DECLARE @resultado INT =
        CASE WHEN @numProductos = 0 THEN 0
             WHEN @numCajas > 0 THEN -1
             WHEN @soloConsultar = 1 THEN 2
             ELSE 1 END;

    IF @resultado = 1
        DELETE FROM dbo.catProductos WHERE id IN (SELECT id FROM @productos);

    SELECT @resultado AS Resultado, @numProductos AS Productos, @numCajas AS Cajas;
END
GO
