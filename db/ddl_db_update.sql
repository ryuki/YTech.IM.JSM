
CREATE TABLE [dbo].[M_CUSTOMER_PRICE](
	[CUSTOMER_PRICE_ID] [nvarchar](50) NOT NULL,
	[CUSTOMER_ID] [nvarchar](50) NULL,
	[ITEM_ID] [nvarchar](50) NULL,
	[PRICE] [numeric](18, 5) NULL,
	[DATA_STATUS] [nvarchar](50) NULL,
	[CREATED_BY] [nvarchar](50) NULL,
	[CREATED_DATE] [datetime] NULL,
	[MODIFIED_BY] [varchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[ROW_VERSION] [timestamp] NULL,
 CONSTRAINT [PK_M_CUSTOMER_PRICE] PRIMARY KEY CLUSTERED 
(
	[CUSTOMER_PRICE_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[M_CUSTOMER_PRICE]  WITH CHECK ADD  CONSTRAINT [FK_M_CUSTOMER_PRICE_M_CUSTOMER] FOREIGN KEY([CUSTOMER_ID])
REFERENCES [dbo].[M_CUSTOMER] ([CUSTOMER_ID])
GO

ALTER TABLE [dbo].[M_CUSTOMER_PRICE] CHECK CONSTRAINT [FK_M_CUSTOMER_PRICE_M_CUSTOMER]
GO

ALTER TABLE [dbo].[M_CUSTOMER_PRICE]  WITH CHECK ADD  CONSTRAINT [FK_M_CUSTOMER_PRICE_M_ITEM] FOREIGN KEY([ITEM_ID])
REFERENCES [dbo].[M_ITEM] ([ITEM_ID])
GO

ALTER TABLE [dbo].[M_CUSTOMER_PRICE] CHECK CONSTRAINT [FK_M_CUSTOMER_PRICE_M_ITEM]
GO