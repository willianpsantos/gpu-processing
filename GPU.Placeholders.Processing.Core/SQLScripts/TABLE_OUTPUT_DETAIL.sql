USE [ILGPU]
GO

/****** Object:  Table [dbo].[OUTPUT_DETAIL]    Script Date: 2024-10-12 11:01:46 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TYPE [dbo].[TABLE_OUTPUT_DETAIL] AS TABLE
(
	[COMPANYCODE] [nvarchar](20) NOT NULL,
	[BILL_DESC] [nvarchar](255) NULL,
	[BILL_NO] [nvarchar](255) NOT NULL,
	[SITEID] [nvarchar](20) NOT NULL,
	[TYPE] [nvarchar](255) NOT NULL,
	[unit_AMT] [varchar](8000) NULL,
	[unit_AMT_NGN] [varchar](8000) NULL,
	[Qty] [float] NOT NULL,
	[AMT] [float] NOT NULL,
	[AMT_NGN] [float] NOT NULL,
	[AMT_USD] [float] NULL,
	[AMT_USD_PORTION] [float] NULL,
	[AMT_USD_PORTION_NGN] [float] NULL,
	[CURRENCY] [nvarchar](255) NOT NULL,
	[BILL_DATE] [date] NOT NULL,
	[T_ID] [int] NULL,
	[RECUSER] [nvarchar](255) NULL,
	[recdate] [datetime] NULL,
	[RFI_DATE] [date] NULL,
	[IFRS_RFI_DATE] [date] NULL,
	[IFRS_END] [date] NULL
)
GO


