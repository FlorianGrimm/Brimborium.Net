CREATE TABLE [history].[ProjectHistory] (
    [ActivityId]    UNIQUEIDENTIFIER   NOT NULL,
    [Id]            UNIQUEIDENTIFIER   NOT NULL,
    [Title]         NVARCHAR (50)      NOT NULL,
    [ValidFrom]     DATETIMEOFFSET (7) NOT NULL,
    [ValidTo]       DATETIMEOFFSET (7) NOT NULL,
    [SerialVersion] ROWVERSION         NOT NULL,
    CONSTRAINT [PK_history_ProjectHistory] PRIMARY KEY CLUSTERED ([ValidTo] ASC, [ValidFrom] ASC, [ActivityId] ASC, [Id] ASC),
    CONSTRAINT [FK_history_ProjectHistory_dbo_Activity] FOREIGN KEY ([ValidFrom], [ActivityId]) REFERENCES [dbo].[Activity] ([CreatedAt], [Id])
);

