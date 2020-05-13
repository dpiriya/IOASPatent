using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class PatFilesVM
    {
        public long tranx_id { get; set; }
        public long FileNo { get; set; }
        public int DocId { get; set; }
        public string DocPath { get; set; }
        public string DocName { get; set; }
    }
}