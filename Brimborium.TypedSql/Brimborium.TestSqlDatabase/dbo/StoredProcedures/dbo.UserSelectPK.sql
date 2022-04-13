CREATE PROCEDURE [dbo].[UserSelectPK]
    @Id uniqueidentifier
AS BEGIN
    SET NOCOUNT ON;

    SELECT
            [Id],
            [UserName],
            [ActivityId],
            [CreatedAt],
            [ModifiedAt],
            [SerialVersion] = CAST([SerialVersion] as BIGINT)
        FROM
            [dbo].[User]
        WHERE
            (@Id = [Id])
        ;
END;
