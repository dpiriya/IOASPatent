using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IOAS.Models;
using IOAS.GenericServices;
using System.Collections.Generic;
using System.Web.Security;
using IOAS.Infrastructure;
using System.IO;
using IOAS.Filter;

namespace IOAS.Controllers
{
    [Authorized]
    public class ProjectController : Controller
    {
        // Creation of Project (Project Opening)

        public ActionResult ProjectOpening(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                CreateProjectModel model = new CreateProjectModel();
                var country = Common.getCountryList();
                var department = Common.getDepartment();
                var gender = Common.getGender();
                var PI = Common.GetProjectPIWithDetails();
                var Doctype = Common.GetDocTypeList(9);
                var scheme = Common.getschemes();
                var agency = Common.getagency();
                var projecttype = Common.getprojecttype();
                var proposalnumber = Common.getproposalnumber();
                var ministry = Common.getMinistry();
                var projectsponsubtype = Common.getsponprojectsubtype();
                var projectconssubtype = Common.getconsprojectsubtype();
                var categoryofproject = Common.getcategoryofproject();
                var Cadre = Common.getFacultyCadre();
                var fundingcategory = Common.getconsfundingcategory();
                var allocatehead = Common.getallocationhead();
                var taxservice = Common.gettaxservice();
                var internalfundingagency = Common.getinternalfundingagency();
                var staffcategory = Common.getstaffcategory();
                var fundingtype = Common.getfundingtype();
                var fundedby = Common.getfundedby();
                var indfundinggovtbody = Common.getindfundinggovtbody();
                var indfundingnongovtbody = Common.getindfundingnongovtbody();
                var forgnfundinggovtbody = Common.getforgnfundinggovtbody();
                var forgnfundingnongovtbody = Common.getforgnfundingnongovtbody();
                var typeofproject = Common.gettypeofproject();
                var sponprojectcategory = Common.getsponprojectcategory();
                var constaxtype = Common.getconstaxtype();
                var taxregstatus = Common.gettaxregstatus();
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.projectcategory = Common.getprojectcategory();
                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                ViewBag.ProjectList = new List<MasterlistviewModel>();
                ViewBag.consScheme = AccountService.getcategory(2);
                ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.Scheme = AccountService.getcategory(1);
                ViewBag.Agency = agency;
                ViewBag.projecttype = projecttype;
                ViewBag.proposalnumber = proposalnumber;
                ViewBag.ministry = ministry;
                ViewBag.projectsponsubtype = projectsponsubtype;
                ViewBag.projectconssubtype = projectconssubtype;
                ViewBag.categoryofproject = categoryofproject;
                ViewBag.Cadre = Cadre;
                ViewBag.fundingcategory = fundingcategory;
                ViewBag.allocatehead = allocatehead;
                ViewBag.taxservice = taxservice;
                ViewBag.internalfundingagency = internalfundingagency;
                ViewBag.staffcategory = staffcategory;
                ViewBag.fundingtype = fundingtype;
                ViewBag.fundedby = fundedby;
                ViewBag.fundingtypeWOBoth = Common.GetFundingTypeWOBoth();
                ViewBag.sponprojectcategory = sponprojectcategory;
                ViewBag.indfundinggovtbody = indfundinggovtbody;
                ViewBag.indfundingnongovtbody = indfundingnongovtbody;
                ViewBag.forgnfundinggovtbody = forgnfundinggovtbody;
                ViewBag.forgnfundingnongovtbody = forgnfundingnongovtbody;
                ViewBag.typeofproject = typeofproject;
                ViewBag.constaxtype = constaxtype;
                ViewBag.taxregstatus = taxregstatus;
                //model.Inputdate = DateTime.Now;

                //ViewBag.Msg = System.Web.HttpContext.Current.Request.ApplicationPath;
                // model.ProposalID = 0;
                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }


        [HttpPost]
        public ActionResult ProjectOpening(CreateProjectModel model, HttpPostedFileBase[] file, HttpPostedFileBase taxprooffile)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                //var username = User.Identity.Name;
                //int userid = Common.GetUserid(username);
                var country = Common.getCountryList();
                var department = Common.getDepartment();
                var gender = Common.getGender();
                var PI = Common.GetProjectPIWithDetails();
                var Doctype = Common.GetDocTypeList(9);
                var scheme = Common.getschemes();
                var agency = Common.getagency();
                var projecttype = Common.getprojecttype();
                var proposalnumber = Common.getproposalnumber();
                var ministry = Common.getMinistry();
                var projectsponsubtype = Common.getsponprojectsubtype();
                var projectconssubtype = Common.getconsprojectsubtype();
                var categoryofproject = Common.getcategoryofproject();
                var Cadre = Common.getFacultyCadre();
                var fundingcategory = Common.getconsfundingcategory();
                var allocatehead = Common.getallocationhead();
                var taxservice = Common.gettaxservice();
                var internalfundingagency = Common.getinternalfundingagency();
                var staffcategory = Common.getstaffcategory();
                var fundingtype = Common.getfundingtype();
                var fundedby = Common.getfundedby();
                var indfundinggovtbody = Common.getindfundinggovtbody();
                var indfundingnongovtbody = Common.getindfundingnongovtbody();
                var forgnfundinggovtbody = Common.getforgnfundinggovtbody();
                var forgnfundingnongovtbody = Common.getforgnfundingnongovtbody();
                var typeofproject = Common.gettypeofproject();
                var sponprojectcategory = Common.getsponprojectcategory();
                var constaxtype = Common.getconstaxtype();
                var taxregstatus = Common.gettaxregstatus();
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.projectcategory = Common.getprojectcategory();
                ViewBag.consScheme = AccountService.getcategory(2);
                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                ViewBag.finYearList = Common.GetFinYearList();
                ViewBag.ProjectList = Common.GetMainProjectNumberList(model.Prjcttype ?? 0);
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.Scheme = AccountService.getcategory(1);
                ViewBag.Agency = agency;
                ViewBag.projecttype = projecttype;
                ViewBag.proposalnumber = proposalnumber;
                ViewBag.ministry = ministry;
                ViewBag.projectsponsubtype = projectsponsubtype;
                ViewBag.projectconssubtype = projectconssubtype;
                ViewBag.categoryofproject = categoryofproject;
                ViewBag.Cadre = Cadre;
                ViewBag.fundingcategory = fundingcategory;
                ViewBag.allocatehead = allocatehead;
                ViewBag.taxservice = taxservice;
                ViewBag.internalfundingagency = internalfundingagency;
                ViewBag.staffcategory = staffcategory;
                ViewBag.fundingtype = fundingtype;
                ViewBag.fundingtypeWOBoth = Common.GetFundingTypeWOBoth();
                ViewBag.fundedby = fundedby;
                ViewBag.sponprojectcategory = sponprojectcategory;
                ViewBag.indfundinggovtbody = indfundinggovtbody;
                ViewBag.indfundingnongovtbody = indfundingnongovtbody;
                ViewBag.forgnfundinggovtbody = forgnfundinggovtbody;
                ViewBag.forgnfundingnongovtbody = forgnfundingnongovtbody;
                ViewBag.typeofproject = typeofproject;
                ViewBag.constaxtype = constaxtype;
                ViewBag.taxregstatus = taxregstatus;
                // model.PIEmail = model.PIEmail;

                var ForgnProjectFundingGovtBody = model.ForgnProjectFundingGovtBody_Qust_1;


                ViewBag.SelectedIds = ForgnProjectFundingGovtBody;
                if (ModelState.IsValid)
                {
                    Nullable<decimal> ttlAllowVal = 0, ttlEMIVal = 0;
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    if (model.IsYearWiseAllocation)
                    {
                        foreach (var item in model.YearWiseHead)
                        {
                            Nullable<decimal> ttlEMIValOfYear = item.EMIValue != null ? item.EMIValue.Sum() : 0;
                            ttlAllowVal = ttlAllowVal + item.AllocationValueYW.Sum();
                            ttlEMIVal = ttlEMIVal + ttlEMIValOfYear;
                            var groupsVal = item.AllocationHeadYW.GroupBy(v => v);
                            if (item.AllocationHeadYW.Length != groupsVal.Count())
                            {
                                ViewBag.errMsg = "Duplicate allocation head exist in year " + item.Year + ". Please select a different allocation.";
                                return View(model);
                            }
                            if (ttlEMIValOfYear > 0 && ttlEMIValOfYear != item.EMIValueForYear)
                            {
                                ViewBag.errMsg = "Sum of No of Installment values is different from total installment value.";
                                return View(model);
                            }
                        }
                    }
                    else if(model.IsSubProject)
                    {
                        var pVals = Common.GetMainAndSubProjectValues(model.MainProjectId ?? 0,model.ProjectID ?? 0);
                        if(pVals.Item1 < (pVals.Item2 + model.BaseValue))
                        {
                            ViewBag.errMsg = "The total sanctioned value of sub projects exceeds the total sanctioned value of the main project.";
                            return View(model);
                        }
                    }
                    else if (model.TaxException && taxprooffile == null && String.IsNullOrEmpty(model.Docpathfornotax))
                    {
                        ViewBag.errMsg = "Tax exception proof field is required.";
                        return View(model);
                    }
                    else
                    {
                        ttlAllowVal = model.Allocationvalue.Sum();
                        var groupsVal = model.Allocationhead.GroupBy(v => v);
                        if (model.Allocationhead.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Duplicate allocation head exist. Please select a different allocation.";
                            return View(model);
                        }
                        ttlEMIVal = model.ArrayEMIValue != null ? model.ArrayEMIValue.Sum() : 0;
                    }
                    if (ttlEMIVal > 0 && ttlEMIVal != model.BaseValue)
                    {
                        ViewBag.errMsg = "Overall installment values is different from project value.";
                        return View(model);
                    }
                    else if (model.IsYearWiseAllocation && ttlAllowVal != model.BaseValue)
                    {
                        ViewBag.errMsg = "Overall allocation values is different from project value.";
                        return View(model);
                    }
                    if (model.CoPIname != null && model.CoPIname[0] != 0)
                    {
                        var groupsVal = model.CoPIname.GroupBy(v => v);
                        if (model.CoPIname.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Co PI are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                        else if (model.CoPIname.Contains(model.PIname ?? 0))
                        {
                            ViewBag.errMsg = "Co PI should not be the PI what you have declared. Please rectify and submit again.";
                            return View(model);
                        }
                    }

                    if (taxprooffile != null)
                    {
                        string taxprooffilename = Path.GetFileName(taxprooffile.FileName);
                        var docextension = Path.GetExtension(taxprooffilename);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
                            return RedirectToAction("ProjectOpening", "Project");
                        }
                    }
                    for (int i = 0; i < model.DocType.Length; i++)
                    {
                        if (file[i] != null)
                        {
                            string docname = Path.GetFileName(file[i].FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
                                return View(model);
                            }
                        }

                    }
                    ProjectService _ps = new ProjectService();
                    model.ProjectcrtdID = logged_in_userid;

                    var projectid = _ps.ProjectOpening(model, file, taxprooffile);

                    if ((model.ProjectID == 0 || model.ProjectID == null) && projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        ViewBag.succMsg = "Project has been opened successfully with Project number - " + projectnumber + ".";
                        return View(model);
                    }
                    if (model.ProjectID > 0 && projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        ViewBag.succMsg = "Project - " + projectnumber + " updated successfully.";
                        return View(model);
                    }
                    else if (projectid == 0)
                    {
                        ViewBag.errMsg = "Project " + model.Projecttitle + "Already Exists";
                    }
                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";

                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                //  return RedirectToAction("ProjectOpening", "Project");
                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }

        public ActionResult ViewProject(int ProjectId)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                CreateProjectModel model = new CreateProjectModel();
                var country = Common.getCountryList();
                var department = Common.getDepartment();
                var gender = Common.getGender();
                var PI = Common.GetProjectPIWithDetails();
                var Doctype = Common.GetDocTypeList(9);
                var scheme = Common.getschemes();
                var agency = Common.getagency();
                var projecttype = Common.getprojecttype();
                var proposalnumber = Common.getproposalnumber();
                var ministry = Common.getMinistry();
                var projectsponsubtype = Common.getsponprojectsubtype();
                var projectconssubtype = Common.getconsprojectsubtype();
                var categoryofproject = Common.getcategoryofproject();
                var Cadre = Common.getFacultyCadre();
                var fundingcategory = Common.getconsfundingcategory();
                var allocatehead = Common.getallocationhead();
                var taxservice = Common.gettaxservice();
                var internalfundingagency = Common.getinternalfundingagency();
                var staffcategory = Common.getstaffcategory();
                var fundingtype = Common.getfundingtype();
                var fundedby = Common.getfundedby();
                var indfundinggovtbody = Common.getindfundinggovtbody();
                var indfundingnongovtbody = Common.getindfundingnongovtbody();
                var forgnfundinggovtbody = Common.getforgnfundinggovtbody();
                var forgnfundingnongovtbody = Common.getforgnfundingnongovtbody();
                var typeofproject = Common.gettypeofproject();
                var sponprojectcategory = Common.getsponprojectcategory();
                var constaxtype = Common.getconstaxtype();
                var taxregstatus = Common.gettaxregstatus();
                ViewBag.Currency = Common.getCurrency(true);
                ViewBag.projectcategory = Common.getprojectcategory();
                ViewBag.consScheme = AccountService.getcategory(2);
                ViewBag.schemeCodeList = Common.GetSponsoredSchemeCodeList();
                ViewBag.finYearList = Common.GetFinYearList();
                //ViewBag.ProjectList = new List<MasterlistviewModel>();
                ViewBag.country = country;
                ViewBag.deprtmnt = department;
                ViewBag.gender = gender;
                ViewBag.Docmenttype = Doctype;
                ViewBag.PI = PI;
                ViewBag.Scheme = AccountService.getcategory(1);
                ViewBag.Agency = agency;
                ViewBag.projecttype = projecttype;
                ViewBag.proposalnumber = proposalnumber;
                ViewBag.ministry = ministry;
                ViewBag.projectsponsubtype = projectsponsubtype;
                ViewBag.projectconssubtype = projectconssubtype;
                ViewBag.categoryofproject = categoryofproject;
                ViewBag.Cadre = Cadre;
                ViewBag.fundingcategory = fundingcategory;
                ViewBag.allocatehead = allocatehead;
                ViewBag.taxservice = taxservice;
                ViewBag.internalfundingagency = internalfundingagency;
                ViewBag.staffcategory = staffcategory;
                ViewBag.fundingtype = fundingtype;
                ViewBag.fundingtypeWOBoth = Common.GetFundingTypeWOBoth();
                ViewBag.fundedby = fundedby;
                ViewBag.sponprojectcategory = sponprojectcategory;
                ViewBag.indfundinggovtbody = indfundinggovtbody;
                ViewBag.indfundingnongovtbody = indfundingnongovtbody;
                ViewBag.forgnfundinggovtbody = forgnfundinggovtbody;
                ViewBag.forgnfundingnongovtbody = forgnfundingnongovtbody;
                ViewBag.typeofproject = typeofproject;
                ViewBag.constaxtype = constaxtype;
                ViewBag.taxregstatus = taxregstatus;

                model = ProjectService.EditProject(ProjectId);
                ViewBag.ProjectList = model.MainProjectList;
                //int prjtype = 0;
                //if(model.Prjcttype==1)
                //{
                //    prjtype = 1;
                //}
                //else
                //{
                //    prjtype = 2;
                //}
                //    ViewBag.ProjectList = Common.GetMainProjectNumberList(prjtype);
                //ViewBag.Msg = System.Web.HttpContext.Current.Request.ApplicationPath;
                // model.ProposalID = 0;

                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [Authorize]
        public ActionResult ProjectEnhancement(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectEnhancementModel model = new ProjectEnhancementModel();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var allocatehead = Common.getallocationhead();
                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.allocatehead = allocatehead;
                //ViewBag.deprtmnt = Common.getDepartment();
                //ViewBag.PI = Common.GetProjectPIWithDetails();
                ViewBag.Cadre = Common.getFacultyCadre();
                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }
        [Authorize]
        [HttpPost]
        public ActionResult ProjectEnhancement(ProjectEnhancementModel model, HttpPostedFileBase file)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();
                var allocatehead = Common.getallocationhead();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;
                ViewBag.allocatehead = allocatehead;
                //ViewBag.deprtmnt = Common.getDepartment();
                //ViewBag.PI = Common.GetProjectPIWithDetails();
                ViewBag.Cadre = Common.getFacultyCadre();
                if (ModelState.IsValid)
                {
                    if (model.Enhancement_Qust_1 == "No" && model.Extension_Qust_1 == "No")
                    {
                        ViewBag.errMsg = "At least do anyone of these actions enhancement or extension.";
                        return View(model);
                    }
                    if (model.Enhancement_Qust_1 == "Yes" && model.EnhancedAllocationvalue.Sum() != model.EnhancedSanctionValue && model.Allochead[0] != 0)
                    {
                        ViewBag.errMsg = "The enhanced sanction value is not equal to enhanced allocation value. Please check the values.";
                        return View(model);
                    }
                    string errMsg = Common.ValidateEnhancementAndExtension(model);
                    if (errMsg != "Valid")
                    {
                        ViewBag.errMsg = errMsg;
                        return View(model);
                    }
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };

                    if (file != null)
                    {
                        string docname = Path.GetFileName(file.FileName);
                        var docextension = Path.GetExtension(docname);
                        if (!allowedExtensions.Contains(docextension))
                        {
                            ViewBag.errMsg = "Please upload any one of these type doc [.pdf, .doc, .docx]";
                            return View(model);
                        }
                    }
                    if (model.Allocationhead != null && model.Allocationhead[0] != 0)
                    {
                        var groupsVal = model.Allocationhead.GroupBy(v => v);
                        if (model.Allocationhead.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Allocation heads are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                    }
                    if (model.Allochead != null && model.Allochead[0] != 0)
                    {
                        var groupsVal = model.Allochead.GroupBy(v => v);
                        if (model.Allochead.Length != groupsVal.Count())
                        {
                            ViewBag.errMsg = "Some of the Allocation heads are duplicated. Please rectify and submit again.";
                            return View(model);
                        }
                    }

                    ProjectService _ps = new ProjectService();
                    model.CrtdUserid = logged_in_userid;

                    var projectid = _ps.ProjectEnhancement(model, file);
                    if (projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        if (model.Enhancement_Qust_1 == "Yes" && model.Extension_Qust_1 == "No")
                        {
                            ViewBag.succMsg = "Enhancement successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }

                        if (model.Extension_Qust_1 == "Yes" && model.Enhancement_Qust_1 == "No")
                        {
                            ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }
                        if (model.Extension_Qust_1 == "Yes" && model.Enhancement_Qust_1 == "Yes")
                        {
                            ViewBag.succMsg = "Enhancement and Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }
                    }
                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";


                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                //  return RedirectToAction("ProjectOpening", "Project");
                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }

        [Authorize]
        [HttpPost]
        public ActionResult ProjectExtension(ProjectEnhancementModel model, HttpPostedFileBase file)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                if (ModelState.IsValid)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
                    if ((model.AttachmentName != null && model.AttachmentName != ""))
                    {

                        if (file != null)
                        {
                            string docname = Path.GetFileName(file.FileName);
                            var docextension = Path.GetExtension(docname);
                            if (!allowedExtensions.Contains(docextension))
                            {
                                ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
                                View(model);
                            }
                        }
                        ProjectService _ps = new ProjectService();
                        model.CrtdUserid = logged_in_userid;

                        var projectid = _ps.ProjectExtension(model, file);
                        if (projectid > 0)
                        {
                            var projectnumber = Common.getprojectnumber(projectid);
                            ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }

                        else
                            ViewBag.errMsg = "Something went wrong please contact administrator";

                    }

                    else
                    {
                        ProjectService _ps = new ProjectService();
                        model.CrtdUserid = logged_in_userid;

                        var projectid = _ps.ProjectExtension(model, file);
                        if (projectid > 0)
                        {
                            var projectnumber = Common.getprojectnumber(projectid);
                            ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
                            return View(model);
                        }

                        else
                            ViewBag.errMsg = "Something went wrong please contact administrator";
                    }
                }
                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }

                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }
        [Authorize]
        public ActionResult ProjectExtension(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectEnhancementModel model = new ProjectEnhancementModel();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }
        //[Authorize]
        //[HttpPost]
        //public ActionResult ProjectExtension(ProjectEnhancementModel model)
        //{
        //    try
        //    {
        //        string user_logged_in = User.Identity.Name;
        //        var data = Common.getUserIdAndRole(user_logged_in);
        //        int logged_in_userid = data.Item1;
        //        int user_role = data.Item2;
        //        //if (user_role != 1)
        //        //    return RedirectToAction("Index", "Home");
        //        var Projecttitle = Common.GetProjecttitledetails();
        //        var projecttype = Common.getprojecttype();

        //        ViewBag.Project = Projecttitle;
        //        ViewBag.projecttype = projecttype;

        //        if (ModelState.IsValid)
        //        {
        //            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".DOC", ".DOCX", ".PDF" };
        //            if ((model.AttachmentName != null && model.AttachmentName != ""))
        //            {

        //                if (model.file != null)
        //                {
        //                    string docname = Path.GetFileName(model.file.FileName);
        //                    var docextension = Path.GetExtension(docname);
        //                    if (!allowedExtensions.Contains(docextension))
        //                    {
        //                        ModelState.AddModelError("", "Please upload any one of these type doc [.pdf, .doc, .docx]");
        //                        View(model);
        //                    }
        //                }
        //                ProjectService _ps = new ProjectService();
        //                model.CrtdUserid = logged_in_userid;

        //                var projectid = _ps.ProjectExtension(model);
        //                if (projectid > 0)
        //                {
        //                    var projectnumber = Common.getprojectnumber(projectid);
        //                    ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
        //                    return View(model);
        //                }

        //                else
        //                    ViewBag.errMsg = "Something went wrong please contact administrator";

        //            }

        //            else
        //            {
        //                ProjectService _ps = new ProjectService();
        //                model.CrtdUserid = logged_in_userid;

        //                var projectid = _ps.ProjectExtension(model);
        //                if (projectid > 0)
        //                {
        //                    var projectnumber = Common.getprojectnumber(projectid);
        //                    ViewBag.succMsg = "Extension successfully done for Project - " + projectnumber + ".";
        //                    return View(model);
        //                }

        //                else
        //                    ViewBag.errMsg = "Something went wrong please contact administrator";
        //            }
        //        }
        //        else
        //        {
        //            string messages = string.Join("", ModelState.Values
        //                                .SelectMany(x => x.Errors)
        //                                .Select(x => x.ErrorMessage));

        //            ViewBag.errMsg = messages;
        //        }

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        ViewBag.errMsg = "Something went wrong please contact administrator";
        //        return View(model);
        //    }

        //}
        [HttpGet]
        public JsonResult GetProjectList()
        {
            object output = ProjectService.GetProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSearchProposalList(string keyword)
        {
            object output = ProposalService.GetProposalDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult EditProject(int ProjectId)
        {
            object output = ProjectService.EditProject(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteProject(int ProjectId)
        {
            object output = ProjectService.DeleteProject(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ShowDocument(string file, string filepath)
        {
            try
            {
                int roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1 && roleId != 3)
                //    return new EmptyResult();
                string fileType = Common.GetMimeType(Path.GetExtension(file));
                byte[] fileData = file.GetFileData(Server.MapPath(filepath));
                Response.AddHeader("Content-Disposition", "inline; filename=\"" + file + "\"");
                return File(fileData, fileType);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException(404, "File not found.");
            }
        }



        [HttpPost]
        public JsonResult Loadproposaldetailsbyid(int ProposalId)
        {

            object output = ProjectService.getproposaldetails(ProposalId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProposalList()
        {
            object output = ProjectService.GetProposalDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Loadprojectdetailsbytype(string projecttype)
        {
            projecttype = projecttype == "" ? "0" : projecttype;
            var locationdata = ProjectService.LoadProjecttitledetails(Convert.ToInt32(projecttype));
            return Json(locationdata, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEnhancedProjectList()
        {
            object output = ProjectService.GetEnhancedProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult LoadProjectdetailsforenhance(int ProjectId)
        {

            object output = ProjectService.getprojectdetailsforenhance(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public JsonResult DeleteEnhancement(int EnhanceId)
        {
            int userId = Common.GetUserid(User.Identity.Name);
            ProjectService ps = new ProjectService();
            bool output = ps.DeleteEnhamcement(EnhanceId, userId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public JsonResult EditProjectenhancement(int EnhanceId)
        {
            object output = ProjectService.EditEnhancement(EnhanceId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExtendedProjectList()
        {
            object output = ProjectService.GetExtendedProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LoadProjectdetailsforextend(int ProjectId)
        {

            object output = ProjectService.getprojectdetailsforextension(ProjectId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public JsonResult EditProjectextension(int ExtensionId)
        {
            object output = ProjectService.EditExtension(ExtensionId);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [Authorized]
        [HttpPost]
        public JsonResult LoadAgencycodebyagency(string agencyid)
        {
            object output = AccountService.Geteditagency(Convert.ToInt32(agencyid));//AccountService.getagencycode(Convert.ToInt32(agencyid));
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CloseProject(int pId = 0)
        {
            try
            {
                var roleId = Common.GetRoleId(User.Identity.Name);
                //if (roleId != 1)
                //    return RedirectToAction("Index", "Home");
                ProjectClosingModel model = new ProjectClosingModel();
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                return View(model);

            }
            catch (Exception ex)
            {

                return View();
            }

        }
        [Authorize]
        [HttpPost]
        public ActionResult CloseProject(ProjectClosingModel model)
        {
            try
            {
                string user_logged_in = User.Identity.Name;
                var data = Common.getUserIdAndRole(user_logged_in);
                int logged_in_userid = data.Item1;
                int user_role = data.Item2;
                //if (user_role != 1)
                //    return RedirectToAction("Index", "Home");
                var Projecttitle = Common.GetProjecttitledetails();
                var projecttype = Common.getprojecttype();

                ViewBag.Project = Projecttitle;
                ViewBag.projecttype = projecttype;

                if (ModelState.IsValid)
                {

                    ProjectService _ps = new ProjectService();
                    model.UpdtUserid = logged_in_userid;
                    model.Updt_TS = DateTime.Now;

                    var projectid = _ps.CloseProject(model);
                    if (projectid > 0)
                    {
                        var projectnumber = Common.getprojectnumber(projectid);
                        ViewBag.succMsg = "Project - " + projectnumber + "has been closed successfully.";
                        return View(model);
                    }

                    else
                        ViewBag.errMsg = "Something went wrong please contact administrator";
                }

                else
                {
                    string messages = string.Join("", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }

                return View(model);
            }
            catch (Exception ex)
            {

                ViewBag.errMsg = "Something went wrong please contact administrator";
                return View(model);
            }

        }

        [HttpGet]
        public JsonResult GetClosedProjectList()
        {
            object output = ProjectService.GetClosedProjectList();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProjectStatus()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetProjectDetails()
        {
            object output = ProjectService.GetProjectDetails();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadActionDDL()
        {
            try
            {
                object output = ProjectService.LoadControls();
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Error:LoadControls", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateStatusDetails(UpdateProjectStatusModel model)
        {
            try
            {
                ProjectStatusModel newModel = new ProjectStatusModel();
                var user = User.Identity.Name;
                var UserId = Common.GetUserid(user);
                string docname = "";
                if (ModelState.IsValid)
                {
                    if (model.file != null)
                    {
                        docname = Path.GetFileName(model.file.FileName);
                        var fileId = Guid.NewGuid().ToString();
                        docname = fileId + "_" + docname;
                        model.file.SaveAs(Server.MapPath("~/Content/OtherDocuments/" + docname));
                    }
                    int result = ProjectService.UpdateProjectDetails(model, UserId, docname);
                    if (result == 1)
                    {
                        ViewBag.SuccMsg = "Status has been updated successfully";
                    }
                    else
                    {
                        ViewBag.ErrMsg = "Somenthing went wrong please contact administrator";
                    }

                }
                else
                {
                    string messages = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                    ViewBag.errMsg = messages;
                }
                return View("ProjectStatus", newModel);
            }
            catch (Exception ex)
            {
                ProjectStatusModel newModel = new ProjectStatusModel();
                ViewBag.ErrMsg = "Somenthing went wrong please contact administrator";
                return View("ProjectStatus", newModel);
            }
        }

        [HttpPost]
        public ActionResult PopupUpdateStatus(int ProjectId, string StatusId)
        {
            try
            {
                UpdateProjectStatusModel model = new UpdateProjectStatusModel();
                model.ProjectID = ProjectId;
                model.StatusID = StatusId;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return new EmptyResult();
            }
        }

        //[HttpPost]
        //public JsonResult SearchProjectList(/*int ProjectType, string ProposalNumber, string FromSODate, string ToSOdate, DateTime Fromdate, DateTime Todate*/)
        //{
        //    object output = ProjectService.SearchProjectList(ProjectType, ProposalNumber, FromSODate, ToSOdate);
        //    //object output = "";
        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}
        

        public ActionResult _ExtensionandEnhancementHistory(int Projectid)
        {
            ProjectService _ps = new ProjectService();
            ProjectEnhanceandExtenDetailsModel model = new ProjectEnhanceandExtenDetailsModel();
            model = _ps.GetEnhancementandExtensionDetails(Projectid);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SearchProjectList(int pageIndex, int pageSize, ProjectSearchFieldModel model, DateFilterModel PrpsalApprovedDate)
        {

            //ProjectSearchFieldModel model = new ProjectSearchFieldModel();

            object output = ProjectService.SearchProjectList(model, pageIndex, pageSize, PrpsalApprovedDate);
            //object output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
    }
}