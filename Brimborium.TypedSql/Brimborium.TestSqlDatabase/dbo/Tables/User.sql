CREATE TABLE [dbo].[User] (
    [Id]            UNIQUEIDENTIFIER   NOT NULL,
    [UserName]      NVARCHAR (50)      NOT NULL,
    [ActivityId]    UNIQUEIDENTIFIER   NULL,
    [CreatedAt]     DATETIMEOFFSET (7) NOT NULL,
    [ModifiedAt]    DATETIMEOFFSET (7) NOT NULL,
    [SerialVersion] ROWVERSION         NOT NULL,
    CONSTRAINT [PK_dbo_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo_User_dbo_Activity] FOREIGN KEY ([ModifiedAt], [ActivityId]) REFERENCES [dbo].[Activity] ([CreatedAt], [Id])
);

