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
    
    public partial class tblTDSIncomeTaxDetails
    {
        public int TDSIncomeTaxDetailsId { get; set; }
        public Nullable<int> tblTDSPaymentId { get; set; }
        public Nullable<int> BOATransationId { get; set; }
        public string Party { get; set; }
        public string PAN { get; set; }
        public string ReferenceNo { get; set; }
        public Nullable<System.DateTime> DateOfTransaction { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public string Status { get; set; }
        public Nullable<int> SubLedgerId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
    }
}
