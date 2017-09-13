CREATE TABLE [dbo].[WithdrawLog] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [AccountId]      INT             NOT NULL,
    [ActivityId]     INT             NOT NULL,
    [ActivityLogId]  INT             NOT NULL,
    [Money]          DECIMAL (18, 5) NOT NULL,
    [State]          INT             NOT NULL,
    [AddTime]        DATETIME        NOT NULL,
    [OrderNumber]    VARCHAR (100)   NULL,
    [Description]    NVARCHAR (100)  NULL,
    [LastUpdateTime] DATETIME        NOT NULL,
    CONSTRAINT [PK_WithdrawLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_WithdrawLog] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_APP_RedPackage_Activity_Award_Log_WithdrawLog] FOREIGN KEY ([ActivityLogId]) REFERENCES [dbo].[APP_RedPackage_Activity_Award_Log] ([Id]),
    CONSTRAINT [FK_APP_RedPackage_Activity_WithdrawLog] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[APP_RedPackage_Activity] ([Id])
);

