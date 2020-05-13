using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class ReceiptSecVM
    {
        public decimal trans_id { get; set; }
        public string ReceiptNo { get; set; }
        public int SlNo { get; set; }
        public long FileNo { get; set; }
        public string Title { get; set; }
        public string RGroup { get; set; }
        public Nullable<decimal> SplitAmtInr { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}