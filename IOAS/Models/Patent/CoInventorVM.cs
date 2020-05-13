using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class CoInventorVM
    {
        public long SNo { get; set; }
        public long FileNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Dept { get; set; }
        public string Ph { get; set; }
        public string Mail { get; set; }
    }
}