using IOAS.DataModel;
using System;
using System.Linq;

namespace IOAS.GenericServices
{
    public class ProcessSuccessService
    {
        CoreAccountsService coreAccountService = new CoreAccountsService();
        public bool TADWFInitSuccess(int travelBillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TAD");
                    if (query != null)
                    {
                        query.Status = "Pending Bill Entry";// "Pending Commitment";
                        query.UPTD_By = logged_in_user;
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
        public bool TSTWFInitSuccess(int travelBillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "TST");
                    if (query != null)
                    {
                        if (query.BalanceinAdvance == 0 && query.PaymentValue == 0)
                        {
                            if (!coreAccountService.BalancedTSTBOATransaction(travelBillId))
                                return false;
                        }
                        query.Status = "Completed";// "Pending Commitment";
                        query.UPTD_By = logged_in_user;
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
        public bool DTVWFInitSuccess(int travelBillId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == travelBillId && m.Status == "Submit for approval" && m.TransactionTypeCode == "DTV");
                    if (query != null)
                    {
                        query.Status = "Completed";// "Pending Commitment";
                        query.UPTD_By = logged_in_user;
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
        public bool BillWFInitSuccess(int billId, int loggedInUser)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.Status == "Submit for approval");
                    if (query != null)
                    {
                        if (query.TransactionTypeCode != "ADV")
                        {
                            if (!coreAccountService.BillBackEndEntry(billId, loggedInUser))
                                return false;
                            else if (!coreAccountService.BillBOATransaction(billId))
                                return false;
                        }

                        query.Status = "Completed";
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
        public bool SalaryInitSuccess(int PaymentHeadId, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSalaryPaymentHead.FirstOrDefault(m => m.PaymentHeadId == PaymentHeadId && m.Status == "Approval Pending");
                    if (query != null)
                    {
                        query.Status = "Completed";
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
        public bool IDFInitSuccess(int FileNo, int loggedInUser)
        {
            try
            {
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_IDFRequest.Where(m => m.FileNo == FileNo).ToList();
                    foreach (var item in query)
                    {
                        if (item.Status == "Recommended by IPAdmin")
                        {
                            item.Status = "Dean Approved";
                            item.ModifiedBy = loggedInUser.ToString();
                            item.ModifiedOn = DateTime.Now;
                        }
                        else
                        {
                            item.Status = "Another version is already approved";
                        }
                    }
                    var q = context.tblIDFRequest.FirstOrDefault(m => m.FileNo == FileNo);
                    if (q != null)
                    {
                        q.Status = "Dean Approved";
                    }
                    else
                    {
                        return false;
                    }
                    context.SaveChanges();
                    return true;
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