CREATE TABLE [dbo].[Activity] (
    [Id]            UNIQUEIDENTIFIER   NOT NULL,
    [Title]         NVARCHAR (100)     NOT NULL,
    [EntityType]    NVARCHAR (100)     NOT NULL,
    [EntityId]      NVARCHAR (100)     NOT NULL,
    [Data]          NVARCHAR (MAX)     NULL,
    [CreatedAt]     DATETIMEOFFSET (7) NOT NULL,
    [SerialVersion] ROWVERSION         NOT NULL,
    CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([CreatedAt] ASC,[Id] ASC)
);

