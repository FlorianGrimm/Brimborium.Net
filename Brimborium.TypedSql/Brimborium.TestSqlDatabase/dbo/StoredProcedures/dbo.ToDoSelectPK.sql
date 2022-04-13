CREATE PROCEDURE [dbo].[ToDoSelectPK]
    @Id uniqueidentifier
AS BEGIN
    SET NOCOUNT ON;

    SELECT
            [Id],
            [ProjectId],
            [UserId],
            [Title],
            [Done],
            [ActivityId],
            [CreatedAt],
            [ModifiedAt],
            [SerialVersion] = CAST([SerialVersion] as BIGINT)
        FROM
            [dbo].[ToDo]
        WHERE
            (@Id = [Id])
        ;
END;
