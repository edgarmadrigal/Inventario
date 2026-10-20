USE [Inventario]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Edgar Madrigal
-- Create date: 2026-09-09
-- Description:	Consulta el detalle de una caja mixta por id_Inventario
-- =============================================
CREATE PROCEDURE [dbo].[ConsultaInventarioDetalle]
@id_Inventario bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.id,
        d.id_Inventario,
        d.Estilo as estilo,
        d.po_item as poItem,
        d.po_numero as po,
        d.cantidad as Cantidad,
        d.size_izquierdo,
        d.size_derecho,
        d.upc,
        d.fecha,
        cp.color as color
    FROM [Inventario].[dbo].[InventarioDetalle] d
    LEFT JOIN [Inventario].[dbo].[catProductos] cp
        ON cp.po_numero = d.po_numero
        AND cp.size_izquierdo = d.size_izquierdo
        AND cp.estilo = d.Estilo
    WHERE d.id_Inventario = @id_Inventario
END
