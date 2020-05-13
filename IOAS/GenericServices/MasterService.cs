using IOAS.DataModel;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class MasterService
    {
        public static VendorSearchModel GetVendorList(VendorSearchModel model, int page, int pageSize)
        {
            try
            {
                VendorSearchModel list = new VendorSearchModel();
                List<VendorMasterViewModel> getVendor = new List<VendorMasterViewModel>();
                using (var context = new IOASDBEntities())
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
                    var query = (from V in context.tblVendorMaster
                                 join C in context.tblCountries on V.Country equals C.countryID into CO
                                 from cl in CO.DefaultIfEmpty()
                                 orderby V.VendorId descending
                                 where V.Status == "Active"
                                 && (V.Name.Contains(model.INVendorSearchname) || model.INVendorSearchname == null)
                                 && (V.VendorCode.Contains(model.INVendorsearchCode) || model.INVendorsearchCode == null)
                                 && (V.Country == model.EXCountryName || model.EXCountryName == null)
                                 && (V.Name.Contains(model.EXVendorSearchname) || model.EXVendorSearchname == null)
                                 && (V.VendorCode.Contains(model.EXINVendorsearchCode) || model.EXINVendorsearchCode == null)
                                 select new { V.VendorId, V.VendorCode, V.Name, V.Country, cl.countryName }).Skip(skiprec).Take(pageSize).ToList();
                    list.TotalRecords = (from V in context.tblVendorMaster
                                         join C in context.tblCountries on V.Country equals C.countryID into CO
                                         from cl in CO.DefaultIfEmpty()
                                         orderby V.VendorId descending
                                         where V.Status == "Active"
                                        && (V.Name.Contains(model.INVendorSearchname) || model.INVendorSearchname == null)
                                  && (V.VendorCode.Contains(model.INVendorsearchCode) || model.INVendorsearchCode == null)
                                  && (V.Country == model.EXCountryName || model.EXCountryName == null)
                                  && (V.Name.Contains(model.EXVendorSearchname) || model.EXVendorSearchname == null)
                                  && (V.VendorCode.Contains(model.EXINVendorsearchCode) || model.EXINVendorsearchCode == null)
                                         select new { V.VendorId, V.VendorCode, V.Name, V.Country, cl.countryName }).Count();
                    if (query.Count > 0)
                    {

                        for (int i = 0; i < query.Count; i++)
                        {
                            var countrylist = "";
                            if (query[i].Country == 128)
                            {
                                countrylist = "INDIA";
                            }
                            else
                            {
                                countrylist = query[i].countryName;
                            }
                            int sno = i + 1;
                            getVendor.Add(new VendorMasterViewModel()
                            {
                                sno = sno,
                                VendorId = query[i].VendorId,
                                Name = query[i].Name,
                                VendorCode = query[i].VendorCode,
                                CountryName = countrylist
                            });
                        }
                    }
                    list.VendorList = getVendor;
                    return list;
                }
            }
            catch (Exception ex)
            {
                VendorSearchModel list = new VendorSearchModel();
                return list;
            }
        }
        
        public static int VendorMaster(VendorMasterViewModel model)
        {

            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {

                        if (model.VendorId == null || model.VendorId == 0)
                        {

                            tblVendorMaster regvendor = new tblVendorMaster();
                            if (model.PFMSVendorCode != null)
                            {
                                var chkvendor = context.tblVendorMaster.FirstOrDefault(M => M.PFMSVendorCode == model.PFMSVendorCode && M.Status == "Active");
                                if (chkvendor != null)
                                    return 4;

                            }
                            if (model.AccountNumber != null)
                            {
                                var chkAccNo = context.tblVendorMaster.FirstOrDefault(M => M.AccountNumber == model.AccountNumber && M.Status == "Active");
                                if (chkAccNo != null)
                                    return 2;
                            }
                            var sqnbr = (from S in context.tblVendorMaster
                                         select S.SeqNbr
                                       ).Max();
                            regvendor.Nationality = model.Nationality;
                            regvendor.VendorCode = 'V' + (Convert.ToInt32(sqnbr) + 1).ToString("00000");
                            regvendor.SeqNbr = (Convert.ToInt32(sqnbr) + 1);
                            regvendor.PFMSVendorCode = model.PFMSVendorCode;
                            regvendor.Name = model.Name;
                            regvendor.Address = model.Address;
                            regvendor.Email = model.Email;
                            regvendor.ContactPerson = model.ContactPerson;
                            regvendor.PhoneNumber = model.PhoneNumber;
                            regvendor.MobileNumber = model.MobileNumber;
                            regvendor.City = model.City;
                            regvendor.Pincode = model.PinCode;
                            if (model.CountryId != null)
                            {
                                regvendor.Country = model.CountryId;

                            }
                            else
                            {
                                regvendor.Country = 128;
                            }
                            if (model.StateId != 0)
                            {
                                regvendor.StateId = model.StateId;
                            }
                            regvendor.RegisteredName = model.RegisteredName;
                            regvendor.PAN = model.PAN;
                            regvendor.TAN = model.TAN;
                            regvendor.GSTExempted = model.GSTExempted;
                            regvendor.Reason = model.Reason;
                            regvendor.GSTIN = model.GSTIN;
                            regvendor.AccountHolderName = model.AccountHolderName;
                            regvendor.BankName = model.BankName;
                            regvendor.Branch = model.Branch;
                            regvendor.IFSC = model.IFSCCode;
                            regvendor.AccountNumber = model.AccountNumber;
                            regvendor.BankAddress = model.BankAddress;
                            regvendor.ABANumber = model.ABANumber;
                            regvendor.SortCode = model.SortCode;
                            regvendor.IBAN = model.IBAN;
                            regvendor.SWIFTorBICCode = model.SWIFTorBICCode;
                            regvendor.Category = model.ServiceCategory;
                            regvendor.ServiceType = model.ServiceType;
                            regvendor.SupplyType = model.SupplierType;
                            regvendor.ReverseTax = model.ReverseTax;
                            regvendor.TDSExcempted = model.TDSExcempted;
                            regvendor.ReasonForReservieTax = model.ReverseTaxReason;
                            regvendor.CertificateNumber = model.CertificateNumber;
                            regvendor.ValidityPeriod = model.ValidityPeriod;
                            regvendor.BankNature = model.BankNature;
                            regvendor.BankEmailId = model.BankEmailId;
                            regvendor.MICRCode = model.MICRCode;
                            regvendor.CRTD_UserID = model.UserId;
                            regvendor.CRTD_TS = DateTime.Now;
                            regvendor.Status = "Active";
                            regvendor.StateCode = model.StateCode;
                            context.tblVendorMaster.Add(regvendor);
                            context.SaveChanges();
                            var vendoId = regvendor.VendorId;
                            if (model.GSTAttachName[0] != "")
                            {
                                for (int i = 0; i < model.GSTDocumentType.Length; i++)
                                {
                                    if (model.GSTDocumentType[i] != 0)
                                    {


                                        //if (query.Count == 0)
                                        //{
                                        string docgstpath = "";
                                        docgstpath = System.IO.Path.GetFileName(model.GSTFile[i].FileName);
                                        var docgstfileId = Guid.NewGuid().ToString();
                                        var docgstname = docgstfileId + "_" + docgstpath;

                                        /*Saving the file in server folder*/
                                        model.GSTFile[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/GstDocument/" + docgstname));
                                        tblGstDocument document = new tblGstDocument();
                                        document.VendorId = vendoId;
                                        if (model.GSTFile[i] != null)
                                        {
                                            document.GstVendorDocument = model.GSTFile[i].FileName;

                                        }
                                        document.GstAttachmentPath = docgstname;
                                        document.GstAttachmentName = model.GSTAttachName[i];
                                        document.GstDocumentType = model.GSTDocumentType[i];
                                        document.GstDocumentUploadUserId = model.UserId;
                                        document.GstDocumentUpload_Ts = DateTime.Now;
                                        context.tblGstDocument.Add(document);
                                        context.SaveChanges();
                                        //}
                                        //else
                                        //{
                                        //    query[0].GstDocumentType = model.GSTDocumentType[i];
                                        //    query[0].GstAttachmentName = model.GSTAttachName[i];
                                        //    query[0].GstDocumentUploadUserId = model.UserId;
                                        //    query[0].GstDocumentUpload_Ts = DateTime.Now;
                                        //    query[0].IsCurrentVersion = false;
                                        //    context.SaveChanges();
                                        //}
                                    }
                                }
                            }

                            if (model.VendorAttachName[0] != "")
                            {
                                for (int i = 0; i < model.VendorDocumentType.Length; i++)
                                {
                                    if (model.VendorDocumentType[i] != 0)
                                    {
                                        //var docid = model.VendorDocumentId[i];

                                        //if (query.Count == 0)
                                        //{
                                        string doctaxpath = "";
                                        doctaxpath = System.IO.Path.GetFileName(model.VendorFile[i].FileName);
                                        var doctaxfileId = Guid.NewGuid().ToString();
                                        var doctaxname = doctaxfileId + "_" + doctaxpath;

                                        /*Saving the file in server folder*/
                                        model.VendorFile[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/ReverseTaxDocument/" + doctaxname));
                                        tblReverseTaxDocument document = new tblReverseTaxDocument();
                                        document.VendorId = vendoId;
                                        if (model.VendorFile[i] != null)
                                        {
                                            document.TaxDocument = model.VendorFile[i].FileName;

                                        }
                                        document.TaxAttachmentPath = doctaxname;
                                        document.TaxAttachmentName = model.VendorAttachName[i];
                                        document.TaxDocumentType = model.VendorDocumentType[i];
                                        document.IsCurrentVersion = false;
                                        document.TaxDocumentUploadUserId = model.UserId;
                                        document.TaxDocumentUpload_Ts = DateTime.Now;
                                        context.tblReverseTaxDocument.Add(document);
                                        context.SaveChanges();
                                        //}
                                        //else
                                        //{
                                        //    query[0].TaxDocumentType = model.GSTDocumentType[i];
                                        //    query[0].TaxAttachmentName = model.GSTAttachName[i];
                                        //    query[0].TaxDocumentUploadUserId = model.UserId;
                                        //    query[0].TaxDocumentUpload_Ts = DateTime.Now;
                                        //    query[0].IsCurrentVersion = false;
                                        //    context.SaveChanges();
                                        //}
                                    }
                                }

                            }

                            if (model.TDSAttachName[0] != "")
                            {
                                for (int i = 0; i < model.TDSDocumentType.Length; i++)
                                {
                                    if (model.TDSDocumentType[i] != 0)
                                    {


                                        //if (query.Count == 0)
                                        //{
                                        string doctdspath = "";
                                        doctdspath = System.IO.Path.GetFileName(model.TDSFile[i].FileName);
                                        var doctdsfileId = Guid.NewGuid().ToString();
                                        var doctdsname = doctdsfileId + "_" + doctdspath;

                                        /*Saving the file in server folder*/
                                        model.TDSFile[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/TDSDocument/" + doctdsname));
                                        tblTDSDocument document = new tblTDSDocument();
                                        document.VendorId = vendoId;
                                        if (model.TDSFile[i] != null)
                                        {
                                            document.IdentityVendorDocument = model.TDSFile[i].FileName;

                                        }
                                        document.IdentityVendorAttachmentPath = doctdsname;
                                        document.IdentityAttachmentName = model.VendorAttachName[i];
                                        document.IdentityVendorDocumentType = model.VendorDocumentType[i];
                                        document.IsCurrentVersion = true;
                                        document.IdentityDocumentUploadUserId = model.UserId;
                                        document.IdentityDocumentUpload_Ts = DateTime.Now;
                                        context.tblTDSDocument.Add(document);
                                        context.SaveChanges();
                                        //}
                                        //else
                                        //{
                                        //    query[0].IdentityVendorDocumentType = model.GSTDocumentType[i];
                                        //    query[0].IdentityAttachmentName = model.GSTAttachName[i];
                                        //    query[0].IdentityDocumentUploadUserId = model.UserId;
                                        //    query[0].IdentityDocumentUpload_Ts = DateTime.Now;
                                        //    query[0].IsCurrentVersion = true;
                                        //    context.SaveChanges();
                                        //}
                                    }
                                }

                            }
                            if (model.Section[0] != 0)
                            {
                                for (int i = 0; i < model.Section.Length; i++)
                                {
                                    tblVendorTDSDetail tdsapplicable = new tblVendorTDSDetail();
                                    tdsapplicable.VendorId = vendoId;
                                    tdsapplicable.Section = model.Section[i];
                                    tdsapplicable.NatureOfIncome = model.NatureOfIncome[i];
                                    tdsapplicable.TDSPercentage = model.TDSPercentage[i];
                                    context.tblVendorTDSDetail.Add(tdsapplicable);
                                    context.SaveChanges();
                                }
                            }
                            transaction.Commit();
                            return 1;

                        }
                        else
                        {
                            var chkvendor = context.tblVendorMaster.FirstOrDefault(M => M.VendorId == model.VendorId);
                            if (chkvendor != null)
                            {
                                chkvendor.AccountHolderName = model.AccountHolderName;
                                chkvendor.BankName = model.BankName;
                                chkvendor.Branch = model.Branch;
                                chkvendor.IFSC = model.IFSCCode;
                                chkvendor.AccountNumber = model.AccountNumber;
                                chkvendor.BankAddress = model.BankAddress;
                                chkvendor.ABANumber = model.ABANumber;
                                chkvendor.SortCode = model.SortCode;
                                chkvendor.IBAN = model.IBAN;
                                chkvendor.SWIFTorBICCode = model.SWIFTorBICCode;
                                chkvendor.BankNature = model.BankNature;
                                chkvendor.BankEmailId = model.BankEmailId;
                                chkvendor.MICRCode = model.MICRCode;
                                chkvendor.Category = model.ServiceCategory;
                                chkvendor.ServiceType = model.ServiceType;
                                chkvendor.SupplyType = model.SupplierType;
                                chkvendor.CertificateNumber = model.CertificateNumber;
                                chkvendor.ValidityPeriod = model.ValidityPeriod;
                                chkvendor.UPDT_UserID = model.UserId;
                                chkvendor.UPDT_TS = DateTime.Now;
                                chkvendor.ReverseTax = model.ReverseTax;
                                chkvendor.TDSExcempted = model.TDSExcempted;
                                chkvendor.ReasonForReservieTax = model.ReverseTaxReason;

                                context.SaveChanges();
                                if (model.GSTAttachName[0] != "")
                                {
                                    var deldocument = (from RD in context.tblGstDocument
                                                       where RD.VendorId == model.VendorId && !model.GSTDocumentId.Contains(RD.GstVendorDocumentId)
                                                       && RD.IsCurrentVersion != true
                                                       select RD).ToList();
                                    int delCount = deldocument.Count();
                                    if (delCount > 0)
                                    {
                                        for (int i = 0; i < delCount; i++)
                                        {
                                            deldocument[i].IsCurrentVersion = true;
                                            deldocument[i].GstDocumentUploadUserId = model.UserId;
                                            deldocument[i].GstDocumentUpload_Ts = DateTime.Now;
                                            context.SaveChanges();
                                        }
                                    }
                                    for (int i = 0; i < model.GSTDocumentType.Length; i++)
                                    {
                                        if (model.GSTDocumentType[i] != 0)
                                        {
                                            var docid = model.GSTDocumentId[i];
                                            var query = (from G in context.tblGstDocument
                                                         where (G.GstVendorDocumentId == docid && G.VendorId == model.VendorId && G.IsCurrentVersion != true)
                                                         select G).ToList();
                                            if (query.Count == 0)
                                            {
                                                string docgstpath = "";
                                                docgstpath = System.IO.Path.GetFileName(model.GSTFile[i].FileName);
                                                var docgstfileId = Guid.NewGuid().ToString();
                                                var docgstname = docgstfileId + "_" + docgstpath;

                                                /*Saving the file in server folder*/
                                                model.GSTFile[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/GstDocument/" + docgstname));
                                                tblGstDocument document = new tblGstDocument();
                                                document.VendorId = model.VendorId;
                                                if (model.GSTFile[i] != null)
                                                {
                                                    document.GstVendorDocument = model.GSTFile[i].FileName;

                                                }
                                                document.GstAttachmentPath = docgstname;
                                                document.GstAttachmentName = model.GSTAttachName[i];
                                                document.GstDocumentType = model.GSTDocumentType[i];
                                                document.GstDocumentUploadUserId = model.UserId;
                                                document.GstDocumentUpload_Ts = DateTime.Now;
                                                context.tblGstDocument.Add(document);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].GstDocumentType = model.GSTDocumentType[i];
                                                query[0].GstAttachmentName = model.GSTAttachName[i];
                                                query[0].GstDocumentUploadUserId = model.UserId;
                                                query[0].GstDocumentUpload_Ts = DateTime.Now;
                                                query[0].IsCurrentVersion = false;
                                                context.SaveChanges();
                                            }
                                        }
                                    }



                                }

                                if (model.VendorAttachName[0] != "")
                                {
                                    var deldocument = (from RD in context.tblReverseTaxDocument
                                                       where RD.VendorId == model.VendorId && !model.VendorDocumentId.Contains(RD.RevereseTaxDocumentId)
                                                       && RD.IsCurrentVersion != true
                                                       select RD).ToList();
                                    int delCount = deldocument.Count();
                                    if (delCount > 0)
                                    {
                                        for (int i = 0; i < delCount; i++)
                                        {
                                            deldocument[i].IsCurrentVersion = true;
                                            deldocument[i].TaxDocumentUploadUserId = model.UserId;
                                            deldocument[i].TaxDocumentUpload_Ts = DateTime.Now;
                                            context.SaveChanges();
                                        }
                                    }
                                    for (int i = 0; i < model.VendorDocumentType.Length; i++)
                                    {
                                        if (model.VendorDocumentType[i] != 0)
                                        {
                                            var docid = model.VendorDocumentId[i];
                                            var query = (from T in context.tblReverseTaxDocument
                                                         where (T.RevereseTaxDocumentId == docid && T.VendorId == model.VendorId && T.IsCurrentVersion != true)
                                                         select T).ToList();
                                            if (query.Count == 0)
                                            {
                                                string doctaxpath = "";
                                                doctaxpath = System.IO.Path.GetFileName(model.VendorFile[i].FileName);
                                                var doctaxfileId = Guid.NewGuid().ToString();
                                                var doctaxname = doctaxfileId + "_" + doctaxpath;

                                                /*Saving the file in server folder*/
                                                model.VendorFile[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/ReverseTaxDocument/" + doctaxname));
                                                tblReverseTaxDocument document = new tblReverseTaxDocument();
                                                document.VendorId = model.VendorId;
                                                if (model.VendorFile[i] != null)
                                                {
                                                    document.TaxDocument = model.VendorFile[i].FileName;

                                                }
                                                document.TaxAttachmentPath = doctaxname;
                                                document.TaxAttachmentName = model.VendorAttachName[i];
                                                document.TaxDocumentType = model.VendorDocumentType[i];
                                                document.IsCurrentVersion = false;
                                                document.TaxDocumentUploadUserId = model.UserId;
                                                document.TaxDocumentUpload_Ts = DateTime.Now;
                                                context.tblReverseTaxDocument.Add(document);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].TaxDocumentType = model.VendorDocumentType[i];
                                                query[0].TaxAttachmentName = model.VendorAttachName[i];
                                                query[0].TaxDocumentUploadUserId = model.UserId;
                                                query[0].TaxDocumentUpload_Ts = DateTime.Now;
                                                query[0].IsCurrentVersion = false;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }
                                if (model.TDSAttachName[0] != "")
                                {
                                    var deldocument = (from RD in context.tblTDSDocument
                                                       where RD.VendorId == model.VendorId && !model.TDSDocumentId.Contains(RD.VendorIdentityDocumentId)
                                                       && RD.IsCurrentVersion != true
                                                       select RD).ToList();
                                    int delCount = deldocument.Count();
                                    if (delCount > 0)
                                    {
                                        for (int i = 0; i < delCount; i++)
                                        {
                                            deldocument[i].IsCurrentVersion = true;
                                            deldocument[i].IdentityDocumentUploadUserId = model.UserId;
                                            deldocument[i].IdentityDocumentUpload_Ts = DateTime.Now;
                                            context.SaveChanges();
                                        }
                                    }
                                    for (int i = 0; i < model.TDSDocumentType.Length; i++)
                                    {
                                        if (model.TDSDocumentType[i] != 0)
                                        {
                                            var docid = model.TDSDocumentId[i];
                                            var query = (from T in context.tblTDSDocument
                                                         where (T.VendorIdentityDocumentId == docid && T.VendorId == model.VendorId && T.IsCurrentVersion != true)
                                                         select T).ToList();
                                            if (query.Count == 0)
                                            {
                                                string doctdspath = "";
                                                doctdspath = System.IO.Path.GetFileName(model.TDSFile[i].FileName);
                                                var doctdsfileId = Guid.NewGuid().ToString();
                                                var doctdsname = doctdsfileId + "_" + doctdspath;

                                                /*Saving the file in server folder*/
                                                model.TDSFile[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/TDSDocument/" + doctdsname));
                                                tblTDSDocument document = new tblTDSDocument();
                                                document.VendorId = model.VendorId;
                                                if (model.TDSFile[i] != null)
                                                {
                                                    document.IdentityVendorDocument = model.TDSFile[i].FileName;

                                                }
                                                document.IdentityVendorAttachmentPath = doctdsname;
                                                document.IdentityAttachmentName = model.TDSAttachName[i];
                                                document.IdentityVendorDocumentType = model.TDSDocumentType[i];
                                                document.IsCurrentVersion = false;
                                                document.IdentityDocumentUploadUserId = model.UserId;
                                                document.IdentityDocumentUpload_Ts = DateTime.Now;
                                                context.tblTDSDocument.Add(document);
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                query[0].IdentityVendorDocumentType = model.TDSDocumentType[i];
                                                query[0].IdentityAttachmentName = model.TDSAttachName[i];
                                                query[0].IdentityDocumentUploadUserId = model.UserId;
                                                query[0].IdentityDocumentUpload_Ts = DateTime.Now;
                                                query[0].IsCurrentVersion = false;
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                }
                                if (model.Section[0] != 0)
                                {
                                    var delvendortds = (from VTD in context.tblVendorTDSDetail
                                                        where VTD.VendorId == model.VendorId && !model.VendorTDSDetailId.Contains(VTD.VendorTDSDetailId)
                                                        select VTD).ToList();
                                    int delCount = delvendortds.Count();
                                    if (delCount > 0)
                                    {
                                        context.tblVendorTDSDetail.RemoveRange(delvendortds);
                                        context.SaveChanges();
                                    }
                                    for (int i = 0; i < model.Section.Length; i++)
                                    {

                                        var tdsid = model.VendorTDSDetailId[i];
                                        var query = (from T in context.tblVendorTDSDetail
                                                     where (T.VendorTDSDetailId == tdsid && T.VendorId == model.VendorId)
                                                     select T).ToList();
                                        if (query.Count == 0)
                                        {

                                            tblVendorTDSDetail tdsapplicable = new tblVendorTDSDetail();
                                            tdsapplicable.VendorId = model.VendorId;
                                            tdsapplicable.Section = model.Section[i];
                                            tdsapplicable.NatureOfIncome = model.NatureOfIncome[i];
                                            tdsapplicable.TDSPercentage = model.TDSPercentage[i];
                                            context.tblVendorTDSDetail.Add(tdsapplicable);
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            query[0].VendorId = model.VendorId;
                                            query[0].Section = model.Section[i];
                                            query[0].NatureOfIncome = model.NatureOfIncome[i];
                                            query[0].TDSPercentage = model.TDSPercentage[i];
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                            return 3;
                        }


                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return -1;
                    }


                }
            }

        }
       
        public static VendorMasterViewModel EditVendor(int vendorId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    VendorMasterViewModel editVendor = new VendorMasterViewModel();
                    var query = (from V in context.tblVendorMaster
                                 where (V.VendorId == vendorId && V.Status == "Active")
                                 select V).FirstOrDefault();
                    var Gstfile = (from G in context.tblGstDocument
                                   where G.VendorId == vendorId && G.IsCurrentVersion != true
                                   select G).ToList();
                    var vendorfile = (from vf in context.tblReverseTaxDocument
                                      where vf.VendorId == vendorId && vf.IsCurrentVersion != true
                                      select vf).ToList();
                    var tdsFile = (from TF in context.tblTDSDocument
                                   where TF.VendorId == vendorId && TF.IsCurrentVersion != true
                                   select TF).ToList();
                    var tdsdetail = (from TD in context.tblVendorTDSDetail
                                     where TD.VendorId == vendorId
                                     select TD).ToList();

                    if (query != null)
                    {
                        editVendor.VendorId = query.VendorId;
                        editVendor.Nationality = Convert.ToInt32(query.Nationality);
                        editVendor.PFMSVendorCode = query.PFMSVendorCode;
                        editVendor.Name = query.Name;
                        editVendor.Address = query.Address;
                        editVendor.Email = query.Email;
                        editVendor.ContactPerson = query.ContactPerson;
                        editVendor.PhoneNumber = query.PhoneNumber;
                        editVendor.MobileNumber = query.MobileNumber;
                        editVendor.City = query.City;
                        editVendor.PinCode = query.Pincode;
                        editVendor.CountryId = Convert.ToInt32(query.Country);
                        editVendor.StateId = Convert.ToInt32(query.StateId);
                        editVendor.StateCode = Convert.ToInt32(query.StateCode);
                        editVendor.RegisteredName = query.RegisteredName;
                        editVendor.PAN = query.PAN;
                        editVendor.TAN = query.TAN;
                        editVendor.GSTExempted = Convert.ToBoolean(query.GSTExempted);
                        editVendor.Reason = query.Reason;
                        editVendor.GSTIN = query.GSTIN;
                        editVendor.AccountHolderName = query.AccountHolderName;
                        editVendor.BankName = query.BankName;
                        editVendor.Branch = query.Branch;
                        editVendor.IFSCCode = query.IFSC;
                        editVendor.AccountNumber = query.AccountNumber;
                        editVendor.BankAddress = query.BankAddress;
                        editVendor.ABANumber = query.ABANumber;
                        editVendor.SortCode = query.SortCode;
                        editVendor.IBAN = query.IBAN;
                        editVendor.SWIFTorBICCode = query.SWIFTorBICCode;
                        editVendor.BankNature = query.BankNature;
                        editVendor.BankEmailId = query.BankEmailId;
                        editVendor.MICRCode = query.MICRCode;
                        editVendor.ServiceCategory = Convert.ToInt32(query.Category);
                        editVendor.ServiceType = Convert.ToInt32(query.ServiceType);
                        editVendor.SupplierType = Convert.ToInt32(query.SupplyType);
                        editVendor.ReverseTax = Convert.ToBoolean(query.ReverseTax);
                        editVendor.TDSExcempted = Convert.ToBoolean(query.TDSExcempted);
                        editVendor.ReverseTaxReason = query.ReasonForReservieTax;
                        editVendor.CertificateNumber = query.CertificateNumber;
                        editVendor.ValidityPeriod = Convert.ToInt32(query.ValidityPeriod);
                        if (Gstfile.Count > 0)
                        {
                            int[] _docid = new int[Gstfile.Count];
                            int[] _doctype = new int[Gstfile.Count];
                            string[] _docname = new string[Gstfile.Count];
                            string[] _attchname = new string[Gstfile.Count];
                            string[] _docpath = new string[Gstfile.Count];
                            for (int i = 0; i < Gstfile.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(Gstfile[i].GstVendorDocumentId);
                                _doctype[i] = Convert.ToInt32(Gstfile[i].GstDocumentType);
                                _docname[i] = Gstfile[i].GstVendorDocument;
                                _attchname[i] = Gstfile[i].GstAttachmentName;
                                _docpath[i] = Gstfile[i].GstAttachmentPath;
                            }
                            editVendor.GSTDocumentId = _docid;
                            editVendor.GSTDocumentType = _doctype;
                            editVendor.GSTDocumentName = _docname;
                            editVendor.GSTAttachName = _attchname;
                            editVendor.GSTDocPath = _docpath;
                        }
                        if (vendorfile.Count > 0)
                        {
                            int[] _docid = new int[vendorfile.Count];
                            int[] _doctype = new int[vendorfile.Count];
                            string[] _docname = new string[vendorfile.Count];
                            string[] _attchname = new string[vendorfile.Count];
                            string[] _docpath = new string[vendorfile.Count];
                            for (int i = 0; i < vendorfile.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(vendorfile[i].RevereseTaxDocumentId);
                                _doctype[i] = Convert.ToInt32(vendorfile[i].TaxDocumentType);
                                _docname[i] = vendorfile[i].TaxDocument;
                                _attchname[i] = vendorfile[i].TaxAttachmentName;
                                _docpath[i] = vendorfile[i].TaxAttachmentPath;
                            }
                            editVendor.VendorDocumentId = _docid;
                            editVendor.VendorDocumentType = _doctype;
                            editVendor.VendorDocumentName = _docname;
                            editVendor.VendorAttachName = _attchname;
                            editVendor.VendorDocPath = _docpath;
                        }
                        if (tdsFile.Count > 0)
                        {
                            int[] _docid = new int[tdsFile.Count];
                            int[] _doctype = new int[tdsFile.Count];
                            string[] _docname = new string[tdsFile.Count];
                            string[] _attchname = new string[tdsFile.Count];
                            string[] _docpath = new string[tdsFile.Count];
                            for (int i = 0; i < tdsFile.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(tdsFile[i].VendorIdentityDocumentId);
                                _doctype[i] = Convert.ToInt32(tdsFile[i].IdentityVendorDocumentType);
                                _docname[i] = tdsFile[i].IdentityVendorDocument;
                                _attchname[i] = tdsFile[i].IdentityAttachmentName;
                                _docpath[i] = tdsFile[i].IdentityVendorAttachmentPath;
                            }
                            editVendor.TDSDocumentId = _docid;
                            editVendor.TDSDocumentType = _doctype;
                            editVendor.TDSDocumentName = _docname;
                            editVendor.TDSAttachName = _attchname;
                            editVendor.TDSDocPath = _docpath;
                        }
                        if (tdsdetail.Count > 0)
                        {
                            int[] _tdsdetilid = new int[tdsdetail.Count];
                            int[] _tdssection = new int[tdsdetail.Count];
                            string[] _income = new string[tdsdetail.Count];
                            decimal[] _percentage = new decimal[tdsdetail.Count];
                            for (int i = 0; i < tdsdetail.Count; i++)
                            {
                                _tdsdetilid[i] = Convert.ToInt32(tdsdetail[i].VendorTDSDetailId);
                                _tdssection[i] = Convert.ToInt32(tdsdetail[i].Section);
                                _income[i] = tdsdetail[i].NatureOfIncome;
                                _percentage[i] = Convert.ToDecimal(tdsdetail[i].TDSPercentage);
                            }
                            editVendor.VendorTDSDetailId = _tdsdetilid;
                            editVendor.Section = _tdssection;
                            editVendor.NatureOfIncome = _income;
                            editVendor.TDSPercentage = _percentage;
                        }
                    }
                    return editVendor;
                }
            }
            catch (Exception ex)
            {
                VendorMasterViewModel editVendor = new VendorMasterViewModel();
                return editVendor;
            }
        }
        public static int InternalAgency(InternalAgencyViewModel model)
        {
            
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                    try
                    {

                        int agencyid = 0;
                    if (model.InternalAgencyId == null)
                    {
                   
                        tblAgencyMaster reginternalagncy = new tblAgencyMaster();
                        var chkagency = context.tblAgencyMaster.FirstOrDefault(M => M.AgencyName == model.InternalAgencyName && M.Status == "Active" && M.AgencyType == 1);
                        if (chkagency != null)
                            return 2;
                        var chkcode = context.tblAgencyMaster.FirstOrDefault(C => C.AgencyCode == model.InternalAgencyCode && C.Status == "Active" && C.AgencyType == 1);
                        if (chkcode != null)
                            return 4;
                        var Sqnbr = (from IA in context.tblAgencyMaster
                                     where IA.AgencyType == 1 select IA.SeqNbr).Max();
                        reginternalagncy.AgencyName = model.InternalAgencyName;
                        reginternalagncy.AgencyCode = model.InternalAgencyCode;
                        reginternalagncy.ContactPerson = model.InternalAgencyContactPerson;
                        reginternalagncy.ContactNumber = model.InternalAgencyContactNumber;
                        reginternalagncy.ContactEmail = model.InternalConatactEmail;
                        reginternalagncy.Address = model.InternalAgencyAddress;
                        reginternalagncy.AgencyRegisterName = model.InternalAgencyRegisterName;
                        reginternalagncy.AgencyRegisterAddress = model.InternalAgencyRegisterAddress;
                        reginternalagncy.District = model.InternalDistrict;
                        reginternalagncy.PinCode = model.InternalPincode;
                        reginternalagncy.State = model.InternalAgencyState;
                        reginternalagncy.Crtd_TS = DateTime.Now;
                        reginternalagncy.Crtd_UserId = model.InternalAgencyUserId;
                        reginternalagncy.AgencyType = 1;
                        reginternalagncy.Status = "Active";
                        reginternalagncy.SeqNbr = (Convert.ToInt32(Sqnbr) + 1);
                            reginternalagncy.ProjectTypeId = model.ProjectType;
                        context.tblAgencyMaster.Add(reginternalagncy);
                        context.SaveChanges();
                        agencyid = reginternalagncy.AgencyId;
                        if (model.AttachName[0] != null && model.AttachName[0] != "")
                        {
                            for (int i = 0; i < model.DocumentType.Length; i++)
                            {
                                string docpath = "";
                                docpath = System.IO.Path.GetFileName(model.File[i].FileName);
                                var docfileId = Guid.NewGuid().ToString();
                                var docname = docfileId + "_" + docpath;

                                /*Saving the file in server folder*/
                                model.File[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/AgencyDocument/" + docname));
                                tblAgencyDocument document = new tblAgencyDocument();
                                document.AgencyId = agencyid;
                                if (model.File[i] != null)
                                {
                                    document.AgencyDocument = model.File[i].FileName;

                                }
                                document.AttachmentPath = docname;
                                document.AttachmentName = model.AttachName[i];
                                document.DocumentType = model.DocumentType[i];
                                document.IsCurrentVersion = true;
                                document.DocumentUploadUserId = model.UserId;
                                document.DocumentUpload_Ts = DateTime.Now;
                                context.tblAgencyDocument.Add(document);
                                context.SaveChanges();
                            }
                                
                        }
                            transaction.Commit();
                            return 1;
                    }
                    else
                    {
                        var reginternalupdate = context.tblAgencyMaster.Where(UI => UI.AgencyId == model.InternalAgencyId).FirstOrDefault();
                        if (reginternalupdate != null)
                        {
                            reginternalupdate.AgencyName = model.InternalAgencyName;
                            reginternalupdate.AgencyCode = model.InternalAgencyCode;
                            reginternalupdate.ContactPerson = model.InternalAgencyContactPerson;
                            reginternalupdate.ContactNumber = model.InternalAgencyContactNumber;
                            reginternalupdate.ContactEmail = model.InternalConatactEmail;
                            reginternalupdate.Address = model.InternalAgencyAddress;
                            reginternalupdate.AgencyRegisterName = model.InternalAgencyRegisterName;
                            reginternalupdate.AgencyRegisterAddress = model.InternalAgencyRegisterAddress;
                            reginternalupdate.District = model.InternalDistrict;
                            reginternalupdate.PinCode = model.InternalPincode;
                            reginternalupdate.State = model.InternalAgencyState;
                            reginternalupdate.Lastupdate_TS = DateTime.Now;
                            reginternalupdate.LastupdatedUserid = model.InternalAgencyUserId;
                                reginternalupdate.ProjectTypeId = model.ProjectType;
                            reginternalupdate.AgencyType = 1;
                            reginternalupdate.Status = "Active";
                            context.SaveChanges();
                            if (model.AttachName[0] != null && model.AttachName[0] != "")
                            {
                                var deldocument = (from RD in context.tblAgencyDocument
                                                   where RD.AgencyId == model.InternalAgencyId &&
                                                   !model.DocumentId.Contains(RD.AgencyDocumentId) && RD.IsCurrentVersion == true
                                                   select RD).ToList();
                                int delCount = deldocument.Count();
                                if (delCount > 0)
                                {
                                    for (int i = 0; i < delCount; i++)
                                    {
                                        deldocument[i].IsCurrentVersion = false;
                                        context.SaveChanges();
                                    }
                                }
                                for (int i = 0; i < model.DocumentType.Length; i++)
                                {
                                    if (model.DocumentType[i] != 0)
                                    {
                                        var docid = model.DocumentId[i];
                                        var query = (from D in context.tblAgencyDocument
                                                     where (D.AgencyDocumentId == docid && D.AgencyId == model.InternalAgencyId && D.IsCurrentVersion == true)
                                                     select D).ToList();
                                        if (query.Count == 0)
                                        {
                                            string docpath = "";
                                            docpath = System.IO.Path.GetFileName(model.File[i].FileName);
                                            var docfileId = Guid.NewGuid().ToString();
                                            var docname = docfileId + "_" + docpath;

                                            /*Saving the file in server folder*/
                                            model.File[i].SaveAs(HttpContext.Current.Server.MapPath("~/Content/AgencyDocument/" + docname));
                                            tblAgencyDocument document = new tblAgencyDocument();
                                            document.AgencyId = model.InternalAgencyId;
                                            if (model.File[i] != null)
                                            {
                                                document.AgencyDocument = model.File[i].FileName;

                                            }
                                            document.AttachmentPath = docname;
                                            document.AttachmentName = model.AttachName[i];
                                            document.DocumentType = model.DocumentType[i];
                                            document.IsCurrentVersion = true;
                                            document.DocumentUploadUserId = model.UserId;
                                            document.DocumentUpload_Ts = DateTime.Now;
                                            context.tblAgencyDocument.Add(document);
                                            context.SaveChanges();
                                        }
                                        else
                                        {
                                            query[0].DocumentType = model.DocumentType[i];
                                            query[0].AttachmentName = model.AttachName[i];
                                            query[0].DocumentUploadUserId = model.UserId;
                                            query[0].DocumentUpload_Ts = DateTime.Now;
                                            query[0].IsCurrentVersion = true;
                                            context.SaveChanges();
                                        }
                                    }
                                }

                            }
                                transaction.Commit();
                            }
                            
                        return 3;
                    }
                }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var Msg = ex;
                        return -1;
                    }
                }
            }
            
        }
        public static List<InternalAgencyViewModel>GetInternalAgency(InternalAgencyViewModel model)
        {
            try
            {
                List<InternalAgencyViewModel> Internallist = new List<InternalAgencyViewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from IA in context.tblAgencyMaster
                                 from C in context.tblCodeControl
                                 where IA.Status == "Active" && IA.AgencyType == 1 && IA.AgencyType == C.CodeValAbbr && C.CodeName == "SponsoredProjectSubtype"
                                 &&(IA.AgencyName.Contains(model.SearchAgencyName)||model.SearchAgencyName==null)
                                 &&(IA.AgencyCode.Contains(model.SearchAgencyCode)||model.SearchAgencyCode == null)

                                 select new { IA.AgencyId, IA.AgencyName, IA.ContactPerson, IA.ContactEmail, C.CodeValDetail }).ToList();
                    if(query.Count>0)
                    {
                        for(int i=0;i<query.Count;i++)
                        {
                            Internallist.Add(new InternalAgencyViewModel() {
                                sno = i + 1,
                                InternalAgencyId=query[i].AgencyId,
                                InternalAgencyName=query[i].AgencyName,
                                InternalAgencyContactPerson=query[i].ContactPerson,
                                InternalConatactEmail=query[i].ContactEmail,
                                InternalAgencyType=query[i].CodeValDetail
                            });
                        }
                    }
                    return Internallist;
                }
            }
            catch(Exception ex)
            {
                List<InternalAgencyViewModel> Internallist = new List<InternalAgencyViewModel>();
                return Internallist;
            }
        }
        public static InternalAgencyViewModel EditInternalAgency(int agencyId)
        {
            try
            {
                InternalAgencyViewModel agency = new InternalAgencyViewModel();
                using (var context = new IOASDBEntities())
                {
                    var filenamelist = (from F in context.tblAgencyDocument
                                        where F.AgencyId == agencyId && F.IsCurrentVersion == true
                                        select new { F.AgencyDocumentId, F.AttachmentPath, F.AgencyDocument, F.AttachmentName, F.DocumentType }).ToList();
                    var query = (from IA in context.tblAgencyMaster
                                 where (IA.AgencyId == agencyId)
                                 select new
                                 {
                                     IA.AgencyId,
                                     IA.AgencyName,
                                     IA.AgencyCode,
                                     IA.ContactPerson,
                                     IA.ContactNumber,
                                     IA.ContactEmail,
                                     IA.Address,
                                     IA.AgencyRegisterName,
                                     IA.AgencyRegisterAddress,
                                     IA.District,
                                     IA.PinCode,
                                     IA.State,
                                     IA.ProjectTypeId
                                    
                                 }).FirstOrDefault();
                    if(query!=null)
                    {
                        agency.InternalAgencyId = query.AgencyId;
                        agency.InternalAgencyName = query.AgencyName;
                        agency.InternalAgencyCode = query.AgencyCode;
                        agency.InternalAgencyContactPerson = query.ContactPerson;
                        agency.InternalAgencyContactNumber = query.ContactNumber;
                        agency.InternalConatactEmail = query.ContactEmail;
                        agency.InternalAgencyAddress = query.Address;
                        agency.InternalAgencyRegisterName = query.AgencyRegisterName;
                        agency.InternalAgencyRegisterAddress = query.AgencyRegisterAddress;
                        agency.InternalDistrict = query.District;
                        agency.InternalPincode = query.PinCode;
                        agency.InternalAgencyState = query.State;
                        agency.ProjectType = Convert.ToInt32(query.ProjectTypeId);
                        if (filenamelist.Count > 0)
                        {
                            int[] _docid = new int[filenamelist.Count];
                            int[] _doctype = new int[filenamelist.Count];
                            string[] _docname = new string[filenamelist.Count];
                            string[] _attchname = new string[filenamelist.Count];
                            string[] _docpath = new string[filenamelist.Count];
                            for (int i = 0; i < filenamelist.Count; i++)
                            {
                                _docid[i] = Convert.ToInt32(filenamelist[i].AgencyDocumentId);
                                _doctype[i] = Convert.ToInt32(filenamelist[i].DocumentType);
                                _docname[i] = filenamelist[i].AgencyDocument;
                                _attchname[i] = filenamelist[i].AttachmentName;
                                _docpath[i] = filenamelist[i].AttachmentPath;
                            }
                            agency.DocumentId = _docid;
                            agency.DocumentType = _doctype;
                            agency.DocumentName = _docname;
                            agency.AttachName = _attchname;
                            agency.DocPath = _docpath;
                        }
                    }
                }
                return agency;
            }
            catch(Exception ex)
            {
                InternalAgencyViewModel agency = new InternalAgencyViewModel();
                return agency;
            }
        }
        public static int DeleteInternalAgency(int agencyId, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var internalAgency = context.tblAgencyMaster.Where(D => D.AgencyId == agencyId).FirstOrDefault();
                    if(internalAgency!=null)
                    {
                        internalAgency.Status = "InActive";
                        internalAgency.Lastupdate_TS = DateTime.Now;
                        internalAgency.LastupdatedUserid = Common.GetUserid(Username);
                        context.SaveChanges();
                    }
                    return 1;
                }
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        public static string InternalAgencyCode()
        {
            try
            {
                string internalagycode = "";
                using (var context = new IOASDBEntities())
                {
                    var maxsqn = (from IA in context.tblAgencyMaster
                                  where IA.AgencyType == 1
                                  select IA.SeqNbr).Max();
                    internalagycode = "I" + (Convert.ToInt32(maxsqn)+ 1).ToString("00");
               }
                return internalagycode;
            }
            catch(Exception ex)
            {
                return "";
            }
        }
        
        public static List<MasterlistviewModel> GetVendorCode()
        {
            try
            {
                List<MasterlistviewModel> vendorcode = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from V in context.tblVendorMaster
                                 where V.Status == "Active"
                                 select new { V.VendorId, V.VendorCode }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            vendorcode.Add(new MasterlistviewModel()
                            {
                                id = query[i].VendorId,
                                name = query[i].VendorCode
                            });
                        }
                    }
                }
                return vendorcode;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> vendorcode = new List<MasterlistviewModel>();
                return vendorcode;
            }
        }
       
        public static TdsSectionModel GetSectiontds(int sectionId)
        {
            try
            {
               TdsSectionModel tds = new TdsSectionModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from T in context.tblTDSMaster
                                 where (T.TdsMasterId == sectionId)
                                 select new { T.NatureOfIncome, T.Percentage }).FirstOrDefault();
                    if(query!=null)
                    {
                        tds.NatureOfIncome = query.NatureOfIncome;
                        tds.Percentage = Convert.ToDecimal(query.Percentage);
                    }
                }
                return tds;
            }
            catch(Exception ex)
            {
                TdsSectionModel tds = new TdsSectionModel();
                return tds;
            }
        }

        public static List<LedgerOBBalanceModel> GetAccountWiseHead(int accounttypid)
        {
            try
            {
                List<LedgerOBBalanceModel> headlist = new List<LedgerOBBalanceModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from vw in context.vwAccountGroupinHeadwise
                                 where (vw.AccountCategoryId == accounttypid && vw.AccountheadStatus == "Active")
                                 select new { vw.Groups, vw.AccountHead, vw.OpeningBal, vw.HeadOpeningBalanceId, vw.FinacialYearId, vw.AccountHeadId }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            headlist.Add(new LedgerOBBalanceModel()
                            {
                                sno = i + 1,
                                AccountGroupName = query[i].Groups,
                                AccountHeadName = query[i].AccountHead,
                                CurrentOpeningBalance = Convert.ToDecimal(query[i].OpeningBal),
                                HeadOpeningBalanceId = Convert.ToInt32(query[i].HeadOpeningBalanceId),
                                FinalYearId = Convert.ToInt32(query[i].FinacialYearId),
                                AccountHeadId = Convert.ToInt32(query[i].AccountHeadId)
                            });
                        }
                    }
                }
                return headlist;
            }
            catch (Exception ex)
            {
                List<LedgerOBBalanceModel> headlist = new List<LedgerOBBalanceModel>();
                return headlist;
            }
        }
        public static LedgerOBBalanceModel GetOpeningBalance(int accheadid)
        {
            try
            {
                LedgerOBBalanceModel model = new LedgerOBBalanceModel();
                using (var context = new IOASDBEntities())
                {
                    var query = (from Q in context.vwAccountGroupinHeadwise
                                 where (Q.AccountHeadId == accheadid)
                                 select new { Q.OpeningBal, Q.FinacialYearId, Q.AccountHeadId, Q.HeadOpeningBalanceId }).FirstOrDefault();
                    if (query != null)
                    {
                        model.PopupCurrentOpeningBalance = Convert.ToDecimal(query.OpeningBal);
                        model.FinalYearId = Convert.ToInt32(query.FinacialYearId);
                        model.AccountHeadId = query.AccountHeadId;
                        model.HeadOpeningBalanceId = Convert.ToInt32(query.HeadOpeningBalanceId);
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                LedgerOBBalanceModel model = new LedgerOBBalanceModel();
                return model;
            }
        }
        public static int AddOpeningBalanceLedger(LedgerOBBalanceModel model, string Username)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            String Encpassword = Cryptography.Encrypt(model.Password, "LFPassW0rd");
                            var chckuser = context.tblUser.Where(ch => ch.UserName == Username && ch.Password == Encpassword).FirstOrDefault();
                            if (chckuser != null)
                            {
                                if (model.Userid == 1)
                                {
                                    LedgerOBBalanceModel addOBLeg = new LedgerOBBalanceModel();
                                    if (model.HeadOpeningBalanceId == 0)
                                    {
                                        tblHeadOpeningBalance addHB = new tblHeadOpeningBalance();
                                        addHB.FinYearId = model.FinalYearId;
                                        addHB.AccountHeadId = model.AccountHeadId;
                                        addHB.OpeningBalance = model.PopModeifiedOpeningBalance;
                                        addHB.CRTD_TS = DateTime.Now;
                                        addHB.CRTD_By = model.Userid;
                                        addHB.Status = "Active";
                                        context.tblHeadOpeningBalance.Add(addHB);
                                        context.SaveChanges();
                                        transaction.Commit();
                                        return 1;
                                    }
                                    else
                                    {
                                        var chkhOB = context.tblHeadOpeningBalance.Where(H => H.HeadOpeningBalanceId == model.HeadOpeningBalanceId).FirstOrDefault();
                                        if (chkhOB != null)
                                        {
                                            chkhOB.FinYearId = model.FinalYearId;
                                            chkhOB.AccountHeadId = model.AccountHeadId;
                                            chkhOB.OpeningBalance = model.PopModeifiedOpeningBalance;
                                            chkhOB.UPTD_TS = DateTime.Now;
                                            chkhOB.UPTD_By = model.Userid;
                                            context.SaveChanges();
                                            tblUpdateLedgerOBLog addlog = new tblUpdateLedgerOBLog();
                                            addlog.HeadId = model.HeadOpeningBalanceId;
                                            addlog.OBUpdated_Ts = DateTime.Now;
                                            addlog.OBUpdatedUserId = model.Userid;
                                            addlog.OBLedgerOldValue = model.PopupCurrentOpeningBalance;
                                            addlog.OBLedgerCurrentValue = model.PopModeifiedOpeningBalance;
                                            context.tblUpdateLedgerOBLog.Add(addlog);
                                            context.SaveChanges();

                                        }
                                        transaction.Commit();
                                        return 2;
                                    }
                                }
                                else
                                {
                                    return 4;
                                }
                            }
                            else
                            {
                                return 3;
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return -1;

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return -1;
            }
        }
    }
}