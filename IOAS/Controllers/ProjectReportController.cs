using CrystalDecisions.CrystalReports.Engine;
using IOAS.GenericServices;
using IOAS.Infrastructure;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CrystalDecisions.Shared;

namespace IOAS.Controllers
{
    public class ProjectReportController : Controller
    {
        // GET: ProjectReport
        [Authorize]
        [HttpGet]
        public ActionResult Sanctionreport(string message, string Errormsg)
        {
            ViewBag.projtype = Common.getprojecttype();
            ViewBag.month = Common.Getmonth();
            ViewBag.year = Common.Getyear();
            ViewBag.report = Common.Getreport();
            if (message != null)
            {
                ViewBag.msg = message;
            }
            if (Errormsg != null)
            {
                ViewBag.error = Errormsg;
            }
            return View(); 
        }
        [Authorize]
        public ActionResult Projectreports(ProjectReportViewModel model)
        {
            try
            {
                ViewBag.projtype = Common.getprojecttype();
                ViewBag.month = Common.Getmonth();
                ViewBag.year = Common.Getyear();
                ViewBag.report = Common.Getreport();
                if (model.Reportname == "Department")
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "SanctionProjectReport.rpt"));
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    List<ProjectReportViewModel> listmodel = new List<ProjectReportViewModel>();
                    listmodel = ProjectReportService.Getdeptwiseproject(model);
                    if (listmodel.Count > 0)
                    {
                        rd.SetDataSource(listmodel);
                        var date = model.Month + "/" + model.year;
                        rd.SetParameterValue("monthdate", date);

                        if (model.Projecttype == 1)
                        {
                            rd.SetParameterValue("Heading", "DEPARTMENT WISE SPONSORED PROJECT SANCTIONED DURING");
                        }
                        else
                        {
                            rd.SetParameterValue("Heading", "DEPARTMENT WISE CONSULTANCY PROJECT SANCTIONED DURING");
                        }

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=Sanctionreport.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });

                    }
                }
                else if (model.Reportname == "Faculty")
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Facultywisesanction.rpt"));
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    List<ProjectReportViewModel> listmodel = new List<ProjectReportViewModel>();
                    listmodel = ProjectReportService.Getfacultywiseproject(model);
                    if (listmodel.Count > 0)
                    {
                        rd.SetDataSource(listmodel);
                        var date = model.Month + "/" + model.year;
                        rd.SetParameterValue("month", date);

                        if (model.Projecttype == 1)
                        {
                            rd.SetParameterValue("Heading", "FACULTY WISE SPONSORED PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "Sponsored");

                        }
                        else
                        {
                            rd.SetParameterValue("Heading", "FACULTY WISE CONSULTANCY PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "consultancy");
                        }

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=Sanctionreport.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });
                    }
                }
                else if (model.Reportname == "Agency")
                {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/CrystalReport"), "Agencywisesanction.rpt"));
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    List<ProjectReportViewModel> listmodel = new List<ProjectReportViewModel>();
                    listmodel = ProjectReportService.Getagencywiseproject(model);
                    if (listmodel.Count > 0)
                    {
                        rd.SetDataSource(listmodel);
                        var date = model.Month + "/" + model.year;
                        rd.SetParameterValue("monthdate", date);

                        if (model.Projecttype == 1)
                        {
                            rd.SetParameterValue("Heading", "AGENCY WISE SPONSORED PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "Sponsored");
                        }
                        else
                        {
                            rd.SetParameterValue("Heading", "AGENCY WISE CONSULTANCY PROJECT SANCTIONED DURING");
                            rd.SetParameterValue("protype", "Consultancy");
                        }

                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        Response.AddHeader("Content-Disposition", "inline; filename=Sanctionreport.pdf");
                        return File(stream, "application/pdf");
                    }
                    else
                    {
                        return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });
                    }
                }
                return RedirectToAction("Sanctionreport", new { message = "No records found for this type of search entry" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Sanctionreport", new { Errormsg = "Something went to wrong please contact admin." });
            }
            }
            
    }
}