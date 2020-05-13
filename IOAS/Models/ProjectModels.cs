using Foolproof;
using IOAS.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Models
{
    public class ProjectModel
    {
        public ProjectSrchFieldsModel srchField { get; set; }
        public PagedData<ProjectResultModels> resultField { get; set; }
    }
    public class ProjectSrchFieldsModel
    {
        public string srchKeyword { get; set; }
        public Nullable<int> selMinistry { get; set; }
        public Nullable<int> Institute { get; set; }
        public Nullable<int> Industry { get; set; }
        public Nullable<int> Proposalstatus { get; set; }
    }
   
    public class ProjectResultModels
    {
        public int proposalId { get; set; }
        public string proposalTitle { get; set; }
        public Nullable<int> ProposalType { get; set; }
        public string nameOfPI { get; set; }
        public string instituteOfPI { get; set; }
        public string DEC_Domain { get; set; }
        public string DEC_Group { get; set; }
        public string Doc_Name { get; set; }
        public string ProsNumber { get; set; }
        public string MHRD { get; set; }
        public string Ministry { get; set; }
        public string Industry { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal proposedBudget { get; set; }
        public int status { get; set; }
        public string statusString { get; set; }
        public bool isReviewAllocated { get; set; }
        public bool isReviewed { get; set; }

        public Nullable<int> PIUserID { get; set; }
    }

    public class CreateProjectModel
    {
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public Nullable<int> Sno { get; set; }
        [Required]
        public string Projecttitle { get; set; }


        public Nullable<int> PIname { get; set; }
        public Nullable<int> PIDesignation { get; set; }
        public String PIEmail { get; set; }
        public Nullable<int> Prjcttype { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string EmpCode { get; set; }
        public string ProjectTypeName { get; set; }
        public Nullable<int> ProjectSubType { get; set; }
        public Nullable<int> ConsFundingCategory { get; set; }
        public string NameofPI { get; set; }
        [Required(ErrorMessage = "Agency field is required.")]
        public Nullable<int> SponsoringAgency { get; set; }
        public string SponsoringAgencyName { get; set; }
        public Nullable<int> ProjectcrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        public Nullable<int> PrpsalNumber { get; set; }
        [Required]
        public Nullable<int> FinancialYear { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Budget { get; set; }
        public int[] Docid { get; set; }
        public string[] DocPath { get; set; }
        public int[] DocType { get; set; }
        public string[] DocDescrip { get; set; }
        public string[] DocName { get; set; }
        public string[] AttachName { get; set; }
        public string[] DocCrtdUserid { get; set; }
        public Nullable<DateTime>[] DocCrtd_TS { get; set; }

        //public HttpPostedFileBase[] file { get; set; }
        //public HttpPostedFileBase taxprooffile { get; set; }
        public string taxprooffilename { get; set; }

        [Required]
        public string Department { get; set; }
        public string PIDepartmentName { get; set; }
        //[Required]
        //[DataType(DataType.DateTime)]
        //public Nullable<DateTime> Proposalinwarddate { get; set; }
        //public string Prpsalinwrddate { get; set; }

        public string workphone { get; set; }
        public int[] CoPIname { get; set; }
        public int[] CoPIid { get; set; }
        public int[] CoPIDesignation { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string[] CoPIEmail { get; set; }
        public string[] CoPIDepartment { get; set; }
        public Nullable<int> Projectdurationyears { get; set; }
        public Nullable<int> Projectdurationmonths { get; set; }
        public Nullable<int> Schemename { get; set; }
        public Nullable<int> SchemeCode { get; set; }
        public string Personapplied { get; set; }
        public string Remarks { get; set; }

        [RequiredIf("ProjectSubType", 2, ErrorMessage = "Project Funding Type field is required")]
        public Nullable<int> ProjectFundingType_Qust_1 { get; set; }
        [RequiredIf("Prjcttype", 2, ErrorMessage = "Project Funding Type field is required")]
        public Nullable<int> ConsProjectFundingType_Qust_1 { get; set; }


        [RequiredIf("IndAndBoth", true, ErrorMessage = "Indian Project Funded By field is required")]
        public Nullable<int> ProjectFundedby_Qust_1 { get; set; }

        [RequiredIf("ForgnAndBoth", true, ErrorMessage = "Foreign currency field is required")]
        public Nullable<int> SelCurr { get; set; }

        [RequiredIf("ForgnAndBoth", true, ErrorMessage = "Conversion rate field is required")]
        public Nullable<decimal> ConversionRate { get; set; }

        [RequiredIf("ConsFundTypeForgnAndBoth", true, ErrorMessage = "Foreign currency field is required")]
        public Nullable<int> ConsSelCurr { get; set; }

        [RequiredIf("ConsFundTypeForgnAndBoth", true, ErrorMessage = "Conversion rate field is required")]
        public Nullable<decimal> ConsConversionRate { get; set; }


        [RequiredIf("ForgnAndBoth", true, ErrorMessage = "Foreign Project Funded By field is required")]
        public Nullable<int> ForgnProjectFundedby_Qust_1 { get; set; }

        [RequiredIf("ProjectFundingType_Qust_1", 1, ErrorMessage = "Project Category field is required")]
        public string Projectcatgry_Qust_1 { get; set; }
        public Nullable<int> ProjectFundingBody_Qust_1 { get; set; }

        [RequiredIf("IndGovFund_MHRD", true, ErrorMessage = "Indian Government Department field is required")]
        public string indprjctfundbodygovt_Agencydeptname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("IndGovFund_MHRD", true, ErrorMessage = "Indian Govt. Department Amount field is required")]
        public Nullable<Decimal> indprjctfundbodygovt_deptAmount { get; set; }

        [RequiredIf("IndGovFund_Mnstry", true, ErrorMessage = "Indian Government Ministry field is required")]
        public Nullable<int> indprjctfundbodygovt_Agencymnstryname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("IndGovFund_Mnstry", true, ErrorMessage = "Indian Government Ministry Amount field is required")]
        public Nullable<Decimal> indprjctfundbodygovt_mnstryAmount { get; set; }

        [RequiredIf("IndGovFund_Univ", true, ErrorMessage = "Indian Government University field is required")]
        public string indprjctfundbodygovt_Agencyunivname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("IndGovFund_Univ", true, ErrorMessage = "Indian Government University Amount field is required")]
        public Nullable<Decimal> indprjctfundbodygovt_univAmount { get; set; }
        [RequiredIf("ForgnGovFund_Dep", true, ErrorMessage = "Foreign Government Department Country field is required")]
        public Nullable<int> forgnprjctfundbodygovt_country { get; set; }
        [RequiredIf("ForgnGovFund_Dep", true, ErrorMessage = "Foreign Government Department field is required")]
        public string forgnprjctfundbodygovt_Agencydeptname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("ForgnGovFund_Dep", true, ErrorMessage = "Foreign Govt. Department Amount field is required")]
        public Nullable<Decimal> forgnprjctfundbodygovt_deptAmount { get; set; }
        [RequiredIf("ForgnGovFund_Univ", true, ErrorMessage = "Foreign Government University Country field is required")]
        public Nullable<int> forgnprjctfundbodygovt_univcountry { get; set; }
        [RequiredIf("ForgnGovFund_Univ", true, ErrorMessage = "Foreign Government University field is required")]
        public string forgnprjctfundbodygovt_Agencyunivname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("ForgnGovFund_Univ", true, ErrorMessage = "Foreign Govt. Univ. Amount field is required")]
        public Nullable<Decimal> forgnprjctfundbodygovt_univAmount { get; set; }
        [RequiredIf("ForgnGovFund_Other", true, ErrorMessage = "Foreign Government Agency Country field is required")]
        public Nullable<int> forgnprjctfundbodygovt_otherscountry { get; set; }
        [RequiredIf("ForgnGovFund_Other", true, ErrorMessage = "Foreign Government Agency field is required")]
        public string forgnprjctfundbodygovt_othersagncyname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("ForgnGovFund_Other", true, ErrorMessage = "Foreign Govt. Agency Amount field is required")]
        public Nullable<Decimal> forgnprjctfundbodygovt_othersAmount { get; set; }
        [RequiredIf("ForgnNonGovFund_Dep", true, ErrorMessage = "Foreign Non Gov. Dep. Country field is required")]
        public Nullable<int> forgnprjctfundbodynongovt_country { get; set; }
        [RequiredIf("ForgnNonGovFund_Dep", true, ErrorMessage = "Foreign Non Government Department field is required")]
        public string forgnprjctfundbodynongovt_Agencydeptname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("ForgnNonGovFund_Dep", true, ErrorMessage = "Foreign Non Govt. Dept. Amount field is required")]
        public Nullable<Decimal> forgnprjctfundbodynongovt_deptAmount { get; set; }
        [RequiredIf("ForgnNonGovFund_Univ", true, ErrorMessage = "Foreign Non Govt. Univ. Country field is required")]
        public Nullable<int> forgnprjctfundbodynongovt_univcountry { get; set; }
        [RequiredIf("ForgnNonGovFund_Univ", true, ErrorMessage = "Foreign Non Government University field is required")]
        public string forgnprjctfundbodynongovt_Agencyunivname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("ForgnNonGovFund_Univ", true, ErrorMessage = "Foreign Non Govt. Univ. Amount field is required")]
        public Nullable<Decimal> forgnprjctfundbodynongovt_univAmount { get; set; }
        [RequiredIf("ForgnNonGovFund_Other", true, ErrorMessage = "Foreign Non Govt. Agency Country field is required")]
        public Nullable<int> forgnprjctfundbodynongovt_otherscountry { get; set; }
        [RequiredIf("ForgnNonGovFund_Other", true, ErrorMessage = "Foreign Non Government Other Agency name field is required")]
        public string forgnprjctfundbodynongovt_othersagncyname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("ForgnNonGovFund_Other", true, ErrorMessage = "Foreign Non Govt. Agency Amount field is required")]
        public Nullable<Decimal> forgnprjctfundbodynongovt_othersAmount { get; set; }
        public string SchemeAgency { get; set; }
        public Nullable<int> InternalSchemeFundingAgency { get; set; }
        public string ExternalSchemename { get; set; }
        public string SchemePersonApplied { get; set; }
        public string SchemePersonAppliedDesignation { get; set; }
        [RequiredIf("IndNonGovFund_Indus", true, ErrorMessage = "Indian Non Government Industry field is required")]
        public string indprjctfundbodynongovt_AgencyIndstryname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("IndNonGovFund_Indus", true, ErrorMessage = "Indian Non Govt. Industry Amount field is required")]
        public Nullable<Decimal> indprjctfundbodynongovt_IndstryAmount { get; set; }
        [RequiredIf("IndNonGovFund_Univ", true, ErrorMessage = "Indian Non Government University field is required")]
        public string indprjctfundbodynongovt_Agencyunivname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("IndNonGovFund_Univ", true, ErrorMessage = "Indian Non Govt Univ. Amount field is required")]
        public Nullable<Decimal> indprjctfundbodynongovt_univAmount { get; set; }
        [RequiredIf("IndNonGovFund_Other", true, ErrorMessage = "Indian Non Government Agency field is required")]
        public string indprjctfundbodynongovt_Agencyothersname { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("IndNonGovFund_Other", true, ErrorMessage = "Indian Non Govt. Agency Amount field is required")]
        public Nullable<Decimal> indprjctfundbodynongovt_othersAmount { get; set; }
        public Nullable<int> Categoryofproject { get; set; }
        //[Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        //[Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }

        public int[] Allocationhead { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        //   [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] Allocationvalue { get; set; }
        public int[] Allocationid { get; set; }
        public Nullable<Decimal> Allocationtotal { get; set; }
        [RequiredIf("Prjcttype", 2, ErrorMessage = "Tax Service field is required")]
        public Nullable<int> constaxservice { get; set; }
        [RequiredIf("IndTaxService", true, ErrorMessage = "State of funding agency field is required")]
        public string indfundngagncystate { get; set; }
        [RequiredIf("IndTaxService", true, ErrorMessage = "Location of Funding Agency field is required")]
        public string indfundngagncylocation { get; set; }
        [RequiredIf("constaxservice", 3, ErrorMessage = "Country field is required")]
        public Nullable<int> forgnfndngagncycountry { get; set; }
        [RequiredIf("constaxservice", 3, ErrorMessage = "State field is required")]
        public string forgnfundngagncystate { get; set; }
        [RequiredIf("constaxservice", 3, ErrorMessage = "Location field is required")]
        public string forgnfundngagncylocation { get; set; }
        [RequiredIf("Prjcttype", 2, ErrorMessage = "Tax Status field is required")]
        public Nullable<int> ConsProjectTaxType_Qust_1 { get; set; }
        [RequiredIf("TaxException", true, ErrorMessage = "Tax exception reason field is required")]
        public string ConsProjectReasonfornotax { get; set; }
        public string Docpathfornotax { get; set; }
        [RequiredIf("Taxserviceregstatus", 1, ErrorMessage = "GSTIN field is required")]
        public string GSTNumber { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        public Nullable<int> TotalNoofProjectStaffs { get; set; }
        public int[] StaffCategoryID { get; set; }
        public int[] CategoryofStaffs { get; set; }
        public int[] NoofStaffs { get; set; }
        public Decimal[] SalaryofStaffs { get; set; }
        public Nullable<int> SumofStaffs { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> SumSalaryofStaffs { get; set; }
        //[Required]
        //public int AgencyCode { get; set; }
        [Required]
        public Nullable<int> AgencyCodeid { get; set; }
        public string ScientistName { get; set; }
        public string ScientistEmail { get; set; }
        public string ScientistMobile { get; set; }
        public string ScientistAddress { get; set; }
        public string AgencyCode { get; set; }        
        public string Agencyregname { get; set; }        
        public string Agencyregaddress { get; set; }
        public string Agencycontactperson { get; set; }
        public string Agencycontactpersondesignation { get; set; }
        public string AgencycontactpersonEmail { get; set; }
        public string Agencycontactpersonmobile { get; set; }
        [Required]
        [Display(Name = "Tentative Start Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> TentativeStartdate { get; set; }
        [Required]
        [Display(Name = "Tentative Close Date")]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> TentativeClosedate { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Startdate { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Closedate { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> ProposalApprovedDate { get; set; }
        public string PrpsalApprovedDate { get; set; }
        public string SanctionOrderNumber { get; set; }

        [DataType(DataType.DateTime)]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        public string SODate { get; set; }
        public string TentativestrtDate { get; set; }
        public string strtDate { get; set; }
        public string TentativeclsDate { get; set; }
        public string clsDate { get; set; }
        public Nullable<int> ConsProjectSubType { get; set; }
        public List<MasterlistviewModel>[] PIListDepWise { get; set; }
        public string[] CoPIInstitute { get; set; }
        public string[] OtherInstituteCoPIDepartment { get; set; }
        public string[] OtherInstituteCoPIName { get; set; }
        public int[] OtherInstituteCoPIid { get; set; }
        public string[] RemarksforOthrInstCoPI { get; set; }
        public List<MasterlistviewModel> MasterPIListDepWise { get; set; }
        public List<MasterlistviewModel> MainProjectList { get; set; }
        public ProjectSearchFieldModel SearchField { get; set; }
        public PagedData<ProjectSearchResultModel> SearchResult { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> BaseValue { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 99.99, ErrorMessage = "Applicable Tax should not be greater than 100 percentage")]
        public Nullable<Decimal> ApplicableTax { get; set; }
        public string ProjectSubTypeName { get; set; }

        [RequiredIf("GovAndBoth", true, ErrorMessage = "Indian Government Funding Body field is required")]
        public Nullable<int>[] IndProjectFundingGovtBody_Qust_1 { get; set; }


        [RequiredIf("NonGovAndBoth", true, ErrorMessage = "Indian Non Government Funding Body field is required")]
        public Nullable<int>[] IndProjectFundingNonGovtBody_Qust_1 { get; set; }
        [RequiredIf("ForgnGovAndBoth", true, ErrorMessage = "Foreign Government Funding Body field is required")]
        public Nullable<int>[] ForgnProjectFundingGovtBody_Qust_1 { get; set; }
        [RequiredIf("ForgnNonGovAndBoth", true, ErrorMessage = "Foreign Non Government Funding Body field is required")]
        public Nullable<int>[] ForgnProjectFundingNonGovtBody_Qust_1 { get; set; }
        public int[] IndProjectFundingGovtBodyId { get; set; }
        public int[] IndProjectFundingNonGovtBodyId { get; set; }
        public int[] ForgnProjectFundingGovtBodyId { get; set; }
        public int[] ForgnProjectFundingNonGovtBodyId { get; set; }
        public Nullable<int> TypeofProject { get; set; }
        [RequiredIf("TypeofProject", 2, ErrorMessage = "Name of the Coordinator field is required")]
        public string Collaborativeprojectcoordinator { get; set; }
        [RequiredIf("TypeofProject", 2, ErrorMessage = "Project type field is required")]
        public Nullable<Int32> CollaborativeProjectType { get; set; }
        [RequiredIf("TypeofProject", 2, ErrorMessage = "Insitute / Industry name field is required")]
        public string CollaborativeprojectAgency { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        //[RequiredIf("TypeofProject", 2, ErrorMessage = "Coordinator email field is required")]
        public string Collaborativeprojectcoordinatoremail { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("TypeofProject", 2, ErrorMessage = "Other Insitute / Industry share field is required")]
        public Nullable<Decimal> Collaborativeprojecttotalcost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        [RequiredIf("TypeofProject", 2, ErrorMessage = "IITM share field is required")]
        public Nullable<Decimal> CollaborativeprojectIITMCost { get; set; }
        public string Agencycontactpersonaddress { get; set; }
        //[Required]
        //[DataType(DataType.DateTime)]
        //public Nullable<DateTime> Inputdate { get; set; }
        public string Inptdate { get; set; }
        public string JointDevelopment_Qust_1 { get; set; }
        public string[] JointDevelopmentCompany { get; set; }
        public int[] JointDevelopmentCompanyId { get; set; }
        public string[] JointDevelopmentRemarks { get; set; }
        public string[] CopiDesig { get; set; }
        //public string [] CopiDep { get; set; }
        public int[] Coinvestgatorname { get; set; }
        public int[] Documenttype { get; set; }
        //[RequiredIf("ConsFundTypeForgnAndBoth", true, ErrorMessage = "Currency Type field is required")]
        public string ConsForgnCurrencyType { get; set; }
        public string TaxserviceGST { get; set; }
        [RequiredIf("constaxservice", 2, ErrorMessage = "Registration Status field is required")]
        public Nullable<int> Taxserviceregstatus { get; set; }
        public string ConsFundingcategoryname { get; set; }
        public string Conssubtypename { get; set; }
        [Display(Name = "PI PCF")]
        public Nullable<decimal> PIPCF { get; set; }
        [Display(Name = "PI RMF")]
        public Nullable<decimal> PIRMF { get; set; }
        [Display(Name = "CoPI PCF")]
        public Nullable<decimal>[] CoPIPCF { get; set; }
        [Display(Name = "CoPI RMF")]
        public Nullable<decimal>[] CoPIRMF { get; set; }
        public Nullable<Decimal> OverheadPercentage { get; set; }
        public Nullable<int> NoOfEMI { get; set; }
        public Nullable<Decimal>[] ArrayEMIValue { get; set; }
        public bool IsYearWiseAllocation { get; set; }
        public List<YearWiseHead> YearWiseHead { get; set; }
        [NotMapped]
        public bool ConsFundTypeForgnAndBoth
        {
            get
            {
                return (this.ConsProjectFundingType_Qust_1 == 2 || this.ConsProjectFundingType_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool TaxException
        {
            get
            {
                return (this.ConsProjectTaxType_Qust_1 == 4 || this.ConsProjectTaxType_Qust_1 == 2 || this.ConsProjectTaxType_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool IndTaxService
        {
            get
            {
                return (this.constaxservice == 1 || this.constaxservice == 2);
            }
        }
        [NotMapped]
        public bool GovAndBoth
        {
            get
            {
                return (this.ProjectFundedby_Qust_1 == 1 || this.ProjectFundedby_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool ForgnGovAndBoth
        {
            get
            {
                return (this.ForgnProjectFundedby_Qust_1 == 1 || this.ForgnProjectFundedby_Qust_1 == 3);
            }
        }

        [NotMapped]
        public bool ForgnNonGovAndBoth
        {
            get
            {
                return (this.ForgnProjectFundedby_Qust_1 == 2 || this.ForgnProjectFundedby_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool NonGovAndBoth
        {
            get
            {
                return (this.ProjectFundedby_Qust_1 == 2 || this.ProjectFundedby_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool IndAndBoth
        {
            get
            {
                return (this.ProjectFundingType_Qust_1 == 1 || this.ProjectFundingType_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool ForgnAndBoth
        {
            get
            {
                return (this.ProjectFundingType_Qust_1 == 2 || this.ProjectFundingType_Qust_1 == 3);
            }
        }
        [NotMapped]
        public bool IndGovFund_MHRD
        {
            get
            {
                return (this.IndProjectFundingGovtBody_Qust_1 != null && this.IndProjectFundingGovtBody_Qust_1.Contains(1));
            }
        }
        [NotMapped]
        public bool IndGovFund_Mnstry
        {
            get
            {
                return (this.IndProjectFundingGovtBody_Qust_1 != null && this.IndProjectFundingGovtBody_Qust_1.Contains(2));
            }
        }
        [NotMapped]
        public bool IndGovFund_Univ
        {
            get
            {
                return (this.IndProjectFundingGovtBody_Qust_1 != null && this.IndProjectFundingGovtBody_Qust_1.Contains(3));
            }
        }
        [NotMapped]
        public bool IndNonGovFund_Indus
        {
            get
            {
                return (this.IndProjectFundingNonGovtBody_Qust_1 != null && this.IndProjectFundingNonGovtBody_Qust_1.Contains(1));
            }
        }
        [NotMapped]
        public bool IndNonGovFund_Univ
        {
            get
            {
                return (this.IndProjectFundingNonGovtBody_Qust_1 != null && this.IndProjectFundingNonGovtBody_Qust_1.Contains(2));
            }
        }
        [NotMapped]
        public bool IndNonGovFund_Other
        {
            get
            {
                return (this.IndProjectFundingNonGovtBody_Qust_1 != null && this.IndProjectFundingNonGovtBody_Qust_1.Contains(3));
            }
        }
        [NotMapped]
        public bool ForgnGovFund_Dep
        {
            get
            {
                return (this.ForgnProjectFundingGovtBody_Qust_1 != null && this.ForgnProjectFundingGovtBody_Qust_1.Contains(1));
            }
        }
        [NotMapped]
        public bool ForgnGovFund_Univ
        {
            get
            {
                return (this.ForgnProjectFundingGovtBody_Qust_1 != null && this.ForgnProjectFundingGovtBody_Qust_1.Contains(2));
            }
        }
        [NotMapped]
        public bool ForgnGovFund_Other
        {
            get
            {
                return (this.ForgnProjectFundingGovtBody_Qust_1 != null && this.ForgnProjectFundingGovtBody_Qust_1.Contains(3));
            }
        }
        [NotMapped]
        public bool ForgnNonGovFund_Dep
        {
            get
            {
                return (this.ForgnProjectFundingNonGovtBody_Qust_1 != null && this.ForgnProjectFundingNonGovtBody_Qust_1.Contains(1));
            }
        }
        [NotMapped]
        public bool ForgnNonGovFund_Univ
        {
            get
            {
                return (this.ForgnProjectFundingNonGovtBody_Qust_1 != null && this.ForgnProjectFundingNonGovtBody_Qust_1.Contains(2));
            }
        }
        [NotMapped]
        public bool ForgnNonGovFund_Other
        {
            get
            {
                return (this.ForgnProjectFundingNonGovtBody_Qust_1 != null && this.ForgnProjectFundingNonGovtBody_Qust_1.Contains(3));
            }
        }

        public bool IsSubProject { get; set; }
        [RequiredIf("IsSubProject", true, ErrorMessage = "Main project field is required")]
        public Nullable<Int32> MainProjectId { get; set; }
    }
    public class ProjectEnhanceandExtenDetailsModel
    {
        public int Sno { get; set; }
        public int ProjectID { get; set; }
        public int[] ProjectEnhancementID { get; set; }
        public int[] ProjectExtensionID { get; set; }
        [RequiredIf("Enhancement_Qust_1", "Yes", ErrorMessage = "Enhanced sanction Value field is required")]
        public string[] EnhanceRefNumber { get; set; }
        public string[] ExtenRefNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Projecttitle { get; set; }
        public string[] EnhancedDate { get; set; }
        public string[] ExtendedDate { get; set; }
        public string[] PrsntDueDate { get; set; }
        public string[] ExtndDueDate { get; set; }
        public Nullable<decimal>[] OldSanctionValue { get; set; }
        public Nullable<decimal>[] EnhancedSanctionValue { get; set; }
        public string[] Enhancedocname { get; set; }
        public string[] Extendocname { get; set; }
        public string[] EnhancedocPath { get; set; }
        public string[] ExtendocPath { get; set; }
    }
    public class YearWiseHead
    {
        public Nullable<int>[] AllocationHeadYW { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<Decimal>[] AllocationValueYW { get; set; }
        public Nullable<int> NoOfInstallment { get; set; }
        public Nullable<Decimal> EMIValueForYear { get; set; }
        public Nullable<Decimal>[] EMIValue { get; set; }
    }

    public class ProjectSearchResultModel
    {
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> Sno { get; set; }
        public string Projecttitle { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<Decimal> Budget { get; set; }
        [Display(Name ="Agency")]
        public Nullable<int> SponsoringAgency { get; set; }
        public string SponsoringAgencyName { get; set; }
        public string NameofPI { get; set; }
        public string PIDepartmentName { get; set; }
        public string EmpCode { get; set; }
        public string PrpsalApprovedDate { get; set; }
    }
    public class ProjectSearchFieldModel
    {

        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> PIName { get; set; }

        public string ProjectNumber { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromSODate { get; set; }
        public Nullable<DateTime> ToSODate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public string ProjectTitle { get; set; }
        public string AgencyName { get; set; }
        public string NameOfPI { get; set; }
        public string PICode { get; set; }
        public Nullable<DateTime> ApproveFromDate { get; set; }
        public Nullable<DateTime> ApproveTODate { get; set; }
        public Nullable<decimal> BudgetValue { get; set; }
        public string EFProjectNumber { get; set; }
    }
    public class ProjectPredicate
    {
        public tblProject prj { get; set; }
        public vwFacultyStaffDetails u { get; set; }
        public tblAgencyMaster agy { get; set; }
    }
    public class ProjectClosingModel
    {
        public int Sno { get; set; }
        public int ProjectID { get; set; }
        public string Projecttitle { get; set; }
        public int PI { get; set; }
        public string PIname { get; set; }
        public string SelectProject { get; set; }
        public Nullable<int> Projecttype { get; set; }
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> PresentDueDate { get; set; }
        public string PrsntDueDate { get; set; }
        public int ProjectStatus { get; set; }
        public int CrtdUserid { get; set; }
        public DateTime Crtd_TS { get; set; }
        public int UpdtUserid { get; set; }
        public DateTime Updt_TS { get; set; }
        public int Action { get; set; }
    }
    public class EditProjectModel
    {
        public int ProposalID { get; set; }
        public string Projecttitle { get; set; }
        [Required]
        [Display(Name = "Type of Project")]
        public Nullable<int> selProjectType { get; set; }
        public string ProjectType { get; set; }
        public int PIname { get; set; }
        public string NameofPI { get; set; }
        public string ProposalPhase { get; set; }
        public int ProposalupdtID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }       
        [StringLength(25)]
        public string Mobile { get; set; }
        public string[] DocPath { get; set; }
        public Nullable<int>[] DocType { get; set; }
        public string[] DocDescrip { get; set; }
        public string[] DocName { get; set; }
        public string[] DocCrtdUserid { get; set; }
        public Nullable<DateTime>[] DocCrtd_TS { get; set; }

        public HttpPostedFileBase[] file { get; set; }
        public List<UploadDocumentDetailsList> Upload { get; set; }
        public Nullable<int> seltypeIndstryname { get; set; }
        [Display(Name = "Ministry")]
        public Nullable<int> selMinistry { get; set; }
        public Nullable<int> selDomain { get; set; }
        public string MHRD { get; set; }
        [Required]
        public Nullable<int> Department { get; set; }
        public string Departmentname { get; set; }
        [Required]
        public Nullable<Int32> Institute { get; set; }
        public string Institutename { get; set; }
        //[Required]
        //[DataType(DataType.DateTime)]
        //public Nullable<DateTime> Startdate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> Enddate { get; set; }
        public string DocPathname { get; set; }
        public Nullable<int> DocTypename { get; set; }
        public string DocDescripname { get; set; }

    }

    public class ProjectEnhancementModel
    {
        public int Sno { get; set; }
        public int ProjectID { get; set; }
        public int ProjectEnhancementID { get; set; }
        //[RequiredIf("Enhancement_Qust_1", "Yes", ErrorMessage = "Enhanced sanction Value field is required")]
        public string DocumentReferenceNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Projecttitle { get; set; }
        public string SelectProject { get; set; }
        public Nullable <int> Projecttype { get; set; }       
        public string PIname { get; set; }
        
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> PresentDueDate { get; set; }
        public string PrsntDueDate { get; set; }
       
        [DataType(DataType.DateTime)]
        [RequiredIf("Extension_Qust_1", "Yes", ErrorMessage = "Extended Due Date field is required")]
        public Nullable<DateTime> ExtendedDueDate { get; set; }
        public string ExtndDueDate { get; set; }
        public string Extension_Qust_1 { get; set; }
        public string Enhancement_Qust_1 { get; set; }
        public Nullable<decimal> OldSanctionValue { get; set; }
        
        [RequiredIf("Enhancement_Qust_1", "Yes", ErrorMessage = "Enhanced sanction Value field is required")]
        public Nullable<decimal> EnhancedSanctionValue { get; set; }
        public int CrtdUserid { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        //public HttpPostedFileBase file { get; set; }
        public int[] AllocationId { get; set; }
        public int[] Allochead { get; set; }
        public int[] Allocationhead { get; set; }
        public Nullable<int>[] ProjectEnhancementAllocationId { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        //   [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] OldAllocationvalue { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N}")]
        //   [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] EnhancedAllocationvalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        //   [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] HeadwiseTotalAllocationvalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        //   [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalEnhancedAllocationvalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        //   [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalAllocatedvalue { get; set; }

        public bool isCurrentVersion { get; set; }
    }
    public class ProjectStatusModel
    {
        public List<ListProjectDetails> GetDetails { get; set; }
    }
    public class ListProjectDetails
    {
        public int slNo { get; set; }
        public int ProjectID { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectTittle { get; set; }
        public string SanctionOrderNo { get; set; }
        public string PIName { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
    }

    public class UpdateProjectStatusModel
    {
        public int ProjectID { get; set; }
        public string StatusID { get; set; }
        public string Remarks { get; set; }
        public HttpPostedFileBase file { get; set; }
    }

    public class ProjectViewDetailsModel
    {
        public string ProjectTypeName { get; set; }
        public string TypeOfProject { get; set; }
        public string CoardinatorName { get; set; }
        public string InstituteOrIndustryName { get; set; }
        public string Email { get; set; }
        public decimal OtherInstituteOrIndustryShare { get; set; }
        public decimal IITMShare { get; set; }
        public string PrjAccountType { get; set; }
        public string Sheme { get; set; }
        public string ShemeCode { get; set; }
        public string prjFundingType { get; set; }
        public string prjCatagory { get; set; }
        public string FundedBy { get; set; }
        /*Indian Government funding body*/
        public string IndGevFundingBody { get; set; }
        public string IndGevDepartment { get; set; }
        public string IndGevDepAmount { get; set; }
        public string IndGevMinistry { get; set; }
        public string IndGevMinAmount { get; set; }
        public string IndGevUniversity { get; set; }
        public string IndGevUnivAmount { get; set; }
        public string IndNonGevFundingBody { get; set; }
        public string IndNonGevDepartment { get; set; }
        public string IndNonGevDepAmount { get; set; }
        public string IndNonGevMinistry { get; set; }
        public string IndNonGevMinAmount { get; set; }
        public string IndNonGevUniversity { get; set; }
        public string IndNonGevUnivAmount { get; set; }
        /*Indian Government funding body end*/
        /*Foreign Government funding body*/
        public string ForeignGevFundingBody { get; set; }
        public string ForeignGevDep { get; set; }
        public string ForeignGevDepCountry { get; set; }
        public string ForeignGevDepAmount { get; set; }
        public string ForeignGevUniv { get; set; }
        public string ForeignGevUnivCountry { get; set; }
        public string ForeignGevUnivAmount { get; set; }
        public string ForeignGevAgency { get; set; }
        public string ForeignGevAgencyCountry { get; set; }
        public string ForeignGevAgencyAmount { get; set; }

        public string ForeignNonGevFundingBody { get; set; }
        public string ForeignNonGevDep { get; set; }
        public string ForeignNonGevDepCountry { get; set; }
        public string ForeignNonGevDepAmount { get; set; }
        public string ForeignNonGevUniv { get; set; }
        public string ForeignNonGevUnivCountry { get; set; }
        public string ForeignNonGevUnivAmount { get; set; }
        public string ForeignNonGevAgency { get; set; }
        public string ForeignNonGevAgencyCountry { get; set; }
        public string ForeignNonGevAgencyAmount { get; set; }
        /*Foreign Government funding body end*/
        public string PIDepartment { get; set; }
        public string PIName { get; set; }
        public string PIEmail { get; set; }
        public string ScientistName { get; set; }
        public string ScientistEmai { get; set; }
        public string ScientistMobile { get; set; }
        public string ScientistAddress { get; set; }
        public int TotalNoOfStaff { get; set; }
        public int TotalStaff { get; set; }
        public decimal TotalSalary { get; set; }
        public string JointdevelopmentQuestion { get; set; }
        public string Agency { get; set; }
        public string AgencyCode { get; set; }
        public string AgencyRegName { get; set; }
        public string AgencyRegAddr { get; set; }
        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public string AgencyEmail { get; set; }
        public string ContactNo { get; set; }
        public string ContactAddr { get; set; }
        public string TentativeStartDate { get; set; }
        public string TentativeCloseDate { get; set; }
        public string TentativeDueDate { get; set; }
        public string TaxStatus { get; set; }
        public string GSTIn { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        public string Remarks { get; set; }
        public string YearWiseAllocation { get; set; }
        public ProjectSummaryModel prjSummary { get; set; }           
        public List<CoPiDetailsModel> CoPiDetails { get; set; }
        public List<CoPiDetailsForOtherInstituteModel> CoPiOtherInstitute { get; set; }
        public List<AllocationDetailModel> Allocation { get; set; }
        public List<InstalmentModel> Instalment { get; set; }
        public List<StaffDetailsModel> Staff { get; set; }
        public List<OtherCompanyStaffModel> OtherStaff { get; set; }
        public List<DocumentDetailsModel> DocDetail { get; set; }
        
    }
    public class ProjectSummaryModel
    {
        public string ProjectTittle { get; set; }
        public string ProposalNo { get; set; }
        public string ProjectNo { get; set; }
        public string FinancialYear { get; set; }
        public string ProjectType { get; set; }
        public string ProjectApprovalDate { get; set; }
        public string PIname { get; set; }
        public decimal SanctionedValue { get; set; }
        public string SanctionOrderNo { get; set; }
        public string SanctionOrderDate { get; set; }
        public string ProjectDuration { get; set; }
        public decimal TotalReceipt { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal PreviousCommitment { get; set; }
        public decimal NetBalance { get; set; }
        public decimal BaseValue { get; set; }
        public decimal ApplicableTax { get; set; }
        //public string CommitNo { get; set; }
        public decimal ApprovedNegativeBalance { get; set; }
        public decimal OverHeads { get; set; }
        public decimal GST { get; set; }
        public decimal OpeningBalance { get; set; }
        public bool AllocationNR_f { get; set; }
        public List<HeadWiseDetailModel> HeadWiseAllocation { get; set; }
        public List<HeadWiseDetailModel> HeadWiseCommitment { get; set; }
        public List<HeadWiseDetailModel> HeadWiseSpent { get; set; }

    }

    public class HeadWiseDetailModel
    {
        public int AllocationId { get; set; }
        public string AllocationHeadName { get; set; }
        public decimal Amount { get; set; }
    }

    public class CoPiDetailsModel
    {
        public string PIDepartment { get; set; }
        public string PIName { get; set; }
        public string Email { get; set; }
        public Nullable<int> PIId { get; set; }
    }

    public class CoPiDetailsForOtherInstituteModel
    {
        public string Institute { get; set; }
        public string Department { get; set; }
        public string CoPIName { get; set; }
        public string Remarks { get; set; }

    }

    public class AllocationDetailModel
    {
        public string AllocationHead { get; set; }
        public string AllocationType { get; set; }
        public decimal AllocationValue { get; set; }
    }
    public class InstalmentModel
    {
        public int NoOfInstalment { get; set; }
        public int InstalmentNo { get; set; }
        public decimal InstalmentAmount { get; set; }
     
    }
    public class StaffDetailsModel
    {
        public string Catagory { get; set; }
        public int NoofStaffs { get; set; }
        public decimal Salary { get; set; }
    }
    public class OtherCompanyStaffModel
    {
        public string CompanyName { get; set; }
        public string Remarks { get; set; }       
    }       
    public class DocumentDetailsModel
    {
        public string AttachementType { get; set; }
        public string AttachementName { get; set; }
        public string Attachment { get; set; }
    }
}