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
    
    public partial class tblFellowshipSalaryDetails
    {
        public int FellowshipSalaryDetailsId { get; set; }
        public Nullable<int> FellowshipSalaryId { get; set; }
        public Nullable<int> FellowShipId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> UPDT_By { get; set; }
        public Nullable<System.DateTime> UPDT_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public string Status { get; set; }
        public Nullable<int> CommitmentId { get; set; }
        public string ProjectNo { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<decimal> CommitmentAvaBalance { get; set; }
        public string FellowshipNo { get; set; }
        public string MonthYear { get; set; }
    }
}
