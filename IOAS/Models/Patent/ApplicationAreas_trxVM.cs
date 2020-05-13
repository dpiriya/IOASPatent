using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class ApplicationAreas_trxVM
    {
        public long tranx_id { get; set; }
        public int VersionId { get; set; }
        public int Sno { get; set; }
        public long FileNo { get; set; }
        public int Index { get; set; }
        public string Category { get; set; }
        public string Areas { get; set; }
    }
}