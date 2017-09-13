CREATE TABLE [dbo].[MessageBox] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [AccountId] INT           NOT NULL,
    [Title]     NVARCHAR (50) NOT NULL,
    [Content]   NTEXT         NOT NULL,
    [PicUrl]    VARCHAR (250) NULL,
    [Url]       VARCHAR (150) NULL,
    [HasRead]   BIT           NOT NULL,
    [AddTime]   DATETIME      NOT NULL,
    CONSTRAINT [PK_MessageBox] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MessageBox_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id])
);

