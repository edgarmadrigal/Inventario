-- Fase 2 - Cartón y estilo en los reportes y etiquetas de las cajas MIXTAS (BEN DAVIS)
-- Se puede correr cuantas veces sea; es idempotente.
--
-- Que arregla:
-- 1) El reporte "Reportes" de cajas mixtas aparecia sin CARTONNUMBER ni PRODUCTCODE (los 4
--    renglones del AI1457 BENS). InventarioDetalle no guarda cartón ni estilo; los sacamos
--    del renglon padre dbo.Inventario con id_Inventario.
-- 2) La etiqueta BENS mostraba la descripcion ("GORILLA CUT") en vez del estilo (673). ConsultaUPC_Varchar
--    (lo que llama Funciones.ConsultaUPC) devolvia catProductos.estilo, que para BENS esta NULL. El estilo
--    real esta en catProductos.prod_cd ("673"). Ahora prefiere prod_cd cuando estilo esta vacio.
-- 3) Para los detalles ya guardados sin estilo, los rellena con el prod_cd del padre.

USE [Inventario]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ConsultaUPC_Varchar es el SP que llama Funciones.ConsultaUPC. Preferimos prod_cd cuando estilo esta vacio
-- porque para BENS/ROCKY/etc el estilo real se guarda en prod_cd ("673"); solo algunas marcas (po_item=22)
-- usan la columna estilo. 76,410 de 76,688 productos tienen estilo NULL.
ALTER PROCEDURE [dbo].[ConsultaUPC_Varchar]
@po_numero			VARCHAR(MAX)=NULL,
@upc				VARCHAR(MAX)=NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF (SELECT COUNT(i.id) FROM Inventario.dbo.catProductos as i WHERE i.po_varchar=@po_numero AND upc=@upc) > 0
	BEGIN
		SELECT i.id
			,i.po_varchar as po
			,i.po_item as poItem
			,i.cantidad as Cantidad
			,CASE WHEN i.size_derecho IS NULL OR i.size_derecho=''
				THEN i.size_izquierdo ELSE (i.size_izquierdo +' x '+i.size_derecho) END as Size
			,i.size_izquierdo
			,i.size_derecho
			,i.upc
			,i.create_dtm
			,i.create_iduser
			,i.color
			,COALESCE(NULLIF(i.estilo,''), i.prod_cd) as estilo
		FROM Inventario.dbo.catProductos as i
		WHERE i.po_varchar=@po_numero AND upc=@upc
	END
	ELSE
	BEGIN
		SELECT i.id
			,CONVERT(VARCHAR(MAX),i.po_numero) as po
			,i.po_item as poItem
			,i.cantidad as Cantidad
			,CASE WHEN i.size_derecho IS NULL OR i.size_derecho=''
				THEN i.size_izquierdo ELSE (i.size_izquierdo +' x '+i.size_derecho) END as Size
			,i.size_izquierdo
			,i.size_derecho
			,i.upc
			,i.create_dtm
			,i.create_iduser
			,i.color
			,COALESCE(NULLIF(i.estilo,''), i.prod_cd) as estilo
		FROM Inventario.dbo.catProductos as i
		WHERE i.po_numero=@po_numero AND upc=@upc
	END
END
GO

ALTER PROCEDURE [dbo].[ConsultaInventario]
@fechaInicio				DATETIME		,
@fechaFin					DATETIME		,
@marca						VARCHAR(150)	= NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET DATEFORMAT dmy;

	DECLARE @desde DATETIME = CAST(@fechaInicio AS DATE);
	DECLARE @hasta DATETIME = DATEADD(DAY, 1, CAST(@fechaFin AS DATE));
	IF @marca = '' SET @marca = NULL;

    declare @tablaInventario table
    (ID varchar(50)
    ,PO varchar(350)
    ,poVarchar varchar(250)
    ,POITEM varchar(50)
    ,CANTIDAD numeric(18,0)
    ,SIZE varchar(100)
    ,UPC varchar(50)
    ,CARTONNUMBER varchar(50)
    ,PRODUCTCODE varchar(50)
    ,CARTONTYPE varchar(50)
    ,CREATEDATE datetime
    ,USERCREATE varchar(250)
    ,CLIENTE varchar(250)
    ,FACTURA varchar(250)
    ,TERMINADO varchar(250)
    ,ESTATUS int
    ,codigodeBarras varchar(250)
    ,MARCA varchar(150))

    INSERT INTO @tablaInventario
	SELECT
	   Convert(varchar,i.ID) AS ID
	   ,try_cast((CASE WHEN I.[po_item]='44' then i.po_varchar else try_cast(i.[po_numero] as  varchar(max)) end) as varchar(max)) as PO
	  ,Convert(varchar(250),ISNULL(i.po_varchar,'0')) as poVarchar
      ,Convert(varchar,i.[po_item]) AS POITEM
      ,i.cantidad AS CANTIDAD
      , case when i.[po_item]='77' or i.[po_item]='22'  then i.[size_izquierdo] else i.[size_izquierdo]+' x '+i.[size_derecho] end  AS SIZE
      ,Convert(varchar,i.[upc])  AS UPC
      ,Convert(varchar,[cn_tag_num]) AS CARTONNUMBER
      ,Convert(varchar,[prod_cd]) AS PRODUCTCODE
      ,Convert(varchar,[ctn_type_cd]) AS CARTONTYPE
      ,[create_dtm] AS CREATEDATE
      ,u.usuario AS USERCREATE
      ,'NA' AS CLIENTE
      ,'NA'AS FACTURA
      ,'NA' AS TERMINADO
	  ,i.id_status ESTATUS
      ,try_cast(CONCAT(case when i.po_varchar like '%AI%' then i.po_varchar else CONCAT('AI',i.po_varchar) end,'|',i.prod_cd,'|',i.[size_izquierdo],'|',i.size_derecho,'|',i.cantidad,'|',RIGHT(REPLICATE('0', 5)+ try_CAST(i.cn_tag_num AS VARCHAR(5)), 5)) as varchar) as codigodeBarras
      ,m.nombre AS MARCA
  FROM [Inventario].[dbo].[Inventario] as i
  LEFT JOIN usuarios as u on (u.id=i.create_iduser)
  OUTER APPLY (SELECT TOP 1 cm.nombre FROM [Inventario].[dbo].[clienteMarca] cm
               WHERE cm.clave = i.po_item
               ORDER BY CASE WHEN cm.nombre = 'LEVIS' THEN 1 ELSE 0 END, cm.id) as m
  WHERE
      i.create_dtm >= @desde
  AND i.create_dtm <  @hasta
  AND i.id_status !=4
  AND i.size_izquierdo<>'MIXTO'
  AND (@marca IS NULL OR m.nombre = @marca)

    INSERT INTO @tablaInventario
    SELECT i.id_Inventario AS  ID
	   ,Convert(varchar(350),CASE WHEN I.[po_item]='44' then i.po_numero else Convert(varchar(350),i.[po_numero] ) end) as PO
	  ,Convert(varchar(250),ISNULL(i.po_numero,'0')) as poVarchar
      ,Convert(varchar,i.[po_item]) AS POITEM
      ,i.cantidad  AS CANTIDAD
      ,case when i.[po_item]='77' or i.[po_item]='22'  then i.[size_izquierdo] else i.[size_izquierdo]+' x '+i.[size_derecho] end  AS SIZE
      ,Convert(varchar,i.[upc])  AS UPC
      ,Convert(varchar,ISNULL(p.cn_tag_num,'')) AS CARTONNUMBER
      ,Convert(varchar,COALESCE(NULLIF(i.Estilo,''), p.prod_cd, '')) AS PRODUCTCODE
      ,'MIXTO' AS CARTONTYPE
      ,fecha AS CREATEDATE
      ,ISNULL(u.usuario,'') AS USERCREATE
      ,'NA' AS CLIENTE
      ,'NA' AS FACTURA
      ,'NA' AS TERMINADO
	  ,i.id_status ESTATUS
      ,'' as codigodeBarras
      ,m.nombre AS MARCA
  FROM [Inventario].[dbo].[InventarioDetalle] as i
  LEFT JOIN [Inventario].[dbo].[Inventario] as p ON p.id = i.id_Inventario
  LEFT JOIN usuarios as u on (u.id=p.create_iduser)
  OUTER APPLY (SELECT TOP 1 cm.nombre FROM [Inventario].[dbo].[clienteMarca] cm
               WHERE cm.clave = i.po_item
               ORDER BY CASE WHEN cm.nombre = 'LEVIS' THEN 1 ELSE 0 END, cm.id) as m
  WHERE
      i.fecha >= @desde
  AND i.fecha <  @hasta
  AND i.id_status !=4
  AND (@marca IS NULL OR m.nombre = @marca)

SELECT * FROM @tablaInventario
Order by CREATEDATE asc
OPTION (RECOMPILE)

END
GO

-- Rellena el estilo de los detalles ya guardados que quedaron sin el (p.ej. 4 renglones del AI1457).
UPDATE d
   SET d.Estilo = p.prod_cd
FROM [Inventario].[dbo].[InventarioDetalle] d
JOIN [Inventario].[dbo].[Inventario] p ON p.id = d.id_Inventario
WHERE (d.Estilo IS NULL OR d.Estilo = '')
  AND p.prod_cd IS NOT NULL AND p.prod_cd <> '';
GO
