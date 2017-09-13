CREATE TABLE [dbo].[APP_RedPackage_Activity_Award] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [ActivityId] INT             NOT NULL,
    [AwardName]  NVARCHAR (200)  NOT NULL,
    [Money]      DECIMAL (18, 2) NOT NULL,
    [State]      INT             NOT NULL,
    [Number]     INT             CONSTRAINT [DF_Award_Number] DEFAULT ((0)) NOT NULL,
    [EndTime]    DATETIME        NOT NULL,
    [BeginTime]  DATETIME        NOT NULL,
    [PicUrl]     VARCHAR (250)   NULL,
    [AddTime]    DATETIME        NOT NULL,
    CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_APP_RedPackage_Activity_APP_RedPackage_Activity_Award] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[APP_RedPackage_Activity] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Award]
    ON [dbo].[APP_RedPackage_Activity_Award]([Id] ASC);

