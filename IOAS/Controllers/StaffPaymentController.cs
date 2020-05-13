using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using IOAS.Infrastructure;
using IOAS.Models;
using IOAS.GenericServices;
using System.Web.Script.Serialization;
using IOAS.Filter;

namespace IOAS.Controllers
{
    [Authorized]
    public class StaffPaymentController : Controller
    {
        CoreAccountsService coreAccountService = new CoreAccountsService();
        StaffPaymentService payment = new StaffPaymentService();
        AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
        FinOp fo = new FinOp(System.DateTime.Now);
        DateTime Today = System.DateTime.Now;
        private static readonly Object lockObj = new Object();


        [Authorize]
        public ActionResult Salary(int PaymentHeadId = 0)
        {
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
            int processGuideLineId = Process.ProcessGuidelineId;

            int page = 1;
            int pageSize = 5;

            ViewBag.processGuideLineId = processGuideLineId;
            ViewBag.currentRefId = -1;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ViewBag.ModeOfPayment = payment.GetStatusFields("SalaryPaymentType");
            ViewBag.Department = adhoc.GetDepartments();

            PagedList<AdhocEmployeeModel> model = new PagedList<AdhocEmployeeModel>();
            var peymentHead = adhoc.GetSalayPaymentHead(PaymentHeadId);

            string PaymentMonthYear = "";
            string Status = "open";
            //if (PaymentHeadId > 0)
            //{
            //    ViewBag.currentRefId = PaymentHeadId;
            //    model = adhoc.GetMainSalaryEmployees(PaymentMonthYear, PaymentHeadId, page, pageSize);
            //}
            //else if (PaymentMonthYear != "")
            //{
            //}
            if (PaymentMonthYear != "" || PaymentHeadId > 0 || peymentHead != null)
            {
                PaymentMonthYear = peymentHead.PaymentMonthYear;
                model = adhoc.GetMainSalaryEmployees(PaymentMonthYear, PaymentHeadId, page, pageSize);
            }

            if (peymentHead != null)
            {
                ViewBag.SelectedPaytype = peymentHead.TypeOfPayBill;
                ViewBag.SelectedPaymonth = peymentHead.PaymentMonthYear;
            }

            if (Status == null || Status.ToLower() == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
                ViewBag.Mode = "Edit";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
                ViewBag.Mode = "View";
            }

            model.CurrentPage = page;
            model.pageSize = pageSize;
            model.visiblePages = 5;

            return View("SalaryInitGrid", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Salary(PagedList<AdhocEmployeeModel> model, FormCollection formCollection)
        {
            string msg = "";
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);

            int page = 1;
            int pageSize = 5;
            var btnSave = Request["btnSave"];
            var btnProceed = Request["btnProceed"];
            var btnBack = Request["btnBack"];
            var Department = Request["departmentcode"];
            var PaymentMonthYear = Request["PaymentMonthYear"];
            var paymentHeadId = Request["PaymentHeadId"];
            var Status = Request["Status"];
            var typeOfPayBill = Request["TypeOfPayBill"];
            var EmployeeId = Request["EmployeeId"];
            var EmployeeName = Request["EmployeeName"];
            var DepartmentCode = Request["DepartmentCode"];

            int PaymentHeadId = (paymentHeadId != null && paymentHeadId != "") ? Convert.ToInt32(paymentHeadId) : 0;
            int TypeOfPayBill = (typeOfPayBill != null && typeOfPayBill != "") ? Convert.ToInt32(typeOfPayBill) : 0;


            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ViewBag.ModeOfPayment = payment.GetStatusFields("SalaryPaymentType");
            ViewBag.Department = adhoc.GetDepartments();
            ViewBag.SelectedPaytype = TypeOfPayBill;
            ViewBag.SelectedPaymonth = PaymentMonthYear;
            ViewBag.SelectedDepartment = Department;


            if (TypeOfPayBill <= 0)
            {
                msg = "Please select Type of PayBill";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("SalaryInitGrid", model);
            }
            else if (PaymentMonthYear == "" || PaymentMonthYear == null)
            {
                //msg = "Please select Payment Month Year";
                //ModelState.AddModelError("", msg);
                //ViewBag.Errors = msg;
                return View("SalaryInitGrid", model);
            }
            else if (fo.IsFutureMonthYear(PaymentMonthYear) == false)
            {
                msg = "Please selected Month Year can not be future month";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("SalaryInitGrid", model);
            }
            var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);

            if (PaymentMonthYear != null && PaymentMonthYear != "" && paybill.ToLower() == "main")
            {
                adhoc.setFilter(EmployeeId, EmployeeName, DepartmentCode);
                model = adhoc.GetMainSalaryEmployees(PaymentMonthYear, PaymentHeadId, page, pageSize);
            }
            else if (PaymentMonthYear != null && paybill.ToLower() == "supplementary")
            {
                model = adhoc.GetSubSalaryEmployees(PaymentMonthYear, page, pageSize);
            }

            var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
            int processGuideLineId = Process.ProcessGuidelineId;
            int refId = (PaymentHeadId > 0) ? PaymentHeadId : -1;

            model.CurrentPage = page;
            model.pageSize = pageSize;
            model.visiblePages = 5;

            ViewBag.processGuideLineId = processGuideLineId;
            ViewBag.currentRefId = refId;

            TempData["PaymentHeadId"] = PaymentHeadId;
            TempData["PaymentMonthYear"] = PaymentMonthYear;
            TempData["TypeOfPayBill"] = TypeOfPayBill;
            if (PaymentHeadId <= 0)
            {
                ViewBag.AllowProceed = "disabled";
            }
            else
            {
                ViewBag.AllowProceed = "";
            }
            if (Status == null || Status == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }

            if (btnBack != null)
            {
                return RedirectToAction("List");
            }
            else if (btnProceed != null)
            {
                return RedirectToAction("Commitment");
            }
            else
            {
                return View("SalaryInitGrid", model);
            }
        }


        public ActionResult SearchEmployeeSalary(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, int page)
        {
            string msg = "";
            int pageSize = 5;

            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            List<AdhocEmployeeModel> employee = new List<AdhocEmployeeModel>();
            List<SalaryModel> SalaryList = new List<SalaryModel>();
            ViewBag.ModeOfPayment = payment.GetStatusFields("SalaryPaymentType");
            ViewBag.SelectedPaytype = model.TypeOfPayBill;
            ViewBag.SelectedPaymonth = model.PaymentMonthYear;
            ViewBag.SelectedDepartment = Department;
            var pagedList = new PagedList<AdhocEmployeeModel>();

            if (model.TypeOfPayBill <= 0)
            {
                msg = "Please select Type of PayBill";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("StaffPartialList", pagedList);
            }
            else if (model.PaymentMonthYear == "" || model.PaymentMonthYear == null)
            {
                msg = "Please select Payment Month Year";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("StaffPartialList", pagedList);
            }
            else if (fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
            {
                msg = "Please selected Month Year can not be future month";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("StaffPartialList", pagedList);
            }
            var paybill = adhoc.GetStatusFieldById("PayOfBill", model.TypeOfPayBill);
            if (model.PaymentMonthYear != null && model.PaymentMonthYear != "" && paybill.ToLower() == "main")
            {
                adhoc.setFilter(EmployeeId, EmployeeName, Department);
                pagedList = adhoc.GetMainSalaryEmployees(model.PaymentMonthYear, model.PaymentHeadId, page, pageSize);
            }
            else if (model.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
            {
                pagedList = adhoc.GetSubSalaryEmployees(model.PaymentMonthYear, page, pageSize);
            }

            var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
            int processGuideLineId = Process.ProcessGuidelineId;
            int refId = (model.PaymentHeadId > 0) ? model.PaymentHeadId : -1;
            pagedList.CurrentPage = page;
            pagedList.pageSize = pageSize;
            pagedList.visiblePages = 5;

            return PartialView("StaffPartialList", pagedList);
        }

        public ActionResult SearchSelectedEmployee(SalaryPaymentHead model, string Department, string EmployeeId, string EmployeeName, int page)
        {
            string msg = "";
            int pageSize = 5;

            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            List<AdhocEmployeeModel> employee = new List<AdhocEmployeeModel>();
            List<SalaryModel> SalaryList = new List<SalaryModel>();
            ViewBag.ModeOfPayment = payment.GetStatusFields("SalaryPaymentType");
            ViewBag.SelectedPaytype = model.TypeOfPayBill;
            ViewBag.SelectedPaymonth = model.PaymentMonthYear;
            ViewBag.SelectedDepartment = Department;
            var pagedList = new PagedList<AdhocEmployeeModel>();

            if (model.TypeOfPayBill <= 0)
            {
                msg = "Please select Type of PayBill";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("StaffPartialList", pagedList);
            }
            else if (model.PaymentMonthYear == "" || model.PaymentMonthYear == null)
            {
                msg = "Please select Payment Month Year";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("StaffPartialList", pagedList);
            }
            else if (fo.IsFutureMonthYear(model.PaymentMonthYear) == false)
            {
                msg = "Please selected Month Year can not be future month";
                ModelState.AddModelError("", msg);
                ViewBag.Errors = msg;
                return View("StaffPartialList", pagedList);
            }
            if (model.PaymentHeadId == 0)
            {
                model.PaymentHeadId = -1;
            }
            var paybill = adhoc.GetStatusFieldById("PayOfBill", model.TypeOfPayBill);
            if (model.PaymentMonthYear != null && model.PaymentMonthYear != "" && paybill.ToLower() == "main")
            {
                adhoc.setFilter(EmployeeId, EmployeeName, Department);
                pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, page, pageSize);
            }
            else if (model.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
            {
                pagedList = adhoc.GetEmployeesByPaymentHead(model.PaymentHeadId, page, pageSize);
            }

            var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
            int processGuideLineId = Process.ProcessGuidelineId;
            int refId = (model.PaymentHeadId > 0) ? model.PaymentHeadId : -1;
            pagedList.CurrentPage = page;
            pagedList.pageSize = pageSize;
            pagedList.visiblePages = 5;

            return PartialView("StaffSelectedPartialList", pagedList);
        }

        [Authorize]
        public ActionResult SalaryPayment()
        {
            int page = 1;
            int pageSize = 10;
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            var PaymentHeadId = TempData["PaymentHeadId"];
            int paymentId = 0;
            if (PaymentHeadId != null)
            {
                paymentId = Convert.ToInt32(PaymentHeadId);
            }

            //var monthYear = fo.GetCurrentMonthYear();
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }

            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var model = payment.GetEmployeeSalary(page, pageSize, paymentId);
            ViewBag.SelectedMonth = paymentMonthYear;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            var SalaryPaymentHead = adhoc.GetSalayPaymentHead(paymentId);
            if (SalaryPaymentHead == null || SalaryPaymentHead.Status == null || SalaryPaymentHead.Status.ToLower() == "open")
            {
                ViewBag.AllowSubmit = "";
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSubmit = "disabled";
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }
            TempData["PaymentMonthYear"] = paymentMonthYear;
            TempData["PaymentHeadId"] = paymentId;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SalaryPayment(int page, int pageSize, FormCollection formCollection)
        {
            var btnSave = Request["btnSave"];
            var btnSubmit = Request["btnSubmit"];
            var btnBack = Request["btnBack"];

            var PaymentHeadId = TempData["PaymentHeadId"];
            int paymentId = 0;
            if (PaymentHeadId != null)
            {
                paymentId = Convert.ToInt32(PaymentHeadId);
            }

            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var model = payment.GetEmployeeSalary(page, pageSize, paymentId);
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            ViewBag.SelectedMonth = paymentMonthYear;

            var SalaryPaymentHead = adhoc.GetSalayPaymentHead(paymentId);
            if (SalaryPaymentHead == null || SalaryPaymentHead.Status == null || SalaryPaymentHead.Status.ToLower() == "open")
            {
                ViewBag.AllowSubmit = "";
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSubmit = "disabled";
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }

            TempData["PaymentMonthYear"] = paymentMonthYear;
            TempData["PaymentHeadId"] = paymentId;

            if (btnBack != null)
            {
                return RedirectToAction("Transaction");
            }
            else if (btnSubmit != null)
            {

                SalaryPaymentHead payHead = new SalaryPaymentHead();

                string currentStatus = "open";
                string newStatus = "Approval Pending";
                var msg = adhoc.UpdateSalaryPayment(paymentId, currentStatus, newStatus, userId);
                if (msg == "")
                {
                    return View(model);
                }
                TempData["Message"] = msg;
                return RedirectToAction("List");
            }
            return View(model);
        }


        [Authorize]
        public ActionResult Commitment()
        {
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = (TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : -1;
            var model = payment.GetProjectCommitment(paymentMonthYear, PaymentHeadId);
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");


            TempData["PaymentHeadId"] = PaymentHeadId;
            TempData["PaymentMonthYear"] = paymentMonthYear;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Commitment(int page, int pageSize, FormCollection formCollection)
        {

            var btnSave = Request["btnSave"];
            var btnProceed = Request["btnProceed"];
            var btnBack = Request["btnBack"];

            string message = "";
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = Convert.ToInt32((TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : -1);

            var model = payment.GetProjectCommitment(paymentMonthYear, PaymentHeadId);


            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");

            TempData["PaymentHeadId"] = PaymentHeadId;
            TempData["PaymentMonthYear"] = paymentMonthYear;
            if (btnProceed != null)
            {
                if (model != null)
                {
                    for (int i = 0; i < model.Count; i++)
                    {
                        if (model[i].IsBalanceAavailable == false)
                        {
                            message = "Commitment balance is too low for CommitmentNo : " + model[i].CommitmentNo;
                            break;
                        }
                    }

                }
                else
                {
                    message = "Invalid model data.";
                }
            }


            if (btnBack != null)
            {
                return RedirectToAction("Salary", new { PaymentHeadId = PaymentHeadId });
            }
            else if (btnProceed != null)
            {
                if (message != "")
                {
                    ViewBag.Errors = message;
                    return View(model);
                }
                return RedirectToAction("Transaction");
            }

            return View(model);

        }

        [Authorize]
        public ActionResult Transaction(int billId = 0)
        {

            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }

            if (TempData["PaymentHeadId"] == null || TempData["PaymentHeadId"].ToString() == "")
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = Convert.ToInt32((TempData["PaymentHeadId"] != null) ? TempData["PaymentHeadId"].ToString() : "");

            ViewBag.AccountGroups = adhoc.GetAccountGroup("SAL-01");
            ViewBag.AccountHead = adhoc.GetAccountHead("SAL-01");


            var model = adhoc.GetSalaryTransaction(PaymentHeadId, paymentMonthYear);
            //var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
            //model.ExpenseDetail = tx.ExpenseDetail;
            //model.DeductionDetail = tx.DeductionDetail;

            ViewBag.CreditAmount = model.TotalCredit;
            ViewBag.DebitAmount = model.TotalDebit;
            if (model == null || model.TransactionId <= 0)
            {
                ViewBag.AllowProceed = "disabled";
            }
            else
            {
                ViewBag.AllowProceed = "";
            }

            if (model == null || model.Status == null || model.Status.ToLower() == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }

            TempData["PaymentMonthYear"] = paymentMonthYear;
            TempData["PaymentHeadId"] = PaymentHeadId;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Transaction(SalaryTransaction model, FormCollection formCollection)
        {
            var btnSave = Request["btnSave"];
            var btnProceed = Request["btnProceed"];
            var btnBack = Request["btnBack"];
            var btnAddRow = Request["btnAddRow"];
            var CreditAmount = Request["CreditAmount"];
            var DebitAmount = Request["DebitAmount"];

            string msg = "";
            var user = User.Identity.Name;
            var userId = AdminService.getUserByName(user);
            if (TempData["PaymentMonthYear"] == null)
            {
                return RedirectToAction("Salary");
            }
            var paymentMonthYear = (TempData["PaymentMonthYear"] != null) ? TempData["PaymentMonthYear"].ToString() : "";
            var PaymentHeadId = (TempData["PaymentHeadId"] != null) ? Convert.ToInt32(TempData["PaymentHeadId"].ToString()) : 0;
            ViewBag.months = fo.GetAllMonths();
            ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
            TempData["PaymentMonthYear"] = paymentMonthYear;

            ViewBag.CreditAmount = CreditAmount;
            ViewBag.DebitAmount = DebitAmount;

            ViewBag.AccountGroups = adhoc.GetAccountGroup("SAL-01");
            ViewBag.AccountHead = adhoc.GetAccountHead("SAL-01");

            if (model.ExpenseDetail == null)
            {
                var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
                model.ExpenseDetail = tx.ExpenseDetail;
                model.DeductionDetail = tx.DeductionDetail;
            }


            if (btnAddRow != null)
            {
                if (model.detail == null)
                {
                    model.detail = new List<SalaryTransactionDetail>();
                }
                model.detail.Add(new SalaryTransactionDetail
                {
                    AccountGroupId = 61,
                    AccountHeadId = 138,
                    Amount = 0
                });
                return View(model);
            }

            if (ModelState.IsValid && CreditAmount == DebitAmount)
            {
                msg = adhoc.SalaryTransactionIU(model, userId);
                ViewBag.Message = msg;
                model = adhoc.GetSalaryTransaction(PaymentHeadId, paymentMonthYear);
            }
            else
            {
                var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
                model.ExpenseDetail = tx.ExpenseDetail;
                model.DeductionDetail = tx.DeductionDetail;
                string messages = string.Join("<br />", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));
                if (messages == "" && CreditAmount != DebitAmount)
                {
                    messages = "Credit is not matching with debit.";
                }
                TempData["errMsg"] = messages;
                ViewBag.Message = messages;

            }

            TempData["PaymentHeadId"] = PaymentHeadId;
            if (model == null || model.TransactionId <= 0)
            {
                ViewBag.AllowProceed = "disabled";
            }
            else
            {
                ViewBag.AllowProceed = "";
            }
            if (model == null || model.Status == null || model.Status.ToLower() == "open")
            {
                ViewBag.AllowSave = "";
                ViewBag.AllowEdit = "";
            }
            else
            {
                ViewBag.AllowSave = "disabled";
                ViewBag.AllowEdit = "disabled";
            }
            if (btnBack != null)
            {
                return RedirectToAction("Commitment");
            }
            else if (btnProceed != null)
            {
                return RedirectToAction("SalaryPayment");
            }

            return View(model);
        }



        // GET: List
        [Authorize]
        public ActionResult List()
        {
            try
            {

                int pageSize = 10;
                int page = 1;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                var PaymentMonthYear = fo.GetCurrentMonthYear();
                ViewBag.Message = TempData["Message"];

                var model = adhoc.ListSalayPayment(page, pageSize);
                model.CurrentPage = page;
                model.pageSize = pageSize;
                model.visiblePages = 5;
                ViewBag.months = fo.GetAllMonths();
                ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
                int paymentHeadId = -1;
                //if(model.Data != null && model.Data.Count >0)
                //{
                //    paymentHeadId = model.Data[0].
                //}

                var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
                int processGuideLineId = Process.ProcessGuidelineId;
                ViewBag.processGuideLineId = processGuideLineId;
                ViewBag.currentRefId = -1;

                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: PaymentEntry
        [HttpPost]
        [Authorize]
        public ActionResult List(int page, FormCollection formCollection)
        {
            try
            {
                var testButton = Request["btnViewModal"];

                var btnSave = Request["btnSave"];
                var btnProceed = Request["btnProceed"];
                var PaymentMonthYear = Request["PaymentMonthYear"].ToString();
                var TypeOfPayBill = (Request["TypeOfPayBill"] != null && Request["TypeOfPayBill"] != "") ? Convert.ToInt32(Request["TypeOfPayBill"].ToString()) : 0;
                var PayBillNo = Request["PayBillNo"].ToString();

                int pageSize = 10;

                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                string msg = "";
                ViewBag.selectedMonth = PaymentMonthYear;
                ViewBag.SelectedTypeOfPaybill = TypeOfPayBill;
                ViewBag.PayBillNo = PayBillNo;
                ViewBag.months = fo.GetAllMonths();
                ViewBag.PaymentType = payment.GetStatusFields("PayOfBill");
                ViewBag.ModeOfPayment = payment.GetStatusFields("SalaryPaymentType");
                //var model = adhoc.GetSalayPaymentList(PaymentMonthYear, TypeOfPayBill, PayBillNo, page, pageSize);
                var model = adhoc.ListSalayPayment(page, pageSize);
                model.CurrentPage = page;
                model.pageSize = pageSize;
                model.visiblePages = 5;

                if (btnSave != null || btnProceed != null)
                {
                    if (TypeOfPayBill <= 0)
                    {
                        msg = "Please select Type of PayBill";
                        ModelState.AddModelError("", msg);
                        ViewBag.Errors = msg;
                        return View(model);
                    }
                    else if (PaymentMonthYear == "" || PaymentMonthYear == null)
                    {
                        msg = "Please select Payment Month Year";
                        ModelState.AddModelError("", msg);
                        ViewBag.Errors = msg;
                        return View(model);
                    }
                    else if (fo.IsFutureMonthYear(PaymentMonthYear) == false)
                    {
                        msg = "Please selected Month Year can not be future month";
                        ModelState.AddModelError("", msg);
                        ViewBag.Errors = msg;
                        return View(model);
                    }
                }

                TempData["PaymentMonthYear"] = PaymentMonthYear;
                TempData["TypeOfPayBill"] = TypeOfPayBill;
                TempData["PayBillNo"] = PayBillNo;

                var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
                int processGuideLineId = Process.ProcessGuidelineId;
                int refId = -1;

                ViewBag.processGuideLineId = processGuideLineId;
                ViewBag.currentRefId = refId;

                if (btnProceed != null)
                {
                    return RedirectToAction("Salary");
                }
                else
                {
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: List
        [Authorize]
        public JsonResult EmplyeeSalaryDetail(string EmpNo, string PaymentMonthYear, int TypeOfPayBill)
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                AdhocEmployeeModel model = new AdhocEmployeeModel();

                var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);
                if (PaymentMonthYear != "" && paybill.ToLower() == "main")
                {
                    model = adhoc.GetAnEmployeeDetails(EmpNo, PaymentMonthYear);
                }
                else if (PaymentMonthYear != "" && paybill.ToLower() == "supplementary")
                {
                    model = adhoc.GetSubSalaryEmployee(EmpNo, PaymentMonthYear);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public JsonResult VerifyEmplyeeSalary(AdhocEmployeeModel employee, int PaymentHeadId, bool verify)
        {
            //string EmpNo, string PaymentMonthYear, int TypeOfPayBill, int PaymentHeadId, int modeOfPay
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                SalaryPaymentHead headerModel = new SalaryPaymentHead();
                string EmpNo = employee.EmployeeID;
                string PaymentMonthYear = employee.PaymentMonthYear;
                int TypeOfPayBill = employee.TypeOfPayBill;
                int modeOfPay = employee.ModeOfPayment;
                var paybill = adhoc.GetStatusFieldById("PayOfBill", TypeOfPayBill);
                if (PaymentMonthYear != "" && paybill.ToLower() == "main")
                {
                    model = adhoc.GetAnEmployeeDetails(EmpNo, PaymentMonthYear);
                }
                else if (PaymentMonthYear != "" && paybill.ToLower() == "supplementary")
                {
                    model = adhoc.GetSubSalaryEmployee(EmpNo, PaymentMonthYear);
                }
                model.ModeOfPayment = employee.ModeOfPayment;
                headerModel.PaymentHeadId = PaymentHeadId;
                headerModel.PaymentMonthYear = PaymentMonthYear;
                headerModel.TypeOfPayBill = TypeOfPayBill;
                headerModel.PaymentHeadId = PaymentHeadId;
                headerModel.AdhocEmployees = new List<AdhocEmployeeModel>();
                headerModel.AdhocEmployees.Add(model);
                if (verify == false)
                {
                    int ret = adhoc.RemoveVerifiedEmployee(PaymentHeadId, EmpNo, userId, verify);
                }
                else if (headerModel != null)
                {
                    var result = adhoc.EmployeeSalaryIU(headerModel, userId);
                    if (result > 0)
                    {
                        PaymentHeadId = result;
                        headerModel.PaymentHeadId = PaymentHeadId;
                    }
                }

                return Json(headerModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: List
        [Authorize]
        public JsonResult GetSalaryPayment(int page, int pageSize)
        {
            try
            {
                DateTime Today = System.DateTime.Now;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                var PaymentHeadId = 0;
                var model = payment.GetEmployeeSalary(page, pageSize, PaymentHeadId);

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        #region Agency Salary
        public ActionResult AgencySalaryList()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AgencySalary(int agencySalaryId = 0)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                AgencySalaryModel model = new AgencySalaryModel();
                var data = new PagedData<AgencyStaffDetailsModel>();
                string currMY = fo.GetCurrentMonthYear();
                model.MonthYear = currMY;
                string[] status = { "Open", "Init" };
                if (agencySalaryId > 0 && payment.ValidateAgencySalaryStatus(agencySalaryId, status))
                {
                    model = payment.GetAgencySalaryDetails(agencySalaryId);
                    if (model.CheckListDetail.Count == 0)
                        model.CheckListDetail = Common.GetCheckedList(63);
                }
                else if (agencySalaryId > 0)
                    return RedirectToAction("AgencySalaryList");
                int Page = 1, Pagesize = 5;
                data = payment.GetAgencyEmployeeSalary(Page, Pagesize, agencySalaryId, currMY);
                data.pageSize = 5;
                data.visiblePages = 5;
                data.CurrentPage = Page;
                model.EmployeeDetails = data;
                model.CreditorType = "Agency";
                model.PaymentNo = Common.getPaymentNo(model.AgencySalaryID ?? 0);
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }

        }
        [HttpPost]
        public ActionResult AgencySalary(AgencySalaryModel model)
        {
            try
            {
                model.CreditorType = "Agency";
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
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

                    string validationMsg = ValidateAgencySalary(model);
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
                    int result = payment.CreateSalaryAgency(model, logged_in_user);
                    if (result > 0)
                    {
                        TempData["succMsg"] = "Agency salary payment has been updated successfully.";
                        return RedirectToAction("AgencySalaryList");
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
                ViewBag.months = fo.GetAllMonths();
                ViewBag.PaymentType = Common.GetCodeControlList("PaymentType");
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                TempData["errMsg"] = "Something went wrong please contact administrator.";
                return View(model);
            }
        }

        private string ValidateAgencySalary(AgencySalaryModel model)
        {
            decimal netSalAmt = model.NetAmount ?? 0;
            decimal payableAmt = model.NetPayable ?? 0;
            string msg = "Valid";
            decimal netCrAmt = model.CreditorAmount ?? 0;
            decimal commitmentAmt = model.NetCommitmentAmount ?? 0;
            decimal ttlExpAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Credit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldrAmt = model.ExpenseDetail.Where(m => m.TransactionType == "Debit").Select(m => m.Amount).Sum() ?? 0;
            decimal ttldeductAmt = model.DeductionDetail.Select(m => m.Amount).Sum() ?? 0;
            decimal netDrAmt = ttldrAmt + ttldeductAmt;

            if (netSalAmt > commitmentAmt || payableAmt < commitmentAmt)
                msg = "There is a mismatch between the requested advance value and allocated commitment value. Please update the value to continue.";
            foreach (var item in model.CommitmentDetail)
            {
                if (item.PaymentAmount > item.AvailableAmount)
                    msg = msg == "Valid" ? "Commitment payment value should not be greater than available balance." : msg + "<br /> Commitment payment value should not be greater than available balance.";
            }
            if (netCrAmt != ttlExpAmt || netCrAmt != netDrAmt)
                msg = msg == "Valid" ? "Not a valid entry. Credit and Debit value are not equal" : msg + "<br />Not a valid entry. Credit and Debit value are not equal";

            if (payableAmt != netCrAmt)
                msg = msg == "Valid" ? "TNot a valid entry. Net payable and transaction value are not equal." : msg + "<br /> Not a valid entry. Net payable and transaction value are not equal.";
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
        public ActionResult AgencySalaryView(int agencySalaryId)
        {
            try
            {
                var emptyList = new List<MasterlistviewModel>();
                ViewBag.months = fo.GetAllMonths();
                ViewBag.AccountGroupList =
                ViewBag.TypeOfServiceList =
                ViewBag.AccountHeadList = emptyList;
                ViewBag.DocmentTypeList = Common.GetDocTypeList(63);
                var ptypeList = Common.getprojecttype();
                int firstPType = ptypeList != null ? ptypeList[0].codevalAbbr : 0;
                ViewBag.ProjectTypeList = ptypeList;
                ViewBag.ProjectNumberList = ProjectService.LoadProjecttitledetails(firstPType);
                AgencySalaryModel model = new AgencySalaryModel();
                var data = new PagedData<AgencyStaffDetailsModel>();
                string currMY = fo.GetCurrentMonthYear();
                model.MonthYear = currMY;
                model = payment.GetAgencySalaryDetails(agencySalaryId);
                if (model.CheckListDetail.Count == 0)
                    model.CheckListDetail = Common.GetCheckedList(63);
                int Page = 1, Pagesize = 5;
                data = payment.GetAgencyEmployeeSalary(Page, Pagesize, agencySalaryId, currMY);
                data.pageSize = 5;
                data.visiblePages = 5;
                data.CurrentPage = Page;
                model.EmployeeDetails = data;
                model.CreditorType = "Agency";
                model.PaymentNo = Common.getPaymentNo(model.AgencySalaryID ?? 0);
                ViewBag.disabled = "readonly";
                return View(model);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Dashboard", "Home");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _AgencySalaryStaffDetailsPartial(int Page, int AgencySalaryID, string MonthYear)
        {
            AgencySalaryModel model = new AgencySalaryModel();
            model.EmployeeDetails = new PagedData<AgencyStaffDetailsModel>();
            StaffPaymentService _SPS = new StaffPaymentService();
            int Pagesize = 5;
            model.EmployeeDetails = _SPS.GetAgencyEmployeeSalary(Page, Pagesize, AgencySalaryID, MonthYear);
            model.EmployeeDetails.pageSize = 5;
            model.EmployeeDetails.visiblePages = 5;
            model.EmployeeDetails.CurrentPage = Page;
            model.EmployeeDetails = model.EmployeeDetails;
            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetAgencySalaryList()
        {
            try
            {
                var model = payment.getAgencySalaryList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public JsonResult SearchAgencySalaryList(AgencySearchFieldModel model)
        {
            object output = StaffPaymentService.SearchAgencySalaryList(model);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchAgencySalaryVerifiedList(int pageIndex, int pageSize, int AgencySalaryId, string EmployeeId, string Name)
        {
            object output = payment.GetVerifiedEmployeeSalary(pageIndex, pageSize, AgencySalaryId, EmployeeId, Name);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SearchAgencySalaryCommitmentList(int pageIndex, int pageSize, int AgencySalaryId, string CommitmentNumber, string ProjectNumber, string HeadName)
        {
            object output = payment.GetAgencySalaryCommitmentDetail(pageIndex, pageSize, AgencySalaryId, CommitmentNumber, ProjectNumber, HeadName);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PopupEmployeeDetails(string EmployeeID, int AgencySalaryID, string MonthYear)
        {
            try
            {
                AgencyStaffDetailsModel model = new AgencyStaffDetailsModel();
                string validationMsg = payment.ValidateAgencyVerify(EmployeeID, AgencySalaryID, MonthYear);
                if (validationMsg != "Valid")
                    return new HttpStatusCodeResult(400, validationMsg);
                ViewBag.TypeList = Common.GetCodeControlList("Salary Breakup Type");
                ViewBag.HeadList = new List<MasterlistviewModel>();
                model = payment.getEmployeeSalaryDetails(EmployeeID, AgencySalaryID);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteVerifiedEmployee(int VerifiedSalaryId)
        {
            object output = payment.DeleteVerifiedEmployee(VerifiedSalaryId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AgencySalaryApproved(int agencySalaryId)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    string[] chckStatus = { "Open" };
                    if (payment.ValidateAgencySalaryStatus(agencySalaryId, chckStatus))
                    {
                        bool cStatus = payment.SLACommitmentBalanceUpdate(agencySalaryId, false, false, userId, "SLA");
                        if (!cStatus)
                            return Json(new { status = false, msg = "There is a mismatch between the allocated available value and allocated commitment value." }, JsonRequestBehavior.AllowGet);
                        bool status = payment.ApproveSLA(agencySalaryId, userId);
                        if (!status)
                            payment.SLACommitmentBalanceUpdate(agencySalaryId, true, false, userId, "SLA");
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
        [HttpPost]
        public ActionResult VerifyEmployeeDetails(string stringify)
        {
            try
            {
                var model = new JavaScriptSerializer().Deserialize<AgencyVerifyEmployeeModel>(stringify);
                string validationMsg = payment.ValidateAgencyVerify(model.EmployeeId, model.AgencySalaryID, model.MonthYear, model);
                if (validationMsg != "Valid")
                    return Json(new { id = -1, msg = validationMsg });
                int userId = Common.GetUserid(User.Identity.Name);
                var data = payment.VerifyEmployeeDetails(model, userId);
                if (data.Status == "success")
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Status = "error", msg = "Something went wrong please contact administrator" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyAllEmployeeDetails(int AgencySalaryID, string MonthYear)
        {
            try
            {
                lock (lockObj)
                {
                    int userId = Common.GetUserid(User.Identity.Name);
                    var data = payment.VerifyAllEmployeeDetails(AgencySalaryID, MonthYear, userId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult getNetTotalNetSalary(int AgencySalaryID)
        {
            try
            {
                var Result = Common.getSumNetSalary(AgencySalaryID);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var Result = Common.getSumNetSalary(AgencySalaryID);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSalaryBreakUpHead(Int32 categoryId, int groupId)
        {
            try
            {
                object output = Common.GetCommonHeadList(categoryId, groupId);
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