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
    
    public partial class tblConsultancyFundingCategory
    {
        public int ConsultancyFundingCategoryId { get; set; }
        public string ConsultancyFundingCategory { get; set; }
        public Nullable<int> CrtdUserId { get; set; }
        public Nullable<System.DateTime> CrtdTS { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public Nullable<int> LastUpdatedUserId { get; set; }
        public Nullable<System.DateTime> Lastupdated_TS { get; set; }
    }
}
