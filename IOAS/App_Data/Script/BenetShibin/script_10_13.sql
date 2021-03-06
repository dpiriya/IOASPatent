
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblCodeControl] ON 

GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (1098, N'CommitmentType', 1, N'Staff Commitment', N'Staff Commitment', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (1099, N'CommitmentType', 2, N'General Commitment', N'General Commitment', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (1100, N'CommitmentType', 3, N'Purchase Commitment', N'Purchase Commitment', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (1101, N'CommitmentType', 4, N'Negative Balance Commitment', N'Negative Balance Commitment', NULL, NULL, NULL)
GO
INSERT [dbo].[tblCodeControl] ([CodeID], [CodeName], [CodeValAbbr], [CodeValDetail], [CodeDescription], [UPDT_UserID], [CRTE_TS], [UPDT_TS]) VALUES (1103, N'CommitmentType', 5, N'Commitment in Foreign Currency with exchange rates', N'Commitment in Foreign Currency with exchange rates', NULL, NULL, NULL)
GO


SET IDENTITY_INSERT [dbo].[tblCodeControl] OFF
GO
USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblCommitment]    Script Date: 11/14/2018 1:00:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCommitment](
	[CommitmentId] [int] IDENTITY(1,1) NOT NULL,
	[CommitmentType] [int] NULL,
	[CommitmentNumber] [varchar](50) NULL,
	[ProjectId] [int] NULL,
	[PurchaseOrder] [varchar](50) NULL,
	[VendorName] [int] NULL,
	[Currency] [int] NULL,
	[CurrencyRate] [decimal](18, 2) NULL,
	[ProjectType] [int] NULL,
	[Purpose] [int] NULL,
	[Description] [varchar](max) NULL,
	[CommitmentAmount] [decimal](18, 2) NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[Status] [varchar](50) NULL,
	[Reference] [int] NULL,
	[ReferenceNo] [varchar](max) NULL,
	[EmailDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CommitmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tblCommitmentDetails]    Script Date: 11/14/2018 1:00:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblCommitmentDetails](
	[ComitmentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[CommitmentId] [int] NULL,
	[AllocationHeadId] [int] NULL,
	[Amount] [decimal](18, 0) NULL,
	[BalanceAmount] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ComitmentDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO