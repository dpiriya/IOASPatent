using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IOAS.DataModel;
using CrystalDecisions.CrystalReports.Engine;
using IOAS.Infrastructure;
using System.IO;
using IOAS.GenericServices;
using System.Data;

namespace IOAS.Controllers
{
    public class ReportMasterController : Controller
    {
        #region TrailBalance
        public ActionResult TrailBalanceReport()
        {
            ViewBag.FinYr = GetFinancialYear();
            return View();
        }
        public ActionResult TrailBalanceRep(int Finyear, int format)
        {
            TrailBalanceModel model = new TrailBalanceModel();
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var UserName = User.Identity.Name;
                    //                var Qry = (from b in context.vw_DemoLedgers
                    //                           where String.IsNullOrEmpty(accounts) || b.Accounts.Contains(accounts)
                    //                           select new
                    //                           {
                    //                               b.AccountGroupId,
                    //                               b.AccountHead,
                    //                               b.AccountHeadId,
                    //                               b.Accounts,
                    //                               b.Amount,
                    //                               b.Creditor_f,
                    //                               b.Debtor_f,
                    //                               b.Groups,
                    //                               b.TransactionType
                    //                           }).ToList();
                    //                var AssetCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Asset")
                    //    .GroupBy(a => a.AccountHeadId)
                    //    .Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //    .OrderByDescending(a => a.Amount)
                    //    .ToList();

                    //                var AssetDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Asset")
                    // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    // .OrderByDescending(a => a.Amount)
                    // .ToList();
                    //                decimal? Ass = 0, ttCr = 0, ttDr = 0;
                    //                var Qray=(from Dr in AssetDr
                    //                          join Cr in AssetCr on Dr.Name equals Cr.Name into temp
                    //                         from Cr in temp.DefaultIfEmpty()
                    //                         select new TrailBalanceModel()
                    //                         {   HeadId=Convert.ToInt32(Dr.Name),
                    //                             Debit = Convert.ToDecimal(Dr.Amount),
                    //                             Credit = Convert.ToDecimal(Cr?.Amount),
                    //                         }).ToList();
                    //                for (int i = 0; i < Qray.Count; i++)
                    //                {
                    //                    Ass = Qray[i].Debit - Qray[i].Credit;
                    //                    if (Ass < 0)
                    //                        ttCr += (-Ass);
                    //                    else
                    //                        ttDr += (Ass);
                    //                }

                    //                string TotalAssetDr = Convert.ToString(ttDr ?? 0);                                       
                    //                string TotalAssetCr = Convert.ToString(ttCr ?? 0);

                    //                var LiabilityCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Liability")
                    // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    // .OrderByDescending(a => a.Amount)
                    // .ToList();

                    //                var LiabilityDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Liability")
                    //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //.OrderByDescending(a => a.Amount)
                    //.ToList();
                    //                string TotalLiabilityDr = Convert.ToString(LiabilityDr.Sum(m => m.Amount) ?? 0);
                    //                string TotalLiabilityCr = Convert.ToString(LiabilityCr.Sum(m => m.Amount) ?? 0);
                    //                var IncomeCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Income")
                    //             .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //             .OrderByDescending(a => a.Amount)
                    //             .ToList();
                    //                var IncomeDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Income")
                    //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //.OrderByDescending(a => a.Amount)
                    //.ToList();
                    //                string TotalIncomeDr = Convert.ToString(IncomeDr.Sum(m => m.Amount) ?? 0);
                    //                string TotalIncomeCr = Convert.ToString(IncomeCr.Sum(m => m.Amount) ?? 0);
                    //                var ExpenseCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Expense")
                    //           .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //           .OrderByDescending(a => a.Amount)
                    //           .ToList();

                    //                var ExpenseDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Expense")
                    //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                    //.OrderByDescending(a => a.Amount)
                    //.ToList();
                    //                string TotaExpenseDr = Convert.ToString(ExpenseDr.Sum(m => m.Amount) ?? 0);
                    //                string TotalExpenseCr = Convert.ToString(ExpenseCr.Sum(m => m.Amount) ?? 0);
                    string conn = "IOASDB";
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "TrailBalance2.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection("10.18.0.11,1433", conn, "sa", "IcsR@123#");
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var data = ReportService.TrailBalanceRep2(Finyear);
                    rd.SetDataSource(data);
                    rd.SetParameterValue("Finyear", Common.GetFinancialYear(Finyear));
                    rd.SetParameterValue("UserName", UserName);
                    Stream stream;
                    if (format == 1)
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=TrailBalance(" + Common.GetFinancialYear(Finyear) + ").pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=TrailBalance(" + Common.GetFinancialYear(Finyear) + ").xls");
                        return File(stream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Report", new { Errormsg = "Something went to wrong please contact admin." });

            }
        }
        public ActionResult GetDataForTrailBalance2(int Finyear)
        {
            var em = 0;
            var data = new object();
            var Qry = ReportService.TrailBalanceRep2(Finyear);
            if (Qry.Count > 0)
            {
                em = 1;
            }
            else { em = 2; }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CashBook
        [HttpGet]
        public ActionResult CashBookReport()
        {
            ViewBag.Bank = Common.GetBankAccount();
            return View();
        }
        public ActionResult CashBook(DateTime fromdate, DateTime todate, int BankId, int format)
        {
            var username = User.Identity.Name;
            var em = 0;
            var res = new object();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Pay = (from c in context.vw_CashBookPayment
                               where (c.ReferenceDate < fromdate) && (c.BankHeadID == BankId)
                               select new
                               { c }).ToList();
                    decimal PayAmt = Pay.Select(m => m.c.Amount).Sum() ?? 0;
                    var Rec = (from c in context.vw_CashBookReceipt
                               where (c.ReferenceDate < fromdate) && (c.BankHeadID == BankId)
                               select new
                               { c }).ToList();
                    decimal RecAmt = Rec.Select(m => m.c.Amount).Sum() ?? 0;
                    var OB = (from c in context.tblHeadOpeningBalance
                              where c.AccountHeadId == BankId
                              select new { c }).FirstOrDefault();
                    string FinalOB = "";
                    decimal? COB = 0;
                    if (OB.c.TransactionType == "Credit")
                    {
                        COB = -OB.c.OpeningBalance + (RecAmt - PayAmt);
                        FinalOB = OB.c.OpeningBalance + " Cr";
                    }
                    else if (OB.c.TransactionType == "Debit") { COB = OB.c.OpeningBalance + (RecAmt - PayAmt); FinalOB = OB.c.OpeningBalance + " Dr"; }
                    var PayCB = (from c in context.vw_CashBookPayment
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal PayAmtCB = PayCB.Select(m => m.c.Amount).Sum() ?? 0;
                    var RecCB = (from c in context.vw_CashBookReceipt
                                 where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && (c.BankHeadID == BankId)
                                 select new
                                 { c }).ToList();
                    decimal RecAmtCB = RecCB.Select(m => m.c.Amount).Sum() ?? 0;
                    decimal? CB = 0;
                    if (COB < 0)
                    {
                        CB = -COB + (RecAmtCB - PayAmtCB);
                    }
                    else if (COB > 0) { CB = COB + (RecAmtCB - PayAmtCB); }
                    string FinalCB = "";
                    if (CB < 0) { FinalCB = -CB + " Cr"; } else { FinalCB = CB + " Dr"; }
                    ReportDocument rd = new ReportDocument();
                    string conn = "IOASDB";
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "CashBook.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection("10.18.0.11,1433", conn, "sa", "IcsR@123#");
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var Receipt = ReportService.CashBookReceiptRep(fromdate, todate, BankId);
                    var Payment = ReportService.CashBookPaymentRep(fromdate, todate, BankId);
                    rd.Subreports[0].SetDataSource(Payment);
                    rd.Subreports[1].SetDataSource(Receipt);
                    rd.SetParameterValue("fromdate", fromdate);
                    rd.SetParameterValue("todate", todate);
                    rd.SetParameterValue("BankId", Common.GetBankName(BankId));
                    rd.SetParameterValue("FinalOB", FinalOB);
                    rd.SetParameterValue("FinalCB", FinalCB);
                    if (username != null)
                    {
                        rd.SetParameterValue("username", username);
                    }
                    Stream stream;
                    if (format == 1)
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=CashBook.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=CashBook.xls");
                        return File(stream, "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                em = 1;
                res = new { em = em };
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDataForCashbookReport(DateTime fromdate, DateTime todate, int BankId)
        {
            var pay = 0;
            var rec = 0;
            var data = new object();
            var Receipt = ReportService.CashBookReceiptRep(fromdate, todate, BankId);
            var Payment = ReportService.CashBookPaymentRep(fromdate, todate, BankId);

            if (Payment.Count > 0)
            {
                pay = 1;
            }
            else { pay = 2; }

            if (Receipt.Count > 0)
            {
                rec = 1;
            }
            else { rec = 2; }
            data = new { rec = rec, pay = pay };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Posting
        public ActionResult PostingReport()
        {
            ViewBag.TransactionType = GetTransactionType();
            return View();
        }
        public ActionResult Posting(DateTime fromdate, DateTime todate, string transactiontype, int format)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Postings.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var data = ReportService.PostingsRep(fromdate, todate, transactiontype);
                rd.SetDataSource(data);
                rd.SetParameterValue("fromdate", fromdate);
                rd.SetParameterValue("todate", todate);
                if (transactiontype != null)
                {
                    rd.SetParameterValue("transactiontype", transactiontype);
                }
                Stream stream;
                if (format == 1)
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=PostingReport.pdf");
                    return File(stream, "application/pdf");
                }
                else
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=PostingReport.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetDataForPostingReport(DateTime fromdate, DateTime todate, string transactiontype)
        {
            var em = 0;
            var data = new object();
            List<PostingsModel> boa = new List<PostingsModel>();
            todate = todate.AddDays(1).AddTicks(-1);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_Postings
                           where ((b.PostedDate >= fromdate && b.PostedDate <= todate) && (String.IsNullOrEmpty(transactiontype) ||
                           b.TransType.Contains(transactiontype)) && !String.IsNullOrEmpty(b.TransactionType))
                           orderby b.PostedDate descending
                           select new
                           {
                               b.PostedDate,
                               b.AccountHead,
                               b.Accounts,
                               b.Creditor_f,
                               b.Debtor_f,
                               b.Groups,
                               b.TransactionType,
                               b.Amount,
                               b.TransType

                           }).ToList();

                if (Qry.Count > 0)
                {
                    em = 1;
                }
                else { em = 2; }

            }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Commitment
        public ActionResult CommitmentReport()
        {
            ViewBag.ProjectNumber = GetProjectNumber();
            return View();
        }
        public ActionResult Commitment(DateTime fromdate, DateTime todate, int projecttype, int format, int projectnumber = 0)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                bool appendPIName = true;
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "CommitmentReport.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection("10.18.0.11,1433", conn, "sa", "IcsR@123#");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var data = ReportService.CommitmentRep(fromdate, todate, projecttype, projectnumber);
                rd.SetDataSource(data);
                rd.SetParameterValue("fromdate", fromdate);
                rd.SetParameterValue("todate", todate);
                rd.SetParameterValue("projecttype", Common.getprojectTypeName(projecttype));
                if (projectnumber != 0)
                {
                    rd.SetParameterValue("projectnumber", Common.GetProjectNumber(projectnumber, appendPIName));
                }
                else
                {
                    rd.SetParameterValue("projectnumber", "");

                }
                Stream stream;
                if (format == 1)
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CommitmentReport.pdf");
                    return File(stream, "application/pdf");
                }
                else
                {
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CommitmentReport.xls");
                    return File(stream, "application/vnd.ms-excel");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult GetDataForCommitmentReport(DateTime fromdate, DateTime todate, int projecttype, int projectnumber = 0)
        {
            var em = 0;
            var data = new object();
            List<CommitmentReportModel> com = new List<CommitmentReportModel>();
            todate = todate.AddDays(1).AddTicks(-1);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_CommitmentReport
                           where (((b.CommitmentDate >= fromdate && b.CommitmentDate <= todate)) &&
                           (b.ProjectType == projecttype) && (projectnumber == 0 || b.ProjectId == projectnumber))
                           orderby b.CommitmentDate descending
                           select new
                           {
                               b.ProjectNumber,
                               b.CommitmentNumber,
                               b.ProjectType,
                               b.ProjectTypeName,
                               b.CommitmentType,
                               b.CommitmentDate,
                               b.CommitmentAmount,
                               b.BookedValue,
                           }).ToList();

                if (Qry.Count > 0)
                {
                    em = 1;
                }
                else { em = 2; }

            }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadProjectNo(string ProjType)
        {
            ProjType = ProjType == "" ? "0" : ProjType;
            var locationdata = ProjectService.LoadProjecttitledetails(Convert.ToInt32(ProjType));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //public ActionResult CashBookReport()
        //{
        //    return View();
        //}
        //public ActionResult CashBook(DateTime fromdate,DateTime todate)
        //{
        //    ReportDocument rd = new ReportDocument();
        //    try
        //    {
        //        string conn = "IOASDBTH";
        //        rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), ".rpt"));
        //        for (int i = 0; i < rd.DataSourceConnections.Count; i++)
        //            rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
        //        Response.Buffer = false;
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        DataSet ds = new DataSet();
        //        var data1 = ReportService.CashBookRep(fromdate, todate);
        //        ds.Tables.Add(data1);
        //        rd.SetDataSource(ds);
        //        rd.SetParameterValue("fromdate", fromdate);
        //        rd.SetParameterValue("todate", todate);                                                        
        //        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        Response.AddHeader("Content-Disposition", "inline; filename=NEW.pdf");
        //        return File(stream, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public static List<ProjectNumModel> GetProjectNumber()
        {
            List<ProjectNumModel> projnum = new List<ProjectNumModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblProject
                             orderby C.ProjectNumber
                             select new { C.ProjectNumber }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        projnum.Add(new ProjectNumModel()
                        {
                            ProjectNumber = query[i].ProjectNumber
                        });
                    }
                }
            }
            return projnum;
        }
        public static List<TransactionTypeModel> GetTransactionType()
        {
            List<TransactionTypeModel> transtype = new List<TransactionTypeModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblTransactionTypeCode
                             orderby C.TransactionType
                             select new { C.TransactionType }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        transtype.Add(new TransactionTypeModel()
                        {
                            TransactionType = query[i].TransactionType
                        });
                    }
                }
            }
            return transtype;
        }
        public static List<AccountTypeModel> GetFinancialYear()
        {
            List<AccountTypeModel> acctype = new List<AccountTypeModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblFinYear
                             orderby C.FinYearId
                             select new { C }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        acctype.Add(new AccountTypeModel()
                        {
                            FinancialYear = query[i].C.Year,
                            FinancialId = query[i].C.FinYearId
                        });
                    }
                }
            }
            return acctype;
        }
        public ActionResult DailyBalanceVerification()
        {
            ViewBag.ProjectType = Common.getprojecttype();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetDailyBalSummary(int projectid, DateTime SerDate)
        {
            var locationdata = ReportService.GetDailyBalanceVerfication(projectid, SerDate);
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult BOATransactionDetailsReport()
        {
            ViewBag.ProjectNumber = GetProjectNumber();
            ViewBag.TransactionType = GetTransactionType();
            return View();
        }
        public ActionResult BOATransactionDetails(DateTime fromdate, DateTime todate, string projectnumber, string transactiontype)
        {
            ReportDocument rd = new ReportDocument();
            try
            {
                string conn = "IOASDB";
                rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "BOATransactionDetails.rpt"));
                for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                    rd.DataSourceConnections[i].SetConnection(".", conn, "sa", "Welc0me");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var data = ReportService.BOATransactionDetailsRep(fromdate, todate,
                    projectnumber, transactiontype);
                rd.SetDataSource(data);
                rd.SetParameterValue("fromdate", fromdate);
                rd.SetParameterValue("todate", todate);
                if (transactiontype != null)
                {
                    rd.SetParameterValue("transactiontype", transactiontype);
                }
                if (transactiontype != null)
                {

                    rd.SetParameterValue("projectnumber", projectnumber);
                }
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                Response.AddHeader("Content-Disposition", "inline; filename=NEW.pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region ProjectTransaction
        public ActionResult ProjectTransactionReport()
        {
            return View();
        }
        public ActionResult ProjectTransaction(int ProjId)
        {
            bool appendPIName = true;
            var username = User.Identity.Name;
            ReportDocument rd = new ReportDocument();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    string Copi = "";
                    var qry = (from c in context.tblNegativeBalance
                               where c.ProjectId == ProjId
                               select new
                               { c }).ToList();
                    decimal AvailableNB = qry.Select(m => m.c.NegativeBalanceAmount).Sum() ?? 0;
                    var query = (from a in context.tblProject
                                 join b in context.tblProjectCoPI on a.ProjectId equals b.ProjectId
                                 join c in context.vwFacultyStaffDetails on b.Name equals c.UserId
                                 where b.ProjectId == ProjId
                                 select new { b.ProjectId, a.ProjectNumber, c.FirstName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Copi += i + 1 + "." + query[i].FirstName + " , ";
                        }
                    }
                    string conn = "IOASDB";
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "ProjectTransactionReport.rpt"));
                    for (int i = 0; i < rd.DataSourceConnections.Count; i++)
                        rd.DataSourceConnections[i].SetConnection("10.18.0.11,1433", conn, "sa", "IcsR@123#");
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var data = ReportService.ProjectTransaction(ProjId);
                    var projectdetails = Common.GetProjectsDetails(Convert.ToInt32(ProjId));
                    rd.SetDataSource(data);
                    //rd.SetParameterValue("ProjectNo", Common.GetProjectNumber(ProjId, appendPIName));
                    rd.SetParameterValue("PI", Common.GetProjectNumber(ProjId, appendPIName));
                    rd.SetParameterValue("Copi", Copi);
                    if (username != null)
                    {
                        rd.SetParameterValue("username", username);
                    }
                    rd.SetParameterValue("Title", projectdetails.ProjectTittle);
                    rd.SetParameterValue("SanValue", Convert.ToString(projectdetails.SancationValue));
                    rd.SetParameterValue("SanDate", projectdetails.SancationDate);
                    rd.SetParameterValue("AvailableNB", AvailableNB);
                    Stream stream;
                    stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    Response.AddHeader("Content-Disposition", "inline; filename=CommitmentReport.pdf");
                    return File(stream, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetDataForProjectTransaction(int ProjId)
        {
            var em = 0;
            var data = new object();
            var Qry = ReportService.ProjectTransaction(ProjId);
            if (Qry.Count > 0)
            {
                em = 1;
            }
            else { em = 2; }
            data = new { em = em };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}