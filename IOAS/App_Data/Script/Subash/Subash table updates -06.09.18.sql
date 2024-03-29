--USE [IOASDB]
GO
/****** Object:  Table [dbo].[tblJointDevelopmentCompany]    Script Date: 09/06/2018 18:18:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblJointDevelopmentCompany](
	[JointDevelopCompanyId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NULL,
	[JointDevelopCompanyName] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[CrtdTS] [datetime] NULL,
	[CrtdUserID] [int] NULL,
	[IsDeleted] [bit] NULL,
	[UpdtUserID] [int] NULL,
	[UpdtdTS] [datetime] NULL,
 CONSTRAINT [PK_tblJointDevelopmentCompany] PRIMARY KEY CLUSTERED 
(
	[JointDevelopCompanyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblJointDevelopmentCompany] ON
INSERT [dbo].[tblJointDevelopmentCompany] ([JointDevelopCompanyId], [ProjectId], [JointDevelopCompanyName], [Remarks], [CrtdTS], [CrtdUserID], [IsDeleted], [UpdtUserID], [UpdtdTS]) VALUES (1, 18, N'', N'', CAST(0x0000A95300A960C7 AS DateTime), 1, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tblJointDevelopmentCompany] OFF


/* Updates in table */
truncate table dbo.tblProject

alter table dbo.tblProject 
alter column FundingType int null;

alter table dbo.tblProject 
alter column IndianFundedBy int null;

alter table dbo.tblProject 
alter column FundingGovtBody int null;

alter table dbo.tblProject 
alter column FundingNonGovtBody int null;

alter table dbo.tblProject 
alter column ForeignFundedBy int null;

alter table dbo.tblProject  
alter column TaxStatus int null;


--alter table dbo.tblProject 
--drop column DurationOfProject;

alter table dbo.tblProject 
drop column ForeignFundedFundingCountry;

alter table dbo.tblProject 
drop column ForeignFundedFundingBody;

alter table dbo.tblProject 
drop column ForeignFundedFundingAgencyName;

alter table dbo.tblProject 
drop column ForeignFundedAmount;

alter table dbo.tblProject 
drop column DurationOfProject;

alter table dbo.tblProject 
drop column JRFStaffCount;

alter table dbo.tblProject 
drop column JRFStaffSalary;

alter table dbo.tblProject 
drop column SRFStaffCount;

alter table dbo.tblProject 
drop column SRFStaffSalary;

alter table dbo.tblProject 
drop column RAStaffCount;

alter table dbo.tblProject 
drop column RAStaffSalary;

alter table dbo.tblProject 
drop column PAStaffCount;

alter table dbo.tblProject 
drop column PAStaffSalary;

alter table dbo.tblProject 
drop column PQStaffCount;

alter table dbo.tblProject 
drop column PQStaffSalary;

alter table dbo.tblProject
add DurationOfProjectYears int null;


alter table dbo.tblProject
add DurationOfProjectMonths int null;

alter table dbo.tblProject
add TypeOfProject int null;

alter table dbo.tblProject
add InputDate datetime null;

alter table dbo.tblProject
add ForgnProjectFundingGovtBody_Qust_1 int null;

alter table dbo.tblProject
add ForgnProjectFundingNonGovtBody_Qust_1 int null;

alter table dbo.tblProject
add ForgnFundingGovtAgency nvarchar(MAX)null;

alter table dbo.tblProject
add ForgnFundingGovtAgencyCountry int null;

alter table dbo.tblProject
add ForgnForgnFundingGovtAgencyAmount decimal(18, 2)null;

alter table dbo.tblProject
add ForgnFundingNonGovtAgency nvarchar(MAX)null;

alter table dbo.tblProject
add ForgnFundingNonGovtAgencyCountry int null;

alter table dbo.tblProject
add ForgnForgnFundingNonGovtAgencyAmount decimal(18, 2) null;

alter table dbo.tblProject
add BaseValue decimal(18, 2) null;

alter table dbo.tblProject
add ApplicableTax decimal(18, 2) null;

alter table dbo.tblProject
add Collaborativeprojectcoordinator nvarchar(MAX) null;

alter table dbo.tblProject
add CollaborativeprojectAgency nvarchar(MAX) null;

alter table dbo.tblProject
add Collaborativeprojectcoordinatoremail nvarchar(MAX) null;

alter table dbo.tblProject
add Collaborativeprojecttotalcost decimal(18, 2) null;

alter table dbo.tblProject
add CollaborativeprojectIITMCost decimal(18, 2) null;

alter table dbo.tblProject
add Agencycontactpersonaddress nvarchar(MAX) null;

alter table dbo.tblProject
add ConsForgnCurrencyType nvarchar(MAX) null;

alter table dbo.tblProject
add TaxserviceGST nvarchar(200) null;

alter table dbo.tblProject
add Taxserviceregstatus int null;

alter table dbo.tblProject
add SchemeCode nvarchar(200) null;

alter table dbo.tblProject
add InternalSchemeFundingAgency int null;

alter table dbo.tblProject
add ForgnFundGovtBody int null;

alter table dbo.tblProject
add ForgnFundNonGovtBody int null;


alter table dbo.tblProject
add FundingType int null;

alter table dbo.tblProject
add IndianFundedBy int null;

alter table dbo.tblProject
add FundingGovtBody int null;
/* Proposal table updates */

alter table dbo.tblProposal  
add FundingGovtBody int null;


alter table dbo.tblProposal
add ProjectSubType int null;

alter table dbo.tblProposal
add ProjectCategory int null;

alter table dbo.tblProposal
add BasicValue decimal(18, 2) null;

alter table dbo.tblProposal
add ApplicableTax decimal(18, 2) null;

alter table dbo.tblProposal
add DurationOfProjectMonths int null;

alter table dbo.tblProposal
add Inputdate datetime null;

alter table dbo.tblProposal
add ProposalApproveddate datetime null;

alter table dbo.tblProposal
add PersonAppliedInstitute nvarchar(MAX) null;

alter table dbo.tblProposal
add PersonAppliedPlace nvarchar(MAX) null;

alter table dbo.tblProposal
add ProjectSchemeCode nvarchar(200) null;

alter table dbo.tblProposal
add Otherinstcopi_Qust_1 nvarchar(100) null;

alter table dbo.tblProposal
add SanctionNumber nvarchar(MAX) null;

alter table dbo.tblProposal 
drop column ProposalSource 

alter table dbo.tblProposal
add ProposalSource int null;


