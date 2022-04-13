CREATE TABLE [dbo].[Project] (
    [Id]            UNIQUEIDENTIFIER   NOT NULL,
    [Title]         NVARCHAR (50)      NOT NULL,
    [ActivityId]    UNIQUEIDENTIFIER   NULL,
    [CreatedAt]     DATETIMEOFFSET (7) NOT NULL,
    [ModifiedAt]    DATETIMEOFFSET (7) NOT NULL,
    [SerialVersion] ROWVERSION         NOT NULL,
    CONSTRAINT [PK_dbo_Project] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Project_Activity] FOREIGN KEY ([ModifiedAt], [ActivityId]) REFERENCES [dbo].[Activity] ([CreatedAt], [Id])
);

