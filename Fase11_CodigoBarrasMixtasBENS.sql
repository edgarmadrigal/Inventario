-- Fase11: codigo de barras en renglones de cajas mixtas BENS (po_item 55) del reporte de inventario
-- Fase5: en el reporte de inventario las tallas de cajas mixtas TYNDALE (po_item 11) salian vacias.
-- InventarioDetalle guarda size_derecho en NULL y size_izquierdo + ' x ' + NULL da NULL.
-- Ahora, si size_derecho viene vacio o NULL, se muestra solo size_izquierdo.
-- Respaldo de la version anterior: Respaldo_ConsultaInventario_2026-10-19.sql
ALTER PROCEDURE [dbo].[ConsultaInventario]
    @fechaInicio DATETIME,
    @fechaFin    DATETIME,
    @marca       VARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFORMAT dmy;
    DECLARE @desde DATETIME = CAST(@fechaInicio AS DATE);
    DECLARE @hasta DATETIME = DATEADD(DAY, 1, CAST(@fechaFin AS DATE));
    IF @marca = '' SET @marca = NULL;

    DECLARE @tablaInventario TABLE
    (ID varchar(50), PO varchar(350), poVarchar varchar(250), POITEM varchar(50),
     CANTIDAD numeric(18,0), SIZE varchar(100), UPC varchar(50),
     CARTONNUMBER varchar(50), PRODUCTCODE varchar(50), CARTONTYPE varchar(50),
     CREATEDATE datetime, USERCREATE varchar(250), CLIENTE varchar(250),
     FACTURA varchar(250), TERMINADO varchar(250), ESTATUS int,
     codigodeBarras varchar(250), MARCA varchar(150));

    INSERT INTO @tablaInventario
    SELECT Convert(varchar,i.ID) AS ID,
        try_cast((CASE WHEN I.[po_item]='44' then i.po_varchar else try_cast(i.[po_numero] as varchar(max)) end) as varchar(max)) as PO,
        Convert(varchar(250),ISNULL(i.po_varchar,'0')) as poVarchar,
        Convert(varchar,i.[po_item]) AS POITEM,
        i.cantidad AS CANTIDAD,
        CASE WHEN i.[po_item]='77' OR i.[po_item]='22' THEN i.[size_izquierdo] ELSE i.[size_izquierdo]+ISNULL(' x '+NULLIF(LTRIM(RTRIM(i.[size_derecho])),''),'') END AS SIZE,
        Convert(varchar,i.[upc]) AS UPC,
        Convert(varchar,[cn_tag_num]) AS CARTONNUMBER,
        Convert(varchar,[prod_cd]) AS PRODUCTCODE,
        Convert(varchar,[ctn_type_cd]) AS CARTONTYPE,
        [create_dtm] AS CREATEDATE,
        u.usuario AS USERCREATE,
        'NA' AS CLIENTE, 'NA' AS FACTURA, 'NA' AS TERMINADO,
        i.id_status ESTATUS,
        try_cast(CONCAT(CASE WHEN i.po_varchar like '%AI%' THEN i.po_varchar ELSE CONCAT('AI',i.po_varchar) END,'|',i.prod_cd,'|',i.[size_izquierdo],'|',i.size_derecho,'|',i.cantidad,'|',RIGHT(REPLICATE('0',5)+try_CAST(i.cn_tag_num AS VARCHAR(5)),5)) as varchar) as codigodeBarras,
        m.nombre AS MARCA
    FROM [Inventario].[dbo].[Inventario] as i
    LEFT JOIN usuarios as u ON u.id=i.create_iduser
    OUTER APPLY (SELECT TOP 1 cm.nombre FROM [Inventario].[dbo].[clienteMarca] cm
                 WHERE cm.clave = i.po_item
                 ORDER BY CASE WHEN cm.nombre='LEVIS' THEN 1 ELSE 0 END, cm.id) as m
    WHERE i.create_dtm >= @desde AND i.create_dtm < @hasta
      AND i.id_status != 4
      AND i.size_izquierdo <> 'MIXTO'
      AND (@marca IS NULL OR m.nombre = @marca);

    INSERT INTO @tablaInventario
    SELECT i.id_Inventario AS ID,
        Convert(varchar(350),CASE WHEN I.[po_item]='44' THEN i.po_numero ELSE Convert(varchar(350),i.[po_numero]) END) as PO,
        Convert(varchar(250),ISNULL(i.po_numero,'0')) as poVarchar,
        Convert(varchar,i.[po_item]) AS POITEM,
        i.cantidad AS CANTIDAD,
        CASE WHEN i.[po_item]='77' OR i.[po_item]='22' THEN i.[size_izquierdo] ELSE i.[size_izquierdo]+ISNULL(' x '+NULLIF(LTRIM(RTRIM(i.[size_derecho])),''),'') END AS SIZE,
        Convert(varchar,i.[upc]) AS UPC,
        Convert(varchar,ISNULL(p.cn_tag_num,'')) AS CARTONNUMBER,
        Convert(varchar,COALESCE(NULLIF(i.Estilo,''), p.prod_cd, '')) AS PRODUCTCODE,
        'MIXTO' AS CARTONTYPE,
        fecha AS CREATEDATE,
        ISNULL(u.usuario,'') AS USERCREATE,
        'NA' AS CLIENTE, 'NA' AS FACTURA, 'NA' AS TERMINADO,
        i.id_status ESTATUS,
        CASE WHEN i.[po_item]='55' THEN CONCAT(CASE WHEN i.po_numero LIKE '%AI%' THEN i.po_numero ELSE CONCAT('AI',i.po_numero) END,'|',COALESCE(NULLIF(i.Estilo,''),p.prod_cd),'|',i.[size_izquierdo],'|',i.size_derecho,'|',i.cantidad,'|',RIGHT(REPLICATE('0',5)+TRY_CAST(p.cn_tag_num AS VARCHAR(5)),5)) ELSE '' END as codigodeBarras,
        m.nombre AS MARCA
    FROM [Inventario].[dbo].[InventarioDetalle] as i
    LEFT JOIN [Inventario].[dbo].[Inventario] as p ON p.id = i.id_Inventario
    LEFT JOIN usuarios as u ON u.id = p.create_iduser
    OUTER APPLY (SELECT TOP 1 cm.nombre FROM [Inventario].[dbo].[clienteMarca] cm
                 WHERE cm.clave = i.po_item
                 ORDER BY CASE WHEN cm.nombre='LEVIS' THEN 1 ELSE 0 END, cm.id) as m
    WHERE i.fecha >= @desde AND i.fecha < @hasta
      AND i.id_status != 4
      AND (@marca IS NULL OR m.nombre = @marca);

    SELECT * FROM @tablaInventario ORDER BY CREATEDATE ASC OPTION (RECOMPILE)
END

