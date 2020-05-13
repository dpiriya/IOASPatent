using IOAS.DataModel;
using IOAS.Models.Patent;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace IOAS.GenericServices
{
    public class PatentService
    {
        #region IDFRequest
        public static List<IDFRequest_trxVM> GetIDFRequestDetails()
        {
            List<IDFRequest_trxVM> idf = new List<IDFRequest_trxVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.tbl_trx_IDFRequest.OrderByDescending(m => m.FileNo).Select(m => new { m.FileNo, m.VersionId, m.Status, m.PrimaryInventorName, m.PIDepartment, m.Remarks, m.IDFType }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        idf.Add(new IDFRequest_trxVM()
                        {
                            Remarks = query[i].Remarks,
                            PIDepartment = query[i].PIDepartment,
                            PrimaryInventorName = query[i].PrimaryInventorName,
                            Status = query[i].Status,
                            FileNo = query[i].FileNo,
                            VersionId = query[i].VersionId,
                            IDFType = query[i].IDFType
                        });
                    }
                }
            }
            return idf;
        }
        public static IDFRequest_trxVM EditIDFRequest(IDFRequest_trxVM vm)
        {
            try
            {
                IDFRequest_trxVM idf = new IDFRequest_trxVM();
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_IDFRequest.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).FirstOrDefault();
                    if (query != null)
                    {
                        idf.FileNo = query.FileNo;
                        idf.VersionId = query.VersionId;
                        idf.IDFType = query.IDFType;
                        idf.FieldOfInvention = query.FieldOfInvention;
                        idf.BiologicalMaterial = query.BiologicalMaterial;
                        idf.Description = query.Description;
                        idf.DetailsOfBiologicalMaterial = query.DetailsOfBiologicalMaterial;
                        idf.Disclosure = query.Disclosure;
                        idf.FirstApplicantAddress = query.FirstApplicantAddress;
                        idf.FirstApplicantContactNo = query.FirstApplicantContactNo;
                        idf.FirstApplicantEmailId = query.FirstApplicantEmailId;
                        idf.FirstApplicantName = query.FirstApplicantName;
                        idf.FirstApplicantOrganisation = query.FirstApplicantOrganisation;
                        idf.FirstApplicantPosition = query.FirstApplicantPosition;
                        idf.PIContactNo = query.PIContactNo;
                        idf.PIDepartment = query.PIDepartment;
                        idf.PIEmailId = query.PIEmailId;
                        idf.PrimaryInventorType = query.PrimaryInventorType;
                        idf.PrimaryInventorName = query.PrimaryInventorName;
                        idf.PriorPublication = query.PriorPublication;
                        idf.RelevantInformation = query.RelevantInformation;
                        //idf.RequestedAction = query.RequestedAction;
                        if (idf.IDFType == "Trademark")
                            idf.RequestedTMAction = query.RequestedAction;
                        else if (idf.IDFType == "Copyright")
                        {
                            idf.RequestedCRAction = query.RequestedAction;
                            idf.RequestedCRtxtAction = query.RequestedActionOthers;
                        }
                        else
                        {
                            idf.RequestedAction = query.RequestedAction;
                            idf.RequestedtxtAction = query.RequestedActionOthers;
                        }
                        idf.SourceOfInvention = query.SourceOfInvention;
                        idf.Summary = query.Summary;
                        idf.SupportInformation = query.SupportInformation;
                        idf.Title = query.Title;
                        var anex = context.tbl_trx_AnnexureB1.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).ToList();
                        foreach (var a in anex)
                        {
                            idf.Annex.VersionId = a.VersionId;
                            idf.Annex.BriefDescription = a.BriefDescription;
                            idf.Annex.Comments = a.Comments;
                            idf.Annex.DevelopmentStage = a.DevelopmentStage;
                            idf.Annex.L1Search = a.L1Search;
                            idf.Annex.OtherInfo = a.OtherInfo;
                            idf.Annex.Outcome = a.Outcome;
                            idf.Annex.Party = a.Party;
                            idf.Annex.Tool = a.Tool;
                        }
                        List<CoInventor_trxVM> coinventor = new List<CoInventor_trxVM>();
                        var coin = context.tbl_trx_CoInventor.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => m.SNo).ToList();
                        if (coin.Count > 0)
                        {
                            foreach (var a in coin)
                            {
                                var codetail = context.tbl_trx_CoInventor.Where(m => m.SNo == a && m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).FirstOrDefault();
                                coinventor.Add(new CoInventor_trxVM()
                                {
                                    FileNo = codetail.FileNo,
                                    VersionId = codetail.VersionId,
                                    SNo = codetail.SNo,
                                    Dept = codetail.Dept,
                                    Name = codetail.Name,
                                    Type = codetail.Type,
                                    Mail = codetail.EmailId,
                                    Ph = codetail.ContactNo
                                });
                            }
                            idf.CoIn = coinventor;
                        }
                        List<Applicant_trxVM> applicant = new List<Applicant_trxVM>();
                        var appln = context.tbl_trx_Applicants.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => m.Sno).ToList();
                        if (appln.Count > 0)
                        {
                            foreach (var a in appln)
                            {
                                var appdetail = context.tbl_trx_Applicants.Where(m => m.Sno == a && m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).FirstOrDefault();
                                applicant.Add(new Applicant_trxVM()
                                {
                                    Sno = appdetail.Sno,
                                    VersionId = appdetail.VersionId,
                                    Position = appdetail.Position,
                                    Address = appdetail.Address,
                                    ContactName = appdetail.ContactName,
                                    Organisation = appdetail.Organisation,
                                    EmailId = appdetail.EmailId,
                                    ContactNo = appdetail.ContactNo,
                                    FileNo = appdetail.FileNo
                                });
                            }
                            idf.Appl = applicant;
                        }
                        idf.Annex.ListIndustry = vm.Annex.ListIndustry;
                        idf.Annex.ListIndustry1 = vm.Annex.ListIndustry1;
                        var area = context.tbl_trx_ApplicationAreas.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => new { m.Index, m.Category }).ToList();
                        if (area.Count > 0)
                        {
                            foreach (var a in area)
                            {
                                if (a.Category == "ApplicationIndustry")
                                {
                                    idf.Annex.ListIndustry[a.Index].Selected = true;
                                }
                                else
                                {
                                    idf.Annex.ListIndustry1[a.Index].Selected = true;
                                }
                            }
                        }
                        idf.Annex.IITMode = vm.Annex.IITMode;
                        idf.Annex.JointMode = vm.Annex.JointMode;
                        var mode = context.tbl_trx_CommercialisationMode.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => new { m.IndNo, m.Category }).ToList();
                        if (mode.Count > 0)
                        {
                            foreach (var a in mode)
                            {
                                if (a.Category == "IIT")
                                {
                                    idf.Annex.IITMode[a.IndNo].Selected = true;
                                }
                                else
                                {
                                    idf.Annex.JointMode[a.IndNo].Selected = true;
                                }
                            }
                        }
                        if (idf.IDFType == "Copyright")
                        {
                            var cr = context.tbl_trx_Copyright.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).ToList();
                            foreach (var c in cr)
                            {
                                idf.CR.VersionId = c.VersionId;
                                idf.CR.Category = c.Category;
                                idf.CR.ClassofWork = c.ClassofWork;
                                idf.CR.Description = c.Description;
                                idf.CR.Details = c.Details;
                                idf.CR.isPublished = c.isPublished;
                                idf.CR.isRegistered = c.isRegistered;
                                idf.CR.Language = c.Language;
                                idf.CR.Nature = c.Nature;
                                idf.CR.Original = c.Original;
                                idf.CR.Title = c.Title;
                            }
                            List<CRAuthor_trxVM> authvm = new List<CRAuthor_trxVM>();
                            var auth = context.tbl_trx_CR_Author.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => m.SNo).ToList();
                            if (auth.Count > 0)
                            {
                                foreach (var a in auth)
                                {
                                    var authdetail = context.tbl_trx_CR_Author.Where(m => m.FileNo == vm.FileNo && m.SNo == a && m.VersionId == vm.VersionId).FirstOrDefault();
                                    authvm.Add(new CRAuthor_trxVM()
                                    {
                                        VersionId = authdetail.VersionId,
                                        AUAddress = authdetail.AUAddress,
                                        AUName = authdetail.AUName,
                                        AUNationality = authdetail.AUNationality,
                                        isDeceased = authdetail.isDeceased,
                                        SNo = authdetail.SNo,
                                        deceasedDt = authdetail.deceasedDt
                                    });
                                }
                                idf.CR.Author = authvm;
                            }
                            List<CRPublish_trxVM> pubvm = new List<CRPublish_trxVM>();
                            var pub = context.tbl_trx_CR_Publish.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => m.Sno).ToList();
                            if (pub.Count > 0)
                            {
                                foreach (var a in pub)
                                {
                                    var pubdetail = context.tbl_trx_CR_Publish.Where(m => m.FileNo == vm.FileNo && m.Sno == a && m.VersionId == vm.VersionId).FirstOrDefault();
                                    pubvm.Add(new CRPublish_trxVM()
                                    {
                                        VersionId = pubdetail.VersionId,
                                        Sno = pubdetail.Sno,
                                        Country = pubdetail.Country,
                                        PUAddress = pubdetail.PUAddress,
                                        PUName = pubdetail.PUName,
                                        PUNationality = pubdetail.PUNationality,
                                        Year = pubdetail.Year
                                    });
                                }
                                idf.CR.Publish = pubvm;
                            }
                        }
                        else if (idf.IDFType == "Trademark")
                        {
                            var tr = context.tbl_trx_Trademark.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).ToList();
                            foreach (var t in tr)
                            {
                                idf.Trade.VersionId = t.VersionId;
                                idf.Trade.Language = t.Language;
                                idf.Trade.TMName = t.TMName;
                                idf.Trade.TMImage = t.TMImage;
                                idf.Trade.TMStatement = t.TMStatement;
                                idf.Trade.Category = t.Category;
                                idf.Trade.Class = t.Class;
                                idf.Trade.Description = t.Description;
                            }
                            List<TradeApplicant_trxVM> tradevm = new List<TradeApplicant_trxVM>();
                            var trade = context.tbl_trx_Trade_Applicantdetail.Where(m => m.FileNo == vm.FileNo && m.VersionId == vm.VersionId).Select(m => m.Sno).ToList();
                            if (trade.Count > 0)
                            {
                                foreach (var a in trade)
                                {
                                    var appdetail = context.tbl_trx_Trade_Applicantdetail.Where(m => m.FileNo == vm.FileNo && m.Sno == a && m.VersionId == vm.VersionId).FirstOrDefault();
                                    tradevm.Add(new TradeApplicant_trxVM()
                                    {
                                        AddressOfService = appdetail.AddressOfService,
                                        Country = appdetail.Country,
                                        Organisation = appdetail.Organisation,
                                        Nature = appdetail.Nature,
                                        Sno = appdetail.Sno,
                                        Jurisdiction = appdetail.Jurisdiction,
                                        LegalStatus = appdetail.LegalStatus
                                    });
                                }
                                idf.Trade.TAppl = tradevm;
                            }
                        }
                        List<PatFilesVM> fvm = new List<PatFilesVM>();
                        var upfiles = context.tbl_files_PatentRequest.Where(m => m.FileNo == vm.FileNo).Select(m => m.DocId).ToList();
                        if (upfiles.Count > 0)
                        {
                            foreach (var v in upfiles)
                            {
                                var filedetail = context.tbl_files_PatentRequest.Where(m => m.DocId == v && m.FileNo == vm.FileNo).FirstOrDefault();
                                fvm.Add(new PatFilesVM()
                                {
                                    FileNo = vm.FileNo,
                                    DocId = filedetail.DocId,
                                    DocName = filedetail.DocName,
                                    DocPath = filedetail.DocPath
                                });
                            }
                            idf.Files = fvm;
                        }
                    }

                    return idf;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string UpdateClarification(Int64 fno, int vid, string rem)
        {
            try
            {
                using (PatentNewEntities pat = new PatentNewEntities())
                {
                    var sel = pat.tbl_trx_IDFRequest.Where(m => m.FileNo == fno && m.VersionId == vid).FirstOrDefault();
                    if (sel != null)
                    {
                        sel.Status = "Clarification needed";
                        sel.Remarks = rem;
                        var q = pat.tblIDFRequest.FirstOrDefault(m => m.FileNo == fno);
                        if (q != null)
                        {
                            q.Status = "Clarification needed";
                            q.Remarks = rem;
                            pat.SaveChanges();
                        }
                        else { return "Failed"; }
                        return "Success";
                    }
                    return "Failed";

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion
        #region FileCreation
        public static List<FileCreationVM> GetFileList()
        {
            List<FileCreationVM> file = new List<FileCreationVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.tblIDFRequest.OrderByDescending(m => m.FileNo).Select(m => new { m.FileNo, m.PrimaryInventorName, m.PIDepartment, m.Status, m.Title }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        file.Add(new FileCreationVM()
                        {
                            FileNo = query[i].FileNo,
                            Title = query[i].Title,
                            PrimaryInventorName = query[i].PrimaryInventorName,
                            Status = query[i].Status,
                            PIDepartment = query[i].PIDepartment
                        });
                    }
                }
            }
            return file;
        }
        public static FileCreationVM EditFCBasic(FileCreationVM vm)
        {
            try
            {
                FileCreationVM fc = new FileCreationVM();
                using (var context = new PatentNewEntities())
                {
                    var query = context.tblIDFRequest.Where(m => m.FileNo == vm.FileNo).FirstOrDefault();
                    if (query != null)
                    {
                        fc.FileNo = query.FileNo;
                        fc.FirstApplicantAddress = query.FirstApplicantAddress;
                        fc.FirstApplicantContactNo = query.FirstApplicantContactNo;
                        fc.FirstApplicantEmailId = query.FirstApplicantEmailId;
                        fc.FirstApplicantName = query.FirstApplicantName;
                        fc.FirstApplicantOrganisation = query.FirstApplicantOrganisation;
                        fc.FirstApplicantPosition = query.FirstApplicantPosition;
                        fc.PIContactNo = query.PIContactNo;
                        fc.PIDepartment = query.PIDepartment;
                        fc.PIEmailId = query.PIEmailId;
                        fc.PIInstId = query.PIInstId;
                        fc.PrimaryInventorType = query.PrimaryInventorType;
                        fc.PrimaryInventorName = query.PrimaryInventorName;
                        fc.Title = query.Title;
                        List<CoInventor_trxVM> coinventor = new List<CoInventor_trxVM>();
                        var coin = context.tblCoInventor.Where(m => m.FileNo == vm.FileNo).Select(m => m.SNo).ToList();
                        if (coin.Count > 0)
                        {
                            foreach (var a in coin)
                            {
                                var codetail = context.tblCoInventor.Where(m => m.SNo == a && m.FileNo == vm.FileNo).FirstOrDefault();
                                coinventor.Add(new CoInventor_trxVM()
                                {
                                    FileNo = codetail.FileNo,
                                    SNo = codetail.SNo,
                                    Dept = codetail.Dept,
                                    Name = codetail.Name,
                                    Type = codetail.Type,
                                    Mail = codetail.EmailId,
                                    Ph = codetail.ContactNo
                                });
                            }
                            fc.CoIn = coinventor;
                        }
                        List<Applicant_trxVM> applicant = new List<Applicant_trxVM>();
                        var appln = context.tblApplicants.Where(m => m.FileNo == vm.FileNo).Select(m => m.Sno).ToList();
                        if (appln.Count > 0)
                        {
                            foreach (var a in appln)
                            {
                                var appdetail = context.tblApplicants.Where(m => m.Sno == a && m.FileNo == vm.FileNo).FirstOrDefault();
                                applicant.Add(new Applicant_trxVM()
                                {
                                    Sno = appdetail.Sno,
                                    Position = appdetail.Position,
                                    Address = appdetail.Address,
                                    ContactName = appdetail.ContactName,
                                    Organisation = appdetail.Organisation,
                                    EmailId = appdetail.EmailId,
                                    ContactNo = appdetail.ContactNo,
                                    FileNo = appdetail.FileNo
                                });
                            }
                            fc.Appl = applicant;
                        }
                    }

                    return fc;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string UpdateFCBasic(FileCreationVM vm, string user)
        {
            using (var context = new PatentNewEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var fc = context.tblIDFRequest.Where(m => m.FileNo == vm.FileNo && m.Status == "Dean Approved").FirstOrDefault();
                        if (fc != null)
                        {
                            fc.FileNo = vm.FileNo;
                            fc.FirstApplicantAddress = vm.FirstApplicantAddress;
                            fc.FirstApplicantContactNo = vm.FirstApplicantContactNo;
                            fc.FirstApplicantEmailId = vm.FirstApplicantEmailId;
                            fc.FirstApplicantName = vm.FirstApplicantName;
                            fc.FirstApplicantOrganisation = vm.FirstApplicantOrganisation;
                            fc.FirstApplicantPosition = vm.FirstApplicantPosition;
                            fc.PIContactNo = vm.PIContactNo;
                            fc.PIDepartment = vm.PIDepartment;
                            fc.PIEmailId = vm.PIEmailId;
                            fc.PIInstId = vm.PIInstId;
                            fc.PrimaryInventorType = vm.PrimaryInventorType;
                            fc.PrimaryInventorName = vm.PrimaryInventorName;
                            fc.Title = vm.Title;
                            fc.ModifiedBy = user;
                            fc.ModifiedOn = DateTime.Now;
                            context.SaveChanges();
                        }
                        if (vm.CoIn.Count > 0)
                        {
                            var coin = context.tblCoInventor.Where(m => m.FileNo == vm.FileNo).ToList();
                            int i = 0, y = 1;
                            if (coin.Count == vm.CoIn.Count)
                            {
                                foreach (var a in coin)
                                {
                                    a.FileNo = vm.FileNo;
                                    a.Name = vm.CoIn[i].Name;
                                    a.Type = vm.CoIn[i].Type;
                                    a.ContactNo = vm.CoIn[i].Ph;
                                    a.Dept = vm.CoIn[i].Dept;
                                    a.EmailId = vm.CoIn[i].Mail;
                                    a.ModifiedBy = user;
                                    a.ModifiedOn = DateTime.Now;
                                    a.SNo = ++i;
                                }
                                context.SaveChanges();
                            }
                            else
                            {
                                var co = context.tblCoInventor.Where(m => m.FileNo == vm.FileNo).ToList();
                                if (co != null)
                                {
                                    context.tblCoInventor.RemoveRange(co);
                                    context.SaveChanges();
                                }
                                foreach (var m in vm.CoIn)
                                {
                                    if (m.Name != null)
                                    {
                                        tblCoInventor tco = new tblCoInventor()
                                        {
                                            FileNo=vm.FileNo,
                                            Name = vm.CoIn[i].Name,
                                            SNo = y,
                                            ContactNo = vm.CoIn[i].Ph,
                                            Dept = vm.CoIn[i].Dept,
                                            EmailId = vm.CoIn[i].Mail,
                                            Type = vm.CoIn[i].Type,
                                            ModifiedBy = user,
                                            ModifiedOn = DateTime.Now
                                        }; ++i; ++y;
                                        context.tblCoInventor.Add(tco);
                                    }
                                }
                                context.SaveChanges();
                            }
                        }
                        if (vm.Appl.Count > 0)
                        {
                            var ap = context.tblApplicants.Where(m => m.FileNo == vm.FileNo).ToList();
                            int i = 0, y = 1;
                            if (ap.Count == vm.Appl.Count)
                            {
                                foreach (var a in ap)
                                {
                                    a.FileNo = vm.FileNo;
                                    a.Address = vm.Appl[i].Address;
                                    a.ContactName = vm.Appl[i].ContactName;
                                    a.ContactNo = vm.Appl[i].ContactNo;
                                    a.EmailId = vm.Appl[i].EmailId;
                                    a.ModifiedBy = user;
                                    a.ModifiedOn = DateTime.Now;
                                    a.Organisation = vm.Appl[i].Organisation;
                                    a.Position = vm.Appl[i].Position;
                                    a.Sno = ++i;
                                }
                                context.SaveChanges();
                            }
                            else
                            {
                                var app = context.tblApplicants.Where(m => m.FileNo == vm.FileNo).ToList();
                                if (app != null)
                                {
                                    context.tblApplicants.RemoveRange(app);
                                    context.SaveChanges();
                                }
                                foreach (var m in vm.Appl)
                                {
                                    if (m.Organisation != null)
                                    {
                                        tblApplicants tap = new tblApplicants()
                                        {
                                            FileNo=vm.FileNo,
                                            Organisation = vm.Appl[i].Organisation,
                                            Address = vm.Appl[i].Address,
                                            ContactName = vm.Appl[i].ContactName,
                                            EmailId = vm.Appl[i].EmailId,
                                            ModifiedBy = user,
                                            ModifiedOn = DateTime.Now,
                                            Position = vm.Appl[i].Position,
                                            Sno = y
                                        }; ++i; ++y;
                                        context.tblApplicants.Add(tap);
                                    }
                                }
                                context.SaveChanges();
                            }
                        }

                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return "Failure";
                    }
                }
            }
        }
        public static IndianPatentVM EditIndianPatent(IndianPatentVM vm, string uname)
        {
            try
            {
                using (PatentNewEntities pat = new PatentNewEntities())
                {
                    var q = pat.tblIndianPatent.FirstOrDefault(m => m.FileNo == vm.FileNo);
                    if (q != null)
                    {
                        vm.ApplicationNo = q.ApplicationNo;
                        vm.Attorney = q.Attorney;
                        vm.CompleteFilingDate = q.CompleteFilingDate;
                        vm.CreatedBy = uname;
                        vm.CreatedOn = DateTime.Now;
                        vm.FERIssued = q.FERIssued;
                        vm.FERIssueDate = q.FERIssueDate;
                        vm.FERPlaced = q.FERPlaced;
                        vm.FilingDate = q.FilingDate;
                        vm.PublicationDate = q.PublicationDate;
                        vm.PublicationNo = q.PublicationNo;
                        vm.PublicationPath = q.PublicationPath;
                        vm.Status = q.Status;
                        vm.SubStatus = q.SubStatus;
                    }
                    return vm;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string InsertIndianPatent(IndianPatentVM model, string uname)
        {

            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    tblIndianPatent ip = new tblIndianPatent()
                    {
                        FileNo = model.FileNo,
                        ApplicationNo = model.ApplicationNo,
                        Attorney = model.Attorney,
                        CompleteFilingDate = model.CompleteFilingDate,
                        FilingDate = model.FilingDate,
                        FERIssued = model.FERIssued,
                        FERIssueDate = model.FERIssueDate,
                        FERPlaced = model.FERPlaced,
                        PublicationDate = model.PublicationDate,
                        PublicationNo = model.PublicationNo,
                        PublicationPath = model.PublicationPath,
                        Status = model.Status,
                        SubStatus = model.SubStatus,
                        CreatedBy = uname,
                        CreatedOn = DateTime.Now
                    };
                    pat.tblIndianPatent.Add(ip);
                    pat.SaveChanges();
                    return "Success";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
        public static string UpdateIndianPatent(IndianPatentVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    var ip = pat.tblIndianPatent.Where(m => m.FileNo == model.FileNo).FirstOrDefault();
                    if (ip != null)
                    {
                        ip.FileNo = model.FileNo;
                        ip.ApplicationNo = model.ApplicationNo;
                        ip.Attorney = model.Attorney;
                        ip.CompleteFilingDate = model.CompleteFilingDate;
                        ip.FERIssued = model.FERIssued;
                        ip.FERIssueDate = model.FERIssueDate;
                        ip.FERPlaced = model.FERPlaced;
                        ip.FilingDate = model.FilingDate;
                        ip.Status = model.Status;
                        ip.SubStatus = model.SubStatus;
                        ip.PublicationDate = model.PublicationDate;
                        ip.PublicationNo = model.PublicationNo;
                        ip.PublicationPath = model.PublicationPath;
                        ip.ModifiedBy = uname;
                        ip.ModifiedOn = DateTime.Today.Date;
                        pat.SaveChanges();
                    }
                    return "Success";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
        public static NewInternationalVM EditNewInternational(NewInternationalVM vm)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    var q = pat.tblPCT.Where(m => m.FileNo == vm.FileNo).FirstOrDefault();
                    if (q != null)
                    {
                        vm.isPCT = true;
                        vm.PCTFilingNo = q.PCTFilingNo;
                        vm.Attorney = q.Attorney;
                        vm.PCTPublicationNo = q.PCTPuiblicationNo;
                        vm.PublicationDate = q.PublicationDate;
                    }
                    List<InternationalSecVM> intllist = new List<InternationalSecVM>();
                    var sec = pat.tbl_sec_International.Where(m => m.FileNo == vm.FileNo).Select(m => m.SNo).ToList();
                    if (sec.Count > 0)
                    {
                        foreach (var a in sec)
                        {
                            var country = pat.tbl_sec_International.Where(m => m.SNo == a && m.FileNo == vm.FileNo).FirstOrDefault();
                            if (country != null)
                            {
                                intllist.Add(new InternationalSecVM()
                                {
                                    FileNo = country.FileNo,
                                    SNo = country.SNo,
                                    Appln_FilingNo = country.Appln_FilingNo,
                                    Attorney = country.Attorney,
                                    FilingDate = country.FilingDate,
                                    Country = country.Country,
                                    OfficeAction = country.OfficeAction,
                                    OfficeActionDate = country.OfficeActionDate,
                                    PCT = country.PCT,
                                    PublicationDate = country.PublicationDate,
                                    PublicationNo = country.PublicationNo
                                });
                            }
                        }
                        vm.NationalPhase = intllist;
                    }
                    return vm;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public static string InsertNewInternational(NewInternationalVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.isPCT == true)
                        {
                            tblPCT pct = new tblPCT()
                            {
                                Attorney = model.Attorney,
                                FileNo = model.FileNo,
                                PCTFilingNo = model.PCTFilingNo,
                                PCTPuiblicationNo = model.PCTPublicationNo,
                                PublicationDate = model.PublicationDate
                            };
                            pat.tblPCT.Add(pct);
                            pat.SaveChanges();
                        }
                        if (model.NationalPhase.Count > 0)
                        {
                            int sn = 1;
                            foreach (var intl in model.NationalPhase)
                            {
                                if (intl.PCT != null && intl.Country != null)
                                {
                                    tbl_sec_International country = new tbl_sec_International()
                                    {
                                        SNo = sn,
                                        FileNo = model.FileNo,
                                        Appln_FilingNo = intl.Appln_FilingNo,
                                        Attorney = intl.Attorney,
                                        Country = intl.Country,
                                        CreatedBy = uname,
                                        CreatedOn = DateTime.Now,
                                        FilingDate = intl.FilingDate,
                                        OfficeAction = intl.OfficeAction,
                                        OfficeActionDate = intl.OfficeActionDate,
                                        PCT = intl.PCT,
                                        PublicationDate = intl.PublicationDate,
                                        PublicationNo = intl.PublicationNo
                                    };
                                    pat.tbl_sec_International.Add(country);
                                    pat.SaveChanges();
                                }
                                ++sn;
                            }
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string UpdateNewInternational(NewInternationalVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        var pct = pat.tblPCT.Where(m => m.FileNo == model.FileNo).FirstOrDefault();
                        if (pct != null)
                        {
                            pct.FileNo = model.FileNo;
                            pct.Attorney = model.Attorney;
                            pct.PCTFilingNo = model.PCTFilingNo;
                            pct.PCTPuiblicationNo = model.PCTFilingNo;
                            pct.PublicationDate = model.PublicationDate;
                            pat.SaveChanges();
                        }
                        else if (model.isPCT == true)
                        {
                            tblPCT tpct = new tblPCT()
                            {
                                Attorney = model.Attorney,
                                FileNo = model.FileNo,
                                PCTFilingNo = model.PCTFilingNo,
                                PCTPuiblicationNo = model.PCTPublicationNo,
                                PublicationDate = model.PublicationDate
                            };
                            pat.tblPCT.Add(tpct);
                            pat.SaveChanges();
                        }
                        if (model.NationalPhase.Count > 0)
                        {
                            var intl = pat.tbl_sec_International.Where(m => m.FileNo == model.FileNo).ToList();
                            int i = 0; int y = 1;
                            if (intl.Count == model.NationalPhase.Count)
                            {
                                foreach (var m in intl)
                                {
                                    m.FileNo = model.FileNo;
                                    m.Appln_FilingNo = model.NationalPhase[i].Appln_FilingNo;
                                    m.Attorney = model.NationalPhase[i].Attorney;
                                    m.Country = model.NationalPhase[i].Country;
                                    m.FilingDate = model.NationalPhase[i].FilingDate;
                                    m.OfficeAction = model.NationalPhase[i].OfficeAction;
                                    m.OfficeActionDate = model.NationalPhase[i].OfficeActionDate;
                                    m.PCT = model.NationalPhase[i].PCT;
                                    m.PublicationDate = model.NationalPhase[i].PublicationDate;
                                    m.PublicationNo = model.NationalPhase[i].PublicationNo;
                                    ++i;
                                    m.SNo = i;
                                }
                                pat.SaveChanges();
                            }
                            else
                            {
                                var intl1 = pat.tbl_sec_International.Where(m => m.FileNo == model.FileNo).ToList();
                                if (intl1 != null)
                                {
                                    pat.tbl_sec_International.RemoveRange(intl1);
                                    pat.SaveChanges();
                                }
                                foreach (var m in model.NationalPhase)
                                {
                                    if (m.PCT != null && m.Country != null)
                                    {
                                        tbl_sec_International tblint = new tbl_sec_International()
                                        {
                                            FileNo = model.FileNo,
                                            Appln_FilingNo = model.NationalPhase[i].Appln_FilingNo,
                                            Attorney = model.NationalPhase[i].Attorney,
                                            Country = model.NationalPhase[i].Country,
                                            FilingDate = model.NationalPhase[i].FilingDate,
                                            OfficeAction = model.NationalPhase[i].OfficeAction,
                                            OfficeActionDate = model.NationalPhase[i].OfficeActionDate,
                                            PCT = model.NationalPhase[i].PCT,
                                            PublicationDate = model.NationalPhase[i].PublicationDate,
                                            SNo = y
                                        }; ++i; ++y;
                                        pat.tbl_sec_International.Add(tblint);
                                    }
                                }
                                pat.SaveChanges();
                            }
                        }
                        else
                        {
                            var srec = pat.tbl_sec_International.Where(m => m.FileNo == model.FileNo).ToList();
                            if (srec != null)
                            {
                                pat.tbl_sec_International.RemoveRange(srec);
                                pat.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        #endregion
        #region Service Provider
        public static ServiceProviderVM EditServiceProvider(string aid)
        {
            try
            {
                ServiceProviderVM sp = new ServiceProviderVM();
                using (var context = new PatentNewEntities())
                {
                    var query = (from P in context.Attorney
                                 where (P.AttorneyID == aid)
                                 select P).FirstOrDefault();
                    if (query != null)
                    {
                        sp.AttorneyID = query.AttorneyID;
                        sp.Address1 = query.Address1;
                        sp.Address2 = query.Address2;
                        sp.Address3 = query.Address3;
                        sp.AttorneyName = query.AttorneyName;
                        sp.Category = query.Category;
                        sp.City = query.City;
                        sp.Country = query.Country;
                        sp.EmailID = query.EmailID;
                        sp.FaxNo = query.FaxNo;
                        sp.isUpdate = true;
                        sp.Mobile_No = query.Mobile_No;
                        sp.PhoneNo = query.PhoneNo;
                        sp.PinCode = query.PinCode;
                        sp.RangeOfServices = query.RangeOfServices;
                        sp.SlNo = query.SlNo;
                    }
                    return sp;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string InsertServiceProvider(ServiceProviderVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        Attorney sp = new Attorney()
                        {
                            SlNo = model.SlNo,
                            RangeOfServices = model.RangeOfServices,
                            PinCode = model.PinCode,
                            PhoneNo = model.PhoneNo,
                            Mobile_No = model.Mobile_No,
                            FaxNo = model.FaxNo,
                            EmailID = model.EmailID,
                            Country = model.Country,
                            City = model.City,
                            Category = model.Category,
                            AttorneyName = model.AttorneyName,
                            AttorneyID = model.AttorneyID,
                            Address3 = model.Address3,
                            Address2 = model.Address2,
                            Address1 = model.Address1
                        };
                        pat.Attorney.Add(sp);
                        pat.SaveChanges();
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string UpdateServiceProvider(ServiceProviderVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        var sp = pat.Attorney.Where(m => m.AttorneyID == model.AttorneyID).FirstOrDefault();
                        if (sp != null)
                        {
                            sp.AttorneyID = model.AttorneyID;
                            sp.Address1 = model.Address1;
                            sp.Address2 = model.Address2;
                            sp.Address3 = model.Address3;
                            sp.AttorneyName = model.AttorneyName;
                            sp.Category = model.Category;
                            sp.City = model.City;
                            sp.Country = model.Country;
                            sp.EmailID = model.EmailID;
                            sp.FaxNo = model.FaxNo;
                            sp.Mobile_No = model.Mobile_No;
                            sp.PhoneNo = model.PhoneNo;
                            sp.PinCode = model.PinCode;
                            sp.RangeOfServices = model.RangeOfServices;
                            sp.SlNo = model.SlNo;
                            pat.SaveChanges();
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static List<ServiceProviderVM> GetServiceProviderList()
        {
            List<ServiceProviderVM> sp = new List<ServiceProviderVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.Attorney.OrderByDescending(m => m.SlNo).Select(m => new { m.AttorneyID, m.AttorneyName, m.Category, m.Country, m.RangeOfServices }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        sp.Add(new ServiceProviderVM()
                        {
                            AttorneyID = query[i].AttorneyID,
                            AttorneyName = query[i].AttorneyName,
                            Category = query[i].Category,
                            Country = query[i].Country,
                            RangeOfServices = query[i].RangeOfServices
                        });
                    }
                }
            }
            return sp;
        }
        public static string DeleteServiceProvider(string aid)
        {
            using (var context = new PatentNewEntities())
            {
                string status = "Error";
                try
                {
                    var qry = context.Attorney.FirstOrDefault(c => c.AttorneyID == aid);
                    context.Attorney.Remove(qry);
                    context.SaveChanges();
                    status = "Success";
                    //data.Status = "success";
                    return status;
                }
                catch (Exception ex)
                {
                    return status;
                }
            }
        }
        #endregion
        #region Dispute
        public static string InsertDispute(DisputeVM model, string uname)
        {

            using (PatentNewEntities pat = new PatentNewEntities())
            {

                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        tbl_trx_dispute dis = new tbl_trx_dispute()
                        {
                            DisputeNo = model.DisputeNo,
                            DGroup = model.DGroup,
                            DSource = model.DSource,
                            Coordinator = model.Coordinator,
                            EstimatedValue = model.EstimatedValue,
                            PartyName = model.PartyName,
                            RealizationValue = model.RealizationValue,
                            Remarks = model.Remarks,
                            Title = model.Title,
                            Status = model.Status,
                            CreatedBy = uname
                        };
                        pat.tbl_trx_dispute.Add(dis);
                        pat.SaveChanges();
                        int i = 1;
                        foreach (var no in model.MDOC)
                        {
                            if (no.ContractNo != null)
                            {
                                tbl_trx_DisputeAgreement agreement = new tbl_trx_DisputeAgreement()
                                {
                                    Sno = i.ToString(),
                                    DisputeNo = model.DisputeNo,
                                    ContractNo = no.ContractNo
                                };

                                pat.tbl_trx_DisputeAgreement.Add(agreement);
                                pat.SaveChanges();
                            }
                            ++i;
                        }
                        int j = 1;
                        foreach (var idfno in model.Idf)
                        {
                            if (idfno.FileNo != null)
                            {
                                tbl_trx_disputeIDF didf = new tbl_trx_disputeIDF()
                                {
                                    DisputeNo = model.DisputeNo,
                                    FileNo = idfno.FileNo,
                                    Sno = j.ToString()
                                };
                                pat.tbl_trx_disputeIDF.Add(didf);
                                pat.SaveChanges();
                                ++j;
                            }
                        }
                        int z = 0;
                        //@"\\DMS_SAN\IPIMS\DisputeFiles\";/*+model.DisputeNo.Trim()+"\\";*/
                        string filepath = pat.tbl_mst_filepath.Where(m => m.Category == "Dispute").Select(m => m.FilePath).ToString();
                        if (!Directory.Exists(filepath))
                        {
                            return "Path Error";
                            //  Directory.CreateDirectory(
                        }
                        filepath = filepath + model.DisputeNo.Trim() + "\\";
                        if (!Directory.Exists(filepath))
                        {
                            Directory.CreateDirectory(filepath);
                            // Directory.CreateDirectory(@"\\DMS_SAN\IPIMS\DisputeFiles\" + model.DisputeNo.Trim());
                        }
                        foreach (var item in model.activity)
                        {
                            if (item.ActivityDate != null)
                            {
                                tbl_trx_DisputeActivity act = new tbl_trx_DisputeActivity()
                                {
                                    DisputeNo = model.DisputeNo,
                                    ActivityDate = item.ActivityDate,
                                    ActivityType = item.ActivityType,
                                    Forum = item.Forum,
                                    FileName = item.FileName.FileName,
                                    FilePath = filepath,
                                    Remarks = item.Remarks,
                                    SNo = ++z,
                                    CreatedBy = uname
                                };
                                pat.tbl_trx_DisputeActivity.Add(act);
                                item.FileName.SaveAs(filepath + item.FileName.FileName);
                                pat.SaveChanges();
                            }

                            transaction.Commit();
                        }
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string UpdateDispute(DisputeVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {

                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        var dis = pat.tbl_trx_dispute.Where(m => m.DisputeNo == model.DisputeNo).FirstOrDefault();
                        if (dis != null)
                        {
                            dis.DGroup = model.DGroup;
                            dis.DSource = model.DSource;
                            dis.Title = model.Title;
                            dis.PartyName = model.PartyName;
                            dis.EstimatedValue = model.EstimatedValue;
                            dis.RealizationValue = model.RealizationValue;
                            dis.Coordinator = model.Coordinator;
                            dis.Status = model.Status;
                            dis.Remarks = model.Remarks;
                            dis.ModifiedBy = uname;
                            dis.ModifiedOn = DateTime.Today.ToString("dd/MM/yyyy");
                            pat.SaveChanges();
                        }
                        if (model.MDOC.Count > 0)
                        {
                            var mdoc = pat.tbl_trx_DisputeAgreement.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                            int i = 0;
                            int y = 1;
                            if (mdoc.Count == model.MDOC.Count)
                            {
                                foreach (var m in mdoc)
                                {
                                    m.ContractNo = model.MDOC[i].ContractNo;
                                    ++i;
                                    m.Sno = i.ToString();
                                }
                                pat.SaveChanges();
                            }
                            else
                            {
                                var agr = pat.tbl_trx_DisputeAgreement.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                                if (agr != null)
                                {
                                    pat.tbl_trx_DisputeAgreement.RemoveRange(agr);
                                    pat.SaveChanges();
                                }
                                foreach (var m in model.MDOC)
                                {
                                    if (m.ContractNo != null)
                                    {
                                        tbl_trx_DisputeAgreement tmdoc = new tbl_trx_DisputeAgreement()
                                        {
                                            ContractNo = model.MDOC[i].ContractNo,
                                            Sno = y.ToString(),
                                            DisputeNo = model.DisputeNo
                                        }; ++i; ++y;
                                        pat.tbl_trx_DisputeAgreement.Add(tmdoc);
                                    }
                                }
                                pat.SaveChanges();
                            }
                        }
                        else
                        {
                            var mdoc = pat.tbl_trx_DisputeAgreement.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                            if (mdoc != null)
                            {
                                pat.tbl_trx_DisputeAgreement.RemoveRange(mdoc);
                                pat.SaveChanges();
                            }
                        }
                        if (model.Idf.Count > 0)
                        {
                            var idf = pat.tbl_trx_disputeIDF.Where(m => m.DisputeNo == model.DisputeNo).ToList();

                            int i = 0;
                            int y = 1;
                            if (idf.Count == model.Idf.Count)
                            {
                                foreach (var m in idf)
                                {
                                    m.FileNo = model.Idf[i].FileNo;
                                    ++i;
                                    m.Sno = i.ToString();
                                }
                                pat.SaveChanges();
                            }
                            else
                            {
                                var didf = pat.tbl_trx_disputeIDF.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                                if (didf != null)
                                {
                                    pat.tbl_trx_disputeIDF.RemoveRange(didf);
                                    pat.SaveChanges();
                                }
                                foreach (var m in model.Idf)
                                {
                                    if (m.FileNo != null)
                                    {
                                        tbl_trx_disputeIDF tidf = new tbl_trx_disputeIDF()
                                        {
                                            FileNo = model.Idf[i].FileNo,
                                            Sno = y.ToString(),
                                            DisputeNo = model.DisputeNo
                                        }; ++i; ++y;
                                        pat.tbl_trx_disputeIDF.Add(tidf);
                                    }
                                }
                                pat.SaveChanges();
                            }

                        }
                        else
                        {
                            var idf = pat.tbl_trx_disputeIDF.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                            if (idf != null)
                            {
                                pat.tbl_trx_disputeIDF.RemoveRange(idf);
                                pat.SaveChanges();
                            }
                        }
                        if (model.activity.Count > 0)
                        {
                            var act = pat.tbl_trx_DisputeActivity.Where(m => m.DisputeNo == model.DisputeNo).ToList();

                            int i = 0;
                            int y = 1;
                            if (act.Count == model.activity.Count)
                            {

                                foreach (var m in act)
                                {
                                    string filepath = "";
                                    if (model.activity[i].ActivityDate != null)
                                    {
                                        filepath = @"\\DMS_SAN\IPIMS\DisputeFiles\";/*+model.DisputeNo.Trim()+"\\";*/
                                        if (!Directory.Exists(filepath))
                                        {
                                            return "Path Error";
                                            //  Directory.CreateDirectory(
                                        }
                                        filepath = filepath + model.DisputeNo.Trim() + "\\";
                                        if (!Directory.Exists(filepath))
                                        {
                                            Directory.CreateDirectory(@"\\DMS_SAN\IPIMS\DisputeFiles\" + model.DisputeNo.Trim());
                                        }
                                    }
                                    m.ActivityDate = model.activity[i].ActivityDate;
                                    m.ActivityType = model.activity[i].ActivityType;
                                    m.Forum = model.activity[i].Forum;
                                    m.Remarks = model.activity[i].Remarks;
                                    m.ModifiedBy = uname;
                                    m.SNo = y;
                                    m.FilePath = filepath;
                                    if (model.activity[i].FileName != null)
                                    {
                                        m.FileName = model.activity[i].FileName.FileName;
                                        model.activity[i].FileName.SaveAs(filepath + model.activity[i].FileName.FileName);
                                    }

                                    else
                                    {
                                        if (model.activity[i].fn == null)
                                        {
                                            string file = @"\\DMS_SAN\IPIMS\DisputeFiles\" + model.DisputeNo.Trim() + "\\" + m.FileName;
                                            if (System.IO.File.Exists(file))
                                            {
                                                string movefile = @"\\DMS_SAN\IPIMS\DisputeFiles\DeletedDisputeFiles\";
                                                if (Directory.Exists(movefile))
                                                {
                                                    string des = Path.Combine(movefile, m.FileName);
                                                    File.Move(file, des);
                                                }
                                                //System.IO.File.Delete(file);                                                 
                                            }
                                        }
                                        m.FileName = model.activity[i].fn;
                                    }

                                    m.ModifiedOn = DateTime.Now.ToString("dd/MM/yyyy");

                                }
                                pat.SaveChanges();

                            }
                            else
                            {
                                var dact = pat.tbl_trx_DisputeActivity.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                                if (dact != null)
                                {
                                    pat.tbl_trx_DisputeActivity.RemoveRange(dact);
                                    pat.SaveChanges();
                                }
                                foreach (var m in model.activity)
                                {
                                    if (m.ActivityDate != null)
                                    {
                                        tbl_trx_DisputeActivity tact = new tbl_trx_DisputeActivity()
                                        {
                                            ActivityDate = model.activity[i].ActivityDate,
                                            ActivityType = model.activity[i].ActivityType,
                                            CreatedOn = DateTime.Now.ToString("dd/MM/yyyy"),
                                            CreatedBy = uname,
                                            Forum = model.activity[i].Forum,
                                            Remarks = model.activity[i].Remarks,
                                            SNo = y,
                                            DisputeNo = model.DisputeNo
                                        }; ++i; ++y;
                                        pat.tbl_trx_DisputeActivity.Add(tact);
                                    }
                                }
                                pat.SaveChanges();
                            }

                        }
                        else
                        {
                            var idf = pat.tbl_trx_disputeIDF.Where(m => m.DisputeNo == model.DisputeNo).ToList();
                            if (idf != null)
                            {
                                pat.tbl_trx_disputeIDF.RemoveRange(idf);
                                pat.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static List<DisputeVM> GetDisputeDetails()
        {
            List<DisputeVM> dispute = new List<DisputeVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.tbl_trx_dispute.OrderByDescending(m => m.DisputeNo).Select(m => new { m.DisputeNo, m.DGroup, m.Coordinator, m.PartyName, m.Status }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        dispute.Add(new DisputeVM()
                        {
                            DisputeNo = query[i].DisputeNo,
                            DGroup = query[i].DGroup,
                            Coordinator = query[i].Coordinator,
                            PartyName = query[i].PartyName,
                            Status = query[i].Status
                        });
                    }
                }
            }
            return dispute;
        }
        public static DisputeVM EditDispute(string dispno)
        {
            try
            {
                DisputeVM dis = new DisputeVM();
                using (var context = new PatentNewEntities())
                {
                    var query = (from P in context.tbl_trx_dispute
                                 where (P.DisputeNo == dispno)
                                 select P).FirstOrDefault();
                    if (query != null)
                    {
                        dis.DisputeNo = dispno;
                        dis.Title = query.Title;
                        dis.Coordinator = query.Coordinator;
                        dis.DGroup = query.DGroup;
                        dis.DSource = query.DSource;
                        dis.PartyName = query.PartyName;
                        dis.Status = query.Status;
                        dis.EstimatedValue = query.EstimatedValue;
                        dis.RealizationValue = query.RealizationValue;
                        dis.Remarks = query.Remarks;
                        dis.Title = query.Title;
                    }
                    List<AgreementVM> ag = new List<AgreementVM>();
                    var mdoc = context.tbl_trx_DisputeAgreement.Where(m => m.DisputeNo == dispno).Select(m => m.ContractNo).ToList();
                    if (mdoc.Count > 0)
                    {
                        foreach (var cn in mdoc)
                        {
                            var agredetail = context.Agreement.Where(m => m.ContractNo == cn).FirstOrDefault();
                            if (agredetail != null)
                            {
                                ag.Add(new AgreementVM()
                                {
                                    CoordinatingPerson = agredetail.CoordinatingPerson,
                                    ContractNo = agredetail.ContractNo,
                                    Title = agredetail.Title,
                                    Status = agredetail.Status
                                });
                            }
                        }
                        dis.MDOC = ag;
                        //}

                        //
                    }
                    List<IDFRequestVM> file = new List<IDFRequestVM>();
                    var idf = context.tbl_trx_disputeIDF.Where(m => m.DisputeNo == dispno).Select(m => m.FileNo).ToList();
                    if (idf.Count > 0)
                    {
                        foreach (var fl in idf)
                        {
                            var idfdetail = context.tblIDFRequest.Where(m => m.FileNo == fl).FirstOrDefault();
                            if (idfdetail != null)
                            {
                                file.Add(new IDFRequestVM()
                                {
                                    FileNo = idfdetail.FileNo,
                                    PrimaryInventorName = idfdetail.PrimaryInventorName,
                                    Title = idfdetail.Title,
                                    Status = idfdetail.Status
                                    //Applcn_no = idfdetail.Applcn_no
                                });
                            }
                        }
                        dis.Idf = file;
                    }
                    List<DisputeActivity> da = new List<DisputeActivity>();
                    var act = context.tbl_trx_DisputeActivity.Where(m => m.DisputeNo == dispno).ToList();
                    if (act.Count > 0)
                    {
                        foreach (var active in act)
                        {
                            da.Add(new DisputeActivity()
                            {
                                ActivityDate = active.ActivityDate,
                                ActivityType = active.ActivityType,
                                Forum = active.Forum,
                                fn = active.FileName,
                                FilePath = active.FilePath,
                                Remarks = active.Remarks
                            });
                        }
                        //string[] _ad = new string[act.Count];
                        //string[] _at = new string[act.Count];
                        //string[] _forum = new string[act.Count];
                        //string[] _rem = new string[act.Count];
                        //for (int i = 0; i < act.Count; i++)
                        //{
                        //    _at[i] = act[i].ActivityType;
                        //    _ad[i] = act[i].ActivityDate;
                        //    _forum[i] = act[i].Forum;
                        //    _rem[i] = act[i].Remarks;
                        //}
                        //dis.activity.ActivityType = _at;
                        //dis.activity.ActivityDate = _ad;
                        //dis.activity.Forum = _forum;
                        //dis.activity.Remarks = _rem;
                        dis.activity = da;
                    }
                    dis.isUpdate = true;
                    return dis;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DisputeVM DeleteDispute(string dno)
        {
            using (var context = new PatentNewEntities())
            {
                DisputeVM data = new DisputeVM();
                data.Status = "error";
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // int agencyId = 0;
                        var qry = context.tbl_trx_dispute.FirstOrDefault(c => c.DisputeNo == dno);
                        context.tbl_trx_dispute.Remove(qry);
                        context.SaveChanges();
                        var mdoc = context.tbl_trx_DisputeAgreement.Where(m => m.DisputeNo == dno);
                        context.tbl_trx_DisputeAgreement.RemoveRange(mdoc);
                        context.SaveChanges();
                        var idf = context.tbl_trx_disputeIDF.Where(m => m.DisputeNo == dno);
                        context.tbl_trx_disputeIDF.RemoveRange(idf);
                        context.SaveChanges();
                        var act = context.tbl_trx_DisputeActivity.Where(m => m.DisputeNo == dno);
                        context.tbl_trx_DisputeActivity.RemoveRange(act);
                        context.SaveChanges();
                        transaction.Commit();
                        data.Status = "success";
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
        #endregion
        //#region International
        //public static List<InternationalVM> GetInternationalList()
        //{
        //    List<InternationalVM> intl = new List<InternationalVM>();
        //    using (var context = new PatentNewEntities())
        //    {
        //        var query = context.International.Select(m => new { m.FileNo, m.subFileNo, m.Country, m.Partner, m.Status }).ToList();
        //        if (query.Count > 0)
        //        {
        //            for (int i = 0; i < query.Count; i++)
        //            {
        //                intl.Add(new InternationalVM()
        //                {
        //                    FileNo = query[i].FileNo,
        //                    subFileNo = query[i].subFileNo,
        //                    Country = query[i].Country,
        //                    Partner = query[i].Partner,
        //                    Status = query[i].Status
        //                });
        //            }
        //        }
        //    }
        //    return intl;
        //}
        //public static InternationalVM EditInternational(string sfno)
        //{
        //    try
        //    {
        //        InternationalVM intl = new InternationalVM();
        //        using (var context = new PatentNewEntities())
        //        {
        //            var query = context.International.Where(m => m.subFileNo == sfno).FirstOrDefault();
        //            //(from P in context.International where (P.subFileNo == sfno) select P).FirstOrDefault();
        //            if (query != null)
        //            {
        //                intl.FileNo = query.FileNo;
        //                intl.subFileNo = sfno;
        //                intl.ApplicationNo = query.ApplicationNo;
        //                intl.Attorney = query.Attorney;
        //                intl.Commercial = query.Commercial;
        //                intl.Country = query.Country;
        //                intl.Status = query.Status;
        //                intl.FilingDt = query.FilingDt;
        //                //intl.InputDt = query.InputDt;
        //                intl.Remark = query.Remark;
        //                //intl.Partner = query.Partner;
        //                //intl.PartnerNo = query.PartnerNo;
        //                intl.PatentDt = query.PatentDt;
        //                intl.PatentNo = query.PatentNo;
        //                intl.PublicationDt = query.PublicationDt;
        //                intl.PublicationNo = query.PublicationNo;
        //                //intl.RequestDt = query.RequestDt;
        //                intl.SubStatus = query.SubStatus;
        //                //intl.Type = query.Type;
        //                intl.isUpdate = true;
        //            }
        //            return intl;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //public static string InsertInternational(InternationalVM model, string uname)
        //{

        //    using (PatentNewEntities pat = new PatentNewEntities())
        //    {

        //        using (var transaction = pat.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                International intl = new International()
        //                {
        //                    FileNo = model.FileNo,
        //                    subFileNo = model.subFileNo,
        //                    ApplicationNo = model.ApplicationNo,
        //                    Attorney = model.Attorney,
        //                    Commercial = model.Commercial,
        //                    Country = model.Country,
        //                    FilingDt = model.FilingDt,
        //                    InputDt = model.InputDt,
        //                    Partner = model.Partner,
        //                    PartnerNo = model.PartnerNo,
        //                    CreatedOn = DateTime.Now.Date,
        //                    PatentDt = model.PatentDt,
        //                    PatentNo = model.PatentNo,
        //                    PublicationDt = model.PublicationDt,
        //                    PublicationNo = model.PublicationNo,
        //                    Remark = model.Remark,
        //                    RequestDt = model.RequestDt,
        //                    SubStatus = model.SubStatus,
        //                    Type = model.Type,
        //                    UserName = uname,
        //                    Status = model.Status
        //                };
        //                pat.International.Add(intl);
        //                pat.SaveChanges();
        //                transaction.Commit();
        //                return "Success";
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                return ex.ToString();
        //            }
        //        }
        //    }
        //}
        //public static string UpdateInternational(InternationalVM model, string uname)
        //{
        //    using (PatentNewEntities pat = new PatentNewEntities())
        //    {

        //        using (var transaction = pat.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var intl = pat.International.Where(m => m.subFileNo == model.subFileNo).FirstOrDefault();
        //                if (intl != null)
        //                {
        //                    intl.FileNo = model.FileNo;
        //                    intl.subFileNo = model.subFileNo;
        //                    intl.ApplicationNo = model.ApplicationNo;
        //                    intl.Attorney = model.Attorney;
        //                    intl.Commercial = model.Commercial;
        //                    intl.Country = model.Country;
        //                    intl.FilingDt = model.FilingDt;
        //                    intl.Status = model.Status;
        //                    intl.InputDt = model.InputDt;
        //                    intl.PartnerNo = model.PartnerNo;
        //                    intl.PatentDt = model.PatentDt;
        //                    intl.PatentNo = model.PatentNo;
        //                    intl.PublicationDt = model.PublicationDt;
        //                    intl.PublicationNo = model.PublicationNo;
        //                    intl.Remark = model.Remark;
        //                    intl.RequestDt = model.RequestDt;
        //                    intl.SubStatus = model.SubStatus;
        //                    intl.Type = model.Type;
        //                    intl.ModifiedBy = uname;
        //                    intl.Partner = model.Partner;
        //                    intl.ModifiedOn = DateTime.Today.Date;
        //                    pat.SaveChanges();
        //                }
        //                transaction.Commit();
        //                return "Success";
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                return ex.ToString();
        //            }
        //        }
        //    }
        //}
        //#endregion
        #region DueDiligence
        public static List<DueDiligenceVM> GetDuediligenceList()
        {
            List<DueDiligenceVM> dd = new List<DueDiligenceVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.tbl_trx_duediligence.OrderByDescending(m => m.CreatedOn).Select(m => new { m.FileNo, m.Sno, m.ReportType, m.Allocation, m.Mode }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        dd.Add(new DueDiligenceVM()
                        {
                            FileNo = query[i].FileNo,
                            Sno = query[i].Sno,
                            ReportType = query[i].ReportType,
                            Allocation = query[i].Allocation,
                            Mode = query[i].Mode
                        });
                    }
                }
            }
            return dd;
        }
        public static DueDiligenceVM EditDueDiligence(long fno, int sno)
        {
            try
            {
                DueDiligenceVM dd = new DueDiligenceVM();
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_duediligence.Where(m => m.FileNo == fno && m.Sno == sno).FirstOrDefault();
                    if (query != null)
                    {
                        dd.FileNo = query.FileNo;
                        dd.Sno = sno;
                        dd.Allocation = query.Allocation;
                        dd.Comment = query.Comment;
                        dd.Followup = query.Followup;
                        dd.InventorInput = query.InventorInput;
                        dd.IPCCode = query.IPCCode;
                        dd.Mode = query.Mode;
                        dd.Participants = query.Participants;
                        dd.ReportDt = query.ReportDt;
                        dd.ReportType = query.ReportType;
                        dd.RequestDt = query.RequestDt;
                        dd.SRNo = query.SRNo;
                        dd.Summary = query.Summary;
                        dd.TechnologyAction = query.TechnologyAction;
                        dd.isUpdate = true;
                        dd.fn = query.FileName;
                        dd.FilePath = query.FilePath;
                    }
                    return dd;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string InsertDueDiligence(DueDiligenceVM model, string uname)
        {

            using (PatentNewEntities pat = new PatentNewEntities())
            {

                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        string fp = null;
                        if (model.FileName != null)
                        {
                            fp = pat.tbl_mst_filepath.Where(m => m.Category == "PatentDocument").Select(m => m.FilePath).FirstOrDefault();
                            if (fp != null && !Directory.Exists(fp))
                            {
                                return "Path Error";
                                //  Directory.CreateDirectory(
                            }
                            fp = fp + model.FileNo + "\\DueDiligence\\";
                            if (!Directory.Exists(fp))
                            {
                                Directory.CreateDirectory(fp);
                            }
                        }
                        tbl_trx_duediligence dd = new tbl_trx_duediligence()
                        {
                            FileNo = model.FileNo,
                            Allocation = model.Allocation,
                            Comment = model.Comment,
                            EntryDt = DateTime.Now.Date,
                            Followup = model.Followup,
                            InventorInput = model.InventorInput,
                            IPCCode = model.IPCCode,
                            Mode = model.Mode,
                            Participants = model.Participants,
                            ReportDt = model.ReportDt,
                            CreatedOn = DateTime.Now.Date,
                            ReportType = model.ReportType,
                            Sno = model.Sno,
                            SRNo = model.SRNo,
                            Summary = model.Summary,
                            TechnologyAction = model.TechnologyAction,
                            RequestDt = model.RequestDt,
                            FileName = model.FileName.FileName,
                            FilePath = fp,
                            CreatedBy = uname
                        };
                        pat.tbl_trx_duediligence.Add(dd);
                        pat.SaveChanges();
                        if (model.FileName != null)
                        {
                            model.FileName.SaveAs(fp + model.FileName.FileName);
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string UpdateDueDiligence(DueDiligenceVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        var ddvm = pat.tbl_trx_duediligence.Where(m => m.FileNo == model.FileNo && m.Sno == model.Sno).FirstOrDefault();
                        if (ddvm != null)
                        {
                            ddvm.Allocation = model.Allocation;
                            ddvm.Comment = model.Comment;
                            ddvm.Followup = model.Followup;
                            ddvm.InventorInput = model.InventorInput;
                            ddvm.IPCCode = model.IPCCode;
                            ddvm.Mode = model.Mode;
                            ddvm.Participants = model.Participants;
                            ddvm.ReportDt = model.ReportDt;
                            ddvm.ReportType = model.ReportType;
                            ddvm.ModifiedBy = uname;
                            ddvm.ModifiedOn = DateTime.Now.Date;
                            ddvm.RequestDt = model.RequestDt;
                            ddvm.Sno = model.Sno;
                            ddvm.SRNo = model.SRNo;
                            ddvm.Summary = model.Summary;
                            ddvm.TechnologyAction = model.TechnologyAction;
                            ddvm.EntryDt = model.EntryDt;
                            if (model.FileName != null)
                            {
                                ddvm.FileName = model.FileName.FileName;
                                string fp = pat.tbl_mst_filepath.Where(m => m.Category == "PatentDocument").Select(m => m.FilePath).FirstOrDefault();
                                if (fp != null && !Directory.Exists(fp))
                                {
                                    return "Path Error";
                                    //  Directory.CreateDirectory(
                                }
                                fp = fp + model.FileNo + "\\DueDiligence\\";
                                if (!Directory.Exists(fp))
                                {
                                    Directory.CreateDirectory(fp + model.FileNo + "\\DueDiligence\\");
                                }
                                model.FileName.SaveAs(fp + model.FileName.FileName);
                                ddvm.FilePath = fp;
                            }
                            pat.SaveChanges();
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string DeleteDuediligence(long dno, int sno)
        {
            using (var context = new PatentNewEntities())
            {
                string status = "Error";
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // int agencyId = 0;

                        var qry = context.tbl_trx_duediligence.FirstOrDefault(c => c.FileNo == dno && c.Sno == sno);
                        var ofile = qry.FileName;
                        var file = Path.Combine(qry.FilePath, ofile);
                        context.tbl_trx_duediligence.Remove(qry);
                        context.SaveChanges();
                        if (file != null)
                        {
                            var newpath = context.tbl_mst_filepath.Where(m => m.Category == "DeleteFiles").Select(m => m.FilePath).FirstOrDefault();
                            if (newpath != null)
                            {
                                newpath = Path.Combine(newpath, dno.ToString().Trim());
                                if (!Directory.Exists(newpath))
                                {
                                    Directory.CreateDirectory(newpath);
                                }
                                var newfile = Path.Combine(newpath, ofile);
                                File.Move(file, newfile);
                                status = "Success";
                            }
                            else
                            {
                                transaction.Rollback();
                                return status;
                            }

                        }
                        else
                            status = "Success";
                        transaction.Commit();
                        //data.Status = "success";
                        return status;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return status;
                    }
                }
            }
        }
        #endregion
        #region Receipt
        public static ReceiptVM EditReceipt(string rno)
        {
            try
            {
                ReceiptVM rec = new ReceiptVM();
                using (var context = new PatentNewEntities())
                {
                    var query = (from P in context.tbl_primary_receipt
                                 where (P.ReceiptNo == rno)
                                 select P).FirstOrDefault();
                    if (query != null)
                    {
                        rec.ReceiptNo = query.ReceiptNo;
                        rec.Accno = query.Accno;
                        rec.AmountINR = query.AmountINR;
                        rec.Comment = query.Comment;
                        rec.IntimationDt = query.IntimationDt;
                        rec.IntimationRef = query.IntimationRef;
                        rec.IPAccno = query.IPAccno;
                        rec.Party = query.Party;
                        rec.PartyRefNo = query.PartyRefNo;
                        rec.ReceiptDesc = query.ReceiptDesc;
                        rec.ReceiptDt = query.ReceiptDt;
                        rec.ReceiptRef = query.ReceiptRef;
                        rec.Source = query.Source;
                        rec.TransferAmt = query.TransferAmt;
                        rec.TransferDt = query.TransferDt;
                    }
                    List<ReceiptSecVM> r = new List<ReceiptSecVM>();
                    var secreceipt = context.tbl_sec_receiptfileno.Where(m => m.ReceiptNo == rno).ToList();
                    if (secreceipt.Count > 0)
                    {
                        foreach (var sec in secreceipt)
                        {
                            r.Add(new ReceiptSecVM()
                            {
                                FileNo = sec.FileNo,
                                Remarks = sec.Remarks,
                                RGroup = sec.RGroup,
                                SlNo = sec.SlNo,
                                SplitAmtInr = sec.SplitAmtInr,
                                Title = sec.Title,
                                ReceiptNo = rno
                            });
                        }
                        rec.RDetail = r;
                    }
                    rec.isUpdate = true;
                    return rec;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string InsertReceipt(ReceiptVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        tbl_primary_receipt rec = new tbl_primary_receipt()
                        {
                            Accno = model.Accno,
                            AmountINR = model.AmountINR,
                            Comment = model.Comment,
                            IntimationDt = model.IntimationDt,
                            IntimationRef = model.IntimationRef,
                            IPAccno = model.IPAccno,
                            Party = model.Party,
                            PartyRefNo = model.PartyRefNo,
                            ReceiptDesc = model.ReceiptDesc,
                            ReceiptDt = model.ReceiptDt,
                            ReceiptNo = model.ReceiptNo,
                            ReceiptRef = model.ReceiptRef,
                            Source = model.Source,
                            TransferAmt = model.TransferAmt,
                            TransferDt = model.TransferDt,
                            CreatedOn = DateTime.Now.Date,
                            CreatedBy = uname
                        };
                        pat.tbl_primary_receipt.Add(rec);
                        pat.SaveChanges();
                        int i = 1;
                        foreach (var rd in model.RDetail)
                        {
                            if (rd.FileNo != null)
                            {
                                tbl_sec_receiptfileno secreceipt = new tbl_sec_receiptfileno()
                                {
                                    SlNo = i,
                                    ReceiptNo = model.ReceiptNo,
                                    FileNo = rd.FileNo,
                                    Remarks = rd.Remarks,
                                    RGroup = rd.RGroup,
                                    SplitAmtInr = rd.SplitAmtInr,
                                    Title = rd.Title,
                                    CreatedBy = uname,
                                    CreatedOn = DateTime.Now
                                };
                                pat.tbl_sec_receiptfileno.Add(secreceipt);
                                pat.SaveChanges();
                            }
                            ++i;
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string UpdateReceipt(ReceiptVM model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {

                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        var rec = pat.tbl_primary_receipt.Where(m => m.ReceiptNo == model.ReceiptNo).FirstOrDefault();
                        if (rec != null)
                        {
                            rec.Accno = model.Accno;
                            rec.AmountINR = model.AmountINR;
                            rec.Comment = model.Comment;
                            rec.IntimationDt = model.IntimationDt;
                            rec.IntimationRef = model.IntimationRef;
                            rec.IPAccno = model.IPAccno;
                            rec.Party = model.Party;
                            rec.PartyRefNo = model.PartyRefNo;
                            rec.ReceiptDesc = model.ReceiptDesc;
                            rec.ReceiptDt = model.ReceiptDt;
                            rec.ReceiptNo = model.ReceiptNo;
                            rec.ReceiptRef = model.ReceiptRef;
                            rec.TransferAmt = model.TransferAmt;
                            rec.TransferDt = model.TransferDt;
                            rec.ModifiedBy = uname;
                            rec.ModifiedOn = DateTime.Now.Date;
                            pat.SaveChanges();
                        }
                        if (model.RDetail.Count > 0)
                        {
                            var secreceipt = pat.tbl_sec_receiptfileno.Where(m => m.ReceiptNo == model.ReceiptNo).ToList();
                            int i = 0; int y = 1;
                            if (secreceipt.Count == model.RDetail.Count)
                            {
                                foreach (var m in secreceipt)
                                {
                                    m.FileNo = model.RDetail[i].FileNo;
                                    m.ReceiptNo = model.ReceiptNo;
                                    m.RGroup = model.RDetail[i].RGroup;
                                    m.Remarks = model.RDetail[i].Remarks;
                                    m.SplitAmtInr = model.RDetail[i].SplitAmtInr;
                                    m.Title = model.RDetail[i].Title;
                                    ++i;
                                    m.SlNo = i;
                                }
                                pat.SaveChanges();
                            }
                            else
                            {
                                if (secreceipt != null)
                                {
                                    pat.tbl_sec_receiptfileno.RemoveRange(secreceipt);
                                    pat.SaveChanges();
                                }
                                foreach (var m in model.RDetail)
                                {
                                    if (m.FileNo != null)
                                    {
                                        tbl_sec_receiptfileno tblsec = new tbl_sec_receiptfileno()
                                        {
                                            ReceiptNo = model.ReceiptNo,
                                            FileNo = model.RDetail[i].FileNo,
                                            Remarks = model.RDetail[i].Remarks,
                                            RGroup = model.RDetail[i].RGroup,
                                            SplitAmtInr = model.RDetail[i].SplitAmtInr,
                                            Title = model.RDetail[i].Title,
                                            SlNo = y
                                        }; ++i; ++y;
                                        pat.tbl_sec_receiptfileno.Add(tblsec);
                                    }
                                }
                                pat.SaveChanges();
                            }
                        }
                        else
                        {
                            var srec = pat.tbl_sec_receiptfileno.Where(m => m.ReceiptNo == model.ReceiptNo).ToList();
                            if (srec != null)
                            {
                                pat.tbl_sec_receiptfileno.RemoveRange(srec);
                                pat.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static List<ReceiptVM> GetReceiptList()
        {
            List<ReceiptVM> rec = new List<ReceiptVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.tbl_primary_receipt.Select(m => new { m.ReceiptNo, m.Party, m.Source, m.AmountINR, m.Accno }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        rec.Add(new ReceiptVM()
                        {
                            ReceiptNo = query[i].ReceiptNo,
                            Party = query[i].Party,
                            Source = query[i].Source,
                            AmountINR = query[i].AmountINR,
                            Accno = query[i].Accno
                        });
                    }
                }
            }
            return rec;
        }
        #endregion
        #region ServiceRequest
        public static List<ServiceRequestVM> EditServiceRequest(string srno)
        {
            try
            {
                List<ServiceRequestVM> rec = new List<ServiceRequestVM>();
                using (var context = new PatentNewEntities())
                {
                    var query = (from P in context.tbl_trx_servicerequest
                                 where (P.SRNo == srno)
                                 select P).ToList();
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            rec.Add(new ServiceRequestVM()
                            {
                                SRNo = item.SRNo,
                                Action = item.Action,
                                ActualDt = item.ActualDt,
                                AttorneyID = item.AttorneyID,
                                FileNo = item.FileNo,
                                IntimationDt = item.IntimationDt,
                                isUpdate = true,
                                MDocNo = item.MDocNo,
                                Remarks = item.Remarks,
                                Share = item.Share,
                                SharingParty = item.SharingParty,
                                Sno = item.Sno,
                                Status = item.Status,
                                TargetDt = item.TargetDt
                            });
                        }
                    }
                    return rec;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string InsertServiceRequest(List<ServiceRequestVM> model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {

                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        //Int16 sno = 1;
                        foreach (var item in model)
                        {
                            if (item.FileNo != 0)
                            {
                                tbl_trx_servicerequest trec = new tbl_trx_servicerequest()
                                {
                                    SRNo = model[0].SRNo,
                                    AttorneyID = model[0].AttorneyID,
                                    IntimationDt = model[0].IntimationDt,
                                    Action = item.Action,
                                    FileNo = item.FileNo,
                                    MDocNo = item.MDocNo,
                                    ActualDt = item.ActualDt,
                                    Remarks = item.Remarks,
                                    Share = item.Share,
                                    SharingParty = item.SharingParty,
                                    Sno = item.Sno,
                                    Status = item.Status,
                                    TargetDt = item.TargetDt,
                                    CreatedBy = uname,
                                    CreatedOn = DateTime.Now.Date
                                };
                                pat.tbl_trx_servicerequest.Add(trec);
                                pat.SaveChanges();
                                //++sno;
                            }
                        }
                        transaction.Commit();
                        return "Success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static string UpdateServiceRequest(List<ServiceRequestVM> model, string uname)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                using (var transaction = pat.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.Count > 0)
                        {
                            var srno = model[0].SRNo;
                            var at = model[0].AttorneyID;
                            var intdt = model[0].IntimationDt;
                            var req = pat.tbl_trx_servicerequest.Where(m => m.SRNo == srno).ToList();
                            if (req.Count == model.Count)
                            {
                                foreach (var item in model)
                                {
                                    var sr = pat.tbl_trx_servicerequest.Where(m => m.SRNo == srno && m.Sno == item.Sno).FirstOrDefault();

                                    if (sr != null)
                                    {
                                        sr.Sno = item.Sno;
                                        sr.Action = item.Action;
                                        sr.ActualDt = item.ActualDt;
                                        sr.AttorneyID = model[0].AttorneyID;
                                        sr.FileNo = item.FileNo;
                                        sr.IntimationDt = model[0].IntimationDt;
                                        sr.MDocNo = item.MDocNo;
                                        sr.Remarks = item.Remarks;
                                        sr.Share = item.Share;
                                        sr.SharingParty = item.SharingParty;
                                        sr.Status = item.Status;
                                        sr.TargetDt = item.TargetDt;
                                        sr.UpdatedBy = uname;
                                        sr.UpdatedOn = DateTime.Now.Date;
                                    }
                                    pat.SaveChanges();
                                }
                            }
                            else
                            {
                                if (req != null)
                                {
                                    pat.tbl_trx_servicerequest.RemoveRange(req);
                                    pat.SaveChanges();
                                }
                                foreach (var m in model)
                                {
                                    if (m.FileNo != 0)
                                    {
                                        tbl_trx_servicerequest tsr = new tbl_trx_servicerequest()
                                        {
                                            Action = m.Action,
                                            ActualDt = m.ActualDt,
                                            FileNo = m.FileNo,
                                            AttorneyID = at,
                                            IntimationDt = intdt,
                                            MDocNo = m.MDocNo,
                                            Remarks = m.Remarks,
                                            Share = m.Share,
                                            SharingParty = m.SharingParty,
                                            Sno = m.Sno,
                                            SRNo = srno,
                                            Status = m.Status,
                                            TargetDt = m.TargetDt,
                                            UpdatedBy = uname,
                                            UpdatedOn = DateTime.Now
                                        };
                                        pat.tbl_trx_servicerequest.Add(tsr);
                                    }
                                    else
                                    {
                                        pat.tbl_trx_servicerequest.RemoveRange(req);
                                        pat.SaveChanges();
                                    }
                                }
                                pat.SaveChanges();
                            }


                            transaction.Commit();
                            return "Success";
                        }
                        else { return "No rows to update"; }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ex.ToString();
                    }
                }
            }
        }
        public static List<ServiceRequestVM> GetSRList()
        {
            List<ServiceRequestVM> sr = new List<ServiceRequestVM>();
            using (var context = new PatentNewEntities())
            {
                var query = context.tbl_trx_servicerequest.OrderByDescending(m => m.trx_id).ThenByDescending(m => m.SRNo).Select(m => new { m.SRNo, m.Sno, m.AttorneyID, m.FileNo, m.Action }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        sr.Add(new ServiceRequestVM()
                        {
                            SRNo = query[i].SRNo,
                            Sno = query[i].Sno,
                            AttorneyID = query[i].AttorneyID,
                            Action = query[i].Action,
                            FileNo = query[i].FileNo
                        });
                    }
                }
            }
            return sr;
        }
        public static string DeleteServiceRequest(string srno,int sno)
        {
            using (var context = new PatentNewEntities())
            {
                string status = "Error";
                try
                {
                    var qry = context.tbl_trx_servicerequest.FirstOrDefault(c => c.SRNo == srno && c.Sno == sno);
                    context.tbl_trx_servicerequest.Remove(qry);
                    context.SaveChanges();
                    status = "Success";
                    //data.Status = "success";
                    return status;
                }
                catch (Exception ex)
                {
                    return status;
                }
            }
        }
        public static List<SRReportVM> SRReport(string srno)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    //SRReportListVM vm = new SRReportListVM();
                    List<SRReportVM> list = new List<SRReportVM>();
                    list = pat.tbl_trx_servicerequest.Where(m => m.SRNo == srno).Select(m => new SRReportVM()
                    {
                        SRNo = m.SRNo,
                        FileNo = m.FileNo,
                        Action = m.Action,
                        ActualDt = m.ActualDt ?? DateTime.Now,
                        AttorneyID = m.AttorneyID,
                        CreatedBy = m.CreatedBy,
                        CreatedOn = m.CreatedOn ?? DateTime.Now,
                        IntimationDt = m.IntimationDt ?? DateTime.Now,
                        MDocNo = m.MDocNo,
                        Remarks = m.Remarks,
                        Share = m.Share ?? 0,
                        SharingParty = m.SharingParty,
                        Sno = m.Sno,
                        Status = m.Status,
                        TargetDt = m.TargetDt ?? DateTime.Now,
                        trx_id = m.trx_id,
                        UpdatedBy = m.UpdatedBy,
                        UpdatedOn = m.UpdatedOn ?? DateTime.Now
                    }).ToList();
                    //List<SRReportListVM> srlist = new List<SRReportListVM>() { vm };
                    return list;
                }
                catch (Exception ex) { return null; }
            }
        }
        public static List<IDFRequestVM> SRIDFReport(long fno)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    List<IDFRequestVM> list = new List<IDFRequestVM>();
                    list = pat.tblIDFRequest.Where(m => m.FileNo == fno).Select(m => new IDFRequestVM()
                    {
                        FileNo = m.FileNo,
                        PIContactNo = m.PIContactNo,
                        PIDepartment = m.PIDepartment,
                        PIEmailId = m.PIEmailId,
                        PrimaryInventorName = m.PrimaryInventorName,
                        PrimaryInventorType = m.PrimaryInventorType,
                        Title = m.Title
                    }).ToList();
                    return list;
                }
                catch (Exception ex) { return null; }
            }
        }
        public static List<CoInventorVM> SRCoReport(long fno)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    List<CoInventorVM> list = new List<CoInventorVM>();
                    list = pat.tblCoInventor.Where(m => m.FileNo == fno).Select(m => new CoInventorVM()
                    {
                        FileNo = m.FileNo,
                        Name = m.Name,
                        Dept = m.Dept,
                        Mail = m.EmailId,
                        Ph = m.ContactNo,
                        SNo = m.SNo,
                        Type = m.Type
                    }).ToList();
                    return list;
                }
                catch (Exception ex) { return null; }
            }
        }
        public static string GetSRAttorney(string Srno)
        {
            using (PatentNewEntities pat = new PatentNewEntities())
            {
                try
                {
                    var att = pat.tbl_trx_servicerequest.Where(m => m.SRNo == Srno).Select(m => m.AttorneyID).FirstOrDefault();
                    return att;
                }
                catch (Exception ex) { return null; }
            }
        }

        #endregion
        #region IDF Approval 
        public static bool POWFInit(long fno, int logged_in_user, int vid)
        {
            try
            {
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_IDFRequest.FirstOrDefault(m => m.FileNo == fno && m.Status == "InProcess" && m.VersionId == vid);
                    if (query != null)
                    {
                        int FileNo = Convert.ToInt32(fno);
                        int pgId = 203;
                        var fw = FlowEngine.Init(pgId, logged_in_user, FileNo, "FileNo");
                        string url = string.Empty;
                        url = "/Patent/IDFRequest?ReqNo=" + FileNo + "&vid=" + vid;
                        fw.ActionLink(url);
                        fw.FailedMethod("IDFInitFailure");
                        fw.ClarifyMethod("IDFInitClarify");
                        fw.SuccessMethod("IDFInitSuccess");
                        fw.ProcessInit();
                        if (String.IsNullOrEmpty(fw.errorMsg))
                        {
                            query.Status = "Recommended by IPAdmin";
                            query.ModifiedBy = logged_in_user.ToString();
                            query.ModifiedOn = DateTime.Now;
                            var q1 = context.tbl_trx_IDFRequest.Where(m => m.FileNo == fno && m.VersionId != vid).ToList();
                            foreach (var item in q1)
                            {
                                item.Status = "Version " + vid + " is recommended";
                                item.ModifiedBy = logged_in_user.ToString();
                                item.ModifiedOn = DateTime.Now;
                            }
                            var q = context.tblIDFRequest.Where(m => m.FileNo == fno).FirstOrDefault();
                            if (q != null)
                            {
                                q.Status = "Recommended by IPAdmin";
                            }
                            context.SaveChanges();
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IDFInitFailure(long fno, int loggedInUser)
        {
            try
            {
                using (var context = new PatentNewEntities())
                {
                    var query = context.tbl_trx_IDFRequest.Where(m => m.FileNo == fno && m.Status == "Submit for approval").FirstOrDefault();
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
        public static bool LockEditOption(long fno, int vid)
        {
            try
            {
                using (PatentNewEntities pat = new PatentNewEntities())
                {
                    var q1 = pat.tbl_trx_IDFRequest.FirstOrDefault(m => m.FileNo == fno && m.VersionId == vid);
                    if (q1 != null)
                    {
                        q1.Status = "InProcess";
                        var q = pat.tblIDFRequest.FirstOrDefault(m => m.FileNo == fno);
                        if (q != null)
                        {
                            q.Status = "InProcess";
                            pat.SaveChanges();
                        }
                        else { return false; }
                    }
                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
        #endregion

    }
}