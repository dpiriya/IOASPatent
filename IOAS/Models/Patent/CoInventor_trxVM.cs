using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class CoInventor_trxVM
    {
        public long tranx_id { get; set; }
        public int VersionId { get; set; }
        public long SNo { get; set; }
        public long FileNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Ph { get; set; }
        public string Mail { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }

    }
}