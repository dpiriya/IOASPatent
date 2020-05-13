using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class InternationalVM
    {
        public Nullable<System.DateTime> InputDt { get; set; }
        public string FileNo { get; set; }
        public string subFileNo { get; set; }
        public Nullable<System.DateTime> RequestDt { get; set; }
        public string Country { get; set; }
        public string Partner { get; set; }
        public string PartnerNo { get; set; }
        public string Type { get; set; }
        public string Attorney { get; set; }
        public string ApplicationNo { get; set; }       
        public Nullable<System.DateTime> FilingDt { get; set; }
        public string PublicationNo { get; set; }
        public Nullable<System.DateTime> PublicationDt { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public string PatentNo { get; set; }
        public Nullable<System.DateTime> PatentDt { get; set; }
        public string Commercial { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }
        public bool isUpdate { get; set; }


    }
}