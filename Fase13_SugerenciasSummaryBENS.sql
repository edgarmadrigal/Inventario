-- Fase13: el Summary de Ben Davis recuerda lo que se capturo (como sugerencia para la siguiente vez).
--  - SummaryBENSPedido: ORDER QTY por PO y talla.
--  - SummaryBENSPeso:   peso bruto / neto por prenda por PO. En una PO sin peso se sugiere el ultimo usado.

IF OBJECT_ID('dbo.SummaryBENSPedido') IS NULL
    CREATE TABLE dbo.SummaryBENSPedido
    (
        po         VARCHAR(150) NOT NULL,
        talla      VARCHAR(150) NOT NULL,
        pedido     INT          NOT NULL,
        id_usuario INT          NULL,
        fecha      DATETIME     NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_SummaryBENSPedido PRIMARY KEY (po, talla)
    );
GO

IF OBJECT_ID('dbo.SummaryBENSPeso') IS NULL
    CREATE TABLE dbo.SummaryBENSPeso
    (
        po         VARCHAR(150)  NOT NULL PRIMARY KEY,
        pesoBruto  DECIMAL(9, 4) NOT NULL,
        pesoNeto   DECIMAL(9, 4) NOT NULL,
        id_usuario INT           NULL,
        fecha      DATETIME      NOT NULL DEFAULT GETDATE()
    );
GO

IF OBJECT_ID('dbo.ConsultaSugerenciaPedidoBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.ConsultaSugerenciaPedidoBENS AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.ConsultaSugerenciaPedidoBENS
    @po VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT talla, pedido FROM dbo.SummaryBENSPedido WHERE po = @po;
END
GO

IF OBJECT_ID('dbo.ConsultaSugerenciaPesoBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.ConsultaSugerenciaPesoBENS AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.ConsultaSugerenciaPesoBENS
    @po VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    -- El de la PO; si no tiene, el ultimo que se uso en cualquier PO
    SELECT TOP 1 pesoBruto, pesoNeto
    FROM dbo.SummaryBENSPeso
    ORDER BY CASE WHEN po = @po THEN 0 ELSE 1 END, fecha DESC;
END
GO

IF OBJECT_ID('dbo.GuardarSugerenciaPedidoBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.GuardarSugerenciaPedidoBENS AS BEGIN SET NOCOUNT ON; END');
GO
-- @pedido NULL borra la sugerencia de esa talla
ALTER PROCEDURE dbo.GuardarSugerenciaPedidoBENS
    @po     VARCHAR(150),
    @talla  VARCHAR(150),
    @pedido INT = NULL,
    @idUser INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @pedido IS NULL
        DELETE FROM dbo.SummaryBENSPedido WHERE po = @po AND talla = @talla;
    ELSE IF EXISTS (SELECT 1 FROM dbo.SummaryBENSPedido WHERE po = @po AND talla = @talla)
        UPDATE dbo.SummaryBENSPedido SET pedido = @pedido, id_usuario = @idUser, fecha = GETDATE() WHERE po = @po AND talla = @talla;
    ELSE
        INSERT INTO dbo.SummaryBENSPedido (po, talla, pedido, id_usuario) VALUES (@po, @talla, @pedido, @idUser);
    SELECT 1 AS Resultado;
END
GO

IF OBJECT_ID('dbo.GuardarSugerenciaPesoBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.GuardarSugerenciaPesoBENS AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.GuardarSugerenciaPesoBENS
    @po        VARCHAR(150),
    @pesoBruto DECIMAL(9, 4),
    @pesoNeto  DECIMAL(9, 4),
    @idUser    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM dbo.SummaryBENSPeso WHERE po = @po)
        UPDATE dbo.SummaryBENSPeso SET pesoBruto = @pesoBruto, pesoNeto = @pesoNeto, id_usuario = @idUser, fecha = GETDATE() WHERE po = @po;
    ELSE
        INSERT INTO dbo.SummaryBENSPeso (po, pesoBruto, pesoNeto, id_usuario) VALUES (@po, @pesoBruto, @pesoNeto, @idUser);
    SELECT 1 AS Resultado;
END
GO
