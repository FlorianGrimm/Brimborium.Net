CREATE PROCEDURE [dbo].[ActivityInsert]
    @Id uniqueidentifier,
    @Title nvarchar(20),
    @Data nvarchar(MAX),
    @CreatedAt datetimeoffset
AS BEGIN
    SET NOCOUNT ON;

    IF (@CreatedAt IS NULL) BEGIN
        SET @CreatedAt = GETUTCDATE();
    END;

    INSERT INTO [dbo].[Activity] (
        [Id],
        [Title],
        [Data],
        [CreatedAt]
    ) Values (
        @Id,
        @Title,
        @Data,
        @CreatedAt
    );

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
