CREATE PROCEDURE [dbo].[ToDoUpsert]
    @Id uniqueidentifier,
    @ProjectId uniqueidentifier,
    @UserId uniqueidentifier,
    @Title nvarchar(50),
    @Done bit,
    @ActivityId uniqueidentifier,
    @CreatedAt datetimeoffset,
    @ModifiedAt datetimeoffset,
    @SerialVersion BIGINT
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentId uniqueidentifier;
    DECLARE @CurrentProjectId uniqueidentifier;
    DECLARE @CurrentUserId uniqueidentifier;
    DECLARE @CurrentTitle nvarchar(50);
    DECLARE @CurrentDone bit;
    DECLARE @CurrentActivityId uniqueidentifier;
    DECLARE @CurrentCreatedAt datetimeoffset;
    DECLARE @CurrentModifiedAt datetimeoffset;
    DECLARE @CurrentSerialVersion BIGINT;
    DECLARE @ResultValue INT;

    IF (@CurrentSerialVersion > 0) BEGIN
        SELECT TOP(1)
                @CurrentId = [Id],
                @CurrentProjectId = [ProjectId],
                @CurrentUserId = [UserId],
                @CurrentTitle = [Title],
                @CurrentDone = [Done],
                @CurrentActivityId = [ActivityId],
                @CurrentCreatedAt = [CreatedAt],
                @CurrentModifiedAt = [ModifiedAt],
                @CurrentSerialVersion = CAST([SerialVersion] as BIGINT)
            FROM
                [dbo].[ToDo]
            WHERE
                (@Id = [Id])
            ;
    END ELSE BEGIN
        SELECT TOP(1)
                @CurrentSerialVersion = CAST([SerialVersion] as BIGINT)
            FROM
                [dbo].[ToDo]
            WHERE
                (@Id = [Id])
            ;
    END;
    IF ((@CurrentSerialVersion IS NULL)) BEGIN
        INSERT INTO [dbo].[ToDo] (
            [Id],
            [ProjectId],
            [UserId],
            [Title],
            [Done],
            [ActivityId],
            [CreatedAt],
            [ModifiedAt]
        ) Values (
            @Id,
            @ProjectId,
            @UserId,
            @Title,
            @Done,
            @ActivityId,
            @CreatedAt,
            @ModifiedAt
        );
        SET @ResultValue = 1; /* Inserted */
        INSERT INTO [history].[ToDoHistory] (
            [Id],
            [ProjectId],
            [UserId],
            [Title],
            [Done],
            [ActivityId],
            [ValidFrom],
            [ValidTo]
        ) Values (
            @Id,
            @ProjectId,
            @UserId,
            @Title,
            @Done,
            @ActivityId,
            @ModifiedAt,
            CAST('3141-05-09T00:00:00Z' as datetimeoffset)
        );
    END ELSE BEGIN
        IF ((@SerialVersion <= 0)
            OR ((0 < @SerialVersion) AND (@SerialVersion = @CurrentSerialVersion))
        ) BEGIN
            IF (EXISTS(
                    SELECT
                        @Id,
                        @ProjectId,
                        @UserId,
                        @Title,
                        @Done
                    EXCEPT
                    SELECT
                        @CurrentId,
                        @CurrentProjectId,
                        @CurrentUserId,
                        @CurrentTitle,
                        @CurrentDone
                )) BEGIN
                UPDATE TOP(1) [dbo].[ToDo]
                    SET
                        [ProjectId] = @ProjectId,
                        [UserId] = @UserId,
                        [Title] = @Title,
                        [Done] = @Done,
                        [ActivityId] = @ActivityId,
                        [CreatedAt] = @CreatedAt,
                        [ModifiedAt] = @ModifiedAt
                    WHERE
                        ([Id] = @Id)
                ;
                SET @ResultValue = 2; /* Updated */
                UPDATE TOP(1) [history].[ToDoHistory]
                    SET
                        [ValidTo] = @ModifiedAt
                    WHERE
                        ([ValidTo] = CAST('3141-05-09T00:00:00Z' as datetimeoffset))
                        AND ([ActivityId] = @ActivityId)
                        AND ([Id] = @Id)
                ;
                INSERT INTO [history].[ToDoHistory] (
                    [Id],
                    [ProjectId],
                    [UserId],
                    [Title],
                    [Done],
                    [ActivityId],
                    [ValidFrom],
                    [ValidTo]
                ) Values (
                    @Id,
                    @ProjectId,
                    @UserId,
                    @Title,
                    @Done,
                    @ActivityId,
                    @ModifiedAt,
                    CAST('3141-05-09T00:00:00Z' as datetimeoffset)
                );
            END ELSE BEGIN
                SET @ResultValue = 0; /* NoNeedToUpdate */
            END;
        END ELSE BEGIN
            SET @ResultValue = -1 /* RowVersionMismatch */;
        END;
    END;
    SELECT TOP(1)
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
    SELECT ResultValue = @ResultValue, Message='';
END;
