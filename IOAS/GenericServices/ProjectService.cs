using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class ProjectService
    {
        // Creation of Proposal (Proposal Opening) - Save data in DB 
        public int ProjectOpening(CreateProjectModel model, HttpPostedFileBase[] file, HttpPostedFileBase taxprooffile)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        var proposalid = model.ProposalID;
                        tblProject create = new tblProject();
                        tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                        var PIname = Convert.ToInt32(model.PIname);
                        var proposal = context.tblProposal.FirstOrDefault(dup => dup.ProposalId == model.ProposalID);
                        //var chkproject = context.tblProject.FirstOrDefault(dup => dup.ProjectTitle == model.Projecttitle && dup.ProjectId != model.ProjectID);
                        var query = context.tblProject.FirstOrDefault(dup => dup.ProjectId == model.ProjectID);
                        //if (chkproject != null)
                        //    return 0;
                        if (query == null)
                        {
                            var userquery = context.vwFacultyStaffDetails.FirstOrDefault(m => m.UserId == model.PIname);

                            if (taxprooffile != null)
                            {
                                string taxprooffilepath = " ";
                                taxprooffilepath = System.IO.Path.GetFileName(taxprooffile.FileName);
                                var taxdocfileId = Guid.NewGuid().ToString();
                                var taxdocname = taxdocfileId + "_" + taxprooffilepath;

                                /*Saving the file in server folder*/
                                taxprooffile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + taxdocname));
                                create.TaxExemptionDocPath = taxdocname;
                            }

                            create.ProposalNumber = model.ProposalNumber;
                            create.ProjectType = model.Prjcttype;
                            create.ProjectTitle = model.Projecttitle;
                            create.PIDepartment = model.Department;
                            create.ProposalId = model.ProposalID;
                            create.PIName = model.PIname;
                            create.PCF = model.PIPCF;
                            create.RMF = model.PIRMF;
                            create.ProjectClassification = 1;
                            create.ScientistEmail = model.ScientistEmail;
                            create.ScientistAddress = model.ScientistAddress;
                            create.ScientistMobile = model.ScientistMobile;
                            create.ScientistName = model.ScientistName;
                            create.PIDesignation = model.PIDesignation;
                            create.SponsoringAgency = model.SponsoringAgency;
                            create.SanctionOrderDate = model.SanctionOrderDate;
                            create.SanctionOrderNumber = model.SanctionOrderNumber;
                            if (model.IsSubProject)
                            {
                                create.IsSubProject = true;
                                create.MainProjectId = model.MainProjectId;
                            }
                            else
                            {
                                create.IsSubProject = false;
                                create.MainProjectId = null;
                            }
                            if (create.ProjectType == 1)
                            {
                                create.ProjectSubType = model.ProjectSubType;
                                if (model.ProjectSubType == 2)
                                {
                                    create.FundingType = model.ProjectFundingType_Qust_1;
                                    create.FundingGovtAgencyDept = model.indprjctfundbodygovt_Agencydeptname;
                                    create.FundingGovtDeptAmount = model.indprjctfundbodygovt_deptAmount;
                                    create.FundingGovtMinistry = model.indprjctfundbodygovt_Agencymnstryname;
                                    create.FundingGovtMinistryAmount = model.indprjctfundbodygovt_mnstryAmount;
                                    create.FundingGovtUniv = model.indprjctfundbodygovt_Agencyunivname;
                                    create.FundingGovtUnivAmount = model.indprjctfundbodygovt_univAmount;

                                    create.FundingNonGovtAgencyIndustry = model.indprjctfundbodynongovt_AgencyIndstryname;
                                    create.FundingNonGovtIndstryAmount = model.indprjctfundbodynongovt_IndstryAmount;
                                    create.FundingNonGovtUniv = model.indprjctfundbodynongovt_Agencyunivname;
                                    create.FundingNonGovtUnivAmount = model.indprjctfundbodynongovt_univAmount;
                                    create.FundingNonGovtOthers = model.indprjctfundbodynongovt_Agencyothersname;
                                    create.FundingNonGovtOthersAmount = model.indprjctfundbodynongovt_othersAmount;

                                    create.ForgnGovtAgencyDepartmentCountry = model.forgnprjctfundbodygovt_country;
                                    create.ForgnGovtAgencyDepartment = model.forgnprjctfundbodygovt_Agencydeptname;
                                    create.ForgnGovtAgencyDepartmentAmount = model.forgnprjctfundbodygovt_deptAmount;

                                    create.ForgnGovtUnivCountry = model.forgnprjctfundbodygovt_univcountry;
                                    create.ForgnGovtUniversity = model.forgnprjctfundbodygovt_Agencyunivname;
                                    create.ForgnGovtUniversityAmount = model.forgnprjctfundbodygovt_univAmount;

                                    create.ForgnGovtOthersCountry = model.forgnprjctfundbodygovt_otherscountry;
                                    create.ForgnGovtOthers = model.forgnprjctfundbodygovt_othersagncyname;
                                    create.ForgnGovtOthersAmount = model.forgnprjctfundbodygovt_othersAmount;


                                    create.ForgnNonGovtAgencyDepartmentCountry = model.forgnprjctfundbodynongovt_country;
                                    create.ForgnNonGovtAgencyDepartment = model.forgnprjctfundbodynongovt_Agencydeptname;
                                    create.ForgnNonGovtAgencyDepartmentAmount = model.forgnprjctfundbodynongovt_deptAmount;

                                    create.ForgnNonGovtAgencyUnivCountry = model.forgnprjctfundbodynongovt_univcountry;
                                    create.ForgnNonGovtAgencyUniversity = model.forgnprjctfundbodynongovt_Agencyunivname;
                                    create.ForgnNonGovtAgencyUnivAmount = model.forgnprjctfundbodynongovt_univAmount;

                                    create.ForgnNonGovtOthersCountry = model.forgnprjctfundbodynongovt_otherscountry;
                                    create.ForgnNonGovtOthers = model.forgnprjctfundbodynongovt_othersagncyname;
                                    create.ForgnNonGovtOthersAmount = model.forgnprjctfundbodynongovt_othersAmount;
                                }
                            }

                            create.ConsultancyFundingCategory = model.ConsFundingCategory;
                            create.SponsoringAgency = model.SponsoringAgency;
                            create.FinancialYear = model.FinancialYear;
                            //create.SanctionValue = model.Sanctionvalue;
                            create.SchemeName = model.Schemename;
                            create.SchemeCode = model.SchemeCode;
                            create.SchemeAgencyName = model.SchemeAgency;
                            create.JointdevelopmentQuestion = model.JointDevelopment_Qust_1;

                            if (create.ProjectType == 2)
                            {
                                create.ConsProjectSubType = model.ConsProjectSubType;
                                create.ConsultancyFundingCategory = model.ConsFundingCategory;
                                create.FundingType = model.ConsProjectFundingType_Qust_1;
                                create.ConversionRate = model.ConsConversionRate;
                                create.SelCurr = model.ConsSelCurr;
                            }

                            create.SponProjectCategory = model.Projectcatgry_Qust_1;
                            //   create.ProjectCategory = model.SponsoringAgency;
                            //create.DurationOfProjectYears = model.Projectdurationyears;
                            //create.DurationOfProjectMonths = model.Projectdurationmonths;
                            create.SchemePersonApplied = model.SchemePersonApplied;
                            create.SchemePersonDesignation = model.SchemePersonAppliedDesignation;
                            create.AgencyRegisteredName = model.Agencyregname;
                            create.CategoryOfProject = model.Categoryofproject;

                            create.ConsultancyTaxServiceType = model.constaxservice;
                            create.ProjectAgencyCountry = model.forgnfndngagncycountry;
                            create.IndianProjectAgencyState = model.indfundngagncystate;

                            create.IndianProjectAgencyLocation = model.indfundngagncylocation;
                            create.TaxStatus = model.ConsProjectTaxType_Qust_1;
                            create.ForeignProjectAgencyState = model.forgnfundngagncystate;
                            create.ForeignProjectAgencyLocation = model.forgnfundngagncylocation;

                            create.TaxExemptionReason = model.ConsProjectReasonfornotax;

                            create.GSTIN = model.GSTNumber;
                            create.PAN = model.PAN;
                            create.TAN = model.TAN;
                            create.SponsoringAgencyCode = model.AgencyCodeid;
                            create.SponsoringAgencySOAddress = model.Agencyregaddress;
                            create.SponsoringAgencyContactPerson = model.Agencycontactperson;
                            create.SponsoringAgencyContactPersonDesignation = model.Agencycontactpersondesignation;
                            create.SponsoringAgencyContactPersonEmail = model.AgencycontactpersonEmail;
                            create.SponsoringAgencyContactPersonMobile = model.Agencycontactpersonmobile;
                            create.TotalProjectStaffCount = model.TotalNoofProjectStaffs;
                            create.SumofStaffCount = model.SumofStaffs;
                            create.SumSalaryofStaff = model.SumSalaryofStaffs;
                            create.TentativeStartDate = model.TentativeStartdate;
                            create.ActualStartDate = model.TentativeStartdate;// model.Startdate;
                            create.TentativeCloseDate = model.TentativeClosedate;
                            create.ActuaClosingDate = model.TentativeClosedate;// model.Closedate;
                            create.ProposalApprovedDate = model.ProposalApprovedDate;
                            create.Remarks = model.Remarks;
                            //create.InputDate = model.Inputdate;
                            create.InternalSchemeFundingAgency = model.InternalSchemeFundingAgency;
                            create.Collaborativeprojectcoordinator = model.Collaborativeprojectcoordinator;
                            create.CollaborativeProjectType = model.CollaborativeProjectType;
                            create.CollaborativeprojectAgency = model.CollaborativeprojectAgency;

                            create.Collaborativeprojectcoordinatoremail = model.Collaborativeprojectcoordinatoremail;
                            create.Collaborativeprojecttotalcost = model.Collaborativeprojecttotalcost;
                            create.CollaborativeprojectIITMCost = model.CollaborativeprojectIITMCost;
                            create.Agencycontactpersonaddress = model.Agencycontactpersonaddress;
                            //create.ConsForgnCurrencyType = model.ConsForgnCurrencyType;
                            create.TaxserviceGST = model.TaxserviceGST;
                            create.Taxserviceregstatus = model.Taxserviceregstatus;
                            create.InternalSchemeFundingAgency = model.InternalSchemeFundingAgency;
                            create.BaseValue = model.BaseValue;
                            create.SanctionValue = model.BaseValue;
                            create.ApplicableTax = model.ApplicableTax;
                            create.TypeOfProject = model.TypeofProject;


                            create.CrtdUserId = model.ProjectcrtdID;
                            create.CrtdTS = DateTime.Now;
                            var Departmentcode = model.Department;
                            var facultycode = Common.getfacultycode(PIname);
                            var AgencyID = model.AgencyCodeid ?? 0;
                            var fundingcategory = Convert.ToInt32(model.ConsFundingCategory);
                            var Consprjcttype = Common.getconsprjctype(fundingcategory);
                            var Agencycode = Common.getagencycode(AgencyID);
                            //  var institutecode = "IITM"; /*Common.getInstituteCode(PIusername);*/
                            var financialyear = Common.GetFinYear(model.FinancialYear ?? 0);
                            var Sequencenumber = Common.GetProjectSequenceNumber(model.FinancialYear ?? 0);
                            create.SequenceNumber = Sequencenumber;
                            if (Sequencenumber > 0)
                            {
                                if (model.Prjcttype == 1)
                                {
                                    model.ProjectNumber = Departmentcode.Trim() + financialyear + Sequencenumber.ToString("000") + Agencycode + facultycode;
                                }
                                else if (model.Prjcttype == 2)
                                {
                                    model.ProjectNumber = Consprjcttype + financialyear + Departmentcode.Trim() + Sequencenumber.ToString("000") + Agencycode + facultycode;
                                }
                            }
                            else
                            {
                                if (model.Prjcttype == 1)
                                {
                                    model.ProjectNumber = Departmentcode.Trim() + financialyear + "001" + Agencycode + facultycode;

                                }
                                else if (model.Prjcttype == 2)
                                {
                                    model.ProjectNumber = Consprjcttype + financialyear + Departmentcode.Trim() + "001" + Agencycode + facultycode;
                                }

                            }
                            create.ProjectNumber = model.ProjectNumber;
                            create.Status = "Active";
                            create.IsYearWiseAllocation = model.IsYearWiseAllocation;
                            context.tblProject.Add(create);
                            context.SaveChanges();
                            int projectid = create.ProjectId;
                            if (projectid > 0)
                            {

                                proposal.Status = "Project Open";
                                context.SaveChanges();
                                tblProjectStatusLog status = new tblProjectStatusLog();
                                status.FromStatus = "";
                                status.ToStatus = "Active";
                                status.ProjectId = projectid;
                                status.UpdtdUserId = model.ProjectcrtdID;
                                status.UpdtdTS = DateTime.Now;
                                context.tblProjectStatusLog.Add(status);
                                context.SaveChanges();


                                if (create.ProjectType == 1 && create.FundingType == 1)
                                {
                                    create.IndianFundedBy = model.ProjectFundedby_Qust_1;
                                    if (create.IndianFundedBy == 1)
                                    {
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();

                                            }
                                        }

                                    }

                                    if (create.IndianFundedBy == 2)
                                    {
                                        if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    if (create.IndianFundedBy == 3)
                                    {
                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }


                                        if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                if (create.ProjectType == 1 && create.FundingType == 2)
                                {
                                    create.ForeignFundedBy = model.ForgnProjectFundedby_Qust_1;
                                    create.SelCurr = model.SelCurr;
                                    create.ConversionRate = model.ConversionRate;
                                    if (create.ForeignFundedBy == 1)
                                    {
                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();

                                            }
                                        }
                                    }
                                    if (create.ForeignFundedBy == 2)
                                    {

                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }

                                    }

                                    if (create.ForeignFundedBy == 3)
                                    {
                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }


                                        if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                // if Sponsored - funding type is both

                                if (create.ProjectType == 1 && create.FundingType == 3)
                                {
                                    // if spon indian funded
                                    create.IndianFundedBy = model.ProjectFundedby_Qust_1;

                                    // if spon indian funded govt
                                    if (create.IndianFundedBy == 1)
                                    {
                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }

                                    }
                                    // if spon indian funded non govt
                                    if (create.IndianFundedBy == 2)
                                    {
                                        if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();

                                            }
                                        }
                                    }
                                    // if spon indian funded both govt and non govt
                                    if (create.IndianFundedBy == 3)
                                    {
                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();

                                            }
                                        }

                                        if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                                create.SponProjectCategory = model.Projectcatgry_Qust_1;

                                            }
                                        }
                                    }

                                    // if spon foreign funded
                                    create.ForeignFundedBy = model.ForgnProjectFundedby_Qust_1;

                                    create.SelCurr = model.SelCurr;
                                    create.ConversionRate = model.ConversionRate;

                                    // if spon foreign funded govt
                                    if (create.ForeignFundedBy == 1)
                                    {

                                        if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();

                                            }
                                        }
                                    }
                                    // if spon foreign funded non govt
                                    if (create.ForeignFundedBy == 2)
                                    {

                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }

                                    }
                                    // if spon foreign funded both govt and non govt
                                    if (create.ForeignFundedBy == 3)
                                    {
                                        create.SponProjectCategory = model.Projectcatgry_Qust_1;
                                        // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                        if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;

                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }
                                        }


                                        if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                        {
                                            for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                            {
                                                // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();

                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();

                                            }
                                        }

                                    }

                                }


                                if (!model.IsYearWiseAllocation)
                                {
                                    if (model.Allocationhead[0] != 0)
                                    {
                                        for (int i = 0; i < model.Allocationhead.Length; i++)
                                        {
                                            tblProjectAllocation Allocation = new tblProjectAllocation();
                                            Allocation.AllocationHead = model.Allocationhead[i];
                                            Allocation.AllocationValue = model.Allocationvalue[i];
                                            Allocation.CrtdUserId = model.ProjectcrtdID;
                                            Allocation.CrtdTS = DateTime.Now;
                                            Allocation.ProjectId = projectid;
                                            context.tblProjectAllocation.Add(Allocation);
                                            context.SaveChanges();
                                        }
                                    }
                                    if (model.ArrayEMIValue != null)
                                    {
                                        for (int i = 0; i < model.ArrayEMIValue.Length; i++)
                                        {
                                            tblInstallment EMI = new tblInstallment();
                                            EMI.NoOfInstallment = model.NoOfEMI;
                                            EMI.InstallmentNo = i + 1;
                                            EMI.InstallmentValue = model.ArrayEMIValue[i];
                                            EMI.ProjectId = projectid;
                                            context.tblInstallment.Add(EMI);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var _ai in model.YearWiseHead)
                                    {
                                        if (_ai.AllocationHeadYW[0] != 0)
                                        {
                                            for (int i = 0; i < _ai.AllocationHeadYW.Length; i++)
                                            {
                                                tblProjectAllocation Allocation = new tblProjectAllocation();
                                                Allocation.AllocationHead = _ai.AllocationHeadYW[i];
                                                Allocation.AllocationValue = _ai.AllocationValueYW[i];
                                                Allocation.Year = _ai.Year;
                                                Allocation.ProjectId = projectid;
                                                context.tblProjectAllocation.Add(Allocation);
                                                context.SaveChanges();
                                            }
                                        }
                                        if (_ai.EMIValue != null)
                                        {
                                            for (int i = 0; i < _ai.EMIValue.Length; i++)
                                            {
                                                tblInstallment EMI = new tblInstallment();
                                                EMI.NoOfInstallment = _ai.NoOfInstallment;
                                                EMI.Year = _ai.Year;
                                                EMI.InstallmentValueForYear = _ai.EMIValueForYear;
                                                EMI.InstallmentNo = i + 1;
                                                EMI.InstallmentValue = _ai.EMIValue[i];
                                                EMI.ProjectId = projectid;
                                                context.tblInstallment.Add(EMI);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                if (model.CategoryofStaffs[0] != 0)
                                {
                                    for (int i = 0; i < model.CategoryofStaffs.Length; i++)
                                    {
                                        tblProjectStaffCategorywiseBreakup ProjectStaff = new tblProjectStaffCategorywiseBreakup();
                                        ProjectStaff.ProjectStaffCategory = model.CategoryofStaffs[i];
                                        ProjectStaff.NoofStaffs = model.NoofStaffs[i];
                                        ProjectStaff.SalaryofStaffs = model.SalaryofStaffs[i];
                                        ProjectStaff.CrtdUserId = model.ProjectcrtdID;
                                        ProjectStaff.CrtdTS = DateTime.Now;
                                        ProjectStaff.ProjectId = projectid;
                                        context.tblProjectStaffCategorywiseBreakup.Add(ProjectStaff);
                                        context.SaveChanges();
                                    }
                                }
                                if (model.JointDevelopmentCompany[0] != null && model.JointDevelopmentCompany[0] != "")
                                {
                                    for (int i = 0; i < model.JointDevelopmentCompany.Length; i++)
                                    {
                                        tblJointDevelopmentCompany company = new tblJointDevelopmentCompany();
                                        company.JointDevelopCompanyName = model.JointDevelopmentCompany[i];
                                        company.Remarks = model.JointDevelopmentRemarks[i];
                                        company.CrtdUserID = model.ProjectcrtdID;
                                        company.CrtdTS = DateTime.Now;
                                        company.ProjectId = projectid;
                                        context.tblJointDevelopmentCompany.Add(company);
                                        context.SaveChanges();
                                    }
                                }



                                for (int i = 0; i < model.CoPIname.Length; i++)
                                {
                                    if (model.CoPIname[i] != 0)
                                    {
                                        tblProjectCoPI Copi = new tblProjectCoPI();
                                        Copi.ProjectId = projectid;
                                        Copi.Name = model.CoPIname[i];
                                        Copi.Department = model.CoPIDepartment[i];
                                        Copi.Designation = Common.GetPIDesignation(model.CoPIname[i]);
                                        Copi.Email = model.CoPIEmail[i];
                                        Copi.Status = "Active";
                                        Copi.CrtdUserId = model.ProjectcrtdID;
                                        Copi.PCF = model.CoPIPCF[i];
                                        Copi.RMF = model.CoPIRMF[i];
                                        Copi.Crtd_TS = DateTime.Now;
                                        context.tblProjectCoPI.Add(Copi);
                                        context.SaveChanges();

                                    }
                                }
                                for (int i = 0; i < model.OtherInstituteCoPIid.Length; i++)
                                {
                                    if (model.OtherInstituteCoPIName[i] != "")
                                    {
                                        tblProjectOtherInstituteCoPI Copi = new tblProjectOtherInstituteCoPI();
                                        Copi.ProjectId = projectid;
                                        Copi.Name = model.OtherInstituteCoPIName[i];
                                        Copi.Department = model.OtherInstituteCoPIDepartment[i];
                                        Copi.Institution = model.CoPIInstitute[i];
                                        Copi.Remarks = model.RemarksforOthrInstCoPI[i];
                                        Copi.Status = "Active";
                                        Copi.CrtdUserId = model.ProjectcrtdID;
                                        Copi.Crtd_TS = DateTime.Now;
                                        context.tblProjectOtherInstituteCoPI.Add(Copi);
                                        context.SaveChanges();

                                    }

                                }
                                for (int i = 0; i < model.DocType.Length; i++)
                                {

                                    var docid = model.Docid[i];
                                    var doctype = model.DocType[i];
                                    var path = model.DocPath[i];
                                    var docquery = (from doc in context.tblSupportDocuments
                                                    where (doc.DocType == doctype && doc.ProposalId == proposalid && doc.IsCurrentVersion == true)
                                                    select doc).ToList();

                                    if (docquery.Count == 0 && file[i] != null)
                                    {
                                        string docpath = " ";
                                        docpath = System.IO.Path.GetFileName(file[i].FileName);
                                        var docfileId = Guid.NewGuid().ToString();
                                        var docname = docfileId + "_" + docpath;

                                        /*Saving the file in server folder*/
                                        file[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));

                                        tblSupportDocuments Document = new tblSupportDocuments();
                                        Document.ProposalId = proposalid;
                                        Document.ProjectId = projectid;
                                        Document.DocName = file[i].FileName;
                                        Document.AttachmentName = model.AttachName[i];
                                        Document.DocType = model.DocType[i];
                                        Document.AttachmentPath = docname;
                                        Document.DocUploadUserid = model.ProjectcrtdID;
                                        Document.DocUpload_TS = DateTime.Now;
                                        Document.IsCurrentVersion = true;
                                        context.tblSupportDocuments.Add(Document);
                                        context.SaveChanges();

                                    }
                                    else if (docquery.Count > 0)
                                    {

                                        docquery[0].ProposalId = proposalid;
                                        docquery[0].ProjectId = projectid;
                                        docquery[0].AttachmentName = model.AttachName[i];
                                        docquery[0].AttachmentPath = path;
                                        docquery[0].DocType = model.DocType[i];
                                        docquery[0].DocUploadUserid = model.ProjectcrtdID;
                                        docquery[0].DocUpload_TS = DateTime.Now;
                                        docquery[0].IsCurrentVersion = true;
                                        context.SaveChanges();
                                    }
                                }
                            }
                            transaction.Commit();
                            return projectid;
                        }
                        else
                        {

                            //var userquery = context.tblUser.FirstOrDefault(m => m.UserId == model.PIname);

                            if (taxprooffile != null)
                            {
                                string taxprooffilepath = " ";
                                taxprooffilepath = System.IO.Path.GetFileName(taxprooffile.FileName);
                                var taxdocfileId = Guid.NewGuid().ToString();
                                var taxdocname = taxdocfileId + "_" + taxprooffilepath;

                                /*Saving the file in server folder*/
                                taxprooffile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + taxdocname));
                                query.TaxExemptionDocPath = taxdocname;
                            }
                            var projectid = query.ProjectId;
                            //query.ProposalNumber = model.ProposalNumber;
                            //query.ProjectNumber = model.ProjectNumber;
                            query.ProjectType = model.Prjcttype;
                            query.ProjectTitle = model.Projecttitle;
                            query.PIDepartment = model.Department;
                            query.PIName = model.PIname;
                            query.PCF = model.PIPCF;
                            query.RMF = model.PIRMF;
                            query.ScientistEmail = model.ScientistEmail;
                            query.ScientistAddress = model.ScientistAddress;
                            query.ScientistMobile = model.ScientistMobile;
                            query.ScientistName = model.ScientistName;
                            query.PIDesignation = model.PIDesignation;
                            query.SponsoringAgency = model.SponsoringAgency;
                            query.SanctionOrderDate = model.SanctionOrderDate;
                            query.SanctionOrderNumber = model.SanctionOrderNumber;
                            if (model.IsSubProject)
                            {
                                query.IsSubProject = true;
                                query.MainProjectId = model.MainProjectId;
                            }
                            else
                            {
                                query.IsSubProject = false;
                                query.MainProjectId = null;
                            }
                            if (model.Prjcttype == 1)
                            {
                                query.ProjectSubType = model.ProjectSubType;

                                if (model.ProjectSubType == 2)
                                {
                                    query.FundingType = model.ProjectFundingType_Qust_1;
                                    query.FundingGovtAgencyDept = model.indprjctfundbodygovt_Agencydeptname;
                                    query.FundingGovtDeptAmount = model.indprjctfundbodygovt_deptAmount;
                                    query.FundingGovtMinistry = model.indprjctfundbodygovt_Agencymnstryname;
                                    query.FundingGovtMinistryAmount = model.indprjctfundbodygovt_mnstryAmount;
                                    query.FundingGovtUniv = model.indprjctfundbodygovt_Agencyunivname;
                                    query.FundingGovtUnivAmount = model.indprjctfundbodygovt_univAmount;

                                    query.FundingNonGovtAgencyIndustry = model.indprjctfundbodynongovt_AgencyIndstryname;
                                    query.FundingNonGovtIndstryAmount = model.indprjctfundbodynongovt_IndstryAmount;
                                    query.FundingNonGovtUniv = model.indprjctfundbodynongovt_Agencyunivname;
                                    query.FundingNonGovtUnivAmount = model.indprjctfundbodynongovt_univAmount;
                                    query.FundingNonGovtOthers = model.indprjctfundbodynongovt_Agencyothersname;
                                    query.FundingNonGovtOthersAmount = model.indprjctfundbodynongovt_othersAmount;

                                    query.ForgnGovtAgencyDepartmentCountry = model.forgnprjctfundbodygovt_country;
                                    query.ForgnGovtAgencyDepartment = model.forgnprjctfundbodygovt_Agencydeptname;
                                    query.ForgnGovtAgencyDepartmentAmount = model.forgnprjctfundbodygovt_deptAmount;

                                    query.ForgnGovtUnivCountry = model.forgnprjctfundbodygovt_univcountry;
                                    query.ForgnGovtUniversity = model.forgnprjctfundbodygovt_Agencyunivname;
                                    query.ForgnGovtUniversityAmount = model.forgnprjctfundbodygovt_univAmount;

                                    query.ForgnGovtOthersCountry = model.forgnprjctfundbodygovt_otherscountry;
                                    query.ForgnGovtOthers = model.forgnprjctfundbodygovt_othersagncyname;
                                    query.ForgnGovtOthersAmount = model.forgnprjctfundbodygovt_othersAmount;

                                    query.ForgnNonGovtAgencyDepartmentCountry = model.forgnprjctfundbodynongovt_country;
                                    query.ForgnNonGovtAgencyDepartment = model.forgnprjctfundbodynongovt_Agencydeptname;
                                    query.ForgnNonGovtAgencyDepartmentAmount = model.forgnprjctfundbodynongovt_deptAmount;

                                    query.ForgnNonGovtAgencyUnivCountry = model.forgnprjctfundbodynongovt_univcountry;
                                    query.ForgnNonGovtAgencyUniversity = model.forgnprjctfundbodynongovt_Agencyunivname;
                                    query.ForgnNonGovtAgencyUnivAmount = model.forgnprjctfundbodynongovt_univAmount;

                                    query.ForgnNonGovtOthersCountry = model.forgnprjctfundbodynongovt_otherscountry;
                                    query.ForgnNonGovtOthers = model.forgnprjctfundbodynongovt_othersagncyname;
                                    query.ForgnNonGovtOthersAmount = model.forgnprjctfundbodynongovt_othersAmount;
                                }
                            }

                            query.ConsultancyFundingCategory = model.ConsFundingCategory;
                            query.SponsoringAgency = model.SponsoringAgency;
                            query.FinancialYear = model.FinancialYear;
                            //query.SanctionValue = model.Sanctionvalue;
                            query.SchemeName = model.Schemename;
                            query.SchemeAgencyName = model.SchemeAgency;
                            query.FundingType = model.ProjectFundingType_Qust_1;
                            query.JointdevelopmentQuestion = model.JointDevelopment_Qust_1;
                            query.SponProjectCategory = model.Projectcatgry_Qust_1;
                            if (query.ProjectType == 1 && query.FundingType == 1)
                            {
                                query.IndianFundedBy = model.ProjectFundedby_Qust_1;
                                if (query.IndianFundedBy == 1)
                                {
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.IndProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingGovtBody != null
                                                        && !model.IndProjectFundingGovtBody_Qust_1.Contains(funding.IndProjectFundingGovtBody)
                                                        && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }

                                if (query.IndianFundedBy == 2)
                                {
                                    if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                                query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingNonGovtBody != null &&
                                                        !model.IndProjectFundingNonGovtBody_Qust_1.Contains(funding.IndProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }

                                    }
                                }

                                if (query.IndianFundedBy == 3)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                    {

                                        for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.IndProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {

                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingGovtBody != null &&
                                                        !model.IndProjectFundingGovtBody_Qust_1.Contains(funding.IndProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                    if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                                query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingNonGovtBody != null &&
                                                        !model.IndProjectFundingNonGovtBody_Qust_1.Contains(funding.IndProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }

                            if (query.ProjectType == 1 && query.FundingType == 2)
                            {
                                query.ForeignFundedBy = model.ForgnProjectFundedby_Qust_1;
                                query.SelCurr = model.SelCurr;
                                query.ConversionRate = model.ConversionRate;
                                if (query.ForeignFundedBy == 1)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingGovtBody != null &&
                                                        !model.ForgnProjectFundingGovtBody_Qust_1.Contains(funding.ForgnProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                if (query.ForeignFundedBy == 2)
                                {

                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingNonGovtBody != null &&
                                                        !model.ForgnProjectFundingNonGovtBody_Qust_1.Contains(funding.ForgnProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }

                                if (query.ForeignFundedBy == 3)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingGovtBody != null &&
                                                        !model.ForgnProjectFundingGovtBody_Qust_1.Contains(funding.ForgnProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }


                                    if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingNonGovtBody != null &&
                                                        !model.ForgnProjectFundingNonGovtBody_Qust_1.Contains(funding.ForgnProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }

                            // if Sponsored - funding type is both

                            if (query.ProjectType == 1 && query.FundingType == 3)
                            {
                                // if spon indian funded

                                query.IndianFundedBy = model.ProjectFundedby_Qust_1;
                                // Govt 
                                if (query.IndianFundedBy == 1)
                                {
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.IndProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingGovtBody != null &&
                                                        !model.IndProjectFundingGovtBody_Qust_1.Contains(funding.IndProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                // Non Govt
                                if (query.IndianFundedBy == 2)
                                {
                                    if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                                query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingNonGovtBody != null &&
                                                        !model.IndProjectFundingNonGovtBody_Qust_1.Contains(funding.IndProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                // Both Govt and Non Govt
                                if (query.IndianFundedBy == 3)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.IndProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.IndProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = model.IndProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingGovtBody != null &&
                                                        !model.IndProjectFundingGovtBody_Qust_1.Contains(funding.IndProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                    if (model.IndProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.IndProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.IndProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            //  tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                                query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                            }
                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = model.IndProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.IndProjectFundingNonGovtBody != null &&
                                                        !model.IndProjectFundingNonGovtBody_Qust_1.Contains(funding.IndProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }


                                // if spon foreign funded
                                query.ForeignFundedBy = model.ForgnProjectFundedby_Qust_1;
                                query.SelCurr = model.SelCurr;
                                query.ConversionRate = model.ConversionRate;
                                if (query.ForeignFundedBy == 1)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingGovtBody != null &&
                                                        !model.ForgnProjectFundingGovtBody_Qust_1.Contains(funding.ForgnProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                if (query.ForeignFundedBy == 2)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingNonGovtBody != null &&
                                                        !model.ForgnProjectFundingNonGovtBody_Qust_1.Contains(funding.ForgnProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }

                                if (query.ForeignFundedBy == 3)
                                {
                                    query.SponProjectCategory = model.Projectcatgry_Qust_1;
                                    // create.FundingGovtBody = model.IndProjectFundingGovtBody_Qust_1;
                                    if (model.ForgnProjectFundingGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundinggovtbody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingGovtBody == fundinggovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                fundingquery.ForgnProjectFundingNonGovtBody = null;
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1[i];
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = null;
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingGovtBody != null &&
                                                        !model.ForgnProjectFundingGovtBody_Qust_1.Contains(funding.ForgnProjectFundingGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }


                                    if (model.ForgnProjectFundingNonGovtBody_Qust_1.Length > 0)
                                    {
                                        for (int i = 0; i < model.ForgnProjectFundingNonGovtBody_Qust_1.Length; i++)
                                        {
                                            var fundingnongovtbody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                            var fundingquery = (from P in context.tblProjectFundingBody
                                                                where (P.ProjectId == query.ProjectId && P.ForgnProjectFundingNonGovtBody == fundingnongovtbody)
                                                                select P).FirstOrDefault();
                                            // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                            if (fundingquery != null)
                                            {
                                                fundingquery.ProjectId = projectid;
                                                fundingquery.IndProjectFundingGovtBody = null;
                                                fundingquery.IndProjectFundingNonGovtBody = null;
                                                fundingquery.ForgnProjectFundingGovtBody = null;
                                                fundingquery.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                fundingquery.UpdtTS = DateTime.Now;
                                                fundingquery.UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }

                                            if (fundingquery == null)
                                            {
                                                ProjectFunding.ProjectId = projectid;
                                                ProjectFunding.IndProjectFundingGovtBody = null;
                                                ProjectFunding.IndProjectFundingNonGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingGovtBody = null;
                                                ProjectFunding.ForgnProjectFundingNonGovtBody = model.ForgnProjectFundingNonGovtBody_Qust_1[i];
                                                ProjectFunding.CrtdTS = DateTime.Now;
                                                ProjectFunding.CrtdUserId = model.ProjectcrtdID;
                                                context.tblProjectFundingBody.Add(ProjectFunding);
                                                context.SaveChanges();
                                            }

                                        }
                                        var delQuery = (from funding in context.tblProjectFundingBody
                                                        where funding.ProjectId == projectid && funding.ForgnProjectFundingNonGovtBody != null &&
                                                        !model.ForgnProjectFundingNonGovtBody_Qust_1.Contains(funding.ForgnProjectFundingNonGovtBody) && funding.IsDeleted != true
                                                        select funding).ToList();
                                        int delCount = delQuery.Count();
                                        if (delCount > 0)
                                        {
                                            for (int i = 0; i < delCount; i++)
                                            {
                                                delQuery[i].IsDeleted = true;
                                                delQuery[i].UpdtTS = DateTime.Now;
                                                delQuery[i].UpdtUserId = model.ProjectcrtdID;
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                            if (query.ProjectType == 2)
                            {
                                query.ProjectCategory = model.ConsProjectSubType;
                                query.FundingType = model.ConsProjectFundingType_Qust_1;

                                if (query.ProjectType == 2 && query.FundingType == 1)
                                {
                                    query.ConsProjectSubType = model.ConsProjectSubType;
                                    query.ConsultancyFundingCategory = model.ConsFundingCategory;
                                }
                                if (query.ProjectType == 2 && query.FundingType == 2)
                                {
                                    query.ConsultancyFundingCategory = model.ConsFundingCategory;
                                    query.ConversionRate = model.ConsConversionRate;
                                    query.SelCurr = model.ConsSelCurr;
                                    //query.ConsForgnCurrencyType = model.ConsForgnCurrencyType;
                                }
                                if (query.ProjectType == 2 && query.FundingType == 3)
                                {
                                    query.ConsProjectSubType = model.ConsProjectSubType;
                                    query.ConsultancyFundingCategory = model.ConsFundingCategory;
                                    query.ConversionRate = model.ConsConversionRate;
                                    query.SelCurr = model.ConsSelCurr;
                                }
                            }
                            query.SponProjectCategory = model.Projectcatgry_Qust_1;
                            //   create.ProjectCategory = model.SponsoringAgency;
                            //query.DurationOfProjectYears = model.Projectdurationyears;
                            //query.DurationOfProjectMonths = model.Projectdurationmonths;
                            query.SchemeAgencyName = model.SchemeAgency;
                            query.AgencyRegisteredName = model.Agencyregname;
                            query.CategoryOfProject = model.Categoryofproject;

                            query.ConsultancyTaxServiceType = model.constaxservice;
                            query.ProjectAgencyCountry = model.forgnfndngagncycountry;
                            query.IndianProjectAgencyState = model.indfundngagncystate;

                            query.IndianProjectAgencyLocation = model.indfundngagncylocation;
                            query.TaxStatus = model.ConsProjectTaxType_Qust_1;
                            query.ForeignProjectAgencyState = model.forgnfundngagncystate;
                            query.ForeignProjectAgencyLocation = model.forgnfundngagncylocation;

                            query.TaxExemptionReason = model.ConsProjectReasonfornotax;

                            query.GSTIN = model.GSTNumber;
                            query.PAN = model.PAN;
                            query.TAN = model.TAN;
                            query.SponsoringAgencyCode = model.AgencyCodeid;
                            query.SponsoringAgencySOAddress = model.Agencyregaddress;
                            query.SponsoringAgencyContactPerson = model.Agencycontactperson;
                            query.SponsoringAgencyContactPersonDesignation = model.Agencycontactpersondesignation;
                            query.SponsoringAgencyContactPersonEmail = model.AgencycontactpersonEmail;
                            query.SponsoringAgencyContactPersonMobile = model.Agencycontactpersonmobile;
                            query.TotalProjectStaffCount = model.TotalNoofProjectStaffs;
                            query.SumofStaffCount = model.SumofStaffs;
                            query.SumSalaryofStaff = model.SumSalaryofStaffs;
                            query.TentativeStartDate = model.TentativeStartdate;
                            query.ActualStartDate = model.TentativeStartdate; // model.Startdate;
                            query.TentativeCloseDate = model.TentativeClosedate;
                            query.ActuaClosingDate = model.TentativeClosedate; // model.Closedate;
                            query.ProposalApprovedDate = model.ProposalApprovedDate;
                            query.Remarks = model.Remarks;
                            query.SchemePersonApplied = model.SchemePersonApplied;
                            query.SchemePersonDesignation = model.SchemePersonAppliedDesignation;
                            query.BaseValue = model.BaseValue;
                            query.ApplicableTax = model.ApplicableTax;
                            query.TypeOfProject = model.TypeofProject;

                            query.ProposalApprovedDate = model.ProposalApprovedDate;
                            query.Remarks = model.Remarks;
                            //query.InputDate = model.Inputdate;
                            query.InternalSchemeFundingAgency = model.InternalSchemeFundingAgency;
                            query.Collaborativeprojectcoordinator = model.Collaborativeprojectcoordinator;
                            query.CollaborativeProjectType = model.CollaborativeProjectType;
                            query.CollaborativeprojectAgency = model.CollaborativeprojectAgency;

                            query.Collaborativeprojectcoordinatoremail = model.Collaborativeprojectcoordinatoremail;
                            query.Collaborativeprojecttotalcost = model.Collaborativeprojecttotalcost;
                            query.CollaborativeprojectIITMCost = model.CollaborativeprojectIITMCost;
                            query.Agencycontactpersonaddress = model.Agencycontactpersonaddress;
                            //  query.ConsForgnCurrencyType = model.ConsForgnCurrencyType;
                            query.TaxserviceGST = model.TaxserviceGST;
                            query.Taxserviceregstatus = model.Taxserviceregstatus;
                            query.InternalSchemeFundingAgency = model.InternalSchemeFundingAgency;

                            query.UpdatedUserId = model.ProjectcrtdID;
                            query.UpdatedTS = DateTime.Now;

                            query.Status = "Active";
                            query.ProjectNumber = model.ProjectNumber;
                            query.SanctionValue = model.BaseValue + Common.UpdateSanctionValue(projectid, false);
                            query.IsYearWiseAllocation = model.IsYearWiseAllocation;
                            context.tblProjectAllocation.RemoveRange(context.tblProjectAllocation.Where(m => m.ProjectId == projectid));
                            context.tblInstallment.RemoveRange(context.tblInstallment.Where(m => m.ProjectId == projectid));
                            context.SaveChanges();
                            if (!model.IsYearWiseAllocation)
                            {
                                if (model.Allocationhead[0] != 0)
                                {
                                    for (int i = 0; i < model.Allocationhead.Length; i++)
                                    {
                                        tblProjectAllocation Allocation = new tblProjectAllocation();
                                        Allocation.AllocationHead = model.Allocationhead[i];
                                        Allocation.AllocationValue = model.Allocationvalue[i];
                                        Allocation.CrtdUserId = model.ProjectcrtdID;
                                        Allocation.CrtdTS = DateTime.Now;
                                        Allocation.ProjectId = projectid;
                                        context.tblProjectAllocation.Add(Allocation);
                                        context.SaveChanges();
                                    }
                                }
                                if (model.ArrayEMIValue != null)
                                {
                                    for (int i = 0; i < model.ArrayEMIValue.Length; i++)
                                    {
                                        tblInstallment EMI = new tblInstallment();
                                        EMI.NoOfInstallment = model.NoOfEMI;
                                        EMI.InstallmentNo = i + 1;
                                        EMI.InstallmentValue = model.ArrayEMIValue[i];
                                        EMI.ProjectId = projectid;
                                        context.tblInstallment.Add(EMI);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                foreach (var _ai in model.YearWiseHead)
                                {
                                    if (_ai.AllocationHeadYW[0] != null)
                                    {
                                        for (int i = 0; i < _ai.AllocationHeadYW.Length; i++)
                                        {
                                            tblProjectAllocation Allocation = new tblProjectAllocation();
                                            Allocation.AllocationHead = _ai.AllocationHeadYW[i];
                                            Allocation.AllocationValue = _ai.AllocationValueYW[i];
                                            Allocation.Year = _ai.Year;
                                            Allocation.ProjectId = projectid;
                                            context.tblProjectAllocation.Add(Allocation);
                                            context.SaveChanges();
                                        }
                                    }
                                    if (_ai.EMIValue != null)
                                    {
                                        for (int i = 0; i < _ai.EMIValue.Length; i++)
                                        {
                                            tblInstallment EMI = new tblInstallment();
                                            EMI.NoOfInstallment = _ai.NoOfInstallment;
                                            EMI.Year = _ai.Year;
                                            EMI.InstallmentValueForYear = _ai.EMIValueForYear;
                                            EMI.InstallmentNo = i + 1;
                                            EMI.InstallmentValue = _ai.EMIValue[i];
                                            EMI.ProjectId = projectid;
                                            context.tblInstallment.Add(EMI);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            if (model.CategoryofStaffs[0] != 0)
                            {
                                for (int i = 0; i < model.CategoryofStaffs.Length; i++)
                                {
                                    var categoryofstaffs = model.CategoryofStaffs[i];
                                    var projectstaffquery = (from P in context.tblProjectStaffCategorywiseBreakup
                                                             where (P.ProjectId == projectid && P.ProjectStaffCategory == categoryofstaffs)
                                                             select P).FirstOrDefault();
                                    if (projectstaffquery != null)
                                    {

                                        projectstaffquery.ProjectStaffCategory = model.CategoryofStaffs[i];
                                        projectstaffquery.NoofStaffs = model.NoofStaffs[i];
                                        projectstaffquery.SalaryofStaffs = model.SalaryofStaffs[i];
                                        projectstaffquery.UpdtUserID = model.ProjectcrtdID;
                                        projectstaffquery.UpdtTS = DateTime.Now;
                                        projectstaffquery.ProjectId = projectid;
                                        context.SaveChanges();
                                    }

                                    else
                                    {

                                        tblProjectStaffCategorywiseBreakup Projectstaff = new tblProjectStaffCategorywiseBreakup();
                                        Projectstaff.ProjectStaffCategory = model.CategoryofStaffs[i];
                                        Projectstaff.NoofStaffs = model.NoofStaffs[i];
                                        Projectstaff.SalaryofStaffs = model.SalaryofStaffs[i];
                                        Projectstaff.CrtdUserId = model.ProjectcrtdID;
                                        Projectstaff.CrtdTS = DateTime.Now;
                                        Projectstaff.ProjectId = projectid;
                                        context.tblProjectStaffCategorywiseBreakup.Add(Projectstaff);
                                        context.SaveChanges();

                                    }
                                }
                            }
                            if (model.JointDevelopment_Qust_1 == "Yes")
                            {
                                if (model.JointDevelopmentCompany[0] != null && model.JointDevelopmentCompany[0] != "")
                                {
                                    for (int i = 0; i < model.JointDevelopmentCompany.Length; i++)
                                    {
                                        var companyid = model.JointDevelopmentCompanyId[i];
                                        var companyquery = (from P in context.tblJointDevelopmentCompany
                                                            where (P.ProjectId == projectid && P.JointDevelopCompanyId == companyid)
                                                            select P).FirstOrDefault();
                                        if (companyquery != null)
                                        {

                                            companyquery.JointDevelopCompanyName = model.JointDevelopmentCompany[i];
                                            companyquery.Remarks = model.JointDevelopmentRemarks[i];
                                            companyquery.UpdtUserID = model.ProjectcrtdID;
                                            companyquery.UpdtdTS = DateTime.Now;
                                            companyquery.ProjectId = projectid;
                                            context.SaveChanges();
                                        }

                                        else
                                        {

                                            tblJointDevelopmentCompany company = new tblJointDevelopmentCompany();
                                            company.JointDevelopCompanyName = model.JointDevelopmentCompany[i];
                                            company.Remarks = model.JointDevelopmentRemarks[i];
                                            company.CrtdUserID = model.ProjectcrtdID;
                                            company.CrtdTS = DateTime.Now;
                                            company.ProjectId = projectid;
                                            context.tblJointDevelopmentCompany.Add(company);
                                            context.SaveChanges();

                                        }
                                    }
                                    var delQuery = (from jointcompany in context.tblJointDevelopmentCompany
                                                    where jointcompany.ProjectId == projectid &&
                                                    !model.JointDevelopmentCompany.Contains(jointcompany.JointDevelopCompanyName) && jointcompany.IsDeleted != true
                                                    select jointcompany).ToList();
                                    int delCount = delQuery.Count();
                                    if (delCount > 0)
                                    {
                                        for (int i = 0; i < delCount; i++)
                                        {
                                            delQuery[i].IsDeleted = true;
                                            delQuery[i].UpdtdTS = DateTime.Now;
                                            delQuery[i].UpdtUserID = model.ProjectcrtdID;
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            if (model.JointDevelopment_Qust_1 == "No")
                            {
                                var delQuery = (from jointcompany in context.tblJointDevelopmentCompany
                                                where jointcompany.ProjectId == projectid && jointcompany.IsDeleted != true
                                                select jointcompany).ToList();
                                int delCount = delQuery.Count();
                                if (delCount > 0)
                                {
                                    for (int i = 0; i < delCount; i++)
                                    {
                                        delQuery[i].IsDeleted = true;
                                        delQuery[i].UpdtdTS = DateTime.Now;
                                        delQuery[i].UpdtUserID = model.ProjectcrtdID;
                                        context.SaveChanges();
                                    }
                                }
                            }


                            context.tblProjectOtherInstituteCoPI.Where(x => x.ProjectId == projectid && !model.OtherInstituteCoPIid.Contains(x.OtherCoPIId) && x.Status != "InActive")
                            .ToList()
                            .ForEach(m =>
                            {
                                m.Status = "InActive";
                                m.DeletedDate = DateTime.Now;
                                m.DeletedUserid = model.ProjectcrtdID;
                            });
                            for (int i = 0; i < model.OtherInstituteCoPIid.Length; i++)
                            {
                                var copi = model.OtherInstituteCoPIid[i];
                                var copiquery = (from CoPI in context.tblProjectOtherInstituteCoPI
                                                 where CoPI.OtherCoPIId == copi && CoPI.ProjectId == projectid && CoPI.Status == "Active"
                                                 select CoPI).ToList();
                                if (copiquery.Count == 0 && model.OtherInstituteCoPIName[i] != "")
                                {
                                    tblProjectOtherInstituteCoPI Copi = new tblProjectOtherInstituteCoPI();
                                    Copi.ProjectId = projectid;
                                    Copi.Name = model.OtherInstituteCoPIName[i];
                                    Copi.Department = model.OtherInstituteCoPIDepartment[i];
                                    Copi.Institution = model.CoPIInstitute[i];
                                    Copi.Remarks = model.RemarksforOthrInstCoPI[i];
                                    Copi.Status = "Active";
                                    Copi.CrtdUserId = model.ProjectcrtdID;
                                    Copi.Crtd_TS = DateTime.Now;
                                    context.tblProjectOtherInstituteCoPI.Add(Copi);
                                    context.SaveChanges();

                                }
                                else if (copiquery.Count > 0)
                                {
                                    copiquery[0].ProjectId = projectid;
                                    copiquery[0].ProjectId = projectid;
                                    copiquery[0].Name = model.OtherInstituteCoPIName[i];
                                    copiquery[0].Department = model.OtherInstituteCoPIDepartment[i];
                                    copiquery[0].Institution = model.CoPIInstitute[i];
                                    copiquery[0].Remarks = model.RemarksforOthrInstCoPI[i];
                                    copiquery[0].UpdtUserId = model.ProjectcrtdID;
                                    copiquery[0].Updt_TS = DateTime.Now;
                                    context.SaveChanges();
                                }
                            }

                            context.tblProjectCoPI.Where(x => x.ProjectId == projectid && !model.CoPIid.Contains(x.CoPIId) && x.Status != "InActive")
                            .ToList()
                            .ForEach(m =>
                            {
                                m.Status = "InActive";
                                m.DeletedDate = DateTime.Now;
                                m.DeletedUserid = model.ProjectcrtdID;
                            });
                            for (int i = 0; i < model.CoPIid.Length; i++)
                            {
                                var copi = model.CoPIid[i];
                                var copiquery = (from CoPI in context.tblProjectCoPI
                                                 where CoPI.CoPIId == copi && CoPI.ProjectId == projectid && CoPI.Status == "Active"
                                                 select CoPI).ToList();
                                if (copiquery.Count == 0 && model.CoPIname[i] != 0)
                                {
                                    tblProjectCoPI Copi = new tblProjectCoPI();
                                    Copi.ProjectId = projectid;
                                    Copi.Name = model.CoPIname[i];
                                    Copi.Department = model.CoPIDepartment[i];
                                    Copi.Email = model.CoPIEmail[i];
                                    Copi.Designation = Common.GetPIDesignation(model.CoPIname[i]);
                                    Copi.RMF = model.CoPIRMF[i];
                                    Copi.PCF = model.CoPIPCF[i];
                                    Copi.Status = "Active";
                                    Copi.CrtdUserId = model.ProjectcrtdID;
                                    Copi.Crtd_TS = DateTime.Now;
                                    context.tblProjectCoPI.Add(Copi);
                                    context.SaveChanges();

                                }
                                else if (copiquery.Count > 0)
                                {
                                    if (model.CoPIname == null || model.CoPIname[i] == 0)
                                    {
                                        copiquery[0].Status = "InActive";
                                        copiquery[0].DeletedDate = DateTime.Now;
                                        copiquery[0].DeletedUserid = model.ProjectcrtdID;
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        copiquery[0].ProjectId = projectid;
                                        copiquery[0].Name = model.CoPIname[i];
                                        copiquery[0].Department = model.CoPIDepartment[i];
                                        copiquery[0].Designation = Common.GetPIDesignation(model.CoPIname[i]);
                                        copiquery[0].RMF = model.CoPIRMF[i];
                                        copiquery[0].PCF = model.CoPIPCF[i];
                                        copiquery[0].Email = model.CoPIEmail[i];
                                        copiquery[0].UpdtUserId = model.ProjectcrtdID;
                                        copiquery[0].Updt_TS = DateTime.Now;
                                        context.SaveChanges();
                                    }
                                }
                            }

                            context.tblSupportDocuments.Where(x => x.ProjectId == projectid && !model.Docid.Contains(x.DocId) && x.IsCurrentVersion == true)
                           .ToList()
                           .ForEach(m => m.IsCurrentVersion = false);
                            context.SaveChanges();

                            for (int i = 0; i < model.DocType.Length; i++)
                            {
                                var docid = model.Docid[i];
                                var docquery = (from doc in context.tblSupportDocuments
                                                where (doc.DocId == docid && doc.ProjectId == projectid)
                                                select doc).ToList();

                                if (docquery.Count == 0 && file[i] != null)
                                {
                                    string docpath = " ";
                                    docpath = System.IO.Path.GetFileName(file[i].FileName);
                                    var docfileId = Guid.NewGuid().ToString();
                                    var docname = docfileId + "_" + docpath;

                                    /*Saving the file in server folder*/
                                    file[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));

                                    tblSupportDocuments Document = new tblSupportDocuments();
                                    Document.ProposalId = proposalid;
                                    Document.ProjectId = projectid;
                                    Document.DocName = file[i].FileName;
                                    Document.AttachmentName = model.AttachName[i];
                                    Document.DocType = model.DocType[i];
                                    Document.AttachmentPath = docname;
                                    Document.DocUploadUserid = model.ProjectcrtdID;
                                    Document.DocUpload_TS = DateTime.Now;
                                    Document.IsCurrentVersion = true;
                                    context.tblSupportDocuments.Add(Document);
                                    context.SaveChanges();

                                }
                                else if (docquery.Count > 0)
                                {
                                    if (file[i] != null)
                                    {
                                        string docpath = " ";
                                        docpath = System.IO.Path.GetFileName(file[i].FileName);
                                        var docfileId = Guid.NewGuid().ToString();
                                        var docname = docfileId + "_" + docpath;

                                        /*Saving the file in server folder*/
                                        file[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));
                                        docquery[0].AttachmentPath = docname;
                                    }
                                    docquery[0].ProposalId = proposalid;
                                    docquery[0].ProjectId = projectid;
                                    docquery[0].AttachmentName = model.AttachName[i];
                                    docquery[0].DocType = model.DocType[i];
                                    docquery[0].DocUploadUserid = model.ProjectcrtdID;
                                    docquery[0].DocUpload_TS = DateTime.Now;
                                    docquery[0].IsCurrentVersion = true;
                                    context.SaveChanges();
                                }

                            }
                            transaction.Commit();
                            return projectid;
                        }

                    }
                    catch (Exception ex)
                    {
                        Infrastructure.IOASException.Instance.HandleMe(this, ex);
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }
        public PagedData<ProposalResultModels> GetProposal(ProposalSrchFieldsModel model, int page, int pageSize)
        {
            try
            {
                List<ProposalResultModels> proposalList = new List<ProposalResultModels>();
                var searchData = new PagedData<ProposalResultModels>();

                int skiprec = 0;

                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }

                using (var context = new IOASDBEntities())
                {
                    int[] status = { 1, 2, 3 };
                    var result = (from psl in context.tblProposal
                                  join user in context.vwFacultyStaffDetails on psl.PI equals user.UserId
                                  where (psl.IsDeleted != true)
                                  && (String.IsNullOrEmpty(model.srchKeyword) || psl.ProposalNumber == model.srchKeyword
                                  || psl.ProposalTitle.Contains(model.srchKeyword))
                                  orderby psl.ProposalId descending
                                  select new { psl, user.FirstName }).Skip(skiprec).Take(pageSize).ToList();
                    //var result = query.Skip(skiprec).Take(pageSize).ToList();
                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            int proposalId = result[i].psl.ProposalId;
                            proposalList.Add(new ProposalResultModels()
                            {
                                proposalId = proposalId,
                                nameOfPI = result[i].FirstName,
                                PIUserID = result[i].psl.PI,
                                proposalTitle = result[i].psl.ProposalTitle,
                                proposedBudget = Convert.ToDecimal(result[i].psl.ProposalValue),
                                status = result[i].psl.Status

                            });

                        }
                        var records = (from psl in context.tblProposal
                                       join user in context.vwFacultyStaffDetails on psl.PI equals user.UserId
                                       where (psl.IsDeleted != true)
                                       && (String.IsNullOrEmpty(model.srchKeyword) || psl.ProposalNumber == model.srchKeyword
                                       || psl.ProposalTitle.Contains(model.srchKeyword))
                                       orderby psl.ProposalId descending
                                       select new { psl, user.FirstName }).Count();
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));

                    }
                }
                searchData.Data = proposalList;
                searchData.visiblePages = 10;
                searchData.CurrentPage = page;
                searchData.pageSize = pageSize;
                return searchData;

            }
            catch (Exception ex)
            {
                List<ProposalResultModels> proposalList = new List<ProposalResultModels>();
                var searchData = new PagedData<ProposalResultModels>();
                searchData.Data = proposalList;
                return searchData;
            }
        }
        public static CreateProjectModel EditProject(int ProjectId)
        {
            try
            {
                CreateProjectModel editProject = new CreateProjectModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 where (P.ProjectId == ProjectId)
                                 select P).FirstOrDefault();
                    var CoPIquery = (from CoPI in context.tblProjectCoPI
                                     where CoPI.ProjectId == ProjectId && CoPI.Status == "Active"
                                     select CoPI).ToList();
                    var OtherInstituteCoPIquery = (from CoPI in context.tblProjectOtherInstituteCoPI
                                                   where CoPI.ProjectId == ProjectId && CoPI.Status == "Active"
                                                   select CoPI).ToList();
                    var prjctsubtypequery = (from C in context.tblCodeControl
                                             where (C.CodeName == "ConsultancyProjectSubtype" && C.CodeValAbbr == query.ConsProjectSubType)
                                             select C).FirstOrDefault();
                    var schemenamequery = (from c in context.tblSchemes
                                           where c.SchemeId == query.ConsultancyFundingCategory
                                           select c).FirstOrDefault();
                    var SupportDocquery = (from Doc in context.tblSupportDocuments
                                           where Doc.ProjectId == ProjectId && Doc.IsCurrentVersion == true
                                           select Doc).ToList();
                    var allocationquery = (from alloc in context.tblProjectAllocation
                                           where (alloc.ProjectId == ProjectId)
                                           select alloc).ToList();
                    var emiQuery = (from emi in context.tblInstallment
                                    where (emi.ProjectId == ProjectId)
                                    select emi).ToList();
                    var prjctstaffquery = (from staff in context.tblProjectStaffCategorywiseBreakup
                                           where (staff.ProjectId == ProjectId)
                                           select staff).ToList();
                    var ProjectTypeName = (from C in context.tblCodeControl
                                           where C.CodeName == "Projecttype" && C.CodeValAbbr == query.ProjectType
                                           select C).FirstOrDefault();
                    var Agencyquery = (from C in context.tblAgencyMaster
                                       where C.AgencyId == query.SponsoringAgency
                                       select C).FirstOrDefault();
                    var Companyquery = (from C in context.tblJointDevelopmentCompany
                                        where C.ProjectId == ProjectId
                                        select C).ToList();
                    bool isYearWiseAH = false;

                    int allocatedYear = 0;
                    if (query != null)
                    {
                        if (allocationquery.Count > 0)
                        {
                            allocatedYear = allocationquery.OrderByDescending(m => m.Year).FirstOrDefault().Year ?? 0;
                        }
                        editProject.MainProjectList = Common.GetMainProjectNumberList(query.ProjectType ?? 0);
                        //int months = (query.DurationOfProjectMonths ?? 0) > 0 ? 1 : 0;
                        //allocatedYear = (query.DurationOfProjectYears ?? 0) + months;
                        editProject.ProjectID = ProjectId;
                        editProject.ProposalID = query.ProposalId;
                        editProject.ProposalNumber = query.ProposalNumber;
                        editProject.ProjectNumber = query.ProjectNumber;
                        isYearWiseAH = query.IsYearWiseAllocation ?? false;
                        //  editProject.ProjectType = Convert.ToInt32(query.ProjectType);
                        if (query.ProposalApprovedDate != null)
                        {
                            editProject.PrpsalApprovedDate = String.Format("{0:dd}", (DateTime)query.ProposalApprovedDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ProposalApprovedDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ProposalApprovedDate);
                            editProject.ProposalApprovedDate = (DateTime)query.ProposalApprovedDate;
                        }
                        editProject.FinancialYear = query.FinancialYear;
                        editProject.Projecttitle = query.ProjectTitle;
                        editProject.ProjectType = query.ProjectType;
                        editProject.ProjectTypeName = ProjectTypeName.CodeValDetail;
                        //if (query.InputDate != null)
                        //{
                        //    editProject.Inptdate = String.Format("{0:dd}", (DateTime)query.InputDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.InputDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.InputDate);
                        //    editProject.Inputdate = (DateTime)query.InputDate;
                        //}

                        if (editProject.ProjectType == 1)
                        {
                            editProject.ProjectSubType = query.ProjectSubType;
                            editProject.ProjectFundingType_Qust_1 = query.FundingType;
                            if (editProject.ProjectSubType == 1)
                            {
                                editProject.InternalSchemeFundingAgency = Convert.ToInt32(query.InternalSchemeFundingAgency);
                            }
                            if (editProject.ProjectSubType == 2)
                            {
                                editProject.indprjctfundbodygovt_Agencydeptname = query.FundingGovtAgencyDept;
                                editProject.indprjctfundbodygovt_deptAmount = query.FundingGovtDeptAmount;

                                editProject.indprjctfundbodygovt_Agencymnstryname = query.FundingGovtMinistry;
                                editProject.indprjctfundbodygovt_mnstryAmount = query.FundingGovtMinistryAmount;
                                editProject.indprjctfundbodygovt_Agencyunivname = query.FundingGovtUniv;
                                editProject.indprjctfundbodygovt_univAmount = query.FundingGovtUnivAmount;

                                editProject.indprjctfundbodynongovt_AgencyIndstryname = query.FundingNonGovtAgencyIndustry;
                                editProject.indprjctfundbodynongovt_IndstryAmount = query.FundingNonGovtIndstryAmount;

                                editProject.indprjctfundbodynongovt_Agencyunivname = query.FundingNonGovtUniv;
                                editProject.indprjctfundbodynongovt_univAmount = query.FundingNonGovtUnivAmount;
                                editProject.indprjctfundbodynongovt_Agencyothersname = query.FundingNonGovtOthers;
                                editProject.indprjctfundbodynongovt_othersAmount = query.FundingNonGovtOthersAmount;


                                editProject.forgnprjctfundbodygovt_country = query.ForgnGovtAgencyDepartmentCountry;
                                editProject.forgnprjctfundbodygovt_Agencydeptname = query.ForgnGovtAgencyDepartment;
                                editProject.forgnprjctfundbodygovt_deptAmount = query.ForgnGovtAgencyDepartmentAmount;

                                editProject.forgnprjctfundbodygovt_univcountry = query.ForgnGovtUnivCountry;
                                editProject.forgnprjctfundbodygovt_Agencyunivname = query.ForgnGovtUniversity;
                                editProject.forgnprjctfundbodygovt_univAmount = query.ForgnGovtUniversityAmount;

                                editProject.forgnprjctfundbodygovt_otherscountry = query.ForgnGovtOthersCountry;
                                editProject.forgnprjctfundbodygovt_othersagncyname = query.ForgnGovtOthers;
                                editProject.forgnprjctfundbodygovt_othersAmount = query.ForgnGovtOthersAmount;

                                editProject.forgnprjctfundbodynongovt_country = query.ForgnNonGovtAgencyDepartmentCountry;
                                editProject.forgnprjctfundbodynongovt_Agencydeptname = query.ForgnNonGovtAgencyDepartment;
                                editProject.forgnprjctfundbodynongovt_deptAmount = query.ForgnNonGovtAgencyDepartmentAmount;

                                editProject.forgnprjctfundbodynongovt_univcountry = query.ForgnNonGovtAgencyUnivCountry;
                                editProject.forgnprjctfundbodynongovt_Agencyunivname = query.ForgnNonGovtAgencyUniversity;
                                editProject.forgnprjctfundbodynongovt_univAmount = query.ForgnNonGovtAgencyUnivAmount;

                                editProject.forgnprjctfundbodynongovt_otherscountry = query.ForgnNonGovtOthersCountry;
                                editProject.forgnprjctfundbodynongovt_othersagncyname = query.ForgnNonGovtOthers;
                                editProject.forgnprjctfundbodynongovt_othersAmount = query.ForgnNonGovtOthersAmount;
                            }

                        }
                        editProject.Schemename = query.SchemeName;
                        editProject.SchemeCode = query.SchemeCode;
                        editProject.SchemeAgency = query.SchemeAgencyName;
                        editProject.Department = query.PIDepartment;
                        editProject.PIname = Convert.ToInt32(query.PIName);
                        editProject.PIDesignation = query.PIDesignation;
                        editProject.PIEmail = Common.GetPIEmail(query.PIName ?? 0);
                        editProject.PIPCF = query.PCF;
                        editProject.PIRMF = query.RMF;
                        editProject.ScientistEmail = query.ScientistEmail;
                        editProject.ScientistAddress = query.ScientistAddress;
                        editProject.ScientistMobile = query.ScientistMobile;
                        editProject.ScientistName = query.ScientistName;
                        editProject.IsYearWiseAllocation = isYearWiseAH;
                        editProject.SponsoringAgency = query.SponsoringAgency;
                        editProject.AgencyCodeid = Convert.ToInt32(query.SponsoringAgencyCode);
                        editProject.AgencyCode = Agencyquery.AgencyCode;
                        editProject.Agencyregname = query.AgencyRegisteredName;
                        editProject.Agencyregaddress = query.SponsoringAgencySOAddress;
                        editProject.Agencycontactpersonmobile = query.SponsoringAgencyContactPersonMobile;
                        editProject.Agencycontactpersondesignation = query.SponsoringAgencyContactPersonDesignation;
                        editProject.AgencycontactpersonEmail = query.SponsoringAgencyContactPersonEmail;
                        editProject.Agencycontactperson = query.SponsoringAgencyContactPerson;
                        editProject.SanctionOrderNumber = query.SanctionOrderNumber;
                        editProject.MainProjectId = query.MainProjectId;
                        editProject.IsSubProject = query.IsSubProject ?? false;
                        editProject.JointDevelopment_Qust_1 = query.JointdevelopmentQuestion;
                        if (query.SanctionOrderDate != null)
                        {
                            editProject.SODate = String.Format("{0:dd}", (DateTime)query.SanctionOrderDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.SanctionOrderDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.SanctionOrderDate);
                            editProject.SanctionOrderDate = (DateTime)query.SanctionOrderDate;
                        }
                        if (query.TentativeStartDate != null)
                        {
                            editProject.TentativestrtDate = String.Format("{0:dd}", (DateTime)query.TentativeStartDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.TentativeStartDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.TentativeStartDate);
                            editProject.TentativeStartdate = (DateTime)query.TentativeStartDate;
                        }

                        //if (query.ActualStartDate != null)
                        //{
                        //    editProject.strtDate = String.Format("{0:dd}", (DateTime)query.ActualStartDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ActualStartDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ActualStartDate);
                        //    editProject.Startdate = (DateTime)query.ActualStartDate;
                        //}
                        if (query.TentativeCloseDate != null)
                        {
                            editProject.TentativeclsDate = String.Format("{0:dd}", (DateTime)query.TentativeCloseDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.TentativeCloseDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.TentativeCloseDate);
                            editProject.TentativeClosedate = (DateTime)query.TentativeCloseDate;
                        }
                        //if (query.ActuaClosingDate != null)
                        //{
                        //    editProject.clsDate = String.Format("{0:dd}", (DateTime)query.ActuaClosingDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ActuaClosingDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ActuaClosingDate);
                        //    editProject.Closedate = (DateTime)query.ActuaClosingDate;
                        //}
                        editProject.Budget = query.BaseValue;
                        //editProject.Projectdurationyears = query.DurationOfProjectYears;
                        //editProject.Projectdurationmonths = query.DurationOfProjectMonths;
                        editProject.SchemePersonAppliedDesignation = query.SchemePersonDesignation;
                        editProject.Personapplied = query.SchemePersonApplied;
                        editProject.TotalNoofProjectStaffs = query.TotalProjectStaffCount;
                        //editProject.NoofJRFStaffs = query.JRFStaffCount;
                        //editProject.SalaryofJRFStaffs = query.JRFStaffSalary;
                        //editProject.NoofSRFStaffs = query.SRFStaffCount;
                        //editProject.SalaryofSRFStaffs = query.SRFStaffSalary;
                        //editProject.NoofRAStaffs = query.RAStaffCount;
                        //editProject.SalaryofRAStaffs = query.RAStaffSalary;
                        //editProject.NoofPAStaffs = query.PAStaffCount;
                        //editProject.SalaryofPAStaffs = query.PAStaffSalary;
                        //editProject.NoofPQStaffs = query.PQStaffCount;
                        //editProject.SalaryofPQStaffs = query.PQStaffSalary;
                        editProject.SumofStaffs = query.SumofStaffCount;
                        editProject.SumSalaryofStaffs = query.SumSalaryofStaff;
                        editProject.Allocationtotal = Convert.ToDecimal(query.BaseValue);
                        editProject.Projectcatgry_Qust_1 = query.SponProjectCategory;
                        if (editProject.ProjectType == 2)
                        {
                            editProject.ConsProjectSubType = query.ConsProjectSubType;
                            editProject.ConsFundingCategory = query.ConsultancyFundingCategory;
                            editProject.constaxservice = query.ConsultancyTaxServiceType;
                            editProject.ConsProjectTaxType_Qust_1 = query.TaxStatus;
                            editProject.Conssubtypename = prjctsubtypequery.CodeValDetail;
                            editProject.ConsFundingcategoryname = schemenamequery.SchemeName;
                            editProject.ConsProjectFundingType_Qust_1 = query.FundingType;
                            if (query.ConsultancyTaxServiceType == 1 || query.ConsultancyTaxServiceType == 2)
                            {
                                editProject.indfundngagncystate = query.IndianProjectAgencyState;
                                editProject.indfundngagncylocation = query.IndianProjectAgencyLocation;
                            }
                            if (query.ConsultancyTaxServiceType == 3)
                            {
                                editProject.forgnfndngagncycountry = query.ProjectAgencyCountry;
                                editProject.forgnfundngagncystate = query.ForeignProjectAgencyState;
                                editProject.forgnfundngagncylocation = query.ForeignProjectAgencyLocation;
                            }
                            if (editProject.ConsProjectTaxType_Qust_1 == 2 || editProject.ConsProjectTaxType_Qust_1 == 3 || editProject.ConsProjectTaxType_Qust_1 == 4)
                            {
                                editProject.ConsProjectReasonfornotax = query.TaxExemptionReason;
                                editProject.Docpathfornotax = query.TaxExemptionDocPath;
                                string taxprooffilepath = " ";
                                taxprooffilepath = System.IO.Path.GetFileName(editProject.Docpathfornotax);
                                string taxdocfilename = taxprooffilepath.Substring(taxprooffilepath.LastIndexOf("_") + 1);
                                editProject.taxprooffilename = taxdocfilename;
                                editProject.GSTNumber = query.GSTIN;
                                editProject.TAN = query.TAN;
                                editProject.PAN = query.PAN;
                            }
                            if (editProject.ConsProjectTaxType_Qust_1 == 1)
                            {
                                editProject.GSTNumber = query.GSTIN;
                                editProject.TAN = query.TAN;
                                editProject.PAN = query.PAN;
                            }

                        }


                        if (editProject.ProjectType == 1 && editProject.ProjectFundingType_Qust_1 == 1)
                        {

                            editProject.ProjectFundedby_Qust_1 = query.IndianFundedBy;
                            if (editProject.ProjectFundedby_Qust_1 == 1)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.IndProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _indgovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _indgovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        _indgovtbody[i] = Convert.ToInt32(fundingbodyquery[i].IndProjectFundingGovtBody);
                                        _indgovtbodyid[i] = Convert.ToInt32((fundingbodyquery[i].FundingBodyId));
                                    }
                                    editProject.IndProjectFundingGovtBody_Qust_1 = _indgovtbody;
                                    editProject.IndProjectFundingGovtBodyId = _indgovtbodyid;
                                }

                            }
                            else if (editProject.ProjectFundedby_Qust_1 == 2)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.IndProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _indnongovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _indnongovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _indnongovtbody[i] = Convert.ToInt32(fundingbodyquery[i].IndProjectFundingNonGovtBody);
                                        _indnongovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                    editProject.IndProjectFundingNonGovtBody_Qust_1 = _indnongovtbody;
                                    editProject.IndProjectFundingNonGovtBodyId = _indnongovtbodyid;
                                }
                            }
                            else if (editProject.ProjectFundedby_Qust_1 == 3)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.IndProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _indgovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _indgovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        _indgovtbody[i] = Convert.ToInt32(fundingbodyquery[i].IndProjectFundingGovtBody);
                                        _indgovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                    editProject.IndProjectFundingGovtBody_Qust_1 = _indgovtbody;
                                }

                                var fundingnongovtbodyquery = (from f in context.tblProjectFundingBody
                                                               where f.ProjectId == ProjectId && f.IndProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                               select f).ToList();
                                Nullable<int>[] _indnongovtbody = new Nullable<int>[fundingnongovtbodyquery.Count];
                                int[] _indnongovtbodyid = new int[fundingnongovtbodyquery.Count];
                                if (fundingnongovtbodyquery != null)
                                {
                                    for (int i = 0; i < fundingnongovtbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _indnongovtbody[i] = Convert.ToInt32(fundingnongovtbodyquery[i].IndProjectFundingNonGovtBody);
                                        _indnongovtbodyid[i] = Convert.ToInt32(fundingnongovtbodyquery[i].FundingBodyId);
                                    }
                                    editProject.IndProjectFundingNonGovtBody_Qust_1 = _indnongovtbody;
                                    editProject.IndProjectFundingNonGovtBodyId = _indnongovtbodyid;
                                }
                            }

                        }


                        if (editProject.ProjectType == 1 && editProject.ProjectFundingType_Qust_1 == 2)
                        {

                            editProject.ForgnProjectFundedby_Qust_1 = query.ForeignFundedBy;
                            editProject.SelCurr = query.SelCurr;
                            editProject.ConversionRate = query.ConversionRate;
                            if (editProject.ForgnProjectFundedby_Qust_1 == 1)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.ForgnProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                //  editProject.ForgnProjectFundingGovtBody_Qust_1 = query.ForgnFundGovtBody;                              

                                Nullable<int>[] _forgngovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _forgngovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgngovtbody[i] = Convert.ToInt32(fundingbodyquery[i].ForgnProjectFundingGovtBody);
                                        _forgngovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);

                                    }
                                    editProject.ForgnProjectFundingGovtBody_Qust_1 = _forgngovtbody;
                                    editProject.ForgnProjectFundingGovtBodyId = _forgngovtbodyid;
                                }

                            }
                            else if (editProject.ForgnProjectFundedby_Qust_1 == 2)
                            {
                                editProject.Projectcatgry_Qust_1 = query.SponProjectCategory;
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.ForgnProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                //   editProject.IndProjectFundingNonGovtBody_Qust_1 = query.FundingNonGovtBody;
                                Nullable<int>[] _forgnnongovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _forgnnongovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgnnongovtbody[i] = Convert.ToInt32(fundingbodyquery[i].ForgnProjectFundingNonGovtBody);
                                        _forgnnongovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);

                                    }
                                    editProject.ForgnProjectFundingNonGovtBody_Qust_1 = _forgnnongovtbody;
                                    editProject.ForgnProjectFundingNonGovtBodyId = _forgnnongovtbodyid;
                                }
                            }
                            else if (editProject.ForgnProjectFundedby_Qust_1 == 3)
                            {
                                editProject.Projectcatgry_Qust_1 = query.SponProjectCategory;
                                //  editProject.ForgnProjectFundingGovtBody_Qust_1 = query.ForgnFundGovtBody;                              
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.ForgnProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _forgngovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _forgngovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgngovtbody[i] = Convert.ToInt32(fundingbodyquery[i].ForgnProjectFundingGovtBody);
                                        _forgngovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                    editProject.ForgnProjectFundingGovtBody_Qust_1 = _forgngovtbody;
                                    editProject.ForgnProjectFundingGovtBodyId = _forgngovtbodyid;
                                }

                                var Forgnnongovtquery = (from f in context.tblProjectFundingBody
                                                         where f.ProjectId == ProjectId && f.ForgnProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                         select f).ToList();
                                Nullable<int>[] _forgnnongovtbody = new Nullable<int>[Forgnnongovtquery.Count];
                                int[] _forgnnongovtbodyid = new int[Forgnnongovtquery.Count];
                                if (Forgnnongovtquery != null)
                                {
                                    for (int i = 0; i < Forgnnongovtquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgnnongovtbody[i] = Convert.ToInt32(Forgnnongovtquery[i].ForgnProjectFundingNonGovtBody);
                                        _forgnnongovtbodyid[i] = Convert.ToInt32(Forgnnongovtquery[i].FundingBodyId);

                                    }
                                    editProject.ForgnProjectFundingNonGovtBody_Qust_1 = _forgnnongovtbody;
                                    editProject.ForgnProjectFundingNonGovtBodyId = _forgnnongovtbodyid;
                                }
                            }
                        }

                        if (editProject.ProjectType == 1 && editProject.ProjectFundingType_Qust_1 == 3)
                        {

                            // spon Ind funded
                            editProject.ProjectFundedby_Qust_1 = query.IndianFundedBy;
                            // Spon Ind funded govt
                            if (editProject.ProjectFundedby_Qust_1 == 1)
                            {

                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.IndProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _indgovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _indgovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        _indgovtbody[i] = Convert.ToInt32(fundingbodyquery[i].IndProjectFundingGovtBody);
                                        _indgovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                }
                                editProject.IndProjectFundingGovtBody_Qust_1 = _indgovtbody;
                                editProject.IndProjectFundingGovtBodyId = _indgovtbodyid;
                            }

                            // Spon Ind funded non govt
                            else if (editProject.ProjectFundedby_Qust_1 == 2)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.IndProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _indnongovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _indnongovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _indnongovtbody[i] = Convert.ToInt32(fundingbodyquery[i].IndProjectFundingNonGovtBody);
                                        _indnongovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                }
                                editProject.IndProjectFundingNonGovtBody_Qust_1 = _indnongovtbody;
                                editProject.IndProjectFundingNonGovtBodyId = _indnongovtbodyid;
                            }

                            // Spon Ind funded both govt and non govt
                            else if (editProject.ProjectFundedby_Qust_1 == 3)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.IndProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();


                                Nullable<int>[] _indgovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _indgovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        _indgovtbody[i] = Convert.ToInt32(fundingbodyquery[i].IndProjectFundingGovtBody);
                                        _indgovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                }
                                editProject.IndProjectFundingGovtBody_Qust_1 = _indgovtbody;
                                editProject.IndProjectFundingGovtBodyId = _indgovtbodyid;


                                var Indnongovtquery = (from f in context.tblProjectFundingBody
                                                       where f.ProjectId == ProjectId && f.IndProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                       select f).ToList();
                                Nullable<int>[] _indnongovtbody = new Nullable<int>[Indnongovtquery.Count];
                                int[] _indnongovtbodyid = new int[Indnongovtquery.Count];
                                if (Indnongovtquery != null)
                                {
                                    for (int i = 0; i < Indnongovtquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _indnongovtbody[i] = Convert.ToInt32(Indnongovtquery[i].IndProjectFundingNonGovtBody);
                                        _indnongovtbodyid[i] = Convert.ToInt32(Indnongovtquery[i].FundingBodyId);
                                    }

                                }
                                editProject.IndProjectFundingNonGovtBody_Qust_1 = _indnongovtbody;
                                editProject.IndProjectFundingNonGovtBodyId = _indnongovtbodyid;
                            }


                            // Spon Forgn Funded

                            editProject.ForgnProjectFundedby_Qust_1 = query.ForeignFundedBy;
                            editProject.SelCurr = query.SelCurr;
                            editProject.ConversionRate = query.ConversionRate;
                            // Spon Forgn Funded govt
                            if (editProject.ForgnProjectFundedby_Qust_1 == 1)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.ForgnProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _forgngovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _forgngovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgngovtbody[i] = Convert.ToInt32(fundingbodyquery[i].ForgnProjectFundingGovtBody);
                                        _forgngovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);

                                    }

                                }
                                editProject.ForgnProjectFundingGovtBody_Qust_1 = _forgngovtbody;
                                editProject.ForgnProjectFundingGovtBodyId = _forgngovtbodyid;

                            }
                            // Spon Forgn Funded non govt
                            else if (editProject.ForgnProjectFundedby_Qust_1 == 2)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.ForgnProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();
                                Nullable<int>[] _forgnnongovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _forgnnongovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgnnongovtbody[i] = Convert.ToInt32(fundingbodyquery[i].ForgnProjectFundingNonGovtBody);
                                        _forgnnongovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);
                                    }
                                }
                                editProject.ForgnProjectFundingNonGovtBody_Qust_1 = _forgnnongovtbody;
                                editProject.ForgnProjectFundingNonGovtBodyId = _forgnnongovtbodyid;
                            }

                            // Spon Forgn Funded both govt and non govt
                            else if (editProject.ForgnProjectFundedby_Qust_1 == 3)
                            {
                                var fundingbodyquery = (from f in context.tblProjectFundingBody
                                                        where f.ProjectId == ProjectId && f.ForgnProjectFundingGovtBody != null && f.IsDeleted != true
                                                        select f).ToList();

                                Nullable<int>[] _forgngovtbody = new Nullable<int>[fundingbodyquery.Count];
                                int[] _forgngovtbodyid = new int[fundingbodyquery.Count];
                                if (fundingbodyquery != null)
                                {
                                    for (int i = 0; i < fundingbodyquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgngovtbody[i] = Convert.ToInt32(fundingbodyquery[i].ForgnProjectFundingGovtBody);
                                        _forgngovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);

                                    }
                                }
                                editProject.ForgnProjectFundingGovtBody_Qust_1 = _forgngovtbody;
                                editProject.ForgnProjectFundingGovtBodyId = _forgngovtbodyid;

                                var Forgnnongovtquery = (from f in context.tblProjectFundingBody
                                                         where f.ProjectId == ProjectId && f.ForgnProjectFundingNonGovtBody != null && f.IsDeleted != true
                                                         select f).ToList();
                                Nullable<int>[] _forgnnongovtbody = new Nullable<int>[Forgnnongovtquery.Count];
                                int[] _forgnnongovtbodyid = new int[Forgnnongovtquery.Count];
                                if (Forgnnongovtquery != null)
                                {
                                    for (int i = 0; i < Forgnnongovtquery.Count; i++)
                                    {
                                        // tblProjectFundingBody ProjectFunding = new tblProjectFundingBody();
                                        _forgnnongovtbody[i] = Convert.ToInt32(Forgnnongovtquery[i].ForgnProjectFundingNonGovtBody);
                                        _forgnnongovtbodyid[i] = Convert.ToInt32(fundingbodyquery[i].FundingBodyId);

                                    }
                                    editProject.ForgnProjectFundingNonGovtBody_Qust_1 = _forgnnongovtbody;
                                    editProject.ForgnProjectFundingNonGovtBodyId = _forgnnongovtbodyid;
                                }
                            }

                        }

                        editProject.Remarks = query.Remarks;

                        editProject.Collaborativeprojectcoordinator = query.Collaborativeprojectcoordinator;
                        editProject.CollaborativeProjectType = query.CollaborativeProjectType;
                        editProject.CollaborativeprojectAgency = query.CollaborativeprojectAgency;

                        editProject.Collaborativeprojectcoordinatoremail = query.Collaborativeprojectcoordinatoremail;
                        editProject.Collaborativeprojecttotalcost = query.Collaborativeprojecttotalcost;
                        editProject.CollaborativeprojectIITMCost = query.CollaborativeprojectIITMCost;
                        editProject.Agencycontactpersonaddress = query.Agencycontactpersonaddress;
                        editProject.ConsConversionRate = query.ConversionRate;
                        editProject.ConsSelCurr = query.SelCurr;
                        //editProject.ConsForgnCurrencyType = query.ConsForgnCurrencyType;
                        editProject.TaxserviceGST = query.TaxserviceGST;
                        editProject.Taxserviceregstatus = query.Taxserviceregstatus;
                        editProject.BaseValue = query.BaseValue;
                        editProject.ApplicableTax = query.ApplicableTax;
                        editProject.TypeofProject = query.TypeOfProject;


                        editProject.Categoryofproject = query.CategoryOfProject;

                        editProject.PIname = Convert.ToInt32(query.PIName);
                        editProject.PIDesignation = query.PIDesignation;
                        editProject.SponsoringAgency = query.SponsoringAgency;
                        editProject.Sanctionvalue = query.SanctionValue;
                        //editProject.Projectdurationyears = query.DurationOfProjectYears;
                        //editProject.Projectdurationmonths = query.DurationOfProjectMonths;
                        editProject.SchemePersonAppliedDesignation = query.SchemePersonDesignation;
                        editProject.SchemePersonApplied = query.SchemePersonApplied;
                        editProject.ProjectFundingType_Qust_1 = query.FundingType;
                        editProject.Remarks = query.Remarks;

                    }

                    if (CoPIquery.Count != 0)
                    {
                        int[] _prposalid = new int[CoPIquery.Count];
                        int[] _prjectid = new int[CoPIquery.Count];
                        int[] _copiid = new int[CoPIquery.Count];
                        string[] _copidepartment = new string[CoPIquery.Count];
                        int[] _copiname = new int[CoPIquery.Count];
                        int[] _copidesignation = new int[CoPIquery.Count];
                        //string[] _copidepartmentname = new string[CoPIquery.Count];
                        //string[] _copidesig = new string[CoPIquery.Count];
                        string[] _copiemail = new string[CoPIquery.Count];
                        Nullable<Decimal>[] _copiPCF = new Nullable<Decimal>[CoPIquery.Count];
                        Nullable<Decimal>[] _copiRMF = new Nullable<Decimal>[CoPIquery.Count];
                        //List<MasterlistviewModel>[] _piList = new List<MasterlistviewModel>[CoPIquery.Count];
                        for (int i = 0; i < CoPIquery.Count; i++)
                        {
                            var copiid = CoPIquery[i].Name;
                            var copidetails = (from C in context.vwFacultyStaffDetails
                                               where C.UserId == copiid
                                               select C).ToList();
                            //var copidesig = copidetails[0].Designation;
                            //var desigquery = (from CC in context.tblCodeControl
                            //                  where (CC.CodeName == "FacultyCadre" && CC.CodeValAbbr == copidesig)
                            //                  select CC).SingleOrDefault();
                            _prjectid[i] = Convert.ToInt32(CoPIquery[i].ProjectId);
                            _copiid[i] = CoPIquery[i].CoPIId;
                            _copidepartment[i] = CoPIquery[i].Department;

                            _copiname[i] = Convert.ToInt32(CoPIquery[i].Name);
                            _copiemail[i] = copidetails.Count > 0 ? copidetails[0].Email : string.Empty;
                            _copiRMF[i] = CoPIquery[i].RMF;
                            _copiPCF[i] = CoPIquery[i].PCF;
                        }
                        editProject.CoPIname = _copiname;
                        editProject.CoPIDepartment = _copidepartment;
                        editProject.CoPIEmail = _copiemail;
                        //editProject.CoPIDesignation = _copidesignation;
                        editProject.CoPIid = _copiid;
                        //editProject.PIListDepWise = _piList;
                        //editProject.CopiDesig = _copidesig;
                        editProject.CoPIRMF = _copiRMF;
                        editProject.CoPIPCF = _copiPCF;


                    }
                    int _oicopiCount = OtherInstituteCoPIquery.Count; ;
                    if (_oicopiCount > 0)
                    {
                        int[] _copiid = new int[_oicopiCount];
                        string[] _copiinstitute = new string[_oicopiCount];
                        string[] _copidepartment = new string[_oicopiCount];
                        string[] _copiname = new string[_oicopiCount];
                        string[] _copiremarks = new string[_oicopiCount];
                        for (int i = 0; i < _oicopiCount; i++)
                        {
                            _copiinstitute[i] = OtherInstituteCoPIquery[i].Institution;
                            _copidepartment[i] = OtherInstituteCoPIquery[i].Department;
                            _copiname[i] = OtherInstituteCoPIquery[i].Name;
                            _copiid[i] = OtherInstituteCoPIquery[i].OtherCoPIId;
                            _copiremarks[i] = OtherInstituteCoPIquery[i].Remarks;
                        }
                        editProject.CoPIInstitute = _copiinstitute;
                        editProject.OtherInstituteCoPIDepartment = _copidepartment;
                        editProject.OtherInstituteCoPIName = _copiname;
                        editProject.RemarksforOthrInstCoPI = _copiremarks;
                        editProject.OtherInstituteCoPIid = _copiid;
                    }
                    if (SupportDocquery.Count > 0)
                    {
                        int[] _docid = new int[SupportDocquery.Count];
                        int[] _doctype = new int[SupportDocquery.Count];
                        string[] _docname = new string[SupportDocquery.Count];
                        string[] _attachname = new string[SupportDocquery.Count];
                        string[] _docpath = new string[SupportDocquery.Count];
                        for (int i = 0; i < SupportDocquery.Count; i++)
                        {
                            _docid[i] = Convert.ToInt32(SupportDocquery[i].DocId);
                            _doctype[i] = Convert.ToInt32(SupportDocquery[i].DocType);
                            _docname[i] = SupportDocquery[i].DocName;
                            _docpath[i] = SupportDocquery[i].AttachmentPath;
                            _attachname[i] = SupportDocquery[i].AttachmentName;
                        }
                        editProject.DocType = _doctype;
                        editProject.AttachName = _attachname;
                        editProject.DocName = _docname;
                        editProject.DocPath = _docpath;
                        editProject.Docid = _docid;
                    }

                    if (!isYearWiseAH)
                    {
                        if (allocationquery.Count > 0)
                        {
                            int[] _allocationhead = new int[allocationquery.Count];
                            Nullable<decimal>[] _allocationvalue = new Nullable<decimal>[allocationquery.Count];

                            for (int i = 0; i < allocationquery.Count; i++)
                            {
                                _allocationhead[i] = Convert.ToInt32(allocationquery[i].AllocationHead);
                                _allocationvalue[i] = allocationquery[i].AllocationValue;

                            }

                            editProject.Allocationhead = _allocationhead;
                            editProject.Allocationvalue = _allocationvalue;

                        }
                        if (emiQuery.Count > 0)
                        {
                            var OAQuery = emiQuery.OrderBy(m => m.InstallmentNo).ToList();
                            int count = OAQuery.Count;
                            Nullable<decimal>[] _emiValue = new Nullable<decimal>[count];
                            editProject.NoOfEMI = OAQuery[0].NoOfInstallment;
                            for (int i = 0; i < count; i++)
                            {
                                _emiValue[i] = OAQuery[i].InstallmentValue;

                            }
                            editProject.ArrayEMIValue = _emiValue;
                        }
                    }
                    else
                    {
                        List<YearWiseHead> headList = new List<YearWiseHead>();

                        for (int i = 1; i <= allocatedYear; i++)
                        {
                            YearWiseHead ywh = new YearWiseHead();
                            ywh.Year = i;
                            var alloYWQuery = allocationquery.Where(m => m.Year == i).ToList();
                            int allocCount = alloYWQuery.Count;

                            Nullable<int>[] _allocationhead = new Nullable<int>[allocCount];
                            Nullable<decimal>[] _allocationvalue = new Nullable<decimal>[allocCount];

                            if (allocCount > 0)
                            {
                                for (int j = 0; j < allocCount; j++)
                                {
                                    _allocationhead[j] = alloYWQuery[j].AllocationHead;
                                    _allocationvalue[j] = alloYWQuery[j].AllocationValue;
                                }
                                ywh.AllocationHeadYW = _allocationhead;
                                ywh.AllocationValueYW = _allocationvalue;
                            }
                            if (emiQuery.Count > 0)
                            {
                                var OAQuery = emiQuery.Where(m => m.Year == i).OrderBy(m => m.InstallmentNo).ToList();
                                int count = OAQuery.Count;
                                if (count > 0)
                                {
                                    Nullable<decimal>[] _emiValue = new Nullable<decimal>[count];
                                    for (int j = 0; j < count; j++)
                                    {
                                        _emiValue[j] = OAQuery[j].InstallmentValue;
                                    }
                                    ywh.EMIValue = _emiValue;
                                    ywh.NoOfInstallment = OAQuery[0].NoOfInstallment;
                                    ywh.EMIValueForYear = OAQuery[0].InstallmentValueForYear;
                                }

                            }
                            headList.Add(ywh);
                        }
                        editProject.YearWiseHead = headList;
                    }
                    if (prjctstaffquery.Count > 0)
                    {
                        int[] _prjectid = new int[prjctstaffquery.Count];
                        int[] _staffcategoryid = new int[prjctstaffquery.Count];
                        int[] _staffcategory = new int[prjctstaffquery.Count];
                        int[] _noofstaff = new int[prjctstaffquery.Count];
                        decimal[] _staffsalary = new decimal[prjctstaffquery.Count];

                        for (int i = 0; i < prjctstaffquery.Count; i++)
                        {
                            _prjectid[i] = Convert.ToInt32(prjctstaffquery[i].ProjectId);
                            _staffcategoryid[i] = prjctstaffquery[i].ProjectStaffCategoryId;
                            _staffcategory[i] = Convert.ToInt32(prjctstaffquery[i].ProjectStaffCategory);
                            _noofstaff[i] = Convert.ToInt32(prjctstaffquery[i].NoofStaffs);
                            _staffsalary[i] = Convert.ToDecimal(prjctstaffquery[i].SalaryofStaffs);

                        }

                        editProject.StaffCategoryID = _staffcategoryid;
                        editProject.CategoryofStaffs = _staffcategory;
                        editProject.NoofStaffs = _noofstaff;
                        editProject.SalaryofStaffs = _staffsalary;

                    }

                    if (Companyquery.Count > 0)
                    {
                        int[] _prjectid = new int[Companyquery.Count];
                        int[] _companyid = new int[Companyquery.Count];
                        string[] _companyname = new string[Companyquery.Count];
                        string[] _remarks = new string[Companyquery.Count];

                        for (int i = 0; i < Companyquery.Count; i++)
                        {
                            _prjectid[i] = Convert.ToInt32(Companyquery[i].ProjectId);
                            _companyid[i] = Companyquery[i].JointDevelopCompanyId;
                            _companyname[i] = Companyquery[i].JointDevelopCompanyName;
                            _remarks[i] = Companyquery[i].Remarks;

                        }

                        editProject.JointDevelopmentCompanyId = _companyid;
                        editProject.JointDevelopmentCompany = _companyname;
                        editProject.JointDevelopmentRemarks = _remarks;

                    }
                    return editProject;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int DeleteProject(int ProjectId)
        {
            try
            {
                tblProject project;

                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblProject
                                 where (D.ProjectId == ProjectId)
                                 select D.ProjectId).FirstOrDefault();

                    project = context.tblProject.Where(P => P.ProjectId == ProjectId).FirstOrDefault();
                    context.Entry(project).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();
                    var proposalid = project.ProposalId;
                    var proposalquery = context.tblProposal.Where(P => P.ProposalId == proposalid).FirstOrDefault();
                    proposalquery.Status = "InActive";
                    context.SaveChanges();
                }
                return 4;
            }
            catch (Exception ex)
            {
                return 4;
            }
        }
        public static List<CreateProjectModel> GetProjectList()
        {
            List<CreateProjectModel> project = new List<CreateProjectModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from P in context.tblProject
                             join pi in context.vwFacultyStaffDetails on P.PIName equals pi.UserId
                             //join user in context.tblUser on P.PIName equals user.UserId
                             //join dept in context.tblPIDepartmentMaster on P.PIDepartment equals dept.DepartmentId
                             join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId
                             where P.ProjectClassification == 1
                             orderby P.ProjectId descending
                             select new { P, pi.FirstName, pi.EmployeeId, pi.DepartmentName, agency.AgencyName }).Take(100).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        project.Add(new CreateProjectModel()
                        {
                            Sno = i + 1,
                            ProjectID = query[i].P.ProjectId,
                            Projecttitle = query[i].P.ProjectTitle,
                            ProjectNumber = query[i].P.ProjectNumber,
                            Budget = query[i].P.BaseValue,
                            SponsoringAgency = query[i].P.SponsoringAgency,
                            SponsoringAgencyName = query[i].AgencyName,
                            NameofPI = query[i].FirstName,
                            PIDepartmentName = query[i].DepartmentName,
                            EmpCode = query[i].EmployeeId,
                            PrpsalApprovedDate = String.Format("{0:s}", query[i].P.ProposalApprovedDate)
                        });
                    }
                }
            }
            return project;
        }
        public static CreateProjectModel getproposaldetails(int proposalid)
        {
            try
            {

                CreateProjectModel Proposaldetails = new CreateProjectModel();

                using (var context = new IOASDBEntities())
                {

                    var query = context.tblProposal.FirstOrDefault(m => m.ProposalId == proposalid);
                    var prjcttypequery = (from C in context.tblCodeControl
                                          where (C.CodeName == "Projecttype" && C.CodeValAbbr == query.ProjectType)
                                          select C).FirstOrDefault();
                    var prjctsubtypequery = (from C in context.tblCodeControl
                                             where (C.CodeName == "ConsultancyProjectSubtype" && C.CodeValAbbr == query.ProjectCategory)
                                             select C).FirstOrDefault();
                    var schemenamequery = (from c in context.tblSchemes
                                           where c.SchemeId == query.Scheme
                                           select c).FirstOrDefault();
                    var pidetailsquery = (from c in context.vwFacultyStaffDetails
                                          where c.UserId == query.PI
                                          select c).FirstOrDefault();
                    var agencydetailsquery = (from c in context.tblAgencyMaster
                                              where c.AgencyId == query.SponsoringAgency
                                              select c).FirstOrDefault();
                    var SupportDocquery = (from Doc in context.tblProposalSupportDocuments
                                           where (Doc.ProposalId == proposalid)
                                           select Doc).ToList();

                    var Proposalid = proposalid;
                    if (query != null)
                    {
                        //int deptID = query.Department ?? 0;
                        //int instID = 3;
                        Proposaldetails.MainProjectList = Common.GetMainProjectNumberList(query.ProjectType ?? 0);
                        Proposaldetails.ProposalNumber = query.ProposalNumber;
                        Proposaldetails.ProposalID = query.ProposalId;
                        //Proposaldetails.Projectdurationyears = query.DurationOfProjectYears;
                        //Proposaldetails.Projectdurationmonths = query.DurationOfProjectMonths;
                        Proposaldetails.ProjectType = query.ProjectType;
                        Proposaldetails.Projecttitle = query.ProposalTitle;
                        Proposaldetails.ProjectTypeName = prjcttypequery.CodeValDetail;
                        Proposaldetails.Department = query.Department;
                        Proposaldetails.PIname = query.PI;
                        //Proposaldetails.PIDesignation = pidetailsquery.Designation;
                        Proposaldetails.PIEmail = pidetailsquery.Email;
                        //Proposaldetails.Sanctionvalue = query.ProposalValue;
                        Proposaldetails.SponsoringAgency = query.SponsoringAgency;
                        Proposaldetails.AgencyCode = agencydetailsquery.AgencyCode;
                        Proposaldetails.AgencyCodeid = agencydetailsquery.AgencyId;
                        Proposaldetails.Agencycontactperson = agencydetailsquery.ContactPerson;
                        Proposaldetails.Agencycontactpersonaddress = agencydetailsquery.Address;
                        Proposaldetails.AgencycontactpersonEmail = agencydetailsquery.ContactEmail;
                        Proposaldetails.Agencycontactpersonmobile = agencydetailsquery.ContactNumber;
                        Proposaldetails.GSTNumber = agencydetailsquery.GSTIN;
                        Proposaldetails.PAN = agencydetailsquery.PAN;
                        Proposaldetails.TAN = agencydetailsquery.TAN;
                        Proposaldetails.Agencyregaddress = agencydetailsquery.AgencyRegisterAddress;
                        Proposaldetails.Agencyregname = agencydetailsquery.AgencyRegisterName;
                        Proposaldetails.MasterPIListDepWise = AccountService.getPIList(query.Department);
                        //if (query.ProposalApproveddate != null)
                        //{
                        //    Proposaldetails.ProposalApprovedDate = (DateTime)query.ProposalApproveddate;
                        //    Proposaldetails.PrpsalApprovedDate = String.Format("{0:dd}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ProposalApproveddate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ProposalApproveddate);
                        //}
                        if (query.TentativeStartDate != null)
                        {
                            Proposaldetails.TentativestrtDate = String.Format("{0:dd}", (DateTime)query.TentativeStartDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.TentativeStartDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.TentativeStartDate);
                            Proposaldetails.TentativeStartdate = (DateTime)query.TentativeStartDate;
                        }
                        if (query.TentativeCloseDate != null)
                        {
                            Proposaldetails.TentativeclsDate = String.Format("{0:dd}", (DateTime)query.TentativeCloseDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.TentativeCloseDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.TentativeCloseDate);
                            Proposaldetails.TentativeClosedate = (DateTime)query.TentativeCloseDate;
                        }
                        if (query.ProjectType == 1)
                        {

                            Proposaldetails.SanctionOrderNumber = query.SanctionNumber;
                            Proposaldetails.ProjectSubType = query.ProjectSubType;
                            Proposaldetails.InternalSchemeFundingAgency = Convert.ToInt32(query.SponsoringAgency);

                            if (query.ProjectSubType == 2)
                            {
                                Proposaldetails.Categoryofproject = query.ProjectCategory;
                            }
                        }
                        Proposaldetails.BaseValue = query.BasicValue;
                        Proposaldetails.ApplicableTax = query.ApplicableTax;

                        Proposaldetails.Schemename = query.Scheme;
                        Proposaldetails.SchemeCode = query.ProjectSchemeCode;

                        if (query.ProjectType == 2)
                        {
                            Proposaldetails.ConsProjectSubType = query.ProjectCategory;
                            Proposaldetails.ConsFundingCategory = query.Scheme;
                            Proposaldetails.Conssubtypename = prjctsubtypequery.CodeValDetail;
                            Proposaldetails.ConsFundingcategoryname = schemenamequery.SchemeName;
                            Proposaldetails.OverheadPercentage = schemenamequery.OverheadPercentage;
                            Proposaldetails.GSTNumber = agencydetailsquery.GSTIN;
                            Proposaldetails.TAN = agencydetailsquery.TAN;
                            Proposaldetails.PAN = agencydetailsquery.PAN;

                            if (agencydetailsquery.AgencyCountryCategoryId == 1)
                            {
                                Proposaldetails.constaxservice = agencydetailsquery.IndianAgencyCategoryId;
                                if (agencydetailsquery.IndianAgencyCategoryId == 2)
                                    Proposaldetails.Taxserviceregstatus = agencydetailsquery.NonSezCategoryId;
                                Proposaldetails.indfundngagncystate = Common.GetStateName(agencydetailsquery.StateId ?? 0);
                                Proposaldetails.indfundngagncylocation = agencydetailsquery.AgencyRegisterAddress;
                            }
                            else
                            {
                                Proposaldetails.constaxservice = 3;
                                Proposaldetails.forgnfndngagncycountry = agencydetailsquery.Country;
                                Proposaldetails.forgnfundngagncystate = agencydetailsquery.State;
                                Proposaldetails.forgnfundngagncylocation = agencydetailsquery.AgencyRegisterAddress;
                            }
                        }
                        //int CurrentYear = (DateTime.Today.Year) % 100;
                        //int PreviousYear = (DateTime.Today.Year - 1) % 100;
                        //int NextYear = (DateTime.Today.Year + 1) % 100;
                        //string PreYear = PreviousYear.ToString();
                        //string NexYear = NextYear.ToString();
                        //string CurYear = CurrentYear.ToString();
                        //string FinYear = null;

                        //if (DateTime.Today.Month > 3)
                        //    FinYear = CurYear + NexYear;
                        //else
                        //    FinYear = PreYear + CurYear;
                        Proposaldetails.FinancialYear = query.FinancialYear;
                    }


                    var copi = (from C in context.tblProposalCoPI
                                where C.ProposalId == Proposalid && C.isdeleted != true
                                select C).ToList();

                    if (copi.Count != 0)
                    {
                        int[] _prposalid = new int[copi.Count];
                        int[] _copiid = new int[copi.Count];
                        string[] _copidepartment = new string[copi.Count];
                        int[] _copiname = new int[copi.Count];
                        string[] _copiemail = new string[copi.Count];
                        //List<MasterlistviewModel>[] _piList = new List<MasterlistviewModel>[copi.Count];
                        for (int i = 0; i < copi.Count; i++)
                        {
                            var copiid = copi[i].Name;
                            //var instid = 3;
                            //int depId = Convert.ToInt32(copi[i].Department);
                            var copidetails = (from C in context.vwFacultyStaffDetails
                                               where C.UserId == copiid
                                               select C).ToList();
                            //var depquery = (from Dep in context.tblPIDepartmentMaster
                            //                where (Dep.DepartmentId == depId)
                            //                select Dep).SingleOrDefault();


                            _prposalid[i] = Convert.ToInt32(copi[i].ProposalId);
                            _copiid[i] = copi[i].CoPIId;
                            _copidepartment[i] = copi[i].Department;
                            //_copidepartmentname[i] = depquery.Department;
                            _copiname[i] = Convert.ToInt32(copi[i].Name);
                            _copiemail[i] = copi[i].Email;
                            //_piList[i] = AccountService.getPIList(depId, instid);

                        }
                        Proposaldetails.CoPIname = _copiname;
                        Proposaldetails.CoPIDepartment = _copidepartment;
                        Proposaldetails.CoPIEmail = _copiemail;
                        Proposaldetails.CoPIid = _copiid;
                        //Proposaldetails.PIListDepWise = _piList;
                    }
                    var OtherInstituteCoPIquery = (from CoPI in context.tblOtherInstituteCoPI
                                                   where CoPI.ProposalId == Proposalid && CoPI.IsDeleted != true
                                                   select CoPI).ToList();
                    var _oicopiCount = OtherInstituteCoPIquery.Count;
                    if (_oicopiCount > 0)
                    {
                        int[] _prposalid = new int[_oicopiCount];
                        int[] _copiid = new int[_oicopiCount];
                        string[] _copiinstitute = new string[_oicopiCount];
                        string[] _copidepartment = new string[_oicopiCount];
                        string[] _copiname = new string[_oicopiCount];
                        string[] _copiremarks = new string[_oicopiCount];
                        for (int i = 0; i < _oicopiCount; i++)
                        {
                            _prposalid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].ProposalId);
                            _copiinstitute[i] = OtherInstituteCoPIquery[i].Institution;
                            _copidepartment[i] = OtherInstituteCoPIquery[i].Department;
                            _copiname[i] = OtherInstituteCoPIquery[i].Name;
                            _copiid[i] = Convert.ToInt32(OtherInstituteCoPIquery[i].CoPIId);
                            _copiremarks[i] = OtherInstituteCoPIquery[i].Remarks;
                        }
                        Proposaldetails.CoPIInstitute = _copiinstitute;
                        Proposaldetails.OtherInstituteCoPIDepartment = _copidepartment;
                        Proposaldetails.OtherInstituteCoPIName = _copiname;
                        Proposaldetails.RemarksforOthrInstCoPI = _copiremarks;
                        Proposaldetails.OtherInstituteCoPIid = _copiid;
                    }
                    if (SupportDocquery.Count > 0)
                    {
                        int[] _docid = new int[SupportDocquery.Count];
                        int[] _doctype = new int[SupportDocquery.Count];
                        string[] _docname = new string[SupportDocquery.Count];
                        string[] _attachname = new string[SupportDocquery.Count];
                        string[] _docpath = new string[SupportDocquery.Count];
                        for (int i = 0; i < SupportDocquery.Count; i++)
                        {
                            _docid[i] = Convert.ToInt32(SupportDocquery[i].DocId);
                            _doctype[i] = Convert.ToInt32(SupportDocquery[i].DocType);
                            _docname[i] = SupportDocquery[i].DocName;
                            _docpath[i] = SupportDocquery[i].AttachmentPath;
                            _attachname[i] = SupportDocquery[i].AttachmentName;
                        }
                        Proposaldetails.DocType = _doctype;
                        Proposaldetails.AttachName = _attachname;
                        Proposaldetails.DocName = _docname;
                        Proposaldetails.DocPath = _docpath;
                        Proposaldetails.Docid = _docid;
                    }


                }

                return Proposaldetails;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CreateProposalModel> GetProposalDetails()
        {
            List<CreateProposalModel> proposal = new List<CreateProposalModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from P in context.tblProposal
                             join pi in context.vwFacultyStaffDetails on P.PI equals pi.UserId
                             //join user in context.tblUser on P.PI equals user.UserId
                             //join dept in context.tblPIDepartmentMaster on P.Department equals dept.DepartmentId
                             where P.Status == "Active" && P.IsDeleted != true
                             orderby P.ProposalId descending
                             select new { P, pi.FirstName, pi.EmployeeId, pi.DepartmentName }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        proposal.Add(new CreateProposalModel()
                        {
                            Sno = i + 1,
                            ProposalID = query[i].P.ProposalId,
                            ProposalNumber = query[i].P.ProposalNumber,
                            Projecttitle = query[i].P.ProposalTitle,
                            BasicValue = query[i].P.BasicValue,
                            NameofPI = query[i].FirstName,
                            EmpCode = query[i].EmployeeId,
                            PIDepartmentName = query[i].DepartmentName
                        });
                    }
                }
            }
            return proposal;
        }
        public static List<MasterlistviewModel> LoadProjecttitledetails(int projecttype)
        {
            try
            {

                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {
                    if (projecttype == 1 || projecttype == 2)
                    {


                        var query = (from C in context.tblProject
                                     join U in context.vwFacultyStaffDetails on C.PIName equals U.UserId
                                     where (C.Status == "Active" && C.ProjectType == projecttype)
                                     orderby C.ProjectId
                                     select new { U.FirstName, C }).ToList();


                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Title.Add(new MasterlistviewModel()
                                {
                                    id = query[i].C.ProjectId,
                                    name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "- " + query[i].FirstName,
                                });
                            }
                        }

                    }
                }



                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<ProjectEnhancementModel> GetEnhancedProjectList()
        {
            List<ProjectEnhancementModel> project = new List<ProjectEnhancementModel>();
            using (var context = new IOASDBEntities())


            {
                var query = (from enhance in context.tblProjectEnhancement
                             join P in context.tblProject on enhance.ProjectId equals P.ProjectId
                             where enhance.Status == "Active"
                             orderby enhance.ProjectEnhancementId descending
                             select new { enhance, P.PIName, P.BaseValue, P.ProjectNumber, P.ProjectId, P.ProjectTitle }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        int projectId = query[i].ProjectId;
                        project.Add(new ProjectEnhancementModel()
                        {
                            Sno = i + 1,
                            ProjectID = projectId,
                            ProjectEnhancementID = query[i].enhance.ProjectEnhancementId,
                            Projecttitle = query[i].ProjectTitle,
                            ProjectNumber = query[i].ProjectNumber,
                            PIname = Common.GetPIName(query[i].PIName ?? 0),
                            OldSanctionValue = query[i].enhance.OldSanctionValue,
                            PresentDueDate = query[i].enhance.PresentDueDate,
                            EnhancedSanctionValue = query[i].enhance.EnhancedSanctionValue ?? 0,
                            isCurrentVersion = query[i].enhance.IsCurrentVersion ?? false,
                            PrsntDueDate = query[i].enhance.ExtendedDueDate == null ? String.Format("{0:s}", query[i].enhance.PresentDueDate) : String.Format("{0:s}", query[i].enhance.ExtendedDueDate),
                            //ExtndDueDate = query[i].enhance.ExtendedDueDate != null ? String.Format("{0:s}", query[i].enhance.ExtendedDueDate) : Common.GetProjectDueDate(projectId)
                        });
                    }
                }
            }
            return project;
        }
        public static ProjectEnhancementModel getprojectdetailsforenhance(int projectid)
        {
            try
            {

                ProjectEnhancementModel Projectdetails = new ProjectEnhancementModel();

                using (var context = new IOASDBEntities())
                {

                    var query = (from P in context.tblProject
                                 where (P.ProjectId == projectid && P.Status == "Active")
                                 select P).FirstOrDefault();
                    var prjctenhancequery = (from E in context.tblProjectEnhancement
                                             where (E.ProjectId == projectid && E.IsCurrentVersion == true)
                                             select E).FirstOrDefault();
                    var enhancementid = 0;
                    bool isYearWiseBH = false;
                    if (query != null)
                    {
                        isYearWiseBH = query.IsYearWiseAllocation ?? false;
                        Projectdetails.ProjectID = query.ProjectId;
                        Projectdetails.ProjectNumber = query.ProjectNumber;
                        Projectdetails.Projecttitle = query.ProjectTitle;
                        Projectdetails.OldSanctionValue = query.SanctionValue;

                        if (prjctenhancequery != null)
                        {
                            if (prjctenhancequery.ExtendedDueDate != null)
                                Projectdetails.PresentDueDate = prjctenhancequery.ExtendedDueDate;
                            enhancementid = prjctenhancequery.ProjectEnhancementId;
                        }
                        if (Projectdetails.PresentDueDate == null)
                        {
                            var extndDate = Common.GetProjectDueDate(query.ProjectId);
                            Projectdetails.PresentDueDate = extndDate == null ? query.ActuaClosingDate : extndDate;
                        }


                    }


                    var enhanceallocation = (from C in context.tblProjectEnhancementAllocation
                                             where C.ProjectId == projectid && C.IsCurrentVersion == true
                                             //orderby C.ProjectEnhancementAllocationId descending
                                             select C).ToList();
                    if (enhanceallocation.Count == 0)
                    {
                        var allocation = (from C in context.tblProjectAllocation
                                          where (C.ProjectId == projectid)
                                          select C).ToList();
                        //int[] _prjectid = new int[allocation.Count];

                        if (isYearWiseBH)
                        {
                            int distCount = allocation.GroupBy(p => p.AllocationHead).Count();
                            int[] _allocationid = new int[distCount];
                            int[] _allocationhead = new int[distCount];
                            Nullable<decimal>[] _allocationvalue = new Nullable<decimal>[distCount];
                            int index = 0;
                            for (int i = 0; i < allocation.Count; i++)
                            {
                                int headId = allocation[i].AllocationHead ?? 0;
                                if (!_allocationhead.Contains(headId))
                                {
                                    _allocationid[index] = allocation[i].AllocationId;
                                    _allocationhead[index] = headId;
                                    _allocationvalue[index] = allocation.Where(m => m.AllocationHead == headId).Sum(m => m.AllocationValue);
                                    index++;
                                }
                            }
                            Projectdetails.AllocationId = _allocationid;
                            Projectdetails.Allocationhead = _allocationhead;
                            Projectdetails.OldAllocationvalue = _allocationvalue;
                        }
                        else
                        {
                            int[] _allocationid = new int[allocation.Count];
                            int[] _allocationhead = new int[allocation.Count];
                            Nullable<decimal>[] _allocationvalue = new Nullable<decimal>[allocation.Count];
                            for (int i = 0; i < allocation.Count; i++)
                            {
                                _allocationid[i] = allocation[i].AllocationId;
                                _allocationhead[i] = Convert.ToInt32(allocation[i].AllocationHead);
                                _allocationvalue[i] = allocation[i].AllocationValue;

                            }
                            Projectdetails.AllocationId = _allocationid;
                            Projectdetails.Allocationhead = _allocationhead;
                            Projectdetails.OldAllocationvalue = _allocationvalue;
                        }
                    }
                    else if (enhanceallocation.Count != 0)
                    {

                        //int[] _prjectid = new int[enhanceallocation.Count];
                        int[] _allocationid = new int[enhanceallocation.Count];
                        int[] _allocationhead = new int[enhanceallocation.Count];
                        Nullable<decimal>[] _allocationvalue = new Nullable<decimal>[enhanceallocation.Count];

                        for (int i = 0; i < enhanceallocation.Count; i++)
                        {
                            //_prjectid[i] = Convert.ToInt32(enhanceallocation[i].ProjectId);
                            _allocationid[i] = enhanceallocation[i].ProjectEnhancementAllocationId;
                            _allocationhead[i] = Convert.ToInt32(enhanceallocation[i].AllocationHead);
                            _allocationvalue[i] = enhanceallocation[i].TotalValue != null ? enhanceallocation[i].TotalValue : enhanceallocation[i].OldValue;

                        }
                        Projectdetails.AllocationId = _allocationid;
                        Projectdetails.Allocationhead = _allocationhead;
                        Projectdetails.OldAllocationvalue = _allocationvalue;


                    }


                }

                return Projectdetails;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public int ProjectEnhancement(ProjectEnhancementModel model, HttpPostedFileBase file)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var projectid = model.ProjectID;
                        tblProjectEnhancement enhance = new tblProjectEnhancement();
                        //var chkproject = context.tblProject.FirstOrDefault(dup => dup.ProjectId == projectid);

                        var enhancequery = (from c in context.tblProjectEnhancement
                                            where c.ProjectId == projectid
                                            select c).ToList();

                        enhancequery.ForEach(m => m.IsCurrentVersion = false);
                        if (file != null)
                        {
                            string filepath = " ";
                            filepath = System.IO.Path.GetFileName(file.FileName);
                            var fileId = Guid.NewGuid().ToString();
                            var docname = fileId + "_" + filepath;

                            /*Saving the file in server folder*/
                            file.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));
                            enhance.AttachmentPath = docname;
                        }

                        enhance.ProjectId = projectid;
                        enhance.Status = "Active";// chkproject.ProjectStatus;
                        enhance.CrtdUserId = model.CrtdUserid;
                        enhance.CrtsTS = DateTime.Now;
                        enhance.PresentDueDate = model.PresentDueDate;
                        enhance.OldSanctionValue = model.OldSanctionValue;

                        if (model.Enhancement_Qust_1 == "Yes" && model.Extension_Qust_1 == "Yes")
                        {
                            enhance.IsExtensiononly = true;
                            enhance.IsEnhancementonly = true;
                            enhance.IsEnhancementWithExtension = true;

                            enhance.DocumentReferenceNumber = model.DocumentReferenceNumber;
                            enhance.EnhancedSanctionValue = model.EnhancedSanctionValue;
                            enhance.TotalAllocatedValue = model.TotalAllocatedvalue;
                            enhance.TotalEnhancedValue = model.TotalEnhancedAllocationvalue;

                            enhance.ExtendedDueDate = model.ExtendedDueDate;
                            enhance.AttachmentName = model.AttachmentName;
                        }
                        else if (model.Enhancement_Qust_1 == "Yes")
                        {
                            enhance.IsEnhancementonly = true;
                            enhance.IsEnhancementWithExtension = false;
                            enhance.IsExtensiononly = false;
                            enhance.DocumentReferenceNumber = model.DocumentReferenceNumber;
                            enhance.EnhancedSanctionValue = model.EnhancedSanctionValue;
                            enhance.TotalAllocatedValue = model.TotalAllocatedvalue;
                            enhance.TotalEnhancedValue = model.TotalEnhancedAllocationvalue;

                        }
                        else if (model.Extension_Qust_1 == "Yes")
                        {
                            enhance.IsEnhancementonly = false;
                            enhance.IsEnhancementWithExtension = false;
                            enhance.IsExtensiononly = true;

                            enhance.ExtendedDueDate = model.ExtendedDueDate;
                            enhance.AttachmentName = model.AttachmentName;

                        }

                        enhance.IsCurrentVersion = true;
                        context.tblProjectEnhancement.Add(enhance);
                        context.SaveChanges();
                        int projectenhancementid = enhance.ProjectEnhancementId;
                        if (model.Enhancement_Qust_1 == "Yes")
                        {
                            var enhanceallocationquery = (from alloc in context.tblProjectEnhancementAllocation
                                                          where alloc.ProjectId == projectid
                                                          select alloc).ToList();
                            enhanceallocationquery.ForEach(m => m.IsCurrentVersion = false);
                            if (model.Allocationhead != null)
                            {
                                for (int i = 0; i < model.Allocationhead.Length; i++)
                                {
                                    if (model.Allocationhead[i] != 0)
                                    {
                                        tblProjectEnhancementAllocation EnhanceAllocation = new tblProjectEnhancementAllocation();
                                        EnhanceAllocation.AllocationHead = model.Allocationhead[i];
                                        EnhanceAllocation.ProjectId = projectid;
                                        EnhanceAllocation.ProjectEnhancementId = projectenhancementid;
                                        EnhanceAllocation.OldValue = model.OldAllocationvalue[i];
                                        EnhanceAllocation.EnhancedValue = model.EnhancedAllocationvalue[i];
                                        EnhanceAllocation.TotalValue = model.HeadwiseTotalAllocationvalue[i];
                                        EnhanceAllocation.CrtdUserId = model.CrtdUserid;
                                        EnhanceAllocation.CrtdTS = DateTime.Now;
                                        EnhanceAllocation.Status = "Active";

                                        EnhanceAllocation.IsCurrentVersion = true;
                                        context.tblProjectEnhancementAllocation.Add(EnhanceAllocation);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            if (model.Allochead != null)
                            {
                                for (int i = 0; i < model.Allochead.Length; i++)
                                {
                                    if (model.Allochead[i] != 0)
                                    {
                                        tblProjectEnhancementAllocation EnhanceAllocation = new tblProjectEnhancementAllocation();
                                        EnhanceAllocation.AllocationHead = model.Allochead[i];
                                        EnhanceAllocation.ProjectId = projectid;
                                        EnhanceAllocation.ProjectEnhancementId = projectenhancementid;
                                        EnhanceAllocation.OldValue = model.OldAllocationvalue[i];
                                        EnhanceAllocation.EnhancedValue = model.EnhancedAllocationvalue[i];
                                        EnhanceAllocation.TotalValue = model.HeadwiseTotalAllocationvalue[i];
                                        EnhanceAllocation.CrtdUserId = model.CrtdUserid;
                                        EnhanceAllocation.CrtdTS = DateTime.Now;
                                        EnhanceAllocation.Status = "Active";

                                        EnhanceAllocation.IsCurrentVersion = true;
                                        context.tblProjectEnhancementAllocation.Add(EnhanceAllocation);
                                        context.SaveChanges();

                                    }

                                }
                            }
                            context.SaveChanges();
                        }
                        transaction.Commit();
                        if (model.Enhancement_Qust_1 == "Yes")
                            Common.UpdateSanctionValue(projectid);
                        return projectid;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }

        }

        public int ProjectExtension(ProjectEnhancementModel model, HttpPostedFileBase file)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var projectid = 0;
                    tblProjectEnhancement extend = new tblProjectEnhancement();
                    var chkproject = context.tblProject.FirstOrDefault(dup => dup.ProjectId == model.ProjectID);

                    var userquery = context.vwFacultyStaffDetails.FirstOrDefault(m => m.UserId == chkproject.PIName);
                    var extendquery = context.tblProjectEnhancement.FirstOrDefault(m => m.ProjectEnhancementId == model.ProjectEnhancementID && m.IsCurrentVersion == true);
                    var chkextendproject = (from C in context.tblProjectEnhancement
                                            where (C.ProjectId == model.ProjectID && (C.IsExtensiononly == true || C.IsEnhancementWithExtension == true) && C.IsExtensionOnlyCurrentversion == true)
                                            select C).ToList();

                    if (extendquery == null)
                    {
                        if (model.AttachmentName != null)
                        {
                            string filepath = " ";
                            filepath = System.IO.Path.GetFileName(file.FileName);
                            var fileId = Guid.NewGuid().ToString();
                            var docname = fileId + "_" + filepath;

                            /*Saving the file in server folder*/
                            file.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));
                            extend.AttachmentPath = docname;
                        }

                        extend.ProjectId = model.ProjectID;
                        extend.DocumentReferenceNumber = model.DocumentReferenceNumber;
                        extend.OldSanctionValue = model.OldSanctionValue;
                        extend.Status = "Active";
                        extend.PresentDueDate = model.PresentDueDate;
                        extend.ExtendedDueDate = model.ExtendedDueDate;
                        extend.CrtdUserId = model.CrtdUserid;
                        extend.CrtsTS = DateTime.Now;
                        extend.AttachmentName = model.AttachmentName;
                        if (extend.ExtendedDueDate != null && extend.EnhancedSanctionValue == null)
                        {
                            extend.IsEnhancementonly = false;
                            extend.IsEnhancementWithExtension = false;
                            extend.IsExtensiononly = true;
                        }
                        else if (extend.ExtendedDueDate == null)
                        {
                            extend.IsEnhancementonly = false;
                            extend.IsEnhancementWithExtension = false;
                            extend.IsExtensiononly = false;
                        }
                        extend.IsExtensionOnlyCurrentversion = true;
                        context.tblProjectEnhancement.Add(extend);
                        context.SaveChanges();
                        int prjctid = Convert.ToInt32(extend.ProjectId);
                        projectid = prjctid;
                        if (chkextendproject != null)
                        {
                            for (int i = 0; i < chkextendproject.Count(); i++)
                            {
                                chkextendproject[i].IsExtensionOnlyCurrentversion = false;
                            }

                        }
                    }
                    else
                    {
                        if (model.AttachmentName != null)
                        {
                            string filepath = " ";
                            filepath = System.IO.Path.GetFileName(file.FileName);
                            var fileId = Guid.NewGuid().ToString();
                            var docname = fileId + "_" + filepath;

                            /*Saving the file in server folder*/
                            file.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));
                            extendquery.AttachmentPath = docname;
                        }

                        extendquery.ProjectId = model.ProjectID;
                        extendquery.DocumentReferenceNumber = model.DocumentReferenceNumber;
                        extendquery.OldSanctionValue = model.OldSanctionValue;
                        //extendquery.Status = chkproject.ProjectStatus;
                        extendquery.PresentDueDate = model.PresentDueDate;
                        extendquery.ExtendedDueDate = model.ExtendedDueDate;
                        extendquery.CrtdUserId = model.CrtdUserid;
                        extendquery.CrtsTS = DateTime.Now;
                        extendquery.AttachmentName = model.AttachmentName;
                        if (extendquery.ExtendedDueDate != null && extendquery.EnhancedSanctionValue == null)
                        {
                            extendquery.IsEnhancementonly = false;
                            extendquery.IsEnhancementWithExtension = false;
                            extendquery.IsExtensiononly = true;
                        }
                        else if (extendquery.ExtendedDueDate == null)
                        {
                            extendquery.IsEnhancementonly = false;
                            extendquery.IsEnhancementWithExtension = false;
                            extendquery.IsExtensiononly = false;
                        }
                        extendquery.IsCurrentVersion = true;
                        context.SaveChanges();
                        int prjctid = Convert.ToInt32(extendquery.ProjectId);
                        projectid = prjctid;
                    }
                    return projectid;
                }
            }

            catch (Exception ex)
            {

                return -1;
            }
        }
        public bool DeleteEnhamcement(int enhanceId, int loggedInUserId)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int projectId = 0;
                        var enhanceQuery = context.tblProjectEnhancement.Where(m => m.ProjectEnhancementId == enhanceId && m.IsCurrentVersion == true).FirstOrDefault();
                        if (enhanceQuery != null)
                        {
                            enhanceQuery.Status = "InActive";
                            enhanceQuery.LastUpdtTS = DateTime.Now;
                            enhanceQuery.LastUpdtUserId = loggedInUserId;
                            enhanceQuery.IsCurrentVersion = false;

                            projectId = enhanceQuery.ProjectId ?? 0;
                            var preEnhanceQuery = context.tblProjectEnhancement.Where(m => m.ProjectId == projectId && m.Status == "Active" && m.ProjectEnhancementId != enhanceId).OrderByDescending(p => p.ProjectEnhancementId).FirstOrDefault();
                            if (preEnhanceQuery != null)
                            {
                                preEnhanceQuery.IsCurrentVersion = true;
                                preEnhanceQuery.LastUpdtTS = DateTime.Now;
                                preEnhanceQuery.LastUpdtUserId = loggedInUserId;
                            }
                            var delAllocQuery = context.tblProjectEnhancementAllocation.Where(m => m.ProjectEnhancementId == enhanceId && m.IsCurrentVersion == true).ToList();
                            if (delAllocQuery.Count > 0)
                            {
                                delAllocQuery.ForEach(m =>
                                {
                                    m.Status = "InActive";
                                    m.IsCurrentVersion = false;
                                });
                                var preAllocQuery = context.tblProjectEnhancementAllocation.Where(m => m.ProjectId == projectId && m.Status == "Active" && m.ProjectEnhancementId != enhanceId).OrderByDescending(p => p.ProjectEnhancementAllocationId).FirstOrDefault();
                                if (preAllocQuery != null)
                                {
                                    int preEnahnceId = preAllocQuery.ProjectEnhancementId ?? 0;
                                    context.tblProjectEnhancementAllocation.Where(m => m.ProjectEnhancementId == preEnahnceId).ToList()
                                        .ForEach(p => p.IsCurrentVersion = true);
                                }

                            }

                        }
                        context.SaveChanges();
                        transaction.Commit();
                        Common.UpdateSanctionValue(projectId);
                        return true;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

        }
        public static ProjectEnhancementModel EditEnhancement(int EnhanceId)
        {
            try
            {
                ProjectEnhancementModel editenhancement = new ProjectEnhancementModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProjectEnhancement
                                 where (P.ProjectEnhancementId == EnhanceId)
                                 select P).FirstOrDefault();
                    var allocationquery = (from alloc in context.tblProjectEnhancementAllocation
                                           where (alloc.IsCurrentVersion == true && alloc.ProjectEnhancementId == EnhanceId)
                                           select alloc).ToList();
                    var projectallocquery = (from prjct in context.tblProjectAllocation
                                             where prjct.ProjectId == query.ProjectId
                                             select prjct).ToList();
                    ///********* Added by Benet Shibin 08-09-2010 (purpose:Get CoPI & PI details ) **********/
                    //int ProjectId = Convert.ToInt32(query.ProjectId);
                    //var QryCo_PIDetails = (from C in context.tblProjectCoPI where C.ProjectId == ProjectId && C.Status == "Active" select new { C.Department, C.Name, C.Designation }).ToList();
                    //var Project = (from P in context.tblProject
                    //               where (P.ProjectId == ProjectId)
                    //               select P).FirstOrDefault();
                    ///********* End **********/
                    if (query != null)
                    {
                        editenhancement.ProjectID = Convert.ToInt32(query.ProjectId);
                        editenhancement.ProjectEnhancementID = query.ProjectEnhancementId;
                        editenhancement.DocumentReferenceNumber = query.DocumentReferenceNumber;
                        if (query.PresentDueDate != null)
                        {
                            editenhancement.PrsntDueDate = String.Format("{0:dd}", (DateTime)query.PresentDueDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.PresentDueDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.PresentDueDate);
                            editenhancement.PresentDueDate = (DateTime)query.PresentDueDate;
                            if (editenhancement.ExtendedDueDate != null)
                            {
                                editenhancement.ExtndDueDate = String.Format("{0:dd}", (DateTime)query.ExtendedDueDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ExtendedDueDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ExtendedDueDate);
                                editenhancement.ExtendedDueDate = (DateTime)query.ExtendedDueDate;
                            }
                        }

                        editenhancement.OldSanctionValue = query.OldSanctionValue;
                        editenhancement.EnhancedSanctionValue = query.EnhancedSanctionValue;
                        editenhancement.TotalEnhancedAllocationvalue = query.TotalEnhancedValue;
                        editenhancement.TotalAllocatedvalue = query.TotalAllocatedValue;
                        editenhancement.Enhancement_Qust_1 = (query.IsEnhancementonly ?? false) ? "Yes" : "No";
                        editenhancement.Extension_Qust_1 = (query.IsExtensiononly ?? false) ? "Yes" : "No";
                        editenhancement.AttachmentName = query.AttachmentName;

                        editenhancement.AttachmentPath = query.AttachmentPath;
                        /********* Edited by Benet Shibin 07-09-2010 **********/
                        //editenhancement.Department = Project.PIDepartment ?? 0;
                        //editenhancement.PIDesignation = Project.PIDesignation ?? 0;
                        //editenhancement.PI = Project.PIName ?? 0;
                        ////int[] Department;
                        //if (QryCo_PIDetails.Count > 0)
                        //{
                        //    int[] _CoPIDep = new int[QryCo_PIDetails.Count];
                        //    int[] _CoPI = new int[QryCo_PIDetails.Count];
                        //    int[] _CoPIDesig = new int[QryCo_PIDetails.Count];
                        //    for (int i = 0; i < QryCo_PIDetails.Count; i++)
                        //    {
                        //        _CoPIDep[i] = Convert.ToInt32(QryCo_PIDetails[i].Department);
                        //        _CoPI[i] = Convert.ToInt32(QryCo_PIDetails[i].Name);
                        //        _CoPIDesig[i] = Convert.ToInt32(QryCo_PIDetails[i].Designation);
                        //    }
                        //    editenhancement.CoPIDepartment = _CoPIDep;
                        //    editenhancement.CoPIid = _CoPI;
                        //    editenhancement.CoPIDesignation = _CoPIDesig;
                        //}
                        /********* End **********/
                        var projectid = editenhancement.ProjectID;
                        var projectquery = (from P in context.tblProject
                                            where (P.ProjectId == projectid)
                                            select P).FirstOrDefault();
                        editenhancement.ProjectNumber = projectquery.ProjectNumber;
                        editenhancement.Projecttitle = projectquery.ProjectTitle;
                        if (query.IsEnhancementWithExtension == true)
                        {
                            editenhancement.ExtendedDueDate = query.ExtendedDueDate;
                        }

                    }
                    if (allocationquery.Count > 0)
                    {
                        int[] _prjectid = new int[allocationquery.Count];
                        int[] _allocationid = new int[allocationquery.Count];
                        int[] _allocationhead = new int[allocationquery.Count];
                        Nullable<decimal>[] _oldallocationvalue = new Nullable<decimal>[allocationquery.Count];
                        Nullable<decimal>[] _enhancedallocationvalue = new Nullable<decimal>[allocationquery.Count];
                        Nullable<decimal>[] _totalallocationvalue = new Nullable<decimal>[allocationquery.Count];

                        for (int i = 0; i < allocationquery.Count; i++)
                        {
                            _prjectid[i] = Convert.ToInt32(allocationquery[i].ProjectId);
                            _allocationid[i] = projectallocquery[i].AllocationId;
                            _allocationhead[i] = Convert.ToInt32(allocationquery[i].AllocationHead);
                            _oldallocationvalue[i] = allocationquery[i].OldValue;
                            _enhancedallocationvalue[i] = allocationquery[i].EnhancedValue;
                            _totalallocationvalue[i] = allocationquery[i].TotalValue;
                        }
                        editenhancement.AllocationId = _allocationid;
                        editenhancement.Allocationhead = _allocationhead;
                        editenhancement.OldAllocationvalue = _oldallocationvalue;
                        editenhancement.EnhancedAllocationvalue = _enhancedallocationvalue;
                        editenhancement.HeadwiseTotalAllocationvalue = _totalallocationvalue;
                    }
                    return editenhancement;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<ProjectEnhancementModel> GetExtendedProjectList()
        {
            List<ProjectEnhancementModel> project = new List<ProjectEnhancementModel>();
            using (var context = new IOASDBEntities())


            {
                var query = (from enhance in context.tblProjectEnhancement
                             join P in context.tblProject on enhance.ProjectId equals P.ProjectId
                             orderby enhance.ProjectEnhancementId
                             where ((enhance.IsExtensiononly == true || enhance.IsEnhancementWithExtension == true) && enhance.IsExtensionOnlyCurrentversion == true)
                             select new { enhance, P.PIName, P.BaseValue, P.ProjectNumber, P.ProjectId, P.ProjectTitle, P.ActuaClosingDate }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        var userid = query[i].PIName;
                        var userquery = context.vwFacultyStaffDetails.FirstOrDefault(dup => dup.UserId == userid);
                        Nullable<decimal> presentsanctnval = 0;
                        Nullable<DateTime> oldduedate = DateTime.MinValue;
                        Nullable<DateTime> presentduedate = DateTime.MinValue;
                        var PrsntDueDate = "";

                        if (query[i].enhance.OldSanctionValue == 0 || query[i].enhance.OldSanctionValue == null)
                        {
                            presentsanctnval = query[i].BaseValue;

                        }
                        else
                        {
                            presentsanctnval = Convert.ToDecimal(query[i].enhance.OldSanctionValue);
                        }
                        if (query[i].enhance.PresentDueDate == null)
                        {
                            oldduedate = query[i].ActuaClosingDate;
                        }
                        else
                        {
                            oldduedate = query[i].enhance.PresentDueDate;
                        }
                        if (query[i].enhance.ExtendedDueDate == null)
                        {
                            PrsntDueDate = String.Format("{0:dd}", (DateTime)query[i].ActuaClosingDate) + "-" + String.Format("{0:MMMM}", (DateTime)query[i].ActuaClosingDate) + "-" + String.Format("{0:yyyy}", (DateTime)query[i].ActuaClosingDate);
                            presentduedate = query[i].ActuaClosingDate;
                        }
                        else
                        {
                            PrsntDueDate = String.Format("{0:dd}", (DateTime)query[i].enhance.ExtendedDueDate) + "-" + String.Format("{0:MMMM}", (DateTime)query[i].enhance.ExtendedDueDate) + "-" + String.Format("{0:yyyy}", (DateTime)query[i].enhance.ExtendedDueDate);
                            presentduedate = query[i].enhance.ExtendedDueDate;
                        }
                        project.Add(new ProjectEnhancementModel()
                        {
                            Sno = i + 1,
                            ProjectID = query[i].ProjectId,
                            ProjectEnhancementID = query[i].enhance.ProjectEnhancementId,
                            Projecttitle = query[i].ProjectTitle,
                            ProjectNumber = query[i].ProjectNumber,
                            PIname = userquery.FirstName,
                            PresentDueDate = oldduedate,
                            EnhancedSanctionValue = presentsanctnval,
                            ExtndDueDate = PrsntDueDate,
                            PrsntDueDate = String.Format("{0:s}", presentduedate)
                        });
                    }
                }
            }
            return project;
        }
        public static ProjectEnhancementModel getprojectdetailsforextension(int projectid)
        {
            try
            {

                ProjectEnhancementModel Projectdetails = new ProjectEnhancementModel();

                using (var context = new IOASDBEntities())
                {

                    var query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectid);
                    var selectprojectid = query.ProjectId;
                    var prjctenhancequery = (from C in context.tblProjectEnhancement
                                             where (C.ProjectId == selectprojectid && (C.IsExtensiononly == true || C.IsEnhancementWithExtension == true) && C.IsCurrentVersion == true)
                                             select C).FirstOrDefault();
                    var pidetailsquery = (from c in context.vwFacultyStaffDetails
                                          where c.UserId == query.PIName
                                          select c).FirstOrDefault();

                    if (query != null)
                    {

                        Projectdetails.ProjectID = query.ProjectId;
                        Projectdetails.ProjectNumber = query.ProjectNumber;
                        Projectdetails.Projecttitle = query.ProjectTitle;

                        if (prjctenhancequery != null)
                        {
                            if (prjctenhancequery.ExtendedDueDate != null)
                            {
                                Projectdetails.PrsntDueDate = String.Format("{0:dd}", (DateTime)prjctenhancequery.ExtendedDueDate) + "-" + String.Format("{0:MMMM}", (DateTime)prjctenhancequery.ExtendedDueDate) + "-" + String.Format("{0:yyyy}", (DateTime)prjctenhancequery.ExtendedDueDate);
                                Projectdetails.PresentDueDate = prjctenhancequery.ExtendedDueDate;
                            }
                            else
                            {
                                Projectdetails.PrsntDueDate = String.Format("{0:dd}", (DateTime)query.ActuaClosingDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ActuaClosingDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ActuaClosingDate);
                                Projectdetails.PresentDueDate = query.ActuaClosingDate;
                            }
                            Projectdetails.OldSanctionValue = prjctenhancequery.TotalAllocatedValue;
                        }
                        else
                        {
                            Projectdetails.PrsntDueDate = String.Format("{0:dd}", (DateTime)query.ActuaClosingDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ActuaClosingDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ActuaClosingDate);
                            Projectdetails.PresentDueDate = query.ActuaClosingDate;
                            Projectdetails.OldSanctionValue = query.BaseValue;
                        }


                    }


                }

                return Projectdetails;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public int ProjectExtension(ProjectEnhancementModel model)
        //{
        //    try
        //    {
        //        using (var context = new IOASDBEntities())
        //        {
        //            var projectid = 0;
        //            tblProjectEnhancement extend = new tblProjectEnhancement();
        //            var chkproject = context.tblProject.FirstOrDefault(dup => dup.ProjectId == model.ProjectID);

        //            var userquery = context.vwFacultyStaffDetails.FirstOrDefault(m => m.UserId == chkproject.PIName);
        //            var extendquery = context.tblProjectEnhancement.FirstOrDefault(m => m.ProjectEnhancementId == model.ProjectEnhancementID && m.IsCurrentVersion == true);
        //            var chkextendproject = (from C in context.tblProjectEnhancement
        //                                    where (C.ProjectId == model.ProjectID && (C.IsExtensiononly == true || C.IsEnhancementWithExtension == true) && C.IsExtensionOnlyCurrentversion == true)
        //                                    select C).ToList();

        //            if (extendquery == null)
        //            {
        //                if (model.AttachmentName != null)
        //                {
        //                    string filepath = " ";
        //                    filepath = System.IO.Path.GetFileName(model.file.FileName);
        //                    var fileId = Guid.NewGuid().ToString();
        //                    var docname = fileId + "_" + filepath;

        //                    /*Saving the file in server folder*/
        //                    model.file.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));
        //                    extend.AttachmentPath = docname;
        //                }

        //                extend.ProjectId = model.ProjectID;
        //                extend.DocumentReferenceNumber = model.DocumentReferenceNumber;
        //                extend.OldSanctionValue = model.OldSanctionValue;
        //                extend.Status = "Active";
        //                extend.PresentDueDate = model.PresentDueDate;
        //                extend.ExtendedDueDate = model.ExtendedDueDate;
        //                extend.CrtdUserId = model.CrtdUserid;
        //                extend.CrtsTS = DateTime.Now;
        //                extend.AttachmentName = model.AttachmentName;
        //                if (extend.ExtendedDueDate != null && extend.EnhancedSanctionValue == null)
        //                {
        //                    extend.IsEnhancementonly = false;
        //                    extend.IsEnhancementWithExtension = false;
        //                    extend.IsExtensiononly = true;
        //                }
        //                else if (extend.ExtendedDueDate == null)
        //                {
        //                    extend.IsEnhancementonly = false;
        //                    extend.IsEnhancementWithExtension = false;
        //                    extend.IsExtensiononly = false;
        //                }
        //                extend.IsExtensionOnlyCurrentversion = true;
        //                context.tblProjectEnhancement.Add(extend);
        //                context.SaveChanges();
        //                int prjctid = Convert.ToInt32(extend.ProjectId);
        //                projectid = prjctid;
        //                if (chkextendproject != null)
        //                {
        //                    for (int i = 0; i < chkextendproject.Count(); i++)
        //                    {
        //                        chkextendproject[i].IsExtensionOnlyCurrentversion = false;
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                if (model.AttachmentName != null)
        //                {
        //                    string filepath = " ";
        //                    filepath = System.IO.Path.GetFileName(model.file.FileName);
        //                    var fileId = Guid.NewGuid().ToString();
        //                    var docname = fileId + "_" + filepath;

        //                    /*Saving the file in server folder*/
        //                    model.file.SaveAs(HttpContext.Current.Server.MapPath("~/Content/SupportDocuments/" + docname));
        //                    extendquery.AttachmentPath = docname;
        //                }

        //                extendquery.ProjectId = model.ProjectID;
        //                extendquery.DocumentReferenceNumber = model.DocumentReferenceNumber;
        //                extendquery.OldSanctionValue = model.OldSanctionValue;
        //                //extendquery.Status = chkproject.ProjectStatus;
        //                extendquery.PresentDueDate = model.PresentDueDate;
        //                extendquery.ExtendedDueDate = model.ExtendedDueDate;
        //                extendquery.CrtdUserId = model.CrtdUserid;
        //                extendquery.CrtsTS = DateTime.Now;
        //                extendquery.AttachmentName = model.AttachmentName;
        //                if (extendquery.ExtendedDueDate != null && extendquery.EnhancedSanctionValue == null)
        //                {
        //                    extendquery.IsEnhancementonly = false;
        //                    extendquery.IsEnhancementWithExtension = false;
        //                    extendquery.IsExtensiononly = true;
        //                }
        //                else if (extendquery.ExtendedDueDate == null)
        //                {
        //                    extendquery.IsEnhancementonly = false;
        //                    extendquery.IsEnhancementWithExtension = false;
        //                    extendquery.IsExtensiononly = false;
        //                }
        //                extendquery.IsCurrentVersion = true;
        //                context.SaveChanges();
        //                int prjctid = Convert.ToInt32(extendquery.ProjectId);
        //                projectid = prjctid;
        //            }
        //            return projectid;
        //        }
        //    }

        //    catch (Exception ex)
        //    {

        //        return -1;
        //    }
        //}

        public static ProjectEnhancementModel EditExtension(int EnhanceId)
        {
            try
            {
                ProjectEnhancementModel editextension = new ProjectEnhancementModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProjectEnhancement
                                 where (P.ProjectEnhancementId == EnhanceId)
                                 select P).FirstOrDefault();


                    if (query != null)
                    {
                        editextension.ProjectID = Convert.ToInt32(query.ProjectId);
                        editextension.ProjectEnhancementID = query.ProjectEnhancementId;
                        editextension.DocumentReferenceNumber = query.DocumentReferenceNumber;
                        if (query.PresentDueDate != null && query.ExtendedDueDate != null)
                        {
                            editextension.PrsntDueDate = String.Format("{0:dd}", (DateTime)query.PresentDueDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.PresentDueDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.PresentDueDate);
                            editextension.PresentDueDate = (DateTime)query.PresentDueDate;
                            editextension.ExtndDueDate = String.Format("{0:dd}", (DateTime)query.ExtendedDueDate) + "-" + String.Format("{0:MMMM}", (DateTime)query.ExtendedDueDate) + "-" + String.Format("{0:yyyy}", (DateTime)query.ExtendedDueDate);
                            editextension.ExtendedDueDate = (DateTime)query.ExtendedDueDate;
                        }

                        editextension.OldSanctionValue = query.OldSanctionValue;
                        editextension.AttachmentName = query.AttachmentName;
                        editextension.AttachmentPath = query.AttachmentPath;
                        var projectid = editextension.ProjectID;
                        var projectquery = (from P in context.tblProject
                                            where (P.ProjectId == projectid)
                                            select P).FirstOrDefault();
                        editextension.ProjectNumber = projectquery.ProjectNumber;
                        editextension.Projecttitle = projectquery.ProjectTitle;

                    }
                    return editextension;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int CloseProject(ProjectClosingModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var projectid = 0;
                    tblProject close = new tblProject();
                    var chkproject = context.tblProject.FirstOrDefault(dup => dup.ProjectId == model.ProjectID);

                    if (chkproject != null)
                    {

                        chkproject.Status = "InActive";
                        chkproject.UpdatedUserId = model.UpdtUserid;
                        chkproject.UpdatedTS = model.Updt_TS;

                        context.SaveChanges();
                        int prjctid = Convert.ToInt32(chkproject.ProjectId);
                        projectid = prjctid;
                    }


                    return projectid;
                }
            }

            catch (Exception ex)
            {

                return -1;
            }
        }

        public static List<CreateProjectModel> GetClosedProjectList()
        {
            List<CreateProjectModel> project = new List<CreateProjectModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from P in context.tblProject
                             join pi in context.vwFacultyStaffDetails on P.PIName equals pi.UserId
                             //join user in context.tblUser on P.PIName equals user.UserId
                             //join dept in context.tblPIDepartmentMaster on P.PIDepartment equals dept.DepartmentId
                             join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId
                             orderby P.ProjectId
                             select new { P, pi.FirstName, pi.EmployeeId, pi.DepartmentName, agency.AgencyName }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        project.Add(new CreateProjectModel()
                        {
                            Sno = i + 1,
                            ProjectID = query[i].P.ProjectId,
                            Projecttitle = query[i].P.ProjectTitle,
                            ProjectNumber = query[i].P.ProjectNumber,
                            Budget = query[i].P.BaseValue,
                            SponsoringAgency = query[i].P.SponsoringAgency,
                            SponsoringAgencyName = query[i].AgencyName,
                            NameofPI = query[i].FirstName,
                            PIDepartmentName = query[i].DepartmentName,
                            EmpCode = query[i].EmployeeId
                        });
                    }
                }
            }
            return project;
        }

        public static List<ListProjectDetails> GetProjectDetails()
        {
            List<ListProjectDetails> GetDetail = new List<ListProjectDetails>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblProject
                                 //where C.IsInward == true
                             orderby C.ProjectId descending
                             select C).ToList();

                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {

                        GetDetail.Add(new ListProjectDetails()
                        {
                            slNo = i + 1,
                            ProjectID = query[i].ProjectId,
                            ProjectNo = query[i].ProjectNumber,
                            ProjectTittle = query[i].ProjectTitle,
                            SanctionOrderNo = query[i].SanctionOrderNumber,
                            PIName = Common.GetPIName(query[i].PIName ?? 0),
                            Status = query[i].Status,
                            //StatusId = Convert.ToInt32(query[i].ProjectStatus),
                        });

                    }
                }
            }
            return GetDetail;
        }

        public static List<MasterlistviewModel> LoadControls()
        {

            List<MasterlistviewModel> Status = new List<MasterlistviewModel>();
            using (var context = new IOASDBEntities())
            {
                var QryStatus = (from F in context.tblCodeControl
                                 where F.CodeName == "ProjectStatus"
                                 select F).ToList();
                if (QryStatus.Count > 0)
                {
                    for (int i = 0; i < QryStatus.Count; i++)
                    {
                        Status.Add(new MasterlistviewModel()
                        {
                            id = QryStatus[i].CodeValAbbr,
                            name = QryStatus[i].CodeValDetail
                        });
                    }

                }
            }

            return Status;
        }

        public static int UpdateProjectDetails(UpdateProjectStatusModel model, int UserId, string DocName)
        {
            try
            {
                int Result = 0;
                using (var context = new IOASDBEntities())
                {
                    //update first Statuslog existing 

                    var StatusQuery = (from D in context.tblProjectStatusLog where D.ProjectId == model.ProjectID orderby D.ProjectStatusLogId descending select D).FirstOrDefault();
                    if (StatusQuery != null)
                    {
                        StatusQuery.IsCurrentStatus = false;
                        context.SaveChanges();
                    }

                    //Update tblProject table status
                    var UpQuery = (from C in context.tblProject where C.ProjectId == model.ProjectID select C).FirstOrDefault();
                    if (UpQuery != null)
                    {
                        // Insert new Status log table
                        tblProjectStatusLog prjStatus = new tblProjectStatusLog();
                        prjStatus.ProjectId = model.ProjectID;
                        prjStatus.FromStatus = UpQuery.Status;
                        prjStatus.ToStatus = model.StatusID;
                        prjStatus.Remarks = model.Remarks;
                        prjStatus.DocumentName = DocName;
                        prjStatus.UpdtdTS = DateTime.Now;
                        prjStatus.UpdtdUserId = UserId;
                        prjStatus.IsCurrentStatus = true;
                        context.tblProjectStatusLog.Add(prjStatus);
                        context.SaveChanges();
                        //Update tblProject status
                        UpQuery.Status = model.StatusID;
                        UpQuery.UpdatedTS = DateTime.Now;
                        UpQuery.UpdatedUserId = UserId;
                        context.SaveChanges();
                        Result = 1;
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        //public PagedData<ProjectSearchResultModel> GetProjectList(ProjectSearchFieldModel model, int page, int pageSize)
        //{
        //    try
        //    {
        //        List<ProjectSearchResultModel> list = new List<ProjectSearchResultModel>();
        //        var searchData = new PagedData<ProjectSearchResultModel>();
        //        int skiprec = 0;
        //        if (page == 1)
        //        {
        //            skiprec = 0;
        //        }
        //        else
        //        {
        //            skiprec = (page - 1) * pageSize;
        //        }
        //        using (var context = new IOASDBEntities())
        //        {
        //            var predicate = PredicateBuilder.BaseAnd<tblSRB>();
        //            if (!string.IsNullOrEmpty(model.ItemName))
        //                predicate = predicate.And(d => d.ItemName.Contains(model.ItemName));
        //            if (model.ItemCategory != null)
        //                predicate = predicate.And(d => d.ItemCategoryId == model.ItemCategory);
        //            if (model.PIName != null)
        //                predicate = predicate.And(d => d.PIId == model.PIName);
        //            if (model.FromPODate != null && model.ToPODate != null)
        //                predicate = predicate.And(d => d.PurchaseDate >= model.FromPODate && d.PurchaseDate <= model.ToPODate);
        //            if (model.FromSRBDate != null && model.ToSRBDate != null)
        //                predicate = predicate.And(d => d.InwardDate >= model.FromSRBDate && d.InwardDate <= model.ToSRBDate);
        //            var query = context.tblSRB.Where(predicate).OrderByDescending(m => m.SRBId).Skip(skiprec).Take(pageSize).ToList();
        //            if (query.Count > 0)
        //            {
        //                for (int i = 0; i < query.Count; i++)
        //                {
        //                    var doc = query[i].PODocs.Split(new char[] { '_' }, 2);
        //                    list.Add(new SRBSearchResultModel()
        //                    {
        //                        DocFullName = query[i].PODocs,
        //                        DocName = doc[1],
        //                        InwardDate = String.Format("{0:ddd dd-MMM-yyyy}", query[i].InwardDate),
        //                        ItemCategory = query[i].ItemCategoryId,
        //                        ItemName = query[i].ItemName,
        //                        PONumber = query[i].PONumber,
        //                        SRBId = query[i].SRBId
        //                    });

        //                }
        //            }
        //            var records = context.tblSRB.Where(predicate).OrderByDescending(m => m.SRBId).Count();
        //            searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)records / pageSize));
        //            searchData.Data = list;
        //            searchData.pageSize = pageSize;
        //            searchData.visiblePages = 10;
        //            searchData.CurrentPage = page;
        //        }

        //        return searchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public ProjectSummaryModel getProjectSummary(int ProjectId)
        {
            try
            {
                ProjectSummaryModel prjModel = new ProjectSummaryModel();
                using (var context = new IOASDBEntities())
                {
                    var qryProject = (from prj in context.tblProject
                                      where prj.ProjectId == ProjectId
                                      select prj).FirstOrDefault();
                    var qryPreviousCommit = (from C in context.tblCommitment
                                             join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                             where C.ProjectId == ProjectId && C.Status == "Active"
                                             select new { D.BalanceAmount, D.ReversedAmount }).ToList();
                    var BalanceAmt = qryPreviousCommit.Select(m => m.BalanceAmount).Sum();
                    var ReversedAmount = qryPreviousCommit.Select(m => m.ReversedAmount).Sum();

                    /*Spent amount calculation Start*/

                    decimal? Debit = 0, Credit = 0, spentAmt = 0;
                    var qrySpenAmt = (from C in context.vwCommitmentSpentBalance where C.ProjectId == ProjectId select C.AmountSpent).Sum();
                    if (qrySpenAmt == null)
                        qrySpenAmt = 0;
                    spentAmt = qrySpenAmt;
                    var FundTransferDebit = (from C in context.tblProjectTransfer
                                             from D in context.tblProjectTransferDetails
                                             where C.ProjectTransferId == D.ProjectTransferId
                                             where C.DebitProjectId == ProjectId
                                             select D).ToList();
                    if (FundTransferDebit.Count > 0)
                    {
                        Debit = FundTransferDebit.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum();
                        if (Debit != 0)
                            spentAmt = spentAmt + Debit;
                    }
                    var FundTransferCredit = (from C in context.tblProjectTransfer
                                              from D in context.tblProjectTransferDetails
                                              where C.ProjectTransferId == D.ProjectTransferId
                                              where C.CreditProjectId == ProjectId
                                              select D).ToList();
                    if (FundTransferCredit.Count > 0)
                    {
                        Credit = FundTransferCredit.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum();
                        if (Credit != 0)
                            spentAmt = spentAmt - Credit;
                    }
                    /*claim amount institute start*/
                    var qryInstituteClaim = (from Neg in context.tblInstituteClaims
                                             where Neg.ProjectId == ProjectId && Neg.Status == "Completed" 
                                             select Neg).ToList();
                    var claimAmt = qryInstituteClaim.Select(m => m.ClaimAmount).Sum();
                    spentAmt = spentAmt - claimAmt;
                    /*claim amount institute end*/
                    /*Spent amount calculation End*/
                    var AvailableCommitment = BalanceAmt + ReversedAmount;
                    var QryReceipt = (from C in context.tblReceipt where C.ProjectId == ProjectId select C).ToList();
                    decimal receiptAmt = QryReceipt.Select(m => m.ReceiptAmount).Sum() ?? 0;
                    decimal OverHead = QryReceipt.Select(m => m.ReceiptOverheadValue).Sum() ?? 0;
                    decimal CGST = QryReceipt.Select(m => m.CGST).Sum() ?? 0;
                    decimal SGST = QryReceipt.Select(m => m.SGST).Sum() ?? 0;
                    decimal IGST = QryReceipt.Select(m => m.IGST).Sum() ?? 0;
                    decimal GST = CGST + SGST + IGST;
                    receiptAmt = receiptAmt - (GST + OverHead);
                    /* Negative balance taking query*/
                    var qryNegativeBal = (from Neg in context.tblNegativeBalance
                                          where Neg.ProjectId == ProjectId && Neg.Status == "Approved"
                                          select Neg.NegativeBalanceAmount).Sum();
                    /* Negative balance taking query end*/
                    /* Opening balance taking query*/
                    decimal qryOpeningBal = (from OB in context.tblProjectOB
                                             where OB.ProjectId == ProjectId
                                             select OB.OpeningBalance).Sum() ?? 0;
                    /* Opening balance taking query end*/
                    if (qryProject != null)
                    {
                        prjModel.ProjectTittle = qryProject.ProjectTitle;
                        prjModel.PIname = Common.GetPIName(qryProject.PIName ?? 0);
                        prjModel.SanctionedValue = qryProject.SanctionValue ?? 0;
                        prjModel.OpeningBalance = qryOpeningBal;
                        //sum(ReciptAmt-(GST+OverHeads ))
                        prjModel.TotalReceipt = receiptAmt;// + qryOpeningBal;
                        prjModel.AmountSpent = spentAmt ?? 0;
                        prjModel.PreviousCommitment = AvailableCommitment ?? 0;
                        //TotalReceipt - AmountSpent + PreviousCommitment
                        prjModel.AvailableBalance = ((qryOpeningBal + prjModel.TotalReceipt) - (prjModel.AmountSpent + prjModel.PreviousCommitment));
                        prjModel.FinancialYear = Common.GetFinYear(qryProject.FinancialYear ?? 0);
                        prjModel.SanctionOrderNo = qryProject.SanctionOrderNumber;
                        prjModel.SanctionOrderDate = String.Format("{0:ddd dd-MMM-yyyy}", qryProject.SanctionOrderDate);
                        prjModel.ProjectApprovalDate = string.Format("{0:ddd dd-MMM-yyyy}", qryProject.ProposalApprovedDate);
                        prjModel.ProjectDuration = prjModel.ProjectDuration;
                        prjModel.ProposalNo = qryProject.ProposalNumber;
                        prjModel.ProjectNo = qryProject.ProjectNumber;
                        prjModel.BaseValue = qryProject.BaseValue ?? 0;
                        prjModel.ProjectType = Common.getprojectTypeName(qryProject.ProjectType ?? 0);
                        prjModel.ApplicableTax = qryProject.ApplicableTax ?? 0;
                        //var Data= Common.getProjectNo(qryProject.ProjectType ?? 0);
                        //prjModel.CommitNo = Data.Item2;
                        prjModel.ApprovedNegativeBalance = qryNegativeBal ?? 0;
                        prjModel.NetBalance = (prjModel.AvailableBalance + prjModel.ApprovedNegativeBalance);
                        prjModel.OverHeads = OverHead;
                        prjModel.AllocationNR_f = qryProject.ProjectClassification != 1 ? true : qryProject.AllocationNR_f ?? false;
                        //sum(CGST+SGST+IGST) 
                        prjModel.GST = GST;
                    }

                    //taking total commitment amount headwise
                    var qryHeadCommit = (from C in context.tblCommitment
                                         join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId
                                         where C.ProjectId == ProjectId && C.Status == "Active"
                                         select new { D.AllocationHeadId, D.Amount }).ToList();
                    List<HeadWiseDetailModel> List = new List<HeadWiseDetailModel>();
                    List<HeadWiseDetailModel> ListSpent = new List<HeadWiseDetailModel>();
                    List<HeadWiseDetailModel> Balance = new List<HeadWiseDetailModel>();
                    if (qryHeadCommit.Count > 0)
                    {
                        var distCount = qryHeadCommit.Select(m => m.AllocationHeadId).Distinct().ToArray();
                        for (int i = 0; i < distCount.Length; i++)
                        {
                            int headId = distCount[i] ?? 0;
                            var HeadName = Common.getAllocationHeadName(headId);
                            decimal amt = qryHeadCommit.Where(m => m.AllocationHeadId == headId).Sum(m => m.Amount) ?? 0;
                            List.Add(new HeadWiseDetailModel()
                            {
                                AllocationId = headId,
                                AllocationHeadName = HeadName,
                                Amount = amt
                            });
                        }

                    }

                    //taking total spent amount headwise
                    var qrySpent = (from C in context.vwCommitmentSpentBalance
                                    where C.ProjectId == ProjectId
                                    select new { C.AllocationHeadId, C.AmountSpent }).ToList();

                    if (qrySpent.Count > 0)
                    {
                        var distCount = qrySpent.Select(m => m.AllocationHeadId).Distinct().ToArray();
                        for (int i = 0; i < distCount.Length; i++)
                        {
                            int headId = distCount[i] ?? 0;
                            var HeadName = Common.getAllocationHeadName(headId);
                            decimal amt = qrySpent.Where(m => m.AllocationHeadId == headId).Sum(m => m.AmountSpent) ?? 0;
                            ListSpent.Add(new HeadWiseDetailModel()
                            {
                                AllocationId = headId,
                                AllocationHeadName = HeadName,
                                Amount = amt
                            });
                        }

                    }
                    //taking balance for future commitments
                    var qryAllocation = (from C in context.tblProjectAllocation
                                         where C.ProjectId == ProjectId
                                         select new { C.AllocationHead, C.AllocationValue }
                    ).ToList();

                    //var qryAllocation

                    if (qryAllocation.Count > 0)
                    {
                        decimal balance = 0;
                        for (int j = List.Count() - 1; j < qryAllocation.Count(); j++)
                        {
                            if (j + 1 == List.Count())
                            {
                                for (int k = 0; k < List.Count(); k++)
                                {
                                    decimal amt = qryAllocation[k].AllocationValue ?? 0;
                                    balance = amt - List[k].Amount;
                                    //if (qryInstituteClaim[k].ClaimAmount != null)
                                    //{
                                    //    balance = balance - qryInstituteClaim[k].ClaimAmount ?? 0;
                                    //}
                                    Balance.Add(new HeadWiseDetailModel()
                                    {
                                        AllocationId = qryAllocation[k].AllocationHead ?? 0,
                                        AllocationHeadName = List[k].AllocationHeadName,
                                        Amount = balance
                                    });
                                }
                            }
                            else
                            {
                                var HeadId = qryAllocation[j].AllocationHead ?? 0;
                                var HeadName = Common.getAllocationHeadName(HeadId);
                                Balance.Add(new HeadWiseDetailModel()
                                {
                                    AllocationId = HeadId,
                                    AllocationHeadName = HeadName,
                                    Amount = qryAllocation[j].AllocationValue ?? 0
                                });
                            }
                        }
                    }
                    prjModel.HeadWiseCommitment = List;
                    prjModel.HeadWiseSpent = ListSpent;
                    prjModel.HeadWiseAllocation = Balance;
                }
                return prjModel;
            }
            catch (Exception ex)
            {
                ProjectSummaryModel prjModel = new ProjectSummaryModel();
                return prjModel;
            }
        }
        public ProjectViewDetailsModel getProjectViewDetails(int ProjectId)
        {
            try
            {
                ProjectViewDetailsModel prjModel = new ProjectViewDetailsModel();
                List<CoPiDetailsModel> listCoPI = new List<CoPiDetailsModel>();
                List<CoPiDetailsForOtherInstituteModel> CoPIOther = new List<CoPiDetailsForOtherInstituteModel>();
                List<AllocationDetailModel> listAllocation = new List<AllocationDetailModel>();
                List<InstalmentModel> ListInstalment = new List<InstalmentModel>();
                List<StaffDetailsModel> listStaffDetails = new List<StaffDetailsModel>();
                List<OtherCompanyStaffModel> listOtherStaff = new List<OtherCompanyStaffModel>();
                List<DocumentDetailsModel> listDocument = new List<DocumentDetailsModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 where (P.ProjectId == ProjectId)
                                 select P).FirstOrDefault();
                    var CoPIquery = (from CoPI in context.tblProjectCoPI
                                     where CoPI.ProjectId == ProjectId && CoPI.Status == "Active"
                                     select CoPI).ToList();
                    var OtherInstituteCoPIquery = (from CoPI in context.tblProjectOtherInstituteCoPI
                                                   where CoPI.ProjectId == ProjectId && CoPI.Status == "Active"
                                                   select CoPI).ToList();
                    var prjctsubtypequery = (from C in context.tblCodeControl
                                             where (C.CodeName == "ConsultancyProjectSubtype" && C.CodeValAbbr == query.ConsProjectSubType)
                                             select C).FirstOrDefault();
                    var schemenamequery = (from c in context.tblSchemes
                                           where c.SchemeId == query.ConsultancyFundingCategory
                                           select c).FirstOrDefault();
                    var SupportDocquery = (from Doc in context.tblSupportDocuments
                                           where Doc.ProjectId == ProjectId && Doc.IsCurrentVersion == true
                                           select Doc).ToList();
                    var allocationquery = (from alloc in context.tblProjectAllocation
                                           where (alloc.ProjectId == ProjectId)
                                           select alloc).ToList();
                    var emiQuery = (from emi in context.tblInstallment
                                    where (emi.ProjectId == ProjectId)
                                    select emi).ToList();
                    var prjctstaffquery = (from staff in context.tblProjectStaffCategorywiseBreakup
                                           where (staff.ProjectId == ProjectId)
                                           select staff).ToList();
                    var OtherCompanyStaff = (from OtherStaff in context.tblJointDevelopmentCompany
                                             where OtherStaff.ProjectId == ProjectId
                                             select OtherStaff).ToList();
                    var ProjectTypeName = (from C in context.tblCodeControl
                                           where C.CodeName == "Projecttype" && C.CodeValAbbr == query.ProjectType
                                           select C).FirstOrDefault();
                    var Agencyquery = (from C in context.tblAgencyMaster
                                       where C.AgencyId == query.SponsoringAgency
                                       select C).FirstOrDefault();
                    if (query != null)
                    {

                        prjModel.ProjectTypeName = Common.getProjecTypeName(query.ProjectType ?? 0);
                        prjModel.TypeOfProject = Common.getTypeofPrjoectName(query.TypeOfProject ?? 0);
                        prjModel.CoardinatorName = query.Collaborativeprojectcoordinator;
                        prjModel.InstituteOrIndustryName = query.CollaborativeprojectAgency;
                        prjModel.Email = query.Collaborativeprojectcoordinatoremail;
                        prjModel.OtherInstituteOrIndustryShare = query.Collaborativeprojecttotalcost ?? 0;
                        prjModel.IITMShare = query.CollaborativeprojectIITMCost ?? 0;
                        prjModel.InstituteOrIndustryName = query.CollaborativeprojectAgency;
                        prjModel.PrjAccountType = Common.getProjectSubTypeName(query.ProjectSubType ?? 0);
                        prjModel.Sheme = Common.getSchemeName(query.SchemeName ?? 0);
                        prjModel.ShemeCode = Common.getSchemeCodeName(query.SchemeCode ?? 0);
                        prjModel.prjFundingType = Common.getPrjoectFundingTypeName(query.FundingType ?? 0);
                        if (prjModel.ProjectTypeName == "Sponsored")
                        {
                            prjModel.prjCatagory = Common.getSponseredPrjCatagoryName(Convert.ToInt32(query.SponProjectCategory));
                        }
                        else
                        {
                            prjModel.prjCatagory = Common.getConsultancyPrjCatagoryName(Convert.ToInt32(query.ProjectCategory));
                        }
                        prjModel.FundedBy = Common.getPrjoectFundedByName(query.IndianFundedBy ?? 0);

                        /*Investigator Details*/
                        prjModel.PIDepartment = Common.getStaffDepartName(query.PIDepartment);
                        prjModel.PIName = Common.GetPIName(query.PIName ?? 0);
                        prjModel.PIEmail = Common.GetPIEmail(query.PIName ?? 0);
                        prjModel.ScientistName = query.ScientistName;
                        prjModel.ScientistEmai = query.ScientistEmail;
                        prjModel.ScientistMobile = query.ScientistMobile;
                        prjModel.ScientistAddress = query.ScientistAddress;
                        prjModel.JointdevelopmentQuestion = query.JointdevelopmentQuestion;
                        prjModel.Agency = Common.getagencycode(query.SponsoringAgency ?? 0);
                        prjModel.AgencyCode = Agencyquery.AgencyCode;
                        prjModel.AgencyRegName = query.AgencyRegisteredName;
                        prjModel.AgencyRegAddr = query.SponsoringAgencySOAddress;
                        prjModel.ContactPerson = query.SponsoringAgencyContactPersonMobile;
                        prjModel.Designation = query.SponsoringAgencyContactPersonDesignation;
                        prjModel.AgencyEmail = query.SponsoringAgencyContactPersonEmail;
                        prjModel.TentativeStartDate = String.Format("{0:ddd dd-MMM-yyyy}", query.TentativeStartDate);
                        prjModel.TentativeCloseDate = String.Format("{0:ddd dd-MMM-yyyy}", query.TentativeCloseDate);
                        prjModel.TentativeDueDate = String.Format("{0:ddd dd-MMM-yyyy}", query.ActuaClosingDate);
                        prjModel.TaxStatus = Common.getTaxStatusById(query.TaxStatus ?? 0);
                        prjModel.GSTIn = query.GSTIN;
                        prjModel.TAN = query.TAN;
                        prjModel.PAN = query.PAN;
                        prjModel.Remarks = query.Remarks;
                        if (query.IsYearWiseAllocation == true)
                        {
                            prjModel.YearWiseAllocation = "Yes";
                        }
                        else
                        {
                            prjModel.YearWiseAllocation = "No";
                        }

                        if (CoPIquery.Count > 0)
                        {
                            for (int i = 0; i < CoPIquery.Count; i++)
                            {
                                listCoPI.Add(new CoPiDetailsModel()
                                {
                                    PIDepartment = CoPIquery[i].Department,
                                    PIName = Common.GetPIName(CoPIquery[i].Name ?? 0),
                                    Email = CoPIquery[i].Email
                                });
                            }

                        }
                        if (OtherInstituteCoPIquery.Count > 0)
                        {
                            for (int i = 0; i < OtherInstituteCoPIquery.Count; i++)
                            {
                                CoPIOther.Add(new CoPiDetailsForOtherInstituteModel()
                                {
                                    Institute = Common.getStaffDepartName(OtherInstituteCoPIquery[i].Institution),
                                    Department = Common.GetDepartmentName(Convert.ToInt32(OtherInstituteCoPIquery[i].Department)),
                                    Remarks = OtherInstituteCoPIquery[i].Remarks
                                });
                            }
                        }
                        /*Investigator Details End*/
                        prjModel.prjSummary = getProjectSummary(ProjectId);


                        if (query.IsYearWiseAllocation == true)
                        {
                            var year = allocationquery.Select(m => m.Year).Distinct().ToArray();
                            //if(year.Length>)


                        }
                        else
                        {
                            if (allocationquery.Count > 0)
                            {
                                for (int i = 0; i < allocationquery.Count; i++)
                                {
                                    listAllocation.Add(new AllocationDetailModel()
                                    {
                                        AllocationHead = Common.getAllocationHeadName(allocationquery[i].AllocationHead ?? 0),
                                        AllocationType = Common.getAllocationType(allocationquery[i].AllocationHead ?? 0),
                                        AllocationValue = allocationquery[i].AllocationValue ?? 0
                                    });
                                }
                            }
                            if (emiQuery.Count > 0)
                            {
                                for (int i = 0; i < emiQuery.Count; i++)
                                {
                                    ListInstalment.Add(new InstalmentModel()
                                    {
                                        NoOfInstalment = emiQuery[i].NoOfInstallment ?? 0,
                                        InstalmentNo = emiQuery[i].InstallmentNo ?? 0,
                                        InstalmentAmount = emiQuery[i].InstallmentValue ?? 0
                                    });
                                }
                            }
                        }


                        if (prjctstaffquery.Count > 0)
                        {
                            for (int i = 0; i < prjctstaffquery.Count; i++)
                            {
                                listStaffDetails.Add(new StaffDetailsModel()
                                {
                                    Catagory = Common.getStaffCategoryName(prjctstaffquery[i].ProjectStaffCategory ?? 0),
                                    NoofStaffs = prjctstaffquery[i].NoofStaffs ?? 0,
                                    Salary = prjctstaffquery[i].SalaryofStaffs ?? 0
                                });
                            }
                        }
                        if (OtherCompanyStaff.Count > 0)
                        {
                            for (int i = 0; i < OtherCompanyStaff.Count; i++)
                            {
                                listOtherStaff.Add(new OtherCompanyStaffModel()
                                {
                                    CompanyName = OtherCompanyStaff[i].JointDevelopCompanyName,
                                    Remarks = OtherCompanyStaff[i].Remarks
                                });
                            }
                        }
                        if (SupportDocquery.Count > 0)
                        {
                            for (int i = 0; i < SupportDocquery.Count; i++)
                            {
                                listDocument.Add(new DocumentDetailsModel()
                                {
                                    AttachementType = Common.getDocumentTypeName(SupportDocquery[i].DocType ?? 0),
                                    AttachementName = SupportDocquery[i].AttachmentName,
                                    Attachment = SupportDocquery[i].DocName,
                                });
                            }
                        }
                    }
                }
                prjModel.CoPiDetails = listCoPI;
                prjModel.CoPiOtherInstitute = CoPIOther;
                prjModel.Allocation = listAllocation;
                prjModel.Instalment = ListInstalment;
                prjModel.Staff = listStaffDetails;
                prjModel.OtherStaff = listOtherStaff;
                prjModel.DocDetail = listDocument;
                return prjModel;
            }
            catch (Exception ex)
            {
                ProjectViewDetailsModel prjModel = new ProjectViewDetailsModel();
                return prjModel;
            }
        }

        public ProjectEnhanceandExtenDetailsModel GetEnhancementandExtensionDetails(int projectid)
        {
            try
            {
                ProjectEnhanceandExtenDetailsModel pjct = new ProjectEnhanceandExtenDetailsModel();
                using (var context = new IOASDBEntities())
                {
                    var project = (from en in context.tblProject
                                   where (en.ProjectId == projectid)
                                   select en).FirstOrDefault();
                    var enhancedetails = (from en in context.tblProjectEnhancement
                                          where (en.ProjectId == projectid && (en.IsEnhancementonly == true || en.IsEnhancementWithExtension == true))
                                          select en).ToList();
                    var extensiondetails = (from en in context.tblProjectEnhancement
                                            where (en.ProjectId == projectid && (en.IsExtensiononly == true || en.IsEnhancementWithExtension == true))
                                            select en).ToList();
                    var DocumentPath = "~/Content/SupportDocuments";
                    int[] enhancementid = new int[enhancedetails.Count()];
                    string[] enhancementordernumber = new string[enhancedetails.Count()];
                    decimal?[] enhancedamount = new decimal?[enhancedetails.Count()];
                    decimal?[] previousamount = new decimal?[enhancedetails.Count()];
                    DateTime?[] enhancementDate = new DateTime?[enhancedetails.Count()];
                    string[] enhancedate = new string[enhancedetails.Count()];
                    string[] enhancedocpath = new string[enhancedetails.Count()];
                    string[] enhancedocname = new string[enhancedetails.Count()];
                    int[] extensionid = new int[extensiondetails.Count()];
                    string[] extensionordernumber = new string[extensiondetails.Count()];
                    string[] extenDate = new string[extensiondetails.Count()];
                    string[] extendedDate = new string[extensiondetails.Count()];
                    string[] prevextenDate = new string[extensiondetails.Count()];
                    string[] extendocpath = new string[extensiondetails.Count()];
                    string[] extendocname = new string[extensiondetails.Count()];

                    for (int i = 0; i < enhancedetails.Count(); i++)
                    {
                        enhancementid[i] = enhancedetails[i].ProjectEnhancementId;
                        enhancementordernumber[i] = enhancedetails[i].DocumentReferenceNumber;
                        enhancementDate[i] = enhancedetails[i].CrtsTS;
                        enhancedate[i] = String.Format("{0:ddd dd-MMM-yyyy}", enhancedetails[i].CrtsTS);
                        enhancedocname[i] = enhancedetails[i].AttachmentPath;
                        enhancedamount[i] = enhancedetails[i].EnhancedSanctionValue;
                        previousamount[i] = enhancedetails[i].OldSanctionValue;
                        enhancedocpath[i] = DocumentPath;
                    }
                    for (int i = 0; i < extensiondetails.Count(); i++)
                    {
                        extensionid[i] = extensiondetails[i].ProjectEnhancementId;
                        extensionordernumber[i] = extensiondetails[i].DocumentReferenceNumber;
                        extenDate[i] = String.Format("{0:ddd dd-MMM-yyyy}", extensiondetails[i].ExtendedDueDate);
                        prevextenDate[i] = String.Format("{0:ddd dd-MMM-yyyy}", extensiondetails[i].PresentDueDate);
                        extendedDate[i] = String.Format("{0:ddd dd-MMM-yyyy}", extensiondetails[i].CrtsTS);
                        extendocname[i] = extensiondetails[i].AttachmentPath;
                        extendocpath[i] = DocumentPath;
                    }

                    if (enhancedetails.Count() > 0)
                    {

                        pjct.ProjectEnhancementID = enhancementid;
                        pjct.EnhanceRefNumber = enhancementordernumber;
                        pjct.EnhancedDate = enhancedate;
                        pjct.EnhancedSanctionValue = enhancedamount;
                        pjct.OldSanctionValue = previousamount;
                        pjct.EnhancedocPath = enhancedocpath;
                        pjct.Enhancedocname = enhancedocname;

                    }
                    if (extensiondetails.Count() > 0)
                    {

                        pjct.ProjectExtensionID = extensionid;
                        pjct.ExtenRefNumber = extensionordernumber;
                        pjct.ExtndDueDate = extenDate;
                        pjct.PrsntDueDate = prevextenDate;
                        pjct.ExtendedDate = extendedDate;
                        pjct.ExtendocPath = extendocpath;
                        pjct.Extendocname = extendocname;
                    }
                    pjct.ProjectNumber = project.ProjectNumber;
                    pjct.Projecttitle = project.ProjectTitle;

                }
                return pjct;
            }
            catch (Exception ex)
            {
                return new ProjectEnhanceandExtenDetailsModel();
            }
        }

        public static PagedData<ProjectSearchResultModel> SearchProjectList(ProjectSearchFieldModel model, int page, int pageSize, DateFilterModel PrpsalApprovedDate)
        {
            var project = new PagedData<ProjectSearchResultModel>();
            List<ProjectSearchResultModel> list = new List<ProjectSearchResultModel>();
            using (var context = new IOASDBEntities())
            {
                int skiprec = 0;
                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                var prequery = from p in context.tblProject
                               join us in context.vwFacultyStaffDetails on p.PIName equals us.UserId
                               join ag in context.tblAgencyMaster on p.SponsoringAgency equals ag.AgencyId
                               select new ProjectPredicate
                               {
                                   prj = p,
                                   u = us,
                                   agy = ag
                               };
                var predicate = PredicateBuilder.BaseAnd<ProjectPredicate>();
                //if (!string.IsNullOrEmpty(ProjectNumber))
                //    predicate = predicate.And(d => d.ProjectNumber.Contains(ProjectNumber));
                predicate = predicate.And(d => d.prj.Status == "Active");

                if (!string.IsNullOrEmpty(model.ProjectNumber))
                    predicate = predicate.And(d => d.prj.ProjectNumber.Contains(model.ProjectNumber));
                if (!string.IsNullOrEmpty(model.EFProjectNumber))
                    predicate = predicate.And(d => d.prj.ProjectNumber.Contains(model.EFProjectNumber));
                if (model.ProjectType != null)
                    predicate = predicate.And(d => d.prj.ProjectType == model.ProjectType);
                if (model.FromSODate != null && model.ToSODate != null)
                {
                    model.ToSODate = model.ToSODate.Value.Date.AddDays(1).AddTicks(-1);
                    predicate = predicate.And(d => d.prj.SanctionOrderDate >= model.FromSODate && d.prj.SanctionOrderDate <= model.ToSODate);
                }
                if (model.PIName != null)
                    predicate = predicate.And(d => d.prj.PIName == model.PIName);
                if (model.ProjectTitle != null)
                    predicate = predicate.And(d => d.prj.ProjectTitle.Contains(model.ProjectTitle));
                if (model.AgencyName != null)
                    predicate = predicate.And(d => d.agy.AgencyName.Contains(model.AgencyName));
                if (model.NameOfPI != null)
                    predicate = predicate.And(d => d.u.FirstName.Contains(model.NameOfPI));
                if (model.PICode != null)
                    predicate = predicate.And(d => d.u.EmployeeId == model.PICode);
                if (PrpsalApprovedDate.from != null && PrpsalApprovedDate.to != null)
                {
                    PrpsalApprovedDate.to = PrpsalApprovedDate.to.Value.Date.AddDays(1).AddTicks(-1);
                    predicate = predicate.And(d => d.prj.ProposalApprovedDate >= PrpsalApprovedDate.from && d.prj.ProposalApprovedDate <= PrpsalApprovedDate.to);
                }
                if (model.BudgetValue != null)
                    predicate = predicate.And(d => d.prj.BaseValue == model.BudgetValue);
                //if (model.FromSRBDate != null && model.ToSRBDate != null)
                //    predicate = predicate.And(d => d.InwardDate >= model.FromSRBDate && d.InwardDate <= model.ToSRBDate);
                var query = prequery.Where(predicate).OrderByDescending(m => m.prj.ProjectId).Skip(skiprec).Take(pageSize).ToList();
                project.TotalRecords = prequery.Where(predicate).Count();
                //var query = (from P in context.tblProject
                //             join user in context.tblUser on P.PIName equals user.UserId
                //             join dept in context.tblPIDepartmentMaster on P.PIDepartment equals dept.DepartmentId
                //             join agency in context.tblAgencyMaster on P.SponsoringAgency equals agency.AgencyId
                //             where ((String.IsNullOrEmpty(ProposalNumber) || P.ProposalNumber.Contains(ProposalNumber)) && (P.ProjectType == ProjectType) /*|| user.UserId == PIname*/ && (P.SanctionOrderDate >= FromSODate) && (P.SanctionOrderDate <= ToSOdate) /*&& (P.CrtdTS >= Fromdate) && (P.CrtdTS <= Todate)*/)
                //             orderby P.ProjectId
                //             select new { P, user.FirstName, user.LastName, user.EMPCode, dept.Department, agency.AgencyName }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        //var pi = query[i].PIName;
                        //var agency = query[i].SponsoringAgency;
                        //var dept = query[i].PIDepartment;
                        //var userquery = (from U in context.tblUser
                        //                 where (U.UserId == pi)
                        //                 select U).FirstOrDefault();
                        //var agencyquery = (from A in context.tblAgencyMaster
                        //                   where (A.AgencyId == agency)
                        //                   select A).FirstOrDefault();
                        //var userquery = (from D in context.vwFacultyStaffDetails
                        //                 where (D.UserId == pi)
                        //                 select D).FirstOrDefault();
                        //if (userquery != null)
                        //{
                        list.Add(new ProjectSearchResultModel()
                        {
                            Sno = i + 1,
                            ProjectId = query[i].prj.ProjectId,
                            Projecttitle = query[i].prj.ProjectTitle,
                            ProjectNumber = query[i].prj.ProjectNumber,
                            Budget = query[i].prj.BaseValue,
                            SponsoringAgency = query[i].prj.SponsoringAgency,
                            SponsoringAgencyName = query[i].agy.AgencyName,
                            NameofPI = query[i].u.FirstName,
                            PIDepartmentName = query[i].u.DepartmentName,
                            EmpCode = query[i].u.EmployeeId,
                            PrpsalApprovedDate = String.Format("{0:s}", query[i].prj.ProposalApprovedDate)
                        });
                        //}
                    }
                }
            }
            project.Data = list;
            return project;
        }

        public ProjectSummaryModel getProjectSummaryForDailyBalance(int ProjectId, DateTime? Date = null)        {            try            {                ProjectSummaryModel prjModel = new ProjectSummaryModel();                using (var context = new IOASDBEntities())                {                    var qryProject = (from prj in context.tblProject                                      where (prj.ProjectId == ProjectId)
                                      select prj).FirstOrDefault();                    var qryPreviousCommit = (from C in context.tblCommitment                                             join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId                                             where (C.ProjectId == ProjectId && C.Status == "Active") && (!(Date == null || C.CRTD_TS == Date))                                             select new { D.BalanceAmount, D.ReversedAmount }).ToList();                    var BalanceAmt = qryPreviousCommit.Select(m => m.BalanceAmount).Sum();                    var ReversedAmount = qryPreviousCommit.Select(m => m.ReversedAmount).Sum();

                    /*Spent amount calculation Start*/

                    decimal? Debit = 0, Credit = 0, spentAmt = 0;                    var qrySpenAmt = (from C in context.vwCommitmentSpentBalance where (C.ProjectId == ProjectId) && (!(Date == null || C.CRTD_TS == Date)) select C.AmountSpent).Sum();                    if (qrySpenAmt == null)                        qrySpenAmt = 0;                    spentAmt = qrySpenAmt;
                    //var projOB=from C in context.tblProjectOB
                    var FundTransferDebit = (from C in context.tblProjectTransfer                                             from D in context.tblProjectTransferDetails                                             where C.ProjectTransferId == D.ProjectTransferId                                             where (C.DebitProjectId == ProjectId) && (!(Date == null || C.CRTD_TS == Date))                                             select D).ToList();                    if (FundTransferDebit.Count > 0)                    {                        Debit = FundTransferDebit.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum();                        if (Debit != 0)                            spentAmt = spentAmt + Debit;                    }                    var FundTransferCredit = (from C in context.tblProjectTransfer                                              from D in context.tblProjectTransferDetails                                              where C.ProjectTransferId == D.ProjectTransferId                                              where (C.CreditProjectId == ProjectId) && (!(Date == null || C.CRTD_TS == Date))                                              select D).ToList();                    if (FundTransferCredit.Count > 0)                    {                        Credit = FundTransferCredit.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum();                        if (Credit != 0)                            spentAmt = spentAmt - Debit;                    }
                    /*claim amount institute start*/
                    var qryInstituteClaim = (from Neg in context.tblInstituteClaims                                             where Neg.ProjectId == ProjectId && Neg.Status == "Completed" && (!(Date == null || Neg.CRTD_TS == Date))                                             select Neg).ToList();                    var claimAmt = qryInstituteClaim.Select(m => m.ClaimAmount).Sum();                    spentAmt = spentAmt - claimAmt;
                    /*claim amount institute end*/
                    /*Spent amount calculation End*/
                    var AvailableCommitment = BalanceAmt + ReversedAmount;                    var QryReceipt = (from C in context.tblReceipt where (C.ProjectId == ProjectId) && (!(Date == null || C.CrtdTS == Date)) select C).ToList();                    var receiptAmt = QryReceipt.Select(m => m.ReceiptAmount).Sum();                    var OverHead = QryReceipt.Select(m => m.ReceiptOverheadValue).Sum();
                    decimal CGST = QryReceipt.Select(m => m.CGST).Sum() ?? 0;                    decimal SGST = QryReceipt.Select(m => m.SGST).Sum() ?? 0;                    decimal IGST = QryReceipt.Select(m => m.IGST).Sum() ?? 0;                    var GST = CGST + SGST + IGST;
                    receiptAmt = receiptAmt - (GST + OverHead);
                    /* Negative balance taking query*/
                    var qryNegativeBal = (from Neg in context.tblNegativeBalance                                          where (Neg.ProjectId == ProjectId && Neg.Status == "Approved") && (!(Date == null || Neg.CRTD_TS == Date))                                          select Neg.NegativeBalanceAmount).Sum();
                    /* Negative balance taking query end*/
                    /* Opening balance taking query*/
                    decimal qryOpeningBal = (from OB in context.tblProjectOB                                             where OB.ProjectId == ProjectId                                             select OB.OpeningBalance).Sum() ?? 0;
                    /* Opening balance taking query end*/

                    if (qryProject != null)                    {                        prjModel.ProjectTittle = qryProject.ProjectTitle;                        prjModel.PIname = Common.GetPIName(qryProject.PIName ?? 0);                        prjModel.SanctionedValue = qryProject.SanctionValue ?? 0;
                        prjModel.OpeningBalance = qryOpeningBal;
                        //sum(ReciptAmt-(GST+OverHeads ))
                        prjModel.TotalReceipt = Convert.ToDecimal(receiptAmt);                        prjModel.AmountSpent = spentAmt ?? 0;                        prjModel.PreviousCommitment = AvailableCommitment ?? 0;
                        //TotalReceipt - AmountSpent + PreviousCommitment
                        prjModel.AvailableBalance = (prjModel.TotalReceipt - (prjModel.AmountSpent + prjModel.PreviousCommitment));                        prjModel.FinancialYear = Common.GetFinYear(qryProject.FinancialYear ?? 0);                        prjModel.SanctionOrderNo = qryProject.SanctionOrderNumber;                        prjModel.SanctionOrderDate = String.Format("{0:ddd dd-MMM-yyyy}", qryProject.SanctionOrderDate);                        prjModel.ProjectApprovalDate = string.Format("{0:ddd dd-MMM-yyyy}", qryProject.ProposalApprovedDate);                        prjModel.ProjectDuration = prjModel.ProjectDuration;                        prjModel.ProposalNo = qryProject.ProposalNumber;                        prjModel.ProjectNo = qryProject.ProjectNumber;                        prjModel.BaseValue = qryProject.BaseValue ?? 0;                        prjModel.ProjectType = Common.getprojectTypeName(qryProject.ProjectType ?? 0);                        prjModel.ApplicableTax = qryProject.ApplicableTax ?? 0;
                        //var Data= Common.getProjectNo(qryProject.ProjectType ?? 0);
                        //prjModel.CommitNo = Data.Item2;
                        prjModel.ApprovedNegativeBalance = qryNegativeBal ?? 0;                        prjModel.NetBalance = (prjModel.AvailableBalance + prjModel.ApprovedNegativeBalance);                        prjModel.OverHeads = OverHead ?? 0;
                        
                        //sum(CGST+SGST+IGST) 
                        prjModel.GST = GST;                    }

                    //taking total commitment amount headwise
                    var qryHeadCommit = (from C in context.tblCommitment                                         join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId                                         where (C.ProjectId == ProjectId && C.Status == "Active") && (!(Date == null || C.CRTD_TS == Date))                                         select new { D.AllocationHeadId, D.Amount }).ToList();                    List<HeadWiseDetailModel> List = new List<HeadWiseDetailModel>();                    List<HeadWiseDetailModel> ListSpent = new List<HeadWiseDetailModel>();                    List<HeadWiseDetailModel> Balance = new List<HeadWiseDetailModel>();                    if (qryHeadCommit.Count > 0)                    {                        var distCount = qryHeadCommit.Select(m => m.AllocationHeadId).Distinct().ToArray();                        for (int i = 0; i < distCount.Length; i++)                        {                            int headId = distCount[i] ?? 0;                            var HeadName = Common.getAllocationHeadName(headId);                            decimal amt = qryHeadCommit.Where(m => m.AllocationHeadId == headId).Sum(m => m.Amount) ?? 0;                            List.Add(new HeadWiseDetailModel()                            {                                AllocationId = headId,                                AllocationHeadName = HeadName,                                Amount = amt                            });                        }                    }

                    //taking total spent amount headwise
                    var qrySpent = (from C in context.vwCommitmentSpentBalance                                    where (C.ProjectId == ProjectId) && (!(Date == null || C.CRTD_TS == Date))                                    select new { C.AllocationHeadId, C.AmountSpent }).ToList();                    if (qrySpent.Count > 0)                    {                        var distCount = qrySpent.Select(m => m.AllocationHeadId).Distinct().ToArray();                        for (int i = 0; i < distCount.Length; i++)                        {                            int headId = distCount[i] ?? 0;                            var HeadName = Common.getAllocationHeadName(headId);                            decimal amt = qrySpent.Where(m => m.AllocationHeadId == headId).Sum(m => m.AmountSpent) ?? 0;                            ListSpent.Add(new HeadWiseDetailModel()                            {                                AllocationId = headId,                                AllocationHeadName = HeadName,                                Amount = amt                            });                        }                    }
                    //taking balance for future commitments
                    var qryAllocation = (from C in context.tblProjectAllocation                                         where (C.ProjectId == ProjectId) && (!(Date == null || C.CrtdTS == Date))                                         select new { C.AllocationHead, C.AllocationValue }
                                       ).ToList();

                    //var qryAllocation

                    if (qryAllocation.Count > 0)                    {                        decimal balance = 0;                        for (int j = List.Count() - 1; j < qryAllocation.Count(); j++)                        {                            if (j + 1 == List.Count())                            {                                for (int k = 0; k < List.Count(); k++)                                {                                    decimal amt = qryAllocation[k].AllocationValue ?? 0;                                    balance = amt - List[k].Amount;                                    Balance.Add(new HeadWiseDetailModel()                                    {                                        AllocationId = qryAllocation[k].AllocationHead ?? 0,                                        AllocationHeadName = List[k].AllocationHeadName,                                        Amount = balance                                    });                                }                            }                            else                            {                                var HeadId = qryAllocation[j].AllocationHead ?? 0;                                var HeadName = Common.getAllocationHeadName(HeadId);                                Balance.Add(new HeadWiseDetailModel()                                {                                    AllocationId = HeadId,                                    AllocationHeadName = HeadName,                                    Amount = qryAllocation[j].AllocationValue ?? 0                                });                            }                        }                    }                    prjModel.HeadWiseCommitment = List;                    prjModel.HeadWiseSpent = ListSpent;                    prjModel.HeadWiseAllocation = Balance;                }                return prjModel;            }            catch (Exception ex)            {                ProjectSummaryModel prjModel = new ProjectSummaryModel();                return prjModel;            }        }
    }
}