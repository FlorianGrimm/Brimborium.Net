CREATE TABLE [dbo].[ToDoHistory] (
    [ActivityId]    UNIQUEIDENTIFIER   NOT NULL,
    [Id]            UNIQUEIDENTIFIER   NOT NULL,
    [ProjectId]     UNIQUEIDENTIFIER   NULL,
    [UserId]        UNIQUEIDENTIFIER   NULL,
    [Title]         NVARCHAR (50)      NOT NULL,
    [Done]          BIT                NOT NULL,
    [ValidFrom]     DATETIMEOFFSET (7) NOT NULL,
    [ValidTo]       DATETIMEOFFSET (7) NOT NULL,
    [SerialVersion] ROWVERSION         NOT NULL,
    CONSTRAINT [PK_dbo_ToDoistory] PRIMARY KEY CLUSTERED ([ValidTo] ASC, [ValidFrom] ASC, [ActivityId] ASC, [Id] ASC),
    CONSTRAINT [FK_ToDoHistory_Activity] FOREIGN KEY ([ValidFrom], [ActivityId]) REFERENCES [dbo].[Activity] ([CreatedAt], [Id])
);

