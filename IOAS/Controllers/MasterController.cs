using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Drawing;
using System.IO;

namespace IOAS.Controllers
{
    public class MasterController : Controller
    {
        // GET: Master
        [Authorized]
        [HttpGet]
        public ActionResult InternalAgency()
        {
            ViewBag.project = Common.getprojecttype();
            ViewBag.agencydoc = Common.GetDocTypeList(19);
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult InternalAgency(InternalAgencyViewModel model,HttpPostedFileBase File)
        {
            ViewBag.project = Common.getprojecttype();
            ViewBag.agencydoc = Common.GetDocTypeList(19);
            var Username = User.Identity.Name;
            model.InternalAgencyUserId = Common.GetUserid(Username);
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
            if (ModelState.IsValid)
            {
                if ((model.AttachName[0] != null && model.AttachName[0] != ""))
                {
                    for (int i = 0; i < model.DocumentType.Length; i++)
                    {
                        if (model.File[i] != null)
                        {
                            string docname = Path.GetFileName(model.File[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View();
                            }
                        }

                    }
                }
                int internalstatus = MasterService.InternalAgency(model);
                if (internalstatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (internalstatus == 2)
                    ViewBag.Msg = "This agency name already exits";
                else if (internalstatus == 3)
                    ViewBag.update = "Agency updated successfully";
                else if (internalstatus == 4)
                    ViewBag.intalcode = "Internal agency Code already exits";
                else
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                return View();
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                   .SelectMany(x => x.Errors)
                                   .Select(x => x.ErrorMessage));

                ViewBag.error = messages;

                return View();
            }
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetInternalAgency(InternalAgencyViewModel model)
        {
            object output = MasterService.GetInternalAgency(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetInternalAgencycode()
        {
            object output = MasterService.InternalAgencyCode();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditInternalAgency(int agencyId)
        {
            object output = MasterService.EditInternalAgency(agencyId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public ActionResult DeleteInternalAgency(int agencyId)
        {
            string Username = User.Identity.Name;
            object output = MasterService.DeleteInternalAgency(agencyId, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {

                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.GetFileData(Server.MapPath(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }
        [Authorized]
        [HttpGet]
        public ActionResult Vendor()
        {
            ViewBag.vendorCountry = Common.GetAgencyType();
            ViewBag.state = Common.GetStatelist();
            ViewBag.country = Common.getCountryList();
            ViewBag.GstDoc = Common.GetGstSupportingDoc();
            ViewBag.Vensupdoc = Common.GetVendorSupportingDoc();
            ViewBag.ventdsdoc = Common.GetVendorTdsDoc();
            ViewBag.vendorcode = MasterService.GetVendorCode();
            ViewBag.serviceCategory = Common.GetCategoryService();
            ViewBag.serviceType = Common.GetServiceTypeList();
            ViewBag.suppliertype = Common.GetSupplierType();
            ViewBag.tdssection = Common.GetTdsList();
            return View();
        }
        [Authorized]
        [HttpPost]
        public ActionResult Vendor(VendorMasterViewModel model)
        {
            ViewBag.vendorCountry = Common.GetAgencyType();
            ViewBag.state = Common.GetStatelist();
            ViewBag.country = Common.getCountryList();
            ViewBag.GstDoc = Common.GetGstSupportingDoc();
            ViewBag.Vensupdoc = Common.GetVendorSupportingDoc();
            ViewBag.ventdsdoc = Common.GetVendorTdsDoc();
            ViewBag.vendorcode = MasterService.GetVendorCode();
            ViewBag.serviceCategory = Common.GetCategoryService();
            ViewBag.serviceType = Common.GetServiceTypeList();
            ViewBag.suppliertype = Common.GetSupplierType();
            ViewBag.tdssection = Common.GetTdsList();
            var Username = User.Identity.Name;
            model.UserId = Common.GetUserid(Username);
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                if ((model.GSTAttachName[0] != null && model.GSTAttachName[0] != ""))
                {
                    for (int i = 0; i < model.GSTDocumentType.Length; i++)
                    {
                        if (model.GSTFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.GSTFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }
                if ((model.VendorAttachName[0] != null && model.VendorAttachName[0] != ""))
                {
                    for (int i = 0; i < model.VendorDocumentType.Length; i++)
                    {
                        if (model.VendorFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.VendorFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }
                if ((model.TDSAttachName[0] != null && model.TDSAttachName[0] != ""))
                {
                    for (int i = 0; i < model.TDSDocumentType.Length; i++)
                    {
                        if (model.TDSFile[i] != null)
                        {
                            string docname = Path.GetFileName(model.TDSFile[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ViewBag.filemsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }

                    }
                }
              
                int vendorStatus = MasterService.VendorMaster(model);
                if (vendorStatus == 1)
                {
                    ViewBag.success = "Saved successfully";
                }
                else if (vendorStatus == 2)
                {
                    ViewBag.Msg = "This Vendor Account Number Already Exits";
                    return View(model);
                }
                else if (vendorStatus == 3)
                {
                    ViewBag.update = "Vendor updated successfully";
                }
                else if (vendorStatus == 4)
                {
                    ViewBag.Msgs = "This PFMS Number Alredy Exits";
                    return View(model);
                }
                else
                {
                    ViewBag.error = "Somthing went to worng please contact Admin!.";
                    return View(model);
                }
                return View();
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                ViewBag.error = messages;

                return View();
            }

        }
        [Authorized]
        [HttpPost]
        public JsonResult GetVendorMaster(VendorSearchModel model, int pageIndex, int pageSize)
        {
            object output = MasterService.GetVendorList(model, pageIndex, pageSize);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult EditVendorlist(int vendorId)
        {
            object output = MasterService.EditVendor(vendorId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult Gettdspercetage(int sectionId)
        {
            object output = MasterService.GetSectiontds(sectionId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        public ActionResult LedgerOBBalance()
        {
            ViewBag.AcctCty = Common.GetAccounttype();
            ViewBag.FinYear = Common.GetFinYearList();
            return View();
        }
        [Authorized]
        [HttpGet]
        public JsonResult LoadListWiseHead(int accounttypid)
        {
            object output = MasterService.GetAccountWiseHead(accounttypid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpGet]
        public JsonResult GetOpeningBal(int accheadid)
        {
            object output = MasterService.GetOpeningBalance(accheadid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult AddOpeningBalanceLedger(LedgerOBBalanceModel model)
        {
            var Username = User.Identity.Name;
            model.Userid = Common.GetUserid(Username);
            object output = MasterService.AddOpeningBalanceLedger(model, Username);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
    }
}