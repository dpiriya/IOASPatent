using IOAS.DataModel;
using IOAS.Models.Patent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Infrastructure
{
    public class Patent
    {
        #region common
        public static List<string> PartyList()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.CompanyMaster.Select(m => m.CompanyName).ToList();
                return query;
            }
        }
        #endregion
        #region IDFRequest
        public static List<string> GetPatentInventorType()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "PatentIDF" && m.Grouping == "InventorType").Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> DepartmentList()
        {
            using (WFADSEntities pat = new WFADSEntities())
            {
                var query = pat.DepartmentMaster.Select(m => m.DepartmentCode).ToList();
                return query;
            }
        }
        //public static List<string> PINameList(string prefix, string type)
        //{
        //    using (WFADSEntities pat = new WFADSEntities())
        //    {
        //        if (type == "Faculty")
        //        {
        //            var query = pat.Faculty_Details.Where(m => m.EmployeeName.Contains(prefix)).Select(m => m.EmployeeName).ToList();
        //            return query;
        //        }
        //        else if (type == "Institute Staff")
        //        {
        //            var query = pat.Staff_Details.Where(m => m.EmployeeName.Contains(prefix)).Select(m => m.EmployeeName).ToList();
        //            return query;
        //        }
        //        else if (type == "Student")
        //        {
        //            var query = pat.Student_Details.Where(m => m.StudentName.Contains(prefix)).Select(m => m.StudentName).ToList();
        //            return query;
        //        }
        //        else
        //            return null;
        //    }
        //}
        //public static string GetDepartment(string type, string name)
        //{
        //    using (WFADSEntities pat = new WFADSEntities())
        //    {
        //        if (type == "Faculty")
        //        {
        //            var query = pat.Faculty_Details.Where(m => m.EmployeeName == name).Select(m => m.DepartmentCode).FirstOrDefault().Trim();
        //            return query;
        //        }
        //        else if (type == "Institute Staff")
        //        {
        //            var query = pat.Staff_Details.Where(m => m.EmployeeName == name).Select(m => m.DepartmentCode).FirstOrDefault().Trim();
        //            return query;
        //        }
        //        else if (type == "Student")
        //        {
        //            var query = pat.Student_Details.Where(m => m.StudentName == name).Select(m => m.DepartmentCode).FirstOrDefault().Trim();
        //            return query;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}
        public static List<string> ActionList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Patent" && m.Grouping == "RequestedAction").Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> TMActionList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Trademark" && m.Grouping == "RequestedAction").Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> CRActionList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Copyright" && m.Grouping == "RequestedAction").Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> TMCategory()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Trademark" && m.Grouping == "Category").Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> StageList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "AnexB" && m.Grouping == "DevelopmentStage").Select(m => (m.ItemText + "-" + m.ItemList)).ToList();
                return query;
            }
        }
        public static List<SelectListItem> IndustryList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "AnexB" && m.Grouping == "ApplicationIndustry").Select(m => new SelectListItem { Value = m.ItemList, Text = m.ItemText }).ToList();
                return query;
            }
        }
        public static List<SelectListItem> IndustryList1()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "AnexB" && m.Grouping == "SubIndustry").Select(m => new SelectListItem { Value = m.ItemList, Text = m.ItemText }).ToList();
                return query;
            }
        }
        public static List<SelectListItem> CommericaliseMode()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "AnexB" && m.Grouping == "CommercialisationModeIIT").Select(m => new SelectListItem { Value = m.ItemList, Text = m.ItemText }).ToList();
                return query;
            }
        }
        public static List<SelectListItem> CommericaliseMode1()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "AnexB" && m.Grouping == "CommercialisationModeJoint").Select(m => new SelectListItem { Value = m.ItemList, Text = m.ItemText }).ToList();
                return query;
            }
        }
        #endregion
        #region File
        public static List<string> GetFileType()
        {            
            using (var pat = new PatentNewEntities())
            {
                var query = pat.IPR_Category.Select(m => m.IPRNAME).ToList();
                return query;
            }
        }
        public static List<long> FileNoList() //only indian
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tblIDFRequest.OrderByDescending(m=>m.FileNo).Select(m => m.FileNo).Distinct().ToList();
                return query;
            }
        }
        public static List<string> CountryList() 
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.IPCountry.Select(m => m.Country).Distinct().ToList();
                return query;
            }
        }
        public static List<string> AttorneyList()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.Attorney.Select(m =>m.AttorneyID+" - "+m.AttorneyName).Distinct().ToList();
                return query;
            }
        }
        public static List<string> AttorneyListWOID()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.Attorney.Select(m =>m.AttorneyName).Distinct().ToList();
                return query;
            }
        }
        public static List<string> PCTList()
        {
            List<string> list = new List<string>();
            list.Add("");
            list.Add("PCT");
            list.Add("NONPCT");
            return list;
        }
        public static List<string> FileStatusList()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m=>m.Category== "Status" && m.Grouping=="File").OrderBy(m=>m.SlNo).Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> FileSubStatusList(string st)
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Sub Status" && m.Grouping == st).OrderBy(m => m.SlNo).Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<long> GetIdf() //both indian and international
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tblIDFRequest.Select(m => m.FileNo).Distinct().ToList();
                return query;
            }
        }
        public static string Getsubfileno(string fno)
        {
            using (var pat = new PatentNewEntities())
            {

                int totalfno= pat.International.Where(m => m.FileNo == fno).Count();
                
                if (totalfno.Equals(null))
                {
                    totalfno = 0;
                }
                char[] subFile = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
                string subFileno = fno + "-" + subFile[totalfno];
                return subFileno;
            }
        }

        public static List<long> ApprovedFileNoList()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tblIDFRequest.Where(m=>m.Status=="Dean Approved").Select(m => m.FileNo).Distinct().ToList();
                return query;
            }
        }
        public static tblIDFRequest GetFileDetails (long fno)
        {
            using (var pat = new PatentNewEntities())
            {

                var idf = pat.tblIDFRequest.FirstOrDefault(m => m.FileNo == fno);                
                return idf;
            }
        }

        public static List<string> PublicationPath()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "IndianPatent" && m.Grouping== "PublicationPath").OrderBy(m=>m.SlNo).Select(m => m.ItemList).Distinct().ToList();
                return query;
            }
        }
        #endregion
        #region ServiceProvider
        public static List<string> AttCategoryList()
        {           
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Attorney" && m.Grouping == "Category").OrderBy(m => m.ItemList).Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static string GetAttorneyNo()
        {
            using (var pat = new PatentNewEntities())
            {
                int query = pat.Attorney.Select(m => m.SlNo).Max();
                query += 1;
                return "A" + query.ToString();
            }
        }
        #endregion
        #region Dispute
        public static string GetDispNo()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tbl_trx_dispute.Select(m => m.DisputeNo.Substring(3)).Max();                
                return query;
            }
        }
        public static List<string> GetDisputeGroup()
        {
            List<string> gp = new List<string>();
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Dispute" && m.Grouping == "DisputeGroup").OrderBy(m => m.ItemList).Select(m=>m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> GetParty(string prefix)
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.CompanyMaster.Where(m => m.CompanyName.Contains(prefix)).Select(m => m.CompanyName).Distinct().ToList();
                return query; 
            }
        }

        public static List<string> GetAttorney(string prefix)
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.Attorney.Where(m => m.AttorneyName.Contains(prefix)).Select(m => m.AttorneyName).Distinct().ToList();
                return query;
            }
        }
        public static List<string> GetApplicant(string prefix)
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tblIDFRequest.Where(m => m.PrimaryInventorName.Contains(prefix)).Select(m => m.PrimaryInventorName).Distinct().Union(pat.tblCoInventor.Where(m=>m.Name.Contains(prefix)).Select(m=>m.Name).Distinct()).ToList();
                return query;
            }
        }

        public static List<string> GetDisputeStatus()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Dispute" && m.Grouping == "Status").OrderBy(m => m.ItemList).Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> GetMdoc()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.Agreement.Select(m=>m.ContractNo).Distinct().ToList();
                return query;
            }
        }
        public static List<AgreementVM> GetMdocList()
        {
            List<AgreementVM> mdoc = new List<AgreementVM>();
            using (var pat = new PatentNewEntities())
            {
                var query = pat.Agreement.Select(m=>new AgreementVM {ContractNo=m.ContractNo,AgreementType=m.AgreementType,CoordinatingPerson=m.CoordinatingPerson,Party=m.Party }).ToList();
                if(query.Count>0)
                {
                    for(int i=0;i<query.Count;i++)
                    {
                        mdoc.Add(new AgreementVM()
                        {
                            SNo = i + 1,
                            ContractNo=query[i].ContractNo,
                            AgreementType=query[i].AgreementType,
                            CoordinatingPerson=query[i].CoordinatingPerson,
                            Party=query[i].Party
                        });
                    }
                }
                return mdoc;
            }
        }
        public static List<IDFRequestVM> GetIdfList()
        {
            List<IDFRequestVM> idf = new List<IDFRequestVM>();
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tblIDFRequest.Select(m => new IDFRequestVM {FileNo=m.FileNo,PrimaryInventorName=m.PrimaryInventorName,Title=m.Title,PIDepartment=m.PIDepartment}).ToList();
                if (query.Count > 0)
                {
                    for(int i=0;i<query.Count;i++)
                    {
                        idf.Add(new IDFRequestVM()
                        {
                            //SNo = i + 1,
                            FileNo=query[i].FileNo,
                            PrimaryInventorName=query[i].PrimaryInventorName,
                            Title=query[i].Title,
                            PIDepartment=query[i].PIDepartment
                        });
                    }
                }
                return idf;
            }
        }
        public static List<string> GetSource()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Dispute" && m.Grouping == "Source").OrderBy(m => m.SlNo).Select(m => m.ItemList).ToList();
                return query;
            }
        }
      
        public static AgreementVM RetreiveMdocDetails(string mdoc)
        {
            using (var pat = new PatentNewEntities())
            {               
                var query = pat.Agreement.Where(m=>m.ContractNo==mdoc).Select(m => new AgreementVM {ContractNo=m.ContractNo, Title = m.Title, CoordinatingPerson = m.CoordinatingPerson, Status = m.Status }).FirstOrDefault();
                return query;
            }
        }
        public static IDFRequestVM RetreiveIdfDetails(long idf)
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tblIDFRequest.Where(m => m.FileNo == idf).Select(m => new IDFRequestVM { FileNo = m.FileNo, Title = m.Title,PrimaryInventorName = m.PrimaryInventorName, Status = m.Status }).FirstOrDefault();
                return query;
            }
        }
        //public static string GetFilePath(string fn,string dno)
        //{
        //    using (var pat = new PatentNewEntities())
        //    {
        //        var fp = pat.tbl_trx_DisputeActivity.Where(m =>m.DisputeNo==dno && m.FileName == fn).Select(m => m.FilePath).FirstOrDefault();
        //        return fp;
        //    }
        //}
        #endregion
        #region DueDiligence
        public static List<string> ServiceRequestList()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.tbl_trx_servicerequest.Select(m => m.SRNo).Distinct().ToList();
                return query;
            }
        }
        public static List<string> DueDiligenceRptType()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m=>m.Category== "DueDiligence" && m.Grouping== "ReportType").Select(m => m.ItemList).Distinct().ToList();
                return query;
            }
        }
        public static List<string> DueDiligenceMode()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "DueDiligence" && m.Grouping == "Mode").Select(m => m.ItemList).Distinct().ToList();
                return query;
            }
        }
        public static int Getsno(long fno)
        {
            using (var pat = new PatentNewEntities())
            {

                int totalfno = pat.tbl_trx_duediligence.Where(m => m.FileNo == fno).Count();

                if (totalfno.Equals(null))
                {
                    totalfno = 1;
                }
                else
                    totalfno = totalfno + 1;
                return totalfno;
            }
        }
        #endregion
        #region Receipt
        public static string GetReceiptNo()
        {
            using (var pat = new PatentNewEntities())
            {
                string fy = "RC " + (DateTime.Today.Month >= 4 ? (DateTime.Today.ToString("yyyy") + "-" + DateTime.Today.AddYears(1).ToString("yy")) : (DateTime.Today.AddYears(-1).ToString("yyyy") + "-" + DateTime.Today.ToString("yy")));                
                int c = fy.Length+1;
                var q = pat.tbl_primary_receipt.Where(m=>m.ReceiptNo.Contains(fy)).Select(m => m.ReceiptNo.Substring(c)).ToList();
                var max = q.Select(int.Parse).Max();
                max += 1;   
                fy = fy +"/"+ max;
                return fy;
            }
        }
        public static List<string> GetReceiptSource()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Source" && m.Grouping == "Receipt").OrderBy(m => m.SlNo).Select(m => m.ItemList).ToList();
                return query;
            }
        }
        public static List<string> ReceiptGroup()
        {
            using (var pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Group" && m.Grouping == "Receipt").OrderBy(m => m.SlNo).Select(m => m.ItemList).ToList();
                return query;
            }
        }

        #endregion
        #region 
        public static string GetServiceRequestNo()
        {
            using (var pat = new PatentNewEntities())
            {
                string fy = "S " + (DateTime.Today.Month >= 4 ? (DateTime.Today.ToString("yyyy") + "-" + DateTime.Today.AddYears(1).ToString("yy")) : (DateTime.Today.AddYears(-1).ToString("yyyy") + "-" + DateTime.Today.ToString("yy")));
                int c = fy.Length + 1;
                var q = pat.tbl_trx_servicerequest.Where(m=>m.SRNo.StartsWith(fy)).Select(m => m.SRNo.Substring(c)).ToList();
                var max = q.Select(int.Parse).Max();
                max += 1;
                fy = fy + "/" + max;
                return fy;
            }
        }
        public static List<string> SRActionList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Action" && m.Grouping == "Service Request").Select(m => m.ItemList).Union(pat.IPCountry.Select(m=>m.Country).Distinct()).ToList();
                return query;
            }
        }
        public static List<string> SRStatusList()
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                var query = pat.ListItemMaster.Where(m => m.Category == "Status" && m.Grouping == "Service Request").Select(m => m.ItemList).ToList();
                return query;
            }
        }
        #endregion
    }
}