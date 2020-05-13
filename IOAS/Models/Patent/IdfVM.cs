using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class IdfVM
    {
        public string FileNo { get; set; }
        public string Title { get; set; }
        public string FirstApplicant { get; set; }
        public string InventorType { get; set; }
        public string Inventor1 { get; set; }
        public string Applcn_no { get; set; }
        public string Status { get; set; }
        public int SNo { get; set; }
        public string Department { get; set; }
    }
}