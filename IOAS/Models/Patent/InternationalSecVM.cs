using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class InternationalSecVM
    {
        public long FileNo { get; set; }
        public string Country { get; set; }
        public string PCT { get; set; }
        public int SNo { get; set; }
        public string Appln_FilingNo { get; set; }
        public System.DateTime FilingDate { get; set; }
        public Nullable<bool> OfficeAction { get; set; }
        public Nullable<System.DateTime> OfficeActionDate { get; set; }
        public string Attorney { get; set; }
        public Nullable<System.DateTime> PublicationDate { get; set; }
        public string PublicationNo { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}