USE [Inventario]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Fase 1 - Reportes con marca y mas rapidos
-- Compatible con SQL Server 2012.
--
-- 1) ConsultaInventario y ConsultaInventarioTallaSeparada:
--    - Nueva columna MARCA (de clienteMarca por po_item).
--    - Nuevo parametro opcional @marca (NULL = todas). La version actual de la app
--      sigue funcionando porque el parametro tiene valor por omision.
--    - El filtro de fechas ya no convierte create_dtm a texto: antes recorria toda la
--      tabla Inventario en cada consulta (medido: 656 ms -> 78 ms para 30 dias, mismas filas).
--    - ConsultaInventarioTallaSeparada fallaba con "Arithmetic overflow" cuando habia cartones
--      de mas de 5 digitos (el reporte POTallasSeparadas salia vacio); se usa TRY_CAST.
-- 2) Indice opcional sobre Inventario.create_dtm para que el filtro por fecha sea todavia mas rapido.
--
-- Nota: clienteMarca tiene claves repetidas (77 = LEVIS y ZUMIES, 1000 = LEVIS y CINTAS).
-- Para no duplicar filas se toma una sola marca por clave, prefiriendo la que no es LEVIS.
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Inventario_create_dtm' AND object_id = OBJECT_ID('dbo.Inventario'))
    CREATE NONCLUSTERED INDEX IX_Inventario_create_dtm ON dbo.Inventario (create_dtm) INCLUDE (id_status, po_item);
GO

-- =============================================
-- Author:		Edgar Madrigal
-- Create date: 2018-10-15
-- Description:	Consulta en Inventario
--declare @fechaInicio datetime
--declare @fechaFin datetime
--set @fechaInicio='2026/06/01'
--set @fechaFin='2026/08/03'
--EXEC [dbo].[ConsultaInventario] @fechaInicio,@fechaFin
--EXEC [dbo].[ConsultaInventario] @fechaInicio,@fechaFin,'TYNDALE'
-- =============================================
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
      ,'' AS CARTONNUMBER
      ,Estilo AS PRODUCTCODE
      ,'MIXTO' AS CARTONTYPE
      ,fecha AS CREATEDATE
      ,'' AS USERCREATE
      ,'NA' AS CLIENTE
      ,'NA'AS FACTURA
      ,'NA' AS TERMINADO
	  ,i.id_status ESTATUS
      ,'' as codigodeBarras
      ,m.nombre AS MARCA
  FROM [Inventario].[dbo].[InventarioDetalle] as i
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

-- =============================================
-- Author:		Edgar Madrigal
-- Create date: 2018-10-15
-- Description:	Consulta en Inventario
-- EXEC ConsultaInventarioTallaSeparada '21/11/2024 12:00:00','26/11/2024 12:00:00'
-- EXEC ConsultaInventarioTallaSeparada '21/11/2024 12:00:00','26/11/2024 12:00:00','TYNDALE'
-- =============================================
ALTER PROCEDURE [dbo].[ConsultaInventarioTallaSeparada]
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

	SELECT
	   Convert(varchar,i.ID) AS ID
	  ,Convert(varchar(350),CASE WHEN [po_item]='44' then i.po_varchar else Convert(varchar(350),[po_numero] ) end) as PO
	  ,Convert(varchar(250),ISNULL(i.po_varchar,'0')) as POVARCHAR
      ,Convert(varchar,[po_item]) AS POITEM
      ,[cantidad] AS CANTIDAD
      ,Convert(varchar,[size_izquierdo]) AS SIZE1
      ,Convert(varchar,[size_derecho]) AS SIZE2
      ,Convert(varchar,[upc]) AS UPC
      ,Convert(varchar,[cn_tag_num]) AS CARTONNUMBER
      ,Convert(varchar,[prod_cd]) AS PRODUCTCODE
      ,Convert(varchar,[ctn_type_cd]) AS CARTONTYPE
      ,[create_dtm] AS CREATEDATE
      ,u.usuario AS USERCREATE
      ,Convert(varchar,ISNULL(c.descripcion,'NA')) AS CLIENTE
      ,Convert(varchar,ISNULL(f.descripcion,'NA')) AS FACTURA
      ,Convert(varchar,ISNULL(t.descripcion,'NA')) AS TERMINADO
	  ,i.id_status ESTATUS
      -- TRY_CAST: con CAST el reporte fallaba (overflow) cuando el carton tiene mas de 5 digitos
      ,CONCAT(case when i.po_varchar like '%AI%' then i.po_varchar else CONCAT('AI',i.po_varchar) end,'|',i.prod_cd,'|',i.[size_izquierdo],'|',i.size_derecho,'|',i.cantidad,'|',RIGHT(REPLICATE('0', 5)+ TRY_CAST(i.cn_tag_num AS VARCHAR(5)), 5)) as codigodeBarras
      ,Convert(varchar(150),m.nombre) AS MARCA
  FROM [Inventario].[dbo].[Inventario] as i
  LEFT JOIN Inventario.dbo.usuarios as u on (u.id=i.create_iduser)
  LEFT JOIN Inventario.dbo.cliente as c on (c.id=i.id_cliente)
  LEFT JOIN Inventario.dbo.factura as f on (f.id=i.id_factura)
  LEFT JOIN Inventario.dbo.terminado as t on (t.id=i.id_terminado)
  OUTER APPLY (SELECT TOP 1 cm.nombre FROM [Inventario].[dbo].[clienteMarca] cm
               WHERE cm.clave = i.po_item
               ORDER BY CASE WHEN cm.nombre = 'LEVIS' THEN 1 ELSE 0 END, cm.id) as m
  WHERE
      i.create_dtm >= @desde
  AND i.create_dtm <  @hasta
  AND i.id_status !=4
  AND (@marca IS NULL OR m.nombre = @marca)
	Order by CREATEDATE asc
	OPTION (RECOMPILE)

END
GO

-- Lista de marcas para el combo de filtro de los reportes
IF OBJECT_ID('dbo.ConsultaMarcasReporte') IS NULL
    EXEC('CREATE PROCEDURE dbo.ConsultaMarcasReporte AS BEGIN SET NOCOUNT ON; END');
GO
-- =============================================
-- Description:	Marcas distintas para filtrar reportes (una por clave, igual que ConsultaInventario)
-- =============================================
ALTER PROCEDURE dbo.ConsultaMarcasReporte
AS
BEGIN
	SET NOCOUNT ON;
	SELECT DISTINCT m.nombre AS marca
	FROM (SELECT DISTINCT clave FROM dbo.clienteMarca) k
	CROSS APPLY (SELECT TOP 1 cm.nombre FROM dbo.clienteMarca cm
	             WHERE cm.clave = k.clave
	             ORDER BY CASE WHEN cm.nombre = 'LEVIS' THEN 1 ELSE 0 END, cm.id) m
	ORDER BY m.nombre
END
GO
