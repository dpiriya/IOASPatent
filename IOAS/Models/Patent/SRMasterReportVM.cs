using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class SRMasterReportVM
    {
       
        public List<SRReportVM> SR { get; set; }
        public List<IDFRequestVM> IDF { get; set; }
        public List<CoInventorVM> CoIn { get; set; }
        public SRMasterReportVM()
        {
            IDF = new List<IDFRequestVM>();
            CoIn = new List<CoInventorVM>();
            SR = new List<SRReportVM>();
        }
    }
}