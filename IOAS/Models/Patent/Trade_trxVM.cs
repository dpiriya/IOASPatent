using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class Trade_trxVM
    {
        public long trx_id { get; set; }
        public int VersionId { get; set; }
        public long FileNo { get; set; }
        public string Category { get; set; }
        public List<string> Catlist { get; set; }
        public string TMName { get; set; }
        public string Description { get; set; }
        public bool TMImage { get; set; }
        public string Language { get; set; }
        public string Class { get; set; }
        public string TMStatement { get; set; }
        public List<TradeApplicant_trxVM> TAppl { get; set; }
        public Trade_trxVM()
        {
            TAppl = new List<TradeApplicant_trxVM>();
        }
    }
}