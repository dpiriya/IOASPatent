using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.Models;
using IOAS.GenericServices;


namespace IOAS.Controllers
{
    public class ITDeclarationController : Controller
    {

        StaffPaymentService payment = new StaffPaymentService();
        AdhocSalaryProcess adhoc = new AdhocSalaryProcess();
        OfficeOrderService order = new OfficeOrderService();
        FinOp fp = new FinOp(System.DateTime.Now);
        // GET: ITDeclaration
        public ActionResult Index()
        {
            return View();
        }

        // GET: List
        [Authorize]
        public ActionResult List()
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                EmpITDeductionModel model = new EmpITDeductionModel();
                model.ItList = payment.GetITEmpDeclarations("");
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome();
                model.EmpInfo = adhoc.GetEmployeeByEmpId("emp01");
                model.FinPeriod = fp.FinStart().Year.ToString() + "-" + fp.FinEnd().Year.ToString();
                ViewBag.EmpList = adhoc.GetEmployeeList();
                //model.CurrentPage = page;
                //model.pageSize = pageSize;
                //model.visiblePages = 5;

                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: List
        [Authorize]
        [HttpPost]
        public ActionResult List(EmpITDeductionModel model)
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                string msg = validate(model);
                ViewBag.EmpList = adhoc.GetEmployeeList();

                if (msg != "")
                {

                    model.errMsg = msg;
                    ModelState.Remove("errMsg");
                    ModelState.AddModelError("", msg);
                    ViewBag.Errors = msg;
                    return View(model);
                }
                msg = payment.ITEmpDeclarationIU(model);

                model.ItList = payment.GetITEmpDeclarations(model.EmpInfo.EmployeeID);
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome();
                model.EmpInfo = adhoc.GetEmployeeByEmpId(model.EmpInfo.EmployeeID);
                ModelState.Clear();
                return View(model);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetSOPDeduction()
        {
            try
            {
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);
                EmpITDeductionModel model = new EmpITDeductionModel();
                model.ItList = payment.GetITEmpDeclarations("emp01");
                model.ItSOP = payment.GetITEmpSOP();
                model.ItOtherIncome = payment.GetITEmpOtherIncome();
                model.EmpInfo = adhoc.GetEmployeeByEmpId("emp01");
                //model.CurrentPage = page;
                //model.pageSize = pageSize;
                //model.visiblePages = 5;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetOtherIncomeDeduction()
        {
            try
            {
                int pageSize = 10;
                int page = 1;
                var user = User.Identity.Name;
                var userId = AdminService.getUserByName(user);

                var model = payment.GetITEmpDeclarations("emp01");
                //model.CurrentPage = page;
                //model.pageSize = pageSize;
                //model.visiblePages = 5;

                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        private string validate(EmpITDeductionModel model)
        {
            try
            {
                string msg = "";

                foreach(var item in model.ItList)
                {
                    if(item.MaxLimit != 0 && item.MaxLimit < item.Amount)
                    {
                        msg = item.SectionName + " exceeds max limit " + item.Amount;
                        return msg;
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}