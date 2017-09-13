CREATE TABLE [dbo].[Banner] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [PicUrl]    VARCHAR (250) NOT NULL,
    [ProductId] INT           NOT NULL,
    [Url]       VARCHAR (100) NOT NULL,
    [AddTime]   DATETIME      NOT NULL,
    CONSTRAINT [PK_Banner] PRIMARY KEY CLUSTERED ([Id] ASC)
);

