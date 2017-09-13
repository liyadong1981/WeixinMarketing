CREATE TABLE [dbo].[SystemConfig] (
    [Id]                  INT             NOT NULL,
    [SystemName]          VARCHAR (150)   NULL,
    [SmsPlatform]         INT             CONSTRAINT [DF_SystemConfig_SmsPlatform] DEFAULT ((0)) NOT NULL,
    [SmsServiceAddress]   VARCHAR (50)    NULL,
    [SmsAccountName]      VARCHAR (50)    NULL,
    [SmsAccountPassword]  VARCHAR (50)    NULL,
    [SmsAccountCORPID]    VARCHAR (50)    NULL,
    [SmsAccountSubNumber] VARCHAR (20)    NULL,
    [SmsSign]             VARCHAR (20)    NULL,
    [MchId]               VARCHAR (20)    NULL,
    [MchKey]              VARCHAR (150)   NULL,
    [TenPayAppId]         VARCHAR (50)    NULL,
    [Rate]                DECIMAL (18, 2) CONSTRAINT [DF_SystemConfig_Rate] DEFAULT ((0)) NOT NULL,
    [TotalCoupon]         DECIMAL (18, 2) CONSTRAINT [DF_SystemConfig_Coupon] DEFAULT ((0)) NOT NULL,
    [CouponNum]           INT             CONSTRAINT [DF_SystemConfig_CouponNum] DEFAULT ((0)) NOT NULL,
    [CouponAmount]        INT             CONSTRAINT [DF_SystemConfig_CouponAmount] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SystemConfig] PRIMARY KEY CLUSTERED ([Id] ASC)
);

