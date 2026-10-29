-- Fase14: captura del Cut Order de Ben Davis (lo manda el cliente escrito a mano en foto).
--  - CutOrderBENS: historial de cada captura guardada: PO, estilo, TOTAL UNITS escrito por el cliente,
--    lo capturado por talla, la foto, quien y desde que equipo. El ORDER QTY vigente queda en SummaryBENSPedido (Fase13).

IF OBJECT_ID('dbo.CutOrderBENS') IS NULL
    CREATE TABLE dbo.CutOrderBENS
    (
        id             INT IDENTITY(1,1) PRIMARY KEY,
        fecha          DATETIME       NOT NULL DEFAULT GETDATE(),
        po             VARCHAR(150)   NOT NULL,
        estilo         VARCHAR(150)   NULL,
        totalUnits     INT            NOT NULL,
        totalCapturado INT            NOT NULL,
        detalle        VARCHAR(MAX)   NULL,      -- "32 x 30=50;33 x 30=100;..."
        foto           VARBINARY(MAX) NULL,
        fotoNombre     VARCHAR(260)   NULL,
        id_usuario     INT            NULL,
        usuario        VARCHAR(250)   NULL,
        maquina        VARCHAR(250)   NULL,
        ip             VARCHAR(250)   NULL,
        usuarioWindows VARCHAR(250)   NULL
    );
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CutOrderBENS_po' AND object_id = OBJECT_ID('dbo.CutOrderBENS'))
    CREATE INDEX IX_CutOrderBENS_po ON dbo.CutOrderBENS (po, fecha);
GO

IF OBJECT_ID('dbo.GuardarCutOrderBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.GuardarCutOrderBENS AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.GuardarCutOrderBENS
    @po             VARCHAR(150),
    @estilo         VARCHAR(150),
    @totalUnits     INT,
    @totalCapturado INT,
    @detalle        VARCHAR(MAX),
    @foto           VARBINARY(MAX) = NULL,
    @fotoNombre     VARCHAR(260) = NULL,
    @idUser         INT = NULL,
    @maquina        VARCHAR(250) = NULL,
    @ip             VARCHAR(250) = NULL,
    @usuarioWindows VARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CutOrderBENS (po, estilo, totalUnits, totalCapturado, detalle, foto, fotoNombre, id_usuario, usuario, maquina, ip, usuarioWindows)
    SELECT @po, @estilo, @totalUnits, @totalCapturado, @detalle, @foto, @fotoNombre, @idUser,
           (SELECT TOP 1 u.usuario FROM dbo.usuarios u WHERE u.id = @idUser), @maquina, @ip, @usuarioWindows;
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS id;
END
GO

IF OBJECT_ID('dbo.ConsultaUltimoCutOrderBENS') IS NULL
    EXEC('CREATE PROCEDURE dbo.ConsultaUltimoCutOrderBENS AS BEGIN SET NOCOUNT ON; END');
GO
-- Ultima captura de la PO (para volver a abrirla con la foto y el total)
ALTER PROCEDURE dbo.ConsultaUltimoCutOrderBENS
    @po VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 id, fecha, po, ISNULL(estilo, '') AS estilo, totalUnits, totalCapturado, ISNULL(detalle, '') AS detalle,
           foto, ISNULL(fotoNombre, '') AS fotoNombre, ISNULL(usuario, '') AS usuario
    FROM dbo.CutOrderBENS
    WHERE po = @po
    ORDER BY fecha DESC, id DESC;
END
GO
