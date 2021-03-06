USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblTDSMaster]    Script Date: 1/7/2019 1:20:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTDSMaster](
	[TdsMasterId] [int] IDENTITY(1,1) NOT NULL,
	[Section] [nvarchar](max) NULL,
	[NatureOfIncome] [nvarchar](max) NULL,
	[Percentage] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tblTDSMaster_1] PRIMARY KEY CLUSTERED 
(
	[TdsMasterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


alter table tblVendorTDSDetail DROP Section

alter table tblVendorTDSDetail add Section int