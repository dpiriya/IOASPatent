using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
        public class BankMasterViewModel
        {
            public string Bankname { get; set; }
            public string AccountNumber { get; set; }
            public string BranchName { get; set; }
            public string SwiftCode { get; set; }
            public string MICRCode { get; set; }
            public string IFSCCode { get; set; }
            public string BankAddress { get; set; }
        }
        
    
    public class InternalAgencyViewModel
    {
        public int sno { get; set; }
        public Nullable<int> InternalAgencyId { get; set; }
        [Required]
        [Display(Name = "Agency Name")]
        public string InternalAgencyName { get; set; }
        [Required]
        [Display(Name = "Agency Code")]
        public string InternalAgencyCode { get; set; }
        //[Required]
        //[Display(Name = "Contact Person")]
        public string InternalAgencyContactPerson { get; set; }
        //[Required]
        //[DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string InternalConatactEmail { get; set; }
        //[Required]
        //[Display(Name = "Contact Number")]
        public string InternalAgencyContactNumber { get; set; }
        //[Required]
        //[Display(Name = "Agency Address")]
        public string InternalAgencyAddress { get; set; }
        //[Required]
        //[Display(Name = "Agency Register Name")]
        public string InternalAgencyRegisterName { get; set; }
        //[Required]
        //[Display(Name = "Agency Register Address")]
        public string InternalAgencyRegisterAddress { get; set; }
        public string InternalDistrict { get; set; }
        public Nullable<int> InternalPincode { get; set; }
        //[Required]
        //[Display(Name = "State")]
        public string InternalAgencyState { get; set; }
        
        public int InternalAgencyUserId { get; set; }
        public string InternalAgencyType { get; set; }
        public HttpPostedFileBase[] File { get; set; }
        public string[] DocPath { get; set; }
        public int[] DocumentType { get; set; }
        public string[] DocumentName { get; set; }
        public string[] AttachName { get; set; }
        public int[] DocumentId { get; set; }
        public int UserId { get; set; }
        public int ProjectType { get; set; }
        public string SearchAgencyName { get; set; }
        public string SearchAgencyCode { get; set; }
    }
    public class TdsSectionModel
    {
        public string NatureOfIncome { get; set; }
        public decimal Percentage { get; set; }
    }
    public class VendorSearchModel
    {

        public string INVendorSearchname { get; set; }
        public string INVendorsearchCode { get; set; }
        public Nullable<int> EXCountryName { get; set; }
        public string EXVendorSearchname { get; set; }
        public string EXINVendorsearchCode { get; set; }
        public int TotalRecords { get; set; }
        public List<VendorMasterViewModel> VendorList { get; set; }
    }
    public class LedgerOBBalanceModel
    {
        public int AccountCategoryId { get; set; }
        public int FinalYearId { get; set; }
        public string AccountGroupName { get; set; }
        public string AccountHeadName { get; set; }
        public decimal CurrentOpeningBalance { get; set; }
        public decimal PopupCurrentOpeningBalance { get; set; }
        public decimal PopModeifiedOpeningBalance { get; set; }
        public int Userid { get; set; }
        public string Password { get; set; }
        public int HeadOpeningBalanceId { get; set; }
        public int AccountHeadId { get; set; }
        public string Username { get; set; }
        public int sno { get; set; }

    }

}