using System;

namespace IOAS.Models.Patent
{
    public class CRPublish_trxVM
    {
        public int VersionId { get; set; }
        public Nullable<long> FileNo { get; set; }
        public Nullable<int> Sno { get; set; }
        public string PUName { get; set; }
        public string PUAddress { get; set; }
        public string PUNationality { get; set; }
        public Nullable<int> Year { get; set; }
        public string Country { get; set; }
    }
}