using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.GenericServices
{
    public class ProjectReportService
    {
        public static List<ProjectReportViewModel>Getdeptwiseproject(ProjectReportViewModel model)
        {
            try
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 from D in context.vwFacultyStaffDetails
                                 orderby P.PIDepartment
                                 where (P.SanctionOrderDate.Value.Month == model.Month&&P.SanctionOrderDate.Value.Year==model.year && P.PIDepartment == D.DepartmentCode &&P.ProjectType==model.Projecttype)
                                 select new {D.DepartmentName,P.PIDepartment, P.ProjectNumber, P.ProjectTitle, P.SanctionOrderDate, P.SanctionValue, P.Remarks }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Getspon.Add(new ProjectReportViewModel()
                            {
                                //Departmentid=(Int32)query[i].PIDepartment,
                                PIDepartment = query[i].DepartmentName,
                                Projectnumber = query[i].ProjectNumber,
                                Projecttitle = query[i].ProjectTitle,
                                SanctionOrderDate = (DateTime)query[i].SanctionOrderDate,
                                SanctionValue = (decimal)query[i].SanctionValue,
                                Remarks = query[i].Remarks
                            });
                        }
                    }
                }
                return Getspon;
            }
            catch(Exception ex)
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();
                return Getspon;
            }
        }


        public static List<ProjectReportViewModel>Getfacultywiseproject(ProjectReportViewModel model)
        {
            try
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 from D in context.vwFacultyStaffDetails
                                 from PI in context.tblUser
                                 orderby P.PIDepartment
                                 where (P.SanctionOrderDate.Value.Month == model.Month && P.SanctionOrderDate.Value.Year == model.year && P.PIDepartment == D.DepartmentCode && P.ProjectType == model.Projecttype && P.PIName == PI.UserId)
                                 select new { D.DepartmentName, P.PIDepartment, P.PIName,PI.FirstName,P.AgencyRegisteredName, P.ProjectTitle,P.SanctionValue}).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Getspon.Add(new ProjectReportViewModel()
                            {
                                //Departmentid = (Int32)query[i].PIDepartment,
                                PIDepartment = query[i].DepartmentName,
                                PIName = query[i].FirstName,
                                AgencyRegisteredName=query[i].AgencyRegisteredName,
                                Projecttitle = query[i].ProjectTitle,
                                SanctionValue = (decimal)query[i].SanctionValue,
                                
                            });
                        }
                    }
                }
                return Getspon;
            }
            catch (Exception ex)
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();
                return Getspon;
            }
        }
        public static List<ProjectReportViewModel> Getagencywiseproject(ProjectReportViewModel model)
        {
            try
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from P in context.tblProject
                                 from D in context.vwFacultyStaffDetails
                                 from PI in context.tblUser
                                 orderby PI.FirstName
                                 where (P.SanctionOrderDate.Value.Month == model.Month && P.SanctionOrderDate.Value.Year == model.year && P.PIDepartment == D.DepartmentCode && P.ProjectType == model.Projecttype && P.PIName == PI.UserId)
                                 select new { D.DepartmentName, P.PIDepartment, P.PIName, PI.FirstName, P.AgencyRegisteredName, P.ProjectTitle, P.SanctionValue }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Getspon.Add(new ProjectReportViewModel()
                            {
                                //Departmentid = (Int32)query[i].PIDepartment,
                                PIDepartment = query[i].DepartmentName,
                                PIName = query[i].FirstName,
                                AgencyRegisteredName = query[i].AgencyRegisteredName,
                                Projecttitle = query[i].ProjectTitle,
                                SanctionValue = (decimal)query[i].SanctionValue,

                            });
                        }
                    }
                }
                return Getspon;
            }
            catch (Exception ex)
            {
                List<ProjectReportViewModel> Getspon = new List<ProjectReportViewModel>();
                return Getspon;
            }
        }
    }
}