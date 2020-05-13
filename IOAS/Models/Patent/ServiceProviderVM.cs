using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Patent
{
    public class ServiceProviderVM
    {
        public int SlNo { get; set; }
        public string AttorneyID { get; set; }
        public string AttorneyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string EmailID { get; set; }
        public string Category { get; set; }
        public string Country { get; set; }
        public string Mobile_No { get; set; }
        public string RangeOfServices { get; set; }
        public bool isUpdate { get; set; }
    }
}