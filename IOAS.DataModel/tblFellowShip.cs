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
    
    public partial class tblFellowShip
    {
        public int FellowShipId { get; set; }
        public string FellowShipNumber { get; set; }
        public string ProjectNo { get; set; }
        public Nullable<int> PayeeId { get; set; }
        public Nullable<System.DateTime> DurationFromdate { get; set; }
        public Nullable<System.DateTime> DurationTodate { get; set; }
        public Nullable<decimal> FellowShipValue { get; set; }
        public string CommitmentNo { get; set; }
        public Nullable<decimal> Balancevalue { get; set; }
        public Nullable<System.DateTime> CRTD_TS { get; set; }
        public Nullable<int> CRTD_By { get; set; }
        public Nullable<System.DateTime> UPTD_TS { get; set; }
        public Nullable<int> UPTD_By { get; set; }
        public string Status { get; set; }
        public Nullable<int> ProjectId { get; set; }
    }
}
