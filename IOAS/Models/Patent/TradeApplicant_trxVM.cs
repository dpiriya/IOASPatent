using System;

namespace IOAS.Models.Patent
{
    public class TradeApplicant_trxVM
    {
        public int VersionId { get; set; }
        public long Sno { get; set; }
        public long FileNo { get; set; }
        public string Organisation { get; set; }
        public string Country { get; set; }
        public string Jurisdiction { get; set; }
        public string AddressOfService { get; set; }
        public string Nature { get; set; }
        public string LegalStatus { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}