using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class IDFRequest_trxVM
    {
        public long tranx_id { get; set; }
        public int VersionId { get; set; }
        public long FileNo { get; set; }
        public string IDFType { get; set; }
        public string PrimaryInventorType { get; set; }
        public string PrimaryInventorName { get; set; }
        public string PIDepartment { get; set; }
        public string PIEmailId { get; set; }
        public string PIInstId { get; set; }
        public string PIContactNo { get; set; }
        public string FirstApplicantName { get; set; }
        public string FirstApplicantOrganisation { get; set; }
        public string FirstApplicantPosition { get; set; }
        public string FirstApplicantAddress { get; set; }
        public string FirstApplicantEmailId { get; set; }
        public string FirstApplicantContactNo { get; set; }
        public string Title { get; set; }
        public string FieldOfInvention { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string PriorPublication { get; set; }
        public string SupportInformation { get; set; }
        public Nullable<bool> SourceOfInvention { get; set; }
        public string Disclosure { get; set; }
        public Nullable<bool> BiologicalMaterial { get; set; }
        public string DetailsOfBiologicalMaterial { get; set; }
        public string RelevantInformation { get; set; }
        public string RequestedAction { get; set; }
        public string RequestedCRAction { get; set; }
        public string RequestedTMAction { get; set; }
        public string RequestedtxtAction { get; set; }
        public string RequestedCRtxtAction { get; set; }
        public string RequestedActionOthers { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }      
        public List<string> TMListAction { get; set; }
        public List<string> CRListAction { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }

        public List<string> ListAction { get; set; }
        public List<CoInventor_trxVM> CoIn { get; set; }
        public List<Applicant_trxVM> Appl { get; set; }
        public AnnexureB1_trxVM Annex { get; set; }
        public Trade_trxVM Trade { get; set; }
        public CopyRight_trxVM CR { get; set; }
        public List<PatFilesVM> Files { get; set; }

        public IDFRequest_trxVM()
        {
            CoIn = new List<CoInventor_trxVM>();
            Appl = new List<Applicant_trxVM>();
            Annex = new AnnexureB1_trxVM();
            Files = new List<PatFilesVM>();
            Trade = new Trade_trxVM();
            CR = new CopyRight_trxVM();
        }
    }
}