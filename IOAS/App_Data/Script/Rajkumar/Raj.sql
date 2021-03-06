alter table tblUser Add ExpiryDate datetime
update tblUser set ExpiryDate='2019-05-30 00:00:00.000'

insert into tblFunctionDocument(FunctionId,DocumentId)values(40,7)
insert into tblFunctionDocument(FunctionId,DocumentId)values(40,10)
insert into tblFunction(FunctionId,FunctionName,ActionName,ControllerName,ModuleID,MenuGroupID,MenuSeq)values(40,'Vendor Management','Vendor','Master',1,2,40)

USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblVendorMaster]    Script Date: 11/23/2018 11:54:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tblVendorMaster](
	[VendorId] [int] IDENTITY(1,1) NOT NULL,
	[Nationality] [int] NULL,
	[VendorCode] [nvarchar](max) NULL,
	[PFMSVendorCode] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[ContactPerson] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[MobileNumber] [nvarchar](max) NULL,
	[RegisteredName] [nvarchar](max) NULL,
	[PAN] [nvarchar](max) NULL,
	[TAN] [nvarchar](max) NULL,
	[GSTExempted] [bit] NULL,
	[Reason] [nvarchar](max) NULL,
	[GSTIN] [nvarchar](max) NULL,
	[AccountHolderName] [nvarchar](max) NULL,
	[BankName] [nvarchar](max) NULL,
	[Branch] [nvarchar](max) NULL,
	[IFSC] [nvarchar](max) NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[BankAddress] [nvarchar](max) NULL,
	[ABANumber] [nvarchar](max) NULL,
	[SortCode] [nvarchar](max) NULL,
	[IBAN] [nvarchar](max) NULL,
	[SWIFTorBICCode] [nvarchar](max) NULL,
	[ReverseTax] [bit] NULL,
	[TDSExcempted] [bit] NULL,
	[CertificateNumber] [nvarchar](max) NULL,
	[ValidityPeriod] [int] NULL,
	[CRTD_TS] [datetime] NULL,
	[UPDT_TS] [datetime] NULL,
	[CRTD_UserID] [int] NULL,
	[UPDT_UserID] [int] NULL,
	[Status] [nvarchar](max) NULL,
	[SeqNbr] [int] NULL,
	[Country] [int] NULL,
	[StateCode] [int] NULL,
	[StateId] [int] NULL,
	[Category] [int] NULL,
	[ServiceType] [int] NULL,
	[SupplyType] [int] NULL,
	[ReasonForReservieTax] [varchar](max) NULL,
	[BankNature] [nvarchar](max) NULL,
	[BankEmailId] [nvarchar](max) NULL,
	[MICRCode] [nvarchar](max) NULL,
 CONSTRAINT [PK__tblVendo__FC8618F3661079C2] PRIMARY KEY CLUSTERED 
(
	[VendorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[tblVendorMaster] ON 

GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (1, 1, N'V0001', N'PFMS001', N'Rajkumar', N'chennai', N'raj@gmail.com', N'Rajkumar', N'45545435', N'354354543', N'Rajkumar', N'AAAPL1234C', N'DELA99999B', 0, NULL, N'33AAWFS6761Q2Z1', N'Rajkumar', N'Axis Bank', N'chennai', N'AXIOB000343', N'4657465488', N'chennai', NULL, NULL, NULL, NULL, 0, 0, NULL, 0, CAST(0x0000A998010BF02A AS DateTime), NULL, 1, NULL, N'Active', 1, 128, 33, 33, 2, NULL, 4, NULL, N'Savings Account', N'bankemail@gmail.com', NULL)
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (2, 2, N'V0002', NULL, N'JAMES VASANTH', N'No 29 first main road', N'VASANTH@GMAIL.COM', N'JAMES VASANTH', NULL, N'7436744738478', N'JAMES VASANTH', NULL, NULL, 0, NULL, NULL, N'JAMES VASANTH', N'Dubai Bank', N'behrain branch', N'ifs0001', N'32443434343', N'bahrain', N'7874834', NULL, NULL, NULL, 0, 0, NULL, 0, CAST(0x0000A9A000C2B201 AS DateTime), NULL, 1, NULL, N'Active', 2, 110, 0, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (3, 2, N'V0003', N'PFMS0005', N'Harry peter', N'N0-20 Silver palas street', N'peter@gmail.com', N'Harry peter', NULL, N'324324242', N'Harry peter', NULL, NULL, 0, NULL, NULL, N'Harry peter', N'ameica bank', N'america', N'IFSc 0002', N'44546435435345', N'US', N'ABA1234', NULL, NULL, NULL, 0, 0, NULL, 0, CAST(0x0000A9A000E643C4 AS DateTime), CAST(0x0000A9A0010609AB AS DateTime), 1, 1, N'Active', 3, 104, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (4, 1, N'V0004', N'PFMS003', N'Kumar', N'VASANTH &CO 5TH CROSS SAITH PATTAI', N'kumar@gmail.com', N'Kumar', NULL, N'34353545454', N'Kumar', N'AAAPL1234C', NULL, 0, NULL, N'33AAWFS6761Q2Z1', N'Kumar', N'Axis bank', N'vadapalani', N'IFSC1234', N'13344435554', N'No-3 2nd Cross Street Vadapalani -28', NULL, NULL, NULL, NULL, 1, 0, NULL, NULL, CAST(0x0000A9A000FA0E3B AS DateTime), NULL, 1, NULL, N'Active', 4, 128, 33, 33, 1, 2, NULL, N'Nothing', N'Current Account', N'Axis@mail.com', N'mic1223')
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (5, 1, N'V0005', N'PFMS00032', N'karthick', N'No-9 Vallasaravakkam Chennai - 26', N'karthik@gmail.com', N'karthick', NULL, N'434343423434', N'karthick', N'AAAPL1234C', NULL, 0, NULL, N'33AAWFS6761Q2Z1', N'karthick', N'hdfc bank', N'Chennai', N'AXIS0001', N'432353555', N'No-3 2nd Cross Street Vadapalani -28', NULL, NULL, NULL, NULL, 0, 0, NULL, 0, CAST(0x0000A9A00110D20B AS DateTime), CAST(0x0000A9A00111E96F AS DateTime), 1, 1, N'Active', 5, 128, 33, 33, 1, 2, NULL, NULL, N'Current Account', N'hdfc@gmail.com', NULL)
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (6, 1, N'V0006', N'PFMS123', N'VASANTH KUMAR', N'VASANTH &CO 5TH CROSS SAITH PATTAI', N'VASANTH@GMAIL.COM', N'VASANTH KUMAR', NULL, N'434343244', N'VASANTH KUMAR', N'AAAPL1234C', NULL, 0, NULL, N'33AAWFS6761Q2Z1', N'VASANTH KUMAR', N'IOB', N'Trichy', N'IO00123', N'4343545454353', N'trichy ', NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, CAST(0x0000A9A0011624CC AS DateTime), NULL, 1, NULL, N'Active', 6, 128, 33, 33, 2, NULL, 4, NULL, N'Current Account', N'iob@mail.com', N'4342')
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (7, 2, N'V0007', NULL, N'James', N'wasing ton', N'JAMES@GMAIL.COM', N'James', NULL, N'4324324535', N'James', NULL, NULL, 0, NULL, NULL, N'James', N'ameica bank', N'america', N'IFSC1234', N'343443243243', N'america branch', N'aba909', NULL, NULL, NULL, 0, 0, NULL, NULL, CAST(0x0000A9A0011FCC5C AS DateTime), NULL, 1, NULL, N'Active', 7, 104, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[tblVendorMaster] ([VendorId], [Nationality], [VendorCode], [PFMSVendorCode], [Name], [Address], [Email], [ContactPerson], [PhoneNumber], [MobileNumber], [RegisteredName], [PAN], [TAN], [GSTExempted], [Reason], [GSTIN], [AccountHolderName], [BankName], [Branch], [IFSC], [AccountNumber], [BankAddress], [ABANumber], [SortCode], [IBAN], [SWIFTorBICCode], [ReverseTax], [TDSExcempted], [CertificateNumber], [ValidityPeriod], [CRTD_TS], [UPDT_TS], [CRTD_UserID], [UPDT_UserID], [Status], [SeqNbr], [Country], [StateCode], [StateId], [Category], [ServiceType], [SupplyType], [ReasonForReservieTax], [BankNature], [BankEmailId], [MICRCode]) VALUES (8, 1, N'V0008', NULL, N'vignesh', N'chennai', N'vignesh@gmail.com', N'Vignesh kumar', NULL, N'35435435654654', N'vignesh', N'AAAPL1234C', NULL, 0, NULL, N'33AAWFS6761Q2Z1', N'vignesh', N'axisbank', N'Chennai', N'IFSC1234', N'354536436', N'Chennai', NULL, NULL, NULL, NULL, 0, 1, N'3244324', 2018, CAST(0x0000A9A001205792 AS DateTime), NULL, 1, NULL, N'Active', 8, 128, 33, 33, 1, 2, NULL, NULL, N'Savings Account', N'Axis@mail.com', NULL)
GO
SET IDENTITY_INSERT [dbo].[tblVendorMaster] OFF
GO
--------------------------------------------------------------------------------------------
--28/11/2017
------------------------------------------------------------------------------------------
USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblGstDocument]    Script Date: 11/28/2018 6:38:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblGstDocument](
	[GstVendorDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NULL,
	[GstVendorDocument] [nvarchar](max) NULL,
	[GstAttachmentPath] [nvarchar](max) NULL,
	[GstAttachmentName] [nvarchar](max) NULL,
	[GstDocumentType] [int] NULL,
	[IsCurrentVersion] [bit] NULL,
	[GstDocumentUploadUserId] [int] NULL,
	[GstDocumentUpload_Ts] [datetime] NULL,
 CONSTRAINT [PK_tblGstDocument] PRIMARY KEY CLUSTERED 
(
	[GstVendorDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblReverseTaxDocument]    Script Date: 11/28/2018 6:38:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblReverseTaxDocument](
	[RevereseTaxDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NULL,
	[TaxDocument] [nvarchar](max) NULL,
	[TaxAttachmentPath] [nvarchar](max) NULL,
	[TaxAttachmentName] [nvarchar](max) NULL,
	[TaxDocumentType] [int] NULL,
	[IsCurrentVersion] [bit] NULL,
	[TaxDocumentUploadUserId] [int] NULL,
	[TaxDocumentUpload_Ts] [datetime] NULL,
 CONSTRAINT [PK_tblReverseTaxDocument] PRIMARY KEY CLUSTERED 
(
	[RevereseTaxDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tblTDSDocument]    Script Date: 11/28/2018 6:38:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblTDSDocument](
	[VendorIdentityDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NULL,
	[IdentityVendorDocument] [nvarchar](max) NULL,
	[IdentityVendorAttachmentPath] [nvarchar](max) NULL,
	[IdentityAttachmentName] [nvarchar](max) NULL,
	[IdentityVendorDocumentType] [int] NULL,
	[IsCurrentVersion] [bit] NULL,
	[IdentityDocumentUploadUserId] [int] NULL,
	[IdentityDocumentUpload_Ts] [datetime] NULL,
 CONSTRAINT [PK_tblTDSDocument] PRIMARY KEY CLUSTERED 
(
	[VendorIdentityDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

