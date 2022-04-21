CREATE PROCEDURE [dbo].[ToDoDeletePK]
    @Id uniqueidentifier,
    @ActivityId uniqueidentifier,
    @ModifiedAt datetimeoffset,
    @SerialVersion bigint
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @Result AS TABLE (
        [Id] uniqueidentifier
    );

    DELETE FROM [dbo].[ToDo]
        OUTPUT
            DELETED.[Id]
        INTO @Result
        WHERE (@Id = [Id])
        ;

    IF (EXISTS(
        SELECT
            [Id]
            FROM @Result
        )
    ) BEGIN
        UPDATE TOP(1) [history].[ToDoHistory]
            SET
                [ValidTo] = @ModifiedAt
            WHERE
                    ([ActivityId] = @ActivityId)
                    AND ([ValidTo] = CAST('3141-05-09T00:00:00Z' as datetimeoffset))
                        AND (@Id = [Id])
        ;
    END;
    SELECT
        [Id]
        FROM @Result
        ;
END;
