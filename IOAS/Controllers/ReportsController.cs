using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.IO;
using IOAS.GenericServices;
using IOAS.Models;
using IOAS.Filter;
using DataAccessLayer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;

namespace IOAS.Controllers
{
    [Authorized]
    public class ReportsController : Controller
    {
        private AdminService adminService = new AdminService();

        public ActionResult List()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsReport = db.getReportDetails(-1);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

                var result = Converter.DataTableToDict(dtReport);
                var model = Converter.GetEntities<SqlReportModel>(dtReport);
                var reports = new PagedData<SqlReportModel>();
                reports.Data = model;
                return View(reports);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult List(int page, int size)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataSet dsReport = db.getReportDetails(-1);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                var result = Converter.DataTableToDict(dtReport);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ReportBuilder()
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dt = db.getAllViews();
            SqlReportModel model = new SqlReportModel();
            List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
            List<SqlViewsModel> vwList = new List<SqlViewsModel>();

            vwList = Converter.GetEntityList<SqlViewsModel>(dt);

            var roles = AdminService.GetRoles();
            var modules = AdminService.GetModules();

            //ViewBag.Tables = vwList;
            ViewBag.TableProperties = vwFields;
            ViewBag.Modules = modules;
            ViewBag.Tables = Converter.GetEntityList<SqlViewsModel>(dt);
            var selectedRoles = new List<RolesModel>();
            model.AvailableRoles = roles;
            model.SelectedRoles = new List<RolesModel>();
            //model.AvailableFields = vwFields;
            //model.SelectedFields = new List<SqlViewsPropertyModel>();

            ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
            ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
            ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

            return View("ReportBuilder", model);
        }


        [Authorize]
        public ActionResult EditReport(int ReportID)
        {
            SqlReportModel model = new SqlReportModel();

            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dt = db.getAllViews();
            List<SqlViewsPropertyModel> vwFields = new List<SqlViewsPropertyModel>();
            List<SqlViewsModel> vwList = new List<SqlViewsModel>();
            vwList = Converter.GetEntityList<SqlViewsModel>(dt);

            var roles = AdminService.GetRoles();
            var modules = AdminService.GetModules();

            ViewBag.TableProperties = vwFields;
            ViewBag.Modules = modules;
            ViewBag.Tables = vwList;
            var selectedRoles = new List<RolesModel>();
            model.AvailableRoles = roles;
            model.SelectedRoles = new List<RolesModel>();

            ViewBag.Roles = new MultiSelectList(roles, "RoleId", "RoleName");
            ViewBag.SelectedRoles = new MultiSelectList(selectedRoles, "RoleId", "RoleName");
            ViewBag.fields = new MultiSelectList(vwFields, "ID", "fieldName");

            if (ReportID > 0)
            {
                DataSet dsReport = db.getReportDetails(ReportID);
                var result = new { };
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    var dtReport = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                    var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                    var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                    var dtSelectedRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                    var dtRoles = Converter.GetEntities<RolesModel>(dsReport.Tables[4]);
                    DataRow report = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                    model = Converter.DataRowTonEntity<SqlReportModel>(report);
                    model.dtReportFields = dtFields;
                    model.dtFilterFields = dtFilter;
                    ViewBag.SelectedRoles = dtSelectedRoles;
                    //Note: Exclude already selected roles from available roles.
                    ViewBag.Roles = new MultiSelectList(dtRoles, "RoleID", "RoleName"); ;
                }
            }

            return View(model);
        }




        public ActionResult ReportMenu(string ReportName)
        {
            var user = User.Identity.Name;
            if (ReportName == null)
            {
                ReportName = "";
            }
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportByUser(-1, user, ReportName);
            DataTable dtReport = (dsReport != null && dsReport.Tables.Count > 0) ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            return View(model);
        }

        [Authorize]
        public ActionResult ReportViewer()
        {
            int ReportID = Convert.ToInt32(@Request.QueryString["ReportID"].ToString());
            var user = User.Identity.Name;
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportByUser(ReportID, user, "");
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                if (Convert.ToInt32(row["ReportID"].ToString()) == ReportID)
                {
                    ViewBag.ReportName = row["ReportName"].ToString();
                }

                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            return View("ReportViewer", model);
        }

        [Authorize]
        public ActionResult ReportView(int ReportID)
        {
            var user = User.Identity.Name;
            ListDatabaseObjects db = new ListDatabaseObjects();
            SqlReportModel model = new SqlReportModel();
            DataSet dsReport = db.getReportByUser(ReportID, user, "");
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

            List<SqlReportModel> vwList = new List<SqlReportModel>();
            foreach (DataRow row in dtReport.Rows)
            {
                if (Convert.ToInt32(row["ReportID"].ToString()) == ReportID)
                {
                    ViewBag.ReportName = row["ReportName"].ToString();
                }
                vwList.Add(new SqlReportModel
                {
                    ReportID = Convert.ToInt32(row["ReportID"].ToString()),
                    ReportName = row["ReportName"].ToString()
                });

            }

            ViewBag.Reports = vwList;
            return View("ReportViewer", model);
        }


        [HttpPost]
        public ActionResult ReportViewer(SqlReportModel model)
        {
            ListDatabaseObjects db = new ListDatabaseObjects();
            var dbView = db.getAllViews();
            DataTable dt = db.getAllViews();
            SqlViewsModel vwModel = new SqlViewsModel();
            List<SqlViewsPropertyModel> vwPropertyList = new List<SqlViewsPropertyModel>();

            List<SqlViewsModel> vwList = new List<SqlViewsModel>();
            foreach (DataRow row in dt.Rows)
            {
                vwList.Add(new SqlViewsModel
                {
                    ID = row["ID"].ToString(),
                    name = row["name"].ToString()
                });

            }
            ViewBag.Tables = vwList;
            ViewBag.Reports = dt;
            //WebGridClass.GetDetailsForGrid(vwList, "View List", "ID");
            return View("ReportViewer", model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public JsonResult getReportListByUser(string ReportName)
        {
            try
            {
                var user = User.Identity.Name;
                var ReportID = -1;
                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportByUser(ReportID, user, ReportName);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;

                var result = dtReport != null ? Converter.DataTableToDict(dtReport) : new List<Dictionary<string, object>>();
                var resultJson = new { result = result };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult getReportDetails(int ReportID)
        {
            try
            {
                var user = User.Identity.Name;

                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportDetails(ReportID);
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                dynamic filters = new Dictionary<string, Object>();
                if (dtReport.Rows.Count > 0)
                {
                    var data = db.pagingReportView(ReportID, "", "",1,10);

                    if (dsReport != null && dsReport.Tables.Count > 0)
                    {
                        var report = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                        var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                        var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                        var dtRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                        DataRow drReport = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                        model = Converter.DataRowTonEntity<SqlReportModel>(drReport);
                        model.dtReportFields = dtFields;
                        model.dtFilterFields = dtFilter;
                        model.SelectedRoles = dtRoles;

                        DataTable dtFilters = dsReport.Tables[2];

                        for (int i = 0; i < dtFilters.Rows.Count; i++)
                        {
                            if (dtFilters.Rows[i]["FieldType"].ToString() == "Dropdown")
                            {
                                var key = dtFilters.Rows[i]["ReportField"].ToString();
                                var dtResultField = db.getFilterDetails(ReportID, key);
                                if (dtResultField != null)
                                {
                                    filters[key] = Converter.DataTableToDict(dtResultField);
                                }

                            }
                        }
                    }
                    var result = data != null ? Converter.DataTableToDict(data.Item1) : new List<Dictionary<string, object>>();
                    var resultJson = new { result = result, recordCount = data.Item2, schema = model, filters = filters };
                    return Json(resultJson, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = new List<Dictionary<string, object>>(), recordCount = 0, schema = model, filters = filters }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult getReportDetailByUser(int ReportID)
        {
            try
            {
                var user = User.Identity.Name;

                ListDatabaseObjects db = new ListDatabaseObjects();
                SqlReportModel model = new SqlReportModel();
                DataSet dsReport = db.getReportByUser(ReportID, user, "");
                DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
                dynamic filters = new Dictionary<string, Object>();
                if (dtReport.Rows.Count > 0)
                {
                    var data = db.pagingReportView(ReportID, "", "",1,10);

                    if (dsReport != null && dsReport.Tables.Count > 0)
                    {
                        var report = Converter.GetEntities<SqlViewsModel>(dsReport.Tables[0]);
                        var dtFields = Converter.GetEntityList<ReportFieldModel>(dsReport.Tables[1]);
                        var dtFilter = Converter.GetEntityList<FilterFieldModel>(dsReport.Tables[2]);
                        var dtRoles = Converter.GetEntityList<RolesModel>(dsReport.Tables[3]);
                        DataRow drReport = (dsReport.Tables[0].Rows.Count > 0) ? dsReport.Tables[0].Rows[0] : null;
                        model = Converter.DataRowTonEntity<SqlReportModel>(drReport);
                        model.dtReportFields = dtFields;
                        model.dtFilterFields = dtFilter;
                        model.SelectedRoles = dtRoles;

                        DataTable dtFilters = dsReport.Tables[2];

                        for (int i = 0; i < dtFilters.Rows.Count; i++)
                        {
                            if (dtFilters.Rows[i]["FieldType"].ToString() == "Dropdown")
                            {
                                var key = dtFilters.Rows[i]["ReportField"].ToString();
                                var dtResultField = db.getFilterDetails(ReportID, key);
                                if (dtResultField != null)
                                {
                                    filters[key] = Converter.DataTableToDict(dtResultField);
                                }

                            }
                        }
                    }
                    var result = data != null ? Converter.DataTableToDict(data.Item1) : new List<Dictionary<string, object>>();
                    var resultJson = new { result = result, recordCount = data.Item2, schema = model, filters = filters };
                    return Json(resultJson, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = new List<Dictionary<string, object>>(), recordCount = 0, schema = model, filters = filters }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult searchReportDetails(int ReportID, string condition)
        {
            try
            {
                var user = User.Identity.Name;
                var criteria = "";
                int pageSize = 10;
                int pageIndex = 1;
                Dictionary<string, string> dtBetween = new Dictionary<string, string>();
                if (condition != null)
                {
                    var filter = System.Web.Helpers.Json.Decode(condition);
                    var properties = ((System.Web.Helpers.DynamicJsonObject)filter).GetDynamicMemberNames();
                    var key = "";
                    var val = "";
                    foreach (var item in properties)
                    {
                        if (item.IndexOf("ddl_") > -1 && criteria == "")
                        {
                            criteria = item.Replace("ddl_", "") + " = '" + filter[item] + "'";
                        }
                        else if (item.IndexOf("ddl_") > -1 && criteria != "")
                        {
                            criteria = criteria + " and " + item.Replace("ddl_", "") + " = '" + filter[item] + "'";
                        }
                        if (item.IndexOf("txt") > -1 && item.IndexOf("_from") > -1 && filter[item].IndexOfAny(new char[] { '/', '-' }) > -1)
                        {
                            key = item.Replace("txt", "").Replace("_from", "");
                            val = "convert(varchar(12), " + item.Replace("txt", "").Replace("_from", "") + ", 103)" + " between '" + filter[item] + "' and '";
                            dtBetween.Add(key, val);
                        }
                        else if (item.IndexOf("txt") > -1 && item.IndexOf("_to") > -1 && filter[item].IndexOfAny(new char[] { '/', '-' }) > -1)
                        {
                            key = item.Replace("txt", "").Replace("_to", "");
                            val = filter[item] + "'";
                            if (dtBetween[key] != null)
                            {
                                dtBetween[key] = dtBetween[key] + val;
                            }

                        }
                        else if (item.IndexOf("txt") > -1 && item.IndexOf("_from") > -1)
                        {
                            key = item.Replace("txt", "").Replace("_from", "");
                            val = item.Replace("txt", "").Replace("_from", "") + " between " + filter[item] + " and ";
                            dtBetween.Add(key, val);
                        }
                        else if (item.IndexOf("txt") > -1 && item.IndexOf("_to") > -1)
                        {
                            key = item.Replace("txt", "").Replace("_to", "");
                            val = filter[item];
                            if (dtBetween[key] != null)
                            {
                                dtBetween[key] = dtBetween[key] + val;
                            }

                        }
                        else if (item.IndexOf("txt") > -1 && criteria == "")
                        {
                            criteria = item.Replace("txt", "") + " like '%" + filter[item] + "%'";
                        }
                        else if (item.IndexOf("txt") > -1 && criteria != "")
                        {
                            criteria = criteria + " and " + item.Replace("txt", "") + " like '%" + filter[item] + "%'";
                        }
                        else if (item == "pageSize")
                            pageSize = Convert.ToInt32(filter[item]);
                        else if (item == "pageIndex")
                            pageIndex = Convert.ToInt32(filter[item]);
                    }
                }

                foreach(var item in dtBetween.Keys)
                {
                    //criteria = criteria + dtBetween[item];
                    criteria = String.IsNullOrEmpty(criteria) ? criteria + dtBetween[item] : criteria + " and " + dtBetween[item];
                }

                ListDatabaseObjects db = new ListDatabaseObjects();
                //DataTable dtResult = new DataTable();
                //dtResult = db.getReportView(ReportID, criteria, user);
                var data = db.pagingReportView(ReportID, criteria, user,pageIndex,pageSize);
                var result = data != null ? Converter.DataTableToDict(data.Item1) : new List<Dictionary<string, object>>();
                var resultJson = new { result = result,recordCount = data.Item2 };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        //private bool validateDate(string strDate)
        //{
        //    bool isValid = false;
        //    string[] format = new string[] { "dd-mm-yyyy", "dd/mm/yyyy" };
        //    foreach(var item in format)
        //    {
        //        if (DateTime.TryParseExact(strDate, item, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
        //        {
        //            isValid = true;
        //        }
        //    }


        //    return isValid;
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getTables()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getAllTables();
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getTablesAndViews()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getAllTablesAndViews();
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getFields(string tableName)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getFieldDetails(tableName);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getRefFields(string tableName)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                string objType = db.getObjectType(tableName) != "VIEW" ?  "" : "view";
                DataTable dtFields = db.getAllProperties(tableName, objType);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public JsonResult getFilterDetails(int ReportID, string ReportField)
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                DataTable dtFields = db.getFilterDetails(ReportID, ReportField);
                var result = Converter.DataTableToDict(dtFields);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ValidateAntiForgeryToken]
        public JsonResult getRoles()
        {
            try
            {
                ListDatabaseObjects db = new ListDatabaseObjects();
                var roles = AdminService.GetRoles();
                return Json(roles, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveReportData(SqlReportModel model)
        {
            var user = User.Identity.Name;
            var msg = "";
            if (model.ReportName == null || model.ReportName == "")
            {
                msg = "Report Name is missing";
            }
            else if (model.ModuleID == 0)
            {
                msg = "Module Name is missing";
            }
            else if (model.TableName == null || model.TableName == "")
            {
                msg = "Table Name is missing";
            }
            else if (model.SelectedRoles == null || model.SelectedRoles.Count == 0)
            {
                msg = "Please select roles";
            }
            else if (model.dtReportFields == null || model.dtReportFields.Count == 0)
            {
                msg = "Please select report fields(atleast one)";
            }
            if (msg != "")
            {
                var error = new { msg = msg };
                return Json(error, JsonRequestBehavior.AllowGet);
            }
            ListDatabaseObjects db = new ListDatabaseObjects();
            ReportsProfileHandler prop = new ReportsProfileHandler();
            prop.ReportID = model.ReportID;
            prop.ReportName = model.ReportName;
            prop.ReportDescription = "";
            prop.TableName = model.TableName;

            prop.dtReportFields = Converter.ToDataTable<ReportFieldModel>(model.dtReportFields);
            prop.dtFilterFields = Converter.ToDataTable<FilterFieldModel>(model.dtFilterFields);
            prop.dtRoles = Converter.ToDataTable<RolesModel>(model.SelectedRoles);

            prop.IsActive = true;
            prop.RoleId = 0;
            prop.ModuleId = model.ModuleID;
            prop.UserId = AdminService.getUserByName(User.Identity.Name);
            prop.CanExport = model.CanExport;
            prop.ToPDF = model.ToPDF;
            prop.ToExcel = model.ToExcel;

            var reportId = db.AddReportDetails(prop);
            DataSet dsReport = db.getReportDetails(reportId);
            var result = new { };
            if (dsReport != null && dsReport.Tables.Count > 0)
            {
                var dtReport = Converter.DataTableToDict(dsReport.Tables[0]);
                var dtFields = Converter.DataTableToDict(dsReport.Tables[1]);
                var dtFilter = Converter.DataTableToDict(dsReport.Tables[2]);
                var dtRoles = Converter.DataTableToDict(dsReport.Tables[3]);
                var report = (dtReport != null && dtReport.Count > 0) ? dtReport[0] : null;
                var resultJson = new { Report = report, Fields = dtFields, Filter = dtFilter, Roles = dtRoles };
                return Json(resultJson, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public FileStreamResult ExportData(int ReportID, string filter, string filetype)
        {
            var user = User.Identity.Name;

            var data = System.Web.Helpers.Json.Decode(filter);

            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dtResult = new DataTable();
            bool CanExport = false;
            bool ToPDF = false;
            bool ToExcel = false;
            DataSet dsReport = db.getReportByUser(ReportID, user, "");
            DataTable dtReport = dsReport.Tables.Count > 0 ? dsReport.Tables[0] : null;
            if (dtReport != null && dtReport.Rows.Count > 0)
            {
                CanExport = Convert.ToBoolean(dtReport.Rows[0]["CanExport"]);
                ToPDF = Convert.ToBoolean(dtReport.Rows[0]["ToPDF"]);
                ToExcel = Convert.ToBoolean(dtReport.Rows[0]["ToExcel"]);
            }
            var criteria = "";
            var properties = ((System.Web.Helpers.DynamicJsonObject)data).GetDynamicMemberNames();
            foreach (var item in properties)
            {
                if (item.IndexOf("ddl_") > -1 && criteria == "")
                {
                    criteria = item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                else if (item.IndexOf("ddl_") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                if (item.IndexOf("txt") > -1 && criteria == "")
                {
                    criteria = item.Replace("txt_", "") + " like '%" + data[item] + "%'";
                }
                else if (item.IndexOf("txt") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("txt_", "") + " like '%" + data[item] + "%'";
                }
            }

            dtResult = db.getReportView(ReportID, criteria, user);
            if (ToPDF && filetype == "topdf")
            {
                return toPdf(dtResult);
            }
            else if (ToExcel && filetype == "toexcel")
            {
                return toSpreadSheet(dtResult);
            }
            else
            {
                MemoryStream workStream = new MemoryStream();
                Document document = new Document();
                PdfWriter.GetInstance(document, workStream).CloseStream = false;
                string msg = "Sorry, You are not authorized to perform this action. Please contact your administrator.";

                document.Open();
                document.Add(new Paragraph(msg));
                document.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return new FileStreamResult(workStream, "application/pdf");
            }

        }

        [HttpPost]
        [Authorize]
        public FileStreamResult ExportTo(int ReportID, string filter)
        {
            var user = User.Identity.Name;
            ListDatabaseObjects db = new ListDatabaseObjects();
            DataTable dtResult = new DataTable();
            var data = (filter == null) ? new { } : System.Web.Helpers.Json.Decode(filter);

            var criteria = "";
            var properties = ((System.Web.Helpers.DynamicJsonObject)data).GetDynamicMemberNames();
            foreach (var item in properties)
            {
                if (item.IndexOf("ddl_") > -1 && criteria == "")
                {
                    criteria = item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                else if (item.IndexOf("ddl_") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("ddl_", "") + " = '" + data[item] + "'";
                }
                if (item.IndexOf("txt") > -1 && criteria == "")
                {
                    criteria = item.Replace("txt_", "") + " like '%" + data[item] + "%'";
                }
                else if (item.IndexOf("txt") > -1 && criteria != "")
                {
                    criteria = criteria + " and " + item.Replace("txt_", "") + " like '%" + data[item] + "%'";
                }
            }
            dtResult = db.getReportView(ReportID, criteria, user);

            return toPdf(dtResult);
        }

        public FileStreamResult toPdf(DataTable dtReport)
        {
            if (dtReport != null)
            {
                string[] columnNames = (from dc in dtReport.Columns.Cast<DataColumn>()
                                        select dc.ColumnName).ToArray();
                int count = columnNames.Length;
                object[] array = new object[count];
                int cols = dtReport.Columns.Count;
                int rows = dtReport.Rows.Count;

                MemoryStream workStream = new MemoryStream();
                Document document = new Document();
                PdfWriter.GetInstance(document, workStream).CloseStream = false;

                document.Open();
                PdfPTable table = new PdfPTable(cols + 1);

                PdfPCell cellSNO = new PdfPCell(new Phrase("S.No", new Font(Font.FontFamily.HELVETICA, 14F)));
                cellSNO.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#cccccc"));
                cellSNO.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.AddCell(cellSNO);
                var total = 0;
                var widths = new int[cols + 1];
                var AllColwidths = new float[cols + 1];
                Font font = new Font(Font.FontFamily.HELVETICA, 14F);
                var colWidth = font.GetCalculatedBaseFont(true).GetWidth("S.NO");
                total += colWidth;
                widths[0] = colWidth;
                for (int i = 0; i < cols; i++)
                {
                    var w = font.GetCalculatedBaseFont(true).GetWidth(dtReport.Columns[i].ColumnName);
                    total += w;
                    widths[i + 1] = w;
                }

                for (int i = 0; i < widths.Length; i++)
                {
                    AllColwidths[i] = (float)widths[i] / total * 100;
                }
                table.SetWidths(AllColwidths);

                //creating table headers  
                for (int i = 0; i < cols; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dtReport.Columns[i].ColumnName, new Font(Font.FontFamily.HELVETICA, 14F)));
                    cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#cccccc"));

                    //cell.Colspan = 3;
                    //cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                    //cell.BorderColor = new BaseColor(System.Drawing.Color.Red); //Style
                    //cell.Border = Rectangle.BOTTOM_BORDER; // | Rectangle.TOP_BORDER;
                    //cell.BorderWidthBottom = 3f;

                    //PdfPCell cellCols = new PdfPCell();
                    //Chunk chunkCols = new Chunk();
                    //cellCols.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#548B54"));
                    //Font ColFont = FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD, BaseColor.WHITE);

                    //chunkCols = new Chunk(dtReport.Columns[i].ColumnName, ColFont);
                    //cellCols.Chunks.Add(chunkCols);

                    table.AddCell(cell);
                }


                var result = new float[cols];
                for (int k = 0; k < rows; k++)
                {
                    PdfPCell cellSNo = new PdfPCell(new Phrase((k + 1).ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                    string color = (k % 2 == 0) ? "#ffffff" : "#cccccc";
                    cellSNo.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(color));
                    table.AddCell(cellSNo);
                    for (int j = 0; j < cols; j++)
                    {
                        PdfPCell cellRows = new PdfPCell(new Phrase(dtReport.Rows[k][j].ToString(), new Font(Font.FontFamily.HELVETICA, 12)));
                        cellRows.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(color));
                        Font RowFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                        Chunk chunkRows = new Chunk(dtReport.Rows[k][j].ToString(), RowFont);
                        //cellRows.Chunks.Add(chunkRows);
                        cellRows.AddElement(chunkRows);
                        table.AddCell(cellRows);
                    }
                }
                Paragraph header = new Paragraph("List", FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD));
                PdfPCell headrCell = new PdfPCell(header);
                PdfPTable headerTbl = new PdfPTable(1);
                //headerTbl.TotalWidth = 300;
                headerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                headrCell.Border = 0;
                headrCell.PaddingLeft = 10;
                var Colwidth = new float[1];
                Colwidth[0] = total * 100;
                headerTbl.SetWidths(Colwidth);
                headerTbl.AddCell(headrCell);

                document.Add(headerTbl);
                document.Add(table);
                document.Close();

                //document.Open();
                //document.Add(new Paragraph("Hello World"));
                //document.Add(new Paragraph(DateTime.Now.ToString()));
                //document.Close();

                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

                return new FileStreamResult(workStream, "application/pdf");
            }
            else
            {
                return new FileStreamResult(new MemoryStream(), "application/pdf");
            }


        }

        public FileStreamResult toSpreadSheet(DataTable dtReport)
        {
            MemoryStream workStream = new MemoryStream();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);


            if (dtReport != null)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtReport, "Customers");
                    wb.SaveAs(workStream);
                    workStream.Position = 0;
                }
            }
            return new FileStreamResult(workStream, "application/vnd.ms-excel");
        }

    }
}
