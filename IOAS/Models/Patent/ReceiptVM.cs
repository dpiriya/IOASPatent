using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class ReceiptVM
    {
        public decimal trans_id { get; set; }
        [Required(ErrorMessage = "Error Connecting DB")]
        public string ReceiptNo { get; set; }
        public Nullable<System.DateTime> ReceiptDt { get; set; }
        [Required(ErrorMessage = "Mandatory Field")]
        public string Source { get; set; }
        public string Accno { get; set; }
        public string Party { get; set; }
        public string PartyRefNo { get; set; }
        public string ReceiptRef { get; set; }
        public string ReceiptDesc { get; set; }
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a number")]
        public Nullable<decimal> AmountINR { get; set; }
        public string IntimationRef { get; set; }
        public Nullable<System.DateTime> IntimationDt { get; set; }
        public string Comment { get; set; }
        public string IPAccno { get; set; }
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a number")]
        public Nullable<decimal> TransferAmt { get; set; }
        public Nullable<System.DateTime> TransferDt { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool isUpdate { get; set; }
        public List<ReceiptSecVM> RDetail {get;set;}
        public ReceiptVM()
        {
            RDetail = new List<ReceiptSecVM>();
        }
    }
}