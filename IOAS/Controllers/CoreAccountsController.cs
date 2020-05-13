using IOAS.DataModel;
using IOAS.Filter;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    [Authorized]
    public class CoreAccountsController : Controller
    {
        CoreAccountsService coreAccountService = new CoreAccountsService();
        private static readonly Object lockObj = new Object();

        #region Payment
        #region Purchase Order
        #region Advance

        public ActionResult AdvanceBillPaymentList()
        {
            //ViewBag.processGuideLineId = 1006;
            //var fe = FlowEngine.Init(1006, 1, 4, "BillId");
            //fe.ProcessInit();
            return View();

        }


        public ActionResult AdvanceBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidateBillOnEdit(billId, "ADV"))
                {
                    model = coreAccountService.GetBillDetails(billId);
                    
                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(29);
                model.InclusiveOfTax_f = false;
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult AdvanceBillPaymentView(int billId)
        {
            try
            {
                ViewBag.disabled = "disabled";
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                BillEntryModel model = new BillEntryModel();

                model = coreAccountService.GetBillDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                if (amt > 50000)
                    ViewBag.processGuideLineId = 2;
                else
                    ViewBag.processGuideLineId = 3;
                
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);

                model.InclusiveOfTax_f = false;
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AdvanceBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                ModelState.Remove("InvoiceNumber");
                ModelState.Remove("InvoiceDate");
                ModelState.Remove("BankHead");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateAdvanceBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.AdvanceBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been added successfully.";

                        return RedirectToAction("AdvanceBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been updated successfully.";
                        return RedirectToAction("AdvanceBillPaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Advance Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    //else if (result == -4)
                    //    TempData["errMsg"] = "Same invoice number exists for this Vendor.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateAdvanceBillPayment(BillEntryModel model)
        {
            decimal ttlAdvAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttldrAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = (item.TotalAmount * model.AdvancePercentage / 100) ?? 0;
                ttlAdvAmt += advAmt;
            }

            if (ttlAdvAmt != commitmentAmt)
                msg = "There is a mismatch between the requested advance value and allocated commitment value. Please update the value to continue.";
            //if (ttlAdvAmt != ttlExpAmt)
            //    msg = "There is a mismatch between the requested advance value and transaction value. Please update the value to continue.";
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";

            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }

        #endregion
        #region Part
        public ActionResult AdvancePartBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                 ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidatePartBillOnEdit(billId, true))
                {
                    model = coreAccountService.GetBillDetails(billId);
                    
                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(29);

                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult AdvancePartBillPaymentView(int billId)
        {
            try
            {
                ViewBag.disabled = "disabled";
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                BillEntryModel model = new BillEntryModel();
                model = coreAccountService.GetBillDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                if (amt > 50000)
                    ViewBag.processGuideLineId = 2;
                else
                    ViewBag.processGuideLineId = 3;
                
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdvancePartBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }

                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateAdvancePartBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.AdvancePartBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Part Bill has been added successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Part Bill has been updated successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Advance Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    //else if (result == -4)
                    //    TempData["errMsg"] = "Same invoice number exists for this vendor.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetAdvancedPercentageList();
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(29);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateAdvancePartBillPayment(BillEntryModel model)
        {
            decimal netAdvAmt = 0, ttlAdvAmt = 0, ttlGSTElgAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = (item.TotalAmount * model.AdvancePercentage / 100) ?? 0;
                decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                ttlAdvAmt += advAmt;
                netAdvAmt += advAmt + advTax;
                if (item.IsTaxEligible)
                    ttlGSTElgAmt = ttlGSTElgAmt + advTax;
            }
            if ((netAdvAmt - ttlGSTElgAmt) != commitmentAmt)
                msg = "There is a mismatch between the part payment total value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (TransAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the expense value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the expense value and allocated commitment value. Please update the value to continue.";

            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            if (99 < model.AdvancePercentage)
                msg = msg == "Valid" ? "Part percentage should not be greater than 99%." : msg + "<br /> Part percentage should not be greater than 99%.";

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }
        public ActionResult PartBillPaymentList()
        {
            return View();

        }


        public ActionResult PartBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                //ViewBag.AdvPctList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidatePartBillOnEdit(billId))
                {
                    model = coreAccountService.GetBillDetails(billId);
                    
                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                    //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId, model.BillId);
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(30);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult PartBillPaymentView(int billId)
        {
            try
            {
                ViewBag.disabled = "disabled";
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BillEntryModel model = new BillEntryModel();

                model = coreAccountService.GetBillDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                if (amt > 50000)
                    ViewBag.processGuideLineId = 2;
                else
                    ViewBag.processGuideLineId = 3;
                
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId, model.BillId);
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult PartBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId, model.BillId);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidatePartBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.PartBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Payment Bill has been added successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Payment Bill has been updated successfully.";
                        return RedirectToAction("PartBillPaymentList");
                    }
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                //ViewBag.AdvPctList = Common.GetBillRMNGPercentageList(model.PONumber, model.VendorId);
                
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "PTM");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(30);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(headId);
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }

        private string ValidatePartBillPayment(BillEntryModel model)
        {
            decimal netAdvAmt = 0, ttlAdvAmt = 0, ttlGSTElgAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = (item.TotalAmount * model.AdvancePercentage / 100) ?? 0;
                decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                ttlAdvAmt += advAmt;
                netAdvAmt += advAmt + advTax;
                if (item.IsTaxEligible)
                    ttlGSTElgAmt = ttlGSTElgAmt + advTax;
            }
            if ((netAdvAmt - ttlGSTElgAmt) != commitmentAmt)
                msg = "There is a mismatch between the part payment total value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (TransAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the expense value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the expense value and allocated commitment value. Please update the value to continue.";
            //if (!model.InclusiveOfTax_f && gst != "NotEligible" && ttlAdvAmt != netCrAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between the part payment value and credit value. Please update the value to continue." : msg + "<br />There is a mismatch between the part payment value and credit value. Please update the value to continue.";
            //else if ((model.InclusiveOfTax_f || gst == "NotEligible") && netAdvAmt != netCrAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between the part payment total value and credit value. Please update the value to continue." : msg + "<br />There is a mismatch between the part payment total value and credit value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            string poNumber = Common.GetBillPONumber(Convert.ToInt32(model.PONumber));
            if (Common.GetBillRMNGPercentage(poNumber, model.VendorId, model.BillId) < model.AdvancePercentage)
                msg = msg == "Valid" ? "Part percentage should not be greater than remaining percentage." : msg + "<br /> Part percentage should not be greater than remaining percentage.";

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }
        #endregion
        #region Settlement

        public ActionResult SettlementBillPaymentList()
        {
            return View();

        }


        public ActionResult SettlementBillPayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetCodeControlList("SettlementType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BillEntryModel model = new BillEntryModel();
                if (billId > 0 && Common.ValidateBillOnEdit(billId, "STM"))
                {
                    model = coreAccountService.GetBillDetails(billId);
                    
                    ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                    ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(31);

                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult SettlementBillPaymentView(int billId)
        {
            try
            {
                ViewBag.disabled = "disabled";
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetCodeControlList("SettlementType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BillEntryModel model = new BillEntryModel();
                model = coreAccountService.GetBillDetails(billId);
                var amt = model.BillAmount + model.BillTaxAmount;
                if (amt > 50000)
                    ViewBag.processGuideLineId = 2;
                else
                    ViewBag.processGuideLineId = 3;
                
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult SettlementBillPayment(BillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetCodeControlList("SettlementType");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSettlementBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SettlementBillPaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been added successfully.";
                        return RedirectToAction("SettlementBillPaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been updated successfully.";
                        return RedirectToAction("SettlementBillPaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.VendorTDSList = Common.GetVendorTDSList(model.VendorId);
                
                ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId, model.PONumber, "ADV");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }

        private string ValidateSettlementBillPayment(BillEntryModel model)
        {
            decimal netAdvAmt = 0, ttlAdvAmt = 0, ttlGSTElgAmt = 0;
            string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (model.PaymentType != 2)
            {
                foreach (var item in model.PODetail)
                {
                    decimal advAmt = item.TotalAmount ?? 0;
                    decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                    //ttlAdvAmt += advAmt;
                    //netAdvAmt += advAmt + advTax;
                    if (item.IsTaxEligible)
                        ttlGSTElgAmt = ttlGSTElgAmt + advTax;
                }
                ttlAdvAmt = model.InvoiceAmount ?? 0;
                netAdvAmt = ttlAdvAmt + (model.InvoiceTaxAmount ?? 0);
            }
            else
            {
                foreach (var item in model.PODetail)
                {
                    if (item.IsTaxEligible)
                    {
                        decimal advAmt = item.TotalAmount ?? 0;
                        decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                        ttlGSTElgAmt = ttlGSTElgAmt + advTax;
                    }
                }
                ttlAdvAmt = (model.InvoiceAmount - model.hiddenSettAmt) ?? 0;
                netAdvAmt = ttlAdvAmt + (model.InvoiceTaxAmount - model.hiddenSettTaxAmt) ?? 0;
            }
            netAdvAmt = netAdvAmt - ttlGSTElgAmt;
            if (netAdvAmt != commitmentAmt)
                msg = "There is a mismatch between the settlement value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (TransAmt != (model.InvoiceAmount + model.InvoiceTaxAmount))
                msg = msg == "Valid" ? "There is a mismatch between the credit value and invoice value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and invoice value. Please update the value to continue.";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between the expense value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the expense value and allocated commitment value. Please update the value to continue.";
            //if (gst == "NotEligible" && netCrAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "Total Credit and Total Commitment Values are not equal." : msg + "<br />Total Credit and Total Commitment Values are not equal.";
            //if (model.PaymentType == 2)
            //{
            //    var data = Common.ValidateSettlement(Common.GetBillPONumber(model.selPONumber ?? 0), model.VendorId, ttldeductAmt, ttlExpAmt, model.BillId);
            //    if (data != "Valid")
            //        msg = msg == "Valid" ? data : msg + "<br /> " + data;
            //}
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }

            //var groupNames = new string[model.CommitmentDetail.Count];
            //int count = 0;
            //foreach (var item in model.CommitmentDetail)
            //{
            //    decimal? commitmentAmtForHead = 0, headWiseExp = 0;
            //    if (!groupNames.Contains(item.HeadName))
            //    {
            //        commitmentAmtForHead = model.CommitmentDetail.Where(m => m.HeadName == item.HeadName).Select(m => m.PaymentAmount).Sum();
            //        int headId = Common.GetAccountGroupId(item.HeadName);
            //        headWiseExp = model.ExpenseDetail.Where(m => m.AccountGroupId == headId).Select(m => m.Amount).Sum();
            //        if (commitmentAmtForHead != headWiseExp)
            //        {
            //            msg = msg == "Valid" ? "The amount enter for " + item.HeadName + " is not equal to commitment value." : msg + "<br />The amount enter for " + item.HeadName + " is not equal to commitment value.";
            //        }
            //        groupNames[count] = item.HeadName;
            //    }
            //    count++;
            //}
            return msg;
        }
        #endregion
        #region Common

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PreviousBillHistory(int vendorId)
        {
            try
            {
                ViewBag.data = coreAccountService.GetBillHistoryList(vendorId);
                return PartialView();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        [HttpPost]
        public ActionResult POWFInit(int billId, string transTypeCode)
        {
            try
            {
                lock (lockObj)
                {
                    if (Common.ValidateBillOnEdit(billId, transTypeCode))
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool cStatus = coreAccountService.BillCommitmentBalanceUpdate(billId, false, false, userId, transTypeCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.POWFInit(billId, userId, transTypeCode);
                        if (!status)
                            coreAccountService.BillCommitmentBalanceUpdate(billId, true, false, userId, transTypeCode);
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetBillPaymentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetBillPaymentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetVendorDetails(int vendorId, bool poNumberRequired = false, string transTypeCode = "", bool TDSRequired = false)
        {
            try
            {
                var output = coreAccountService.GetVendorDetails(vendorId);
                if (poNumberRequired)
                    output.PONumberList = Common.GetBillPONumberList(vendorId, null, transTypeCode);
                if (TDSRequired)
                    output.TDSList = Common.GetVendorTDSList(vendorId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTypeOfServiceList(int type)
        {
            try
            {
                object output = Common.GetTypeOfServiceList(type);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetServiceTypeDetail(int serviceType)
        {
            try
            {
                var data = Common.GetServiceDetail(serviceType);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchCommitments(DateTime? fromDate, DateTime? toDate, int? projectType, int projectId, string keyword, int commitmentType = 0)
        {
            try
            {
                object output = coreAccountService.SearchCommitments(fromDate, toDate, projectType, projectId, keyword, commitmentType);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetSelectedCommitmentDetails(Int32[] selCommitment)
        {
            try
            {
                selCommitment = selCommitment.Distinct().ToArray();
                object output = coreAccountService.GetSelectedCommitmentDetails(selCommitment);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTransactionDetails(string typeCode, bool interstate_f = false, bool eligibilityCheck_f = false, int deductionCategoryId = 0, string tSubCode = "1", List<int?> TDSDetailId = null)
        {
            try
            {
                object output = coreAccountService.GetTransactionDetails(deductionCategoryId, interstate_f, typeCode, tSubCode, eligibilityCheck_f, TDSDetailId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetAddNewExpenseDetails(string typeCode, string tSubCode = "1")
        {
            try
            {
                object output = coreAccountService.GetAddNewExpenseDetails(typeCode, tSubCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetAccountHead(Int32 accountGroupId, bool? isBank = null)
        {
            try
            {
                object output = Common.GetAccountHeadList(accountGroupId, 0, "", "", isBank);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetBillPODetails(Int32 billId, bool isSettlement = false)
        {
            try
            {
                var output = coreAccountService.GetBillPODetails(billId);
                var typeList = Common.GetTypeOfServiceList(output.BillType ?? 0);
                if (!isSettlement)
                {
                    var rmnBal = Common.GetBillRMNGPercentage(output.PONumber, output.VendorId);
                    //rmnBal -= 1;
                    return Json(new { data = output, rmnBal = rmnBal, typeList = typeList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = Common.GetBillPaidAndRMNGAmt(output.PONumber, output.VendorId);
                    return Json(new { data = output, billAmt = data.Item3, billTaxAmt = data.Item4, settAmt = data.Item1, settTaxAmt = data.Item2, typeList = typeList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetTransactionType(int accountGroupId, int accountHeadId, string typeCode, string tSubCode = "1")
        {
            try
            {
                var output = Common.GetTransactionType(accountGroupId, accountHeadId, typeCode, tSubCode);
                return Json(new { tType = output.Item1, isJv = output.Item2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #endregion
        #region Travel
        #region Advance

        public ActionResult TravelAdvancePaymentList()
        {
            return View();
        }


        public ActionResult TravelAdvancePayment(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                TravelAdvanceModel model = new TravelAdvanceModel();
                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TAD", "Open"))
                {
                    model = coreAccountService.GetTravelAdvanceDetails(travelBillId);
                    
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult TravelAdvancePaymentView(int travelBillId)
        {
            try
            {
                ViewBag.processGuideLineId = 1;
                ViewBag.disabled = "disabled";
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList =emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                TravelAdvanceModel model = new TravelAdvanceModel();
                model = coreAccountService.GetTravelAdvanceDetails(travelBillId);
                

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpPost]
        public ActionResult TravelAdvancePayment(TravelAdvanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.AdvanceTravelBillIU(model, logged_in_user);
                    if (model.TravelBillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been added successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else if (model.TravelBillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Advance Bill has been updated successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }

        public ActionResult TravelAdvanceBillEntry(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TravelAdvanceBillEntryModel model = new TravelAdvanceBillEntryModel();

                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Entry"))
                {
                    var advModel = coreAccountService.GetTravelAdvanceDetails(travelBillId);
                    model = JsonConvert.DeserializeObject<TravelAdvanceBillEntryModel>(JsonConvert.SerializeObject(advModel));
                    
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(36);
                }
                else if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Approval"))
                {
                    model = coreAccountService.GetTravelAdvanceBillEntryDetails(travelBillId);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                    
                }
                model.CreditorType = "PI / Clearance agent / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult TravelAdvanceBillEntryView(int travelBillId)
        {
            try
            {
                ViewBag.disabled = "disabled";
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TravelAdvanceBillEntryModel model = new TravelAdvanceBillEntryModel();

                model = coreAccountService.GetTravelAdvanceBillEntryDetails(travelBillId);
                model.CreditorType = "PI / Clearance agent / Student";
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult TravelAdvanceBillEntry(TravelAdvanceBillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model.CreditorType = "PI / Clearance agent / Student";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTADPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.TravelADVBillEntryIU(model, logged_in_user);
                    if (result == 1)
                    {
                        TempData["succMsg"] = "Travel advance bill entry has been added successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else if (result == 2)
                    {
                        TempData["succMsg"] = "Travel advance bill entry has been updated successfully.";
                        return RedirectToAction("TravelAdvancePaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult DeleteTravelAdvanceBill(int travelBillId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.DeleteTravelAdvanceBill(travelBillId, userId);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult TravelAdvanceBillWFInit(int travelBillId, string transCode)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (transCode == "TAD")
                    {
                        bool status = coreAccountService.TravelAdvanceBillWFInit(travelBillId, userId, transCode);
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    else if (Common.ValidateTravelBillStatus(travelBillId, transCode, "Open"))
                    {
                        bool reversed = false;
                        if (transCode == "TST")
                            reversed = Common.TSTBillIsReceipt(travelBillId);
                        bool cStatus = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, false, reversed, userId, transCode);
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.TravelAdvanceBillWFInit(travelBillId, userId, transCode);
                        if (!status)
                            coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, reversed, userId, transCode);
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult _TravelADVCommitment(int travelBillId)
        //{
        //    CommitmentModel model = new CommitmentModel();
        //    ViewBag.CommitmentType = Common.getCommitmentType();
        //    ViewBag.Purpose = Common.getPurpose();
        //    ViewBag.Currency = Common.getCurrency();
        //    ViewBag.BudgetHead = Common.getBudgetHead();
        //    ViewBag.ProjectType = Common.getprojecttype();
        //    ViewBag.AccountGroup = Common.getAccountGroup();
        //    ViewBag.AccountHead = Common.getBudgetHead();
        //    ViewBag.Vendor = Common.getVendor();
        //    model = coreAccountService.GetTABillDetailsForCommitment(travelBillId);
        //    if (model.selRequestRefrence == 0)
        //        throw new Exception();
        //    var Data = Common.getProjectNo(model.selProjectType);
        //    ViewBag.ProjectNo = Data.Item1;
        //    model.TravelBillId = travelBillId;
        //    model.CommitmentNo = "0";
        //    model.commitmentValue = 0;
        //    model.currencyRate = 0;
        //    return PartialView(model);

        //}
        //[HttpPost]
        //public ActionResult _TravelADVCommitment(CommitmentModel model)
        //{
        //    try
        //    {
        //        var UserId = Common.GetUserid(User.Identity.Name);
        //        AccountService _AS = new AccountService();
        //        int result = 0;
        //        if (ModelState.IsValid)
        //        {
        //            result = _AS.SaveCommitDetails(model, UserId, true);
        //            if (result > 0)
        //            {
        //                coreAccountService.UpdateTAStatusOnBookCommitment(model.TravelBillId ?? 0, UserId, result);
        //                TempData["succMsg"] = "Commitment has been booked successfully";
        //            }
        //            else
        //            {
        //                TempData["errMsg"] = "Something went wrong please contact administrator";
        //            }
        //        }
        //        else
        //        {
        //            string messages = string.Join("<br />", ModelState.Values
        //                               .SelectMany(x => x.Errors)
        //                               .Select(x => x.ErrorMessage));

        //            TempData["errMsg"] = messages;
        //        }
        //        return RedirectToAction("TravelAdvancePaymentList");
        //    }
        //    catch (Exception ex)
        //    {

        //        TempData["errMsg"] = "Something went wrong please contact administrator";
        //        return RedirectToAction("TravelAdvancePaymentList");
        //    }
        //}
        [HttpPost]
        public ActionResult TravelAdvanceBillApproved(int travelBillId)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateTravelBillStatus(travelBillId, "TAD", "Pending Bill Approval"))
                    {
                        bool cStatus = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, false, false, userId, "TAD");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = coreAccountService.TravelAdvanceBillApproved(travelBillId, userId);
                        if (!status)
                            coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, userId, "TAD");
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        private string ValidateTADPayment(TravelAdvanceBillEntryModel model)
        {

            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal netADVAmt = model.AdvanceValue ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            decimal ttlGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlGSTElg = Math.Round(ttlGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal validCmtAmt = netADVAmt - ttlGSTElg;
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }

            if (validCmtAmt != commitmentAmt)
                msg = "There is a mismatch between the requested advance value and allocated commitment value. Please update the value to continue.";
            if (model.AdvanceValue != paymentBUAmt)
                msg = msg == "Valid" ? "There is a mismatch between the requested advance value and payment break up total value. Please update the value to continue." : msg + "<br /> There is a mismatch between the requested advance value and payment break up total value. Please update the value to continue.";
            if (netDrAmt != crAmt || netCrAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        #endregion
        #region Settlement
        public ActionResult TravelSettlementPaymentList()
        {
            return View();
        }

        public ActionResult TravelSettlementPayment(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.TravelAdvBillNoList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                TravelSettlementModel model = new TravelSettlementModel();
                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "TST", "Open"))
                {
                    model = coreAccountService.GetTravelSettlementDetails(travelBillId);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);
                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(43);
                    model.NeedUpdateTransDetail = true;
                }
                model.CreditorType = "PI / Clearance agent / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpPost]
        public ActionResult TravelSettlementPayment(TravelSettlementModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.TravelAdvBillNoList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                if (model.TravelBillId > 0)
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);

                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                model.CreditorType = "PI / Clearance agent / Student";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (model.OverallExpense < model.AdvanceAmount)
                {
                    for (int i = 0; i < model.PaymentBreakDetail.Count(); i++)
                    {
                        ModelState.Remove("PaymentBreakDetail[" + i + "].CategoryId");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].ModeOfPayment");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].PaymentAmount");
                    }
                }
                else if (model.OverallExpense == model.AdvanceAmount)
                {
                    for (int i = 0; i < model.PaymentBreakDetail.Count(); i++)
                    {
                        ModelState.Remove("PaymentBreakDetail[" + i + "].CategoryId");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].ModeOfPayment");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.ExpenseDetail.Count(); i++)
                    {
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountGroupId");
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountHeadId");
                        ModelState.Remove("ExpenseDetail[" + i + "].TransactionType");
                        ModelState.Remove("ExpenseDetail[" + i + "].Amount");
                        ModelState.Remove("ExpenseDetail[" + i + "].IsJV");
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTSTPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.TravelSettlementIU(model, logged_in_user);
                    if (model.TravelBillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Travel settlement has been added successfully.";
                        return RedirectToAction("TravelSettlementPaymentList");
                    }
                    else if (model.TravelBillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Travel settlement has been updated successfully.";
                        return RedirectToAction("TravelSettlementPaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.TravelAdvBillNoList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                if (model.TravelBillId > 0)
                    ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);

                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }

        [HttpGet]
        public JsonResult GetTravelAdvanceDetails(int travelBillId)
        {
            try
            {
                object output = coreAccountService.GetTravelAdvanceDetailsForSettlement(travelBillId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult TravelSettlementPaymentView(int travelBillId)
        {
            try
            {
                ViewBag.disabled = "disabled";
                ViewBag.processGuideLineId = 1;
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TravellerList = Common.GetPIWithDetails();
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.CurrencyList = Common.getCurrency();
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(43);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                TravelSettlementModel model = new TravelSettlementModel();

                model = coreAccountService.GetTravelSettlementDetails(travelBillId);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                ViewBag.TravelAdvBillNoList = Common.GetTravelADVList(model.selADVBillNumber);
                
                model.CreditorType = "PI / Clearance agent / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpGet]
        public JsonResult GetTravellerDailyAllowance(int countryId)
        {
            try
            {
                decimal output = Common.GetTravellerDailyAllowance(countryId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private string ValidateTSTPayment(TravelSettlementModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal overallExp = model.OverallExpense ?? 0;
            decimal piAdvAmt = model.AdvanceValueWOClearanceAgent ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlGSTElg = Math.Round(ttlGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            if (piAdvAmt < overallExp)
            {
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                decimal validCmtAmt = overallExp - piAdvAmt - ttlGSTElg;
                if (validCmtAmt != model.CommitmentAmount)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if ((overallExp - piAdvAmt) != paymentBUAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the payable value and payment break up total value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payable value and payment break up total value. Please update the value to continue.";

            }
            else if (piAdvAmt > model.OverallExpense)
            {
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount < item.ReversedAmount)
                        msg = msg == "Valid" ? "Commitment reversed value should not be less than booked value." : msg + "<br /> Commitment reversed value should not be less than booked value.";
                }
                decimal validCmtAmt = piAdvAmt - overallExp + ttlGSTElg;
                if (validCmtAmt != model.CommitmentAmount)
                    msg = msg == "Valid" ? "There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue.";
                if (0 != paymentBUAmt)
                    msg = msg == "Valid" ? "Not a valid entry. You can't give Payment Break Up value." : msg + "<br /> Not a valid entry. You can't give Payment Break Up value.";
            }
            else
            {
                if (0 != (model.CommitmentAmount ?? 0))
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if (0 != paymentBUAmt)
                    msg = msg == "Valid" ? "Not a valid entry. You can't give Payment Break Up value." : msg + "<br /> Not a valid entry. You can't give Payment Break Up value.";
            }
            if (netCrAmt != model.PayableValue)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;

            if (netDrAmt != crAmt || netCrAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        #endregion
        #region Domestic Travel
        public ActionResult DomesticTravelPaymentList()
        {
            return View();
        }
        public ActionResult DomesticTravelPayment(int travelBillId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                DomesticTravelBillEntryModel model = new DomesticTravelBillEntryModel();
                if (travelBillId > 0 && Common.ValidateTravelBillStatus(travelBillId, "DTV", "Open"))
                {
                    model = coreAccountService.GetDomesticTravelDetails(travelBillId);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(57);
                    model.NeedUpdateTransDetail = true;
                }
                model.CreditorType = "PI / Student";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult DomesticTravelPayment(DomesticTravelBillEntryModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model.CreditorType = "PI / Student";
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateDTVPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    if (model.InvoiceAttachment != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string taxprooffilename = Path.GetFileName(model.InvoiceAttachment.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.DomesticTravelIU(model, logged_in_user);
                    if (model.TravelBillId == null && result > 0)
                    {
                        TempData["succMsg"] = "Domestic travel bill entry has been added successfully.";
                        return RedirectToAction("DomesticTravelPaymentList");
                    }
                    else if (model.TravelBillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Domestic travel bill entry has been updated successfully.";
                        return RedirectToAction("DomesticTravelPaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult DomesticTravelPaymentView(int travelBillId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList =
                ViewBag.TravellerList = emptyList;
                ViewBag.ExpenseTypeList = Common.GetCodeControlList("TravelExpenseType");
                ViewBag.CategoryList = Common.GetCodeControlList("TravellerCategory");
                ViewBag.ProjectList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(57);
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "DTV");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                DomesticTravelBillEntryModel model = new DomesticTravelBillEntryModel();
                model = coreAccountService.GetDomesticTravelDetails(travelBillId);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                model.CreditorType = "PI / Student";
                ViewBag.disabled = "disabled";
                ViewBag.processGuideLineId = 1;
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateDTVPayment(DomesticTravelBillEntryModel model)
        {

            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal overAllExp = model.BreakUpDetail.Select(m => m.ProcessedAmount).Sum() ?? 0;
            decimal payBUTtl = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            payBUTtl = payBUTtl + (model.PaymentTDSAmount ?? 0);
            decimal ttlGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlGSTElg = Math.Round(ttlGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal validCmtAmt = overAllExp - ttlGSTElg;
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;

            if (validCmtAmt != commitmentAmt || overAllExp == 0)
                msg = "There is a mismatch between the overall expense value and allocated commitment value. Please update the value to continue.";
            if (payBUTtl != overAllExp)
                msg = msg == "Valid" ? "There is a mismatch between the payable value and payment break up total value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payable value and payment break up total value. Please update the value to continue.";
            if (netDrAmt != crAmt || netCrAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (netCrAmt != model.OverallExpense)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and allocated commitment value. Please update the value to continue." : msg + "<br />There is a mismatch between the credit value and allocated commitment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = msg == "Valid" ? "Selected attachment type already exist. Please select a different attachment type." : msg + "<br />Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            var gPayee = model.PaymentBreakDetail.GroupBy(m => m.UserId);
            if (gPayee.Count() != model.PaymentBreakDetail.Count())
                msg = msg == "Valid" ? "Selected payee already exist. Please select a different payee." : msg + "<br /> Selected payee already exist. Please select a different payee.";
            //var gBreakUp = model.BreakUpDetail.GroupBy(m => m.ExpenseTypeId);
            //if (gBreakUp.Count() != model.BreakUpDetail.Count())
            //    msg = msg == "Valid" ? "Selected expense type already exist. Please select a different expense type." : msg + "<br /> Selected expense type already exist. Please select a different expense type.";
            var gTraveller = model.TravelerDetail.GroupBy(m => m.UserId);
            if (gTraveller.Count() != model.TravelerDetail.Count())
                msg = msg == "Valid" ? "Selected traveller already exist. Please select a different traveller." : msg + "<br /> Selected traveller already exist. Please select a different traveller.";

            return msg;
        }
        #endregion
        #region Common

        [HttpGet]
        public JsonResult GetTravelBillList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetTravelBillList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadPIList(string term)
        {
            try
            {
                var data = Common.GetAutoCompletePIWithDetails(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadTypeOfServiceList(string term, int? type = null)
        {
            try
            {
                var data = Common.GetAutoCompleteTypeOfServiceList(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadClearanceAgentList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteClearanceAgent(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult LoadStudentList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteStudentList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult GetTravelCommitmentDetails(int travelBillId)
        {
            var data = coreAccountService.GetTravelCommitmentDetails(travelBillId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadProjectList(string term, int? type = null, int? classification = null)
        {
            try
            {
                lock (lockObj)
                {
                    var data = Common.GetAutoCompleteProjectList(term, type);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPIADVList(int PI)
        {
            try
            {
                var data = Common.GetTravelADVList(null, PI);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #endregion
        #region SBICardRecoupment
        [HttpGet]
        public JsonResult GetEcardRecoupmentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetEcardRecoupmentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult SBIECardRecoupmentList()
        {
            return View();
        }
        public ActionResult SBIECardRecoupment(int SBICardRecoupId = 0, int SBICardProjectDetailsId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(46);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                model.SBIEcardProjectDetailsId = SBICardProjectDetailsId;

                if (SBICardRecoupId > 0)
                {
                    model = coreAccountService.GetSBIECardRecoupmentDetails(SBICardRecoupId);
                    
                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                else
                {
                    model = coreAccountService.GetSBIECardDetails(SBICardProjectDetailsId);
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(46);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult SBIECardRecoupmentView(int SBICardProjectDetailsId, int SBICardRecoupId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(46);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                if (SBICardRecoupId > 0)
                {
                    model = coreAccountService.GetSBIECardRecoupmentDetails(SBICardRecoupId);
                    
                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult SBIECardRecoupment(SBIECardModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(46);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                // ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSBIECardRecoupmentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SBIECardRecoupment(model, logged_in_user);
                    if (model.RecoupmentId == 0 && result > 0)
                    {
                        ViewBag.succMsg = "Recoupment has been added successfully.";

                    }
                    else if (model.RecoupmentId > 0 && result > 0)
                    {
                        ViewBag.succMsg = "Recoupment has been updated successfully.";
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.UOMList = Common.GetCodeControlList("UOM");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateSBIECardRecoupmentBillPayment(SBIECardModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpPost]
        public ActionResult SBIECardRecoupmentApprove(int recoupmentId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.SBIECardRecoupmentBalanceUpdate(recoupmentId, false, false, userId, "ECR");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.SBIECardRecoupmentBillApproved(recoupmentId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpGet]
        //public JsonResult GetSBICardTransactionDetails(string typeCode, int deductionCategoryId = 0, string tSubCode = "1")
        //{
        //    try
        //    {
        //        object output = coreAccountService.GetSBICardTransactionDetails(deductionCategoryId, typeCode, tSubCode);
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        #endregion
        #region SBI Card
        public ActionResult SBIECardList()
        {
            var servicetype = Common.getservicetype();
            var invoicetype = Common.getinvoicetype();
            var projecttype = Common.getprojecttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.Project = emptyList;
            ViewBag.typeofservice = servicetype;
            ViewBag.TypeofInvoice = invoicetype;
            ViewBag.projecttype = projecttype;
            ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.Gender = Common.getGender();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
            return View();
        }
        public ActionResult SBIECard(int ProjectId = 0, int CardID = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                model.SBIEcardId = CardID;
                model.ProjectID = ProjectId;
                model.TotalValueofCard = 0;
                var projectdetails = Common.GetPIdetailsbyProject(ProjectId);
                model.NameofPI = projectdetails.PIName;
                model.PIDepartmentName = projectdetails.PIDepartment;
                model.PIId = projectdetails.PIId;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (ProjectId > 0 && CardID > 0 && Common.ValidateSBICardPjctdtlsOnEdit(ProjectId, "ECD"))
                {
                    model = coreAccountService.EditProjectCardDetails(ProjectId, CardID);
                    model.SBIEcardId = CardID;
                }
                else if (ProjectId > 0 && CardID == 0)
                {
                    string validationMsg = Common.ValidateSBICardPjctAddition(ProjectId, CardID);
                    if (validationMsg != "Valid")
                    {
                        ViewBag.errMsg = validationMsg;
                        return View(model);
                    }
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(39);
                }
                else if (ProjectId == 0 && CardID == 0)
                {
                    return RedirectToAction("SBIECardList", "CoreAccounts");
                }
                else
                    model.CreditorType = "PI";
                model.CheckListDetail = Common.GetCheckedList(39);
                return View(model);
            }
            catch (Exception ex)
            {
                SBIECardModel model = new SBIECardModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult SBIECard(SBIECardModel model)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                CoreAccountsService _ps = new CoreAccountsService();
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSBIECardBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                model.NeedUpdateTransDetail = true;
                                return View(model);
                            }
                        }
                    }
                    if (model.CurrentProjectAllotmentValue > 10000)
                    {
                        TempData["errMsg"] = "The amount allocated per project cannot be greater than Rs 10000";
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }
                    var SBIECardID = _ps.CreateSBIECard(model, loggedinuserid);
                    if (SBIECardID > 0)
                    {
                        var SBIECardNumber = Common.getSBIEcardnumber(SBIECardID);
                        ViewBag.succMsg = "Project added to SBI Prepaid Card with Card number - " + SBIECardNumber + ".";
                    }
                    if (SBIECardID == -2)
                    {
                        var SBIECardNumber = Common.getSBIEcardnumber(model.SBIEcardId);
                        ViewBag.succMsg = SBIECardNumber + " - Card and Project details updated.";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                model.NeedUpdateTransDetail = true;
                return View(model);
            }
        }

        private string ValidateSBIECardBillPayment(SBIECardModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            // decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }

            return msg;
        }
        public ActionResult ExistingSBICard(int CardID = 0, int ProjectId = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Gender = Common.getGender();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                var projectdetails = Common.GetPIdetailsbyProject(ProjectId);
                SBIECardModel model = new SBIECardModel();
                model.NameofPI = projectdetails.PIName;
                model.PIDepartmentName = projectdetails.PIDepartment;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (ProjectId == 0 && CardID == 0)
                {
                    return RedirectToAction("SBIECardList", "CoreAccounts");
                }
                string validationMsg = Common.ValidateSBICardPjctAddition(ProjectId, CardID);
                if (validationMsg != "Valid")
                {
                    ViewBag.errMsg = validationMsg;
                    return View(model);
                }

                model = coreAccountService.GetCardandPjctDetailsbyID(CardID, ProjectId);
                model.CreditorType = "PI";
                model.NeedUpdateTransDetail = true;
                model.CheckListDetail = Common.GetCheckedList(39);
                return View(model);

            }
            catch (Exception ex)
            {
                SBIECardModel model = new SBIECardModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult LoadSBIEcardList()
        {
            try
            {
                object output = coreAccountService.GetSBIEcardList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProject(string PIUserId)
        {
            try
            {
                PIUserId = PIUserId == "" ? "0" : PIUserId;
                var locationdata = coreAccountService.GetProjectList(Convert.ToInt32(PIUserId));
                return Json(locationdata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult LoadProjectCardDetails(string ProjectID)
        {
            try
            {
                ProjectID = ProjectID == "" ? "0" : ProjectID;
                object output = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectID));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchSBIECardList(SBIECardSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchSBICardList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchSBICardRecoupmentList(SBIECardSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchSBICardRecoupmentList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult SBICardView(int ProjectId = 0, int CardID = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SBIECardModel model = new SBIECardModel();
                //  model = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectId));
                model = coreAccountService.EditProjectCardDetails(ProjectId, CardID);
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult ExistingPIDetails(int PIID = 0, int ProjectId = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Gender = Common.getGender();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                SBIECardModel model = new SBIECardModel();
                model = coreAccountService.GetCardPIDetailsbyID(PIID, ProjectId);
                return View("ExistingSBICard", model);
            }
            catch (Exception ex)
            {
                SBIECardModel model = new SBIECardModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult SBIECardProjectApprove(string prjctdetailsid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.SBIECardProjectApprove(Convert.ToInt32(prjctdetailsid), userId);
                //if(status == true)
                //{
                //    bool cStatus = coreAccountService.SBIECardProjectBalanceUpdate(Convert.ToInt32(prjctdetailsid), false, false);
                //    if (!cStatus)
                //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion     
        #region Temporary

        #region Advance
        public ActionResult TemporaryAdvancePayment(int tmpadvanceId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.TravellerList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(40);
                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                model.CreditorType = "PI";
                if (tmpadvanceId > 0 && Common.ValidateTempAdvBillOnEdit(tmpadvanceId, "TMP"))
                {
                    model = coreAccountService.GetTemporaryAdvanceDetails(tmpadvanceId);
                    
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(40);
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }


        public ActionResult TemporaryAdvancePaymentView(int tmpadvanceId)
        {

            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.TravellerList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(40);
                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                model.CreditorType = "PI";
                model = coreAccountService.GetTemporaryAdvanceDetails(tmpadvanceId);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }


        [HttpPost]
        public ActionResult TemporaryAdvancePayment(TemporaryAdvanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.VendorTDSList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(41);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTempAdvancePayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateTemporaryAdvance(model, logged_in_user);
                    if (model.TemporaryAdvanceId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Temporary Advance has been added successfully.";
                        return RedirectToAction("TemporaryAdvancePaymentList");
                    }
                    else if (model.TemporaryAdvanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Temporary Advance has been updated successfully.";
                        return RedirectToAction("TemporaryAdvancePaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateTempAdvancePayment(TemporaryAdvanceModel model)
        {
            //decimal ttlAdvAmt = 0;
            //string gst = model.GST;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        public ActionResult TemporaryAdvancePaymentList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetTemporaryAdvanceList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetTemporaryAdvanceList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult TemporayAdvanceApprove(string tempAdvId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.TempAdvCommitmentBalanceUpdate(Convert.ToInt32(tempAdvId), false, false, userId, "TMP");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.TempAdvApprove(Convert.ToInt32(tempAdvId), userId);
                if (!status)
                    coreAccountService.TempAdvCommitmentBalanceUpdate(Convert.ToInt32(tempAdvId), true, false, userId, "TMP");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIProject(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = Common.getProjectListofPI(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIDetails(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = Common.getProjectListofPI(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectDetails(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = Common.GetProjectsDetails(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadAdvanceDetails(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = Common.getprojectadvancedetails(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTemporaryADVCommitmentDetails(int temporaryAdvanceId)
        {
            var data = coreAccountService.GetTemporaryADVCommitmentDetails(temporaryAdvanceId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTempAdvTransactionDetails(string typeCode, int deductionCategoryId = 0, string tSubCode = "1")
        {
            try
            {
                object output = coreAccountService.GetTempAdvTransactionDetails(deductionCategoryId, typeCode, tSubCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PendingSettlementDetails(int Projectid)
        {
            TemporaryAdvanceModel model = new TemporaryAdvanceModel();
            model = coreAccountService.GetPendingAdvanceDetails(Projectid);
            return PartialView(model);
        }
        [HttpGet]
        public JsonResult SearchTemporaryAdvanceList(TempAdvSearchFieldModel model)
        {
            object output = coreAccountService.SearchTempAdvList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Settlement
        public ActionResult TemporaryAdvanceSettlementList()
        {
            return View();
        }

        public ActionResult TemporaryAdvanceSettlement(int TempAdvId = 0, int TempAdvsettlId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                if (TempAdvId > 0 && TempAdvsettlId == 0)
                {
                    string validationMsg = Common.ValidateTempAdvforSettlement(TempAdvId);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return RedirectToAction("TemporaryAdvanceSettlementList", "CoreAccounts");
                    }
                    model = coreAccountService.GetTempAdvanceDetails(TempAdvId);
                    model.NeedUpdateTransDetail = true;                    
                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                if (TempAdvsettlId > 0 && Common.ValidateTempAdvSettlementStatus(TempAdvsettlId, "TMS", "Open"))
                {
                    model = coreAccountService.GetTemporaryAdvanceSettlDetails(TempAdvsettlId);
                    
                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(47);

                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult TemporaryAdvanceSettlementView(int TempAdvsettlId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TemporaryAdvanceModel model = new TemporaryAdvanceModel();
                model = coreAccountService.GetTemporaryAdvanceSettlDetails(TempAdvsettlId);
                

                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult TemporaryAdvanceSettlement(TemporaryAdvanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                
                // ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                // ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (model.AmountofItem.Sum() == model.TemporaryAdvanceValue)
                {
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.ExpenseDetail.Count(); i++)
                    {
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountGroupId");
                        ModelState.Remove("ExpenseDetail[" + i + "].AccountHeadId");
                        ModelState.Remove("ExpenseDetail[" + i + "].TransactionType");
                        ModelState.Remove("ExpenseDetail[" + i + "].Amount");
                        ModelState.Remove("ExpenseDetail[" + i + "].IsJV");
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateTempSettlementBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.TemporaryAdvanceSettlement(model, logged_in_user);
                    if (model.TempAdvSettlId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been added successfully.";
                        return RedirectToAction("TemporaryAdvanceSettlementList");
                    }
                    else if (model.TempAdvSettlId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Settlement Bill has been updated successfully.";
                        return RedirectToAction("TemporaryAdvanceSettlementList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.UOMList = Common.GetCodeControlList("UOM");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                
                ViewBag.DocmentTypeList = Common.GetDocTypeList(47);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        [HttpGet]
        public JsonResult GetTempAdvSettlList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetTempAdvSettlList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult TemporaryAdvanceSettlementApproved(int id)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    if (Common.ValidateTempAdvSettlementStatus(id, "TMS", "Open"))
                    {
                        bool cStatus = coreAccountService.TempAdvSettlementCommitmentBalanceUpdate(id, false, userId, "TMS");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool boaStatus = coreAccountService.BalancedTMSBOATransaction(id);
                        bool status = coreAccountService.TempAdvSettlementApproved(id, userId);
                        if (!status)
                            coreAccountService.TempAdvCommitmentBalanceUpdate(id, true, false, userId, "TMS");
                        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { status = false, msg = "This bill already approved" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        private string ValidateTempSettlementBillPayment(TemporaryAdvanceModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal billAmt = model.AmountofItem.Sum() ?? 0;
            decimal advAmt = model.TemporaryAdvanceValue ?? 0;
            if (billAmt > advAmt)
            {
                decimal paymentVal = billAmt - advAmt;
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (paymentVal != commitmentAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value.Please update the value to continue.";
                if (crAmt != billAmt)
                    msg = msg == "Valid" ? "There is a mismatch between bill value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            else if (billAmt < advAmt)
            {
                decimal reversedVal = advAmt - billAmt;
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.ReversedAmount > item.PaymentAmount)
                        msg = msg == "Valid" ? "Commitment reversed value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (reversedVal != commitmentAmt)
                    msg = msg == "Valid" ? "There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between amount to be recevied and allocated commitment value. Please update the value to continue.";
                if (crAmt != advAmt)
                    msg = msg == "Valid" ? "There is a mismatch between bill value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            else
            {
                if (0 != commitmentAmt)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if (crAmt != advAmt)
                    msg = msg == "Valid" ? "There is a mismatch between bill value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }

            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != crAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);

            return msg;

        }
        #endregion

        #endregion
        #region SummerInternship 
        public ActionResult SummerInternshipStudentList()
        {
            return View();
        }
        public ActionResult SummerInternshipStudent(int internId = 0)
        {
            try
            {
                var emptyList = new List<SummerInternshipModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(44);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                SummerInternshipModel model = new SummerInternshipModel();
                model.CreditorType = "Student";
                if (internId > 0 && Common.ValidateSummerInternshipOnEdit(internId))
                {
                    model = coreAccountService.GetSummerInternshipDetails(internId);
                    
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(44);
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpGet]
        public ActionResult SummerInternshipApprove(int InternId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.SummerInternshipBalanceUpdate(InternId, false, false, userId, "SMI");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.SummerInternshipBillApproved(InternId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SummerInternshipStudentView(int internId)
        {

            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(44);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                SummerInternshipModel model = new SummerInternshipModel();

                model = coreAccountService.GetSummerInternshipDetails(internId);
                

                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SummerInternshipStudent(SummerInternshipModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(44);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateSummerInternship(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateSummerInternship(model, logged_in_user);
                    if (model.SummrInternStudentId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Summer Internship Student has been added successfully.";
                        return RedirectToAction("SummerInternshipStudentList");
                    }
                    else if (model.SummrInternStudentId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Summer Internship Student has been updated successfully.";
                        return RedirectToAction("SummerInternshipStudentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateSummerInternship(SummerInternshipModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpGet]
        public JsonResult GetSummerInternshipStudentList()
        {
            try
            {
                object output = coreAccountService.GetSummerInternshipStudentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchSummerInternshipList(SummerInternshipSearchFieldModel model)
        {
            object output = coreAccountService.SearchSummerInternshipList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region PartTime 
        public ActionResult PartTimeStudentList()
        {
            return View();
        }
        public ActionResult PartTimeStudent(int internId = 0)
        {
            try
            {
                var emptyList = new List<PartTimePaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.SessionList = Common.GetCodeControlList("Session");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(56);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                PartTimePaymentModel model = new PartTimePaymentModel();
                model.CreditorType = "PI";
                if (internId > 0 && Common.ValidatePartTimePaymentOnEdit(internId))
                {
                    model = coreAccountService.GetPartTimePaymentDetails(internId);
                    
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(56);
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult PartTimeStudentView(int internId)
        {

            try
            {
                var emptyList = new List<PartTimePaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.SessionList = Common.GetCodeControlList("Session");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(56);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                PartTimePaymentModel model = new PartTimePaymentModel();

                model = coreAccountService.GetPartTimePaymentDetails(internId);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult PartTimeStudent(PartTimePaymentModel model)
        {
            try
            {
                var emptyList = new List<PartTimePaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.SessionList = Common.GetCodeControlList("Session");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(56);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidatePartTimeStudent(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    foreach (var item in model.StudentDetails)
                    {
                        if (item.Name != null)
                        {
                            var stipendval = item.StipendValueperHour;
                            var session = item.Session;
                            var hrs = item.Duration;
                            if ((session == 1 || session == 2) && (stipendval < 100 || stipendval > 300))
                            {
                                TempData["errMsg"] = "Stipend value cannot be less than Rs 100 or greater than Rs 300";
                                return View(model);
                            }
                            if (session == 1 && hrs > 40)
                            {
                                TempData["errMsg"] = "Permited working hrs for Academic session is 40 hrs only.";
                                return View(model);
                            }
                            if (session == 2 && hrs > 60)
                            {
                                TempData["errMsg"] = "Permited working hrs for Non Academic session is 60 hrs only.";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreatePartTimePayment(model, logged_in_user);
                    if (model.PartTimePaymentId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Time Student has been added successfully.";
                        return RedirectToAction("PartTimeStudentList");
                    }
                    else if (model.PartTimePaymentId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Part Time Student has been updated successfully.";
                        return RedirectToAction("PartTimeStudentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidatePartTimeStudent(PartTimePaymentModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }

        [HttpGet]
        public JsonResult GetPartTimeStudentList()
        {
            try
            {
                object output = coreAccountService.GetPartTimeStudentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchPartTimePaymentList(PartTimePaymentSearchFieldModel model)
        {
            object output = coreAccountService.SearchPartTimePaymentList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetPartTimePaymentTransactionDetails(string typeCode, string tSubCode = "1")
        {
            try
            {
                object output = coreAccountService.GetPartTimePaymentTransactionDetails(typeCode, tSubCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadStudentByDepartment(string Departmentname)
        {
            var locationdata = coreAccountService.getStudentListbyDepartment(Departmentname);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadStudentDetails(string RollNo)
        {
            var locationdata = coreAccountService.getStudentDetails(RollNo);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PartPaymentApprove(int paymentId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.PartPaymentBalanceUpdate(paymentId, false, false, userId, "PTP");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.PartPaymentBillApproved(paymentId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region ClearancePayment
        public ActionResult ClearancePayment(int billId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ClearanceAgentList = Common.GetClearanceAgentList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetSettlementTypeList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ClearancePaymentEntryModel model = new ClearancePaymentEntryModel();
                if (billId > 0 && Common.ValidateClearancePaymentOnEdit(billId, "CLP"))
                {
                    model = coreAccountService.GetClearancePaymentDetails(billId);

                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                    ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                    ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                }
                else
                    model.CheckListDetail = Common.GetCheckedList(45);
                model.CreditorType = "Clearance Agent";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult ClearancePaymentView(int billId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ClearanceAgentList = Common.GetClearanceAgentList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetSettlementTypeList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ClearancePaymentEntryModel model = new ClearancePaymentEntryModel();
                model = coreAccountService.GetClearancePaymentDetails(billId);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                ViewBag.disabled = "Disabled";

                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                TempData["viewMode"] = "ViewOnly";
                model.CreditorType = "Clearance Agent";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ClearancePayment(ClearancePaymentEntryModel model)
        {
            try
            {
                model.CreditorType = "Clearance Agent";
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ClearanceAgentList = Common.GetClearanceAgentList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.PaymentTypeList = Common.GetSettlementTypeList();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();

                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateClearancePayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ClearancePaymentIU(model, logged_in_user);
                    if (model.BillId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Clearance Payment Bill has been added successfully.";
                        return RedirectToAction("ClearancePaymentList");
                    }
                    else if (model.BillId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Clearance Payment Bill has been updated successfully.";
                        return RedirectToAction("ClearancePaymentList");
                    }
                    else if (result == -2)
                        TempData["errMsg"] = "Clearance Payment Bill already exists for this PO Number with the Clearance Agent.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.TaxPctList = Common.GetCodeControlList("TaxPercentage");
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "CLP");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();

                ViewBag.TypeOfServiceList = Common.GetCLPTypeOfServiceList();
                ViewBag.PONumberList = Common.GetClearancePaymentPONumberList(model.ClearanceAgentId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(31);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult ClearancePaymentList()
        {
            return View();

        }

       
        private string ValidateClearancePayment(ClearancePaymentEntryModel model)
        {
            decimal netAdvAmt = 0, ttlGSTElgAmt = 0;
            string msg = "Valid";
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlInvGSTElg = model.InvoiceBreakDetail.Where(m => m.IsTaxEligible).Sum(m => m.TaxValue) ?? 0;
            ttlInvGSTElg = Math.Round(ttlInvGSTElg, 2, MidpointRounding.AwayFromZero);
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            foreach (var item in model.PODetail)
            {
                decimal advAmt = item.TotalAmount ?? 0;
                decimal advTax = (advAmt * item.TaxPct / 100) ?? 0;
                netAdvAmt += advAmt + advTax;
                if (item.IsTaxEligible)
                    ttlGSTElgAmt = ttlGSTElgAmt + advTax;
            }
            ttlGSTElgAmt = Math.Round(ttlGSTElgAmt, 2, MidpointRounding.AwayFromZero);
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal netCrAmt = model.CreditorAmount ?? 0;
            if (netAdvAmt != paymentBUAmt)
                msg = "Not a valid entry.The Payable value and Payment Break Up Total value are not equal.";
            if (ttlGSTElgAmt != ttlInvGSTElg)
                msg = msg == "Valid" ? "Not a valid entry. The PO tax eligible value and invoice tax eligible value are not equal." : msg + "<br /> Not a valid entry. The PO tax eligible value and invoice tax eligible value are not equal.";
            if ((netAdvAmt - ttlInvGSTElg) != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between the bill value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the bill value and allocated commitment value. Please update the value to continue.";
            if (netDrAmt != crAmt || (netCrAmt + ttlJVExpVal) != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            if (netCrAmt != paymentBUAmt)
                msg = msg == "Valid" ? "There is a mismatch between the credit value and payable value. Please update the value to continue." : msg + "<br /> There is a mismatch between the credit value and payable value. Please update the value to continue.";
            if (TransAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";

            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            return msg;
        }
        [HttpGet]
        public JsonResult GetClearancePaymentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetClearancePaymentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ClearancePaymentApprove(string CLPId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.CLPCommitmentBalanceUpdate(Convert.ToInt32(CLPId), false, false, userId, "CLP");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ClearancePaymentApprove(Convert.ToInt32(CLPId), userId);
                if (!status)
                    coreAccountService.CLPCommitmentBalanceUpdate(Convert.ToInt32(CLPId), true, false, userId, "CLP");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetClearanceAgentDetails(int agentId, bool poNumberRequired = false, string transTypeCode = "", bool TDSRequired = false)
        {
            try
            {
                var output = coreAccountService.GetClearanceAgentDetails(agentId);
                if (poNumberRequired)
                    output.PONumberList = Common.GetClearancePaymentPONumberList(agentId, transTypeCode);
                if (TDSRequired)
                    output.TDSList = Common.GetClearanceAgentTDSList(agentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PreviousCLPBillHistory(int agentId)
        {
            try
            {
                ViewBag.data = coreAccountService.GetCLPBillHistoryList(agentId);
                return PartialView();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public ActionResult _CLPCommitment()
        {
            CommitmentModel model = new CommitmentModel();
            ViewBag.CommitmentType = Common.getCommitmentType();
            ViewBag.Purpose = Common.getPurpose();
            ViewBag.Currency = Common.getCurrency();
            ViewBag.BudgetHead = Common.getBudgetHead();
            ViewBag.ProjectType = Common.getprojecttype();
            //   ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.AccountGroup = Common.getAccountGroup();
            ViewBag.AccountHead = Common.getBudgetHead();
            ViewBag.ClearanceAgent = Common.getClearanceAgent();
            //model = coreAccountService.GetTempAdvDetailsForCommitment(temporaryAdvanceId);
            //if (model.selRequestRefrence == 0)
            //    throw new Exception();
            // model.TemporaryAdvanceId = temporaryAdvanceId;
            model.CommitmentNo = "0";
            model.commitmentValue = 0;
            model.currencyRate = 0;
            return PartialView(model);

        }


        [HttpGet]
        public JsonResult GetCLPServiceTypeDetail(int serviceType)
        {
            try
            {
                var data = Common.GetCLPServiceDetail(serviceType);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult _CLPSaveCommitment(CommitmentModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            AccountService _AS = new AccountService();
            object output = _AS.SaveCommitDetails(model, UserId, true);
            // object output = coreAccountService.SearchSummerInternshipList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadCommitmentList(Int32[] CommitmentId)
        {
            try
            {
                CommitmentId = CommitmentId.Distinct().ToArray();
                object output = Common.GetCommitmentlistbyId(CommitmentId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //[AcceptVerbs(HttpVerbs.Get)]
        //public JsonResult LoadCommitmentList(Int32[] CommitmentId)
        //{
        //    CommitmentId = CommitmentId.Distinct().ToArray();
        //    var projectData = Common.GetCommitmentlistbyId(CommitmentId);
        //    return Json(projectData, JsonRequestBehavior.AllowGet);
        //}
        #endregion
        #region BankImprest 
        #region Imprest Payment
        public ActionResult ImprestPaymentList()
        {
            var servicetype = Common.getservicetype();
            var invoicetype = Common.getinvoicetype();
            var projecttype = Common.getprojecttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.Project = emptyList;
            ViewBag.typeofservice = servicetype;
            ViewBag.TypeofInvoice = invoicetype;
            ViewBag.projecttype = projecttype;
            ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.Gender = Common.getGender();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
            return View();
        }
        public ActionResult ImprestPayment(int PIID = 0, int ImpID = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                model.TotalValueofCard = 0;
                model.CurrentImprestValue = 0;
                model.PIId = PIID;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (PIID > 0 && ImpID > 0 && Common.ValidateImprestOnEdit(ImpID, "IMP"))
                {
                    
                    model = coreAccountService.EditImprestPaymentDetails(PIID, ImpID);
                    model.CreditorType = "PI";
                }
                else if (PIID > 0 && ImpID == 0)
                {
                    string validationMsg = Common.ValidateImprestonAddition(PIID);
                    if (validationMsg != "Valid")
                    {
                        ViewBag.errMsg = validationMsg;
                        return View(model);
                    }
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                }

                else if (PIID == 0 && ImpID == 0)
                {
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ImprestPaymentModel model = new ImprestPaymentModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult ImprestPayment(ImprestPaymentModel model)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                
                CoreAccountsService _ps = new CoreAccountsService();
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateIMPaymentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                model.NeedUpdateTransDetail = true;
                                return View(model);
                            }
                        }
                    }

                    var ImprestCardID = _ps.CreateImprestPayment(model, loggedinuserid);
                    if (ImprestCardID > 0)
                    {
                        var CardNumber = Common.getImprestcardnumber(ImprestCardID);
                        ViewBag.succMsg = "Imprest payment added to Imprest Card with Card number - " + CardNumber + ".";
                    }
                    else if (ImprestCardID == -2)
                    {
                        var CardNumber = Common.getImprestcardnumber(model.ImprestcardId);
                        ViewBag.succMsg = CardNumber + " - Imprest Payment details updated.";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    model.NeedUpdateTransDetail = true;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                model.NeedUpdateTransDetail = true;
                return View(model);
            }
        }
        private string ValidateIMPaymentBillPayment(ImprestPaymentModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            // decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            var amt = model.ImprestValue;
            var sancamt = model.TotalProjectsValue;
            var previmptotal = model.TotalPrevImprestValue;
            var imprestpercent = (sancamt * 20) / 100;
            decimal? balance = 0;
            if (imprestpercent < 500000)
            {
                balance = imprestpercent - previmptotal;
                if (balance < amt)
                {
                    msg = "Imprest amount claimed cannot be greater than 20% of the total projects value.";
                }
            }
            if (imprestpercent > 500000)
            {
                balance = 500000 - previmptotal;
                if (balance < amt)
                {
                    msg = "Imprest amount claimed cannot be greater than Rs 500000.";
                }
            }

            return msg;
        }

        [HttpGet]
        public JsonResult LoadImprestPaymentList()
        {
            try
            {
                object output = coreAccountService.GetImprestPaymentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult LoadImprestProjectDetails(string PIID)
        {
            try
            {
                PIID = PIID == "" ? "0" : PIID;
                object output = coreAccountService.GetProjectdetailsbyPI(Convert.ToInt32(PIID));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchImprestPaymentList(ImprestPaymentSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchImprestPaymentList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ImprestPaymentView(int PIID = 0, int ImpID = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                //  model = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectId));
                model = coreAccountService.EditImprestPaymentDetails(PIID, ImpID);
                ViewBag.disabled = "Disabled";
                model.CreditorType = "PI";
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult ImprestPaymentProjectApprove(string prjctdetailsid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.ImprestPaymentApprove(Convert.ToInt32(prjctdetailsid), userId);
                //if(status == true)
                //{
                //    bool cStatus = coreAccountService.SBIECardProjectBalanceUpdate(Convert.ToInt32(prjctdetailsid), false, false);
                //    if (!cStatus)
                //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Imprest Enhancement
        public ActionResult ImprestEnhancementList()
        {
            var servicetype = Common.getservicetype();
            var invoicetype = Common.getinvoicetype();
            var projecttype = Common.getprojecttype();
            var emptyList = new List<MasterlistviewModel>();
            ViewBag.Project = emptyList;
            ViewBag.typeofservice = servicetype;
            ViewBag.TypeofInvoice = invoicetype;
            ViewBag.projecttype = projecttype;
            ViewBag.PIName = Common.GetPIWithDetails();
            ViewBag.Gender = Common.getGender();
            ViewBag.DocmentTypeList = Common.GetDocTypeList(39);
            return View();
        }
        public ActionResult ImprestEnhancement(int PIID = 0, int IMEID = 0)
        {
            try
            {

                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                model.TotalValueofCard = 0;
                model.CurrentImprestValue = 0;
                model.PIId = PIID;
                model.CurrentFinancialYear = Common.GetCurrentFinYear();
                if (PIID > 0 && IMEID > 0 && Common.ValidateImprestEnhanceOnEdit(IMEID, "IME"))
                {

                    
                    model = coreAccountService.EditImprestEnhanceDetails(PIID, IMEID);
                    model.CreditorType = "PI";
                }
                else if (PIID > 0 && IMEID == 0)
                {
                    string validationMsg = Common.ValidateImprestonAddition(PIID);
                    if (validationMsg != "Valid")
                    {
                        ViewBag.errMsg = validationMsg;
                        return View(model);
                    }
                    model = coreAccountService.GetIMPEnhancedetailsbyPI(PIID);
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                }

                else if (PIID == 0 && IMEID == 0)
                {
                    TempData["errMsg"] = "Imprest Account does not exist.";
                    model.CreditorType = "PI";
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(50);
                    return View(model);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ImprestPaymentModel model = new ImprestPaymentModel();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult ImprestEnhancement(ImprestPaymentModel model)
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.Bank = Common.getBank();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                
                CoreAccountsService _ps = new CoreAccountsService();
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateIMPaymentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        model.NeedUpdateTransDetail = true;
                        return View(model);
                    }

                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                model.NeedUpdateTransDetail = true;
                                return View(model);
                            }
                        }
                    }

                    var ImprestCardID = _ps.CreateImprestEnhance(model, loggedinuserid);
                    if (ImprestCardID > 0)
                    {
                        var CardNumber = Common.getImprestcardnumber(ImprestCardID);
                        ViewBag.succMsg = "Imprest Enhancement done successfully";
                    }
                    else if (ImprestCardID == -2)
                    {
                        var CardNumber = Common.getImprestcardnumber(model.ImprestcardId);
                        ViewBag.succMsg = "Imprest Enhancement updated successfully.";
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    model.NeedUpdateTransDetail = true;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                model.NeedUpdateTransDetail = true;
                return View(model);
            }
        }
        private string ValidateImpEnhanceBill(ImprestPaymentModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            // decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            var TransAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV != true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVExpVal = model.ExpenseDetail.Where(m => m.TransactionType == "Debit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            decimal ttlJVDrVal = model.ExpenseDetail.Where(m => m.TransactionType == "Credit" && m.IsJV == true).Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlJVExpVal != ttlJVDrVal)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value of JV are not equal" : msg + "<br />Not a valid entry. Credit and Debit value of JV are not equal";
            //if (ttlExpAmt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            if (model.ExpenseDetail != null)
            {
                //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
                //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
                var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
                var gAH = ah.GroupBy(v => v.AccountHeadId);
                if (ah.Count() != gAH.Count())
                    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            }
            var amt = model.ImprestValue;
            var sancamt = model.TotalProjectsValue;
            var previmptotal = model.TotalPrevImprestValue;
            var imprestpercent = (sancamt * 20) / 100;
            decimal? balance = 0;
            if (imprestpercent < 500000)
            {
                balance = imprestpercent - previmptotal;
                if (balance < amt)
                {
                    msg = "Imprest amount claimed cannot be greater than 20% of the total projects value.";
                }
            }
            if (imprestpercent > 500000)
            {
                balance = 500000 - previmptotal;
                if (balance < amt)
                {
                    msg = "Imprest amount claimed cannot be greater than Rs 500000.";
                }
            }

            return msg;
        }

        [HttpGet]
        public JsonResult LoadImprestEnhanceList()
        {
            try
            {
                object output = coreAccountService.GetImprestEnhancementList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult LoadImprestEnhanceDetails(string PIID)
        {
            try
            {
                PIID = PIID == "" ? "0" : PIID;
                object output = coreAccountService.GetProjectdetailsbyPI(Convert.ToInt32(PIID));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult SearchImprestEnhanceList(ImprestPaymentSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchImprestEnhanceList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ImprestEnhancementView(int PIID = 0, int IMEID = 0)
        {
            try
            {
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Gender = Common.getGender();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(50);
                ViewBag.Bank = Common.getBank();
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                //  model = coreAccountService.GetProjectCardDetails(Convert.ToInt32(ProjectId));
                model = coreAccountService.EditImprestEnhanceDetails(PIID, IMEID);
                ViewBag.disabled = "Disabled";
                model.CreditorType = "PI";
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult ImprestEnhanceApprove(string prjctdetailsid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool status = coreAccountService.ImprestEnhancementApprove(Convert.ToInt32(prjctdetailsid), userId);
                //if(status == true)
                //{
                //    bool cStatus = coreAccountService.SBIECardProjectBalanceUpdate(Convert.ToInt32(prjctdetailsid), false, false);
                //    if (!cStatus)
                //        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Imprest Recoupment
        [HttpGet]
        public JsonResult GetImprestRecoupmentList(string typeCode)
        {
            try
            {
                object output = coreAccountService.GetImprestRecoupmentList(typeCode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult ImprestPaymentRecoupmentList()
        {
            return View();
        }
        public ActionResult ImprestPaymentRecoupment(int ImprestRecoupId = 0, int ImprestCardId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(49);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                model.ImprestcardId = ImprestCardId;

                if (ImprestRecoupId > 0)
                {
                    model = coreAccountService.GetImprestRecoupmentDetails(ImprestRecoupId);
                    
                    //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                    //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                }
                else
                {
                    model = coreAccountService.GetImprestCardDetails(ImprestCardId);
                    model.NeedUpdateTransDetail = true;
                    model.CheckListDetail = Common.GetCheckedList(49);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult ImprestRecoupmentView(int ImprestRecoupId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.PONumberList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(49);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ImprestPaymentModel model = new ImprestPaymentModel();
                if (ImprestRecoupId > 0)
                {
                    model = coreAccountService.GetImprestRecoupmentDetails(ImprestRecoupId);
                }
                ViewBag.disabled = "Disabled";
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult ImprestPaymentRecoupment(ImprestPaymentModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                
                //ViewBag.TypeOfServiceList = Common.GetTypeOfServiceList(model.BillType ?? 0);
                //ViewBag.PONumberList = Common.GetBillPONumberList(model.VendorId);
                ViewBag.DocmentTypeList = Common.GetDocTypeList(49);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                // ModelState.Remove("AdvancePercentage");
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateImprestRecoupmentBillPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ImprestRecoupment(model, logged_in_user);
                    if (model.RecoupmentId == 0 && result > 0)
                    {
                        ViewBag.succMsg = "Imprest Bill Booking done successfully.";

                    }
                    else if (model.RecoupmentId > 0 && result > 0)
                    {
                        ViewBag.succMsg = "Imprest Bill Booking has been updated successfully.";
                    }
                    //else if (result == -2)
                    //    TempData["errMsg"] = "Bill already exists for this PO Number with the Vendor.";
                    else if (result == -3)
                        TempData["errMsg"] = "Please select the valid commitment from the list.";
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.VendorList = Common.GetVendorList();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BillTypeList = Common.GetBillTypeList();
                // ViewBag.UOMList = Common.GetCodeControlList("UOM");
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.AccountGroupList =
                ViewBag.AccountHeadList = emptyList;
                

                ViewBag.DocmentTypeList = Common.GetDocTypeList(45);
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateImprestRecoupmentBillPayment(ImprestPaymentModel model)
        {
            // decimal netAdvAmt = model.TemporaryAdvanceValue ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (ttlExpAmt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and transaction value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and transaction value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public ActionResult ImprestRecoupmentApprove(int recoupmentId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.ImprestRecoupmentBalanceUpdate(recoupmentId, false, false, userId, "IMR");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ImprestRecoupmentBillApproved(recoupmentId, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetPIdetailsbyProject(string PIId)
        {
            try
            {
                object output = Common.GetPIdetailsbyProject(Convert.ToInt32(PIId));
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchImprestRecoupmentList(ImprestPaymentSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchImprestRecoupmentList(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
        #region Imprest Bills Recoupment
        public ActionResult ImprestBillsRecoupList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetImprestBillsRecoupList()
        {
            try
            {
                object output = coreAccountService.GetImprestRecoupBillList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveImprestBillsRecoup(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveImprestBillRecoupment(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ImprestBillsRecoupment(int id = 0, int BillRecoupid = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ImprestBillRecoupModel model = new ImprestBillRecoupModel();
                if (id == 0 && BillRecoupid > 0 && Common.ValidateImprestBillRecoupStatus(BillRecoupid, "Open"))
                {
                    model = coreAccountService.GetIMPBillRecoupDetails(BillRecoupid);
                    
                }
                if (id > 0 && BillRecoupid == 0 && Common.ValidateImprestBillStatus(id, "Approved"))
                {
                    model = coreAccountService.GetIMPBillDetails(id);
                }
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ImprestBillsRecoupment(ImprestBillRecoupModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                
                foreach (var item in model.CrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                foreach (var item in model.DrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateImprestBillsRecoupment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ImprestBillRecoupIU(model, logged_in_user);
                    if (model.ImprestBillRecoupId == null && result > 0)
                    {
                        TempData["succMsg"] = "Imprest Bills Recoupment has been added successfully.";
                        return RedirectToAction("ImprestBillsRecoupList");
                    }
                    else if (model.ImprestBillRecoupId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Imprest Bills Recoupment has been updated successfully.";
                        return RedirectToAction("ImprestBillsRecoupList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult ImprestBillsRecoupmentView(int id)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ViewBag.disabled = "disabled";
                ImprestBillRecoupModel model = new ImprestBillRecoupModel();
                model = coreAccountService.GetIMPBillRecoupDetails(id);
                
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateImprestBillsRecoupment(ImprestBillRecoupModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.DrDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlCrAmt != ttlDrAmt && ttlCrAmt != 0)
                msg = "Not a valid entry. Credit and Debit value are not equal";
            var gCrBH = model.CrDetail.GroupBy(v => v.AccountHeadId);
            if (model.CrDetail.Count() != gCrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Credit details. Please select a different head." : msg + "<br /> Duplicate head exist in Credit details. Please select a different head.";

            var gDrBH = model.DrDetail.GroupBy(v => v.AccountHeadId);
            if (model.DrDetail.Count() != gDrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Debit details. Please select a different head." : msg + "<br /> Duplicate head exist in Debit details. Please select a different head.";
            foreach (var item in model.CrDetail)
            {
                int headId = item.AccountHeadId ?? 0;
                decimal balAmt = Common.GetAccountHeadBalance(headId);
                if (balAmt < item.Amount)
                {
                    msg = msg == "Valid" ? "Some of the amount exceed balance amount. Please correct and submit again." : msg + "<br /> Some of the amount exceed balance amount. Please correct and submit again.";
                    break;
                }
            }
            return msg;
        }

        #endregion
        #endregion
        #region Commitment
        public ActionResult _BookCommitment()
        {
            CommitmentModel model = new CommitmentModel();
            ViewBag.CommitmentType = Common.getCommitmentType();
            ViewBag.Purpose = Common.getPurpose();
            ViewBag.Currency = Common.getCurrency();
            ViewBag.BudgetHead = Common.getBudgetHead();
            ViewBag.Employee = Common.GetEmployeeName();
            ViewBag.AccountHead = Common.getBudgetHead();
            ViewBag.ProjectNo = Common.getProjectNumber();
            ViewBag.Vendor = Common.getVendor();
            ViewBag.RequestRef = Common.getprojectsource();
            ViewBag.FundingBody = Common.GetFundingBody(model.SelProjectNumber);
            ViewBag.RefNo = new List<MasterlistviewModel>();
            model.CommitmentNo = "0";
            model.commitmentValue = 0;
            model.currencyRate = 0;
            return PartialView(model);

        }

        [HttpPost]
        public JsonResult _SaveCommitment(CommitmentModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            AccountService _AS = new AccountService();
            object output = _AS.SaveCommitDetails(model, UserId, true);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Journal

        public ActionResult JournalList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetJournalList()
        {
            try
            {
                object output = coreAccountService.GetJournalList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveJournal(int journalId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveJournal(journalId, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Journal(int journalId = 0)
        {
            try
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(55);
                JournalModel model = new JournalModel();
                if (journalId > 0 && Common.ValidateJournalStatus(journalId, "Open"))
                {
                    model = coreAccountService.GetJournalDetails(journalId);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        [HttpPost]
        public ActionResult Journal(JournalModel model)
        {
            try
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(55);
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountGroupList = Common.GetAccountGroup(false);
                    item.AccountHeadList = Common.GetAccountHeadList(headId, 0, "", "", false);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateJournal(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string taxprooffilename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(taxprooffilename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.JournalIU(model, logged_in_user);
                    if (model.JournalId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Journal has been added successfully.";
                        return RedirectToAction("JournalList");
                    }
                    else if (model.JournalId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Journal has been updated successfully.";
                        return RedirectToAction("JournalList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(55);
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult JournalView(int journalId)
        {
            try
            {
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.ReasonList = Common.GetCodeControlList("Journal Reason");
                ViewBag.DocmentTypeList = Common.GetDocTypeList(36);
                JournalModel model = new JournalModel();
                model = coreAccountService.GetJournalDetails(journalId);
                ViewBag.disabled = "disabled";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateJournal(JournalModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;

            if (ttlCrAmt != ttlDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";

            if (ttlCrAmt == 0)
                msg = msg == "Valid" ? "Please enter the valid credit and debit value." : msg + "<br /> Please enter the valid credit and debit value.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var gAH = model.ExpenseDetail.GroupBy(v => v.AccountHeadId);
            //if (model.ExpenseDetail.Count() != gAH.Count())
            //    msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";

            return msg;
        }
        #endregion
        #region Adhoc Payment 
        public ActionResult AdhocPaymentList()
        {
            return View();
        }
        public ActionResult AdhocPayment(int adhocId = 0)
        {
            try
            {
                var emptyList = new List<AdhocPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentMode");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(59);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                AdhocPaymentModel model = new AdhocPaymentModel();
                model.CreditorType = "PI/Student/Others";
                if (adhocId > 0 && Common.ValidateAdhocPaymentOnEdit(adhocId))
                {
                    model = coreAccountService.GetAdhocPaymentDetails(adhocId);
                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(59);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult AdhocPaymentView(int adhocId)
        {

            try
            {
                var emptyList = new List<AdhocPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentMode");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(59);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                AdhocPaymentModel model = new AdhocPaymentModel();

                model = coreAccountService.GetAdhocPaymentDetails(adhocId);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdhocPayment(AdhocPaymentModel model)
        {
            try
            {
                var emptyList = new List<AdhocPaymentModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentMode");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(59);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateAdhocPayment(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateAdhocPayment(model, logged_in_user);
                    if (model.AdhocId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Ad hoc payment has been added successfully.";
                        return RedirectToAction("AdhocPaymentList");
                    }
                    else if (model.AdhocId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Ad hoc payment has been updated successfully.";
                        return RedirectToAction("AdhocPaymentList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateAdhocPayment(AdhocPaymentModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal adhocamt = model.NetPayableValue ?? 0;
            decimal eligtax = model.EligibleTaxValue ?? 0;
            decimal paymentamt = adhocamt - eligtax;

            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (paymentamt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public JsonResult GetAdhocPaymentList()
        {
            try
            {
                object output = coreAccountService.GetAdhocPaymentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchAdhocPaymentList(AdhocPaySearchFieldModel model)
        {
            object output = coreAccountService.SearchAdhocPaymentList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult AdhocPaymentApprove(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.AdhocPaymentBillApproved(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetTransactionTypecode(string Paymenttype)
        {
            Paymenttype = Paymenttype == "" ? "0" : Paymenttype;
            var locationdata = Common.gettranstypecode(Convert.ToInt32(Paymenttype));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPIDetailsbyName(string Name)
        {
            var locationdata = coreAccountService.getPIDetails(Name);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region ClaimBill
        public ActionResult ClaimBillList()
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                //if (user_role == 8)
                //{
                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetInvoicedetails();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.ProjectNumberList = emptyList;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                var data = new PagedData<ClaimBillSearchResultModel>();
                ClaimBillListModel model = new ClaimBillListModel();
                ClaimBillSearchFieldModel srchModel = new ClaimBillSearchFieldModel();
                data = coreAccountService.GetClaimbillList(srchModel, page, pageSize);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);
                //}
                //if (user_role == 7)
                //{
                //    int page = 1;
                //    int pageSize = 5;
                //    ViewBag.PIName = Common.GetPIWithDetails();
                //    var Projecttitle = Common.GetPIProjectdetails(logged_in_userid);
                //    var projecttype = Common.getprojecttype();
                //    var invoicetype = Common.getinvoicetype();
                //    var Invoice = Common.GetInvoicedetails();
                //    ViewBag.Project = Projecttitle;
                //    ViewBag.projecttype = projecttype;
                //    ViewBag.TypeofInvoice = invoicetype;
                //    ViewBag.Invoice = Invoice;
                //    var data = new PagedData<InvoiceSearchResultModel>();
                //    InvoiceListModel model = new InvoiceListModel();
                //    ProjectService _ps = new ProjectService();
                //    InvoiceSearchFieldModel srchModel = new InvoiceSearchFieldModel();
                //    srchModel.PIName = logged_in_userid;
                //    data = _ps.GetPIInvoiceList(srchModel, page, pageSize);
                //    model.Userrole = user_role;
                //    model.SearchResult = data;
                //    return View(model);
                //}
                //return RedirectToAction("DashBoard", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchClaimBillList(ClaimBillSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<ClaimBillSearchResultModel>();
                ClaimBillListModel model = new ClaimBillListModel();
                CoreAccountsService _cs = new CoreAccountsService();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-1);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-1);
                //}

                data = _cs.GetClaimbillList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }

        public ActionResult ClaimBill(int pId = 0)
        {
            try
            {
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (user_role != 7 && user_role != 8)
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}                    
                ClaimBillModel model = new ClaimBillModel();
                CoreAccountsService _ps = new CoreAccountsService();
                model = _ps.GetProjectDetails(pId);
                if (model.AvailableBalance <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice";
                }
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "No balance available for raising Invoice for this financial year";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ClaimBill(ClaimBillModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "Claim Bill Cannot be generated. No balance available for raising Claim Bill";
                    return View(model);
                }
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");             

                var InvoiceID = coreAccountService.CreateClaimBill(model, loggedinuserid);
                if (InvoiceID == -4)
                {
                    ViewBag.errMsg = "Claim Bill Cannot be generated as the Taxable value has exceeded the balance available for raising Claim Bill. Please enter correct value and try again.";
                    return View(model);
                }
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                    ViewBag.succMsg = "Claim Bill has been created successfully with Claim Bill Number - " + InvoiceNumber + ".";
                }
                else if (InvoiceID == -2)
                {
                    var InvoiceId = Convert.ToInt32(model.InvoiceId);
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceId);
                    ViewBag.succMsg = "Claim Bill with Claim Bill number - " + InvoiceNumber + " has been updated successfully.";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult PickDraftClaimBill(int DraftId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");
                ClaimBillModel model = new ClaimBillModel();
                model = coreAccountService.GetClaimBillDraftDetails(DraftId);

                return View("ProjectInvoice", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult DraftClaimBill(ClaimBillModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");                
                var InvoiceDraftID = coreAccountService.DraftClaimBill(model, loggedinuserid);
                if (InvoiceDraftID > 0)
                {
                    ViewBag.succMsg = "Invoice has been saved as draft";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View("ClaimBill", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult EditClaimBill(int ClaimBillId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ClaimBillModel model = new ClaimBillModel();
                ProjectService _ps = new ProjectService();
                model = coreAccountService.GetClaimBillDetails(ClaimBillId);

                return View("ClaimBill", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSponProjectList(string PIId)
        {
            PIId = PIId == "" ? "0" : PIId;
            var locationdata = coreAccountService.LoadSponProjecttitledetails(Convert.ToInt32(PIId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadClaimBillList(string ProjectId)
        {
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = coreAccountService.LoadClaimBillList(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult LoadTaxpercentage(string servicetype)
        {
            servicetype = servicetype == "" ? "0" : servicetype;
            object output = coreAccountService.gettaxpercentage(Convert.ToInt32(servicetype));
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Project Fund Transfer
        public ActionResult ProjectFundTransferList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetProjectFundTransferList()
        {
            try
            {
                object output = coreAccountService.GetProjectFundTransferList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveProjectFundTransfer(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveProjectFundTransfer(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ProjectFundTransfer(int id = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                ProjectFundTransferModel model = new ProjectFundTransferModel();
                if (id > 0 && Common.ValidateProjectFundTransferStatus(id, "Open"))
                {
                    model = coreAccountService.GetProjectFundTransferDetails(id);
                   
                }
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ProjectFundTransfer(ProjectFundTransferModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
               ViewBag.BudgetHeadId = Common.getBudgetHead();
                
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateProjectFundTransfer(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ProjectFundTransferIU(model, logged_in_user);
                    if (model.ProjectTransferId == null && result > 0)
                    {
                        TempData["succMsg"] = "Fund transfer has been added successfully.";
                        return RedirectToAction("ProjectFundTransferList");
                    }
                    else if (model.ProjectTransferId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Fund transfer has been updated successfully.";
                        return RedirectToAction("ProjectFundTransferList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult ProjectFundTransferView(int id)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.BudgetHeadId = Common.getBudgetHead();
                ViewBag.disabled = "disabled";
                ProjectFundTransferModel model = new ProjectFundTransferModel();
                model = coreAccountService.GetProjectFundTransferDetails(id);
                
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateProjectFundTransfer(ProjectFundTransferModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.DrDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlCrAmt != ttlDrAmt && ttlCrAmt != 0)
                msg = "Not a valid entry. Credit and Debit value are not equal";
            var gCrBH = model.CrDetail.GroupBy(v => v.BudgetHeadId);
            if (model.CrDetail.Count() != gCrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Credit details. Please select a different head." : msg + "<br /> Duplicate head exist in Credit details. Please select a different head.";

            var gDrBH = model.DrDetail.GroupBy(v => v.BudgetHeadId);
            if (model.DrDetail.Count() != gDrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Debit details. Please select a different head." : msg + "<br /> Duplicate head exist in Debit details. Please select a different head.";

            return msg;
        }
        #endregion
        #region Transaction Definition
        [HttpGet]
        public ActionResult TransactionAndTaxesList()
        {
            List<TransactionAndTaxesModel> model = new List<TransactionAndTaxesModel>();
            var transactiontype = Common.GetTransactionType();
            ViewBag.TransType = transactiontype;
            var subcode = Common.GetSubCode();
            ViewBag.subcode = subcode;
            var group = Common.GetAccountGroupList();
            ViewBag.Group = group;
            var head = Common.GetAccountHeadList();
            ViewBag.Head = head;
            var category = Common.GetDeductionCategory();
            ViewBag.Category = category;
            return View();
        }
        public ActionResult Transaction(string transaction, string subcode)
        {
            var model = CoreAccountsService.Transaction(transaction, subcode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddTransaction(string group, string head, string type, string isjv, string transaction, string subcode)
        {
            var model = CoreAccountsService.AddTransaction(group, head, type, isjv, transaction, subcode);
            return Json(model, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DeleteTransaction(string transdefid)
        {
            var model = CoreAccountsService.DeleteTransaction(transdefid);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Taxes(string transaction)
        {
            var model = CoreAccountsService.Taxes(transaction);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTaxes(int deheadid)
        {
            var model = CoreAccountsService.DeleteTaxes(deheadid);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddTaxes(string taxgroup, string taxhead, string taxcategory, string taxinterstate, string transaction)
        {
            var model = CoreAccountsService.AddTaxes(taxgroup, taxhead, taxcategory, taxinterstate, transaction);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadAccHead(string accgrp)
        {

            accgrp = accgrp == "" ? "0" : accgrp;
            var locationdata = Common.LoadGrpWiseHeadList(Convert.ToInt32(accgrp));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSubCode(string transtype)
        {

            transtype = transtype == "" ? "0" : transtype;
            var locationdata = Common.LoadSubCodeList(transtype);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Credit Note
        public ActionResult CreditNoteList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetCreditNoteList()
        {
            try
            {
                object output = coreAccountService.GetCreditNoteList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveCreditNote(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveCreditNote(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult CreditNote(int creditNoteId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                CreditNoteModel model = new CreditNoteModel();
                if (creditNoteId > 0)
                {
                    model = coreAccountService.GetInvoiceDetailsForCreditNote(creditNoteId, true);
                   
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult CreditNote(CreditNoteModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                if (ModelState.IsValid)
                {
                    bool editMode = model.CreditNoteId == null ? false : true;
                    decimal avlBal = 0;
                    if (editMode)
                        avlBal = Common.GetAvailableAmtForCreditNote(model.CreditNoteId ?? 0, editMode);
                    else
                        avlBal = Common.GetAvailableAmtForCreditNote(model.InvoiceId ?? 0, editMode);
                    if (avlBal < model.TotalCreditAmount)
                    {
                        TempData["errMsg"] = "Credit note amount should not be grater than invoice balance amount.";
                        return View(model);
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreditNoteIU(model, logged_in_user);
                    if (model.CreditNoteId == null && result > 0)
                    {
                        TempData["succMsg"] = "Credit note has been added successfully.";
                        return RedirectToAction("CreditNoteList");
                    }
                    else if (model.CreditNoteId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Credit note has been updated successfully.";
                        return RedirectToAction("CreditNoteList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                return View();
            }
        }

        public ActionResult CreditNoteView(int creditNoteId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                ViewBag.disabled = "disabled";
                CreditNoteModel model = new CreditNoteModel();
                model = coreAccountService.GetInvoiceDetailsForCreditNote(creditNoteId, true);
                
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }
        [HttpGet]
        public JsonResult LoadInvoiceList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteInvoceNumber(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInvoiceDetail(int invoiceId)
        {
            try
            {
                var data = coreAccountService.GetInvoiceDetailsForCreditNote(invoiceId, false);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #region Contra
        public ActionResult ContraList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetContraList()
        {
            try
            {
                object output = coreAccountService.GetContraList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveContra(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveContra(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Contra(int id = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ContraModel model = new ContraModel();
                if (id > 0 && Common.ValidateContraStatus(id, "Open"))
                {
                    model = coreAccountService.GetContraDetails(id);
                    
                }
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult Contra(ContraModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                
                foreach (var item in model.CrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                foreach (var item in model.DrDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateContra(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.ContraIU(model, logged_in_user);
                    if (model.ContraId == null && result > 0)
                    {
                        TempData["succMsg"] = "Contra has been added successfully.";
                        return RedirectToAction("ContraList");
                    }
                    else if (model.ContraId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Contra has been updated successfully.";
                        return RedirectToAction("ContraList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =
                
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult ContraView(int id)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList =emptyList;
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ViewBag.disabled = "disabled";
                ContraModel model = new ContraModel();
                model = coreAccountService.GetContraDetails(id);
                
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        private string ValidateContra(ContraModel model)
        {
            string msg = "Valid";
            decimal ttlCrAmt = model.CrDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttlDrAmt = model.DrDetail.Select(m => m.Amount).Sum() ?? 0;
            if (ttlCrAmt != ttlDrAmt && ttlCrAmt != 0)
                msg = "Not a valid entry. Credit and Debit value are not equal";
            var gCrBH = model.CrDetail.GroupBy(v => v.AccountHeadId);
            if (model.CrDetail.Count() != gCrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Credit details. Please select a different head." : msg + "<br /> Duplicate head exist in Credit details. Please select a different head.";

            var gDrBH = model.DrDetail.GroupBy(v => v.AccountHeadId);
            if (model.DrDetail.Count() != gDrBH.Count())
                msg = msg == "Valid" ? "Duplicate head exist in Debit details. Please select a different head." : msg + "<br /> Duplicate head exist in Debit details. Please select a different head.";
            foreach (var item in model.CrDetail)
            {
                int headId = item.AccountHeadId ?? 0;
                decimal balAmt = Common.GetAccountHeadBalance(headId);
                if (balAmt < item.Amount)
                {
                    msg = msg == "Valid" ? "Some of the amount exceed balance amount. Please correct and submit again." : msg + "<br /> Some of the amount exceed balance amount. Please correct and submit again.";
                    break;
                }
            }
            return msg;
        }

        [Authorized]
        public JsonResult GetAccountHeadBalance(int hdId)
        {
            object output = Common.GetAccountHeadBalance(hdId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Distribution 
        public ActionResult DistributionList()
        {
            return View();
        }
        public ActionResult Distribution(int distributionId = 0)
        {
            try
            {
                var emptyList = new List<DistributionModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(61);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                DistributionModel model = new DistributionModel();
                model.CreditorType = "Professor/Staff";
                model.DistributionOverheads = Common.getInstituteOHPercentage();
                model.InstituteOverheadPercentage = model.DistributionOverheads.Select(m => m.OverheadPercentage).Sum() ?? 0;
                if (distributionId > 0 && Common.ValidateDistributionOnEdit(distributionId))
                {
                    model = coreAccountService.GetDistributionDetails(distributionId);
                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(61);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("CoreAccounts", "DistributionList");
            }

        }

        public ActionResult DistributionView(int distributionId)
        {

            try
            {
                var emptyList = new List<DistributionModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(61);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                DistributionModel model = new DistributionModel();

                model = coreAccountService.GetDistributionDetails(distributionId);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Distribution(DistributionModel model)
        {
            try
            {
                var emptyList = new List<DistributionModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(61);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateDistribution(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateDistribution(model, logged_in_user);
                    if (model.DistributionId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Distribution payment has been added successfully.";
                        return RedirectToAction("DistributionList");
                    }
                    else if (model.DistributionId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Distribution payment has been updated successfully.";
                        return RedirectToAction("DistributionList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.FacultyType = Common.GetCodeControlList("FacultyType");
                ViewBag.PaymentMode = Common.GetCodeControlList("DistributionPaymentMode");
                ViewBag.DistributionType = Common.GetCodeControlList("DistributionType");
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateDistribution(DistributionModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal paymentamt = model.DistributionAmount ?? 0;

            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            if (paymentamt != commitmentAmt)
                msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public JsonResult GetDistributionList()
        {
            try
            {
                object output = coreAccountService.GetDistributionList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchDistributionList(DistributionSearchFieldModel model)
        {
            object output = coreAccountService.SearchDistributionList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DistributionApprove(int DistributionId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                bool cStatus = coreAccountService.DistributionCommitmentBalanceUpdate(DistributionId, false, false, userId, "DIS");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.DistributionBillApproved(DistributionId, userId);
                if (!status)
                    coreAccountService.DistributionCommitmentBalanceUpdate(DistributionId, true, false, userId, "DIS");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult LoadDistributeProjectList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteProjects(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectsummary(string ProjectId)
        {
            AccountService _as = new AccountService();
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = _as.getProjectSummary(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectDuration(string ProjectId)
        {
            CoreAccountsService _as = new CoreAccountsService();
            ProjectId = ProjectId == "" ? "0" : ProjectId;
            var locationdata = _as.getProjectDuration(Convert.ToInt32(ProjectId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProjectIdbyNumber(string ProjectNumber)
        {

            ProjectNumber = ProjectNumber == "" ? "0" : ProjectNumber;
            var locationdata = Common.getProjectidbynumber(ProjectNumber);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult LoadStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteStaffList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadProfessorList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteProfList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult LoadProjectStaffList(string term)
        {
            try
            {
                var data = Common.GetAutoCompleteProjectStaffList(term);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetStaffDetailsbyId(int EmpId)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            var locationdata = _cs.getStaffDetails(EmpId);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPIDetailsbyId(string UserId)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            UserId = UserId == "" ? "0" : UserId;
            var locationdata = _cs.getPIDesigandDep(Convert.ToInt32(UserId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Honororium
        public ActionResult HonororiumView(int HonorId)
        {
            try
            {
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                HonororiumModel model = new HonororiumModel();
                model.CreditorType = "PI/Student/Others";
                model = coreAccountService.GetHonororiumDetails(HonorId);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();

            }

        }
        [HttpGet]
        public ActionResult Honororium(int HonorId = 0)
        {
            try
            {
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                HonororiumModel model = new HonororiumModel();
                model.CreditorType = "PI/Student/Others";
                if (HonorId > 0 && Common.ValidateHonororiumOnEdit(HonorId))
                {
                    model = coreAccountService.GetHonororiumDetails(HonorId);
                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(63);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult Honororium(HonororiumModel model)
        {
            try
            {
                var emptyList = new List<HonororiumModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {

                    string validationMsg = ValidateHonororium(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateHonororium(model, logged_in_user);
                    if (model.HonororiumId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Honororium has been added successfully.";
                        return RedirectToAction("HonororiumList");
                    }
                    else if (model.HonororiumId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Honororium has been updated successfully.";
                        return RedirectToAction("HonororiumList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                    }
                }

                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {

                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        public ActionResult HonororiumList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetHonororiumList()
        {
            try
            {
                object output = coreAccountService.GetHonororiumList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalForHonororium(int HonorId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {

                bool cStatus = coreAccountService.HonororiumCommitmentBalanceUpdate(HonorId, false, false, logged_in_user, "HON");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalForHonororium(HonorId, logged_in_user);
                if (!status)
                    coreAccountService.HonororiumCommitmentBalanceUpdate(HonorId, true, false, logged_in_user, "HON");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForHonororium(int HonorId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.HonororiumBillApproved(HonorId, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                //bool boaStatus = coreAccountService.HonororiumBOATransaction(HonorId);
                //var status = Common.ApprovalPendingForHonororium(HonorId, logged_in_user);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ValidateHonororium(HonororiumModel model)
        {

            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public JsonResult SearchHonororiumList(honororiumSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchHonororiumList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region  BRS
        [HttpGet]
        public ActionResult BRS(int BRSId = 0)
        {
            try
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BRSModel model = new BRSModel();
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult BRS(BRSModel model)
        {
            try
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                if (ModelState.IsValid)
                {
                    var isNotReconcile = model.txDetail.Any(m => m.Status == "Open");
                    if (isNotReconcile)
                    {
                        TempData["errMsg"] = "Some of the bank statement still not reconcile.";
                        return View(model);
                    }
                    else if (model.txDetail == null || model.txDetail.Count() == 0)
                    {
                        TempData["errMsg"] = "No bank statement exists.";
                        return View(model);
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.BRSIU(model, logged_in_user);
                    if (result > 0)
                    {
                        TempData["succMsg"] = "BRS has been added successfully.";
                        return RedirectToAction("BRSList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                return View(model);
            }

        }

        public ActionResult BRSList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetBRSList()
        {
            try
            {
                object output = coreAccountService.GetBRSList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult BRSView(int id)
        {
            try
            {
                ViewBag.BankHeadList = Common.GetBankAccountHeadList();
                BRSModel model = new BRSModel();
                model = coreAccountService.GetBRSDetails(id);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public JsonResult ImportBankStatement(HttpPostedFileBase file)
        {
            Utility _uty = new Utility();
            BRSModel model = new BRSModel();
            List<BankStatementDetailModel> list = new List<BankStatementDetailModel>();
            string extension = Path.GetExtension(file.FileName).ToLower();
            string connString = "";
            string[] validFileTypes = { ".xls", ".xlsx" };
            string actName = Path.GetFileName(file.FileName);
            var guid = Guid.NewGuid().ToString();
            var docName = guid + "_" + actName;
            string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/BankStatement"), docName);
            string msg = "Valid";
            model.DocumentActualName = actName;
            model.DocumentName = docName;
            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(Server.MapPath("~/Content/BankStatement"));
            }
            if (validFileTypes.Contains(extension))
            {
                if (System.IO.File.Exists(path1))
                { System.IO.File.Delete(path1); }
                file.SaveAs(path1);

                //Connection String to Excel Workbook  
                if (extension.ToLower().Trim() == ".csv")
                {
                    DataTable dt = _uty.ConvertCSVtoDataTable(path1);
                    list = Converter.GetBRSEntityList<BankStatementDetailModel>(dt);
                }
                else if (extension.ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
                {
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString);
                    list = Converter.GetBRSEntityList<BankStatementDetailModel>(dt);
                }
                else
                {
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    DataTable dt = _uty.ConvertXSLXtoDataTable(path1, connString);
                    list = Converter.GetBRSEntityList<BankStatementDetailModel>(dt);
                }

            }
            else
            {
                msg = "Please Upload Files in .xls or .xlsx format";
            }
            model.txDetail = list;
            return Json(new { status = msg, data = model }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetBOAPaymentDetails(DateTime frmDate, DateTime toDate, int headId)
        {
            try
            {
                toDate = toDate.Date.AddDays(1).AddTicks(-1);
                var data = coreAccountService.GetBOAPaymentDetails(frmDate, toDate, headId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult _AdhocTransaction(int indx, int headId, string txType)
        {
            try
            {
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.AccountHeadList = new List<MasterlistviewModel>();
                CommonPaymentModel model = new CommonPaymentModel();
                model = coreAccountService.GetAdhocTransaction(headId, txType);
                model.RefId = indx;
                return PartialView(model);

            }
            catch (Exception ex)
            {
                throw ex;// new Exception(ex.Message);
            }
        }
        #endregion
        #region GSTOffset
        [HttpGet]
        public ActionResult GSTOffset(int GSTOffsetId = 0)
        {
            try
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                GSTOffsetModel model = new GSTOffsetModel();
                model.CreditorType = "NA";
                if (GSTOffsetId > 0 && Common.ValidateGSTOffsetOnEdit(GSTOffsetId))
                {
                    model = coreAccountService.GetGSTOffsetDetails(GSTOffsetId);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GSTOffset(GSTOffsetModel model)
        {
            try
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;

                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                if (ModelState.IsValid)
                {
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateGSTOffset(model, logged_in_user);
                    if (model.GSTOffsetId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "GSTOffset has been added successfully.";
                        return RedirectToAction("GSTOffsetList");
                    }
                    else if (model.GSTOffsetId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "GSTOffset has been updated successfully.";
                        return RedirectToAction("GSTOffsetList");
                    }
                    else
                    {
                        TempData["errMsg"] = "Something went wrong please contact administrator.";
                    }
                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult GetGSTInputList(DateTime fromdate, DateTime todate)
        {
            try
            {
                object output = CoreAccountsService.GetGSTInputList(fromdate, todate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetGSTOutputList(DateTime fromdate, DateTime todate)
        {
            try
            {
                object output = CoreAccountsService.GetGSTOutputList(fromdate, todate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetTDSList(DateTime fromdate, DateTime todate)
        {
            try
            {
                object output = CoreAccountsService.GetTDSList(fromdate, todate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult GetPreviousGST()
        {
            try
            {
                object output = CoreAccountsService.GetPreviousGST();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GSTOffsetList()
        {
            return View();
        }
        public JsonResult ApprovalForGSTOffset(int GSTOffsetId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalForGSTOffset(GSTOffsetId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForGSTOffset(int GSTOffsetId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool boaStatus = coreAccountService.GSTOffsetBOATransaction(GSTOffsetId);
                if (!boaStatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalPendingForGSTOffset(GSTOffsetId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult GetGSTOffsetList()
        {
            try
            {
                object output = coreAccountService.GetGSTOffsetList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchGSTOffsetList(GSTOffsetSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchGSTOffsetList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GSTOffsetView(int GSTOffsetId = 0)
        {
            try
            {
                var emptyList = new List<GSTOffsetModel>();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                GSTOffsetModel model = new GSTOffsetModel();
                model.CreditorType = "NA";
                model = coreAccountService.GetGSTOffsetDetails(GSTOffsetId);
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Negative Balance
        public ActionResult NegativeBalanceList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetNegativeBalanceList()
        {
            try
            {
                object output = coreAccountService.GetNegativeBalanceList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveNegativeBalance(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveNegativeBalance(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult NegativeBalance(int id = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNo = Common.getProjectNumber();
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                NegativeBalanceModel model = new NegativeBalanceModel();
                if (id > 0 && Common.ValidateNegativeBalanceStatus(id, "Open"))
                {
                    model = coreAccountService.GetNegativeBalanceDetails(id);
                    
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult NegativeBalance(NegativeBalanceModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNo = Common.getProjectNumber();
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                

                if (ModelState.IsValid)
                {
                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.NegativeBalanceIU(model, logged_in_user);
                    if (model.NegativeBalanceId == 0 && result > 0)
                    {

                        TempData["succMsg"] = "Negative Balance has been added successfully.";
                        return RedirectToAction("NegativeBalanceList");
                    }
                    else if (model.NegativeBalanceId > 0 && result > 0)
                    {
                        AccountService _as = new AccountService();
                        var details = _as.getProjectSummary(Convert.ToInt32(model.ProjectId));
                        model.prjDetails = details;
                        TempData["succMsg"] = "Negative Balance has been updated successfully.";
                        return RedirectToAction("NegativeBalanceList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return View(model);
            }

        }
        public ActionResult NegativeBalanceView(int id)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNo = Common.getProjectNumber();
                ViewBag.AccountGroupList = Common.GetBankAccountGroup();
                ViewBag.disabled = "Disabled";
                NegativeBalanceModel model = new NegativeBalanceModel();
                model = coreAccountService.GetNegativeBalanceDetails(id);
                
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }

        public ActionResult _CloseNegativeBalance(int id)
        {
            CloseNegativeBalanceModel model = new CloseNegativeBalanceModel();
            model = coreAccountService.GetNegativeBalCloseDetails(id);
            return PartialView(model);

        }
        [HttpPost]
        public ActionResult _CloseNegativeBalance(CloseNegativeBalanceModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    if (model.Document != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(model.Document.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.SaveCloseNegativeBal(model, logged_in_user);
                    if (model.NegativeBalanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Negative Balance has been closed successfully.";
                        return RedirectToAction("NegativeBalanceList");
                    }

                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                    return RedirectToAction("NegativeBalanceList");
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();

                TempData["errMsg"] = "Somenthing went wrong please contact administrator.";
                return RedirectToAction("NegativeBalanceList");
            }

        }


        #endregion
        #region General Voucher
        public ActionResult GeneralVoucherList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetGeneralVoucherList()
        {
            try
            {
                object output = coreAccountService.GetGeneralVoucherList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveGeneralVoucher(int id)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    bool cStatus = coreAccountService.GVRCommitmentBalanceUpdate(id, false, false, userId, "GVR");
                    if (!cStatus)
                        return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                    bool output = coreAccountService.ApproveGeneralVoucher(id, userId);
                    if (!output)
                        coreAccountService.GVRCommitmentBalanceUpdate(id, true, false, userId, "GVR");
                    return Json(output, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult GeneralVoucher(int id = 0)
        {
            try
            {
                GeneralVoucherModel model = new GeneralVoucherModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.PaymentCategoryList = Common.GetCodeControlList("PaymentCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (id > 0 && Common.ValidateGeneralVoucherStatus(id, "Open"))
                {
                    model = coreAccountService.GetGeneralVoucherDetails(id);
                    ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                }
                else
                {
                    int[] heads = { 32, 33, 34 };
                    model.PaymentDeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        public ActionResult GeneralVoucher(GeneralVoucherModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.PaymentCategoryList = Common.GetCodeControlList("PaymentCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                foreach (var item in model.PaymentExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (model.PaymentCategory == 1)
                {
                    for (int i = 0; i < model.PaymentBreakDetail.Count(); i++)
                    {
                        ModelState.Remove("PaymentBreakDetail[" + i + "].CategoryId");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].ModeOfPayment");
                        ModelState.Remove("PaymentBreakDetail[" + i + "].PaymentAmount");
                    }
                    for (int i = 0; i < model.CommitmentDetail.Count(); i++)
                    {
                        ModelState.Remove("CommitmentDetail[" + i + "].PaymentAmount");
                    }
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateGeneralVoucher(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.GeneralVoucherIU(model, logged_in_user);
                    if (model.VoucherId == null && result > 0)
                    {
                        TempData["succMsg"] = "General voucher has been added successfully.";
                        return RedirectToAction("GeneralVoucherList");
                    }
                    else if (model.VoucherId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "General voucher has been updated successfully.";
                        return RedirectToAction("GeneralVoucherList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.PaymentCategoryList = Common.GetCodeControlList("PaymentCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                foreach (var item in model.PaymentExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                return View(model);
            }
        }
        public ActionResult GeneralVoucherView(int id)
        {
            try
            {
                GeneralVoucherModel model = new GeneralVoucherModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.PaymentCategoryList = Common.GetCodeControlList("PaymentCategory");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.PayerCategoryList = Common.GetCodeControlList("PayerCategory", "TAD");
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model = coreAccountService.GetGeneralVoucherDetails(id);
                ViewBag.paymentTDSAmount = model.PaymentTDSAmount.ToString();
                ViewBag.disabled = "Disabled";
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        private string ValidateGeneralVoucher(GeneralVoucherModel model)
        {
            string msg = "Valid";
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal crAmt = model.PaymentExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal drAmt = model.PaymentExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTax = model.PaymentDeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal paymentBUAmt = model.PaymentBreakDetail.Select(m => m.PaymentAmount).Sum() ?? 0;
            //paymentBUAmt = paymentBUAmt + (model.PaymentTDSAmount ?? 0);
            decimal bankAmt = model.PaymentBankAmount ?? 0;
            drAmt = drAmt + ttlTax;
            crAmt = crAmt + bankAmt;
            if (model.PaymentCategory != 1)
            {
                foreach (var item in model.CommitmentDetail)
                {
                    if (item.PaymentAmount > item.AvailableAmount)
                        msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
                }
                if (crAmt != model.CommitmentAmount)
                    msg = msg == "Valid" ? "There is a mismatch between the payment value and allocated commitment value. Please update the value to continue." : msg + "<br /> There is a mismatch between the payment value and allocated commitment value. Please update the value to continue.";
                if (bankAmt != paymentBUAmt)
                    msg = msg == "Valid" ? "Not a valid entry.The Payable value and Payment Break Up Total value are not equal." : msg + "<br /> Not a valid entry.The Payable value and Payment Break Up Total value are not equal.";
            }
            if (drAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var ah = model.PaymentExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        #endregion
        #region  FellowShip
        public ActionResult FellowShipList(int FellowId = 0, bool view = false, bool edit = false, bool revise = false, bool revisededit = false)
        {
            var emptyList = new List<FellowShipModel>();
            ViewBag.PayeeName =
            ViewBag.AvailBalance =
            ViewBag.CommitmentNumbr = emptyList;

            FellowShipModel model = new FellowShipModel();


            if (FellowId > 0 && edit == true && Common.ValidateFellowShipOnEdit(FellowId))
            {
                model = coreAccountService.GetFellowShipDetails(FellowId, view, edit, revise, revisededit);
            }
            else if (FellowId > 0)
            {
                model = coreAccountService.GetFellowShipDetails(FellowId, view, edit, revise, revisededit);
            }
            if (FellowId > 0)
            {
                //ViewBag.PayeeName = Common.GetPIname(Convert.ToInt32(model.ProjectId));
                ViewBag.CommitmentNumbr = Common.GetCommitmentNo(Convert.ToInt32(model.ProjectId));
            }
            return View(model);

        }
        [HttpPost]
        public ActionResult FellowShipList(FellowShipModel model)
        {
            var emptyList = new List<FellowShipModel>();
            ViewBag.PayeeName =
            ViewBag.AvailBalance =
            ViewBag.CommitmentNumbr = emptyList;
            if (ModelState.IsValid)
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateFellowShip(model, logged_in_user);
                if (model.FellowShipId == 0 && result > 0)
                {
                    TempData["succMsg"] = "FellowShip has been added successfully.";
                    return RedirectToAction("FellowShipList");
                }
                else if (model.FellowShipId > 0 && result > 0)
                {
                    TempData["succMsg"] = "FellowShip has been updated successfully.";
                    return RedirectToAction("FellowShipList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;
            }
            return View(model);
        }
        [HttpGet]
        public JsonResult LoadProjectNumber(string term, int? type = null)
        {
            try
            {
                var data = Common.GetProjectNumber(term, type);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPIname(string term)
        {
            var locationdata = Common.GetAutoCompletePIWithDetails(term);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetCommitmentNo(int projid)
        {
            var locationdata = Common.GetCommitmentNo(projid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAvailableBalance(string commitmentno)
        {
            var locationdata = Common.GetAvailableBalance(commitmentno);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetFellowShipList()
        {
            try
            {
                object output = coreAccountService.GetFellowShipList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalForFellowShip(int FellowId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalForFellowShip(FellowId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForFellowShip(int FellowId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalPendingForFellowShip(FellowId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchFellowShipList(FellowShipSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchFellowShipList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Import Payment 
        #region Foreign Remittance 
        public ActionResult ForeignRemittanceList()
        {
            return View();
        }
        public ActionResult ForeignRemittance(int foreignRemitId = 0)
        {
            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.ExpensesHead = Common.GetCodeControlList("ForgnRemitExpensesHead");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getcurrency();
                // ViewBag.PaymentBank = Common.GetCodeControlList("DistributionType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ForeignRemittanceModel model = new ForeignRemittanceModel();
                if (foreignRemitId > 0 && Common.ValidateForeignRemitOnEdit(foreignRemitId))
                {
                    model = coreAccountService.GetForeignRemitDetails(foreignRemitId);
                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(76);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("CoreAccounts", "DistributionList");
            }

        }

        public ActionResult ForeignRemittanceView(int foreignRemitId)
        {

            try
            {
                var emptyList = new List<DistributionModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.ExpensesHead = Common.GetCodeControlList("ForgnRemitExpensesHead");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getcurrency();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);

                ForeignRemittanceModel model = new ForeignRemittanceModel();

                model = coreAccountService.GetForeignRemitDetails(foreignRemitId);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);

            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult ForeignRemittance(ForeignRemittanceModel model)
        {
            try
            {
                var emptyList = new List<ForeignRemittanceModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getcurrency();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(76);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                //model.TravellerList = Common.GetTravellerList(model.CategoryId);
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateForeignRemittance(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    foreach (var item in model.DocumentDetail)
                    {
                        if (item.DocumentFile != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                            string filename = Path.GetFileName(item.DocumentFile.FileName);
                            var docextension = Path.GetExtension(filename);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                                return View(model);
                            }
                        }
                    }

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.CreateForeignRemittance(model, logged_in_user);
                    if (model.ForeignRemittanceId == 0 && result > 0)
                    {
                        TempData["succMsg"] = "Foreign Remittance payment has been added successfully.";
                        return RedirectToAction("ForeignRemittanceList");
                    }
                    else if (model.ForeignRemittanceId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Foreign Remittance payment has been updated successfully.";
                        return RedirectToAction("ForeignRemittanceList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Supplier = Common.getForeignSupplierList();
                ViewBag.TypeofPayment = Common.GetCodeControlList("ImportPaymentType");
                ViewBag.PaymentBank = Common.GetCodeControlList("ImportPaymentBank");
                ViewBag.PortfolioName = Common.GetCodeControlList("PortfolioName");
                ViewBag.ForgnBankChargesType = Common.GetCodeControlList("ForeignBankChargesType");
                ViewBag.PurposeofRemit = Common.GetCodeControlList("PurposeofRemittance");
                ViewBag.PaymentMode = Common.GetCodeControlList("ForgnRemitPaymentMode");
                ViewBag.Currencyequalantstatus = Common.GetCodeControlList("Forncurrequalantstatus");
                ViewBag.Currency = Common.getcurrency();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }

        }
        private string ValidateForeignRemittance(ForeignRemittanceModel model)
        {
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.CommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlExpDrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTaxesAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = ttlExpDrAmt + ttlTaxesAmt;
            decimal paymentamt = model.ForeignRemittanceAmount ?? 0;

            // decimal netDrAmt = ttlExpAmt + ttldeductAmt;
            if (netCrAmt != ttlExpAmt || netCrAmt != ttldeductAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            //if (paymentamt != commitmentAmt)
            //    msg = msg == "Valid" ? "There is a mismatch between allocated commitment value and payment value. Please update the value to continue." : msg + "<br /> There is a mismatch between allocated commitment value and payment value. Please update the value to continue.";
            var gDoc = model.DocumentDetail.GroupBy(v => v.DocumentType);
            if (model.DocumentDetail.Count() != gDoc.Count())
                msg = "Selected attachment type already exist. Please select a different attachment type.";
            //var ahJV = model.ExpenseDetail.Where(m => m.IsJV).ToList();
            //var gAHJV = ahJV.GroupBy(v => v.AccountHeadId);
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        [HttpGet]
        public JsonResult GetForeignRemittanceList()
        {
            try
            {
                object output = coreAccountService.GetForeignRemittanceList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult SearchForeignRemittanceList(ForeignRemitSearchFieldModel model)
        {
            object output = coreAccountService.SearchForeignRemittanceList(model);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ForeignRemittanceApprove(int foreignRemitId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, false, false, userId, "FRM");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.ForeignRemittanceBillApproved(foreignRemitId, userId);
                if (!status)
                    coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, true, false, userId, "FRM");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #endregion
        #region ManDay
        public ActionResult ManDay(int Mandayid = 0)
        {
            try
            {
                var emptyList = new List<ManDayModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymtMode = Common.GetCodeControlList("PaymentModeManDay");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ManDayModel model = new ManDayModel();
                model.CreditorType = "PI/Student/Others";
                if (Mandayid > 0 && Common.ValidateManDayOnEdit(Mandayid))
                {
                    model = coreAccountService.GetManDayDetails(Mandayid);
                    
                }
                else
                {
                    model.CheckListDetail = Common.GetCheckedList(77);
                    model.NeedUpdateTransDetail = true;
                }

                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }

        }
        [HttpPost]
        public ActionResult ManDay(ManDayModel model)
        {
            try
            {
                var emptyList = new List<ManDayModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.TDS = Common.GetTDS();
                ViewBag.OH = Common.GetOH();
                ViewBag.ReceviedFrom = Common.GetReceivedFrom();
                ViewBag.CategoryList = Common.GetCodeControlList("AdhocCategory");
                ViewBag.PaymentMode = Common.GetCodeControlList("ModeOfPayment");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }


                foreach (var item in model.DocumentDetail)
                {
                    if (item.DocumentFile != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string filename = Path.GetFileName(item.DocumentFile.FileName);
                        var docextension = Path.GetExtension(filename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                }
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateManDay(model, logged_in_user);
                if (model.ManDayId == 0 && result > 0)
                {
                    TempData["succMsg"] = "Mandays has been added successfully.";
                    return RedirectToAction("ManDayList");
                }
                else if (model.ManDayId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Mandays has been updated successfully.";
                    return RedirectToAction("ManDayList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }

                return View(model);

            }
            catch (Exception ex)
            {

                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        public ActionResult ManDayView(int Mandayid)
        {
            try
            {
                var emptyList = new List<ManDayModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymtMode = Common.GetCodeControlList("PaymentModeManDay");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                ManDayModel model = new ManDayModel();
                model.CreditorType = "PI/Student/Others";
                model = coreAccountService.GetManDayDetails(Mandayid);
                
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();

            }

        }
        [HttpGet]
        public JsonResult GetStaffname(string term)
        {
            var locationdata = Common.GetStaffname(term);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult ValidateManDay(int projid, int days, DateTime monyr, int mandayid = 0)
        {
            try
            {
                object output = Common.ValidateManDay(projid, days, monyr, mandayid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalForManDay(int Mandayid)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cStatus = coreAccountService.ManDayCommitmentBalanceUpdate(Mandayid, false, false, logged_in_user, "MDY");
                if (!cStatus)
                    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalForManDay(Mandayid, logged_in_user);
                if (!status)
                    coreAccountService.ManDayCommitmentBalanceUpdate(Mandayid, true, false, logged_in_user, "MDY");
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApprovalPendingForManDay(int Mandayid)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool cstatus = coreAccountService.ManDayBillApproved(Mandayid, logged_in_user);
                if (!cstatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                // bool boaStatus = coreAccountService.ManDayBOATransaction(Mandayid);
                // var status = Common.ApprovalPendingForManDay(Mandayid, logged_in_user);
                return Json(new { status = cstatus, msg = !cstatus ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ManDayList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetManDayList()
        {
            try
            {
                object output = coreAccountService.GetManDayList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [HttpGet]
        public JsonResult SearchManDayList(ManDaySearchFieldModel model)
        {
            object output = CoreAccountsService.SearchManDayList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region TDS Payment
        public ActionResult TDSPayment(int TDSPaymentId = 0, bool payment = false)
        {
            try
            {
                ViewBag.CategoryList = Common.GetCodeControlList("TDSPaymentCategory");
                ViewBag.SectionList = Common.Section();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                TDSPaymentModel model = new TDSPaymentModel();
                if (TDSPaymentId > 0 && Common.ValidateTDSPaymentOnEdit(TDSPaymentId))
                {
                    model = coreAccountService.GetTDSPaymentDetails(TDSPaymentId, payment);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult TDSPayment(TDSPaymentModel model)
        {
            var emptyList = new List<TDSPaymentModel>();
            ViewBag.CategoryList = Common.GetCodeControlList("TDSPaymentCategory");
            ViewBag.SectionList = Common.Section();
            ViewBag.BankList = Common.GetBankAccountHeadList();
            if (ModelState.IsValid)
            {
                int logged_in_user = Common.GetUserid(User.Identity.Name);
                int result = coreAccountService.CreateTDSPayment(model, logged_in_user);
                if (model.TDSPaymentId == 0 && result > 0)
                {
                    TempData["succMsg"] = "TDSPayment has been added successfully.";
                    return RedirectToAction("TDSPaymentList");
                }
                else if (model.TDSPaymentId > 0 && result > 0)
                {
                    TempData["succMsg"] = "TDSPayment has been updated successfully.";
                    return RedirectToAction("TDSPaymentList");
                }
                else
                {
                    TempData["errMsg"] = "Something went wrong please contact administrator.";
                }
            }
            else
            {
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));

                TempData["errMsg"] = messages;
            }
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSection()
        {
            var locationdata = Common.GetSection();
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTDSIncomeTaxList(DateTime fromdate, DateTime todate, int headid)
        {
            try
            {
                object output = CoreAccountsService.GetTDSIncomeTaxList(fromdate, todate, headid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult TDSPaymentList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetTDSGSTList(DateTime fromdate, DateTime todate)
        {
            try
            {
                object output = CoreAccountsService.GetTDSGSTList(fromdate, todate);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult GetTDSPaymentList()
        {
            try
            {
                object output = coreAccountService.GetTDSPaymentList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalForTDSPayment(int TDSPaymentId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var value = Common.ApprovalForTDSPayment(TDSPaymentId, logged_in_user);
                return Json(value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForTDSPayment(int TDSPaymentId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool boaStatus = coreAccountService.TDSPaymentBOATransaction(TDSPaymentId);
                if (!boaStatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);

                var status = Common.ApprovalPendingForTDSPayment(TDSPaymentId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult TDSPaymentView(int TDSPaymentId = 0, bool payment = false)
        {
            try
            {
                ViewBag.CategoryList = Common.GetCodeControlList("TDSPaymentCategory");
                ViewBag.SectionList = Common.Section();
                ViewBag.BankList = Common.GetBankAccountHeadList();
                TDSPaymentModel model = new TDSPaymentModel();
                model = coreAccountService.GetTDSPaymentDetails(TDSPaymentId, payment);
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchTDSPaymentList(TDSPaymentSearchFieldModel model)
        {
            object output = CoreAccountsService.SearchTDSPaymentList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Overheads Posting
        public ActionResult OverheadsPostingList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetOverheadsPostingList()
        {
            try
            {
                object output = coreAccountService.GetOverheadsPostingList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ApproveOverheadsPosting(int ohpid)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);

                //bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, false, false, userId, "FRM");
                //if (!cStatus)
                //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                bool status = coreAccountService.OverheadsPostingBillApproved(ohpid, userId);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult OverheadsPosting(int projecttype = 1)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                // ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                // ViewBag.ContraAccountGroupList = Common.GetOHPostingBankAccountGroup();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                OverheadsPostingModel model = new OverheadsPostingModel();
                if (projecttype > 0)
                {
                    model = coreAccountService.GetOverheadsDetails(projecttype);
                }
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                model.ProjectType = 1;
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult OverheadsPosting(OverheadsPostingModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.AccountGroupList =
                //  ViewBag.ContraAccountHeadList =
                ViewBag.AccountHeadList =
                ViewBag.Bank =
                ViewBag.ProjectNumber = emptyList;
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                if (ModelState.IsValid)
                {

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.OverheadsPostingIU(model, logged_in_user);
                    if (model.OverheadsPostingId == null && result > 0)
                    {
                        TempData["succMsg"] = "Overheads Posting has been done successfully.";
                        return RedirectToAction("OverheadsPostingList");
                    }
                    //else if (model.OverheadsPostingId > 0 && result > 0)
                    //{
                    //    TempData["succMsg"] = "Credit note has been updated successfully.";
                    //    return RedirectToAction("CreditNoteList");
                    //}
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.SourceList = Common.GetSourceList();
                return View();
            }
        }
        public ActionResult OverheadsPostingView(int creditNoteId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.ReasonList = Common.GetCodeControlList("Credit Note Reason");
                ViewBag.disabled = "disabled";
                CreditNoteModel model = new CreditNoteModel();
                model = coreAccountService.GetInvoiceDetailsForCreditNote(creditNoteId, true);

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }
        public ActionResult _OverheadsPostingPIRMFShare(int Projectid, decimal RMFValue)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            OverheadsPostingModel model = new OverheadsPostingModel();
            model = _cs.GetPIRMFShareDetails(Projectid, RMFValue);
            return PartialView(model);
        }
        public ActionResult _OverheadsPostingPIPCFShare(int Projectid, decimal PCFValue)
        {
            CoreAccountsService _cs = new CoreAccountsService();
            OverheadsPostingModel model = new OverheadsPostingModel();
            model = _cs.GetPIPCFShareDetails(Projectid, PCFValue);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _SavePIPCFShare(OverheadsPostingModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            CoreAccountsService _CS = new CoreAccountsService();
            object output = _CS.SavePIPCFShare(model, UserId, true);
            return RedirectToAction("OverheadsPosting");
        }
        [HttpPost]
        public ActionResult _SavePIRMFShare(OverheadsPostingModel model)
        {
            var UserId = Common.GetUserid(User.Identity.Name);
            CoreAccountsService _CS = new CoreAccountsService();
            object output = _CS.SavePIRMFShare(model, UserId, true);
            return RedirectToAction("OverheadsPosting");
        }
        //[HttpGet]
        //public JsonResult GetBackEndCreditDetails(int receiptid, bool interstate_f = false, bool eligibilityCheck_f = false, int deductionCategoryId = 0, string tSubCode = "1", List<int?> TDSDetailId = null)
        //{
        //    try
        //    {
        //        object output = coreAccountService.GetTransactionDetails(deductionCategoryId, interstate_f, typeCode, tSubCode, eligibilityCheck_f, TDSDetailId);
        //        return Json(output, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        #endregion
        #region PCF and Distribution Overheads Posting
        public ActionResult PCFDistributionOverheadsPostingList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetPCFDistributionOHPostingList(int pageIndex, int pageSize, SearchPCFDistributionOH model)
        {
            try
            {
                object output = coreAccountService.GetDistributionOHPList(pageIndex, pageSize, model);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[HttpGet]
        //public ActionResult ApproveDistributionOHPosting(int ohpid)
        //{
        //    try
        //    {
        //        int userId = Common.GetUserid(User.Identity.Name);

        //        //bool cStatus = coreAccountService.ForeignRemittanceCommitmentBalanceUpdate(foreignRemitId, false, false, userId, "FRM");
        //        //if (!cStatus)
        //        //    return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
        //        bool status = coreAccountService.OverheadsPostingBillApproved(ohpid, userId);
        //        return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult PCFDistributionOverheadsPosting(int paymenttype = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.ContraAccountHeadList =
                ViewBag.Bank =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumber = emptyList;
                ViewBag.PaymentNumber = emptyList;
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.PaymentType = Common.GetCodeControlList("PCFDistributionPaymentType");
                DistributionOHPostingModel model = new DistributionOHPostingModel();
                //if (paymenttype > 0)
                //{
                //    //model = coreAccountService.GetOverheadsDetails(paymenttype);
                //    if (model.Source == 1)
                //        ViewBag.SourceRefNumberList = Common.GetWorkflowRefNumberList();
                //    else if (model.Source == 3)
                //    {
                //        int depId = Common.GetDepartmentId(User.Identity.Name);
                //        ViewBag.SourceRefNumberList = Common.GetTapalRefNumberList(depId);
                //    }
                //}
                model.NeedUpdateTransDetail = true;
                model.CreditorType = "PI";
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }

        [HttpPost]
        public ActionResult PCFDistributionOverheadsPosting(DistributionOHPostingModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.ContraAccountHeadList =
                ViewBag.AccountHeadList =
                ViewBag.Bank =
                ViewBag.ProjectNumber = emptyList;
                ViewBag.PaymentNumber = emptyList;
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.PaymentType = Common.GetCodeControlList("PCFDistributionPaymentType");
                if (ModelState.IsValid)
                {

                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    int result = coreAccountService.DistributionOHPostingIU(model, logged_in_user);
                    if (model.OverheadsPostingId == null && result > 0)
                    {
                        TempData["succMsg"] = "Overheads Posting has been done successfully.";
                        return RedirectToAction("PCFDistributionOverheadsPostingList");
                    }
                    //else if (model.OverheadsPostingId > 0 && result > 0)
                    //{
                    //    TempData["succMsg"] = "Credit note has been updated successfully.";
                    //    return RedirectToAction("CreditNoteList");
                    //}
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;

                return View();
            }
        }
        public ActionResult PCFDistributionOHPostingView(int OverheadsPostingId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.ContraAccountHeadList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.Bank = Common.GetAllBankList();
                ViewBag.ProjectNumber = Common.GetProjectNumbers();
                ViewBag.Projecttype = Common.GetCodeControlList("Projecttype");
                ViewBag.PaymentType = Common.GetCodeControlList("PCFDistributionPaymentType");
                ViewBag.disabled = "disabled";
                DistributionOHPostingModel model = new DistributionOHPostingModel();
                model = coreAccountService.GetPCFDOHViewDetails(OverheadsPostingId);
                int paymenttype = model.PaymentTypeId ?? 0;
                ViewBag.PaymentNumber = Common.GetPaymentNumberListbyType(paymenttype);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("PCFDistributionOverheadsPostingList", "CoreAccounts");
            }
        }
        [HttpGet]
        public JsonResult GetPCFDOHPaymentDetails(int paytype, int paynumberid)
        {
            try
            {
                object output = coreAccountService.GetPCFDOHPaymentDetails(paytype, paynumberid);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult GetPaymentNumberList(int paymenttype)
        {
            object output = coreAccountService.GetPaymentNumberList(paymenttype);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Institute Claims
        public ActionResult InstituteClaimsView(int InsClaimId = 0, bool receipt = false)
        {
            try
            {
                var emptyList = new List<InstituteClaims>();
                ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
                ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
                ViewBag.bankachead = Common.getbankcreditaccounthead();
                ViewBag.BudHed = Common.getallocationhead();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                   ViewBag.CommitmentNumbr =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                InstituteClaims model = new InstituteClaims();
                model.CreditorType = "NA";
                if (InsClaimId > 0)
                {
                    model = coreAccountService.GetInstitueClaimsDetails(InsClaimId, receipt);
                }
                if (InsClaimId > 0)
                {

                    ViewBag.CommitmentNumbr = Common.GetComitmentNo(Convert.ToInt32(model.Projectid));
                }
                ViewBag.disabled = "Disabled";
                ViewBag.processGuideLineId = 1;
                TempData["viewMode"] = "ViewOnly";
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }
        public ActionResult InstituteClaims(int InsClaimId = 0, bool receipt = false)
        {
            try
            {
                var emptyList = new List<InstituteClaims>();
                ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
                ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
                ViewBag.bankachead = Common.getbankcreditaccounthead();
                ViewBag.BudHed = Common.getallocationhead();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.CommitmentNumbr =
                    ViewBag.AccountHeadList = emptyList;

                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                InstituteClaims model = new InstituteClaims();
                model.CreditorType = "NA";
                if (InsClaimId > 0)
                {
                    model = coreAccountService.GetInstitueClaimsDetails(InsClaimId, receipt);
                }
                if (InsClaimId > 0)
                {

                    ViewBag.CommitmentNumbr = Common.GetComitmentNo(Convert.ToInt32(model.Projectid));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Dashboard");
            }
        }
        [HttpPost]
        public ActionResult InstituteClaims(InstituteClaims model)
        {
            try
            {
                var emptyList = new List<InstituteClaims>();
                ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
                ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
                ViewBag.BudHed = Common.getallocationhead();
                ViewBag.bankachead = Common.getbankcreditaccounthead();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList = emptyList;
                ViewBag.PIName = Common.GetPIWithDetails();
                ViewBag.Project = Common.GetProjectNumberList();
                ViewBag.Department = Common.getDepartment();
                ViewBag.Student = Common.GetStudentList();
                ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
                ViewBag.SourceRefNumberList =
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                 ViewBag.CommitmentNumbr =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
                var ptypeList = Common.getprojecttype();
                if (model.ExpenseDetail != null)
                {
                    foreach (var item in model.ExpenseDetail)
                    {
                        int headId = item.AccountGroupId ?? 0;
                        item.AccountGroupList = Common.GetAccountGroup(headId);
                        item.AccountHeadList = Common.GetAccountHeadList(headId);
                    }
                }
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                model.CreditorType = "NA";

                int logged_in_user = Common.GetUserid(User.Identity.Name);
                if (model.ReceiptId == 1)
                {
                    bool res = coreAccountService.CreateReceiptInstitueClaims(model, logged_in_user);
                    if (model.InstituteClaimId > 0 && res == true)
                    {
                        TempData["succMsg"] = "Receipt has been added successfully.";
                        return RedirectToAction("InstituteClaimsList");
                    }
                    else
                    {
                        TempData["succMsg"] = "Something went wrong please contact administrator.";
                        return RedirectToAction("InstituteClaimsList");
                    }
                }
                int result = coreAccountService.CreateInstitueClaims(model, logged_in_user);
                if (model.InstituteClaimId == 0 && result > 0)
                {
                    TempData["succMsg"] = "Institute Claims has been added successfully.";
                    return RedirectToAction("InstituteClaimsList");
                }
                else if (model.InstituteClaimId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Institute Claims has been updated successfully.";
                    return RedirectToAction("InstituteClaimsList");
                }


                return View(model);
            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.SourceList = Common.GetSourceList();
                ViewBag.SourceRefNumberList =
                ViewBag.TravellerList = emptyList;
                ViewBag.projecttype = Common.getprojecttype();
                ViewBag.CountryList = Common.getCountryList();
                ViewBag.ProjectNumberList = Common.GetProjectNumberList();
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }
        //public ActionResult InstituteClaimsReceipt(int InsClaimId = 0)

        //{
        //    var emptyList = new List<InstituteClaims>();
        //    ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
        //    ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
        //    ViewBag.BudHed = Common.getallocationhead();
        //    ViewBag.SourceList = Common.GetSourceList();
        //    ViewBag.SourceRefNumberList = emptyList;
        //    ViewBag.PIName = Common.GetPIWithDetails();
        //    ViewBag.Project = Common.GetProjectNumberList();
        //    ViewBag.Department = Common.getDepartment();
        //    ViewBag.Student = Common.GetStudentList();
        //    ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
        //    ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
        //    ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
        //    ViewBag.SourceRefNumberList =
        //    ViewBag.AccountGroupList =
        //    ViewBag.TypeOfServiceList =
        //    ViewBag.AccountHeadList = emptyList;
        //    ViewBag.ProjectNumberList = Common.GetProjectNumberList();
        //    ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
        //    var ptypeList = Common.getprojecttype();
        //    int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
        //    ViewBag.ProjectTypeList = ptypeList;
        //    ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
        //    InstituteClaims model = new InstituteClaims();
        //    model.CreditorType = "PI/Student/Others";
        //    model.InstituteClaimId = InsClaimId;
        //    model.ReceiptValue = Common.GetClaimValue(InsClaimId);
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult InstituteClaimsReceipt(InstituteClaims model)
        //{
        //    try
        //    {
        //        var emptyList = new List<InstituteClaims>();
        //        ViewBag.ClaimTy = Common.GetCodeControlList("ClaimType");
        //        ViewBag.FacUsed = Common.GetCodeControlList("Facilities Used");
        //        ViewBag.BudHed = Common.getallocationhead();
        //        ViewBag.SourceList = Common.GetSourceList();
        //        ViewBag.SourceRefNumberList = emptyList;
        //        ViewBag.PIName = Common.GetPIWithDetails();
        //        ViewBag.Project = Common.GetProjectNumberList();
        //        ViewBag.Department = Common.getDepartment();
        //        ViewBag.Student = Common.GetStudentList();
        //        ViewBag.PaymentMode = Common.GetCodeControlList("PaymentModeInstituteClaims");
        //        ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
        //        ViewBag.ExpType = Common.GetCodeControlList("Expense Type");
        //        ViewBag.SourceRefNumberList =
        //        ViewBag.AccountGroupList =
        //        ViewBag.TypeOfServiceList =
        //        ViewBag.AccountHeadList = emptyList;
        //        ViewBag.ProjectNumberList = Common.GetProjectNumberList();
        //        ViewBag.DocmentTypeList = Common.GetDocTypeList(77);
        //        var ptypeList = Common.getprojecttype();
        //        if (model.ExpenseDetail != null)
        //        {
        //            foreach (var item in model.ExpenseDetail)
        //            {
        //                int headId = item.AccountGroupId ?? 0;
        //                item.AccountGroupList = Common.GetAccountGroup(headId);
        //                item.AccountHeadList = Common.GetAccountHeadList(headId);
        //            }
        //        }
        //        int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
        //        ViewBag.ProjectTypeList = ptypeList;
        //        ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
        //        model.CreditorType = "NA";
        //        int logged_in_user = Common.GetUserid(User.Identity.Name);
        //        int result = coreAccountService.CreateReceiptInstitueClaims(model, logged_in_user);
        //        if (model.InstituteClaimId > 0 && result > 0)
        //        {
        //            TempData["succMsg"] = "Receipt has been added successfully.";
        //            return RedirectToAction("InstituteClaimsList");
        //        }

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        var emptyList = new List<MasterlistviewModel>();
        //        ViewBag.SourceList = Common.GetSourceList();
        //        ViewBag.SourceRefNumberList =
        //        ViewBag.TravellerList = emptyList;
        //        ViewBag.projecttype = Common.getprojecttype();
        //        ViewBag.CountryList = Common.getCountryList();
        //        ViewBag.ProjectNumberList = Common.GetProjectNumberList();
        //        TempData["errMsg"] = "Something went wrong please contact administrator.";
        //        return View(model);
        //    }
        //}
        public ActionResult InstituteClaimsList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetInstituteClaimsList()
        {
            try
            {
                object output = coreAccountService.GetInstitueClaimsList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalForInstituteClaims(int InsClaimId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                var status = Common.ApprovalForInstituteClaims(InsClaimId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ApprovalPendingForInstituteClaims(int InsClaimId)
        {
            int logged_in_user = Common.GetUserid(User.Identity.Name);
            try
            {
                bool boaStatus = coreAccountService.InstitueClaimsBOATransaction(InsClaimId);
                if (!boaStatus)
                    return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
                var status = Common.ApprovalPendingForInstituteClaims(InsClaimId, logged_in_user);
                return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSpendBalance(int commitmentid)
        {
            var locationdata = Common.GetSpendBalance(commitmentid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetComitmentNo(int projid)
        {
            var locationdata = Common.GetComitmentNo(projid);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
        #region Receipt
        public ActionResult CreateReceipt(int ID = 0)
        {
            try
            {
                var pId = Common.getProjectID(ID);
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var invoicenumber = Common.getinvocenumber(pId);
                var budgethead = Common.getbudgethead();
                var receivedfrom = Common.getagency();
                var creditbankachead = Common.getbankcreditaccounthead();
                var receivableshead = Common.getreceivableshead();
                var modeofreceipt = Common.getmodeofreceipt();
                var foreigntransfercountry = Common.getCountryList();
                var foreigntransfercurrency = Common.getcurrency();
                var banktransactiontype = Common.getbanktransactiontype();
                ViewBag.projecttype = projecttype;
                ViewBag.invoice = invoicenumber;
                ViewBag.budgethead = budgethead;
                ViewBag.receivedfrom = receivedfrom;
                ViewBag.bankcredithead = creditbankachead;
                ViewBag.receivableshead = receivableshead;
                ViewBag.receiptmode = modeofreceipt;
                ViewBag.country = foreigntransfercountry;
                ViewBag.currency = foreigntransfercurrency;
                ViewBag.banktrnsctntyp = banktransactiontype;

                CreateReceiptModel model = new CreateReceiptModel();
                model = coreAccountService.GetReceiptDetails(pId, ID);
                if (model.ReceiptAmount > 0)
                {
                    return View(model);
                }
                else if (model.ReceiptAmount == 0)
                {
                    ViewBag.errMsg = "Receipt has already been created for Invoice Value.";

                    //if (model.ProjectType == 1)
                    //    {
                    //        return RedirectToAction("SponReceiptList", "Project");
                    //    }
                    //if (model.ProjectType == 2)
                    //    {
                    //        return RedirectToAction("ConsReceiptList", "Project");
                    //    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateReceipt(CreateReceiptModel model, FormCollection form, HttpPostedFileBase tdsfile)
        {
            try
            {
                var value = form["Buttonvalue"];
                var roleId = Common.GetRoleId(User.Identity.Name);
                var ProjectId = Convert.ToInt32(model.PIId);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                //var invoicetype = Common.getinvoicetype();
                //var invoicenumber = Common.getinvocenumber(pId);
                var budgethead = Common.getbudgethead();
                var receivedfrom = Common.getagency();
                var creditbankachead = Common.getbankcreditaccounthead();
                var receivableshead = Common.getreceivableshead();
                var modeofreceipt = Common.getmodeofreceipt();
                var foreigntransfercountry = Common.getCountryList();
                var foreigntransfercurrency = Common.getcurrency();
                var banktransactiontype = Common.getbanktransactiontype();
                ViewBag.projecttype = projecttype;
                //    ViewBag.invoice = invoicenumber;
                ViewBag.budgethead = budgethead;
                ViewBag.receivedfrom = receivedfrom;
                ViewBag.bankcredithead = creditbankachead;
                ViewBag.receivableshead = receivableshead;
                ViewBag.receiptmode = modeofreceipt;
                ViewBag.country = foreigntransfercountry;
                ViewBag.currency = foreigntransfercurrency;
                ViewBag.banktrnsctntyp = banktransactiontype;
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");
                if (tdsfile != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    string tdsfilename = Path.GetFileName(tdsfile.FileName);
                    var docextension = Path.GetExtension(tdsfilename);
                    if (!allowedExtensions.Contains(docextension))
                    {
                        ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
                        return RedirectToAction("ProjectOpening", "Project");
                    }
                }
                CoreAccountsService _ps = new CoreAccountsService();
                var ReceiptID = _ps.CreateReceipt(model, loggedinuserid, tdsfile);
                if (ReceiptID > 0)
                {
                    var ReceiptNumber = Common.getreceiptnumber(ReceiptID);
                    ViewBag.succMsg = "Receipt has been created successfully with Receipt Number - ." + ReceiptNumber;
                }
                else if (ReceiptID == -5)
                {
                    if (value == "Approve")
                    {
                        var ReceiptId = Convert.ToInt32(model.ReceiptID);
                        BOAModel BOAmodel = new BOAModel();
                        BOAmodel = coreAccountService.getBOAmodeldetails(model);
                        var BOA = _ps.BOATransaction(BOAmodel);
                        if (BOA == true)
                        {
                            using (var context = new DataModel.IOASDBEntities())
                            {
                                var Vquery = context.tblReceipt.SingleOrDefault(m => m.ReceiptId == model.ReceiptID);
                                // Vquery.BOAId = boaId;
                                Vquery.Status = "Completed";
                                context.SaveChanges();
                                var negativebalancequery = (from v in context.tblNegativeBalance
                                                            where v.ProjectId == Vquery.ProjectId && v.Status == "Approved"
                                                            select v).ToList();

                                if (negativebalancequery.Count() > 0)
                                {
                                    decimal? claimamt = 0;
                                    decimal? balancewhenclosing = 0;
                                    decimal? negativebalamt = 0;
                                    decimal? adjustamount = 0;
                                    decimal? overheadsamt = Vquery.ReceiptOverheadValue;
                                    decimal? gstamt = 0;
                                    decimal? newadjustamt = 0;

                                    if (Vquery.IGST > 0 && Vquery.CGST == 0)
                                    {
                                        gstamt = Vquery.IGST;
                                    }
                                    else if (Vquery.CGST > 0 && Vquery.IGST == 0)
                                    {
                                        gstamt = Vquery.CGST + Vquery.SGST;
                                    }
                                    decimal? receiptamt = Vquery.ReceiptAmount - (overheadsamt + gstamt);
                                    decimal? balinreceipt = receiptamt;

                                    for (int i = 0; i < negativebalancequery.Count; i++)
                                    {
                                        claimamt = negativebalancequery[i].ClaimAmount;
                                        balancewhenclosing = negativebalancequery[i].BalanceWhenClosing ?? 0;
                                        negativebalamt = negativebalancequery[i].NegativeBalanceAmount ?? 0;
                                        adjustamount = negativebalancequery[i].ReceiptAdjustmentAmount ?? 0;
                                        decimal? newnegativebalamt = 0;
                                        if (balinreceipt > 0)
                                        {
                                            if (balinreceipt >= negativebalamt)
                                            {
                                                newadjustamt = adjustamount + negativebalamt;
                                                negativebalancequery[i].ReceiptAdjustmentAmount = newadjustamt;
                                                negativebalancequery[i].NegativeBalanceAmount = 0;
                                                negativebalancequery[i].Status = "Closed";
                                                negativebalancequery[i].BalanceWhenClosing = 0;
                                                negativebalancequery[i].ClosedDate = DateTime.Now;
                                                negativebalancequery[i].UPTD_By = loggedinuserid;
                                                negativebalancequery[i].UPTD_TS = DateTime.Now;
                                                negativebalancequery[i].ReasonForClose = "Receipt Created for full negative balance amount.";
                                                balinreceipt = balinreceipt - negativebalamt;
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                newadjustamt = adjustamount + balinreceipt;
                                                newnegativebalamt = claimamt - (newadjustamt + balancewhenclosing);
                                                negativebalancequery[i].ReceiptAdjustmentAmount = newadjustamt;
                                                negativebalancequery[i].NegativeBalanceAmount = newnegativebalamt;
                                                negativebalancequery[i].UPTD_By = loggedinuserid;
                                                negativebalancequery[i].UPTD_TS = DateTime.Now;
                                                balinreceipt = 0;
                                                context.SaveChanges();
                                            }
                                        }
                                        else if (balinreceipt == 0)
                                        {
                                            newadjustamt = 0;
                                            context.SaveChanges();
                                        }

                                    }

                                }
                            }
                        }
                        var ReceiptNumber = Common.getreceiptnumber(ReceiptId);
                        ViewBag.succMsg = "Receipt with Receipt number -" + ReceiptNumber + " has been Approved.";
                    }
                    else if (value != "Approve")
                    {
                        var ReceiptId = Convert.ToInt32(model.ReceiptID);
                        var ReceiptNumber = Common.getreceiptnumber(ReceiptId);
                        ViewBag.succMsg = "Receipt with Receipt number -" + ReceiptNumber + " has been updated successfully.";
                    }

                }
                else if (ReceiptID == -6)
                {
                    ViewBag.errMsg = "The Bank Debit Value and Net Transaction value are not equal. Please check.";
                }
                else if (ReceiptID == -6)
                {
                    ViewBag.errMsg = "The Receipt Value has exceeded the Invoice value. Please check.";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchInvoiceList(ReceiptListModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                //var receiptdata = new PagedData<ReceiptSearchResultModel>();
                //ReceiptSearchFieldModel model = new ReceiptSearchFieldModel();
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                srchModel.Userrole = user_role;
                if (srchModel.InvoiceToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.InvoiceToDate;
                    srchModel.InvoiceToDate = todate.Date.AddDays(1).AddTicks(-1);
                }
                var data = coreAccountService.GetSearchInvoiceList(srchModel, page, pageSize);
                srchModel = data;
                return PartialView(srchModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        public ActionResult EditProjectReceipt(int ReceiptId = 0)
        {
            try
            {
                var pId = Common.getProjectIdbyReceiptId(ReceiptId);
                //var pId = Common.getProjectID(InvoiceId);
                if (pId == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var invoicenumber = Common.getinvocenumber(pId);
                var budgethead = Common.getbudgethead();
                var receivedfrom = Common.getagency();

                var creditbankachead = Common.getbankcreditaccounthead();
                var receivableshead = Common.getreceivableshead();
                var modeofreceipt = Common.getmodeofreceipt();
                var foreigntransfercountry = Common.getCountryList();
                var foreigntransfercurrency = Common.getcurrency();
                var banktransactiontype = Common.getbanktransactiontype();
                ViewBag.projecttype = projecttype;
                ViewBag.invoice = invoicenumber;
                ViewBag.budgethead = budgethead;
                ViewBag.receivedfrom = receivedfrom;
                ViewBag.bankcredithead = creditbankachead;
                ViewBag.receivableshead = receivableshead;
                ViewBag.receiptmode = modeofreceipt;
                ViewBag.country = foreigntransfercountry;
                ViewBag.currency = foreigntransfercurrency;
                ViewBag.banktrnsctntyp = banktransactiontype;


                CreateReceiptModel model = new CreateReceiptModel();
                model = coreAccountService.GetReceiptDetailsbyId(pId, ReceiptId);

                return View("CreateReceipt", model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
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
        public ActionResult InvoiceProcess(int InvoiceId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.projecttype = projecttype;
                InvoiceModel model = new InvoiceModel();

                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult InvoiceProcess(InvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.projecttype = projecttype;
                var InvoiceID = coreAccountService.InvoiceProcess(model, loggedinuserid);
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                    ViewBag.succMsg = "Invoice - " + InvoiceNumber + " has been approved successfully.";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult ProjectInvoice(InvoiceModel model)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                var loggedinuser = User.Identity.Name;
                var loggedinuserid = Common.GetUserid(loggedinuser);
                var servicetype = Common.getservicetype();
                var invoicetype = Common.getinvoicetype();
                var projecttype = Common.getprojecttype();
                ViewBag.typeofservice = servicetype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.projecttype = projecttype;
                if (model.TaxableValue <= 0)
                {
                    ViewBag.errMsg = "Invoice Cannot be generated. No balance available for raising Invoice";
                    return View("InvoiceProcess", model);
                }
                //if (roleId != 1 && roleId != 2)
                //    return RedirectToAction("Index", "Home");              
                CoreAccountsService _ps = new CoreAccountsService();
                var InvoiceID = _ps.CreateInvoice(model, loggedinuserid);
                if (InvoiceID == -4)
                {
                    ViewBag.errMsg = "Invoice Cannot be generated as the Taxable value has exceeded the balance available for raising Invoice. Please enter correct value and try again.";
                    return View("InvoiceProcess", model);
                }
                if (InvoiceID > 0)
                {
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceID);
                    ViewBag.succMsg = "Invoice has been created successfully with Invoice Number - " + InvoiceNumber + ".";
                }
                else if (InvoiceID == -2)
                {
                    var InvoiceId = Convert.ToInt32(model.InvoiceId);
                    var InvoiceNumber = Common.getinvoicenumber(InvoiceId);
                    ViewBag.succMsg = "Invoice with Invoice number - " + InvoiceNumber + " has been updated successfully.";
                }
                else
                {
                    ViewBag.errMsg = "Something went wrong please contact administrator";
                }
                return View("InvoiceProcess", model); ;
            }
            catch (Exception ex)
            {
                return View("InvoiceProcess", model);
            }
        }

        [HttpGet]
        public JsonResult SearchInvoiceList(InvoiceSearchFieldModel model)
        {
            try
            {
                object output = coreAccountService.SearchInvoiceListForApproval(model);
                //object output = "";
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult SponReceiptList()
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;
                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetInvoicedetails();
                ViewBag.Status = Common.getreceiptstatus();
                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                ReceiptSearchFieldModel srchModel = new ReceiptSearchFieldModel();
                InvoiceSearchFieldModel srchinvoiceModel = new InvoiceSearchFieldModel();
                var pjcttype = 1;
                srchModel.ProjectType = pjcttype;
                srchinvoiceModel.ProjectType = pjcttype;
                var frmdate = DateTime.Today.AddDays(-15);
                var todate = DateTime.Now;
                model.InvoiceFromDate = frmdate;
                model.InvoiceToDate = todate;
                model.ProjectType = pjcttype;
                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);
                model = coreAccountService.GetInvoiceList(model);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SponSearchReceiptList(ReceiptSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-1);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-1);
                //}

                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        public ActionResult ConsReceiptList()
        {
            try
            {
                var loggedinuser = User.Identity.Name;
                var user = Common.getUserIdAndRole(loggedinuser);
                int logged_in_userid = user.Item1;
                int user_role = user.Item2;

                int page = 1;
                int pageSize = 5;
                ViewBag.PIName = Common.GetPIWithDetails();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var invoicetype = Common.getinvoicetype();
                var Invoice = Common.GetInvoicedetails();
                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.TypeofInvoice = invoicetype;
                ViewBag.Invoice = Invoice;
                ViewBag.Status = Common.getreceiptstatus();
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                ReceiptSearchFieldModel srchModel = new ReceiptSearchFieldModel();
                InvoiceSearchFieldModel srchinvoiceModel = new InvoiceSearchFieldModel();
                var pjcttype = 2;
                srchModel.ProjectType = pjcttype;
                srchinvoiceModel.ProjectType = pjcttype;
                var frmdate = DateTime.Today.AddDays(-15);
                var todate = DateTime.Now;
                model.InvoiceFromDate = frmdate;
                model.InvoiceToDate = todate;
                model.ProjectType = pjcttype;
                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);
                model = coreAccountService.GetInvoiceList(model);
                model.Userrole = user_role;
                model.SearchResult = data;
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConsSearchReceiptList(ReceiptSearchFieldModel srchModel, int page)
        {
            try
            {
                int pageSize = 5;
                var data = new PagedData<ReceiptSearchResultModel>();
                ReceiptListModel model = new ReceiptListModel();
                if (srchModel.ToDate != null)
                {
                    DateTime todate = (DateTime)srchModel.ToDate;
                    srchModel.ToDate = todate.Date.AddDays(1).AddTicks(-1);
                }
                //else if (srchModel.ToCreateDate != null)
                //{
                //    DateTime todate = (DateTime)srchModel.ToCreateDate;
                //    srchModel.ToCreateDate = todate.Date.AddDays(1).AddTicks(-1);
                //}

                data = coreAccountService.GetReceiptList(srchModel, page, pageSize);

                model.SearchResult = data;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("DashBoard", "Home");
            }
        }

        [HttpPost]
        public JsonResult Loadexchangerate(string Currencyid)
        {
            Currencyid = Currencyid == "" ? "0" : Currencyid;
            object output = coreAccountService.LoadExchangerate(Convert.ToInt32(Currencyid));
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorized]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadCurrency(string CountryId)
        {
            CountryId = CountryId == "" ? "0" : CountryId;
            var locationdata = coreAccountService.getCurrency(Convert.ToInt32(CountryId));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadInvoiceProcessList()
        {
            object output = coreAccountService.GetInvoiceProcessList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LoadInvoiceProcess(int InvoiceId)
        {
            object output = coreAccountService.GetInvoiceDetails(InvoiceId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Other Receipts
        public ActionResult OtherReceiptList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetOtherReceiptList()
        {
            try
            {
                object output = coreAccountService.GetOtherReceiptList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public JsonResult ApproveOtherReceipt(int id)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.ApproveOtherReceipt(id, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult OtherReceipt(int id = 0)
        {
            try
            {
                OtherReceiptModel model = new OtherReceiptModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                //var ptypeList = Common.getprojecttype();
                //int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                //ViewBag.ProjectTypeList = ptypeList;
                //ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                if (id > 0 && Common.ValidateReceiptStatus(id, "Open"))
                {
                    model = coreAccountService.GetOtherReceiptDetails(id);
                }
                else
                {
                    int[] heads = { 36, 37, 38 };
                    model.DeductionDetail = coreAccountService.GetTaxHeadDetails(heads);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }

        [HttpPost]
        public ActionResult OtherReceipt(OtherReceiptModel model)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                if (ModelState.IsValid)
                {
                    string validationMsg = ValidateOtherReceipt(model);
                    if (validationMsg != "Valid")
                    {
                        TempData["errMsg"] = validationMsg;
                        return View(model);
                    }
                    if (model.file != null)
                    {
                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                        string taxprooffilename = Path.GetFileName(model.file.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            TempData["errMsg"] = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    int logged_in_user = Common.GetUserid(User.Identity.Name);
                    model.ClassificationOfReceipt = 1;
                    int result = coreAccountService.OtherReceiptIU(model, logged_in_user);
                    if (model.ReceiptId == null && result > 0)
                    {
                        TempData["succMsg"] = "Receipt has been added successfully.";
                        return RedirectToAction("OtherReceiptList");
                    }
                    else if (model.ReceiptId > 0 && result > 0)
                    {
                        TempData["succMsg"] = "Receipt has been updated successfully.";
                        return RedirectToAction("OtherReceiptList");
                    }
                    else
                        TempData["errMsg"] = "Something went wrong please contact administrator.";

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    TempData["errMsg"] = messages;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                foreach (var item in model.ExpenseDetail)
                {
                    int headId = item.AccountGroupId ?? 0;
                    item.AccountHeadList = Common.GetAccountHeadList(headId);
                }
                return View(model);
            }
        }
        public ActionResult OtherReceiptView(int id)
        {
            try
            {
                OtherReceiptModel model = new OtherReceiptModel();
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.AccountHeadList = emptyList;
                ViewBag.TransactionTypeList = Common.GetCodeControlList("Transaction Type");
                ViewBag.CategoryList = Common.GetCodeControlList("ReceiptCategory", "Adhoc");
                ViewBag.AccountGroupList = Common.GetAccountGroup(false);
                ViewBag.BankList = Common.GetBankAccountHeadList();
                ViewBag.ModeOfReceiptList = Common.GetCodeControlList("ModeofReceipt");
                model = coreAccountService.GetOtherReceiptDetails(id);
                ViewBag.disabled = "Disabled";
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        private string ValidateOtherReceipt(OtherReceiptModel model)
        {
            string msg = "Valid";
            decimal crAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal drAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttlTax = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            drAmt = drAmt + (model.BankAmount ?? 0);
            crAmt = crAmt + ttlTax;
            if (drAmt != crAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";
            var ah = model.ExpenseDetail.Where(m => m.IsJV != true).ToList();
            var gAH = ah.GroupBy(v => v.AccountHeadId);
            if (ah.Count() != gAH.Count())
                msg = msg == "Valid" ? "Duplicate account head exist in expense details. Please select a different head." : msg + "<br />Duplicate account head exist in expense details. Please select a different head.";
            return msg;
        }
        #endregion
        #region Payment Process
        public ActionResult PaymentProcessInitList()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetPaymentProcessInitList()
        {
            try
            {
                object output = coreAccountService.PaymentProcessInitList();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PaymentDetails(string viewType, int? boaDraftId = null, int? payeeId = null, bool isEditMode = false, int? modeOfPayment = null)
        {
            try
            {
                if (viewType != "Verify")
                    ViewBag.ViewMode = "true";
                ViewBag.ViewType = viewType != "Single" && viewType != "Verify" ? "Group" : viewType;
                var data = coreAccountService.GetPaymentDetails(viewType, boaDraftId, payeeId, isEditMode, modeOfPayment);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public ActionResult PaymentProcess(int? boaDraftId = null, string mode = "I")
        {
            try
            {

                PaymentProcessVoucherModel model = new PaymentProcessVoucherModel();
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                if (boaDraftId != null)
                    model = coreAccountService.GetPaymentProcessVoucher(boaDraftId ?? 0);
                else
                    model.VoucherDate = String.Format("{0:ddd dd-MMM-yyyy}", DateTime.Now);
                model.BOADraftId = boaDraftId;
                model.Mode = mode;
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        [HttpPost]
        public ActionResult PaymentProcess(PaymentProcessVoucherModel model)
        {
            try
            {
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                int userId = Common.GetUserid(User.Identity.Name);
                var result = coreAccountService.BOADraftIU(model, userId);
                if (model.BOADraftId == null && result > 0)
                {
                    TempData["succMsg"] = "Payment Process has been initiated successfully.";
                    return RedirectToAction("PaymentProcessInitList");
                }
                else if (model.BOADraftId > 0 && result > 0)
                {
                    TempData["succMsg"] = "Payment Process has been updated successfully.";
                    return RedirectToAction("PaymentProcessInitList");
                }
                else if (result == -2)
                {
                    TempData["succMsg"] = "Please verify at least one payment from the list.";
                    return View(model);
                }
                else if (result == -3)
                {
                    TempData["errMsg"] = "Some of the payment are partially verified.";
                    return View(model);
                }
                else
                    TempData["errMsg"] = "Something went wrong please contact administrator.";

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                ViewBag.ModeOfPaymentList = Common.GetCodeControlList("PaymentMode");
                return View(model);
            }

        }
        [HttpGet]
        public JsonResult GetPaymentProcessList(int? boaDraftId = null, bool isViewMode = false)
        {
            try
            {
                object output = coreAccountService.GetPaymentProcessList(boaDraftId, isViewMode);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult ExecutePaymentSP(int? boaDraftId = null)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                coreAccountService.ExecutePaymentSP(userId);
                object output = coreAccountService.GetPaymentProcessList(boaDraftId, false);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[HttpPost]
        //public ActionResult VerifyPaymentProcess(int paymentPayeeId, int modeOfPayment)
        //{
        //    try
        //    {
        //        lock (lockObj)
        //        {

        //            int userId = Common.GetUserid(User.Identity.Name);
        //            bool status = coreAccountService.VerifyPaymentProcess(paymentPayeeId, modeOfPayment, userId);
        //            return Json(new { status = status, msg = !status ? "Something went wrong please contact administrator" : "" }, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { status = false, msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        [HttpPost]
        public ActionResult VerifyPaymentProcess(VerifyPaymentProcessModel model)
        {
            try
            {
                lock (lockObj)
                {
                    if (ModelState.IsValid)
                    {
                        int userId = Common.GetUserid(User.Identity.Name);
                        bool status = coreAccountService.VerifyPaymentProcess(model, userId);
                        if (status)
                            TempData["succMsg"] = "Bill has been verified for payment process successfully.";
                        else
                            TempData["errMsg"] = "Something went wrong please contact administrator.";
                    }
                    else
                    {
                        string messages = string.Join("<br />", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                        TempData["errMsg"] = messages;
                    }
                    return RedirectToAction("PaymentProcess", new { boaDraftId = model.DraftId });
                }
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return RedirectToAction("PaymentProcess", new { boaDraftId = model.DraftId });
            }
        }

        [HttpGet]
        public JsonResult ApproveBOADraft(int boaDraftId)
        {
            try
            {
                int userId = Common.GetUserid(User.Identity.Name);
                object output = coreAccountService.PaymentBOATransaction(boaDraftId, userId);
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}