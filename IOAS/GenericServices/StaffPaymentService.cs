using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using IOAS.Infrastructure;
using System.Data.Entity;

namespace IOAS.GenericServices
{
    public class StaffPaymentService
    {
        FinOp fo = new FinOp(System.DateTime.Now);
        CoreAccountsService coreAccounts = new CoreAccountsService();

        public List<PaymentTypeModel> GetPaymentType()
        {
            try
            {
                List<PaymentTypeModel> model = new List<PaymentTypeModel>();
                using (var context = new IOASDBEntities())
                {
                    var payType = (from PT in context.tblCodeControl
                                   where PT.CodeName == "SalaryPaymentType"
                                   select new
                                   {
                                       PT.CodeName,
                                       PT.CodeValAbbr,
                                       PT.CodeValDetail,
                                       PT.CodeID
                                   }).ToList();
                    if (payType.Count > 0)
                    {
                        for (int i = 0; i < payType.Count; i++)
                        {
                            model.Add(new PaymentTypeModel
                            {
                                PaymentType = payType[i].CodeValDetail,
                                PaymentTypeId = payType[i].CodeValAbbr
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<tblCodeControl> GetStatusFields(string CodeName)
        {
            try
            {
                List<tblCodeControl> model = new List<tblCodeControl>();
                using (var context = new IOASDBEntities())
                {
                    var payType = (from PT in context.tblCodeControl
                                   where PT.CodeName == CodeName
                                   select new
                                   {
                                       PT.CodeName,
                                       PT.CodeValAbbr,
                                       PT.CodeValDetail,
                                       PT.CodeID
                                   }).ToList();
                    if (payType.Count > 0)
                    {
                        for (int i = 0; i < payType.Count; i++)
                        {
                            model.Add(new tblCodeControl
                            {
                                CodeValDetail = payType[i].CodeValDetail,
                                CodeValAbbr = payType[i].CodeValAbbr
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public decimal GetITExemption(string EmpId)
        {
            try
            {
                decimal Total = 0;
                using (var context = new IOASDBEntities())
                {
                    //var TotalExemption = (from E in context.tblEmpITDeclaration
                    //                      where E.EmpNo == EmpId
                    //                      select new
                    //                      {
                    //                          E.EmpNo,
                    //                          E.Amount,
                    //                          E.MaxLimit
                    //                      }).Sum(i => i.Amount);

                    var EightyCTotal = (from E in context.tblEmpITDeclaration
                                        join IT in context.tblITDeclaration on E.DeclarationID equals IT.DeclarationID
                                        where E.EmpNo == EmpId && (IT.SectionCode == "80C" || IT.SectionCode == "80CCC")
                                        select new
                                        {
                                            E.EmpNo,
                                            E.Amount,
                                            E.MaxLimit
                                        }).Sum(i => i.Amount);


                    var NonEightyCTotal = (from E in context.tblEmpITDeclaration
                                           join IT in context.tblITDeclaration on E.DeclarationID equals IT.DeclarationID
                                           where E.EmpNo == EmpId && (IT.SectionCode != "80C" || IT.SectionCode != "80CCC")
                                           select new
                                           {
                                               E.EmpNo,
                                               E.Amount,
                                               E.MaxLimit
                                           }).Sum(i => i.Amount);
                    if (EightyCTotal > 150000)
                    {
                        Total = 150000 + Convert.ToDecimal(NonEightyCTotal);
                    }
                    else
                    {
                        Total = Convert.ToDecimal(EightyCTotal) + Convert.ToDecimal(NonEightyCTotal);
                    }

                    //Total = Convert.ToDouble(TotalExemption);

                }

                return Total;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }


        public string ITEmpDeclarationIU(EmpITDeductionModel EmpITmodel)
        {
            try
            {
                List<EmpITDeclarationModel> ItModel = new List<EmpITDeclarationModel>();
                ItModel = EmpITmodel.ItList;
                using (var context = new IOASDBEntities())
                {
                    tblEmpITDeclaration taxExemp = new tblEmpITDeclaration();

                    for (int i = 0; i < ItModel.Count; i++)
                    {
                        EmpITDeclarationModel model = new EmpITDeclarationModel();
                        model = ItModel[i];
                        model.EmpNo = EmpITmodel.EmpInfo.EmployeeID;
                        var SectionID = model.SectionID;
                        if (SectionID > 0)
                        {
                            var record = context.tblEmpITDeclaration
                                .SingleOrDefault(it => it.SectionID == SectionID && it.EmpNo == model.EmpNo);
                            record.EmpNo = model.EmpNo;
                            record.DeclarationID = model.DeclarationID;
                            record.SectionName = model.SectionName;
                            record.SectionCode = model.SectionCode;
                            record.Particulars = model.Particulars;
                            record.MaxLimit = model.MaxLimit;
                            record.Amount = model.Amount;
                            record.Age = model.Age;
                            record.UpdatedAt = System.DateTime.Now;
                            record.UpdatedBy = model.UpdatedBy;
                            context.SaveChanges();
                        }
                        else
                        {
                            taxExemp.EmpNo = model.EmpNo;
                            taxExemp.DeclarationID = model.DeclarationID;
                            taxExemp.SectionName = model.SectionName;
                            taxExemp.SectionCode = model.SectionCode;
                            taxExemp.Particulars = model.Particulars;
                            taxExemp.MaxLimit = model.MaxLimit;
                            taxExemp.Age = model.Age;
                            taxExemp.Amount = model.Amount;
                            taxExemp.EmpNo = model.EmpNo;
                            taxExemp.CreatedAt = System.DateTime.Now;
                            taxExemp.CreatedBy = model.CreatedBy;
                            context.tblEmpITDeclaration.Add(taxExemp);
                            context.SaveChanges();
                        }

                    }

                    context.Dispose();

                }
                string msg = "IT declaration updated successfully";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }


        public List<EmpITDeclarationModel> GetITEmpDeclarations(string EmpNo)
        {
            try
            {
                var model = new List<EmpITDeclarationModel>();
                var searchData = new PagedData<EmpITDeclarationModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from AI in context.tblITDeclaration
                                 join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                 from Emp in EmpDeclaration.DefaultIfEmpty()
                                 where Emp.EmpNo == EmpNo
                                 orderby AI.DeclarationID
                                 select new
                                 {
                                     AI.DeclarationID,
                                     AI.SNO,
                                     AI.SectionName,
                                     AI.SectionCode,
                                     AI.Particulars,
                                     AI.MaxLimit,
                                     AI.Age,
                                     AI.CreatedAt,
                                     AI.UpdatedAt,
                                     AI.CreatedBy,
                                     AI.UpdatedBy,
                                     SectionID = (Emp.SectionID == null ? 0 : Emp.SectionID),
                                     Emp.Amount
                                 });
                    var records = query.ToList();
                    if (records.Count == 0)
                    {
                        var qry = (from AI in context.tblITDeclaration
                                   orderby AI.DeclarationID
                                   select new
                                   {
                                       AI.DeclarationID,
                                       AI.SNO,
                                       AI.SectionName,
                                       AI.SectionCode,
                                       AI.Particulars,
                                       AI.MaxLimit,
                                       AI.Age,
                                       AI.CreatedAt,
                                       AI.UpdatedAt,
                                       AI.CreatedBy,
                                       AI.UpdatedBy
                                   });
                        var items = qry.ToList();
                        for (int i = 0; i < items.Count; i++)
                        {
                            model.Add(new EmpITDeclarationModel
                            {
                                SectionID = 0,
                                DeclarationID = items[i].DeclarationID,
                                SectionName = Convert.ToString(items[i].SectionName),
                                SectionCode = Convert.ToString(items[i].SectionCode),
                                Particulars = items[i].Particulars,
                                MaxLimit = Convert.ToDecimal(items[i].MaxLimit),
                                Age = Convert.ToInt32(items[i].Age),
                                Amount = Convert.ToDecimal(0)

                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }
                    }

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITDeclarationModel
                            {
                                SectionID = records[i].SectionID,
                                DeclarationID = records[i].DeclarationID,
                                SectionName = Convert.ToString(records[i].SectionName),
                                SectionCode = Convert.ToString(records[i].SectionCode),
                                Particulars = records[i].Particulars,
                                MaxLimit = Convert.ToDecimal(records[i].MaxLimit),
                                Age = Convert.ToInt32(records[i].Age),
                                Amount = Convert.ToDecimal(records[i].Amount)

                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }
                        //recordCount = records.Count;
                        //searchData.Data = model;
                        //searchData.TotalRecords = records.Count;
                        //searchData.pageSize = records.Count;
                        //searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / searchData.pageSize));
                    }
                }
                //return searchData;

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmpITSOPModel> GetITEmpSOP()
        {
            try
            {
                var model = new List<EmpITSOPModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from sop in context.tblEmpSOPIncome
                                     //join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                     //from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where sop.EmpNo == empNo
                                 orderby sop.EmpNo
                                 select new
                                 {
                                     sop.ID,
                                     sop.EmpNo,
                                     sop.LenderName,
                                     sop.LenderPAN,
                                     sop.Amount,
                                     sop.EligibleAmount,
                                     sop.SubmittedOn,
                                     sop.CreatedAt,
                                     sop.UpdatedAt,
                                     sop.CreatedBy,
                                     sop.UpdatedBy
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITSOPModel
                            {
                                ID = records[i].ID,
                                EmpNo = Convert.ToString(records[i].EmpNo),
                                LenderName = Convert.ToString(records[i].LenderName),
                                LenderPAN = records[i].LenderPAN,
                                Amount = Convert.ToDecimal(records[i].Amount),
                                EligibleAmount = Convert.ToDecimal(records[i].EligibleAmount),
                                SubmittedOn = Convert.ToDateTime(records[i].SubmittedOn)
                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmpITOtherIncomeModel> GetITEmpOtherIncome()
        {
            try
            {
                var model = new List<EmpITOtherIncomeModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from OI in context.tblEmpOtherIncome
                                     //join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                     //from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where sop.EmpNo == empNo
                                 orderby OI.EmpNo
                                 select new
                                 {
                                     OI.ID,
                                     OI.EmpNo,
                                     OI.Amount,
                                     OI.EligibleAmount,
                                     OI.SubmittedOn,
                                     OI.CreatedAt,
                                     OI.UpdatedAt,
                                     OI.CreatedBy,
                                     OI.UpdatedBy
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new EmpITOtherIncomeModel
                            {
                                ID = records[i].ID,
                                EmpNo = Convert.ToString(records[i].EmpNo),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                EligibleAmount = Convert.ToDecimal(records[i].EligibleAmount),
                                SubmittedOn = Convert.ToDateTime(records[i].SubmittedOn)
                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public string ITDeclarationIU(ITDeclarationModel model)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    tblITDeclaration taxExemp = new tblITDeclaration();
                    var DeclarationID = model.DeclarationID;
                    if (DeclarationID > 0)
                    {
                        var record = context.tblITDeclaration.SingleOrDefault(it => it.DeclarationID == DeclarationID);

                        record.SectionName = model.SectionName;
                        record.SectionCode = model.SectionCode;
                        record.Particulars = model.Particulars;
                        record.MaxLimit = model.MaxLimit;
                        record.Age = model.Age;
                        record.UpdatedAt = System.DateTime.Now;
                        record.UpdatedBy = model.UpdatedBy;
                    }
                    else
                    {
                        taxExemp.SectionName = model.SectionName;
                        taxExemp.SectionCode = model.SectionCode;
                        taxExemp.Particulars = model.Particulars;
                        taxExemp.MaxLimit = model.MaxLimit;
                        taxExemp.Age = model.Age;
                        taxExemp.CreatedAt = System.DateTime.Now;
                        taxExemp.CreatedBy = model.CreatedBy;
                        context.tblITDeclaration.Add(taxExemp);
                    }
                    context.SaveChanges();
                    context.Dispose();
                }
                string msg = "";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
        public PagedData<ITDeclarationModel> GetITDeclarations()
        {
            try
            {
                var model = new List<ITDeclarationModel>();
                var searchData = new PagedData<ITDeclarationModel>();
                int recordCount = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from AI in context.tblITDeclaration
                                 join E in context.tblEmpITDeclaration on AI.DeclarationID equals E.DeclarationID into EmpDeclaration
                                 from Emp in EmpDeclaration.DefaultIfEmpty()
                                     //where Emp.DeclarationID == AI.DeclarationID
                                 orderby AI.DeclarationID
                                 select new
                                 {
                                     AI.DeclarationID,
                                     AI.SNO,
                                     AI.SectionName,
                                     AI.SectionCode,
                                     AI.Particulars,
                                     AI.MaxLimit,
                                     AI.Age,
                                     AI.CreatedAt,
                                     AI.UpdatedAt,
                                     AI.CreatedBy,
                                     AI.UpdatedBy,
                                     Emp.Amount
                                 });
                    var records = query.ToList();
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new ITDeclarationModel
                            {
                                DeclarationID = records[i].DeclarationID,
                                SNO = Convert.ToInt32(records[i].SNO),
                                SectionName = Convert.ToString(records[i].SectionName),
                                SectionCode = Convert.ToString(records[i].SectionCode),
                                Particulars = records[i].Particulars,
                                MaxLimit = Convert.ToDecimal(records[i].MaxLimit),
                                Age = Convert.ToInt32(records[i].Age)
                                //CreatedAt = records[i].CreatedAt,
                                //UpdatedAt = records[i].UpdatedAt,
                                //CreatedBy = records[i].CreatedBy,
                                //UpdatedBy = records[i].UpdatedBy
                            });
                        }
                        recordCount = records.Count;
                        searchData.Data = model;
                        searchData.TotalRecords = records.Count;
                        searchData.pageSize = records.Count;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / searchData.pageSize));
                    }
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public EmployeeDetailsModel GetEmpInfo(string EmpNo)
        {
            try
            {
                EmployeeDetailsModel model = new EmployeeDetailsModel();
                using (var context = new IOASDBEntities())
                {
                    var record = (from AI in context.VWAppointInfo
                                  where AI.EmployeeID == EmpNo
                                  orderby AI.EmployeeID
                                  select new
                                  {
                                      AI.EmployeeID,
                                      AI.EmployeeName,
                                      AI.MeetingID,
                                      AI.CandidateID,
                                      AI.DOB,
                                      AI.DesignationCode,
                                      AI.DesignationName,
                                      AI.AppointmentDate,
                                      AI.ToDate,
                                      AI.RelieveDate,
                                      AI.BasicSalary,
                                      AI.PermanentAddress,
                                      AI.CommunicationAddress,
                                      AI.MobileNumber,
                                      AI.EmailID,
                                      AI.BankName,
                                      AI.BranchName,
                                      AI.BankAccountNo,
                                      AI.IFSC_Code,
                                      AI.OutSourcingCompany,
                                      AI.OrderID,
                                      AI.OrderType,
                                      AI.ProjectNo,
                                      AI.FromDate,
                                      AI.DetailToDate,
                                      AI.GrossSalary,
                                      AI.CostToProject,
                                      AI.CommitmentNo,
                                      AI.Remarks
                                  }).SingleOrDefault();
                    if (record != null)
                    {
                        model.EmployeeID = record.EmployeeID;
                        model.EmployeeName = record.EmployeeName;
                        model.MeetingID = Convert.ToString(record.MeetingID);
                        model.CandidateID = Convert.ToString(record.CandidateID);
                        model.DesignationCode = record.DesignationCode;
                        model.DesignationName = record.DesignationName;
                        model.AppointmentDate = record.AppointmentDate;
                        model.ToDate = record.ToDate;
                        model.RelieveDate = Convert.ToDateTime(record.RelieveDate);
                        model.BasicSalary = record.BasicSalary;
                        model.PermanentAddress = record.PermanentAddress;
                        model.BankName = record.BankName;
                        model.BranchName = record.BranchName;
                        model.BankAccountNo = record.BankAccountNo;
                        model.IFSC_Code = record.IFSC_Code;
                        model.OutSourcingCompany = record.OutSourcingCompany;
                        model.OrderID = record.OrderID;
                        model.OrderType = record.OrderType;
                        model.ProjectNo = record.ProjectNo;
                        model.FromDate = record.FromDate;
                        model.DetailToDate = record.DetailToDate;
                        model.GrossSalary = record.GrossSalary;
                        model.CostToProject = record.CostToProject;
                        model.CommitmentNo = record.CommitmentNo;
                        model.Remarks = record.Remarks;
                        model.EmailID = record.EmailID;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedData<SalaryModel> GetEmployeeSalary(int page, int pageSize, int PaymentHeadId)
        {
            try
            {
                var searchData = new PagedData<SalaryModel>();
                List<SalaryModel> model = new List<SalaryModel>();
                int skiprec = 0;

                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var query = (from sm in context.tblSalaryPayment
                                 join AI in context.VWOngoing on sm.EmployeeId equals AI.FileNo
                                 join CC in context.tblCodeControl on sm.ModeOfPayment equals CC.CodeValAbbr
                                 where sm.PaymentHeadId == PaymentHeadId && CC.CodeName == "SalaryPaymentType"
                                 orderby sm.EmployeeId
                                 select new
                                 {
                                     EmployeeName = AI.NAME,
                                     sm.PaymentId,
                                     sm.EmployeeId,
                                     sm.EmpNo,
                                     sm.Basic,
                                     sm.HRA,
                                     sm.MA,
                                     sm.DA,
                                     sm.Conveyance,
                                     sm.Deduction,
                                     sm.Tax,
                                     sm.ProfTax,
                                     sm.TaxableIncome,
                                     sm.NetSalary,
                                     sm.MonthSalary,
                                     sm.MonthlyTax,
                                     sm.AnnualSalary,
                                     sm.AnnualExemption,
                                     sm.PaidDate,
                                     sm.PaymentMonthYear,
                                     sm.PaymentCategory,
                                     sm.PaidAmount,
                                     sm.Status,
                                     sm.IsPaid,
                                     sm.ModeOfPayment,
                                     sm.TypeOfPayBill,
                                     CC.CodeValDetail
                                 });

                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new SalaryModel
                            {
                                EmployeeId = records[i].EmployeeId,
                                EmployeeName = records[i].EmployeeName,
                                PaymentId = records[i].PaymentId,
                                Basic = Convert.ToDecimal(records[i].Basic),
                                HRA = Convert.ToDecimal(records[i].HRA),
                                MA = Convert.ToDecimal(records[i].MA),
                                DA = Convert.ToDecimal(records[i].DA),
                                Conveyance = Convert.ToDecimal(records[i].Conveyance),
                                Deduction = Convert.ToDecimal(records[i].Deduction),
                                Tax = Convert.ToDecimal(records[i].Tax),
                                ProfTax = Convert.ToDecimal(records[i].Basic),
                                TaxableIncome = Convert.ToDecimal(records[i].TaxableIncome),
                                NetSalary = Convert.ToDecimal(records[i].NetSalary),
                                MonthSalary = Convert.ToDecimal(records[i].MonthSalary),
                                MonthlyTax = Convert.ToDecimal(records[i].MonthlyTax),
                                AnnualSalary = Convert.ToDecimal(records[i].AnnualSalary),
                                AnnualExemption = Convert.ToDecimal(records[i].AnnualExemption),
                                PaidAmount = Convert.ToDecimal(records[i].PaidAmount),
                                PaidDate = Convert.ToDateTime(records[i].PaidDate),
                                ModeOfPayment = Convert.ToInt32(records[i].ModeOfPayment),
                                ModeOfPaymentName = records[i].CodeValDetail,
                                Status = records[i].Status
                            });
                        }

                        searchData.Data = model;
                        searchData.TotalRecords = recordCount;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    }

                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<ProjectCommitmentModel> GetProjectCommitment(string PaymentMonthYear, int PaymentHeadId)
        {
            try
            {

                var model = new List<ProjectCommitmentModel>();

                using (var context = new IOASDBEntities())
                {


                    var query = (from SP in context.tblSalaryPayment
                                 join Emp in context.VWOngoing on SP.EmployeeId equals Emp.FileNo
                                 //join Com in context.tblCommitment on Emp.commitmentNo equals Com.CommitmentNumber 
                                 //from C in (from CM in Commit
                                 //           where CM.CommitmentNumber == Emp.commitmentNo
                                 //           select CM).DefaultIfEmpty()
                                 where SP.PaymentMonthYear == PaymentMonthYear && SP.PaymentHeadId == PaymentHeadId
                                 group SP by new
                                 {
                                     ProjectNo = SP.ProjectNo,
                                     CommitmentNo = Emp.commitmentNo,
                                     SP.Basic,
                                     SP.Conveyance,
                                     SP.DA,
                                     SP.HRA,
                                     SP.MA
                                 } into Sal
                                 select new
                                 {
                                     Sal.Key.ProjectNo,
                                     Sal.Key.CommitmentNo,
                                     SalaryPaid = Sal.Sum(i => (i.Basic + i.Conveyance + i.DA + i.HRA + i.MA))
                                 });
                    var records = query.ToList();
                    decimal balanceAfter = 0;
                    decimal SalaryToBePaid = 0;
                    decimal CurrentBalance = 0;
                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            var commitmentNo = string.IsNullOrEmpty(records[i].CommitmentNo) ? "" : records[i].CommitmentNo;
                            var commitment = (from C in context.tblCommitment
                                              join CD in context.tblCommitmentDetails on C.CommitmentId equals CD.CommitmentId
                                              where C.CommitmentNumber == commitmentNo
                                              select new
                                              {
                                                  C.CommitmentNumber,
                                                  CommitmentAmount = CD.Amount,
                                                  CommitmentBalance = CD.BalanceAmount
                                              }).SingleOrDefault();
                            if (commitment != null)
                            {
                                CurrentBalance = Convert.ToDecimal(commitment.CommitmentBalance);
                                SalaryToBePaid = Convert.ToDecimal(records[i].SalaryPaid);
                                balanceAfter = CurrentBalance - SalaryToBePaid;
                            }
                            else
                            {
                                CurrentBalance = 0;
                                SalaryToBePaid = Convert.ToDecimal(records[i].SalaryPaid);
                                balanceAfter = CurrentBalance - SalaryToBePaid;
                            }

                            model.Add(new ProjectCommitmentModel
                            {
                                MakePayment = true,
                                ProjectNo = records[i].ProjectNo,
                                CommitmentNo = records[i].CommitmentNo,
                                SalaryToBePaid = SalaryToBePaid,
                                CurrentBalance = CurrentBalance,
                                BalanceAfter = balanceAfter,
                                IsBalanceAavailable = (balanceAfter > 0)

                            });
                        }

                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


        #region  AgencySalary
        public List<AgencySalaryModel> getAgencySalaryList()
        {
            try
            {
                List<AgencySalaryModel> list = new List<AgencySalaryModel>();
                using (var context = new IOASDBEntities())
                {
                    list = (from d in context.tblAgencySalary
                            orderby d.AgencySalaryId descending
                            select new
                            {
                                d.AgencySalaryId,
                                d.PaymentNo,
                                d.MonthYearStr,
                                d.DateOfPayment,
                                d.TotalEmployees,
                                d.NetPayable,
                                d.Status
                            })
                                 .AsEnumerable()
                                 .Select((x, index) => new AgencySalaryModel()
                                 {
                                     SlNo = index + 1,
                                     AgencySalaryID = x.AgencySalaryId,
                                     PaymentNo = x.PaymentNo,
                                     MonthYear = x.MonthYearStr,
                                     DateOfPayment = String.Format("{0:dd-MMMM-yyyy}", x.DateOfPayment),
                                     TotalEmployee = x.TotalEmployees ?? 0,
                                     TotalAmount = x.NetPayable ?? 0,
                                     Status = x.Status
                                 }).ToList();

                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<AgencySalaryModel>();
            }
        }
        public bool ValidateAgencySalaryStatus(int agencySalaryId, string[] status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == agencySalaryId && status.Contains(m.Status));
                    if (query != null)
                        isValid = true;
                }
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public AgencySalaryModel GetAgencySalaryDetails(int agencySalaryId)
        {
            try
            {
                AgencySalaryModel bill = new AgencySalaryModel();
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == agencySalaryId);
                    if (query != null)
                    {
                        bill.AgencySalaryID = query.AgencySalaryId;
                        bill.CGST = query.CGST;
                        bill.MonthYear = query.MonthYearStr;
                        bill.NetAmount = query.NetAmount;
                        bill.NetPayable = query.NetPayable;
                        bill.PaymentNo = query.PaymentNo;
                        bill.ServiceCharge = query.ServiceCharge;
                        bill.SGST = query.SGST;
                        bill.TotalAmount = query.TotalAmount;
                        bill.CommitmentAmount = context.tblAgencySalaryCommitmentDetail.Where(m => m.AgencySalaryId == agencySalaryId).Sum(m => m.Amount);
                        bill.ExpenseAmount = query.ExpenseAmount;
                        bill.DeductionAmount = query.DeductionAmount;
                        bill.CheckListVerified_By = query.CheckListVerified_By;
                        bill.CheckListVerifierName = Common.GetUserFirstName(query.CheckListVerified_By ?? 0);

                        bill.CommitmentDetail = (from c in context.tblAgencySalaryCommitmentDetail
                                                 join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                                                 join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                                 join p in context.tblProject on com.ProjectId equals p.ProjectId
                                                 join head in context.tblBudgetHead on det.AllocationHeadId equals head.BudgetHeadId
                                                 orderby det.ComitmentDetailId descending
                                                 where c.AgencySalaryId == agencySalaryId && c.VerifiedSalaryId == 0 && c.Status == "Active"
                                                 select new BillCommitmentDetailModel()
                                                 {
                                                     CommitmentDetailId = c.CommitmentDetailId,
                                                     CommitmentNumber = com.CommitmentNumber,
                                                     ProjectNumber = p.ProjectNumber,
                                                     ProjectId = com.ProjectId,
                                                     HeadName = head.HeadName,
                                                     AvailableAmount = det.BalanceAmount ?? 0,
                                                     PaymentAmount = c.Amount,
                                                     BillCommitmentDetailId = c.AgencySalaryCommitmentDetailId
                                                 }).ToList();

                        bill.ExpenseDetail = (from e in context.tblAgencySalaryTransactionDetail
                                              where e.AgencySalaryId == agencySalaryId && e.Status == "Active"
                                              select new
                                              {
                                                  e.AccountHeadId,
                                                  e.Amount,
                                                  e.AccountGroupId,
                                                  e.AgencySalaryTransactionDetailId,
                                                  e.TransactionType,
                                                  e.IsJV_f
                                              })
                                              .AsEnumerable()
                                              .Select((x) => new BillExpenseDetailModel()
                                              {
                                                  AccountHeadId = x.AccountHeadId,
                                                  Amount = x.Amount,
                                                  TransactionType = x.TransactionType,
                                                  AccountGroupList = Common.GetAccountGroup(x.AccountGroupId ?? 0),
                                                  AccountGroupId = x.AccountGroupId,
                                                  AccountHeadList = Common.GetAccountHeadList(x.AccountGroupId ?? 0, x.AccountHeadId ?? 0, "1", "SLA"),
                                                  BillExpenseDetailId = x.AgencySalaryTransactionDetailId,
                                                  IsJV = x.IsJV_f ?? false
                                              }).ToList();

                        bill.DeductionDetail = (from d in context.tblAgencySalaryDeductionDetail
                                                join dh in context.tblDeductionHead on d.DeductionHeadId equals dh.DeductionHeadId
                                                join hd in context.tblAccountHead on dh.AccountHeadId equals hd.AccountHeadId
                                                join g in context.tblAccountGroup on hd.AccountGroupId equals g.AccountGroupId
                                                where d.AgencySalaryId == agencySalaryId && d.Status == "Active"
                                                select new BillDeductionDetailModel()
                                                {
                                                    AccountGroupId = d.AccountGroupId,
                                                    BillDeductionDetailId = d.AgencySalaryDeductionDetailId,
                                                    Amount = d.Amount,
                                                    DeductionHeadId = d.DeductionHeadId,
                                                    AccountGroup = g.AccountGroup,
                                                    DeductionHead = hd.AccountHead
                                                }).ToList();

                        bill.CheckListDetail = (from ck in context.tblAgencySalaryCheckDetail
                                                join chkf in context.tblFunctionCheckList on ck.FunctionCheckListId equals chkf.FunctionCheckListId
                                                where ck.AgencySalaryId == agencySalaryId && ck.Status == "Active"
                                                select new CheckListModel()
                                                {
                                                    CheckList = chkf.CheckList,
                                                    FunctionCheckListId = ck.FunctionCheckListId,
                                                    IsChecked = true
                                                }).ToList();
                        bill.DocumentDetail = (from d in context.tblAgencySalaryDocumentDetail
                                               where d.AgencySalaryId == agencySalaryId && d.Status == "Active"
                                               select new AttachmentDetailModel()
                                               {
                                                   DocumentActualName = d.DocumentActualName,
                                                   DocumentDetailId = d.AgencySalaryDocumentDetailId,
                                                   DocumentName = d.DocumentName,
                                                   DocumentPath = "~/Content/OtherDocuments",
                                                   DocumentType = d.DocumentType,
                                                   Remarks = d.Remarks
                                               }).ToList();

                    }
                }
                return bill;
            }
            catch (Exception ex)
            {
                return new AgencySalaryModel();
            }
        }
        public static List<AgencySalaryModel> SearchAgencySalaryList(AgencySearchFieldModel model)
        {
            List<AgencySalaryModel> honor = new List<AgencySalaryModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var y = Common.Getmonth();
                    var predicate = PredicateBuilder.BaseAnd<tblAgencySalary>();
                    if (!string.IsNullOrEmpty(model.PaymentNo))
                        predicate = predicate.And(d => d.PaymentNo == model.PaymentNo);
                    if (model.FromDate != null && model.ToDate != null)
                    {
                        model.ToDate = model.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                        predicate = predicate.And(d => d.Crtd_TS >= model.FromDate && d.Crtd_TS <= model.ToDate);
                    }
                    var query = context.tblAgencySalary.Where(predicate).OrderByDescending(m => m.AgencySalaryId).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            honor.Add(new AgencySalaryModel()
                            {
                                SlNo = i + 1,
                                AgencySalaryID = query[i].AgencySalaryId,
                                PaymentNo = query[i].PaymentNo,
                                MonthYear = query[i].MonthYearStr,
                                DateOfPayment = String.Format("{0:dd-MMMM-yyyy}", query[i].DateOfPayment),
                                TotalEmployee = query[i].TotalEmployees ?? 0,
                                TotalAmount = query[i].TotalAmount ?? 0,
                                Status = query[i].Status
                            });
                        }
                    }
                    return honor;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedData<AgencyStaffDetailsModel> GetAgencyEmployeeSalary(int page, int pageSize, int AgencySalaryID, string MonthYear)
        {
            try
            {
                var searchData = new PagedData<AgencyStaffDetailsModel>();
                List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
                int skiprec = 0;

                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    if (AgencySalaryID == 0)
                    {
                        var queryMYExists = context.tblAgencySalary.Any(m => m.MonthYearStr == MonthYear);
                        if (queryMYExists)
                        {
                            searchData.Data = model;
                            searchData.TotalRecords = 0;
                            searchData.TotalPages = 0;
                            return searchData;
                        }
                        var query = (from sm in context.vwAppointmentMaster
                                     join ad in context.vwAppointmentDetails on sm.EmployeeId equals ad.EmployeeId
                                     join sd in context.vwSalaryDetails on ad.EmployeeId equals sd.EmployeeId
                                     where sm.status == "Active"
                                     orderby sm.EmployeeId
                                     select new
                                     {
                                         sm,
                                         sd.NetSalary
                                     });

                        var records = query.Skip(skiprec).Take(pageSize).ToList();
                        var recordCount = query.ToList().Count();

                        if (records.Count > 0)
                        {
                            for (int i = 0; i < records.Count; i++)
                            {
                                model.Add(new AgencyStaffDetailsModel()
                                {
                                    EmployeeId = records[i].sm.EmployeeId,
                                    Name = records[i].sm.EmployeeName,
                                    Designation = records[i].sm.DesignationName,
                                    BasicSalary = Convert.ToDecimal(records[i].sm.BasicSalary),
                                    NetSalary = Convert.ToDecimal(records[i].NetSalary)
                                });
                            }
                        }
                        searchData.Data = model;
                        searchData.TotalRecords = recordCount;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    }
                    else
                    {
                        var query = (from sm in context.vwAppointmentMaster
                                     join ad in context.vwAppointmentDetails on sm.EmployeeId equals ad.EmployeeId
                                     join sd in context.vwSalaryDetails on ad.EmployeeId equals sd.EmployeeId
                                     where sm.status == "Active"
                                     && !context.tblAgencyVerifiedSalary.Any(m => m.AgencySalaryId == AgencySalaryID && m.EmployeeID == sm.EmployeeId)
                                     orderby sm.EmployeeId
                                     select new
                                     {
                                         sm,
                                         sd.NetSalary
                                     });

                        var records = query.Skip(skiprec).Take(pageSize).ToList();
                        var recordCount = query.ToList().Count();

                        if (records.Count > 0)
                        {
                            for (int i = 0; i < records.Count; i++)
                            {
                                model.Add(new AgencyStaffDetailsModel()
                                {
                                    EmployeeId = records[i].sm.EmployeeId,
                                    Name = records[i].sm.EmployeeName,
                                    Designation = records[i].sm.DesignationName,
                                    BasicSalary = Convert.ToDecimal(records[i].sm.BasicSalary),
                                    NetSalary = Convert.ToDecimal(records[i].NetSalary)
                                });
                            }                           
                        }
                        searchData.Data = model;
                        searchData.TotalRecords = recordCount;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    }
                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public PagedData<AgencyStaffDetailsModel> GetVerifiedEmployeeSalary(int pageIndex, int pageSize, int AgencySalaryId, string EmployeeId, string Name)
        {
            var searchData = new PagedData<AgencyStaffDetailsModel>();
            List<AgencyStaffDetailsModel> model = new List<AgencyStaffDetailsModel>();
            try
            {
                int skiprec = 0;

                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var TodayDate = DateTime.Now.Month;
                    var query = (from C in context.tblAgencyVerifiedSalary
                                 where C.AgencySalaryId == AgencySalaryId
                                 && (String.IsNullOrEmpty(EmployeeId) || C.EmployeeID.Contains(EmployeeId))
                                 && (String.IsNullOrEmpty(Name) || C.EmployeeName.Contains(Name))
                                 orderby C.EmployeeID
                                 select C);

                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new AgencyStaffDetailsModel()
                            {
                                SlNo = skiprec + i + 1,
                                EmployeeId = records[i].EmployeeID,
                                Name = records[i].EmployeeName,
                                AgencySalaryID = records[i].AgencySalaryId,
                                NetSalary = records[i].Netsalary,
                                VerifiedSalaryId = records[i].VerifiedSalaryId
                            });
                        }
                    }
                    searchData.Data = model;
                    searchData.TotalRecords = recordCount;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                searchData.Data = model;
                return searchData;
            }
        }
        public PagedData<BillCommitmentDetailModel> GetAgencySalaryCommitmentDetail(int pageIndex, int pageSize, int AgencySalaryId, string commitmentNo, string projectNo, string headName)
        {
            var searchData = new PagedData<BillCommitmentDetailModel>();
            List<BillCommitmentDetailModel> model = new List<BillCommitmentDetailModel>();
            try
            {
                int skiprec = 0;

                if (pageIndex == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (pageIndex - 1) * pageSize;
                }
                using (var context = new IOASDBEntities())
                {
                    var query = (from c in context.tblAgencySalaryCommitmentDetail
                                 join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                                 join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                 join p in context.tblProject on com.ProjectId equals p.ProjectId
                                 join head in context.tblBudgetHead on det.AllocationHeadId equals head.BudgetHeadId
                                 orderby det.ComitmentDetailId descending
                                 where c.AgencySalaryId == AgencySalaryId && c.Status == "Active"
                                 && (String.IsNullOrEmpty(commitmentNo) || com.CommitmentNumber.Contains(commitmentNo))
                                 && (String.IsNullOrEmpty(projectNo) || p.ProjectNumber.Contains(projectNo))
                                 && (String.IsNullOrEmpty(headName) || head.HeadName.Contains(headName))
                                 select new
                                 {
                                     c.CommitmentDetailId,
                                     com.CommitmentNumber,
                                     p.ProjectNumber,
                                     com.ProjectId,
                                     head.HeadName,
                                     det.BalanceAmount,
                                     c.Amount
                                 });

                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();

                    if (records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new BillCommitmentDetailModel()
                            {
                                SlNo = skiprec + i + 1,
                                CommitmentDetailId = records[i].CommitmentDetailId,
                                CommitmentNumber = records[i].CommitmentNumber,
                                ProjectNumber = records[i].ProjectNumber,
                                ProjectId = records[i].ProjectId,
                                HeadName = records[i].HeadName,
                                AvailableAmount = records[i].BalanceAmount ?? 0,
                                PaymentAmount = records[i].Amount
                            });
                        }
                    }
                    searchData.Data = model;
                    searchData.TotalRecords = recordCount;
                }

                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                searchData.Data = model;
                return searchData;
            }
        }
        public AgencyStaffDetailsModel getEmployeeSalaryDetails(string EmployeeID, int AgencySalaryID)
        {
            AgencyStaffDetailsModel model = new AgencyStaffDetailsModel();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qrySalary = (from C in context.vwSalaryDetails
                                     join D in context.vwAppointmentDetails on C.EmployeeId equals D.EmployeeId
                                     where C.EmployeeId == EmployeeID
                                     select new { C, D }).FirstOrDefault();
                    if (qrySalary != null)
                    {
                        var EmpName = Common.getAgencyEmployeeName(qrySalary.C.EmployeeId);
                        model.EmployeeId = qrySalary.C.EmployeeId;
                        model.Name = EmpName;
                        model.BasicSalary = qrySalary.C.BasicSalary;
                        model.HRA = qrySalary.C.HRA;
                        model.Bonus = qrySalary.C.Bonus;
                        model.SpecialAllowance = qrySalary.C.SpecialAllowance;
                        model.PF = qrySalary.C.EmployeePF;
                        model.ESI = qrySalary.C.EmployeeESIC;
                        model.IncomeTax = qrySalary.C.ProfessionalTax;
                        model.TotalDeduction = qrySalary.C.EmployeePF + qrySalary.C.EmployeeESIC + qrySalary.C.ProfessionalTax;
                        model.GrossSalary = qrySalary.C.GrossSalary;
                        model.NetSalary = qrySalary.C.NetSalary;
                        model.AgencySalaryID = AgencySalaryID;
                        model.EmployerESI = qrySalary.C.EmployerESIC;
                        model.EmployerPF = qrySalary.C.EmployerPF;
                        model.EmployerContribution = qrySalary.C.TotalContribution;
                        model.GrossTotal = qrySalary.C.GrossTotal;
                        model.ProjectNo = qrySalary.D.ProjectNo;
                        model.CommitmentNo = qrySalary.D.CommitmentNo;
                    }
                }
                return model;
            }
            catch (Exception)
            {

                return model;
            }
        }

        public AgencySalaryModel VerifyEmployeeDetails(AgencyVerifyEmployeeModel model, int UserID)
        {
            using (var context = new IOASDBEntities())
            {
                AgencySalaryModel data = new AgencySalaryModel();
                data.Status = "error";
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var qrySalary = context.vwSalaryDetails.FirstOrDefault(c => c.EmployeeId == model.EmployeeId);
                        if (qrySalary != null)
                        {
                            decimal NetSalary = qrySalary.GrossTotal;
                            if (model != null)
                            {
                                decimal allow = model.buDetail.Where(m => m.TypeId == 1).Sum(m => m.Amount) ?? 0;
                                decimal deduct = model.buDetail.Where(m => m.TypeId == 2).Sum(m => m.Amount) ?? 0;
                                NetSalary = NetSalary + allow - deduct;
                                if (NetSalary < 0)
                                    return data;
                            }
                            var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == model.AgencySalaryID select C).FirstOrDefault();
                            if (qryAgency != null)
                            {
                                decimal overAllVal =  (qryAgency.TotalAmount ?? 0) + NetSalary;
                                qryAgency.DateOfPayment = DateTime.Now;
                                qryAgency.TotalAmount = overAllVal;
                                qryAgency.TotalEmployees += 1;
                                qryAgency.Lastupdate_TS = DateTime.Now;
                                qryAgency.LastupdatedUserid = UserID;
                                context.SaveChanges();
                                model.MonthYear = qryAgency.MonthYearStr;
                            }
                            else
                            {
                                var PayMentNo = Common.getAgencySalarySequenceNumber();
                                tblAgencySalary Agency = new tblAgencySalary();
                                Agency.PaymentNo = PayMentNo.Item1;
                                Agency.SqquenceNo = PayMentNo.Item2;
                                Agency.DateOfPayment = DateTime.Now;
                                Agency.TotalAmount = NetSalary;
                                Agency.TotalEmployees = 1;
                                Agency.Status = "Open";
                                Agency.PaymentMonthYear = fo.GetMonthFirstDate(model.MonthYear);
                                Agency.MonthYearStr = model.MonthYear;
                                Agency.Crtd_TS = DateTime.Now;
                                Agency.Crtd_UserId = UserID;
                                context.tblAgencySalary.Add(Agency);
                                context.SaveChanges();
                                model.AgencySalaryID = Agency.AgencySalaryId;
                            }

                            tblAgencyVerifiedSalary Salary = new tblAgencyVerifiedSalary();
                            var EmpName = Common.getAgencyEmployeeName(qrySalary.EmployeeId);
                            Salary.EmployeeID = model.EmployeeId;
                            Salary.AgencySalaryId = model.AgencySalaryID;
                            Salary.EmployeeName = EmpName;
                            Salary.IsVerified = true;
                            Salary.MonthYear = model.MonthYear;
                            Salary.Netsalary = NetSalary;
                            Salary.OrderId = qrySalary.OrderID;
                            Salary.Crtd_TS = DateTime.Now;
                            Salary.Crtd_UserId = UserID;
                            context.tblAgencyVerifiedSalary.Add(Salary);
                            context.SaveChanges();
                            int verifiedSalaryId = Salary.VerifiedSalaryId;

                            foreach (var item in model.buDetail)
                            {
                                tblAgencySalaryBreakUpDetail BU = new tblAgencySalaryBreakUpDetail();
                                BU.Amount = item.Amount;
                                BU.CategoryId = item.TypeId;
                                BU.HeadId = item.HeadId;
                                BU.Remarks = item.Remarks;
                                BU.VerifiedSalaryId = verifiedSalaryId;
                                context.tblAgencySalaryBreakUpDetail.Add(BU);
                                context.SaveChanges();
                            }
                            var qryCommitment = (from sal in context.vwSalaryDetails
                                                 join det in context.vwAppointmentDetails on sal.EmployeeId equals det.EmployeeId
                                                 join c in context.tblCommitment on det.CommitmentNo equals c.CommitmentNumber
                                                 join cDet in context.tblCommitmentDetails on c.CommitmentId equals cDet.CommitmentId
                                                 where det.EmployeeId == model.EmployeeId
                                                 select new { sal.NetSalary, cDet.BalanceAmount, c.CommitmentId, cDet.ComitmentDetailId }).FirstOrDefault();
                            if (qryCommitment != null && NetSalary <= qryCommitment.BalanceAmount)
                            {
                                tblAgencySalaryCommitmentDetail com = new tblAgencySalaryCommitmentDetail();
                                com.AgencySalaryId = model.AgencySalaryID;
                                com.Amount = NetSalary;
                                com.CommitmentDetailId = qryCommitment.ComitmentDetailId;
                                com.CRTD_By = UserID;
                                com.CRTD_TS = DateTime.Now;
                                com.Status = "Active";
                                com.VerifiedSalaryId = verifiedSalaryId;
                                context.tblAgencySalaryCommitmentDetail.Add(com);
                                context.SaveChanges();
                            }
                            else
                                return data;
                            transaction.Commit();
                            data = UpdateSalaryCalculation(model.AgencySalaryID);
                            data.AgencySalaryID = model.AgencySalaryID;
                            return data;
                        }
                        else
                            return data;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return data;
                    }
                }
            }
        }
        public AgencySalaryModel VerifyAllEmployeeDetails(int AgencySalaryID, string MonthYear, int UserID)
        {
            using (var context = new IOASDBEntities())
            {
                AgencySalaryModel data = new AgencySalaryModel();
                data.Status = "error";

                if (AgencySalaryID == 0)
                {
                    if (String.IsNullOrEmpty(MonthYear))
                        return data;
                    var queryMYExists = context.tblAgencySalary.Any(m => m.MonthYearStr == MonthYear);
                    if (queryMYExists)
                    {
                        data.Status = "Salary process already initiated for " + MonthYear;
                    }
                    var qrySalary = (from sm in context.vwAppointmentMaster
                                     join ad in context.vwAppointmentDetails on sm.EmployeeId equals ad.EmployeeId
                                     join sd in context.vwSalaryDetails on ad.EmployeeId equals sd.EmployeeId
                                     where sm.status == "Active"
                                     orderby sm.EmployeeId
                                     select new
                                     {
                                         sm.EmployeeId,
                                         sd.GrossTotal,
                                         sd.OrderID
                                     }).ToList();
                    if (qrySalary.Count > 0)
                    {
                        foreach (var item in qrySalary)
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    decimal NetSalary = item.GrossTotal;
                                    var qryCommitment = (from sal in context.vwSalaryDetails
                                                         join det in context.vwAppointmentDetails on sal.EmployeeId equals det.EmployeeId
                                                         join c in context.tblCommitment on det.CommitmentNo equals c.CommitmentNumber
                                                         join cDet in context.tblCommitmentDetails on c.CommitmentId equals cDet.CommitmentId
                                                         where det.EmployeeId == item.EmployeeId
                                                         select new { sal.NetSalary, cDet.BalanceAmount, c.CommitmentId, cDet.ComitmentDetailId }).FirstOrDefault();
                                    if (qryCommitment != null && NetSalary <= qryCommitment.BalanceAmount)
                                    {
                                        var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == AgencySalaryID select C).FirstOrDefault();
                                        if (qryAgency != null)
                                        {
                                            decimal overAllVal = (qryAgency.TotalAmount ?? 0) + NetSalary;
                                            qryAgency.DateOfPayment = DateTime.Now;
                                            qryAgency.TotalAmount = overAllVal;
                                            qryAgency.TotalEmployees += 1;
                                            qryAgency.Lastupdate_TS = DateTime.Now;
                                            qryAgency.LastupdatedUserid = UserID;
                                            context.SaveChanges();
                                            MonthYear = qryAgency.MonthYearStr;
                                        }
                                        else
                                        {
                                            var PayMentNo = Common.getAgencySalarySequenceNumber();
                                            tblAgencySalary Agency = new tblAgencySalary();
                                            Agency.PaymentNo = PayMentNo.Item1;
                                            Agency.SqquenceNo = PayMentNo.Item2;
                                            Agency.DateOfPayment = DateTime.Now;
                                            Agency.TotalAmount = NetSalary;
                                            Agency.TotalEmployees = 1;
                                            Agency.PaymentMonthYear = fo.GetMonthFirstDate(MonthYear);
                                            Agency.MonthYearStr = MonthYear;
                                            Agency.Crtd_TS = DateTime.Now;
                                            Agency.Crtd_UserId = UserID;
                                            context.tblAgencySalary.Add(Agency);
                                            context.SaveChanges();
                                            AgencySalaryID = Agency.AgencySalaryId;
                                        }

                                        tblAgencyVerifiedSalary Salary = new tblAgencyVerifiedSalary();
                                        var EmpName = Common.getAgencyEmployeeName(item.EmployeeId);
                                        Salary.EmployeeID = item.EmployeeId;
                                        Salary.AgencySalaryId = AgencySalaryID;
                                        Salary.EmployeeName = EmpName;
                                        Salary.IsVerified = true;
                                        Salary.MonthYear = MonthYear;
                                        Salary.Netsalary = NetSalary;
                                        Salary.OrderId = item.OrderID;
                                        Salary.Crtd_TS = DateTime.Now;
                                        Salary.Crtd_UserId = UserID;
                                        context.tblAgencyVerifiedSalary.Add(Salary);
                                        context.SaveChanges();
                                        int verifiedSalaryId = Salary.VerifiedSalaryId;



                                        tblAgencySalaryCommitmentDetail com = new tblAgencySalaryCommitmentDetail();
                                        com.AgencySalaryId = AgencySalaryID;
                                        com.Amount = NetSalary;
                                        com.CommitmentDetailId = qryCommitment.ComitmentDetailId;
                                        com.CRTD_By = UserID;
                                        com.CRTD_TS = DateTime.Now;
                                        com.Status = "Active";
                                        com.VerifiedSalaryId = verifiedSalaryId;
                                        context.tblAgencySalaryCommitmentDetail.Add(com);
                                        context.SaveChanges();
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return data;
                                }
                            }

                        }
                    }
                    else
                        return data;
                }
                else
                {
                    var qrySalary = (from sm in context.vwAppointmentMaster
                                     join ad in context.vwAppointmentDetails on sm.EmployeeId equals ad.EmployeeId
                                     join sd in context.vwSalaryDetails on ad.EmployeeId equals sd.EmployeeId
                                     where sm.status == "Active"
                                     && !context.tblAgencyVerifiedSalary.Any(m => m.AgencySalaryId == AgencySalaryID && m.EmployeeID == sm.EmployeeId)
                                     orderby sm.EmployeeId
                                     select new
                                     {
                                         sm.EmployeeId,
                                         sd.NetSalary,
                                         sd.OrderID
                                     }).ToList();
                    if (qrySalary.Count > 0)
                    {
                        foreach (var item in qrySalary)
                        {
                            using (var transaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    decimal overAllVal = item.NetSalary;
                                    decimal NetSalary = item.NetSalary;
                                    var qryCommitment = (from sal in context.vwSalaryDetails
                                                         join det in context.vwAppointmentDetails on sal.EmployeeId equals det.EmployeeId
                                                         join c in context.tblCommitment on det.CommitmentNo equals c.CommitmentNumber
                                                         join cDet in context.tblCommitmentDetails on c.CommitmentId equals cDet.CommitmentId
                                                         where det.EmployeeId == item.EmployeeId
                                                         select new { sal.NetSalary, cDet.BalanceAmount, c.CommitmentId, cDet.ComitmentDetailId }).FirstOrDefault();
                                    if (qryCommitment != null && NetSalary <= qryCommitment.BalanceAmount)
                                    {
                                        var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == AgencySalaryID select C).FirstOrDefault();
                                        if (qryAgency != null)
                                        {
                                            overAllVal = (qryAgency.TotalAmount ?? 0) + NetSalary;
                                            qryAgency.DateOfPayment = DateTime.Now;
                                            qryAgency.TotalAmount = overAllVal;
                                            qryAgency.TotalEmployees += 1;
                                            qryAgency.Lastupdate_TS = DateTime.Now;
                                            qryAgency.LastupdatedUserid = UserID;
                                            context.SaveChanges();
                                            MonthYear = qryAgency.MonthYearStr;
                                        }

                                        tblAgencyVerifiedSalary Salary = new tblAgencyVerifiedSalary();
                                        var EmpName = Common.getAgencyEmployeeName(item.EmployeeId);
                                        Salary.EmployeeID = item.EmployeeId;
                                        Salary.AgencySalaryId = AgencySalaryID;
                                        Salary.EmployeeName = EmpName;
                                        Salary.IsVerified = true;
                                        Salary.MonthYear = MonthYear;
                                        Salary.Netsalary = NetSalary;
                                        Salary.OrderId = item.OrderID;
                                        Salary.Crtd_TS = DateTime.Now;
                                        Salary.Crtd_UserId = UserID;
                                        context.tblAgencyVerifiedSalary.Add(Salary);
                                        context.SaveChanges();
                                        int verifiedSalaryId = Salary.VerifiedSalaryId;



                                        tblAgencySalaryCommitmentDetail com = new tblAgencySalaryCommitmentDetail();
                                        com.AgencySalaryId = AgencySalaryID;
                                        com.Amount = NetSalary;
                                        com.CommitmentDetailId = qryCommitment.ComitmentDetailId;
                                        com.CRTD_By = UserID;
                                        com.CRTD_TS = DateTime.Now;
                                        com.Status = "Active";
                                        com.VerifiedSalaryId = verifiedSalaryId;
                                        context.tblAgencySalaryCommitmentDetail.Add(com);
                                        context.SaveChanges();
                                    }
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return data;
                                }
                            }
                        }
                    }
                    else
                        return data;
                }
                data = UpdateSalaryCalculation(AgencySalaryID);
                data.AgencySalaryID = AgencySalaryID;
                return data;
            }
        }
        public AgencySalaryModel DeleteVerifiedEmployee(int VerifiedSalaryId)
        {
            using (var context = new IOASDBEntities())
            {
                AgencySalaryModel data = new AgencySalaryModel();
                data.Status = "error";
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int agencyId = 0;
                        var qrySalary = context.tblAgencyVerifiedSalary.FirstOrDefault(c => c.VerifiedSalaryId == VerifiedSalaryId);
                        if (qrySalary != null)
                        {
                            decimal NetSalary = qrySalary.Netsalary ?? 0;
                            agencyId = Convert.ToInt32(qrySalary.AgencySalaryId);
                            var qryAgency = (from C in context.tblAgencySalary where C.AgencySalaryId == agencyId select C).FirstOrDefault();
                            if (qryAgency != null)
                            {
                                NetSalary = (qryAgency.TotalAmount ?? 0) - NetSalary;
                                qryAgency.TotalAmount = NetSalary;
                                qryAgency.TotalEmployees = qryAgency.TotalEmployees - 1;
                                context.SaveChanges();
                            }
                            else
                                return data;
                        }
                        else
                            return data;
                        context.tblAgencyVerifiedSalary.RemoveRange(context.tblAgencyVerifiedSalary.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        context.tblAgencySalaryBreakUpDetail.RemoveRange(context.tblAgencySalaryBreakUpDetail.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        context.tblAgencySalaryCommitmentDetail.RemoveRange(context.tblAgencySalaryCommitmentDetail.Where(c => c.VerifiedSalaryId == VerifiedSalaryId));
                        context.SaveChanges();
                        transaction.Commit();
                        data = UpdateSalaryCalculation(agencyId);
                        return data;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return data;
                    }
                }
            }
        }
        private AgencySalaryModel UpdateSalaryCalculation(int agencySalId)
        {
            AgencySalaryModel data = new AgencySalaryModel();
            data.Status = "error";
            using (var context = new IOASDBEntities())
            {
                var qryAgency = context.tblAgencySalary.FirstOrDefault(C => C.AgencySalaryId == agencySalId);
                if (qryAgency != null)
                {

                    decimal ttlSal = qryAgency.TotalAmount ?? 0;
                    if (ttlSal > 0)
                    {
                        decimal scPct = (Decimal)1.6;
                        decimal sc = ttlSal * scPct / 100;
                        sc = Math.Round(sc, 2, MidpointRounding.AwayFromZero);
                        decimal netSal = ttlSal + sc;
                        decimal GST = netSal * 18 / 100;
                        GST = Math.Round(GST, 2, MidpointRounding.AwayFromZero);
                        decimal CGST = GST / 2;
                        decimal payable = netSal + GST;
                        payable = Math.Round(payable, 2, MidpointRounding.AwayFromZero);
                        CGST = Math.Round(CGST, 2, MidpointRounding.AwayFromZero);

                        qryAgency.SGST = CGST;
                        qryAgency.CGST = CGST;
                        qryAgency.ServiceCharge = sc;
                        qryAgency.NetAmount = netSal;
                        qryAgency.NetPayable = payable;
                        qryAgency.Status = "Init";

                        data.SGST = CGST;
                        data.CGST = CGST;
                        data.ServiceCharge = sc;
                        data.NetAmount = netSal;
                        data.NetPayable = payable;
                        data.TotalAmount = ttlSal;
                        data.CommitmentAmount = context.tblAgencySalaryCommitmentDetail.Where(m => m.AgencySalaryId == agencySalId).Sum(m => m.Amount);
                        data.Status = "success";
                        context.SaveChanges();
                    }
                    else
                    {
                        qryAgency.SGST = 0;
                        qryAgency.CGST = 0;
                        qryAgency.ServiceCharge = 0;
                        qryAgency.NetAmount = 0;
                        qryAgency.NetPayable = 0;
                        qryAgency.Status = "Init";

                        data.SGST = 0;
                        data.CGST = 0;
                        data.ServiceCharge = 0;
                        data.NetAmount = 0;
                        data.NetPayable = 0;
                        data.TotalAmount = 0;
                        data.CommitmentAmount = 0;
                        data.Status = "success";
                        context.SaveChanges();
                    }
                }
                return data;
            }
        }
        public int CreateSalaryAgency(AgencySalaryModel model, int LoggedInUser)
        {
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.AgencySalaryID > 0)
                        {
                            int AgencyId = model.AgencySalaryID ?? 0;
                            var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == model.AgencySalaryID);
                            if (query != null)
                            {
                                query.LastupdatedUserid = LoggedInUser;
                                query.Lastupdate_TS = DateTime.Now;
                                query.CheckListVerified_By = model.CheckListVerified_By;
                                query.DeductionAmount = model.DeductionDetail != null ? model.DeductionDetail.Sum(m => m.Amount) : 0;
                                query.ExpenseAmount = model.ExpenseDetail.Sum(m => m.Amount);
                                query.Status = "Open";

                                context.tblAgencySalaryDeductionDetail.RemoveRange(context.tblAgencySalaryDeductionDetail.Where(m => m.AgencySalaryId== AgencyId));
                                context.SaveChanges();
                                if (model.DeductionDetail != null)
                                {
                                    foreach (var item in model.DeductionDetail)
                                    {
                                        if (item.Amount != null && item.Amount != 0)
                                        {
                                            if (item.AccountGroupId == null)
                                                return -1;
                                            tblAgencySalaryDeductionDetail deduction = new tblAgencySalaryDeductionDetail();
                                            deduction.AccountGroupId = item.AccountGroupId;
                                            deduction.Amount = item.Amount;
                                            deduction.AgencySalaryId = AgencyId;
                                            deduction.CRTD_By = LoggedInUser;
                                            deduction.CRTD_TS = DateTime.Now;
                                            deduction.DeductionHeadId = item.DeductionHeadId;
                                            deduction.Status = "Active";
                                            context.tblAgencySalaryDeductionDetail.Add(deduction);
                                            context.SaveChanges();
                                        }
                                    }
                                }

                                context.tblAgencySalaryTransactionDetail.RemoveRange(context.tblAgencySalaryTransactionDetail.Where(m => m.AgencySalaryId == model.AgencySalaryID));
                                context.SaveChanges();
                                foreach (var item in model.ExpenseDetail)
                                {
                                    tblAgencySalaryTransactionDetail ASTran = new tblAgencySalaryTransactionDetail();
                                    ASTran.AgencySalaryId = AgencyId;
                                    ASTran.AccountGroupId = item.AccountGroupId;
                                    ASTran.AccountHeadId = item.AccountHeadId;
                                    ASTran.Amount = item.Amount;
                                    ASTran.TransactionType = item.TransactionType;
                                    ASTran.CRTD_By = LoggedInUser;
                                    ASTran.IsJV_f = item.IsJV;
                                    ASTran.CRTD_TS = DateTime.Now;
                                    ASTran.Status = "Active";
                                    context.tblAgencySalaryTransactionDetail.Add(ASTran);
                                    context.SaveChanges();
                                }
                                context.tblAgencySalaryCommitmentDetail.RemoveRange(context.tblAgencySalaryCommitmentDetail.Where(m => m.AgencySalaryId == AgencyId && m.VerifiedSalaryId ==0));
                                context.SaveChanges();
                                foreach (var item in model.CommitmentDetail)
                                {
                                    if (item.CommitmentDetailId == null)
                                        return -3;
                                    tblAgencySalaryCommitmentDetail ASComm = new tblAgencySalaryCommitmentDetail();
                                    ASComm.AgencySalaryId = AgencyId;
                                    ASComm.CommitmentDetailId = item.CommitmentDetailId;
                                    ASComm.Amount = item.PaymentAmount;
                                    ASComm.CRTD_By = LoggedInUser;
                                    ASComm.VerifiedSalaryId = 0;
                                    ASComm.CRTD_TS = DateTime.Now;
                                    ASComm.Status = "Active";
                                    context.tblAgencySalaryCommitmentDetail.Add(ASComm);
                                    context.SaveChanges();
                                }
                                context.tblAgencySalaryCheckDetail.RemoveRange(context.tblAgencySalaryCheckDetail.Where(m => m.AgencySalaryId == AgencyId));
                                context.SaveChanges();
                                foreach (var item in model.CheckListDetail)
                                {
                                    if (item.IsChecked)
                                    {
                                        tblAgencySalaryCheckDetail ASCheck = new tblAgencySalaryCheckDetail();
                                        ASCheck.Verified_By = model.CheckListVerified_By;
                                        ASCheck.FunctionCheckListId = item.FunctionCheckListId;
                                        ASCheck.AgencySalaryId = AgencyId;
                                        ASCheck.CRTD_By = LoggedInUser;
                                        ASCheck.CRTD_TS = DateTime.Now;
                                        ASCheck.Status = "Active";
                                        context.tblAgencySalaryCheckDetail.Add(ASCheck);
                                        context.SaveChanges();
                                    }
                                }
                                var arrList = model.DocumentDetail.Select(m => m.DocumentDetailId ?? 0).ToArray();
                                context.tblAgencySalaryDocumentDetail.Where(x => x.AgencySalaryId == model.AgencySalaryID && !arrList.Contains(x.AgencySalaryDocumentDetailId) && x.Status != "InActive")
                                .ToList()
                                .ForEach(m =>
                                {
                                    m.Status = "InActive";
                                    m.UPDT_By = LoggedInUser;
                                    m.UPDT_TS = DateTime.Now;
                                });
                                foreach (var item in model.DocumentDetail)
                                {
                                    var docQuery = context.tblAgencySalaryDocumentDetail.FirstOrDefault(m => m.AgencySalaryDocumentDetailId == item.DocumentDetailId);
                                    if (docQuery == null)
                                    {
                                        tblAgencySalaryDocumentDetail ASdoc = new tblAgencySalaryDocumentDetail();
                                        string actName = System.IO.Path.GetFileName(item.DocumentFile.FileName);
                                        var guid = Guid.NewGuid().ToString();
                                        var docName = guid + "_" + actName;
                                        item.DocumentFile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/OtherDocuments/" + docName));
                                        ASdoc.CRTD_By = LoggedInUser;
                                        ASdoc.CRTD_TS = DateTime.Now;
                                        ASdoc.DocumentActualName = actName;
                                        ASdoc.DocumentName = docName;
                                        ASdoc.DocumentType = item.DocumentType;
                                        ASdoc.Remarks = item.Remarks;
                                        ASdoc.AgencySalaryId = AgencyId;
                                        ASdoc.Status = "Active";
                                        context.tblAgencySalaryDocumentDetail.Add(ASdoc);
                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        if (item.DocumentFile != null)
                                        {
                                            string actName = System.IO.Path.GetFileName(item.DocumentFile.FileName);
                                            var guid = Guid.NewGuid().ToString();
                                            var docName = guid + "_" + actName;
                                            item.DocumentFile.SaveAs(HttpContext.Current.Server.MapPath("~/Content/OtherDocuments/" + docName));
                                            docQuery.DocumentActualName = actName;
                                            docQuery.DocumentName = docName;
                                        }
                                        docQuery.UPDT_By = LoggedInUser;
                                        docQuery.UPDT_TS = DateTime.Now;
                                        docQuery.DocumentType = item.DocumentType;
                                        docQuery.Remarks = item.Remarks;
                                        context.SaveChanges();
                                    }
                                }
                                transaction.Commit();
                                return AgencyId;
                            }
                            else
                            {
                                return -2;
                            }
                        }
                        else
                            return -1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                }
            }
        }
        public string ValidateAgencyVerify(string EmployeeID, int AgencySalaryID, string MonthYear, AgencyVerifyEmployeeModel model = null)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    if (AgencySalaryID == 0)
                    {
                        if (String.IsNullOrEmpty(MonthYear))
                            return "Somenthing went wrong, Please contact administrator";
                        var queryMYExists = context.tblAgencySalary.Any(m => m.MonthYearStr == MonthYear);
                        if (queryMYExists)
                            return "Salary process already initiated for " + MonthYear;
                    }
                    else
                    {
                        var queryEmpExist = context.tblAgencyVerifiedSalary.Any(m => m.AgencySalaryId == AgencySalaryID && m.EmployeeID == EmployeeID);
                        if (queryEmpExist)
                            return "Salary process already initiated to " + EmployeeID;
                    }
                    var checkCommitment = (from sal in context.vwSalaryDetails
                                           join det in context.vwAppointmentDetails on sal.EmployeeId equals det.EmployeeId
                                           join c in context.tblCommitment on det.CommitmentNo equals c.CommitmentNumber
                                           where det.EmployeeId == EmployeeID
                                           select new { sal.NetSalary, c.CommitmentBalance, c.CommitmentNumber }).FirstOrDefault();
                    if (checkCommitment != null)
                    {
                        decimal NetSalary = checkCommitment.NetSalary;
                        if (model != null)
                        {
                            decimal allow = model.buDetail.Where(m => m.TypeId == 1).Sum(m => m.Amount) ?? 0;
                            decimal deduct = model.buDetail.Where(m => m.TypeId == 2).Sum(m => m.Amount) ?? 0;
                            NetSalary = NetSalary + allow - deduct;
                            if (NetSalary > checkCommitment.CommitmentBalance)
                                return "Employee net salary is greater than commitment available balance.";
                        }
                        else if (NetSalary > checkCommitment.CommitmentBalance)
                            return "Employee net salary is greater than commitment available balance.";
                    }
                    else
                        return "Commitment does not exists for this user " + EmployeeID;
                    return "Valid";
                }
            }
            catch (Exception ex)
            {
                return "Somenthing went wrong, Please contact administrator";
            }
        }
        public bool SLACommitmentBalanceUpdate(Int32 billId, bool revoke, bool isReversed, int uId, string tCode)
        {
            try
            {
                BOAModel model = new BOAModel();
                List<BillCommitmentDetailModel> txList = new List<BillCommitmentDetailModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from c in context.tblAgencySalaryCommitmentDetail
                                 group c by c.CommitmentDetailId into grp
                                 join det in context.tblCommitmentDetails on grp.FirstOrDefault().CommitmentDetailId equals det.ComitmentDetailId
                                 join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                                 where grp.FirstOrDefault().AgencySalaryId == billId && grp.FirstOrDefault().Status == "Active"                                 
                                 select new
                                 {
                                     detailId = grp.Key,
                                     commitmentId = com.CommitmentId,
                                     amount =  grp.Sum(m=>m.Amount)
                                 }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            txList.Add(new BillCommitmentDetailModel()
                            {
                                CommitmentDetailId = query[i].detailId,
                                PaymentAmount = query[i].amount,
                                CommitmentId = query[i].commitmentId,
                                ReversedAmount = query[i].amount
                            });
                        }
                    }
                    //txList = (from c in context.tblAgencySalaryCommitmentDetail
                    //          join det in context.tblCommitmentDetails on c.CommitmentDetailId equals det.ComitmentDetailId
                    //          join com in context.tblCommitment on det.CommitmentId equals com.CommitmentId
                    //          where c.AgencySalaryId == billId && c.Status == "Active"
                    //          select new BillCommitmentDetailModel()
                    //          {
                    //              CommitmentDetailId = c.CommitmentDetailId,
                    //              PaymentAmount = c.Amount,
                    //              CommitmentId = com.CommitmentId,
                    //              ReversedAmount = c.Amount
                    //          }).ToList();
                    if (txList.Count > 0)
                        return coreAccounts.UpdateCommitmentBalance(txList, revoke, isReversed, uId, billId, tCode);
                    else
                        return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ApproveSLA(int billId, int logged_in_user)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAgencySalary.FirstOrDefault(m => m.AgencySalaryId == billId && m.Status == "Open");
                    if (query != null)
                    {
                        query.Status = "Completed";
                        query.LastupdatedUserid = logged_in_user;
                        query.Lastupdate_TS = DateTime.Now;
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

    public class FinOp
    {
        private string StartingMonth = "Apr";
        private int StartingDate = 1;
        private string EndingMonth = "Mar";
        private int EndingDate = 31;

        private DateTime _FinStart;
        private DateTime _FinEnd;
        private DateTime _Today = DateTime.Today;

        string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        private DateTime FinStartDate()
        {
            DateTime dtResult = System.DateTime.Now;
            int month = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            if (month > CurrentMonth)
            {
                dtResult = new DateTime(_Today.Year - 1, month, StartingDate);
            }
            else
            {
                dtResult = new DateTime(_Today.Year, month, StartingDate);
            }
            return dtResult;
        }
        private DateTime FinEndDate()
        {
            DateTime dtResult = System.DateTime.Now;
            int month = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            int CurrentYear = _Today.Year;
            int nextYear = _Today.Year + 1;
            if (month >= CurrentMonth && _Today.Year == CurrentYear)
            {
                dtResult = new DateTime(_Today.Year, month, EndingDate);
            }
            else
            {
                dtResult = new DateTime(nextYear, month, EndingDate);
            }
            return dtResult;
        }
        private void FinStart(int year)
        {
            //DateTime.ParseExact(StartingMonth, "MMMM", CultureInfo.CurrentCulture).Month;
            int month = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            if (month > CurrentMonth)
            {
                _FinStart = new DateTime(year - 1, month, StartingDate);
            }
            else
            {
                _FinStart = new DateTime(year, month, StartingDate);
            }

        }
        private void FinEnd(int year)
        {
            int month = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int CurrentMonth = _Today.Month;
            int CurrentYear = _Today.Year;
            int nextYear = year + 1;
            if (month >= CurrentMonth && year == CurrentYear)
            {
                _FinEnd = new DateTime(year, month, EndingDate);
            }
            else
            {
                _FinEnd = new DateTime(nextYear, month, EndingDate);
            }
        }

        public DateTime FinStart()
        {
            return _FinStart;
        }
        public DateTime FinEnd()
        {
            return _FinEnd;
        }

        public FinOp(int year)
        {
            FinStart(year);
            FinEnd(year);
        }

        public FinOp(DateTime dt)
        {
            int year = System.DateTime.Now.Year;
            int startMonth = DateTime.ParseExact(StartingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            int endMonth = DateTime.ParseExact(EndingMonth, "MMM", CultureInfo.CurrentCulture).Month;
            if (dt.Month >= startMonth)
            {
                year = dt.Year;
            }
            else if (dt.Month < startMonth)
            {
                year = dt.Year - 1;
            }
            FinStart(year);
            FinEnd(year);
        }
        public FinOp getFinPeriod(DateTime start, DateTime end)
        {
            DateTime startDt;
            DateTime endDt;
            startDt = start;
            if (start > end)
            {
                startDt = end;
                endDt = start;
            }
            FinOp period = new FinOp(startDt);

            return period;
        }

        public Dictionary<string, string> GetAllMonths()
        {
            Dictionary<string, string> monthYear = new Dictionary<string, string>(12);

            DateTime dtStart = FinStartDate();
            DateTime dtEnd = FinEndDate();
            int prevStartYear = dtStart.Year - 1;
            int prevStartMonth = dtEnd.Month;
            int prevEndYear = dtStart.Year;
            int prevEndMonth = dtStart.Month;

            for (int i = prevStartMonth; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + prevStartYear.ToString();
                monthYear.Add(key, key);
            }
            for (int i = 1; i < prevEndMonth; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + prevEndYear.ToString();
                monthYear.Add(key, key);
            }
            for (int i = dtStart.Month; i <= 12; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + dtStart.Year.ToString();
                monthYear.Add(key, key);
            }
            for (int i = 1; i <= dtEnd.Month; i++)
            {
                var key = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + dtEnd.Year.ToString();
                monthYear.Add(key, key);
            }
            return monthYear;
        }

        public string GetCurrentMonthYear()
        {
            int month = _Today.Month;
            int year = _Today.Year;
            string currentMonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(month).Substring(0, 3) + " - " + year.ToString();

            return currentMonthYear;
        }

        public DateTime GetMonthLastDate(string MonthYear)
        {
            try
            {
                DateTime lastDate = DateTime.Now;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int year = Convert.ToInt32(dt[1].Trim());
                    lastDate = new DateTime(year, month,
                                    DateTime.DaysInMonth(year, month));
                }
                return lastDate;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        public DateTime GetMonthFirstDate(string MonthYear)
        {
            try
            {
                DateTime firstDate = DateTime.Now;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int year = Convert.ToInt32(dt[1].Trim());
                    firstDate = new DateTime(year, month, 1);
                }
                return firstDate;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        public List<KeyValuePair<string, int>> GetListMonthDays(DateTime start, DateTime end)
        {
            try
            {
                List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
                DateTime lastDate = DateTime.Now;
                DateTime tempDate = DateTime.Now;
                DateTime monthStartDate;
                DateTime monthEndDate;
                string paymonthYear = "";

                int startMonth = start.Month;
                int endMonth = end.Month;
                if (start > end)
                {
                    tempDate = start;
                    start = end;
                    end = tempDate;
                    startMonth = start.Month;
                    endMonth = end.Month;
                }
                if (start == end)
                {
                    return list;
                }

                for (int i = startMonth; i < endMonth; i++)
                {
                    monthStartDate = new DateTime(start.Year, start.Month, start.Day);
                    monthEndDate = new DateTime(start.Year, start.Month,
                                    DateTime.DaysInMonth(start.Year, start.Month));
                    if (start.Year == end.Year)
                    {

                    }

                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + start.Year.ToString();
                    int diff = (monthEndDate.Day - monthStartDate.Day);
                    var keyValue = new KeyValuePair<string, int>(paymonthYear, diff);
                    list.Add(keyValue);

                }


                //// If From Date's Day is bigger than borrow days from previous month
                //// & then subtract.
                //if (start.Day > end.Day)
                //{
                //    objDateTimeToDate = objDateTimeToDate.AddMonths(-1);
                //    int nMonthDays = DateTime.DaysInMonth(objDateTimeToDate.Year, objDateTimeToDate.Month);
                //    m_nDays = objDateTimeToDate.Day + nMonthDays - objDateTimeFromDate.Day;

                //}

                //string[] dt = MonthYear.Split('-');
                //if (dt.Length > 0)
                //{
                //    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                //    int year = Convert.ToInt32(dt[1].Trim());
                //    lastDate = new DateTime(year, month, 1);
                //}
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool IsFutureMonthYear(string MonthYear)
        {
            try
            {
                DateTime dtStart = FinStartDate();
                DateTime dtEnd = FinEndDate();

                bool isValid = false;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int pre_startMonth = DateTime.ParseExact(StartingMonth.ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int pre_EndMonth = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int nxt_startMonth = DateTime.ParseExact(EndingMonth.ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int nxt_EndMonth = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    if (_Today.Year == Convert.ToInt32(dt[1].Trim()) && month <= _Today.Month)
                    {
                        isValid = true;
                    }
                    else if (Convert.ToInt32(dt[1].Trim()) < _Today.Year && month <= 12)
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                    }

                    //int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    //if (_Today.Year <= Convert.ToInt32(dt[1].Trim()) && month <= _Today.Month)
                    //{
                    //    isValid = true;
                    //}
                }
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<KeyValuePair<int, string>> GetAllMonths(int year)
        {
            for (int i = 1; i <= 12; i++)
            {
                yield return new KeyValuePair<int, string>(i, DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + year.ToString());
            }
        }


    }

    public class DateOp
    {
        private int m_nDays = -1;
        private int m_nWeek;
        private int m_nMonth = -1;
        private int m_nYear;

        private DateTime Today = System.DateTime.Now;

        public int Days
        {
            get
            {
                return m_nDays;
            }
        }

        public int Weeks
        {
            get
            {
                return m_nWeek;
            }
        }

        public int Months
        {
            get
            {
                return m_nMonth;
            }
        }

        public int Years
        {
            get
            {
                return m_nYear;
            }
        }

        public void GetDiff(DateTime objDateTimeFromDate, DateTime objDateTimeToDate)
        {
            if (objDateTimeFromDate.Date > objDateTimeToDate.Date)
            {
                DateTime objDateTimeTemp = objDateTimeFromDate;
                objDateTimeFromDate = objDateTimeToDate;
                objDateTimeToDate = objDateTimeTemp;
            }

            if (objDateTimeFromDate == objDateTimeToDate)
            {
                //textBoxDifferenceDays.Text = " Same dates";
                //textBoxAllDifference.Text = " Same dates";
                return;
            }

            // If From Date's Day is bigger than borrow days from previous month
            // & then subtract.
            if (objDateTimeFromDate.Day > objDateTimeToDate.Day)
            {
                objDateTimeToDate = objDateTimeToDate.AddMonths(-1);
                int nMonthDays = DateTime.DaysInMonth(objDateTimeToDate.Year, objDateTimeToDate.Month);
                m_nDays = objDateTimeToDate.Day + nMonthDays - objDateTimeFromDate.Day;

            }

            // If From Date's Month is bigger than borrow 12 Month 
            // & then subtract.
            if (objDateTimeFromDate.Month > objDateTimeToDate.Month)
            {
                objDateTimeToDate = objDateTimeToDate.AddYears(-1);
                m_nMonth = objDateTimeToDate.Month + 12 - objDateTimeFromDate.Month;

            }

            //Below are best cases - simple subtraction
            if (m_nDays == -1)
            {
                m_nDays = objDateTimeToDate.Day - objDateTimeFromDate.Day;
            }

            if (m_nMonth == -1)
            {
                m_nMonth = objDateTimeToDate.Month - objDateTimeFromDate.Month;
            }

            m_nYear = objDateTimeToDate.Year - objDateTimeFromDate.Year;
            m_nWeek = m_nDays / 7;
            m_nDays = (m_nDays % 7) + (m_nWeek * 7);

            if (m_nYear > 0)
            {
                m_nMonth = m_nMonth + (m_nYear * 12);
            }
        }


        public int GetMonth(DateTime dt)
        {
            return dt.Month;
        }

        public int GetYear(DateTime dt)
        {
            return dt.Year;
        }

        public int CurrentMonth()
        {
            return Today.Month;
        }
        public int CurrentYear()
        {
            return Today.Year;
        }
        public int NextYear()
        {
            return Today.Year + 1;
        }
    }

    public class AdhocSalaryProcess
    {
        FinOp fo = new FinOp(System.DateTime.Now);
        StaffPaymentService payment = new StaffPaymentService();
        CoreAccountsService coreAccountService = new CoreAccountsService();

        string EmployeeNo { get; set; }
        string EmployeeName { get; set; }
        string DepartmentCode { get; set; }

        public void setFilter(string EmployeeNo, string EmployeeName, string DepartmentCode)
        {
            this.EmployeeNo = EmployeeNo;
            this.EmployeeName = EmployeeName;
            this.DepartmentCode = DepartmentCode;
        }

        public Dictionary<Tuple<int, int>, int> GetNumberOfDays(DateTime start, DateTime end)
        {
            // assumes end > start
            Dictionary<Tuple<int, int>, int> ret = new Dictionary<Tuple<int, int>, int>();
            DateTime date = end;
            string paymonthYear = "";

            while (date > start)
            {
                if (date.Year == start.Year && date.Month == start.Month)
                {
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).Substring(0, 3) + " - " + start.Year.ToString();
                    ret.Add(
                        Tuple.Create<int, int>(date.Year, date.Month),
                        (date - start).Days + 1);
                    break;
                }
                else
                {
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(date.Month).Substring(0, 3) + " - " + start.Year.ToString();
                    ret.Add(
                        Tuple.Create<int, int>(date.Year, date.Month),
                        date.Day);
                    date = new DateTime(date.Year, date.Month, 1).AddDays(-1);
                }
            }
            return ret;
        }

        private SalaryModel CalcTaxableIncome(SalaryModel model)
        {
            try
            {
                decimal taxable = 0;
                //decimal slab1 = 250000;
                //decimal slab2 = 500000;
                //decimal slab3 = 1000000;
                //decimal slab1Tax = 0;
                //decimal slab2Tax = 0;
                //decimal slab3Tax = 0;
                //if (taxable < 250000)
                //{
                //    model.Tax = 0;
                //}
                //else if (taxable > slab1 && taxable < slab2)
                //{
                //    model.Tax = ((taxable - slab1) / 100) * 5;
                //}
                //else if (taxable > slab1 && taxable > slab2 && taxable < slab3)
                //{
                //    slab1Tax = 0;
                //    slab2Tax = ((slab2 - slab1) / 100) * 5;
                //    slab3Tax = ((taxable - slab2) / 100) * 20;
                //    model.Tax = slab1Tax + slab2Tax + slab3Tax;
                //}
                decimal otherAllowance = 0;
                decimal taxablOA = 0;
                decimal slabAmount = 0;
                decimal taxAmount = 0;
                decimal taxableAfterSlab = 0;
                decimal RangeFrom = 0;
                decimal RangeTo = 0;
                decimal Percent = 0;
                if (model.OtherAllowance != null && model.OtherAllowance.Count > 0)
                {
                    for (int i = 0; i < model.OtherAllowance.Count; i++)
                    {
                        if (model.OtherAllowance[i].taxable == true)
                        {
                            taxablOA = taxablOA + model.OtherAllowance[i].Amount;
                        }
                        else
                        {
                            otherAllowance = otherAllowance + model.OtherAllowance[i].Amount;
                        }
                    }
                }

                if (taxablOA > 0)
                {
                    taxable = model.AnnualSalary - model.AnnualExemption + taxablOA;
                }
                else
                {
                    taxable = model.AnnualSalary - model.AnnualExemption;
                }
                if (model != null && model.taxSlab != null)
                {
                    int count = model.taxSlab.Count;

                    for (int i = 0; i < count; i++)
                    {
                        RangeFrom = model.taxSlab[i].RangeFrom;
                        RangeTo = model.taxSlab[i].RangeTo;
                        Percent = model.taxSlab[i].Percentage;
                        slabAmount = RangeTo - RangeFrom;
                        if (Percent <= 0)
                        {
                            taxableAfterSlab = taxable - RangeTo;
                        }
                        else if (taxableAfterSlab <= 0)
                        {
                            break;
                        }
                        else if (taxableAfterSlab <= RangeTo)
                        {
                            taxAmount = (taxableAfterSlab / 100) * Percent;
                            taxableAfterSlab = 0;
                        }
                        else if (RangeTo <= taxableAfterSlab && Percent > 0)
                        {
                            taxAmount = taxAmount + ((taxableAfterSlab - slabAmount) / 100 * Percent);
                            taxableAfterSlab = taxableAfterSlab - slabAmount;
                        }

                    }
                }

                model.Tax = taxAmount;
                model.TaxableIncome = taxable;
                model.MonthlyTax = model.Tax / 12;
                model.NetSalary = (model.MonthSalary - model.MonthlyTax) + otherAllowance;
                model.OtherAllowanceAmount = otherAllowance + taxablOA;

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


        public SalaryModel GetSalaryDetail(SalaryModel model)
        {
            try
            {
                decimal Basic = model.Basic;
                decimal DA = model.DA;
                decimal MA = model.MA;
                decimal Conveyance = model.Conveyance;
                decimal HRA = model.HRA;
                decimal fellowship = model.fellowship;

                DateOp dtOperation = new DateOp();
                int fy = System.DateTime.Now.Year;
                DateTime salaryStartDate;
                DateTime salaryEndDate;
                string MonthYear = model.PaymentMonthYear;
                string[] dt = MonthYear.Split('-');
                if (dt.Length > 0)
                {
                    int month = DateTime.ParseExact(dt[0].Trim().ToString(), "MMM", CultureInfo.CurrentCulture).Month;
                    int year = Convert.ToInt32(dt[1].Trim());
                    if (month < 4)
                    {
                        fy = year - 1;
                    }
                    else
                    {
                        fy = year;
                    }
                    DateTime firstDate = new DateTime(year, month, 1);
                }

                FinOp fp = new FinOp(fy);
                if (fp.FinStart() < model.FromDate)
                {
                    salaryStartDate = model.FromDate;
                }
                else
                {
                    salaryStartDate = fp.FinStart();
                }
                if (fp.FinEnd() < model.ToDate)
                {
                    salaryEndDate = fp.FinEnd();
                }
                else
                {
                    salaryEndDate = model.ToDate;
                }
                string paymonthYear = "";
                decimal amount = 0;
                decimal monthSalary = Basic + HRA + MA + Conveyance + DA + fellowship;
                decimal currentMonthSalay = 0;
                int noOfDays = 0;
                var listOfMonthDays = GetNumberOfDays(salaryStartDate, salaryEndDate);
                foreach (var m in listOfMonthDays.Keys)
                {
                    int month, year;
                    month = m.Item2;
                    year = m.Item1;
                    paymonthYear = DateTimeFormatInfo.CurrentInfo.GetMonthName(month).Substring(0, 3) + " - " + year.ToString();
                    if (model.PaymentMonthYear == paymonthYear)
                    {
                        noOfDays = listOfMonthDays[m];
                        model.NoOfDaysPresent = noOfDays;
                        if (DateTime.DaysInMonth(year, month) == listOfMonthDays[m])
                        {
                            currentMonthSalay = monthSalary;
                        }
                        else
                        {
                            currentMonthSalay = (monthSalary / 30) * listOfMonthDays[m];
                        }
                    }
                    if (DateTime.DaysInMonth(year, month) == listOfMonthDays[m])
                    {
                        amount = amount + monthSalary;
                    }
                    else
                    {
                        amount = amount + (monthSalary / 30) * listOfMonthDays[m];
                    }
                }
                dtOperation.GetDiff(salaryStartDate, salaryEndDate);
                int noOfMonths = dtOperation.Months;
                model.AnnualExemption = payment.GetITExemption(model.EmployeeId);
                model.OtherAllowance = GetEmpOtherAllowance(model.EmployeeId);
                model.MonthSalary = monthSalary;
                model.CurrentMonthSalary = currentMonthSalay;
                model.AnnualSalary = amount; //model.MonthSalary * noOfMonths;
                model.NoOfMonths = noOfMonths;
                model = CalcTaxableIncome(model);

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmployeeDepartmentModel> GetDepartments()
        {
            try
            {
                List<EmployeeDepartmentModel> model = new List<EmployeeDepartmentModel>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from E in context.VWOngoing
                                   select new
                                   {
                                       E.departmentcode,
                                       E.DEPARTMENT
                                   }).Distinct().ToList();
                    if (records != null)
                    {
                        foreach (var item in records)
                        {
                            model.Add(new EmployeeDepartmentModel
                            {
                                Code = item.departmentcode,
                                Department = item.DEPARTMENT
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


        private string GeneratePaymentNo()
        {
            try
            {
                string paymentNo = "";
                DateTime today = DateTime.Now;
                //int mon = DateTime.ParseExact(today.Month.ToString(), "MMM", CultureInfo.CurrentCulture).Month;

                //string year = Convert.ToString(today.Year);
                //string month = Convert.ToString(mon);
                //string date = Convert.ToString(today.Date);

                paymentNo = "AD-" + today.ToString("yyyyMMddHHmmssffff");

                return paymentNo;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string GetStatusFieldById(string CodeName, int statusCode)
        {
            try
            {
                string statusText = "";
                using (var context = new IOASDBEntities())
                {
                    var payType = (from PT in context.tblCodeControl
                                   where PT.CodeName == CodeName && PT.CodeValAbbr == statusCode
                                   select new
                                   {
                                       PT.CodeName,
                                       PT.CodeValAbbr,
                                       PT.CodeValDetail,
                                       PT.CodeID
                                   }).SingleOrDefault();
                    if (payType != null)
                    {
                        statusText = payType.CodeValDetail;
                    }
                }
                return statusText;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public SalaryModel GetSalaryComponent(string EmployeeId)
        {
            try
            {
                SalaryModel model = new SalaryModel();

                using (var context = new IOASDBEntities())
                {
                    var records = (from S in context.vwPaymaster
                                   join PT in context.vwPaytype on S.paytype equals PT.id
                                   where S.FileNo == EmployeeId
                                   select new
                                   {
                                       S.paytype,
                                       S.Amount,
                                       PT.ptype
                                   }).ToList();
                    if (records != null && records.Count > 0)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            string component = records[i].ptype;

                            switch (component.ToLower())
                            {
                                case "consolidatedpay":
                                    model.Basic = Convert.ToDecimal(records[i].Amount);
                                    break;
                                case "fellowship":
                                    model.fellowship = Convert.ToDecimal(records[i].Amount);
                                    break;
                                case "hra":
                                    model.HRA = Convert.ToDecimal(records[i].Amount);
                                    break;
                                case "medical":
                                    model.Medical = Convert.ToDecimal(records[i].Amount);
                                    break;
                                case "institutehospital":
                                    model.InstituteHospital = Convert.ToDecimal(records[i].Amount);
                                    break;
                                default:
                                    model.Deduction = Convert.ToDecimal(records[i].Amount);
                                    break;
                            }

                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<TaxSlab> GetTaxSlab(string gender)
        {
            try
            {
                List<TaxSlab> model = new List<TaxSlab>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from TS in context.tblTaxSlab
                                   where TS.Gender == gender
                                   select TS).ToList();

                    if (records != null)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            model.Add(new TaxSlab
                            {
                                id = records[i].id,
                                RangeFrom = Convert.ToDecimal(records[i].RangeFrom),
                                RangeTo = Convert.ToDecimal(records[i].RangeTo),
                                Percentage = Convert.ToDecimal(records[i].Percentage),
                                Gender = Convert.ToString(records[i].Gender),
                                Age = Convert.ToInt32(records[i].Age),
                                IsCurrent = Convert.ToBoolean(records[i].IsCurrent)
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public SalaryPaymentHead GetSalayPaymentHead(int PaymentHeadId)
        {
            try
            {
                SalaryPaymentHead salaryHead = new SalaryPaymentHead();
                List<AdhocEmployeeModel> employees = new List<AdhocEmployeeModel>();
                SalaryModel salary = new SalaryModel();

                using (var context = new IOASDBEntities())
                {

                    if (PaymentHeadId > 0)
                    {
                        salaryHead = context.tblSalaryPaymentHead
                            .Where(x => x.PaymentHeadId == PaymentHeadId)
                            .Select(SP => new SalaryPaymentHead
                            {
                                PaymentHeadId = SP.PaymentHeadId,
                                PaymentMonthYear = SP.PaymentMonthYear,
                                PaidDate = (DateTime)SP.PaidDate,
                                PaidAmount = (decimal)SP.PaidAmount,
                                Status = SP.Status,
                                PaymentNo = (string)(SP.PaymentNo),
                                Amount = (decimal)(SP.Amount == null ? 0 : SP.Amount),
                                TypeOfPayBill = (int)(SP.TypeOfPayBill == null ? 0 : SP.TypeOfPayBill)
                            }).SingleOrDefault();
                    }
                    var records = (from AI in context.VWOngoing
                                   join SP in context.tblSalaryPayment on AI.paybill_id equals SP.PayBillId
                                   // To check the employees to date is greater than the end of the pay bill month for main salary.
                                   where SP.PaymentHeadId == PaymentHeadId
                                   orderby AI.paybill_id
                                   select new
                                   {
                                       EmployeeID = AI.FileNo,
                                       BasicSalary = AI.BasicPay,
                                       FromDate = AI.AppointmentDate,
                                       ToDate = AI.ExtensionDate,
                                       EmployeeName = AI.NAME,
                                       AI.paybill_id,
                                       AI.paybill_no,
                                       AI.NAME,
                                       AI.DOB,
                                       AI.Gender,
                                       AI.departmentcode,
                                       AI.DEPARTMENT,
                                       AI.AppointmentDate,
                                       AI.RelieveDate,
                                       AI.ExtensionDate,
                                       AI.BasicPay,
                                       AI.HRA,
                                       AI.Medical,
                                       ProjectNo = AI.PROJECTNO,
                                       CommitmentNo = AI.commitmentNo,
                                       PaymentId = (int?)SP.PaymentId,
                                       PaymentHeadId = (int?)SP.PaymentHeadId,
                                       IsPaid = (bool?)SP.IsPaid,
                                       SalaryDetails = SP
                                   }).ToList();

                    if (records != null)
                    {
                        for (int i = 0; i < records.Count; i++)
                        {
                            salary.Basic = Convert.ToDecimal(records[i].SalaryDetails.Basic);
                            salary.AnnualSalary = Convert.ToDecimal(records[i].SalaryDetails.AnnualSalary);
                            salary.MonthSalary = Convert.ToDecimal(records[i].SalaryDetails.MonthSalary);
                            salary.Tax = Convert.ToDecimal(records[i].SalaryDetails.Tax);
                            salary.TaxableIncome = Convert.ToDecimal(records[i].SalaryDetails.TaxableIncome);
                            salary.MonthlyTax = Convert.ToDecimal(records[i].SalaryDetails.MonthlyTax);
                            salary.NetSalary = Convert.ToDecimal(records[i].SalaryDetails.NetSalary);
                            salary.ModeOfPayment = Convert.ToInt32(records[i].SalaryDetails.ModeOfPayment);
                            salary.CurrentMonthSalary = Convert.ToInt32(records[i].SalaryDetails.MonthSalary);
                            employees.Add(new AdhocEmployeeModel
                            {
                                MakePayment = true,
                                EmployeeID = records[i].EmployeeID,
                                EmployeeName = records[i].EmployeeName,
                                AppointmentDate = Convert.ToDateTime(records[i].AppointmentDate),
                                ToDate = Convert.ToDateTime(records[i].ToDate),
                                RelieveDate = Convert.ToDateTime(records[i].RelieveDate),
                                BasicPay = Convert.ToDecimal(salary.Basic),
                                PROJECTNO = records[i].ProjectNo,
                                FromDate = Convert.ToDateTime(records[i].FromDate),
                                commitmentNo = records[i].CommitmentNo,
                                SalaryDetail = salary,
                                ModeOfPayment = (int)records[i].SalaryDetails.ModeOfPayment
                            });
                        }
                    }
                    salaryHead.AdhocEmployees = employees;
                }

                return salaryHead;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public AdhocEmployeeModel GetAnEmployeeDetails(string EmployeeId, string PaymentMonthYear)
        {
            try
            {
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    DateTime lastDate = fo.GetMonthLastDate(PaymentMonthYear);
                    DateTime startDate = fo.GetMonthFirstDate(PaymentMonthYear);

                    var record = (from AI in context.VWOngoing
                                  join SP in context.tblSalaryPayment on AI.paybill_id equals SP.PayBillId into SalDet
                                  from E in (from ED in SalDet
                                             where ED.PaymentMonthYear == PaymentMonthYear && ED.EmployeeId == AI.FileNo
                                             select ED).DefaultIfEmpty()
                                      // To check the employees to date is greater than the end of the pay bill month for main salary.
                                  where //AI.ExtensionDate >= lastDate && AI.AppointmentDate < startDate && 
                                    AI.FileNo == EmployeeId
                                  orderby AI.paybill_id
                                  select new
                                  {
                                      EmployeeID = AI.FileNo,
                                      BasicSalary = AI.BasicPay,
                                      FromDate = AI.AppointmentDate,
                                      ToDate = AI.ExtensionDate,
                                      EmployeeName = AI.NAME,
                                      AI.paybill_id,
                                      AI.paybill_no,
                                      AI.NAME,
                                      AI.DOB,
                                      AI.Gender,
                                      AI.departmentcode,
                                      AI.DEPARTMENT,
                                      AI.AppointmentDate,
                                      AI.RelieveDate,
                                      AI.ExtensionDate,
                                      AI.BasicPay,
                                      AI.HRA,
                                      AI.Medical,
                                      ProjectNo = AI.PROJECTNO,
                                      CommitmentNo = AI.commitmentNo,
                                      PaymentId = (int?)E.PaymentId,
                                      PaymentHeadId = (int?)E.PaymentHeadId,
                                      IsPaid = (bool?)E.IsPaid
                                  }).SingleOrDefault();

                    if (record != null)
                    {
                        SalaryModel salaryModel = new SalaryModel();
                        var gender = "";
                        if (record.Gender != null && record.Gender.ToLower() == "m")
                        {
                            gender = "Male";
                        }
                        else if (record.Gender != null && record.Gender.ToLower() == "f")
                        {
                            gender = "Female";
                        }
                        salaryModel = GetSalaryComponent(record.EmployeeID);
                        if (salaryModel == null)
                        {
                            salaryModel.Basic = Convert.ToDecimal(record.BasicSalary);
                            salaryModel.HRA = 0;
                            salaryModel.MA = 0;
                            salaryModel.DA = 0;
                            salaryModel.Conveyance = 0;
                        }
                        salaryModel.paybill_id = record.paybill_id;
                        salaryModel.paybill_no = record.paybill_no;
                        salaryModel.EmployeeId = record.EmployeeID;
                        salaryModel.FromDate = Convert.ToDateTime(record.FromDate);
                        salaryModel.ToDate = Convert.ToDateTime(record.ToDate);
                        salaryModel.taxSlab = GetTaxSlab(gender);
                        salaryModel.PaymentMonthYear = PaymentMonthYear;

                        var salary = GetSalaryDetail(salaryModel);
                        salary.PaymentId = Convert.ToInt32(record.PaymentId);
                        salary.IsPaid = Convert.ToBoolean(record.IsPaid);
                        salary.ProjectNo = record.ProjectNo;
                        salary.paybill_id = record.paybill_id;
                        salary.paybill_no = record.paybill_no;
                        salary.PaymentMonthYear = PaymentMonthYear;
                        salary.taxSlab = null;


                        model.MakePayment = true;
                        model.EmployeeID = record.EmployeeID;
                        model.EmployeeName = record.EmployeeName;
                        model.AppointmentDate = Convert.ToDateTime(record.AppointmentDate);
                        model.ToDate = Convert.ToDateTime(record.ToDate);
                        model.RelieveDate = Convert.ToDateTime(record.RelieveDate);
                        model.BasicPay = Convert.ToDecimal(salaryModel.Basic);
                        model.PROJECTNO = record.ProjectNo;
                        model.FromDate = Convert.ToDateTime(record.FromDate);
                        model.commitmentNo = record.CommitmentNo;
                        model.SalaryDetail = salary;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public AdhocEmployeeModel GetAnEmployeeDetailsByPaymentHeadId(string EmployeeId, int PaymentHeadId)
        {
            try
            {
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    var record = (from AI in context.VWOngoing
                                  join SP in context.tblSalaryPayment on AI.FileNo equals SP.EmployeeId
                                  // To check the employees to date is greater than the end of the pay bill month for main salary.
                                  where SP.PaymentHeadId == PaymentHeadId && AI.FileNo == EmployeeId
                                  orderby AI.paybill_id
                                  select new
                                  {
                                      EmployeeID = AI.FileNo,
                                      BasicSalary = AI.BasicPay,
                                      FromDate = AI.AppointmentDate,
                                      ToDate = AI.ExtensionDate,
                                      EmployeeName = AI.NAME,
                                      AI.paybill_id,
                                      AI.paybill_no,
                                      AI.NAME,
                                      AI.DOB,
                                      AI.Gender,
                                      AI.departmentcode,
                                      AI.DEPARTMENT,
                                      AI.AppointmentDate,
                                      AI.RelieveDate,
                                      AI.ExtensionDate,
                                      AI.BasicPay,
                                      AI.HRA,
                                      AI.Medical,
                                      ProjectNo = AI.PROJECTNO,
                                      CommitmentNo = AI.commitmentNo,
                                      PaymentId = (int?)SP.PaymentId,
                                      PaymentHeadId = (int?)SP.PaymentHeadId,
                                      IsPaid = (bool?)SP.IsPaid,
                                      SP.PaymentMonthYear
                                  }).SingleOrDefault();

                    if (record != null)
                    {
                        SalaryModel salaryModel = new SalaryModel();
                        var gender = "";
                        if (record.Gender != null && record.Gender.ToLower() == "m")
                        {
                            gender = "Male";
                        }
                        else if (record.Gender != null && record.Gender.ToLower() == "f")
                        {
                            gender = "Female";
                        }
                        salaryModel = GetSalaryComponent(record.EmployeeID);
                        if (salaryModel == null)
                        {
                            salaryModel.Basic = Convert.ToDecimal(record.BasicSalary);
                            salaryModel.HRA = 0;
                            salaryModel.MA = 0;
                            salaryModel.DA = 0;
                            salaryModel.Conveyance = 0;
                        }
                        salaryModel.paybill_id = record.paybill_id;
                        salaryModel.paybill_no = record.paybill_no;
                        salaryModel.EmployeeId = record.EmployeeID;
                        salaryModel.FromDate = Convert.ToDateTime(record.FromDate);
                        salaryModel.ToDate = Convert.ToDateTime(record.ToDate);
                        salaryModel.taxSlab = GetTaxSlab(gender);
                        salaryModel.PaymentMonthYear = record.PaymentMonthYear;

                        var salary = GetSalaryDetail(salaryModel);
                        salary.PaymentId = Convert.ToInt32(record.PaymentId);
                        salary.IsPaid = Convert.ToBoolean(record.IsPaid);
                        model.MakePayment = true;
                        model.EmployeeID = record.EmployeeID;
                        model.EmployeeName = record.EmployeeName;
                        model.AppointmentDate = Convert.ToDateTime(record.AppointmentDate);
                        model.ToDate = Convert.ToDateTime(record.ToDate);
                        model.RelieveDate = Convert.ToDateTime(record.RelieveDate);
                        model.BasicPay = Convert.ToDecimal(salaryModel.Basic);
                        model.PROJECTNO = record.ProjectNo;
                        model.FromDate = Convert.ToDateTime(record.FromDate);
                        model.commitmentNo = record.CommitmentNo;
                        model.SalaryDetail = salary;
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedData<SalaryPaymentHead> ListSalayPayment(int page, int pageSize)
        {
            try
            {

                int skiprec = 0;

                if (page == 1)
                {
                    skiprec = 0;
                }
                else
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedData<SalaryPaymentHead>();
                List<SalaryPaymentHead> salaryHead = new List<SalaryPaymentHead>();

                decimal totalSalary = 0;

                using (var context = new IOASDBEntities())
                {
                    var query = (from SP in context.tblSalaryPaymentHead
                                 join CC in context.tblCodeControl on SP.TypeOfPayBill equals CC.CodeValAbbr
                                 where CC.CodeName == "PayOfBill"
                                 orderby SP.PaymentHeadId
                                 select new
                                 {
                                     PaymentHeadId = SP.PaymentHeadId,
                                     PaymentNo = (string)(SP.PaymentNo),
                                     Amount = (decimal)(SP.Amount == null ? 0 : SP.Amount),
                                     TypeOfPayBill = (int)(SP.TypeOfPayBill == null ? 0 : SP.TypeOfPayBill),
                                     TypeOfPayBillText = CC.CodeValDetail,
                                     PaidDate = (DateTime)SP.PaidDate,
                                     Status = SP.Status,
                                     PaymentMonthYear = SP.PaymentMonthYear,
                                     ProjectNo = SP.ProjectNo
                                 });
                    var records = query.Skip(skiprec).Take(pageSize).ToList();
                    var recordCount = query.ToList().Count();
                    if (recordCount > 0)
                    {
                        for (var k = 0; k < records.Count; k++)
                        {
                            salaryHead.Add(new SalaryPaymentHead
                            {
                                PaymentHeadId = records[k].PaymentHeadId,
                                PaymentNo = records[k].PaymentNo,
                                Amount = records[k].Amount,
                                TypeOfPayBill = records[k].TypeOfPayBill,
                                PaidDate = records[k].PaidDate,
                                PaymentMonthYear = records[k].PaymentMonthYear,
                                ProjectNo = records[k].ProjectNo,
                                Status = records[k].Status,
                                TypeOfPayBillText = records[k].TypeOfPayBillText
                            });
                        }
                        searchData.Data = salaryHead;
                        searchData.TotalRecords = recordCount;
                        searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    }

                }


                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public SalaryTransaction GetSalaryTransaction(int PaymentHeadId, string PaymentMonthYear)
        {
            try
            {
                CoreAccountsService coreAccountService = new CoreAccountsService();
                SalaryTransaction model = new SalaryTransaction();
                var tx = coreAccountService.GetTransactionDetails(0, false, "SAL", "1", false, null);
                model.ExpenseDetail = tx.ExpenseDetail;
                model.DeductionDetail = tx.DeductionDetail;

                using (var context = new IOASDBEntities())
                {
                    var record = (from ST in context.tblSalaryTransaction
                                  where ST.PaymentHeadId == PaymentHeadId
                                  select ST).SingleOrDefault();
                    if (record != null)
                    {
                        model.TotalCredit = Convert.ToDecimal(record.TotalCredit);
                        model.TotalDebit = Convert.ToDecimal(record.TotalDebit);

                        model.PaymentHeadId = record.PaymentHeadId;
                        model.TransactionId = record.TransactionId;
                        model.PaymentNo = record.PaymentNo;
                        model.PostedDate = Convert.ToDateTime(record.PostedDate);
                        model.ApprovedDate = Convert.ToDateTime(record.ApprovedDate);
                        model.SalaryType = Convert.ToInt32(record.SalaryType);
                        model.TotalAmount = Convert.ToDecimal(record.TotalAmount);
                        model.TotalTaxAmount = Convert.ToDecimal(record.TotalTaxAmount);
                        model.CommitmentAmount = Convert.ToDecimal(record.CommitmentAmount);
                        model.ExpenseAmount = Convert.ToDecimal(record.ExpenseAmount);
                        model.DeductionAmount = Convert.ToDecimal(record.DeductionAmount);

                        model.Status = record.Status;
                        model.PaymentType = Convert.ToInt32(record.PaymentType);
                    }

                    var records = (from STD in context.tblSalaryTransactionDetail
                                   where STD.PaymentHeadId == PaymentHeadId
                                   select STD).ToList();
                    if (records != null)
                    {
                        List<SalaryTransactionDetail> detail = new List<SalaryTransactionDetail>();

                        for (var i = 0; i < records.Count; i++)
                        {
                            detail.Add(new SalaryTransactionDetail
                            {
                                TransactionDetailId = records[i].TransactionDetailId,
                                TransactionId = records[i].TransactionId,
                                PaymentHeadId = records[i].PaymentHeadId,
                                AccountGroupId = Convert.ToInt32(records[i].AccountGroupId),
                                AccountHeadId = Convert.ToInt32(records[i].AccountHeadId),
                                PaymentNo = records[i].PaymentNo,
                                PostedDate = Convert.ToDateTime(records[i].PostedDate),
                                SalaryType = Convert.ToInt32(records[i].SalaryType),
                                Amount = Convert.ToDecimal(records[i].Amount),
                                TransactionType = Convert.ToString(records[i].TransactionType),
                                Status = record.Status,
                                PaymentType = Convert.ToInt32(records[i].PaymentType)
                            });
                            model.ExpenseDetail[i].Amount = records[i].Amount;
                        }

                        model.detail = detail;
                    }

                    var paymentHead = context.tblSalaryPaymentHead
                        .Where(x => x.PaymentHeadId == PaymentHeadId)
                        .Select(SP => new SalaryPaymentHead
                        {
                            PaymentHeadId = SP.PaymentHeadId,
                            PaymentNo = (string)(SP.PaymentNo),
                            Amount = (decimal)(SP.Amount == null ? 0 : SP.Amount),
                            TypeOfPayBill = (int)(SP.TypeOfPayBill == null ? 0 : SP.TypeOfPayBill)
                            //PaidDate = (DateTime)SP.PaidDate,
                            //PaymentMonthYear = SP.PaymentMonthYear,
                            //ProjectNo = SP.ProjectNo
                        }).SingleOrDefault();

                    if (paymentHead != null)
                    {
                        model.PaymentHeadId = paymentHead.PaymentHeadId;
                        model.PaymentNo = paymentHead.PaymentNo;
                        model.Amount = paymentHead.Amount;
                        model.TypeOfPayBill = paymentHead.TypeOfPayBill;
                    }
                }


                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public string SalaryTransactionIU(SalaryTransaction HeaderModel, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {

                    tblSalaryPayment salary = new tblSalaryPayment();
                    tblSalaryTransaction trans = new tblSalaryTransaction();
                    tblSalaryTransactionDetail detail = new tblSalaryTransactionDetail();
                    var transactionId = 0;
                    var salaryTransaction = context.tblSalaryTransaction
                        .SingleOrDefault(it => it.PaymentHeadId == HeaderModel.PaymentHeadId);
                    if (salaryTransaction != null && salaryTransaction.PaymentHeadId > 0)
                    {

                        salaryTransaction.PaymentHeadId = HeaderModel.PaymentHeadId;
                        salaryTransaction.PaymentNo = HeaderModel.PaymentNo;
                        //salaryTransaction.TransactionTypeCode = HeaderModel.TransactionTypeCode;
                        salaryTransaction.PostedDate = Convert.ToDateTime(HeaderModel.PaidDate);
                        //salaryTransaction.ApprovedDate = HeaderModel.;
                        salaryTransaction.TotalAmount = HeaderModel.Amount;
                        salaryTransaction.TotalCredit = HeaderModel.TotalCredit;
                        salaryTransaction.TotalDebit = HeaderModel.TotalDebit;
                        //salaryTransaction.TotalTaxAmount = HeaderModel.Amount;
                        salaryTransaction.UpdatedAt = DateTime.Now;
                        salaryTransaction.UpdatedBy = userId;
                        salaryTransaction.Status = "open";
                        context.SaveChanges();
                        transactionId = trans.TransactionId;
                    }
                    else
                    {
                        trans.PaymentHeadId = HeaderModel.PaymentHeadId;
                        //trans.PaymentNo = HeaderModel.PaymentNo;
                        //trans.TransactionTypeCode = HeaderModel.TransactionTypeCode;
                        //trans.PostedDate = Convert.ToDateTime(HeaderModel.PaidDate);
                        //trans.ApprovedDate = HeaderModel.;
                        trans.TotalAmount = HeaderModel.Amount;
                        trans.TotalCredit = HeaderModel.TotalCredit;
                        trans.TotalDebit = HeaderModel.TotalDebit;
                        //trans.TotalTaxAmount = HeaderModel.Amount;
                        trans.UpdatedAt = DateTime.Now;
                        trans.CreatedBy = userId;
                        trans.Status = "open";
                        context.tblSalaryTransaction.Add(trans);
                        context.SaveChanges();
                        transactionId = trans.TransactionId;
                    }

                    var details = HeaderModel.ExpenseDetail;
                    for (int i = 0; i < details.Count; i++)
                    {
                        detail.PaymentHeadId = HeaderModel.PaymentHeadId;
                        detail.AccountGroupId = details[i].AccountGroupId;
                        detail.AccountHeadId = details[i].AccountHeadId;
                        detail.Amount = details[i].Amount;
                        detail.TransactionId = transactionId;
                        detail.TransactionType = details[i].TransactionType;
                        context.tblSalaryTransactionDetail.Add(detail);
                        context.SaveChanges();
                    }

                    context.Dispose();

                }
                string msg = "Salary details saved successfully";
                return msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public int EmployeeSalaryIU(SalaryPaymentHead HeaderModel, int userId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    List<SalaryModel> SalaryList = new List<SalaryModel>();
                    SalaryList = HeaderModel.Salary;
                    tblSalaryPayment salary = new tblSalaryPayment();
                    tblSalaryPaymentHead salaryHead = new tblSalaryPaymentHead();
                    if (HeaderModel.PaymentHeadId > 0)
                    {
                        var paymentHead = context.tblSalaryPaymentHead
                            .SingleOrDefault(it => it.PaymentHeadId == HeaderModel.PaymentHeadId);
                        paymentHead.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                        paymentHead.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                        paymentHead.ProjectNo = HeaderModel.ProjectNo;
                        paymentHead.CommitmentNo = HeaderModel.CommitmentNo;
                        paymentHead.Amount = HeaderModel.Amount;
                        paymentHead.PaidAmount = HeaderModel.PaidAmount;
                        paymentHead.UpdatedAt = DateTime.Now;
                        paymentHead.UpdatedBy = userId;
                        //paymentHead.Status = HeaderModel.Status;
                        context.SaveChanges();
                        HeaderModel.PaymentNo = paymentHead.PaymentNo;
                    }
                    else
                    {
                        var paymentNo = GeneratePaymentNo();
                        salaryHead.PaymentNo = paymentNo;
                        salaryHead.CommitmentNo = HeaderModel.CommitmentNo;
                        salaryHead.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                        salaryHead.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                        salaryHead.ProjectNo = HeaderModel.ProjectNo;
                        salaryHead.Amount = HeaderModel.Amount;
                        salaryHead.PaidAmount = HeaderModel.PaidAmount;
                        salaryHead.PaidDate = DateTime.Now;
                        salaryHead.CreatedAt = DateTime.Now;
                        salaryHead.CreatedBy = userId;
                        salaryHead.Status = "open";
                        context.tblSalaryPaymentHead.Add(salaryHead);
                        context.SaveChanges();
                        HeaderModel.PaymentHeadId = salaryHead.PaymentHeadId;
                        HeaderModel.PaymentNo = salaryHead.PaymentNo;
                    }
                    SalaryModel model = new SalaryModel();
                    var paybill = GetStatusFieldById("PayOfBill", HeaderModel.TypeOfPayBill);
                    if (HeaderModel.AdhocEmployees != null)
                    {
                        var employee = new AdhocEmployeeModel();
                        for (int i = 0; i < HeaderModel.AdhocEmployees.Count; i++)
                        {
                            if (HeaderModel.AdhocEmployees[i].MakePayment == true)
                            {
                                if (HeaderModel.PaymentMonthYear != null && paybill.ToLower() == "supplementary")
                                {
                                    employee = GetSubSalaryEmployee(HeaderModel.AdhocEmployees[i].EmployeeID, HeaderModel.PaymentMonthYear);
                                }
                                else
                                {
                                    employee = GetAnEmployeeDetails(HeaderModel.AdhocEmployees[i].EmployeeID, HeaderModel.PaymentMonthYear);
                                }
                                model = employee.SalaryDetail;
                                var modeOfPyament = HeaderModel.AdhocEmployees[i].ModeOfPayment;
                                var record = context.tblSalaryPayment
                                    .SingleOrDefault(it => it.PaymentHeadId == HeaderModel.PaymentHeadId && it.EmployeeId == model.EmployeeId);

                                if (record != null)
                                {
                                    record.EmpNo = model.EmpNo;
                                    record.EmployeeId = model.EmployeeId;
                                    record.ProjectNo = model.ProjectNo;
                                    record.Basic = Convert.ToDecimal(model.Basic);
                                    record.HRA = Convert.ToDecimal(model.HRA);
                                    record.MA = Convert.ToDecimal(model.MA);
                                    record.DA = Convert.ToDecimal(model.DA);
                                    record.Conveyance = Convert.ToDecimal(model.Conveyance);
                                    record.Deduction = Convert.ToDecimal(model.Deduction);
                                    record.Tax = Convert.ToDecimal(model.Tax);
                                    record.ProfTax = Convert.ToDecimal(model.ProfTax);
                                    record.AnnualSalary = Convert.ToDecimal(model.AnnualSalary);
                                    record.TaxableIncome = Convert.ToDecimal(model.TaxableIncome);
                                    record.NetSalary = Convert.ToDecimal(model.NetSalary);
                                    record.MonthSalary = Convert.ToDecimal(model.CurrentMonthSalary);
                                    record.CurrentMonthSalary = Convert.ToDecimal(model.CurrentMonthSalary);
                                    record.OtherAllowance = Convert.ToDecimal(model.OtherAllowanceAmount);
                                    record.MonthlyTax = Convert.ToDecimal(model.MonthlyTax);
                                    record.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                                    record.ModeOfPayment = modeOfPyament;
                                    record.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                                    record.PayBillId = model.paybill_id;
                                    record.PaidDate = DateTime.Now;
                                    record.UpdatedAt = DateTime.Now;
                                    record.UpdatedBy = userId;
                                    context.SaveChanges();
                                }
                                else
                                {
                                    salary.EmpNo = model.EmpNo;
                                    salary.EmployeeId = model.EmployeeId;
                                    salary.ProjectNo = model.ProjectNo;
                                    salary.Basic = Convert.ToDecimal(model.Basic);
                                    salary.HRA = Convert.ToDecimal(model.HRA);
                                    salary.MA = Convert.ToDecimal(model.MA);
                                    salary.DA = Convert.ToDecimal(model.DA);
                                    salary.Conveyance = Convert.ToDecimal(model.Conveyance);
                                    salary.Deduction = Convert.ToDecimal(model.Deduction);
                                    salary.Tax = Convert.ToDecimal(model.Tax);
                                    salary.ProfTax = Convert.ToDecimal(model.ProfTax);
                                    salary.TaxableIncome = Convert.ToDecimal(model.TaxableIncome);
                                    salary.AnnualSalary = Convert.ToDecimal(model.AnnualSalary);
                                    salary.NetSalary = Convert.ToDecimal(model.NetSalary);
                                    salary.MonthSalary = Convert.ToDecimal(model.CurrentMonthSalary);
                                    salary.CurrentMonthSalary = Convert.ToDecimal(model.CurrentMonthSalary);
                                    salary.OtherAllowance = Convert.ToDecimal(model.OtherAllowanceAmount);
                                    salary.MonthlyTax = Convert.ToDecimal(model.MonthlyTax);
                                    salary.PaymentMonthYear = HeaderModel.PaymentMonthYear;
                                    salary.ModeOfPayment = modeOfPyament;
                                    salary.TypeOfPayBill = HeaderModel.TypeOfPayBill;
                                    salary.PayBillId = model.paybill_id;

                                    salary.PaidDate = System.DateTime.Now;
                                    salary.CreatedAt = System.DateTime.Now;
                                    salary.CreatedBy = userId;
                                    salary.Status = "open";
                                    salary.PaymentHeadId = HeaderModel.PaymentHeadId;
                                    context.tblSalaryPayment.Add(salary);
                                    context.SaveChanges();
                                    model.PaymentId = salary.PaymentId;
                                }



                                var otherPayment = (from OA in context.tblEmpOtherAllowance
                                                    where OA.EmployeeIdStr == model.EmployeeId &&
                                                    (OA.IsPaid == false || OA.PaymentNo == null)
                                                    select OA).ToList();

                                if (otherPayment != null)
                                {
                                    for (int j = 0; j < otherPayment.Count; j++)
                                    {
                                        otherPayment[j].PaymentHeadId = HeaderModel.PaymentHeadId;
                                        otherPayment[j].PaymentNo = HeaderModel.PaymentNo;
                                        context.SaveChanges();
                                    }
                                }

                            }
                        }
                    }


                    var totalAmount = (from SP in context.tblSalaryPayment
                                       where SP.PaymentHeadId == HeaderModel.PaymentHeadId
                                       select new
                                       {
                                           SP.PaymentHeadId,
                                           SP.MonthSalary,
                                           SP.CurrentMonthSalary
                                       }).Sum(t => t.MonthSalary);

                    var payhead = context.tblSalaryPaymentHead.SingleOrDefault(x => x.PaymentHeadId == HeaderModel.PaymentHeadId);
                    if (payhead != null)
                    {
                        payhead.Amount = totalAmount;
                        context.SaveChanges();
                    }


                    context.Dispose();

                }
                return HeaderModel.PaymentHeadId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        public int RemoveVerifiedEmployee(int PaymentHeadId, string EmployeeId, int userId, bool verify)
        {
            try
            {
                int code = 0;
                using (var context = new IOASDBEntities())
                {
                    var record = context.tblSalaryPayment.Where(x => x.PaymentHeadId == PaymentHeadId && x.EmployeeId == EmployeeId).SingleOrDefault();
                    if (record != null)
                    {
                        context.tblSalaryPayment.Remove(record);
                        context.SaveChanges();


                        var otherPayment = (from OA in context.tblEmpOtherAllowance
                                            where OA.EmployeeIdStr == EmployeeId &&
                                            (OA.IsPaid == false || OA.PaymentNo == null)
                                            select OA).ToList();

                        if (otherPayment != null)
                        {
                            for (int j = 0; j < otherPayment.Count; j++)
                            {
                                otherPayment[j].PaymentHeadId = null;
                                otherPayment[j].PaymentNo = null;
                                context.SaveChanges();
                            }
                        }
                        var totalAmount = (from SP in context.tblSalaryPayment
                                           where SP.PaymentHeadId == PaymentHeadId
                                           select new
                                           {
                                               SP.PaymentHeadId,
                                               SP.MonthSalary,
                                               SP.CurrentMonthSalary
                                           }).Sum(t => t.MonthSalary);
                        var payhead = context.tblSalaryPaymentHead.SingleOrDefault(x => x.PaymentHeadId == PaymentHeadId);
                        if (payhead != null)
                        {
                            payhead.Amount = totalAmount;
                            payhead.UpdatedBy = userId;
                            payhead.UpdatedAt = DateTime.Now;
                            context.SaveChanges();
                        }
                    }

                    context.Dispose();
                    code = 1;
                }

                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
        public List<AccountHeadViewModel> GetAccountHead(string input)
        {
            List<AccountHeadViewModel> head = new List<AccountHeadViewModel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    head = (from AH in context.tblAccountHead
                            where AH.AccountHeadCode == input
                            select new AccountHeadViewModel
                            {
                                AccountHeadId = AH.AccountHeadId,
                                AccountHeadCode = AH.AccountHeadCode,
                                AccountHead = AH.AccountHead
                            }).ToList();

                }
                return head;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return head;
            }
        }

        public List<Accountgroupmodel> GetAccountGroup(string input)
        {
            List<Accountgroupmodel> group = new List<Accountgroupmodel>();

            try
            {
                using (var context = new IOASDBEntities())
                {
                    group = (from AG in context.tblAccountGroup
                             where AG.AccountGroupCode == input
                             select new Accountgroupmodel
                             {
                                 AccountGroupId = AG.AccountGroupId,
                                 AccountGroupCode = AG.AccountGroupCode,
                                 AccountGroup = AG.AccountGroup
                             }).ToList();

                }
                return group;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return group;
            }
        }

        public string UpdateSalaryPayment(int PaymentHeadId, string currentStatus, string newStatus, int userId)
        {
            try
            {
                string msg = "";
                using (var context = new IOASDBEntities())
                {


                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {


                            var pyamentHead = (from PH in context.tblSalaryPaymentHead
                                               where PH.PaymentHeadId == PaymentHeadId && PH.Status == currentStatus
                                               select PH).SingleOrDefault();
                            if (pyamentHead != null)
                            {
                                pyamentHead.Status = newStatus;
                                pyamentHead.UpdatedBy = userId;
                                pyamentHead.UpdatedAt = DateTime.Now;
                                context.SaveChanges();


                                var Process = ProcessEngineService.GetProcessFlowByName("Salary Approval");
                                int processGuideLineId = Process.ProcessGuidelineId;
                                var fe = FlowEngine.Init(processGuideLineId, userId, PaymentHeadId, "PaymentHeadId");
                                fe.ProcessInit();
                            }

                            var salaryPayment = (from SP in context.tblSalaryPayment
                                                 where SP.PaymentHeadId == PaymentHeadId && SP.Status == currentStatus
                                                 select SP).ToList();
                            if (salaryPayment != null)
                            {
                                for (int i = 0; i < salaryPayment.Count; i++)
                                {
                                    salaryPayment[i].Status = newStatus;
                                    salaryPayment[i].UpdatedBy = userId;
                                    salaryPayment[i].UpdatedAt = DateTime.Now;
                                    context.SaveChanges();
                                }

                                var salaryTransaction = context.tblSalaryTransaction
                                    .SingleOrDefault(it => it.PaymentHeadId == PaymentHeadId && it.Status == currentStatus);
                                if (salaryTransaction != null && salaryTransaction.PaymentHeadId > 0)
                                {
                                    salaryTransaction.UpdatedAt = DateTime.Now;
                                    salaryTransaction.UpdatedBy = userId;
                                    salaryTransaction.Status = newStatus;
                                    context.SaveChanges();
                                }
                                var transDetails = (from det in context.tblSalaryTransactionDetail
                                                    where det.PaymentHeadId == PaymentHeadId && det.Status == currentStatus
                                                    select det).ToList();

                                for (int i = 0; i < transDetails.Count; i++)
                                {
                                    transDetails[i].Status = newStatus;
                                    transDetails[i].UpdatedBy = userId;
                                    transDetails[i].UpdatedAt = DateTime.Now;
                                    context.SaveChanges();
                                }
                                var sumOfSalary = (from SP in context.tblSalaryPayment
                                                   where SP.PaymentHeadId == PaymentHeadId
                                                   group SP by new
                                                   {
                                                       CommitmentNo = SP.CommitmentNo,
                                                       SP.Basic,
                                                       SP.Conveyance,
                                                       SP.DA,
                                                       SP.HRA,
                                                       SP.MA
                                                   } into Sal
                                                   select new
                                                   {
                                                       CommitmentNo = Sal.Key.CommitmentNo,
                                                       SalaryPaid = Sal.Sum(i => (i.Basic + i.Conveyance + i.DA + i.HRA + i.MA))
                                                   }).ToList();

                                var CommitmentDetails = (from CB in context.tblCommitment
                                                         join CBD in context.tblCommitmentDetails on CB.CommitmentId equals CBD.CommitmentId
                                                         join AE in context.VWOngoing on CB.CommitmentNumber equals AE.commitmentNo
                                                         join SP in context.tblSalaryPayment on AE.FileNo equals SP.EmployeeId
                                                         where SP.PaymentHeadId == PaymentHeadId
                                                         select new BillCommitmentDetailModel
                                                         {
                                                             CommitmentId = CBD.ComitmentDetailId,
                                                             CommitmentDetailId = CBD.CommitmentId,
                                                             PaymentAmount = 0,
                                                             ReversedAmount = 0
                                                         }).ToList();
                                if (sumOfSalary != null && CommitmentDetails != null)
                                {
                                    for (int i = 0; i < sumOfSalary.Count; i++)
                                    {
                                        for (int j = 0; j < CommitmentDetails.Count; j++)
                                        {
                                            if (CommitmentDetails[j].CommitmentNumber == sumOfSalary[i].CommitmentNo)
                                            {
                                                CommitmentDetails[j].PaymentAmount = sumOfSalary[i].SalaryPaid;
                                            }
                                        }
                                    }
                                    bool result = false;
                                    if (newStatus == "Approval Pending")
                                    {
                                        result = coreAccountService.UpdateCommitmentBalance(CommitmentDetails, false, false, userId, PaymentHeadId, "SAL");
                                    }


                                }
                            }

                            //context.Dispose();
                            msg = "Updated successfully";
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return ex.ToString();
                        }
                    }


                }

                return msg;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //transaction.Rollback();
                return ex.ToString();
            }
        }


        public List<AdhocEmployeeModel> GetEmployeeList()
        {
            try
            {
                List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
                using (var context = new IOASDBEntities())
                {
                    list = (from emp in context.VWOngoing
                            select new AdhocEmployeeModel
                            {
                                EmployeeID = emp.FileNo,
                                EmployeeName = emp.NAME,

                            }).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public AdhocEmployeeModel GetEmployeeByEmpId(string EmployeeId)
        {
            try
            {
                AdhocEmployeeModel model = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    var record = (from AI in context.VWOngoing
                                  where AI.FileNo == EmployeeId
                                  orderby AI.FileNo
                                  select new
                                  {
                                      AI.FileNo,
                                      AI.NAME,
                                      AI.DOB,
                                      AI.DesignationCode,
                                      AI.Designation,
                                      AI.commitmentNo
                                  }).SingleOrDefault();
                    if (record != null)
                    {
                        model.EmployeeID = record.FileNo;
                        model.EmployeeName = record.NAME;
                        model.DesignationCode = record.DesignationCode;
                        model.Designation = record.Designation;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedList<AdhocEmployeeModel> GetEmployeesByPaymentHead(int PaymentHeadId, int page, int pageSize)
        {
            try
            {
                int skiprec = 0;

                if (page > 1)
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedList<AdhocEmployeeModel>();

                List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
                AdhocEmployeeModel salary = new AdhocEmployeeModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from emp in context.VWOngoing
                                 join SP in context.tblSalaryPayment on emp.FileNo equals SP.EmployeeId //into SalDet
                                 where SP.PaymentHeadId == PaymentHeadId
                                    && ((EmployeeName == "" || EmployeeName == null) || emp.NAME.Contains(EmployeeName))
                                    && ((EmployeeNo == "" || EmployeeNo == null) || emp.FileNo.Contains(EmployeeNo))
                                    && ((DepartmentCode == "" || DepartmentCode == null) || emp.departmentcode == DepartmentCode)
                                 orderby emp.NAME
                                 select new AdhocEmployeeModel
                                 {
                                     MakePayment = true,
                                     EmployeeID = emp.FileNo,
                                     EmployeeName = emp.NAME,
                                     commitmentNo = emp.commitmentNo,
                                     PROJECTNO = emp.PROJECTNO,
                                     departmentcode = emp.departmentcode,
                                     DEPARTMENT = emp.DEPARTMENT
                                 });


                    list = query.Skip(skiprec).Take(pageSize).ToList();


                    for (int i = 0; i < list.Count; i++)
                    {

                        if (PaymentHeadId > 0)
                        {
                            salary = GetAnEmployeeDetailsByPaymentHeadId(list[i].EmployeeID, PaymentHeadId);
                        }

                        if (salary != null && salary.SalaryDetail != null)
                        {
                            list[i].SalaryDetail = salary.SalaryDetail;
                            list[i].BasicPay = Convert.ToDecimal(salary.SalaryDetail.Basic);
                        }

                    }
                    var recordCount = query.Count();
                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    searchData.Data = list;
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedList<AdhocEmployeeModel> GetMainSalaryEmployees(string PaymentMonthYear, int PaymentHeadId, int page, int pageSize)
        {
            try
            {
                int skiprec = 0;

                if (page > 1)
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedList<AdhocEmployeeModel>();

                List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
                AdhocEmployeeModel salary = new AdhocEmployeeModel();
                DateTime lastDate = fo.GetMonthLastDate(PaymentMonthYear);
                DateTime startDate = fo.GetMonthFirstDate(PaymentMonthYear);
                using (var context = new IOASDBEntities())
                {
                    var query = (from emp in context.VWOngoing
                                 where emp.AppointmentDate <= startDate && emp.ExtensionDate >= lastDate //&& emp.FileNo != E.EmployeeId
                                    && ((EmployeeName == "" || EmployeeName == null) || emp.NAME.Contains(EmployeeName))
                                    && ((EmployeeNo == "" || EmployeeNo == null) || emp.FileNo.Contains(EmployeeNo))
                                    && ((DepartmentCode == "" || DepartmentCode == null) || emp.departmentcode == DepartmentCode)
                                    && !(from ED in context.tblSalaryPayment
                                         join CC in context.tblCodeControl on ED.TypeOfPayBill equals CC.CodeValAbbr
                                         where ED.PaymentMonthYear == PaymentMonthYear
                                            && CC.CodeValDetail == "Main"
                                            && CC.CodeName == "PayOfBill"
                                            && (PaymentHeadId == 0 || ED.PaymentHeadId == PaymentHeadId)
                                         select ED.EmployeeId).ToList().Contains(emp.FileNo)
                                 orderby emp.NAME
                                 select new AdhocEmployeeModel
                                 {
                                     MakePayment = true,
                                     EmployeeID = emp.FileNo,
                                     EmployeeName = emp.NAME,
                                     commitmentNo = emp.commitmentNo,
                                     PROJECTNO = emp.PROJECTNO,
                                     departmentcode = emp.departmentcode,
                                     DEPARTMENT = emp.DEPARTMENT
                                 });

                    list = query.Skip(skiprec).Take(pageSize).ToList();


                    for (int i = 0; i < list.Count; i++)
                    {

                        salary = GetAnEmployeeDetails(list[i].EmployeeID, PaymentMonthYear);

                        if (salary != null && salary.SalaryDetail != null)
                        {
                            list[i].SalaryDetail = salary.SalaryDetail;
                            list[i].BasicPay = Convert.ToDecimal(salary.SalaryDetail.Basic);
                        }

                    }
                    var recordCount = query.Count();
                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    searchData.Data = list;
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public PagedList<AdhocEmployeeModel> GetSubSalaryEmployees(string PaymentMonthYear, int page, int pageSize)
        {
            try
            {

                int skiprec = 0;

                if (page > 1)
                {
                    skiprec = (page - 1) * pageSize;
                }
                var searchData = new PagedList<AdhocEmployeeModel>();

                SalaryModel salaryModel = new SalaryModel();

                List<AdhocEmployeeModel> list = new List<AdhocEmployeeModel>();
                DateTime lastDate = fo.GetMonthLastDate(PaymentMonthYear);
                DateTime startDate = fo.GetMonthFirstDate(PaymentMonthYear);
                DateTime monthStartDate = fo.GetMonthFirstDate(PaymentMonthYear);
                startDate = lastDate.AddDays(-7);
                string gender = "";
                int days = (lastDate - monthStartDate).Days;

                using (var context = new IOASDBEntities())
                {
                    var query = (from emp in context.VWOngoing
                                 orderby emp.NAME
                                 where emp.AppointmentDate <= startDate && emp.ExtensionDate >= lastDate
                                 && DbFunctions.DiffDays(emp.AppointmentDate, lastDate).Value < days
                                 //&& days > lastDate.Subtract(DB(emp.AppointmentDate))
                                 && !(from SP in context.tblSalaryPayment
                                      join CC in context.tblCodeControl on SP.TypeOfPayBill equals CC.CodeValAbbr
                                      where SP.PaymentMonthYear == PaymentMonthYear
                                      && CC.CodeName == "PayOfBill"
                                      && CC.CodeValDetail != "Main"
                                      select SP.EmployeeId).ToList().Contains(emp.FileNo)
                                 select new AdhocEmployeeModel
                                 {
                                     MakePayment = true,
                                     EmployeeID = emp.FileNo,
                                     EmployeeName = emp.NAME,
                                     commitmentNo = emp.commitmentNo,
                                     PROJECTNO = emp.PROJECTNO,
                                     Gender = emp.Gender,
                                     BasicPay = (decimal)emp.BasicPay,
                                     FromDate = (DateTime)emp.AppointmentDate,
                                     ToDate = (DateTime)emp.ExtensionDate,
                                     paybill_id = emp.paybill_id,
                                     paybill_no = emp.paybill_no
                                     //PaymentId = (int?)E.PaymentId,
                                     //PaymentHeadId = (int?)E.PaymentHeadId,
                                     //IsPaid = (bool?)E.IsPaid
                                     ,
                                     NoOfDays = DbFunctions.DiffDays(emp.AppointmentDate, lastDate).Value
                                 });
                    list = query.Skip(skiprec).Take(pageSize).ToList();

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var workingDays = (lastDate - list[i].FromDate).Days;

                        if (workingDays < days)
                        {
                            if (list[i].Gender != null && list[i].Gender.ToLower() == "m")
                            {
                                gender = "Male";
                            }
                            else if (list[i].Gender != null && list[i].Gender.ToLower() == "f")
                            {
                                gender = "Female";
                            }
                            var taxSlab = GetTaxSlab(gender);
                            salaryModel = GetSalaryComponent(list[i].EmployeeID);
                            if (salaryModel == null)
                            {
                                salaryModel.Basic = Convert.ToDecimal(list[i].BasicPay);
                                salaryModel.HRA = 0;
                                salaryModel.MA = 0;
                                salaryModel.DA = 0;
                                salaryModel.Conveyance = 0;
                            }
                            salaryModel.paybill_id = list[i].paybill_id;
                            salaryModel.paybill_no = list[i].paybill_no;
                            salaryModel.EmployeeId = list[i].EmployeeID;
                            salaryModel.FromDate = Convert.ToDateTime(list[i].FromDate);
                            salaryModel.ToDate = Convert.ToDateTime(list[i].ToDate);
                            salaryModel.taxSlab = taxSlab;
                            salaryModel.PaymentMonthYear = PaymentMonthYear;
                            DateOp dtOperation = new DateOp();
                            dtOperation.GetDiff(list[i].FromDate, lastDate);
                            salaryModel.NoOfDaysPresent = dtOperation.Days;
                            var salary = GetSalaryDetail(salaryModel);
                            //salary.PaymentId = Convert.ToInt32(list[i].PaymentId);
                            //salary.IsPaid = Convert.ToBoolean(list[i].IsPaid);

                            list[i].SalaryDetail = salary;
                            list[i].BasicPay = Convert.ToDecimal(salary.Basic);
                        }
                        else
                        {
                            list.RemoveAt(i);
                        }

                    }
                    var recordCount = query.Count();
                    searchData.TotalRecords = recordCount;
                    searchData.TotalPages = Convert.ToInt32(Math.Ceiling((double)recordCount / pageSize));
                    searchData.Data = list;
                }
                return searchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public AdhocEmployeeModel GetSubSalaryEmployee(string EmployeeId, string PaymentMonthYear)
        {
            try
            {
                AdhocEmployeeModel employee = new AdhocEmployeeModel();
                DateTime lastDate = fo.GetMonthLastDate(PaymentMonthYear);
                DateTime startDate = fo.GetMonthFirstDate(PaymentMonthYear);
                startDate = lastDate.AddDays(-7);
                //lastDate = lastDate.AddDays(-7);
                int days = (lastDate - startDate).Days;
                using (var context = new IOASDBEntities())
                {
                    var query = (from emp in context.VWOngoing
                                 orderby emp.NAME
                                 where emp.AppointmentDate <= startDate && emp.ExtensionDate >= lastDate
                                 && emp.FileNo == EmployeeId
                                    && !(from SP in context.tblSalaryPayment
                                         join CC in context.tblCodeControl on SP.TypeOfPayBill equals CC.CodeValAbbr
                                         where SP.PaymentMonthYear == PaymentMonthYear && CC.CodeName == "PayOfBill"
                                         && CC.CodeValDetail == "Supplementary"
                                         && SP.EmployeeId == EmployeeId
                                         //&& SP.Status == "Approved"
                                         select SP.EmployeeId).ToList().Contains(emp.FileNo)
                                 select new AdhocEmployeeModel
                                 {
                                     MakePayment = true,
                                     EmployeeID = emp.FileNo,
                                     EmployeeName = emp.NAME,
                                     commitmentNo = emp.commitmentNo,
                                     PROJECTNO = emp.PROJECTNO,
                                     Gender = emp.Gender,
                                     BasicPay = (decimal)emp.BasicPay,
                                     FromDate = (DateTime)emp.AppointmentDate,
                                     ToDate = (DateTime)emp.ExtensionDate,
                                     paybill_id = emp.paybill_id,
                                     paybill_no = emp.paybill_no,
                                     //PaymentId = (int?)E.PaymentId,
                                     //PaymentHeadId = (int?)E.PaymentHeadId,
                                     //IsPaid = (bool?)E.IsPaid
                                 });
                    var list = query.ToList();
                    employee = query.SingleOrDefault();
                    if (employee != null)
                    {
                        SalaryModel salaryModel = new SalaryModel();
                        var gender = "";
                        if (employee.Gender != null && employee.Gender.ToLower() == "m")
                        {
                            gender = "Male";
                        }
                        else if (employee.Gender != null && employee.Gender.ToLower() == "f")
                        {
                            gender = "Female";
                        }
                        salaryModel = GetSalaryComponent(employee.EmployeeID);
                        if (salaryModel == null)
                        {
                            salaryModel.Basic = Convert.ToDecimal(employee.BasicPay);
                            salaryModel.HRA = 0;
                            salaryModel.MA = 0;
                            salaryModel.DA = 0;
                            salaryModel.Conveyance = 0;
                        }
                        salaryModel.paybill_id = employee.paybill_id;
                        salaryModel.paybill_no = employee.paybill_no;
                        salaryModel.EmployeeId = employee.EmployeeID;
                        salaryModel.FromDate = Convert.ToDateTime(employee.FromDate);
                        salaryModel.ToDate = Convert.ToDateTime(employee.ToDate);
                        salaryModel.taxSlab = GetTaxSlab(gender);
                        salaryModel.PaymentMonthYear = PaymentMonthYear;

                        DateOp dtOperation = new DateOp();
                        dtOperation.GetDiff(employee.FromDate, lastDate);
                        salaryModel.NoOfDaysPresent = dtOperation.Days;
                        var salary = GetSalaryDetail(salaryModel);
                        //salary.PaymentId = Convert.ToInt32(list[i].PaymentId);
                        //salary.IsPaid = Convert.ToBoolean(list[i].IsPaid);
                        employee.SalaryDetail = salary;
                        employee.BasicPay = Convert.ToDecimal(salary.CurrentMonthSalary);
                    }

                }
                return employee;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<EmpOtherAllowance> GetEmpOtherAllowance(string EmployeeId)
        {
            try
            {
                List<EmpOtherAllowance> model = new List<EmpOtherAllowance>();
                using (var context = new IOASDBEntities())
                {
                    var records = (from OA in context.tblEmpOtherAllowance
                                   join CC in context.tblCodeControl on OA.ComponentName equals CC.CodeValDetail
                                   where OA.EmployeeIdStr == EmployeeId && OA.IsPaid == false && CC.CodeName == "OtherAllowance"
                                   select new
                                   {
                                       EmployeeId = OA.EmployeeIdStr,
                                       ComponentName = OA.ComponentName,
                                       Amount = OA.Amount,
                                       deduction = OA.deduction,
                                       Status = OA.Status,
                                       IsPaid = OA.IsPaid,
                                       taxable = CC.CodeDescription
                                   }).ToList();
                    if (records != null)
                    {
                        foreach (var item in records)
                        {
                            model.Add(new EmpOtherAllowance
                            {
                                EmployeeId = item.EmployeeId,
                                ComponentName = item.ComponentName,
                                Amount = Convert.ToDecimal(item.Amount),
                                deduction = Convert.ToBoolean(item.deduction),
                                Status = item.Status,
                                IsPaid = Convert.ToBoolean(item.IsPaid),
                                taxable = item.taxable.ToLower() == "true" ? true : false
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}