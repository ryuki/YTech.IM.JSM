USE [DB_IM_JSM]
GO
/****** Object:  DdlTrigger [MSmerge_tr_altertrigger]    Script Date: 10/19/2013 02:51:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [MSmerge_tr_altertrigger] on database for ALTER_TRIGGER as 

							set ANSI_NULLS ON
							set ANSI_PADDING ON
							set ANSI_WARNINGS ON
							set ARITHABORT ON
							set CONCAT_NULL_YIELDS_NULL ON
							set NUMERIC_ROUNDABORT OFF
							set QUOTED_IDENTIFIER ON

							declare @EventData xml
							set @EventData=EventData()    
							exec sys.sp_MSmerge_ddldispatcher @EventData, 4
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
DISABLE TRIGGER [MSmerge_tr_altertrigger] ON DATABASE
GO
Enable Trigger [MSmerge_tr_altertrigger] ON Database
GO
