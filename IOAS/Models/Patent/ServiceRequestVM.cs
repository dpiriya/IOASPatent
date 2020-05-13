using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class ServiceRequestVM
    {
        public string SRNo { get; set; }
        public short Sno { get; set; }
        public long FileNo { get; set; }
        public string AttorneyID { get; set; }
        public string SharingParty { get; set; }
        public string Action { get; set; }
        public Nullable<short> Share { get; set; }
        public string MDocNo { get; set; }
        public Nullable<System.DateTime> IntimationDt { get; set; }
        public Nullable<System.DateTime> TargetDt { get; set; }
        public Nullable<System.DateTime> ActualDt { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool isUpdate { get; set; }

    }
}