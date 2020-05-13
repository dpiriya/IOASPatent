using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class IndianPatentVM
    {
        public long FileNo { get; set; }
        public bool isUpdate { get; set; }
        public string Attorney { get; set; }
        public string ApplicationNo { get; set; }
        public Nullable<System.DateTime> FilingDate { get; set; }
        public Nullable<System.DateTime> CompleteFilingDate { get; set; }
        public string PublicationPath { get; set; }
        public string PublicationNo { get; set; }
        public Nullable<System.DateTime> PublicationDate { get; set; }
        public Nullable<bool> FERPlaced { get; set; }
        public Nullable<bool> FERIssued { get; set; }
        public Nullable<System.DateTime> FERIssueDate { get; set; }
        public string Status { get; set; }
        public string SubStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}