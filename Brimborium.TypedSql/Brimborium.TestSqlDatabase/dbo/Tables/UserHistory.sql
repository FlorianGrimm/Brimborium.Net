﻿CREATE TABLE [dbo].[UserHistory] (
    [ActivityId]    UNIQUEIDENTIFIER   NOT NULL,
    [Id]            UNIQUEIDENTIFIER   NOT NULL,
    [UserName]      NVARCHAR (50)      NOT NULL,
    [ValidFrom]     DATETIMEOFFSET (7) NOT NULL,
    [ValidTo]       DATETIMEOFFSET (7) NOT NULL,
    [SerialVersion] ROWVERSION         NOT NULL,
    CONSTRAINT [PK_dbo_UserHistory] PRIMARY KEY CLUSTERED ([ValidTo] ASC, [ValidFrom] ASC, [ActivityId] ASC, [Id] ASC),
    CONSTRAINT [FK_UserHistory_Activity] FOREIGN KEY ([ValidFrom], [ActivityId]) REFERENCES [dbo].[Activity] ([CreatedAt], [Id])
);

