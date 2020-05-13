using IOAS.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class SRReportVM
    {

        public string SRNo { get; set; }
        public short Sno { get; set; }
        public long FileNo { get; set; }
        public string AttorneyID { get; set; }
        public string SharingParty { get; set; }
        public string Action { get; set; }
        public short Share { get; set; }
        public string MDocNo { get; set; }
        public System.DateTime IntimationDt { get; set; }
        public System.DateTime TargetDt { get; set; }
        public System.DateTime ActualDt { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public long trx_id { get; set; }
        
       
    }
}