using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class DisputeActivity
    {
        public string DisputeNo { get; set; }
        public decimal SNo { get; set; }
        //[DataType(DataType.Date,ErrorMessage ="Enter Valid Date")]
        //[DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:dd/MM/yyyy}")]
        public string ActivityDate { get; set; }
        public string Forum { get; set; }
        public string ActivityType { get; set; }
        public string Remarks { get; set; }
        public string FilePath { get; set; }
        public HttpPostedFileBase FileName { get; set; }
        public string fn { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}