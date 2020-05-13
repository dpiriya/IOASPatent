using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class ReportService
    {
        public static List<TrailBalanceModel> TrailBalanceRep2(int Finyear = 0)
        {
            List<TrailBalanceModel> boa = new List<TrailBalanceModel>();
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_DemoLedgers
                           where Finyear == 0 || b.FinancialYear == Finyear
                           select new
                           {
                               b.AccountGroupId,
                               b.AccountHead,
                               b.AccountHeadId,
                               b.Accounts,
                               b.Amount,
                               b.Creditor_f,
                               b.Debtor_f,
                               b.Groups,
                               b.TransactionType
                           }).ToList();
                //                var AssetCr = Qry.Where(m=>m.TransactionType=="Credit"&&m.Accounts== "Asset")
                //    .GroupBy(a => a.AccountHeadId)
                //    .Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //    .OrderByDescending(a => a.Amount)
                //    .ToList();

                //                var AssetDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Asset")
                // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                // .OrderByDescending(a => a.Amount)
                // .ToList();
                //                decimal ttlAssetDr = AssetDr.Sum(m => m.Amount) ?? 0;
                //                decimal ttlAssetCr = AssetCr.Sum(m => m.Amount) ?? 0;
                //                var LiabilityCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Liability")
                // .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                // .OrderByDescending(a => a.Amount)
                // .ToList();

                //                var LiabilityDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Liability")
                //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //.OrderByDescending(a => a.Amount)
                //.ToList();
                //                decimal ttlLiabilityDr = LiabilityDr.Sum(m => m.Amount) ?? 0 ;
                //                decimal ttlLiabilityCr = LiabilityCr.Sum(m => m.Amount) ?? 0;
                //                var IncomeCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Income")
                //             .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //             .OrderByDescending(a => a.Amount)
                //             .ToList();
                //                var IncomeDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Income")
                //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //.OrderByDescending(a => a.Amount)
                //.ToList();
                //                decimal ttlIncomeDr = IncomeDr.Sum(m => m.Amount) ?? 0 ;
                //                decimal ttlIncomeCr = IncomeCr.Sum(m => m.Amount) ?? 0;
                //                var ExpenseCr = Qry.Where(m => m.TransactionType == "Credit" && m.Accounts == "Expense")
                //           .GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //           .OrderByDescending(a => a.Amount)
                //           .ToList();

                //                var ExpenseDr = Qry.Where(m => m.TransactionType == "Debit" && m.Accounts == "Expense")
                //.GroupBy(a => a.AccountHeadId).Select(a => new { Amount = a.Sum(b => b.Amount), Name = a.Key })
                //.OrderByDescending(a => a.Amount)
                //.ToList();
                //                decimal ttlExpenseDr = ExpenseDr.Sum(m => m.Amount) ?? 0 ;
                //                decimal ttlExpenseCr = ExpenseCr.Sum(m => m.Amount) ?? 0;
                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new TrailBalanceModel()
                        {
                            Accounts = Qry[i].Accounts,
                            AccountGroupId = Convert.ToInt32(Qry[i].AccountGroupId),
                            AccountHeadId = Convert.ToInt32(Qry[i].AccountHeadId),
                            AccountHead = Qry[i].AccountHead,
                            TransactionType = Qry[i].TransactionType,
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            Creditor_f = Convert.ToBoolean(Qry[i].Creditor_f),
                            Debtor_f = Convert.ToBoolean(Qry[i].Debtor_f),
                            Groups = Qry[i].Groups
                        });
                    }
                }

            }
            return boa;
        }
        #region CashBook
        public static List<CashBookModel> CashBookPaymentRep(DateTime fromdate, DateTime todate, int BankId)
        {
            List<CashBookModel> boa = new List<CashBookModel>();
            todate = todate.AddDays(1).AddTicks(-1);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from c in context.vw_CashBookPayment
                           where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && c.BankHeadID == BankId
                           select new
                           {
                               c.Amount,
                               c.BankHeadID,
                               c.BOAId,
                               c.BOAPaymentDetailId,
                               c.PayeeBank,
                               c.PayeeName,
                               c.ReferenceDate,
                               c.TransactionType,
                               c.VoucherNumber,

                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new CashBookModel()
                        {
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            BankHeadID = Convert.ToInt32(Qry[i].BankHeadID),
                            BOAId = Convert.ToInt32(Qry[i].BOAId),
                            BOAPaymentDetailId = Convert.ToInt32(Qry[i].BOAPaymentDetailId),
                            TransactionType = Qry[i].TransactionType,
                            PayeeBank = Qry[i].PayeeBank,
                            PayeeName = Qry[i].PayeeName,
                            ReferenceDate = Convert.ToDateTime(Qry[i].ReferenceDate),
                            VoucherNumber = Qry[i].VoucherNumber,
                            VoucherPayee = Qry[i].VoucherNumber + "--" + Qry[i].PayeeName
                        });
                    }
                }

            }
            return boa;
        }
        public static List<CashBookModel> CashBookReceiptRep(DateTime fromdate, DateTime todate, int BankId)
        {
            List<CashBookModel> boa = new List<CashBookModel>();
            todate = todate.AddDays(1).AddTicks(-1);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from c in context.vw_CashBookReceipt
                           where (c.ReferenceDate >= fromdate && c.ReferenceDate <= todate) && c.BankHeadID == BankId
                           select new
                           {
                               c.Amount,
                               c.BankHeadID,
                               c.BOAId,
                               c.BOAPaymentDetailId,
                               c.PayeeBank,
                               c.PayeeName,
                               c.ReferenceDate,
                               c.TransactionType,
                               c.VoucherNumber,
                               c.VoucherPayee
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new CashBookModel()
                        {
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            BankHeadID = Convert.ToInt32(Qry[i].BankHeadID),
                            BOAId = Convert.ToInt32(Qry[i].BOAId),
                            BOAPaymentDetailId = Convert.ToInt32(Qry[i].BOAPaymentDetailId),
                            TransactionType = Qry[i].TransactionType,
                            PayeeBank = Qry[i].PayeeBank,
                            PayeeName = Qry[i].PayeeName,
                            ReferenceDate = Convert.ToDateTime(Qry[i].ReferenceDate),
                            VoucherNumber = Qry[i].VoucherNumber,
                            VoucherPayee = Qry[i].VoucherNumber + "--" + Qry[i].PayeeName
                        });
                    }
                }

            }
            return boa;
        }
        #endregion
        public static List<PostingsModel> PostingsRep(DateTime fromdate, DateTime todate, string transactiontype)
        {

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
                               b.TransType,
                               b.TempVoucherNumber

                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new PostingsModel()
                        {
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            PostedDate = Convert.ToDateTime(Qry[i].PostedDate),
                            Groups = Qry[i].Groups,
                            AccountHead = Qry[i].AccountHead,
                            TransactionType = Qry[i].TransactionType,
                            TransType = Qry[i].TransType,
                            Accounts = Qry[i].Accounts,
                            Creditor_f = Convert.ToBoolean(Qry[i].Creditor_f),
                            Debtor_f = Convert.ToBoolean(Qry[i].Debtor_f),
                            TempVoucherNumber = Qry[i].TempVoucherNumber
                        });
                    }
                }

            }
            return boa;
        }
        public static List<CommitmentReportModel> CommitmentRep(DateTime fromdate, DateTime todate, int projecttype, int projectnumber)
        {
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
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        com.Add(new CommitmentReportModel()
                        {
                            CommitmentAmount = Convert.ToDecimal(Qry[i].CommitmentAmount),
                            CommitmentDate = Convert.ToDateTime(Qry[i].CommitmentDate),
                            ProjectNumber = Qry[i].ProjectNumber,
                            CommitmentNumber = Qry[i].CommitmentNumber,
                            ProjectTypeName = Qry[i].ProjectTypeName,
                            CommitmentType = Qry[i].CommitmentType,
                            BookedValue = Convert.ToDecimal(Qry[i].BookedValue)
                        });
                    }
                }

            }
            return com;
        }
        public static DailyBalanceVerificationModel GetDailyBalanceVerfication(int projectid, DateTime Date)
        {
            try
            {

                DailyBalanceVerificationModel Dailmodel = new DailyBalanceVerificationModel();
                using (var context = new IOASDBEntities())
                {
                    ProjectService proser = new ProjectService();
                    var Service = proser.getProjectSummary(projectid);
                    var Ser = proser.getProjectSummaryForDailyBalance(projectid, Date);
                    var qryProject = (from prj in context.tblProject                                      where (prj.ProjectId == projectid)                                      select prj).FirstOrDefault();
                    var qryPreviousCommit = (from C in context.tblCommitment                                             join D in context.tblCommitmentDetails on C.CommitmentId equals D.CommitmentId                                             where (C.ProjectId == projectid && C.Status == "Active") && ((C.CRTD_TS == Date))                                             select new { D.BalanceAmount, D.ReversedAmount }).FirstOrDefault();
                    decimal? BalanceAmt;
                    decimal? ReversedAmount;
                    if (qryPreviousCommit == null)
                    {
                        BalanceAmt = 0;
                        ReversedAmount = 0;
                    }
                    else
                    {
                        BalanceAmt = qryPreviousCommit.BalanceAmount;
                        ReversedAmount = qryPreviousCommit.ReversedAmount;
                    }
                    decimal? Debit = 0, Credit = 0, spentAmt = 0;                    var qrySpenAmt = (from C in context.vwCommitmentSpentBalance where (C.ProjectId == projectid) && ((C.CRTD_TS == Date)) select C.AmountSpent).Sum();                    if (qrySpenAmt == null)                        qrySpenAmt = 0;                    spentAmt = qrySpenAmt;
                    var FundTransferCredit = (from C in context.tblProjectTransfer                                              from D in context.tblProjectTransferDetails                                              where C.ProjectTransferId == D.ProjectTransferId                                              where (C.CreditProjectId == projectid) && ((C.CRTD_TS == Date))                                              select D).FirstOrDefault();                    if (FundTransferCredit == null)
                    {
                        Credit = 0;

                        spentAmt = spentAmt - Debit;
                    }                    else
                    {
                        Credit = FundTransferCredit.Amount;                        if (Credit != 0)                            spentAmt = spentAmt - Debit;
                    }

                    /*claim amount institute start*/
                    var qryInstituteClaim = (from Neg in context.tblInstituteClaims                                             where Neg.ProjectId == projectid && Neg.Status == "Completed" && ((Neg.CRTD_TS == Date))                                             select Neg).FirstOrDefault();                    decimal? claimAmt = 0;                    if (qryInstituteClaim != null)                        claimAmt = qryInstituteClaim.ClaimAmount;                    spentAmt = spentAmt - claimAmt;
                    /*claim amount institute end*/
                    var AvailableCommitment = BalanceAmt + ReversedAmount;
                    var QryReceipt = (from C in context.tblReceipt where ((C.ProjectId == projectid) && ((C.CrtdTS == Date))) select C).FirstOrDefault();
                    decimal? receiptAmt;
                    decimal? OverHead;
                    decimal? CGST;
                    decimal? SGST;
                    decimal? IGST;
                    decimal? GST;
                    if (QryReceipt == null)
                    {
                        receiptAmt = 0;
                        OverHead = 0;
                        CGST = 0;
                        SGST = 0;
                        IGST = 0;
                        GST = CGST + SGST + IGST;
                    }
                    else
                    {
                        receiptAmt = QryReceipt.ReceiptAmount;
                        OverHead = QryReceipt.ReceiptOverheadValue;
                        CGST = QryReceipt.CGST;
                        SGST = QryReceipt.SGST;
                        IGST = QryReceipt.IGST;
                        GST = CGST + SGST + IGST;
                    }
                    /* Negative balance taking query*/
                    var qryNegativeBal = (from Neg in context.tblNegativeBalance                                          where (Neg.ProjectId == projectid && Neg.Status == "Approved") && ((Neg.CRTD_TS == Date))                                          select Neg.NegativeBalanceAmount).Sum();
                    /* Opening balance taking query*/
                    decimal qryOpeningBal = (from OB in context.tblProjectOB                                             where OB.ProjectId == projectid && ((OB.Crt_TS == Date))                                             select OB.OpeningBalance).Sum() ?? 0;
                    /* Opening balance taking query end*/
                    Dailmodel.ProjectNo = qryProject.ProjectNumber;
                    Dailmodel.PI = Common.GetPIName(qryProject.PIName ?? 0);
                    Dailmodel.OB = qryOpeningBal;
                    //sum(ReciptAmt-(GST+OverHeads ))                 
                    Dailmodel.Receipt = Convert.ToDecimal(receiptAmt);
                    Dailmodel.AmountSpent = spentAmt ?? 0;
                    Dailmodel.PreviousCommitment = AvailableCommitment ?? 0;
                    Dailmodel.TypeName = Common.getprojectTypeName(qryProject.ProjectType ?? 0);
                    Dailmodel.AvailBalance = (Dailmodel.TotalReceipt - (Dailmodel.AmountSpent + Dailmodel.PreviousCommitment));
                    Dailmodel.ApprovedNegativeBalance = qryNegativeBal ?? 0;
                    Dailmodel.NetBalance = (Dailmodel.AvailBalance + Dailmodel.ApprovedNegativeBalance);
                    Dailmodel.OpeningBalance = ((Ser.TotalReceipt + Ser.ApprovedNegativeBalance) - (Ser.PreviousCommitment + Ser.AmountSpent));
                    Dailmodel.TotalReceipt = Service.TotalReceipt;
                    Dailmodel.TotalSanction = Service.SanctionedValue;
                    Dailmodel.TotalNegativeBalance = Service.ApprovedNegativeBalance;
                    Dailmodel.TotalExpent = Service.AmountSpent;
                    Dailmodel.TotalCommitment = Service.PreviousCommitment;
                    Dailmodel.TotalAvailBalance = Service.AvailableBalance;
                    Dailmodel.ClosingBalance = ((Dailmodel.OpeningBalance - (Dailmodel.PreviousCommitment + Dailmodel.AmountSpent) + (Dailmodel.Receipt + Dailmodel.ApprovedNegativeBalance)));
                }
                return Dailmodel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<TapalReportViewModel> Gettapaltansaction(TapalReportViewModel model)
        {
            try
            {
                List<TapalReportViewModel> tpltranslist = new List<TapalReportViewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from TW in context.tblTapalWorkflow
                                 from T in context.tblTapal
                                 from U in context.tblUser
                                 from C in context.tblCodeControl
                                 join DP in context.tblDepartment on TW.MarkTo equals DP.DepartmentId into D
                                 from c in D.DefaultIfEmpty()
                                 join R in context.tblRole on TW.Role equals R.RoleId into RR
                                 from S in RR.DefaultIfEmpty()
                                 where TW.InwardDateTime >= model.fromdate && TW.InwardDateTime <= model.todate
                                       && (TW.MarkTo == model.departmentid || model.departmentid == 0)
                                       && (TW.Role == model.roleid || model.roleid == 0)
                                       && (TW.UserId == model.id || model.id == 0)
                                       && TW.UserId == U.UserId
                                       && TW.TapalId == T.TapalId && C.CodeName == "TapalAction" && C.CodeValAbbr == TW.TapalAction
                                 select new { TW.InwardDateTime, TW.OutwardDateTime, c.DepartmentName, S.RoleName, U.FirstName, U.LastName, C.CodeValDetail, T.TapalId }).Distinct().ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            var deptname = "";
                            var role = "";
                            var outwarddate="";
                            if (query[i].OutwardDateTime == null)
                            {
                                outwarddate = "-";
                            }
                            else
                            {
                                outwarddate = String.Format("{0: dd/MM/yyyy   h:mm:ss tt}", query[i].OutwardDateTime);
                            }
                            if (query[i].DepartmentName==null)
                            {
                                deptname = "NA";
                            }
                            else
                            {
                                deptname = query[i].DepartmentName;
                            }
                            if (query[i].RoleName == null)
                            {
                                role = "NA";
                            }
                            else
                            {
                                role = query[i].RoleName;
                            }
                            tpltranslist.Add(new TapalReportViewModel()
                            {
                                InwardDateTime = (DateTime)query[i].InwardDateTime,
                                TapalAction = query[i].CodeValDetail,
                                OutwardDateTime = outwarddate,
                                MarkTo = deptname,
                                Role =role,
                                UserId = query[i].FirstName + ' ' + query[i].LastName,
                                TapalId = query[i].TapalId
                            });
                        }
                    }
                }
                return tpltranslist;
            }
            catch(Exception ex)
            {
                List<TapalReportViewModel> tpltranslist = new List<TapalReportViewModel>();
                return tpltranslist;
            }
        }
        public static List<BOATransactionDetailsModels> BOATransactionDetailsRep(DateTime fromdate, DateTime todate, string projectnumber, string transactiontype)
        {

            List<BOATransactionDetailsModels> boa = new List<BOATransactionDetailsModels>();
            todate = todate.AddDays(1).AddTicks(-1);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_BOATransactionDetails
                           where ((b.PostedDate >= fromdate && b.PostedDate <= todate) && (String.IsNullOrEmpty(transactiontype) || b.TransactionType.Contains(transactiontype)) && (String.IsNullOrEmpty(b.ProjectNumber) || b.ProjectNumber.Contains(projectnumber)))
                           select new
                           {
                               b.PostedDate,
                               b.CommitmentNumber,
                               b.ProjectNumber,
                               b.HeadName,
                               b.TransactionType,
                               b.Amount
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new BOATransactionDetailsModels()
                        {
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            PostedDate = Convert.ToDateTime(Qry[i].PostedDate),
                            CommitmentNumber = Qry[i].CommitmentNumber,
                            ProjectNumber = Qry[i].ProjectNumber,
                            HeadName = Qry[i].HeadName,
                            TransactionType = Qry[i].TransactionType
                        });
                    }
                }

            }
            return boa;
        }
        //public static List<CashBookModel> CashBookRep(DateTime fromdate ,DateTime todate)
        //{
        //    List <CashBookModel> cash= new List<CashBookModel>();

        //    return cash;
        //}
        public static List<TrailBalanceModel> TrailBalanceRep(string accounts)
        {
            List<TrailBalanceModel> boa = new List<TrailBalanceModel>();
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_DemoLedgers
                           where b.Accounts == accounts
                           select new
                           {
                               b.AccountGroupId,
                               b.AccountHead,
                               b.AccountHeadId,
                               b.Accounts,
                               b.Amount,
                               b.Creditor_f,
                               b.Debtor_f,
                               b.Groups
                           }).ToList();

                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new TrailBalanceModel()
                        {
                            Accounts = Qry[i].Accounts,
                            AccountGroupId = Convert.ToInt32(Qry[i].AccountGroupId),
                            AccountHeadId = Convert.ToInt32(Qry[i].AccountHeadId),
                            AccountHead = Qry[i].AccountHead,
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            Creditor_f = Convert.ToBoolean(Qry[i].Creditor_f),
                            Debtor_f = Convert.ToBoolean(Qry[i].Debtor_f),
                            Groups = Qry[i].Groups
                        });
                    }
                }

            }
            return boa;
        }
        public static List<CommitmentReportModel> CommitmentRep(DateTime fromdate, DateTime todate, string projecttype, string projectnumber)
        {
            List<CommitmentReportModel> com = new List<CommitmentReportModel>();
            todate = todate.AddDays(1).AddTicks(-1);
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_CommitmentReport
                           where (((b.CommitmentDate >= fromdate && b.CommitmentDate <= todate)) &&
                           (b.ProjectTypeName == projecttype) && (String.IsNullOrEmpty(projectnumber) || b.ProjectNumber.Contains(projectnumber)))
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
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        com.Add(new CommitmentReportModel()
                        {
                            CommitmentAmount = Convert.ToDecimal(Qry[i].CommitmentAmount),
                            CommitmentDate = Convert.ToDateTime(Qry[i].CommitmentDate),
                            ProjectNumber = Qry[i].ProjectNumber,
                            CommitmentNumber = Qry[i].CommitmentNumber,
                            ProjectTypeName = Qry[i].ProjectTypeName,
                            CommitmentType = Qry[i].CommitmentType,
                            BookedValue = Convert.ToDecimal(Qry[i].BookedValue)
                        });
                    }
                }

            }
            return com;
        }
        #region ProjectTransaction
        public static List<ProjectTransactionModel> ProjectTransaction(int ProjId)
        {
            List<ProjectTransactionModel> boa = new List<ProjectTransactionModel>();
            using (var context = new IOASDBEntities())
            {
                var Qry = (from b in context.vw_ProjectTransactionReport
                           where b.ProjectId == ProjId
                           orderby b.DateOfTransaction descending
                           select new
                           {
                               b.ProjectId,
                               b.Amount,
                               b.DateOfTransaction,
                               b.RefNo,
                               b.TransType,
                               b.Code,
                               b.FunctionName,
                               b.Category,
                               b.CommitmentNumber
                           }).ToList();
                if (Qry.Count > 0)
                {
                    for (int i = 0; i < Qry.Count; i++)
                    {
                        boa.Add(new ProjectTransactionModel()
                        {
                            ProjectId = Convert.ToInt32(Qry[i].ProjectId),
                            Amount = Convert.ToDecimal(Qry[i].Amount),
                            DateOfTransaction = Convert.ToDateTime(Qry[i].DateOfTransaction),
                            RefNo = Qry[i].RefNo,
                            TransType = Qry[i].TransType,
                            Code = Qry[i].Code,
                            FunctionName = Qry[i].FunctionName,
                            Category = Qry[i].Category,
                            CommitmentNumber = Qry[i].CommitmentNumber
                        });
                    }
                }

            }
            return boa;
        }
        #endregion
    }
}