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
    
    public partial class tblTravelBillBreakUpDetail
    {
        public int BreakUpDetailId { get; set; }
        public Nullable<int> ExpenseTypeId { get; set; }
        public Nullable<int> ClaimedCurrencySpent { get; set; }
        public Nullable<decimal> ClaimedForexAmt { get; set; }
        public Nullable<decimal> ClaimedConvRate { get; set; }
        public Nullable<decimal> ClaimedTotalAmount { get; set; }
        public Nullable<decimal> ProcessedForexAmt { get; set; }
        public Nullable<decimal> ProcessedConvRate { get; set; }
        public Nullable<decimal> ProcessedTotalAmount { get; set; }
        public Nullable<decimal> DifferenceAmt { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<int> TravelBillDetailId { get; set; }
    }
}
