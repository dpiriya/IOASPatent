using System;

namespace IOAS.Models.Patent
{
    public class CRAuthor_trxVM
    {
        public int VersionId { get; set; }
        public long FileNo { get; set; }
        public int SNo { get; set; }
        public string AUName { get; set; }
        public string AUAddress { get; set; }
        public string AUNationality { get; set; }
        public Nullable<bool> isDeceased { get; set; }
        public Nullable<System.DateTime> deceasedDt { get; set; }
        public Nullable<System.DateTime> createdOn { get; set; }
        public string createdBy { get; set; }
    }
}