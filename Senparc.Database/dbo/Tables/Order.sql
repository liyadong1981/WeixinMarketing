CREATE TABLE [dbo].[Order] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [AccountId]       INT             NULL,
    [ActivityId]      INT             NOT NULL,
    [OrderNumber]     VARCHAR (100)   NOT NULL,
    [TotalPrice]      MONEY           CONSTRAINT [DF_Order_TotalPrice] DEFAULT ((0)) NOT NULL,
    [Price]           MONEY           CONSTRAINT [DF_Order_Price] DEFAULT ((0)) NOT NULL,
    [PayMoney]        MONEY           NOT NULL,
    [UsedPoints]      DECIMAL (18, 2) CONSTRAINT [DF_Order_UsedPoints] DEFAULT ((0)) NULL,
    [CompleteTime]    DATETIME        NOT NULL,
    [GetPayOrderTime] DATETIME        NULL,
    [AddIp]           VARCHAR (50)    NULL,
    [Status]          TINYINT         NOT NULL,
    [Description]     VARCHAR (250)   NULL,
    [Type]            TINYINT         NULL,
    [TradeNumber]     VARCHAR (150)   NULL,
    [PrepayId]        VARCHAR (100)   NULL,
    [PrepayCodeUrl]   VARCHAR (100)   NULL,
    [PayType]         INT             CONSTRAINT [DF_Order_PayType] DEFAULT ((0)) NOT NULL,
    [OrderType]       INT             CONSTRAINT [DF_Order_OrderType] DEFAULT ((0)) NOT NULL,
    [PayParam]        NTEXT           NULL,
    [AddTime]         DATETIME        NOT NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_Order] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_APP_RedPackage_Activity_Order] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[APP_RedPackage_Activity] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'微信订单编号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Order', @level2type = N'COLUMN', @level2name = N'PrepayId';

