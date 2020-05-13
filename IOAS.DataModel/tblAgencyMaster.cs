//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IOAS.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAgencyMaster
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public Nullable<int> Country { get; set; }
        public Nullable<System.DateTime> Crtd_TS { get; set; }
        public Nullable<int> Crtd_UserId { get; set; }
        public Nullable<int> LastupdatedUserid { get; set; }
        public Nullable<System.DateTime> Lastupdate_TS { get; set; }
        public string Reason { get; set; }
        public string AgencyCode { get; set; }
        public Nullable<int> AgencyType { get; set; }
        public Nullable<int> Scheme { get; set; }
        public string Status { get; set; }
        public string GSTIN { get; set; }
        public string TAN { get; set; }
        public string PAN { get; set; }
        public Nullable<int> StateId { get; set; }
        public string StateCode { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string SwiftCode { get; set; }
        public string MICRCode { get; set; }
        public string IFSCCode { get; set; }
        public string BankAddress { get; set; }
        public string District { get; set; }
        public Nullable<int> PinCode { get; set; }
        public Nullable<int> AgencyCountryCategoryId { get; set; }
        public Nullable<int> IndianAgencyCategoryId { get; set; }
        public Nullable<int> NonSezCategoryId { get; set; }
        public string AgencyRegisterName { get; set; }
        public string AgencyRegisterAddress { get; set; }
        public Nullable<int> SeqNbr { get; set; }
        public Nullable<int> ProjectTypeId { get; set; }
        public Nullable<int> CompanyType { get; set; }
        public Nullable<int> GovermentAgencyType { get; set; }
    }
}
