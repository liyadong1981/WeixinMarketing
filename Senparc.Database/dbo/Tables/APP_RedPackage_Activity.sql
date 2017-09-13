CREATE TABLE [dbo].[APP_RedPackage_Activity] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (250) NOT NULL,
    [Type]           INT            NOT NULL,
    [State]          INT            NOT NULL,
    [Description]    NTEXT          NULL,
    [PicUrl]         VARCHAR (250)  NULL,
    [EndTime]        DATETIME       NOT NULL,
    [BeginTime]      DATETIME       NOT NULL,
    [AddTime]        DATETIME       NOT NULL,
    [TotalMoney]     MONEY          CONSTRAINT [DF_APP_RedPackage_Activity_TotalMoney1] DEFAULT ((0)) NOT NULL,
    [Rule]           NTEXT          NULL,
    [RemainingMoney] MONEY          CONSTRAINT [DF_APP_RedPackage_Activity_RemainingMoney] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([Id] ASC)
);

