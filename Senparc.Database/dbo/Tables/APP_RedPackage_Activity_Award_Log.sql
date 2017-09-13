CREATE TABLE [dbo].[APP_RedPackage_Activity_Award_Log] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [AccountId]    INT             NOT NULL,
    [AwardId]      INT             NULL,
    [ActivityId]   INT             NOT NULL,
    [AwardName]    NVARCHAR (20)   CONSTRAINT [DF_ActivityAwardLog_RewardName] DEFAULT ('') NOT NULL,
    [Money]        DECIMAL (18, 2) NOT NULL,
    [State]        INT             NOT NULL,
    [AddTime]      DATETIME        NOT NULL,
    [RegisterInfo] NTEXT           NULL,
    CONSTRAINT [PK_AwardLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_APP_RedPackage_Activity_Award_Log] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_APP_RedPackage_Activity_APP_RedPackage_Activity_Award_Log] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[APP_RedPackage_Activity] ([Id]),
    CONSTRAINT [FK_APP_RedPackage_Activity_Award_Log_APP_RedPackage_Activity_Award] FOREIGN KEY ([AwardId]) REFERENCES [dbo].[APP_RedPackage_Activity_Award] ([Id])
);

