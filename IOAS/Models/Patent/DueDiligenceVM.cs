using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class DueDiligenceVM
    {
        public int tranx_id { get; set; }
        public int Sno { get; set; }
        public long FileNo { get; set; }
        public System.DateTime EntryDt { get; set; }
        public string SRNo { get; set; }
        public Nullable<System.DateTime> RequestDt { get; set; }
        public Nullable<System.DateTime> ReportDt { get; set; }
        public string ReportType { get; set; }
        public string Mode { get; set; }
        public string Allocation { get; set; }
        public string Participants { get; set; }
        public string IPCCode { get; set; }
        public string TechnologyAction { get; set; }
        public string Summary { get; set; }
        public string Comment { get; set; }
        public string InventorInput { get; set; }
        public string Followup { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase FileName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool isUpdate { get; set; }        
        public string fn { get; set; }           
    }
}