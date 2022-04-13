CREATE PROCEDURE [dbo].[ActivitySelectPK]
    @CreatedAt datetimeoffset,
    @Id uniqueidentifier
AS BEGIN
    SET NOCOUNT ON;

    SELECT
            [Id],
            [Title],
            [EntityType],
            [EntityId],
            [Data],
            [CreatedAt],
            [SerialVersion] = CAST([SerialVersion] as BIGINT)
        FROM
            [dbo].[Activity]
        WHERE
            (@CreatedAt = [CreatedAt])
             AND (@Id = [Id])
        ;
END;
