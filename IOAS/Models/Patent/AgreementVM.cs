using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class AgreementVM
    {
        public System.DateTime EntryDt { get; set; }
        public string ContractNo { get; set; }
        public string AgreementType { get; set; }
        public string AgreementNo { get; set; }
        public string Title { get; set; }
        public string Scope { get; set; }
        public string Party { get; set; }
        public string CoordinatingPerson { get; set; }
        public string CoorCode { get; set; }
        public string Dept { get; set; }
        public Nullable<System.DateTime> EffectiveDt { get; set; }
        public Nullable<System.DateTime> ExpiryDt { get; set; }
        public string Remark { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> Request { get; set; }
        public string TechTransfer { get; set; }
        public string Status { get; set; }
        public int SNo { get; set; }
    }
}