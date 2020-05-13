using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class NewInternationalVM
    {
        public long FileNo { get; set; }
        public string Attorney { get; set; }
        public bool isUpdate { get; set; }
        public bool isPCT { get; set; }      
        public string PCTFilingNo { get; set; }
        public string PCTPublicationNo { get; set; }
        public Nullable<System.DateTime> PublicationDate { get; set; }
        public List<InternationalSecVM> NationalPhase { get; set; }
        public NewInternationalVM()
        {
            NationalPhase = new List<InternationalSecVM>();
        }
    }
}