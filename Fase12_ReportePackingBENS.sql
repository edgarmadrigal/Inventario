-- Fase12: packing list de Ben Davis (po_item 55) por PO, para el Excel de la pestana Reporte PO.
-- Un renglon por caja normal y, en cajas mixtas, un renglon por talla (todas con el carton de la caja).
-- Columnas: PO#, Barcode, Box #, Style, Waist, Length, SIZE, QTY. Solo cajas activas (id_status <> 4).

IF OBJECT_ID('dbo.ReportePackingBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.ReportePackingBENS AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.ReportePackingBENS
    @po VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @poAI VARCHAR(150) = CASE WHEN @po LIKE '%AI%' THEN @po ELSE 'AI' + @po END;

    SELECT x.PO, x.Barcode, x.Box, x.Style, x.Waist, x.Length, x.Size, x.Qty, x.Mixta, x.idCaja
    FROM (
        -- Cajas normales
        SELECT @poAI AS PO,
               CONCAT(@poAI, '|', i.prod_cd, '|', i.size_izquierdo, '|', i.size_derecho, '|', CAST(i.cantidad AS INT), '|',
                      RIGHT(REPLICATE('0', 5) + CAST(i.cn_tag_num AS VARCHAR(20)), 5)) AS Barcode,
               CAST(i.cn_tag_num AS INT) AS Box,
               ISNULL(i.prod_cd, '') AS Style,
               ISNULL(i.size_izquierdo, '') AS Waist,
               ISNULL(i.size_derecho, '') AS Length,
               ISNULL(i.size_izquierdo, '') + ISNULL(' x ' + NULLIF(i.size_derecho, ''), '') AS Size,
               CAST(i.cantidad AS INT) AS Qty,
               0 AS Mixta,
               i.id AS idCaja,
               0 AS orden
        FROM dbo.Inventario i
        WHERE (i.po_varchar = @po OR i.po_numero = @po OR i.po_varchar = @poAI OR i.po_numero = @poAI)
          AND i.po_item = '55' AND i.id_status <> 4 AND ISNULL(i.size_izquierdo, '') <> 'MIXTO'

        UNION ALL

        -- Cajas mixtas: un renglon por talla del detalle
        SELECT @poAI,
               CONCAT(@poAI, '|', COALESCE(NULLIF(d.Estilo, ''), p.prod_cd), '|', d.size_izquierdo, '|', d.size_derecho, '|', CAST(d.cantidad AS INT), '|',
                      RIGHT(REPLICATE('0', 5) + CAST(p.cn_tag_num AS VARCHAR(20)), 5)),
               CAST(p.cn_tag_num AS INT),
               ISNULL(COALESCE(NULLIF(d.Estilo, ''), p.prod_cd), ''),
               ISNULL(d.size_izquierdo, ''),
               ISNULL(d.size_derecho, ''),
               ISNULL(d.size_izquierdo, '') + ISNULL(' x ' + NULLIF(d.size_derecho, ''), ''),
               CAST(d.cantidad AS INT),
               1,
               p.id,
               d.id
        FROM dbo.InventarioDetalle d
        INNER JOIN dbo.Inventario p ON p.id = d.id_Inventario
        WHERE (p.po_varchar = @po OR p.po_numero = @po OR p.po_varchar = @poAI OR p.po_numero = @poAI
               OR d.po_numero = @po OR d.po_numero = @poAI)
          AND p.po_item = '55' AND p.id_status <> 4 AND d.id_status <> 4
    ) x
    ORDER BY x.Box, x.idCaja, x.orden;
END
GO
