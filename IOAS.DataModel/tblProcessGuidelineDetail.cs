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
    
    public partial class tblProcessGuidelineDetail
    {
        public int ProcessGuidelineDetailId { get; set; }
        public Nullable<int> ProcessGuidelineId { get; set; }
        public string FlowTitle { get; set; }
        public string FlowDescription { get; set; }
        public Nullable<System.DateTime> CreatedTS { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> LastUpdatedTS { get; set; }
        public Nullable<int> LastUpdatedUserId { get; set; }
    }
}
