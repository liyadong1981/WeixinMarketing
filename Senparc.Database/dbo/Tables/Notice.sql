CREATE TABLE [dbo].[Notice] (
    [Id]      INT           IDENTITY (1, 1) NOT NULL,
    [Title]   NVARCHAR (50) NOT NULL,
    [Content] NTEXT         NOT NULL,
    [PicUrl]  VARCHAR (250) NULL,
    [Url]     VARCHAR (150) NULL,
    [Type]    INT           NOT NULL,
    [HasSend] BIT           NOT NULL,
    [AddTime] DATETIME      NOT NULL,
    CONSTRAINT [PK_Notice] PRIMARY KEY CLUSTERED ([Id] ASC)
);

