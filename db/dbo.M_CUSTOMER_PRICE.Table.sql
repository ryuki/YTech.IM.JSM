USE [DB_IM_JSM]
GO
/****** Object:  Table [dbo].[M_CUSTOMER_PRICE]    Script Date: 10/19/2013 02:51:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
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
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_M_CUSTOMER_PRICE] PRIMARY KEY CLUSTERED 
(
	[CUSTOMER_PRICE_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [MSmerge_index_350624292] ON [dbo].[M_CUSTOMER_PRICE] 
(
	[rowguid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
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
ALTER TABLE [dbo].[M_CUSTOMER_PRICE] ADD  CONSTRAINT [MSmerge_df_rowguid_9A92169D428E4EF98691F54BD14545FF]  DEFAULT (newsequentialid()) FOR [rowguid]
GO
