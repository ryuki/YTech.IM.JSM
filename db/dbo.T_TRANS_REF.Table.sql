USE [DB_IM_JSM]
GO
/****** Object:  Table [dbo].[T_TRANS_REF]    Script Date: 10/19/2013 02:51:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_TRANS_REF](
	[TRANS_ID] [nvarchar](50) NOT NULL,
	[TRANS_ID_REF] [nvarchar](50) NOT NULL,
	[TRANS_REF_STATUS] [nvarchar](50) NULL,
	[TRANS_REF_DESC] [nvarchar](max) NULL,
	[DATA_STATUS] [nvarchar](50) NULL,
	[CREATED_BY] [nvarchar](50) NULL,
	[CREATED_DATE] [datetime] NULL,
	[MODIFIED_BY] [nvarchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[ROW_VERSION] [timestamp] NULL,
	[TRANS_REF_ID] [nvarchar](50) NOT NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_T_TRANS_REF] PRIMARY KEY CLUSTERED 
(
	[TRANS_REF_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [MSmerge_index_1938105945] ON [dbo].[T_TRANS_REF] 
(
	[rowguid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[T_TRANS_REF]  WITH NOCHECK ADD  CONSTRAINT [FK_T_TRANS_REF_T_TRANS] FOREIGN KEY([TRANS_ID])
REFERENCES [dbo].[T_TRANS] ([TRANS_ID])
GO
ALTER TABLE [dbo].[T_TRANS_REF] CHECK CONSTRAINT [FK_T_TRANS_REF_T_TRANS]
GO
ALTER TABLE [dbo].[T_TRANS_REF]  WITH NOCHECK ADD  CONSTRAINT [FK_T_TRANS_REF_T_TRANS1] FOREIGN KEY([TRANS_ID_REF])
REFERENCES [dbo].[T_TRANS] ([TRANS_ID])
GO
ALTER TABLE [dbo].[T_TRANS_REF] CHECK CONSTRAINT [FK_T_TRANS_REF_T_TRANS1]
GO
ALTER TABLE [dbo].[T_TRANS_REF] ADD  CONSTRAINT [MSmerge_df_rowguid_F1F9A67722CD4D69915D739FCBE9C254]  DEFAULT (newsequentialid()) FOR [rowguid]
GO
