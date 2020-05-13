using IOAS.DataModel;
using System;
using System.Linq;

namespace IOAS.GenericServices
{
    public class ProcessFailureService
    {
        CoreAccountsService coreAccountService = new CoreAccountsService();
        public bool TADWFInitFailure(int travelBillId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TAD");
                    if (query != null)
                    {
                        query.Status = "Rejected";
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

        public bool BillWFInitFailure(int billId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.Status == "Submit for approval");
                    if (query != null)
                    {
                        var status = coreAccountService.BillCommitmentBalanceUpdate(billId, true, false, loggedInUser,query.TransactionTypeCode);
                        if (!status)
                            return false;
                        query.Status = "Rejected";
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

        public bool TSTWFInitFailure(int travelBillId, int loggedInUser)
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
                        query.Status = "Rejected";
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
        public bool DTVWFInitFailure(int travelBillId, int loggedInUser)
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
                        query.Status = "Rejected";
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
        public bool SalaryInitFailure(int PaymentHeadId, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSalaryPaymentHead.FirstOrDefault(m => m.PaymentHeadId == PaymentHeadId && m.Status == "Approval Pending");
                    if (query != null)
                    {
                        query.Status = "Rejected";
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
        #region patent
        public bool IDFInitFailure(int FileNo, int loggedInUser)
        {
            try
            {
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_IDFRequest.FirstOrDefault(m => m.FileNo == FileNo && m.Status == "Recommended by IPAdmin");
                    if (query != null)
                    {                        
                        query.Status = "Rejected";
                        query.ModifiedBy = loggedInUser.ToString();
                        query.ModifiedOn = DateTime.Now;
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
    }
}