using CrystalDecisions.CrystalReports.Engine;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models.Patent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    public class PatentController : Controller
    {
        string strServer = ConfigurationManager.AppSettings["ServerName"].ToString();
        string strDatabase = ConfigurationManager.AppSettings["DataBaseName"].ToString();
        string strUserID = ConfigurationManager.AppSettings["UserId"].ToString();
        string strPwd = ConfigurationManager.AppSettings["Password"].ToString();

        // GET: Patent
        #region common
        [HttpPost]
        public JsonResult GetParty(string Prefix, string source)
        {
            List<string> types = null;
            if (source == "Customer")
            {
                types = Patent.GetParty(Prefix);
            }
            else if (source == "Service Provider")
            {
                types = Patent.GetAttorney(Prefix);
            }
            else if (source == "CoApplicant")
            {
                types = Patent.GetApplicant(Prefix);
            }
            else if (source == "Inventor-Faculty")
            {
                var list = Common.GetAutoCompleteProfList(Prefix).Select(m => m.label);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else if (source == "Inventor-Staff")
            {
                var list = Common.GetAutoCompleteStaffList(Prefix).Select(m => m.label);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else if (source == "Inventor-Students")
            {
                var list = Common.GetAutoCompleteStudentList(Prefix).Select(m => m.label);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(types, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region IDFRequestApproval
        public ActionResult IDFRequestList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetIDFRequestList()
        {
            try
            {
                object output = PatentService.GetIDFRequestDetails();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult IDFRequest(Int64 ReqNo, int vid)
        {
            IDFRequest_trxVM model = new IDFRequest_trxVM();
            ViewBag.processGuideLineId = 203;
            model.Annex.ListIndustry = Patent.IndustryList();
            model.Annex.ListIndustry1 = Patent.IndustryList1();
            model.Annex.IITMode = Patent.CommericaliseMode();
            model.Annex.JointMode = Patent.CommericaliseMode1();
            model.VersionId = vid;
            model.FileNo = ReqNo;
            model = PatentService.EditIDFRequest(model);
            model.ListAction = Patent.ActionList();
            model.Annex.ListStage = Patent.StageList();
            model.TMListAction = Patent.TMActionList();
            model.CRListAction = Patent.CRActionList();
            model.Trade.Catlist = Patent.TMCategory();
            return View(model);
        }

        [HttpPost]
        public JsonResult Clarification(string fno, string vid, string rem)
        {
            try
            {
                Int64 fileno = Convert.ToInt64(fno);
                int v = Convert.ToInt32(vid);
                object output = PatentService.UpdateClarification(fileno, v, rem);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private static readonly Object lockObj = new Object();
        [HttpPost]
        public ActionResult POWFInit(long fno, int Vid)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool status = PatentService.POWFInit(fno, userId, Vid);
                    return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult LockEdit(long fno, int vid)
        {
            try
            {
                bool locked = PatentService.LockEditOption(fno, vid);
                return Json(new { locked = locked, msg = !locked ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { locked = false, msg = "Something went wrong" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region IndianPatent
        public ActionResult FileList()
        {
            return View();
        }
        public ActionResult FCBasicDetails(long fno = 0)
        {
            FileCreationVM model = new FileCreationVM();
            ViewBag.pitype = Patent.GetPatentInventorType();
            ViewBag.dept = Patent.DepartmentList();
            ViewBag.fno = Patent.FileNoList();
            model.FileNo = fno;
            model = PatentService.EditFCBasic(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult FCBasicDetails(FileCreationVM fc)
        {
            ViewBag.pitype = Patent.GetPatentInventorType();
            ViewBag.dept = Patent.DepartmentList();
            ViewBag.fno = Patent.ApprovedFileNoList();
            string ufc = PatentService.UpdateFCBasic(fc, User.Identity.Name);
            if (ufc == "Success")
                ViewBag.succMsg = "Basic details has been updated successfully with File Number - " + fc.FileNo + ".";
            else
                ViewBag.errMsg = ufc;

            return View(fc);
        }
        public JsonResult GetFileDetails(long fno)
        {
            var file = Patent.GetFileDetails(fno);
            return Json(file, JsonRequestBehavior.AllowGet);
            //var output = "fail";
            //if (!string.IsNullOrEmpty(m)) output = "success";
            //var ReturnVal = new { result = output, tit = m.Title, owner = m.CoordinatingPerson, stat = m.Status };
            //return Json(ReturnVal, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFileList()
        {
            try
            {
                object output = PatentService.GetFileList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult IndianPatent(long fno = 0)
        {
            IndianPatentVM model = new IndianPatentVM();
            model.FileNo = fno;
            model = PatentService.EditIndianPatent(model, User.Identity.Name);
            if (model.Attorney != null) { model.isUpdate = true; }
            else model.isUpdate = false;
            ViewBag.att = Patent.AttorneyListWOID();
            ViewBag.pubpath = Patent.PublicationPath();
            ViewBag.Stat = Patent.FileStatusList();
            return View(model);
        }
        [HttpPost]
        public ActionResult IndianPatent(IndianPatentVM vm)
        {
            ViewBag.att = Patent.AttorneyListWOID();
            ViewBag.pubpath = Patent.PublicationPath();
            ViewBag.Stat = Patent.FileStatusList();
            IndianPatentVM model = new IndianPatentVM();
            if (vm.isUpdate == false) {
                string Iip = PatentService.InsertIndianPatent(vm, User.Identity.Name);
                if(Iip== "Success")
                    ViewBag.succMsg = "Indian patent has been inserted successfully with FileNo Number - " + model.FileNo + ".";
                else
                    ViewBag.errMsg = Iip;
            }
            else
            {
                string Uip = PatentService.UpdateIndianPatent(vm, User.Identity.Name);
                if(Uip=="Success")
                    ViewBag.succMsg = "Indian patent has been updated successfully with FileNo Number - " + model.FileNo + ".";
                else
                    ViewBag.errMsg = Uip;
            }
            return View("FileList");
        }
        #endregion
        #region NewInternational
        public ActionResult NewInternational(long fno=0)
        {
            NewInternationalVM model = new NewInternationalVM();
            model.FileNo = fno;
            model = PatentService.EditNewInternational(model);
            if (model.NationalPhase.Count>0) { model.isUpdate = true; }
            else model.isUpdate = false;
            ViewBag.PCT = Patent.PCTList();
            ViewBag.att = Patent.AttorneyList();
            return View(model);
        }

        [HttpPost]
        public ActionResult NewInternational(NewInternationalVM vm)
        {
            ViewBag.PCT = Patent.PCTList();
            ViewBag.att = Patent.AttorneyList();
            NewInternationalVM model = new NewInternationalVM();
            if (vm.isUpdate == false)
            {
                string Iip = PatentService.InsertNewInternational(vm, User.Identity.Name);
                if (Iip == "Success")
                {
                    ViewBag.succMsg = "International patent has been inserted successfully with FileNo - " + vm.FileNo + ".";
                }
                else
                {
                    ViewBag.errMsg = Iip;
                }
            }
            else
            {
                string Uip = PatentService.UpdateNewInternational(vm, User.Identity.Name);
                if (Uip == "Success")
                {
                    ViewBag.succMsg = "International patent has been updated successfully with FileNo - " + vm.FileNo + ".";
                }
                else
                {
                    ViewBag.errMsg = Uip;
                }
            }
            return View(vm);
        }
        #endregion
        #region International
        //public ActionResult International(string subFileNo = null)
        //{
        //    ViewBag.fno = Patent.FileNoList();
        //    ViewBag.cntry = Patent.CountryList();
        //    ViewBag.ftype = Patent.GetFileType();
        //    ViewBag.stat = Patent.FileStatusList();
        //    InternationalVM model = new InternationalVM();
        //    if (subFileNo == null)
        //    {
        //        model.isUpdate = false;
        //    }
        //    else
        //    {
        //        model = PatentService.EditInternational(subFileNo);
        //        if (model.Status != null)
        //        {
        //            ViewBag.subst = Patent.FileSubStatusList(model.Status);
        //        }
        //    }
        //    return View(model);
        //}
        //public ActionResult InternationalList()
        //{
        //    return View();
        //}

        //public JsonResult GetInternationalList()
        //{
        //    try
        //    {
        //        object output = PatentService.GetInternationalList();
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public JsonResult GetSubFileNo(string fno)
        {
            string m = Patent.Getsubfileno(fno);
            return Json(m, JsonRequestBehavior.AllowGet);
            //var output = "fail";
            //if (!string.IsNullOrEmpty(m)) output = "success";
            //var ReturnVal = new { result = output, tit = m.Title, owner = m.CoordinatingPerson, stat = m.Status };
            //return Json(ReturnVal, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubStatus(string st)
        {
            List<string> m = Patent.FileSubStatusList(st);
            return Json(m, JsonRequestBehavior.AllowGet);
            //var output = "fail";
            //if (!string.IsNullOrEmpty(m)) output = "success";
            //var ReturnVal = new { result = output, tit = m.Title, owner = m.CoordinatingPerson, stat = m.Status };
            //return Json(ReturnVal, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult International(InternationalVM intvm)
        //{
        //    ViewBag.fno = Patent.FileNoList();
        //    ViewBag.cntry = Patent.CountryList();
        //    ViewBag.ftype = Patent.GetFileType();
        //    ViewBag.stat = Patent.FileStatusList();
        //    InternationalVM model = new InternationalVM();
        //    if (model.isUpdate == true)
        //    {
        //        string Intl = PatentService.UpdateInternational(model, User.Identity.Name);
        //        if (Intl == "Success")
        //            ViewBag.succMsg = "International patent has been updated successfully with SubFileNo Number - " + model.subFileNo + ".";
        //        else
        //            ViewBag.errMsg = Intl;
        //    }
        //    else
        //    {
        //        string Intl = PatentService.InsertInternational(model, User.Identity.Name);
        //        if (Intl == "Success")
        //            ViewBag.succMsg = "International patent has been inserted successfully with SubFileNo Number - " + model.subFileNo + ".";
        //        else
        //            ViewBag.errMsg = Intl;
        //    }
        //    return View("InternationalList");
        //}
        #endregion
        #region Service Provider
        public ActionResult ServiceProvider(string aid = null)
        {
            ViewBag.cat = Patent.AttCategoryList();
            ViewBag.cntry = Patent.CountryList();
            ServiceProviderVM model = new ServiceProviderVM();
            if (aid == null)
            {
                var seqno = Patent.GetAttorneyNo();
                if (seqno != null)
                {
                    model.AttorneyID = seqno;
                }
            }
            else
            {
                model = PatentService.EditServiceProvider(aid);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ServiceProvider(ServiceProviderVM spvm)
        {
            ServiceProviderVM model = new ServiceProviderVM();
            if (model.isUpdate == true)
            {
                string sp = PatentService.UpdateServiceProvider(model, User.Identity.Name);
                if (sp == "Success")
                    ViewBag.succMsg = "Service Provider has been updated successfully with ID - " + model.AttorneyID + ".";
                else
                    ViewBag.errMsg = sp;
            }
            else
            {
                string Usp = PatentService.InsertServiceProvider(model, User.Identity.Name);
                if (Usp == "Success")
                    ViewBag.succMsg = "Service Provider has been inserted successfully with ID - " + model.AttorneyID + ".";
                else
                    ViewBag.errMsg = Usp;
            }
            return View(model);
        }
        public ActionResult ServiceProviderList()
        {
            string user = User.Identity.Name;
            ViewBag.role = Common.GetRoleByUserName(user);
            return View();
        }
        public JsonResult GetServiceProviderList()
        {
            try
            {
                object output = PatentService.GetServiceProviderList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteServiceProvider(string aid)
        {
            string output = PatentService.DeleteServiceProvider(aid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region DueDiligence
        public ActionResult Duediligence(long FileNo = 0, int sno = 0)
        {
            ViewBag.fno = Patent.FileNoList();
            ViewBag.sr = Patent.ServiceRequestList();
            ViewBag.rtype = Patent.DueDiligenceRptType();
            ViewBag.inext = Patent.DueDiligenceMode();
            DueDiligenceVM model = new DueDiligenceVM();
            if (FileNo == 0)
            {
                model.isUpdate = false;
            }
            else
            {
                model = PatentService.EditDueDiligence(FileNo, sno);
            }
            return View(model);
        }
        public JsonResult GetSNo(long fno)
        {
            int m = Patent.Getsno(fno);
            return Json(m, JsonRequestBehavior.AllowGet);
            //var output = "fail";
            //if (!string.IsNullOrEmpty(m)) output = "success";
            //var ReturnVal = new { result = output, tit = m.Title, owner = m.CoordinatingPerson, stat = m.Status };
            //return Json(ReturnVal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Duediligence(DueDiligenceVM ddvm)
        {
            ViewBag.fno = Patent.FileNoList();
            ViewBag.sr = Patent.ServiceRequestList();
            ViewBag.rtype = Patent.DueDiligenceRptType();
            ViewBag.inext = Patent.DueDiligenceMode();
            DueDiligenceVM model = new DueDiligenceVM();            
            if (ddvm.isUpdate == true)
            {
                string dd = PatentService.UpdateDueDiligence(ddvm, User.Identity.Name);
                if (dd == "Success")
                    ViewBag.succMsg = "Due Diligence has been updated successfully with File Number - " + ddvm.FileNo + "with Sno -" + ddvm.Sno;
                else
                    ViewBag.errMsg = dd;
            }
            else
            {
                string dd = PatentService.InsertDueDiligence(ddvm, User.Identity.Name);
                if (dd == "Success")
                    ViewBag.succMsg = "Due Diligence has been inserted successfully with File Number - " + ddvm.FileNo + "with Sno -" + ddvm.Sno;
                else
                    ViewBag.errMsg = dd;
            }
            return View(model);
        }
        public ActionResult DuediligenceList()
        {
            string user = User.Identity.Name;
            ViewBag.role = Common.GetRoleByUserName(user);
            return View();
        }
        public JsonResult GetDuediligenceList()
        {
            try
            {
                object output = PatentService.GetDuediligenceList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteDuediligence(long dno,int sno)
        {
            string output = PatentService.DeleteDuediligence(dno,sno);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region dispute
        public ActionResult Dispute(string DisputeNo = null)
        {
            DisputeVM pmodel = new DisputeVM();
            ViewBag.Group = Patent.GetDisputeGroup();
            ViewBag.Source = Patent.GetSource();
            ViewBag.Coor = Common.getPI();
            ViewBag.state = Patent.GetDisputeStatus();
            ViewBag.agree = Patent.GetMdoc();
            ViewBag.Idf = Patent.GetIdf();
            if (DisputeNo == null)
            {
                pmodel.isUpdate = false;
                var seqno = Convert.ToInt64(Patent.GetDispNo());
                if (seqno > 0)
                {
                    seqno += 1;
                    pmodel.DisputeNo = "DIS" + seqno;
                }
            }
            else
            {
                pmodel = PatentService.EditDispute(DisputeNo);
            }
            return View(pmodel);
        }

        [HttpPost]
        public JsonResult GetMdocDetails(string Mdoc)
        {
            AgreementVM m = Patent.RetreiveMdocDetails(Mdoc);
            var output = "fail";
            if (!string.IsNullOrEmpty(m.ContractNo)) output = "success";
            var ReturnVal = new { result = output, tit = m.Title, owner = m.CoordinatingPerson, stat = m.Status };
            return Json(ReturnVal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIdfDetails(long idf)
        {
            IDFRequestVM m = Patent.RetreiveIdfDetails(idf);
            var output = "fail";
            if (m.FileNo!=0) output = "success";
            var ReturnVal = new { result = output, tit = m.Title, owner = m.PrimaryInventorName, stat = m.Status };
            return Json(ReturnVal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Dispute(DisputeVM model)
        {
            DisputeVM pmodel = new DisputeVM();
            ViewBag.Group = Patent.GetDisputeGroup();
            ViewBag.Coor = Common.getPI();
            ViewBag.state = Patent.GetDisputeStatus();
            ViewBag.agree = Patent.GetMdoc();
            ViewBag.Source = Patent.GetSource();
            ViewBag.Idf = Patent.GetIdf();
            if (model.isUpdate == false)
            {
                var seqno = Convert.ToInt64(Patent.GetDispNo());
                if (seqno > 0)
                {
                    seqno += 1;
                    pmodel.DisputeNo = "DIS" + seqno;
                }
                string IDispute = PatentService.InsertDispute(model, User.Identity.Name);
                if (IDispute == "Success")
                    ViewBag.succMsg = "Dispute has been inserted successfully with Dispute Number - " + model.DisputeNo + ".";
                else
                    ViewBag.errMsg = IDispute;
            }
            else
            {
                string IDispute = PatentService.UpdateDispute(model, User.Identity.Name);
                if (IDispute == "Success")
                    ViewBag.succMsg = "Dispute has been updated successfully with Dispute Number - " + model.DisputeNo + ".";
                else
                    ViewBag.errMsg = IDispute;
            }
            return View(pmodel);
        }
        [HttpGet]
        public ActionResult DisputeList()
        {
            string user = User.Identity.Name;
            ViewBag.role = Common.GetRoleByUserName(user);
            return View();
        }
        [HttpGet]
        public JsonResult GetDisputeList()
        {
            try
            {
                object output = PatentService.GetDisputeDetails();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteDispute(string dno)
        {
            object output = PatentService.DeleteDispute(dno.Trim());
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FileLink(string f)
        {
            string mime = MimeMapping.GetMimeMapping(f);
            return File(f, mime);
        }
        [HttpGet]
        public JsonResult GetMdocList()
        {
            object output = Patent.GetMdocList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetIdfList()
        {
            object output = Patent.GetIdfList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult DeleteActFile(string fn,string dno)
        //{
        //    DisputeVM data = new DisputeVM();
        //    data.Status = "error";
        //    string fp = Patent.GetFilePath(fn,dno);
        //    string f = fp + fn;
        //    if (System.IO.File.Exists(f))
        //    {
        //        System.IO.File.Delete(f);
        //        data.Status = "success";
        //    }
        //    return Json(data.Status, JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region Receipt
        public ActionResult Receipt(string Rno = null)
        {
            ReceiptVM model = new ReceiptVM();
            if (Rno == null)
            {
                model.isUpdate = false;
                var seqno = Patent.GetReceiptNo();
                if (seqno != null)
                {
                    model.ReceiptNo = seqno;
                }
            }
            else
            {
                model = PatentService.EditReceipt(Rno);
            }
            ViewBag.src = Patent.GetReceiptSource();
            ViewBag.partylist = Patent.PartyList();
            ViewBag.IDF = Patent.GetIdf();
            ViewBag.rgrp = Patent.ReceiptGroup();
            return View(model);
        }
        [HttpPost]
        public ActionResult Receipt(ReceiptVM model)
        {
            ViewBag.src = Patent.GetReceiptSource();
            ViewBag.partylist = Patent.PartyList();
            ViewBag.IDF = Patent.GetIdf();
            ViewBag.rgrp = Patent.ReceiptGroup();
            ReceiptVM receipt = new ReceiptVM();
            if (model.isUpdate == false)
            {
                string IReceipt = PatentService.InsertReceipt(model, User.Identity.Name);
                if (IReceipt == "Success")
                    ViewBag.succMsg = "Receipt has been inserted successfully with Receipt Number - " + model.ReceiptNo + ".";
                else
                    ViewBag.errMsg = IReceipt;
            }
            else
            {
                string UReceipt = PatentService.UpdateReceipt(model, User.Identity.Name);
                if (UReceipt == "Success")
                    ViewBag.succMsg = "Receipt has been updated successfully with Receipt Number - " + model.ReceiptNo + ".";
                else
                    ViewBag.errMsg = UReceipt;
            }
            return View(model);
        }
        public ActionResult ReceiptList()
        {
            return View();
        }
        public JsonResult GetReceiptList()
        {
            try
            {
                object output = PatentService.GetReceiptList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region ServiceRequest
        public ActionResult ServiceRequest(string Srno = null)
        {
            ViewBag.att = Patent.AttorneyList();
            ViewBag.IDF = Patent.FileNoList();
            ViewBag.act = Patent.SRActionList();
            ViewBag.party = Patent.PartyList();
            ViewBag.MDOC = Patent.GetMdoc();
            ViewBag.st = Patent.SRStatusList();
            List<ServiceRequestVM> model = new List<ServiceRequestVM>();

            if (Srno == null)
            {
                var seqno = Patent.GetServiceRequestNo();
                if (seqno != null)
                {
                    model.Add(new ServiceRequestVM()
                    {
                        isUpdate = false,
                        SRNo = seqno
                    });
                }
            }
            else
            {
                model = PatentService.EditServiceRequest(Srno);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ServiceRequest(List<ServiceRequestVM> model)
        {
            ViewBag.att = Patent.AttorneyList();
            ViewBag.IDF = Patent.FileNoList();
            ViewBag.act = Patent.SRActionList();
            ViewBag.party = Patent.PartyList();
            ViewBag.MDOC = Patent.GetMdoc();
            ViewBag.st = Patent.SRStatusList();
            List<ServiceRequestVM> receipt = new List<ServiceRequestVM>();
            if (model[0].isUpdate == false)
            {
                string ISR = PatentService.InsertServiceRequest(model, User.Identity.Name);
                if (ISR == "Success")
                    ViewBag.succMsg = "Service Request has been inserted successfully with SR Number - " + model[0].SRNo + ".";
                else
                    ViewBag.errMsg = ISR;
            }
            else
            {
                string USR = PatentService.UpdateServiceRequest(model, User.Identity.Name);
                if (USR == "Success")
                    ViewBag.succMsg = "Service Request has been updated successfully with SR Number - " + model[0].SRNo + ".";
                else
                    ViewBag.errMsg = USR;
            }
            return View(model);
        }
        public ActionResult ServiceRequestList()
        {
            return View();
        }
        public JsonResult GetServiceRequestList()
        {
            try
            {
                object output = PatentService.GetSRList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteServiceRequest(string srno,int sno)
        {
            string output = PatentService.DeleteServiceRequest(srno,sno);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SRReport(string Srno)
        {
            List<SRReportVM> srlist = new List<SRReportVM>();
            List<IDFRequestVM> IdfList = new List<IDFRequestVM>();
            List<CoInventorVM> CoList = new List<CoInventorVM>();
            ReportDocument rpt = new ReportDocument();
            rpt.Load(Server.MapPath("~/CrystalReport/ServiceRequestrpt.rpt"));
            
            for (int i = 0; i < rpt.DataSourceConnections.Count; i++)                
                rpt.DataSourceConnections[i].SetConnection(strServer, strDatabase, strUserID, strPwd);
            srlist = PatentService.SRReport(Srno);
            string att = PatentService.GetSRAttorney(Srno);           
            if(srlist.Count>0)
            {
                IdfList = PatentService.SRIDFReport(srlist[0].FileNo);
                CoList = PatentService.SRCoReport(srlist[0].FileNo);
                rpt.Database.Tables["tbl_trx_servicerequest"].SetDataSource(srlist);
                rpt.Subreports["SRIDF"].Database.Tables["tbl_trx_servicerequest"].SetDataSource(srlist);
                rpt.Subreports["SRIDF"].Database.Tables["tblIDFRequest"].SetDataSource(IdfList);
                rpt.Subreports["SRCoIn"].Database.Tables["tblIDFRequest"].SetDataSource(IdfList);
                rpt.Subreports["SRCoIn"].Database.Tables["tblCoInventor"].SetDataSource(CoList);
                rpt.SetParameterValue("SRNo",Srno);
                rpt.SetParameterValue("Att", att??"");
                //rpt.SetParameterValue("Tit", tit ?? "");
                Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=SR.pdf");
                return File(stream, "application/pdf");
            }
            else
            {
                return RedirectToAction("SRReport", new { message = "No records found for this type of search entry" });
            }
        }
        #endregion
    }
}