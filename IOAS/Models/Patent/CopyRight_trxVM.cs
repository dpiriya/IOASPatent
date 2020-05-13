using System.Collections.Generic;

namespace IOAS.Models.Patent
{
    public class CopyRight_trxVM
    {
        public long trx_id { get; set; }
        public int VersionId { get; set; }
        public long FileNo { get; set; }
        public string Category { get; set; }
        public string Nature { get; set; }
        public string ClassofWork { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public bool isPublished { get; set; }
        public string Details { get; set; }
        public bool isRegistered { get; set; }
        public string Original { get; set; }
        public List<CRAuthor_trxVM> Author { get; set; }
        public List<CRPublish_trxVM> Publish { get; set; }
        public CopyRight_trxVM()
        {
            Author = new List<CRAuthor_trxVM>();
            Publish = new List<CRPublish_trxVM>();
        }
    }
}