using DataAccessLayer;
using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Data;
using System.Linq;

namespace IOAS.GenericServices
{
    public class ProcessClarifyService
    {
        CoreAccountsService coreAccountService = new CoreAccountsService();
        public bool TADWFInitClarify(int travelBillId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TAD");
                    if (query != null)
                    {
                        query.Status = "Open";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool BillWFInitClarify(int billId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.Status == "Submit for approval");
                    if (query != null)
                    {
                        var status = coreAccountService.BillCommitmentBalanceUpdate(billId, true, false, loggedInUser, query.TransactionTypeCode);
                        if (!status)
                            return false;
                        query.Status = "Open";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region patent
        public bool IDFInitClarify(int FileNo,int loggedInUser)
        {
            try
            {
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_IDFRequest.Where(m => m.FileNo == FileNo).ToList();
                    if (query.Count > 0)
                    {
                        ProcessEngine db = new ProcessEngine();
                        DataSet dsTransaction = db.GetProcessFlowByUser(203, loggedInUser, FileNo);
                        DataTable dtHistory = dsTransaction.Tables[2];
                        var history = Converter.GetEntityList<ProcessEngineModel>(dtHistory);
                        string comment = history.OrderByDescending(m => m.ProcessTransactionDetailId).Select(m=>m.Comments).FirstOrDefault();
                        foreach (var item in query)
                        {
                            if(item.Status== "Recommended by IPAdmin")
                            {
                                item.Status = "Clarification needed";
                                item.Remarks = comment;
                                item.ModifiedBy = loggedInUser.ToString();
                                item.ModifiedOn = DateTime.Now;
                            }                            
                        }                    
                        var maindb = context.tblIDFRequest.FirstOrDefault(m => m.FileNo == FileNo);
                        if(maindb!=null)
                        {
                            maindb.Status= "Clarification needed";
                            maindb.Remarks = comment;
                            maindb.ModifiedBy = loggedInUser.ToString();
                            maindb.ModifiedOn = DateTime.Now;
                        }
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        public bool TSTWFInitClarify(int travelBillId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TST");
                    if (query != null)
                    {
                        var status = coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, loggedInUser, "TST");
                        if (!status)
                            return false;
                        query.Status = "Open";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DTVWFInitClarify(int travelBillId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "DTV");
                    if (query != null)
                    {
                        if (!coreAccountService.TravelCommitmentBalanceUpdate(travelBillId, true, false, loggedInUser, "DTV"))
                            return false;
                        query.Status = "Open";
                        query.UPTD_By = loggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SalaryInitClarify(int PaymentHeadId, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSalaryPaymentHead.FirstOrDefault(m => m.PaymentHeadId == PaymentHeadId && m.Status == "Approval Pending");
                    if (query != null)
                    {
                        query.Status = "Open";
                        query.UpdatedBy = userId;
                        query.UpdatedAt = DateTime.Now;
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}