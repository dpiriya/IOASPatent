using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class FileCreationVM
    {
        public long tranx_id { get; set; }      
        public long FileNo { get; set; }
        public string PrimaryInventorType { get; set; }
        public string PrimaryInventorName { get; set; }
        public string PIDepartment { get; set; }
        public string PIEmailId { get; set; }
        public string PIContactNo { get; set; }
        public string PIInstId { get; set; }
        public string FirstApplicantName { get; set; }
        public string FirstApplicantOrganisation { get; set; }
        public string FirstApplicantPosition { get; set; }
        public string FirstApplicantAddress { get; set; }
        public string FirstApplicantEmailId { get; set; }
        public string FirstApplicantContactNo { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public List<CoInventor_trxVM> CoIn { get; set; }
        public List<Applicant_trxVM> Appl { get; set; }
       
        public FileCreationVM()
        {
            CoIn = new List<CoInventor_trxVM>();
            Appl = new List<Applicant_trxVM>();
            
        }
    }
}