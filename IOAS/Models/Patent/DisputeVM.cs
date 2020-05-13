using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class DisputeVM
    {
        public string DisputeNo { get; set; }
        public string DGroup { get; set; }
        public string DSource { get; set; }
        public string Title { get; set; }
        [RegularExpression("([0-9]*)",ErrorMessage ="Only digits are allowed")]
        public string EstimatedValue { get; set; }
        public string PartyName { get; set; }
        [RegularExpression("([0-9]*)", ErrorMessage = "Only digits are allowed")]
        public string RealizationValue { get; set; }
        public string Coordinator { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool isUpdate { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }        
        public List<AgreementVM> MDOC { get; set; }        
        //public List<IdfVM> Idf { get; set; }
        public List<IDFRequestVM> Idf { get; set; }
        public List<DisputeActivity> activity { get; set; }
        public DisputeVM()
        {
            activity = new List<DisputeActivity>();
            Idf = new List<IDFRequestVM>();
            MDOC = new List<AgreementVM>();
        }
    }
}