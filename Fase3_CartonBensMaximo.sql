-- Fase 3 (2026-10-12): numero de carton BEN DAVIS repetido.
-- Antes: CartonNumber = COUNT(cajas activas del PO) + 1. Al dar de baja (id_status=4) una caja que no es
-- la ultima, el conteo baja y la siguiente caja recibe un numero que ya tiene otra caja activa.
-- Ahora: MAX(cn_tag_num de cajas activas) + 1, igual que ya calcula la caja mixta (SiguienteCartonBens).
-- Afecta a Inventario viejo y V2 (los dos usan este SP).
--
-- Para regresar al comportamiento anterior, cambiar la linea de CartonNumber por:
--   ,isnull((SELECT count(i.id) from [dbo].Inventario as i where i.po_varchar=@po and ( po_item='55' or  po_item='33') and i.id_status!=4),0)+1 as CartonNumber

ALTER PROCEDURE [dbo].[ConsultaProductosBens]
@po			VARCHAR(MAX),
@size_izquierdo	VARCHAR(MAX),
@size_derecho	VARCHAR(MAX)
AS
BEGIN
SET NOCOUNT ON;

	SELECT
		id as id,
		po_numero PO,
		po_varchar,
		cp.size_izquierdo,
		cp.size_derecho,
		upc as UPC,
		cantidad as Cantidad,
		ctn_type_cd as TipoCarton,
		prod_cd as Estilo,
		color as Color,
		itemDescription as itemDescription
		,CAST(isnull((SELECT max(i.cn_tag_num) from [dbo].Inventario as i where i.po_varchar=@po and ( po_item='55' or  po_item='33') and i.id_status!=4),0)+1 AS int) as CartonNumber
		FROM [dbo].catProductos cp
	WHERE
		 po_varchar =@po
	and  cp.size_izquierdo=@size_izquierdo
	and  cp.size_derecho=@size_derecho
	and  (po_item='55' or  po_item='33')
	ORDER BY
	 create_dtm DESC

END
GO

