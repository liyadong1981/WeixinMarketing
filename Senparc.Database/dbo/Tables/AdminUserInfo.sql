CREATE TABLE [dbo].[AdminUserInfo] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [UserName]      VARCHAR (50)  NOT NULL,
    [Password]      VARCHAR (50)  NOT NULL,
    [PasswordSalt]  VARCHAR (50)  NOT NULL,
    [RealName]      NVARCHAR (50) NULL,
    [Sex]           INT           CONSTRAINT [DF_AdminUserInfo_Sex] DEFAULT ((0)) NOT NULL,
    [Phone]         VARCHAR (50)  NOT NULL,
    [ThisLoginTime] DATETIME      NOT NULL,
    [ThisLoginIP]   VARCHAR (50)  NULL,
    [LastLoginTime] DATETIME      NOT NULL,
    [LastLoginIP]   VARCHAR (50)  NULL,
    [Note]          NTEXT         NULL,
    [AddTime]       DATETIME      NOT NULL,
    [AddIP]         VARCHAR (50)  NULL,
    [UpdateTime]    DATETIME      NOT NULL,
    [UpdateIP]      VARCHAR (50)  NULL,
    CONSTRAINT [PK_AdminUserInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

