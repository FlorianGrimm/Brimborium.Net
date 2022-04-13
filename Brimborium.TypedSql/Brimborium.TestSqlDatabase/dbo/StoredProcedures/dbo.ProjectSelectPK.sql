CREATE PROCEDURE [dbo].[ProjectSelectPK]
    @Id uniqueidentifier
AS BEGIN
    SET NOCOUNT ON;

    SELECT
            [Id],
            [Title],
            [ActivityId],
            [CreatedAt],
            [ModifiedAt],
            [SerialVersion] = CAST([SerialVersion] as BIGINT)
        FROM
            [dbo].[Project]
        WHERE
            (@Id = [Id])
        ;
END;
