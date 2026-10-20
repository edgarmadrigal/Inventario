-- =============================================
-- Author:		Edgar Madrigal
-- Create date: 2018-10-11
-- Description:	Guardar en Inventario
-- =============================================
ALTER PROCEDURE [dbo].[GuardarInventarioBeans]
@id_CatProductos	bigINT				,
@po_numero			varchar(max)  ,
@po_varchar			varchar(350)	,
@po_item			varchar(350)	,
@cantidad			numeric(18, 0)  ,
@size_izquierdo		varchar(150)	,
@size_derecho		varchar(150)	,
@upc				varchar(150)	,
@cn_tag_num         int				,
@prod_cd			varchar(150)	,
@create_iduser		int	,	
@color				varchar(150)	,
@itemDescription    varchar(150)	
AS
BEGIN
	--DECLARE		@id_CatProductos	INT				
	--DECLARE		@po_numero			varchar(max)  
	--DECLARE		@po_varchar			varchar(350)	
	--DECLARE		@po_item			varchar(350)	
	--DECLARE		@cantidad			numeric(18, 0)  
	--DECLARE		@size_izquierdo		varchar(150)	
	--DECLARE		@size_derecho		varchar(150)	
	--DECLARE		@upc				varchar(150)	
	--DECLARE		@cn_tag_num         int				
	--DECLARE		@prod_cd			varchar(150)	
	--DECLARE		@create_iduser		int		
	--DECLARE		@color				varchar(150)	
	--DECLARE		@itemDescription    varchar(150)	

	--SET @id_CatProductos	='78590';
	--SET @po_numero			='1294';
	--SET @po_varchar			='1294';
	--SET @po_item			='55';
	--SET @cantidad			=14;
	--SET @size_izquierdo		='34';
	--SET @size_derecho		='32';
	--SET @upc				='715044694473';
	--SET @cn_tag_num			='165';
	--SET @prod_cd			='694';
	--SET @create_iduser		=57;
	--SET @color				='BLACK';
	--SET @itemDescription	='ORIGINAL BENS';



	
	SET NOCOUNT ON;

	/*   NUEVA FUNCIONALIDAD  */



DECLARE @NUEVACAJA INT
		DECLARE @NUMERO_CAJA_MIN INT
		DECLARE @NUMERO_CAJA_ULTIMA INT
		--DECLARE @NUMERO_CAJAS_REQUERIDAS INT =NULL
		DECLARE @cajasEscaneadasporTalla	INT
		SET @cajasEscaneadasporTalla=(SELECT count(*)  
			FROM  [Inventario].[dbo].[Inventario] with (nolock)
			WHERE po_numero=@po_numero and size_izquierdo=@size_izquierdo and size_derecho=@size_derecho
			AND id_status<>4)


		--SET @NUMERO_CAJAS_REQUERIDAS=(convert(int,(select [cantidadCajas]
		--								 FROM [Inventario].[dbo].catProductos with (nolock)
		--								 where po_numero=@po_numero and size_izquierdo=@size_izquierdo and size_derecho=@size_derecho )) )


		SET @NUMERO_CAJA_ULTIMA=(
			(SELECT MAX(i.cn_tag_num)  
			FROM  [Inventario].[dbo].[Inventario] as i  with (nolock)
			WHERE po_numero=@po_numero  and size_izquierdo=@size_izquierdo and size_derecho=@size_derecho
			AND I.id_status<>4))

				 update c
				 set c.totalCajasPO=( select sum([cantidadCajas]) from [Inventario].[dbo].[catProductos] with (nolock) where po_numero=@po_numero and [po_item]=77)
				 from [Inventario].[dbo].[catProductos] as c
				 update c
				 set c.escaneado=( select count(i.id) from [Inventario].[dbo].[Inventario] as i where i.po_numero=@po_numero AND i.id_status<>4 
				 and i.size_izquierdo=@size_izquierdo
				 and i.size_derecho=@size_derecho 
				 )
				 from [Inventario].[dbo].[catProductos] as c
				  where  po_numero=@po_numero and size_izquierdo=@size_izquierdo and size_derecho=@size_derecho 
			
		
		/*nuevo inicio */
		--IF (@NUMERO_CAJAS_REQUERIDAS  IS NULL) OR (@cajasEscaneadasporTalla<@NUMERO_CAJAS_REQUERIDAS          ) 
		--begin

		/*nuevo fin */
		/*ya estaba inicio*/
				SET @NUEVACAJA=ISNULL((SELECT MAX(cn_tag_num)+1 FROM [Inventario].[dbo].[Inventario] as i  with (nolock)
				WHERE i.po_numero=@po_numero 
				AND I.id_status<>4),1)

				 -- Reusar un numero dado de baja solo si ninguna caja activa lo tiene (evita cartones repetidos, 2026-10-12)
				 UPDATE b SET b.[estatus]=0
				 FROM [Inventario].[dbo].[idBajaPO] b
				 WHERE b.po_numero=@po_numero AND b.[estatus]=1
				   AND EXISTS (SELECT 1 FROM [Inventario].[dbo].[Inventario] i WHERE i.po_numero=@po_numero AND i.cn_tag_num=b.numeroCaja AND i.id_status<>4)

				 IF (SELECT MIN([numeroCaja]) FROM [Inventario].[dbo].[idBajaPO] WHERE po_numero=@po_numero AND [estatus]=1)>0
				 BEGIN
					SET @NUEVACAJA=(SELECT MIN([numeroCaja]) FROM [Inventario].[dbo].[idBajaPO] WHERE po_numero=@po_numero AND [estatus]=1)
					UPDATE b SET b.[estatus]=0
					FROM [Inventario].[dbo].[idBajaPO] b
					WHERE b.po_numero=@po_numero AND b.[estatus]=1 AND b.numeroCaja=@NUEVACAJA
				 END

				 update c
				 set c.totalCajasPO=( select sum([cantidadCajas]) from [Inventario].[dbo].[catProductos] with (nolock) where po_numero=@po_numero and [po_item]=77)
				 from [Inventario].[dbo].[catProductos] as c

				 update c
				 set c.escaneado=( select count(i.id) from [Inventario].[dbo].[Inventario] as i where i.po_numero=@po_numero  AND i.id_status<>4 
				 and i.size_izquierdo=@size_izquierdo
				 and i.size_derecho=@size_derecho 
				 )
				 from [Inventario].[dbo].[catProductos] as c
				 where  po_numero=@po_numero and size_izquierdo=@size_izquierdo and size_derecho=@size_derecho 

				 
		INSERT INTO [Inventario].[dbo].[Inventario] 
		(id_CatProductos
		  ,id_status
		  ,po_numero
		  ,po_varchar
		  ,po_item
		  ,cantidad
		  ,size_izquierdo
		  ,size_derecho
		  ,upc
		  ,cn_tag_num
		  ,prod_cd
		  ,ctn_type_cd
		  ,create_dtm
		  ,create_iduser
		  ,itemDescription
		  ,color
		  ,codigoDeBarras)
		VALUES(@id_CatProductos,
				1,
			   @po_numero,
			   @po_varchar,
			   @po_item,
			   @cantidad,
			   @size_izquierdo,
			   @size_derecho,
			   @upc,
			   @NUEVACAJA,
			   @prod_cd,
			   @itemDescription,
			   GETDATE(),
			   @create_iduser,
			   @itemDescription,
			   @color,
			   (select CONCAT(@po_varchar,'|',@prod_cd,'|',@size_izquierdo,'|',@size_derecho,'|',@cantidad,'|',RIGHT(REPLICATE('0', 5)+ CAST(@NUEVACAJA AS VARCHAR(5)), 5)) )
			   )

			   select CONCAT(@po_varchar,'|',@prod_cd,'|',@size_izquierdo,'|',@size_derecho,'|',@cantidad,'|',RIGHT(REPLICATE('0', 5)+ CAST(@NUEVACAJA AS VARCHAR(5)), 5)) as codigoBarrasTexto

			   --CONCAT(@po_varchar,@prod_cd,@size_izquierdo,@size_derecho,@cantidad,RIGHT(REPLICATE('0', 5)+ CAST(@NUEVACAJA AS VARCHAR(5)), 5)) as codigoBarras ,



			   /*   AQUI TERMINA NUEVA FUNCIONALIDAD  */


		/*   FUNCIONALIDAD ANTERIOR  */


			--			SET NOCOUNT ON;
			--		INSERT INTO [Inventario].[dbo].[Inventario] 
			--		(id_CatProductos
			--		  ,id_status
			--		  ,po_numero
			--		  ,po_varchar
			--		  ,po_item
			--		  ,cantidad
			--		  ,size_izquierdo
			--		  ,size_derecho
			--		  ,upc
			--		  ,cn_tag_num
			--		  ,prod_cd
			--		  ,ctn_type_cd
			--		  ,create_dtm
			--		  ,create_iduser
			--		  ,itemDescription
			--		  ,color)
			--		VALUES(@id_CatProductos,
			--				1,
			--			   @po_numero,
			--			   @po_varchar,
			--			   @po_item,
			--			   @cantidad,
			--			   @size_izquierdo,
			--			   @size_derecho,
			--			   @upc,
			--			   CONVERT(INT,(isnull((SELECT count(i.id) FROM  [Inventario].[dbo].[Inventario] i where i.po_item= @po_item and i.po_numero= @po_numero and i.prod_cd= @prod_cd 
			--			 and i.id_status!=4),0)))+1,
			--			   @prod_cd,
			--			   @itemDescription,
			--			   GETDATE(),
			--			   @create_iduser,
			--			   @itemDescription,
			--			   @color
			--			   )

			--SELECT CAST((SELECT @@IDENTITY) AS VARCHAR(max))



/* AQUI TERMINA  FUNCIONALIDAD ANTERIOR	*/
END

