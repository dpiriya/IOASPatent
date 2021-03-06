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
    
    public partial class tblCommitment
    {
        public int CommitmentId { get; set; }
        public Nullable<int> CommitmentType { get; set; }
        public string CommitmentNumber { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string PurchaseOrder { get; set; }
        public Nullable<int> VendorName { get; set; }
        public string ItemDescription { get; set; }
        public Nullable<decimal> CommitmentAmount { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_UserID { get; set; }
        public Nullable<int> UPDT_UserID { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> CommitmentBalance { get; set; }
        public Nullable<int> Purpose { get; set; }
        public Nullable<int> Reference { get; set; }
        public string ReferenceNo { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<decimal> CurrencyRate { get; set; }
        public Nullable<int> Currency { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> EmailDate { get; set; }
        public Nullable<decimal> AmountSpent { get; set; }
        public Nullable<decimal> ReversedAmount { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public Nullable<decimal> BasicPay { get; set; }
        public Nullable<decimal> MedicalAllowance { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public Nullable<bool> IsDeansApproval { get; set; }
        public string DocName { get; set; }
        public Nullable<decimal> AdditionalCharge { get; set; }
        public Nullable<int> FundingBody { get; set; }
        public Nullable<int> SequenceNo { get; set; }
    }
}
