USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblSBIPrepaidCardProjectDetails]    Script Date: 12/01/2018 10:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSBIPrepaidCardProjectDetails](
	[SBIECardProjectDetailsId] [int] IDENTITY(1,1) NOT NULL,
	[SBIPrepaidCardDetailsId] [int] NULL,
	[ProjectId] [int] NULL,
	[PIId] [int] NULL,
	[AmountAllocated] [decimal](18, 2) NULL,
	[CrtdTS] [datetime] NULL,
	[CrtdUserId] [int] NULL,
	[UpdtTS] [datetime] NULL,
	[UpdtUserId] [int] NULL,
 CONSTRAINT [PK_tblSBIPrepaidCardProjectDetails] PRIMARY KEY CLUSTERED 
(
	[SBIECardProjectDetailsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSBIECardDetails]    Script Date: 12/01/2018 10:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSBIECardDetails](
	[SBIPrepaidCardDetailsId] [int] IDENTITY(1,1) NOT NULL,
	[SBIPrepaidCardNumber] [nvarchar](200) NULL,
	[SBIACNumber] [nvarchar](200) NULL,
	[CardTotalValue] [decimal](18, 2) NULL,
	[CardValidTill] [datetime] NULL,
	[NoofProjectsIncluded] [int] NULL,
	[PIUserId] [int] NULL,
	[PIFirstName] [nvarchar](max) NULL,
	[PIFatherFirstName] [nvarchar](max) NULL,
	[PIMotherMaidenName] [nvarchar](max) NULL,
	[Dateofbirth] [datetime] NULL,
	[Gender] [int] NULL,
	[PIMobileNumber] [nvarchar](200) NULL,
	[PIEmailId] [nvarchar](200) NULL,
	[AddressLine1] [nvarchar](200) NULL,
	[AddressLine2] [nvarchar](200) NULL,
	[District] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[State] [nvarchar](200) NULL,
	[Pincode] [nvarchar](200) NULL,
	[Status] [nvarchar](50) NULL,
	[PIPAN] [nvarchar](50) NULL,
	[CrtdTS] [datetime] NULL,
	[CrtdUserId] [int] NULL,
	[UpdtTS] [datetime] NULL,
	[UpdtUserId] [int] NULL,
 CONSTRAINT [PK_tblSBIECardDetails] PRIMARY KEY CLUSTERED 
(
	[SBIPrepaidCardDetailsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblCardDocumentDetail]    Script Date: 12/01/2018 10:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblCardDocumentDetail](
	[SBICardDocumentDetailId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [int] NULL,
	[DocumentName] [varchar](max) NULL,
	[DocumentActualName] [varchar](max) NULL,
	[Remarks] [varchar](max) NULL,
	[CRTD_TS] [datetime] NULL,
	[CRTD_By] [int] NULL,
	[Status] [varchar](max) NULL,
	[CardId] [int] NULL,
	[UPDT_By] [int] NULL,
	[UPDT_TS] [datetime] NULL,
	[Delete_By] [int] NULL,
 CONSTRAINT [PK_tblCardDocumentDetail] PRIMARY KEY CLUSTERED 
(
	[SBICardDocumentDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
