CREATE TABLE [dbo].[Address] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [AccountId]   INT            NOT NULL,
    [Name]        NVARCHAR (20)  NOT NULL,
    [Phone]       VARCHAR (20)   NOT NULL,
    [PostalCode]  VARCHAR (20)   NOT NULL,
    [UsedCount]   INT            CONSTRAINT [DF_Address_UsedCount] DEFAULT ((0)) NOT NULL,
    [Country]     VARCHAR (20)   NOT NULL,
    [Province]    VARCHAR (20)   NOT NULL,
    [City]        VARCHAR (20)   NOT NULL,
    [District]    VARCHAR (20)   NOT NULL,
    [FullAddress] NVARCHAR (250) NOT NULL,
    [IsDefault]   BIT            NOT NULL,
    [AddTime]     DATETIME       NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Address_Account] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id])
);

