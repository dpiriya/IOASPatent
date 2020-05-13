using IOAS.DataModel;
using IOAS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace IOAS.Infrastructure
{
    public class Common
    {
        public static string GetRolename(int RoleId)
        {

            using (var context = new IOASDBEntities())
            {
                var query = context.tblRole.FirstOrDefault(dup => dup.RoleId == RoleId);
                var userRoleName = "";
                if (query != null)
                {
                    userRoleName = query.RoleName;
                }
                context.Dispose();
                return userRoleName;
            }
        }
        public static int GetRoleId(string username)
        {

            var context = new IOASDBEntities();
            var query = (from U in context.tblUser
                         where (U.UserName == username && U.Status == "Active")
                         select U).FirstOrDefault();
            var userRoleId = 0;
            if (query != null)
            {
                userRoleId = (Int32)query.RoleId;
            }


            return userRoleId;

        }

        public static List<MenuListViewModel> Getaccessrole(int UserId)
        {
            List<MenuListViewModel> addmenu = new List<MenuListViewModel>();

            using (var context = new IOASDBEntities())
            {
                var addtionalroles = (from R in context.tblUserRole
                                      where R.UserId == UserId
                                      select R.RoleId).ToArray();
                var defaultrole = (from R in context.tblUser
                                   where (R.UserId == UserId)
                                   select R.RoleId).FirstOrDefault();

                var roles = (from RA in context.tblRoleaccess
                             from F in context.tblFunction
                             from M in context.tblModules
                             from MG in context.tblMenuGroup
                             where (addtionalroles.Contains(RA.RoleId) || RA.RoleId == defaultrole) && RA.FunctionId == F.FunctionId && F.ModuleID == M.ModuleID && F.MenuGroupID == MG.MenuGroupID
                             select new { F.FunctionId, F.FunctionName, F.ActionName, F.ControllerName, M.ModuleID, M.ModuleIcon, M.ModuleName, MG.MenuGroup, MG.MenuGroupID, F.MenuSeq }).Distinct().ToList();


                //This get rolewise menucount
                if (roles.Count > 0)
                {
                    //This query using get modules in roles count using distinct
                    var module = roles.Select(m => m.ModuleName).Distinct().ToArray();
                    var menugroup = roles.Select(m => m.MenuGroup).Distinct().ToArray();


                    for (int m = 0; m < module.Length; m++)
                    {
                        string moduleName = module[m];
                        var moduleIconName = (from C in roles where C.ModuleName == moduleName select C.ModuleIcon).FirstOrDefault(); //roles.Select(S => S.ModuleName == moduleName).FirstOrDefault();
                        List<submodulemenuviewmodel> submodules = new List<submodulemenuviewmodel>();
                        for (int i = 0; i < menugroup.Length; i++)
                        {

                            string submodule = menugroup[i];
                            var menu = roles.Where(S => S.ModuleName == moduleName && S.MenuGroup == submodule).FirstOrDefault();
                            if (menu != null)
                            {

                                submodules.Add(new submodulemenuviewmodel()
                                {

                                    Menugroupname = submodule,
                                    Submenu = (from sm in roles
                                               where sm.ModuleName == moduleName && sm.MenuGroup == submodule
                                               orderby sm.MenuSeq
                                               select new SubmenuViewModel()
                                               {
                                                   FunctionId = sm.FunctionId,
                                                   Functioname = sm.FunctionName,
                                                   Actionname = sm.ActionName,
                                                   Controllername = sm.ControllerName
                                               }).Distinct().ToList()
                                });
                            }
                        }
                        addmenu.Add(new MenuListViewModel()
                        {
                            Modulename = moduleName,
                            submodule = submodules,
                            ModuleIconName = moduleIconName
                        });

                    }
                }

                return addmenu;
            }
        }
        public static int GetUserid(string Username)
        {

            var context = new IOASDBEntities();
            var query = (from U in context.tblUser
                         where U.UserName == Username && U.Status == "Active"
                         select U).FirstOrDefault();
            var userId = 0;
            if (query != null)
            {
                userId = (Int32)query.UserId;
            }


            return userId;

        }
        public static int getseqncenumber(int financialyear)
        {
            try
            {
                var lastseqnum = 0;
                var context = new IOASDBEntities();
                var Proposalnum = (from proposal in context.tblProposal
                                   where proposal.FinancialYear == financialyear
                                   orderby proposal.ProposalId descending
                                   select proposal.ProposalNumber).FirstOrDefault();
                if (Proposalnum != null)
                {
                    var lastproposalnumber = Proposalnum;
                    var value = lastproposalnumber.Split('_').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var seqnum = Convert.ToInt32(number);
                    lastseqnum = seqnum + 1;
                    return lastseqnum;
                }
                else
                {
                    return lastseqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string GetNewSRBNumber()
        {
            try
            {
                var srbNum = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var num = (from srb in context.tblSRB
                               orderby srb.SRBId descending
                               select srb.SRBNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('_').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "SRB_" + seqnum;
                    }
                    else
                    {
                        return "SRB_1";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<MasterlistviewModel> GetUserlist(int Roleid)
        {
            try
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 where (U.RoleId == Roleid && U.Status == "Active")
                                 select new { U.UserId, U.UserName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            User.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].UserName
                            });
                        }
                    }

                }
                return User;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                return User;
            }
        }
        public static List<MasterlistviewModel> GetUserNameList()
        {
            try
            {
                List<MasterlistviewModel> getUser = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 where (U.Status == "Active")
                                 select new { U.UserId, U.UserName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            getUser.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].UserName

                            });
                        }
                    }
                }
                return getUser;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> getUser = new List<MasterlistviewModel>();
                return getUser;
            }
        }
        public static List<MasterlistviewModel> GetRoleList()
        {
            try
            {
                List<MasterlistviewModel> roleList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from R in context.tblRole
                                 where (R.Status == "Active")
                                 select new { R.RoleId, R.RoleName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            roleList.Add(new MasterlistviewModel()
                            {
                                id = query[i].RoleId,
                                name = query[i].RoleName
                            });
                        }
                    }
                }
                return roleList;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> roleList = new List<MasterlistviewModel>();
                return roleList;
            }
        }
        public static List<MasterlistviewModel> GetCommonHeadList(int categoryId, int groupId)
        {
            try
            {
                List<MasterlistviewModel> roleList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    roleList = (from h in context.tblCommonHeads
                                where h.CategoryId == categoryId
                                && h.GroupId == groupId
                                select new MasterlistviewModel()
                                {
                                    id = h.HeadId,
                                    name = h.Head
                                }).ToList();

                }
                return roleList;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }
        }
        public static List<CountryListViewModel> getCountryList()
        {
            try
            {
                List<CountryListViewModel> country = new List<CountryListViewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCountries
                                 orderby C.countryName
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            country.Add(new CountryListViewModel()
                            {
                                CountryID = query[i].countryID,
                                CountryCode = query[i].countryCode,
                                CountryName = query[i].countryName
                            });
                        }
                    }


                }

                return country;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public static List<MasterlistviewModel> getDepartment()
        {
            try
            {

                List<MasterlistviewModel> department = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from dept in context.vwFacultyStaffDetails
                                 orderby dept.DepartmentName
                                 group dept by dept.DepartmentName into g
                                 select new
                                 {
                                     Department = g.Key,
                                     deptCode = g.Select(m => m.DepartmentCode).FirstOrDefault()
                                 }).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            department.Add(new MasterlistviewModel()
                            {
                                name = query[i].Department,
                                code = query[i].deptCode
                            });
                        }
                    }


                }

                return department;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> getGender()
        {
            try
            {

                List<MasterlistviewModel> gender = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from gnder in context.tblCodeControl
                                 where (gnder.CodeName == "Gender")
                                 select gnder).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            gender.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail,
                            });
                        }
                    }

                }

                return gender;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> getPI()
        {
            try
            {

                List<MasterlistviewModel> PI = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.vwFacultyStaffDetails
                                 orderby C.FirstName
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PI.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].FirstName
                            });
                        }
                    }


                }

                return PI;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetProjectPIWithDetails()
        {
            try
            {

                List<MasterlistviewModel> PI = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.vwFacultyStaffDetails
                                     //join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     //where (C.RoleId == 7)
                                 orderby C.FirstName
                                 select new { C.DepartmentCode, C.UserId, C.FirstName, C.EmployeeId }).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PI.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].EmployeeId + "-" + query[i].FirstName, // + " " + query[i].LastName, + "-" + query[i].Institutecode,
                                code = query[i].DepartmentCode
                            });
                        }
                    }


                }

                return PI;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetPIWithDetails()
        {
            try
            {

                List<MasterlistviewModel> PI = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.vwFacultyStaffDetails
                                     //join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                     //where (C.RoleId == 7)
                                 orderby C.FirstName
                                 select new { C.UserId, C.FirstName, C.EmployeeId }).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PI.Add(new MasterlistviewModel()
                            {
                                id = query[i].UserId,
                                name = query[i].EmployeeId + "-" + query[i].FirstName,
                            });
                        }
                    }


                }

                return PI;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        //        using (var context = new IOASDBEntities())
        //        {
        //            var query = (from C in context.tblFunctionDocument                                 
        //                         where C.FunctionId == 4
        //                         orderby C.FunctionDocumentId
        //                         select C).ToList();


        //            if (query.Count > 0)
        //            {
        //                for (int i = 0; i < query.Count; i++)
        //                {
        //                    var docid = query[i].DocumentId;
        //                    var docquery = (from D in context.tblDocument
        //                                    where D.DocumentId == docid
        //                                    select D).FirstOrDefault();
        //                    Doctype.Add(new MasterlistviewModel()
        //                    {
        //                        id = docquery.DocumentId,
        //                        name = docquery.DocumentName,                               
        //                    });
        //                }
        //            }

        //        }

        //        return Doctype;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        public static Tuple<Int32, Int32> getUserIdAndRole(string username)
        {
            try
            {
                int userId = 0, role = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUser.FirstOrDefault(dup => dup.UserName == username);
                    if (query != null)
                    {
                        userId = query.UserId;
                        role = query.RoleId ?? 0;
                    }
                }
                return Tuple.Create(userId, role);
            }
            catch (Exception ex)
            {
                int userId = 0, role = 0;
                return Tuple.Create(userId, role);
            }
        }

        public static List<MasterlistviewModel> getschemes()
        {
            try
            {

                List<MasterlistviewModel> Scheme = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSchemes
                                 select new { C.SchemeId, C.SchemeName }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Scheme.Add(new MasterlistviewModel()
                            {
                                id = query[i].SchemeId,
                                name = query[i].SchemeName,
                            });
                        }
                    }

                }

                return Scheme;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> getagency()
        {
            try
            {

                List<MasterlistviewModel> Agency = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblAgencyMaster
                                 where C.Status == "Active"
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Agency.Add(new MasterlistviewModel()
                            {
                                id = query[i].AgencyId,
                                name = query[i].AgencyName,
                                code = query[i].AgencyCode
                            });
                        }
                    }

                }

                return Agency;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getprojecttype()
        {
            try
            {

                List<CodeControllistviewModel> Projecttype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Projecttype"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projecttype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Projecttype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
             #region Big freaking list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };
        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        public static string getPIusernamebyname(int PIName)
        {

            var context = new IOASDBEntities();
            var query = (from User in context.tblUser
                         where (User.UserId == PIName)
                         select User).FirstOrDefault();
            var username = "";

            if (query != null)
            {
                username = query.UserName;
            }

            return username;

        }

        public static string GetTapalType(int Id)
        {
            try
            {
                string TapalType = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = context.tblCodeControl.FirstOrDefault(m => m.CodeName == "TapalCatagory" && m.CodeValAbbr == Id).CodeValDetail;
                    if (Query != null)
                    {
                        TapalType = Query;
                    }
                }
                return TapalType;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static string GetPIName(int Id, bool appendEmpId = false)
        {
            try
            {
                string PIName = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = context.vwFacultyStaffDetails.FirstOrDefault(m => m.UserId == Id);
                    if (Query != null)
                    {
                        if (appendEmpId)
                            PIName = Query.EmployeeId + "-" + Query.FirstName;
                        else
                            PIName = Query.FirstName;
                    }
                }
                return PIName;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static string GetPIEmail(int Id)
        {
            try
            {
                string PIEmail = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = context.vwFacultyStaffDetails.FirstOrDefault(m => m.UserId == Id);
                    if (Query != null)
                    {
                        PIEmail = Query.Email;
                    }
                }
                return PIEmail;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static List<MasterlistviewModel> GetDepartment()
        {
            try
            {
                List<MasterlistviewModel> Department = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblDepartment orderby C.DepartmentId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Department.Add(new MasterlistviewModel()
                            {
                                id = Query[i].DepartmentId,
                                name = Query[i].DepartmentName
                            });
                        }
                    }
                    return Department;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Department = new List<MasterlistviewModel>();
                return Department;
            }
        }

        public static List<MasterlistviewModel> GetUserList()
        {
            try
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser orderby C.UserId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            UserList.Add(new MasterlistviewModel()
                            {
                                id = Query[i].UserId,
                                name = Query[i].FirstName + ' ' + Query[i].LastName
                            });
                        }
                    }
                    return UserList;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                return UserList;
            }
        }
        //public static List<MasterlistviewModel> GetUserListByDepId(int ID)
        //{
        //    try
        //    {
        //        List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
        //        using (var context = new IOASDBEntities())
        //        {
        //            var Query = (from C in context.tblUser where C.DepartmentId == ID orderby C.UserId ascending select C).ToList();
        //            if (Query.Count > 0)
        //            {
        //                for (int i = 0; i < Query.Count; i++)
        //                {
        //                    UserList.Add(new MasterlistviewModel()
        //                    {
        //                        id = Query[i].UserId,
        //                        name = Query[i].FirstName + ' ' + Query[i].LastName
        //                    });
        //                }
        //            }
        //            return UserList;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
        //        return UserList;
        //    }
        //}

        public static List<MasterlistviewModel> GetTapalCatagory()
        {
            try
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblCodeControl where C.CodeName == "TapalCatagory" orderby C.CodeValDetail ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Catagory.Add(new MasterlistviewModel()
                            {
                                id = Query[i].CodeValAbbr,
                                name = Query[i].CodeValDetail
                            });
                        }
                    }
                    return Catagory;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                return Catagory;
            }
        }

        public static string GetDepartmentById(int TapalId)
        {
            try
            {
                string Department = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblTapalWorkflow where C.TapalId == TapalId && C.Is_Active == false orderby C.TapalWorkflowId descending select C.MarkTo).FirstOrDefault();
                    if (Query != null)
                    {

                        Department = GetDepartmentName(Query ?? 0);

                    }
                    return Department;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetDepartmentName(int Depid)
        {
            try
            {
                string Department = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblDepartment where C.DepartmentId == Depid select C.DepartmentName).FirstOrDefault();
                    if (Query != null)
                    {

                        Department = Query;

                    }
                    return Department;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string GetTapalDepartmentById(int TapalId)
        {
            try
            {
                string Department = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblTapalWorkflow where C.TapalId == TapalId && C.Is_Active == false orderby C.TapalWorkflowId descending select C.MarkTo).FirstOrDefault();
                    if (Query != null)
                    {

                        Department = GetDepartmentName(Query ?? 0);

                    }
                    return Department;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetUserListById(int UserId)
        {
            try
            {
                string User = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser where C.UserId == UserId select C).FirstOrDefault();
                    if (Query != null)
                    {
                        User = Query.FirstName + ' ' + Query.LastName;
                    }
                    return User;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }


        public static Tuple<string, string> getUploadFileName(int TapalDocId, int TapalId)
        {
            string fileName = "";
            string DocName = "";
            using (var context = new IOASDBEntities())
            {
                var file = (from c in context.tblTapal
                            join d in context.tblTapalDocumentDetail on c.TapalId equals d.TapalId
                            where c.TapalId == TapalId && d.TapalDocumentDetailId == TapalDocId
                            select d).FirstOrDefault();
                if (file != null)
                {
                    fileName = file.FileName;
                    DocName = file.DocumentName;
                }
            }
            return Tuple.Create(fileName, DocName);
        }

        public static List<MasterlistviewModel> GetSRBItemCatagory()
        {
            try
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblSRBItemCategory
                                 where C.Status == "Active"
                                 orderby C.Category
                                 select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Catagory.Add(new MasterlistviewModel()
                            {
                                id = Query[i].SRBItemCategotyId,
                                name = Query[i].Category
                            });
                        }
                    }
                    return Catagory;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                return Catagory;
            }
        }
        public static bool CheckIsAsset(int CategoryId)
        {
            try
            {
                bool isAsset = false;
                using (var context = new IOASDBEntities())
                {
                    var Query = context.tblSRBItemCategory.FirstOrDefault(m => m.SRBItemCategotyId == CategoryId);
                    if (Query != null)
                    {
                        isAsset = Query.Asset_f ?? false;
                    }
                    return isAsset;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<MasterlistviewModel> GetInstitute()
        {
            try
            {
                List<MasterlistviewModel> inusmodel = new List<MasterlistviewModel>();
                {
                    using (var context = new IOASDBEntities())
                    {
                        var query = (from I in context.tblInstituteMaster
                                     orderby I.Institutename
                                     select new { I.InstituteId, I.Institutename }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                inusmodel.Add(new MasterlistviewModel()
                                {
                                    id = query[i].InstituteId,
                                    name = query[i].Institutename
                                });
                            }

                        }
                        return inusmodel;
                    }

                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> inusmodel = new List<MasterlistviewModel>();
                return inusmodel;
            }
        }

        public static List<MasterlistviewModel> GetPIByInstitute(int InstituteId)
        {
            try
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from user in context.tblUser
                                 where user.InstituteId == InstituteId && user.Status == "Active"
                                 orderby user.FirstName, user.LastName
                                 select new { user.UserId, user.FirstName, user.LastName, user.EMPCode }).ToList();
                    Catagory.Add(new MasterlistviewModel()
                    {
                        id = null,
                        name = "Select PI"
                    });
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Catagory.Add(new MasterlistviewModel()
                            {
                                id = Query[i].UserId,
                                name = Query[i].EMPCode + "-" + Query[i].FirstName + " " + Query[i].LastName
                            });
                        }
                    }
                    return Catagory;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> name = new List<MasterlistviewModel>();
                return name;
            }
        }
        public static int Gcd(int a, int b)
        {
            if (a == 0)
                return b;
            else
                return Gcd(b % a, a);
        }
        public static string GetRatio(int a, int b)
        {
            int gcd = Gcd(a, b);
            return (a / gcd).ToString() + ":" + (b / gcd).ToString();
        }

        public static List<MasterlistviewModel> GetPIProjects(int PIId)
        {
            try
            {
                List<MasterlistviewModel> Catagory = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from pro in context.tblProject
                                 where pro.PIName == PIId
                                 orderby pro.ProposalNumber
                                 select new { pro.ProjectId, pro.ProjectNumber }).ToList();
                    //Catagory.Add(new MasterlistviewModel()
                    //{
                    //    id = null,
                    //    name = "Select Project"
                    //});
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Catagory.Add(new MasterlistviewModel()
                            {
                                id = Query[i].ProjectId,
                                name = Query[i].ProjectNumber
                            });
                        }
                    }
                    return Catagory;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> name = new List<MasterlistviewModel>();
                return name;
            }
        }

        public static string GetProjectNumber(int ProjectId, bool appendPIName = false)
        {
            try
            {
                string num = "";
                using (var context = new IOASDBEntities())
                {
                    if (!appendPIName)
                    {
                        var PNum = context.tblProject.Where(m => m.ProjectId == ProjectId).FirstOrDefault().ProjectNumber;
                        if (PNum != null)
                            num = PNum;
                    }
                    else
                    {
                        var query = (from P in context.tblProject
                                     join U in context.vwFacultyStaffDetails on P.PIName equals U.UserId
                                     where P.ProjectId == ProjectId
                                     select new
                                     {
                                         P.ProjectNumber,
                                         U.FirstName
                                     }).FirstOrDefault();
                        if (query != null)
                            num = query.ProjectNumber + "-" + query.FirstName;
                    }
                    return num;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static List<MasterlistviewModel> getproposalnumber()
        {
            try
            {

                List<MasterlistviewModel> Proposalnumber = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProposal
                                 where C.Status == "Active"
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Proposalnumber.Add(new MasterlistviewModel()
                            {
                                id = query[i].ProposalId,
                                name = query[i].ProposalNumber,
                            });
                        }
                    }

                }

                return Proposalnumber;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getMinistry()
        {
            try
            {

                List<MasterlistviewModel> ministry = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from minstry in context.tblMinistryMaster
                                 select minstry).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            ministry.Add(new MasterlistviewModel()
                            {
                                id = query[i].MinistryId,
                                name = query[i].MinistryName,

                            });
                        }
                    }


                }

                return ministry;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> getsponprojectsubtype()
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "SponsoredProjectSubtype"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectsubtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> getconsprojectsubtype()
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ConsultancyProjectSubtype"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectsubtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getcategoryofproject()
        {
            try
            {

                List<MasterlistviewModel> projectcategory = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSchemes
                                 orderby C.SchemeId
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            projectcategory.Add(new MasterlistviewModel()
                            {
                                id = query[i].SchemeId,
                                name = query[i].SchemeName,
                            });
                        }
                    }

                }

                return projectcategory;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> getFacultyCadre()
        {
            try
            {

                List<CodeControllistviewModel> facultycadre = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "FacultyCadre"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            facultycadre.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return facultycadre;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> getprojectcategory()
        {
            try
            {

                List<CodeControllistviewModel> projectcategory = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ConsultancyProjectSubtype"
                                 orderby C.CodeValDetail
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            projectcategory.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return projectcategory;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> getallocationhead()
        {
            try
            {

                List<MasterlistviewModel> allocationhead = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblBudgetHead
                                 orderby C.HeadName
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            allocationhead.Add(new MasterlistviewModel()
                            {
                                id = query[i].BudgetHeadId,
                                name = query[i].HeadName,
                                code = query[i].IsRecurring == true ? "Recurring" : "Non-Recurring"
                            });
                        }
                    }

                }

                return allocationhead;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<CodeControllistviewModel> gettaxservice()
        {
            try
            {

                List<CodeControllistviewModel> allocationhead = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ServiceMode"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            allocationhead.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return allocationhead;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetProjecttitledetails()
        {
            try
            {

                List<MasterlistviewModel> Projectdetails = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 join user in context.tblUser on C.PIName equals user.UserId
                                 where !context.tblProjectEnhancementAllocation.Any(m => m.ProjectId == C.ProjectId && m.IsCurrentVersion == true)
                                 select new { C, user.FirstName, user.LastName, user.EMPCode }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectdetails.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.ProjectId,
                                name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "-" + query[i].FirstName + " " + query[i].LastName,
                            });
                        }
                    }
                }

                return Projectdetails;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Projectdetails = new List<MasterlistviewModel>();
                return Projectdetails;
            }

        }

        //public static string getDepartmentCode(string Departmentid)
        //{
        //    try
        //    {
        //        var departmentcode = " ";
        //        var context = new IOASDBEntities();
        //        var Departquery = (from dept in context.tblPIDepartmentMaster
        //                           where dept.DepartmentId == Departmentid
        //                           select dept).FirstOrDefault();

        //        if (Departquery != null)
        //        {
        //            departmentcode = Departquery.DepartmentCode;
        //            return departmentcode;
        //        }
        //        else
        //        {
        //            return departmentcode;
        //        }

        //    }

        //    catch (Exception ex)
        //    {

        //        throw ex;

        //    }
        //}
        public static string getfacultycode(int PIid)
        {
            try
            {
                var facultycode = " ";
                var context = new IOASDBEntities();
                var query = (from user in context.vwFacultyStaffDetails
                             where user.UserId == PIid
                             select user).FirstOrDefault();

                if (query != null)
                {
                    facultycode = query.EmployeeId;
                    return facultycode;
                }
                else
                {
                    return facultycode;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getinvoicenumber(int invoiceid)
        {
            try
            {
                var invoicenumber = " ";
                var context = new IOASDBEntities();
                var query = (from inv in context.tblProjectInvoice
                             where inv.InvoiceId == invoiceid
                             select inv).FirstOrDefault();

                if (query != null)
                {
                    invoicenumber = query.InvoiceNumber;
                    return invoicenumber;
                }
                else
                {
                    return invoicenumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static List<MasterlistviewModel> getservicetype()
        {
            try
            {
                List<MasterlistviewModel> taxtype = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblTaxMaster
                                 orderby C.TaxMasterId
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            taxtype.Add(new MasterlistviewModel()
                            {
                                id = query[i].TaxMasterId,
                                name = query[i].ServiceType
                            });
                        }
                    }
                }
                return taxtype;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string getagencycode(int AgencyID)
        {
            try
            {
                var agncycode = " ";
                var context = new IOASDBEntities();
                var query = (from agncy in context.tblAgencyMaster
                             where agncy.AgencyId == AgencyID
                             select agncy).FirstOrDefault();

                if (query != null)
                {
                    agncycode = query.AgencyCode;
                    return agncycode;
                }
                else
                {
                    return agncycode;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getprojectnumber(int projectid)
        {
            try
            {
                var projectnumber = " ";
                var context = new IOASDBEntities();
                var query = (from proj in context.tblProject
                             where proj.ProjectId == projectid
                             select proj).FirstOrDefault();

                if (query != null)
                {
                    projectnumber = query.ProjectNumber;
                    return projectnumber;
                }
                else
                {
                    return projectnumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static int GetProjectType(int projectid)
        {
            try
            {
                int type = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from proj in context.tblProject
                                 where proj.ProjectId == projectid
                                 select proj).FirstOrDefault();

                    if (query != null)
                    {
                        type = query.ProjectType ?? 0;
                    }
                }
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int GetProjectSequenceNumber(int financialyear)
        {
            try
            {
                var lastseqnum = 0;
                var context = new IOASDBEntities();
                var Projectquery = (from project in context.tblProject
                                    where project.FinancialYear == financialyear
                                    select project.SequenceNumber).Max();

                int seqnum = Projectquery ?? 0;
                lastseqnum = seqnum + 1;
                return lastseqnum;

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string GetDefaultRoleName(int UserId)
        {
            try
            {
                string _roles = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblUser
                                 from R in context.tblRole
                                 where (U.UserId == UserId && U.RoleId == R.RoleId)
                                 select R.RoleName).FirstOrDefault();
                    if (query != null)
                    {
                        _roles = query;
                    }

                }
                return _roles;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetRoles(int UserId)
        {
            try
            {
                string _roles = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = (from ur in context.tblUserRole
                                 join r in context.tblRole on ur.RoleId equals r.RoleId
                                 where ur.UserId == UserId
                                 select r.RoleName).ToArray();
                    var defaultRole = GetDefaultRoleName(UserId);
                    if (query.Count() > 0)
                    {
                        _roles = string.Join(",", query);
                        _roles = _roles + "," + defaultRole;
                    }
                    else
                    {
                        return defaultRole;
                    }

                }
                return _roles;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetUserFirstName(string Username)
        {
            try
            {
                string _fName = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUser.FirstOrDefault(m => m.UserName == Username && m.Status == "Active");
                    if (query != null)
                    {
                        _fName = query.FirstName + " " + query.LastName;
                    }

                }
                return _fName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetUserFirstName(int UserId)
        {
            try
            {
                string _fName = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblUser.FirstOrDefault(m => m.UserId == UserId);
                    if (query != null)
                    {
                        _fName = query.FirstName + " " + query.LastName;
                    }

                }
                return _fName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetLoginTS(int UserId)
        {
            try
            {
                string _ts = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblLoginDetails.Where(m => m.UserId == UserId).OrderByDescending(m => m.LoginDetailId).FirstOrDefault();
                    if (query != null)
                    {
                        _ts = String.Format("{0:ddd dd-MM-yy h:mm tt}", query.LoginTime);
                    }

                }
                return _ts;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static List<NotificationModel> GetNotification(int logged_in_userId)
        {
            try
            {
                List<NotificationModel> list = new List<NotificationModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from notify in context.tblNotification
                                 join user in context.tblUser on notify.FromUserId equals user.UserId
                                 where notify.ToUserId == logged_in_userId && notify.IsDeleted != true
                                 orderby notify.NotificationId descending
                                 select new { notify, user.FirstName, user.LastName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new NotificationModel()
                            {
                                FromUserName = query[i].FirstName + " " + query[i].LastName,
                                FunctionURL = query[i].notify.FunctionURL,
                                NotificationDateTime = String.Format("{0:ddd dd-MM-yy h:mm tt}", query[i].notify.Crt_Ts),
                                NotificationId = query[i].notify.NotificationId,
                                NotificationType = query[i].notify.NotificationType,
                                ReferenceId = query[i].notify.ReferenceId
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<NotificationModel>();
            }

        }
        public static string getproposalnumber(int proposalid)
        {
            try
            {
                var proposalnumber = " ";
                var context = new IOASDBEntities();
                var query = (from propsl in context.tblProposal
                             where propsl.ProposalId == proposalid
                             select propsl).FirstOrDefault();

                if (query != null)
                {
                    proposalnumber = query.ProposalNumber;
                    return proposalnumber;
                }
                else
                {
                    return proposalnumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static string GetProjectStatus(int Id)
        {
            try
            {
                string Status = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = context.tblCodeControl.FirstOrDefault(m => m.CodeName == "ProjectStatus" && m.CodeValAbbr == Id);
                    if (Query != null)
                    {
                        Status = Query.CodeValDetail;
                    }
                }
                return Status;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static string getconsprjctype(int ConsprojectSubtype)
        {
            try
            {
                var fundingcategory = " ";
                var context = new IOASDBEntities();
                var query = (from cc in context.tblSchemes
                             where cc.SchemeId == ConsprojectSubtype
                             select cc).FirstOrDefault();

                if (query != null)
                {
                    fundingcategory = query.SchemeName;

                }
                return fundingcategory;
            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<CodeControllistviewModel> getprojectsubtype()
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectSubtype"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectsubtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getconsfundingcategory()
        {
            try
            {

                List<CodeControllistviewModel> fundingcategory = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSchemes
                                 where C.ProjectType == 2
                                 orderby C.SchemeId
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundingcategory.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].SchemeName,
                                codevalAbbr = query[i].SchemeId,
                                CodeValDetail = query[i].SchemeName
                            });
                        }
                    }

                }

                return fundingcategory;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> GetSourceList()
        {
            try
            {

                List<CodeControllistviewModel> list = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Source"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<CodeControllistviewModel>();
            }

        }
        public static List<CodeControllistviewModel> getfundingtype()
        {
            try
            {

                List<CodeControllistviewModel> fundingtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectFundingType"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundingtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundingtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> GetFundingTypeWOBoth()
        {
            try
            {

                List<CodeControllistviewModel> fundingtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectFundingType" && C.CodeValAbbr != 3
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundingtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundingtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getfundedby()
        {
            try
            {

                List<CodeControllistviewModel> fundedby = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectFundedBy"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundedby.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundedby;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getindfundinggovtbody()
        {
            try
            {

                List<CodeControllistviewModel> fundedby = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Indfundgovtbody"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundedby.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundedby;
            }
            catch (Exception ex)
            {
                List<CodeControllistviewModel> fundedby = new List<CodeControllistviewModel>();
                return fundedby;

            }
        }
        public static List<MasterlistviewModel> GetAccounttype()
        {
            try
            {
                List<MasterlistviewModel> acct = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from A in context.tblAccountType
                                 orderby A.AccountType
                                 select A).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            acct.Add(new MasterlistviewModel()
                            {

                                id = query[i].AccountTypeId,
                                name = query[i].AccountType,
                                code = query[i].AccountTypeCode
                            });
                        }
                    }
                    return acct;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> acct = new List<MasterlistviewModel>();
                return acct;
            }


        }
        public static List<MasterlistviewModel> Parentaccountgroup()
        {
            try
            {
                List<MasterlistviewModel> PAGlist = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from PG in context.tblAccountGroup
                                 where (PG.Status == "Active")
                                 orderby PG.AccountGroup
                                 select new { PG.AccountGroupId, PG.AccountGroup, PG.AccountGroupCode }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PAGlist.Add(new MasterlistviewModel()
                            {
                                id = query[i].AccountGroupId,
                                name = query[i].AccountGroup + '-' + query[i].AccountGroupCode
                            });
                        }
                    }
                }
                return PAGlist;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> PAGlist = new List<MasterlistviewModel>();
                return PAGlist;
            }
        }
        public static List<CodeControllistviewModel> getindfundingnongovtbody()
        {
            try
            {

                List<CodeControllistviewModel> fundedby = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Indfundnongovtbody"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundedby.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundedby;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getforgnfundinggovtbody()
        {
            try
            {

                List<CodeControllistviewModel> fundedby = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Fornfundgovtbody"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundedby.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundedby;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public static List<CodeControllistviewModel> getforgnfundingnongovtbody()
        //{
        //    try
        //    {
        public static List<CodeControllistviewModel> getforgnfundingnongovtbody()
        {
            try
            {

                List<CodeControllistviewModel> fundedby = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Fornfundnongovtbody"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            fundedby.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return fundedby;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetAllRoles()
        {
            try
            {
                List<MasterlistviewModel> Role = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblRole where C.Status == "Active" orderby C.RoleName ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            Role.Add(new MasterlistviewModel()
                            {
                                id = Query[i].RoleId,
                                name = Query[i].RoleName
                            });
                        }
                    }
                    return Role;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> Role = new List<MasterlistviewModel>();
                return Role;
            }
        }
        public static List<MasterlistviewModel> GetRoleListByDepId(int ID)
        {
            try
            {
                List<MasterlistviewModel> RoleList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblRole where C.DepartmentId == ID && C.DepartmentId != 0 && C.Status == "Active" orderby C.DepartmentId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            RoleList.Add(new MasterlistviewModel()
                            {
                                id = Query[i].RoleId,
                                name = Query[i].RoleName
                            });
                        }
                    }
                    return RoleList;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                return UserList;
            }
        }
        public static List<MasterlistviewModel> GetUserListByRoleId(int ID)
        {
            try
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser where C.RoleId == ID orderby C.UserId ascending select C).ToList();
                    if (Query.Count > 0)
                    {
                        for (int i = 0; i < Query.Count; i++)
                        {
                            UserList.Add(new MasterlistviewModel()
                            {
                                id = Query[i].UserId,
                                name = Query[i].FirstName + ' ' + Query[i].LastName
                            });
                        }
                    }
                    return UserList;
                }
            }
            catch (Exception)
            {
                List<MasterlistviewModel> UserList = new List<MasterlistviewModel>();
                return UserList;
            }
        }

        public static List<MasterlistviewModel> GetTapalAction(int UserID)
        {
            try
            {
                List<MasterlistviewModel> TapalAction = new List<MasterlistviewModel>();
                List<MasterlistviewModel> result = new List<MasterlistviewModel>();
                var AccessRole = GetRoleAccess(UserID, 14);

                using (var context = new IOASDBEntities())
                {

                    if (AccessRole.Count > 0)
                    {
                        var AccessIsApprove = AccessRole.Select(m => m.IsApprove).Distinct().ToArray();
                        for (int i = 0; i < AccessIsApprove.Length; i++)
                        {
                            if (AccessIsApprove[i] == true)
                            {
                                var QryAction = (from C in context.tblCodeControl where C.CodeName == "TapalAction" && C.CodeValAbbr != 0 select C).ToList();
                                if (QryAction.Count > 0)
                                {
                                    for (int j = 0; j < QryAction.Count; j++)
                                    {
                                        TapalAction.Add(new MasterlistviewModel()
                                        {
                                            id = QryAction[j].CodeValAbbr,
                                            name = QryAction[j].CodeValDetail
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var QryAction = (from C in context.tblCodeControl where C.CodeName == "TapalAction" && C.CodeValAbbr != 0 && C.CodeValAbbr != 3 && C.CodeValAbbr != 4 select C).ToList();
                                if (QryAction.Count > 0)
                                {
                                    for (int k = 0; k < QryAction.Count; k++)
                                    {
                                        TapalAction.Add(new MasterlistviewModel()
                                        {
                                            id = QryAction[k].CodeValAbbr,
                                            name = QryAction[k].CodeValDetail
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                var tapal = (from c in TapalAction
                             from d in TapalAction //on c.id equals d.id
                             orderby c.id ascending
                             group c by c.id into g
                             select new
                             {
                                 id = g.Key,
                                 name = g.Select(m => m.name).FirstOrDefault()
                             }).ToList();

                if (tapal.Count > 0)
                {
                    for (int k = 0; k < tapal.Count; k++)
                    {
                        result.Add(new MasterlistviewModel()
                        {
                            id = tapal[k].id,
                            name = tapal[k].name
                        });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> TapalAction = new List<MasterlistviewModel>();
                return TapalAction;
            }
        }

        public static string GetTapalActionById(int ActionId)
        {
            try
            {
                string TapalAction = "";
                using (var context = new IOASDBEntities())
                {
                    var QryAction = (from C in context.tblCodeControl where C.CodeName == "TapalAction" && C.CodeValAbbr == ActionId select C.CodeValDetail).FirstOrDefault();
                    if (QryAction != null)
                    {
                        TapalAction = QryAction;
                    }
                }
                return TapalAction;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string getTapalNo()
        {
            try
            {
                var tapalNo = "";
                using (var context = new IOASDBEntities())
                {
                    var Qry = (from C in context.tblTapal select C).Count();
                    var FinancialYear = context.tblFinYear.FirstOrDefault(m => m.CurrentYearFlag == true).Year;
                    if (Qry > 0)
                    {
                        //var value = lastproposalnumber.Split('_').Last();
                        //string number = Regex.Replace(value, @"\D", "");
                        //var seqnum = Convert.ToInt32(number);
                        //sequencenumber = seqnum + 1;
                        tapalNo = "TPL/" + FinancialYear + (Qry + 1).ToString("000000");
                    }
                    else
                    {
                        tapalNo = "TPL/" + FinancialYear + "/000001";
                    }
                }

                return tapalNo;
            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string GetTapalNo(int id)
        {
            try
            {
                var tapalNo = "";
                using (var context = new IOASDBEntities())
                {
                    tapalNo = context.tblTapal.FirstOrDefault(m => m.TapalId == id).TapalNo;                    
                }

                return tapalNo;
            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string GetSRBNumber(int SRBId)
        {
            try
            {
                var no = "SRB";
                using (var context = new IOASDBEntities())
                {
                    var query = (from cc in context.tblSRB
                                 where cc.SRBId == SRBId
                                 select cc).FirstOrDefault();

                    if (query != null)
                        no = query.SRBNumber;
                }
                return no;
            }
            catch (Exception ex)
            {
                return "SRB";
            }
        }
        public static List<CodeControllistviewModel> gettypeofproject()
        {
            try
            {

                List<CodeControllistviewModel> typeofproject = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "TypeofProject"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            typeofproject.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return typeofproject;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getsponprojectcategory()
        {
            try
            {

                List<CodeControllistviewModel> Projectsubtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "SponProjectCategory"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectsubtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Projectsubtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getconstaxtype()
        {
            try
            {

                List<CodeControllistviewModel> Taxtype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ConsProjecttaxtype"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Taxtype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Taxtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> gettaxregstatus()
        {
            try
            {

                List<CodeControllistviewModel> Taxregstatus = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "TaxServiceRegStatus"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Taxregstatus.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Taxregstatus;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static int GetRoleIdByUserId(int UserId)
        {

            var context = new IOASDBEntities();
            var query = (from U in context.tblUser
                         where (U.UserId == UserId && U.Status == "Active")
                         select U).FirstOrDefault();
            var userRoleId = 0;
            if (query != null)
            {
                userRoleId = (Int32)query.RoleId;
            }


            return userRoleId;

        }
        //Patent by priya
        public static string GetRoleByUserName(string name)
        {
            using(var context = new IOASDBEntities())
            {
                var query = (from U in context.tblUser
                             join r in context.tblRole on U.RoleId equals r.RoleId
                             where (U.UserName == name && U.Status == "Active")
                             select r).FirstOrDefault();
                var userRole = string.Empty;
                if (query != null)
                {
                    userRole = query.RoleName;
                }

                return userRole;

            }
        }
        public static List<MasterlistviewModel> Getmonth()
        {
            try
            {
                List<MasterlistviewModel> getlistmonth = new List<MasterlistviewModel>();
                int Month = 12;
                for (int i = 1; i <= Month; i++)
                {
                    getlistmonth.Add(new MasterlistviewModel()
                    {

                        id = i,
                        name = Convert.ToString(i)
                    });

                }
                return getlistmonth;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> getlistmonth = new List<MasterlistviewModel>();
                return getlistmonth;
            }
        }

        public static List<MasterlistviewModel> Getyear()
        {
            try
            {
                List<MasterlistviewModel> getlistyear = new List<MasterlistviewModel>();
                int year = DateTime.Now.Year;
                for (int i = 2000; i <= year; i++)
                {
                    getlistyear.Add(new MasterlistviewModel()
                    {

                        id = i,
                        name = Convert.ToString(i)
                    });

                }
                return getlistyear;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> getlistyear = new List<MasterlistviewModel>();
                return getlistyear;
            }
        }
        public static List<MasterlistviewModel> Getreport()
        {
            try
            {
                List<MasterlistviewModel> Getlist = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from M in context.tblCodeControl
                                 where (M.CodeName == "ReportGroup")
                                 orderby M.CodeValDetail
                                 select new { M.CodeValAbbr, M.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Getlist.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }
                }
                return Getlist;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Getlist = new List<MasterlistviewModel>();
                return Getlist;
            }

        }

        public static List<MasterlistviewModel> getinternalfundingagency()
        {
            try
            {

                List<MasterlistviewModel> internalfundingagency = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblAgencyMaster
                                 where C.AgencyType == 1 && C.Status == "Active"
                                 orderby C.AgencyId
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            internalfundingagency.Add(new MasterlistviewModel()
                            {
                                id = query[i].AgencyId,
                                name = query[i].AgencyName,

                            });
                        }
                    }

                }

                return internalfundingagency;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        //public static List<MasterlistviewModel> getProposalDocType()
        //{
        //    try
        //    {

        //        List<MasterlistviewModel> Doctype = new List<MasterlistviewModel>();

        //        using (var context = new IOASDBEntities())
        //        {
        //            var query = (from C in context.tblFunctionDocument
        //                         where C.FunctionId == 9
        //                         orderby C.FunctionDocumentId
        //                         select C).ToList();


        //            if (query.Count > 0)
        //            {
        //                for (int i = 0; i < query.Count; i++)
        //                {
        //                    var docid = query[i].DocumentId;
        //                    var docquery = (from D in context.tblDocument
        //                                    where D.DocumentId == docid
        //                                    select D).FirstOrDefault();
        //                    Doctype.Add(new MasterlistviewModel()
        //                    {
        //                        id = docquery.DocumentId,
        //                        name = docquery.DocumentName,
        //                    });
        //                }
        //            }

        //        }

        //        return Doctype;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}


        public static List<MasterlistviewModel> getstaffcategory()
        {
            try
            {

                List<MasterlistviewModel> staffcategory = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProjectStaffCategoryMaster
                                 orderby C.ProjectStaffCategoryId
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            staffcategory.Add(new MasterlistviewModel()
                            {
                                id = query[i].ProjectStaffCategoryId,
                                name = query[i].ProjectStaffCategory,

                            });
                        }
                    }

                }

                return staffcategory;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<RoleAccessDetailModel> GetRoleAccess(int UserId, int FunctionId)
        {
            try
            {
                List<RoleAccessDetailModel> Detail = new List<RoleAccessDetailModel>();
                int RoleID = GetRoleIdByUserId(UserId);
                using (var context = new IOASDBEntities())
                {
                    var addtionalroles = (from R in context.tblUserRole
                                          where R.UserId == UserId
                                          select R.RoleId).ToArray();
                    var defaultrole = (from R in context.tblUser
                                       where (R.UserId == UserId)
                                       select R.RoleId).FirstOrDefault();

                    var roles = (from RA in context.tblRoleaccess
                                 where (addtionalroles.Contains(RA.RoleId) || RA.RoleId == defaultrole) && RA.FunctionId == FunctionId
                                 select new { RA.RoleId, RA.Read_f, RA.Add_f, RA.Approve_f, RA.Update_f, RA.Delete_f }).Distinct().ToList();
                    if (roles.Count > 0)
                    {
                        for (int i = 0; i < roles.Count; i++)
                        {
                            Detail.Add(new RoleAccessDetailModel()
                            {
                                RoleId = roles[i].RoleId ?? 0,
                                IsAdd = roles[i].Add_f ?? false,
                                IsRead = roles[i].Read_f ?? false,
                                IsApprove = roles[i].Approve_f ?? false,
                                IsDelete = roles[i].Delete_f ?? false,
                                IsUpdate = roles[i].Update_f ?? false,
                            });
                        }
                    }
                }
                return Detail;
            }
            catch (Exception ex)
            {
                List<RoleAccessDetailModel> Detail = new List<RoleAccessDetailModel>();
                return Detail;
            }
        }
        public static List<MasterlistviewModel> GetStatelist()
        {
            try
            {
                List<MasterlistviewModel> Statelist = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from S in context.tblStateMaster
                                 orderby S.StateName
                                 select new { S.StateId, S.StateName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Statelist.Add(new MasterlistviewModel()
                            {
                                id = query[i].StateId,
                                name = query[i].StateName
                            });

                        }
                    }
                    return Statelist;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Statelist = new List<MasterlistviewModel>();
                return Statelist;
            }
        }
        public static List<MasterlistviewModel> GetAgencyType()
        {
            try
            {
                List<MasterlistviewModel> agencytype = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from AT in context.tblCodeControl
                                 where (AT.CodeName == "AgencyCountry")
                                 select new { AT.CodeValAbbr, AT.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            agencytype.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }
                }
                return agencytype;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> agencytype = new List<MasterlistviewModel>();
                return agencytype;
            }
        }
        public static List<MasterlistviewModel> GetIndianAgencyCategory()
        {
            try
            {
                List<MasterlistviewModel> indainagency = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from IC in context.tblCodeControl
                                 where IC.CodeName == "IndianAgencyCategory"
                                 select new { IC.CodeValAbbr, IC.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            indainagency.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }
                return indainagency;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> indainagency = new List<MasterlistviewModel>();
                return indainagency;
            }
        }
        public static List<MasterlistviewModel> GetNonSEZCategory()
        {
            try
            {
                List<MasterlistviewModel> NONSEZCategory = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from NS in context.tblCodeControl
                                 where (NS.CodeName == "NonSEZCategory")
                                 select new { NS.CodeValAbbr, NS.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            NONSEZCategory.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }
                }
                return NONSEZCategory;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> NONSEZCategory = new List<MasterlistviewModel>();
                return NONSEZCategory;
            }
        }
        public static List<MasterlistviewModel> GetAccountGroup(bool? isBank = null)
        {
            try
            {
                List<MasterlistviewModel> Acctlist = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from A in context.tblAccountGroup
                                 where A.Status == "Active" && (isBank == null || A.Bank_f == isBank)
                                 orderby A.AccountGroup
                                 select new { A.AccountGroupId, A.AccountGroup, A.AccountGroupCode }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Acctlist.Add(new MasterlistviewModel()
                            {

                                id = query[i].AccountGroupId,
                                name = query[i].AccountGroup + '-' + query[i].AccountGroupCode
                            });
                        }
                    }
                    return Acctlist;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Acctlist = new List<MasterlistviewModel>();
                return Acctlist;
            }
        }
        public static List<MasterlistviewModel> GetAccountGroup(int groupId)
        {
            try
            {
                List<MasterlistviewModel> Acctlist = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    Acctlist = (from A in context.tblAccountGroup
                                orderby A.AccountGroup
                                where A.AccountGroupId == groupId
                                select new MasterlistviewModel()
                                { id = A.AccountGroupId, name = A.AccountGroup }).ToList();

                    return Acctlist;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Acctlist = new List<MasterlistviewModel>();
                return Acctlist;
            }
        }
        public static Int32 GetAccountGroupId(string groupName)
        {
            try
            {
                Int32 gId = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAccountGroup.FirstOrDefault(m => m.AccountGroup == groupName);
                    if (query != null)
                        gId = query.AccountGroupId;
                    return gId;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        //public static List<MasterlistviewModel> GetAgencyDocument()
        //{

        //    try
        //    {
        //        List<MasterlistviewModel> Agencydoc = new List<MasterlistviewModel>();
        //        using (var context = new IOASDBEntities())
        //        {
        //            var documentid = (from FD in context.tblFunctionDocument
        //                              where (FD.FunctionId == 19)
        //                              select FD.DocumentId).ToArray();
        //            if (documentid != null)
        //            {
        //                var query = (from D in context.tblDocument
        //                             where documentid.Contains(D.DocumentId)
        //                             select new { D.DocumentId, D.DocumentName }).ToList();
        //                if (query.Count > 0)
        //                {
        //                    for (int i = 0; i < query.Count; i++)
        //                    {
        //                        Agencydoc.Add(new MasterlistviewModel()
        //                        {

        //                            id = query[i].DocumentId,
        //                            name = query[i].DocumentName
        //                        });
        //                    }
        //                }
        //            }
        //            return Agencydoc;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        List<MasterlistviewModel> Agencydoc = new List<MasterlistviewModel>();
        //        return Agencydoc;
        //    }
        //}

        public static string GetUserEmail(int Id)
        {
            try
            {
                string email = "";
                using (var context = new IOASDBEntities())
                {
                    var Query = context.tblUser.FirstOrDefault(m => m.UserId == Id);
                    if (Query != null)
                    {
                        email = Query.Email;
                    }
                }
                return email;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public static string GetPIDesignation(int PIid)
        {
            try
            {
                var des = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = (from user in context.vwFacultyStaffDetails
                                 where user.UserId == PIid //&& user.RoleId == 7
                                 select user).FirstOrDefault();

                    if (query != null)
                        des = query.Designation;

                    return des;
                }
            }
            catch (Exception ex)
            {
                return "";

            }
        }

        public static List<MasterlistviewModel> GetFinYearList()
        {
            try
            {

                List<MasterlistviewModel> finYear = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblFinYear
                                 orderby C.Year descending
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            finYear.Add(new MasterlistviewModel()
                            {
                                id = query[i].FinYearId,
                                name = query[i].Year,
                            });
                        }
                    }
                }
                return finYear;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> finYear = new List<MasterlistviewModel>();
                return finYear;
            }

        }

        public static string GetFinYear(int finId)
        {
            try
            {

                string finYear = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var query = context.tblFinYear.FirstOrDefault(m => m.FinYearId == finId);


                    if (query != null)
                        finYear = query.Year;
                }
                return finYear;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        public static List<MasterlistviewModel> GetWorkflowRefNumberList()
        {
            try
            {

                List<MasterlistviewModel> listWF = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    listWF.Add(new MasterlistviewModel()
                    {
                        id = null,
                        name = "Select Ref. Number",
                    });
                }
                return listWF;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> listWF = new List<MasterlistviewModel>();
                return listWF;
            }

        }
        public static List<AutoCompleteModel> GetAutoCompleteWorkflowRefNumberList()
        {
            try
            {

                List<AutoCompleteModel> listWF = new List<AutoCompleteModel>();

                
                return listWF;
            }
            catch (Exception ex)
            {
                List<AutoCompleteModel> listWF = new List<AutoCompleteModel>();
                return listWF;
            }

        }
        public static int GetDepartmentId(string username)
        {
            try
            {
                int Department = 0;
                using (var context = new IOASDBEntities())
                {
                    var Query = (from C in context.tblUser where C.UserName == username select C.DepartmentId).FirstOrDefault();
                    if (Query != null)
                        Department = Query ?? 0;
                    return Department;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static List<MasterlistviewModel> GetTapalRefNumberList(int depId)
        {
            try
            {

                List<MasterlistviewModel> listRefNum = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblTapal
                                 join wf in context.tblTapalWorkflow on C.TapalId equals wf.TapalId
                                 orderby C.TapalId descending
                                 where wf.MarkTo == depId && C.IsClosed == true
                                 group C by C.TapalId into g
                                 select new
                                 {
                                     TapalId = g.Key,
                                     TapalNo = g.Select(m => m.TapalNo).FirstOrDefault()
                                 }).ToList();
                    listRefNum.Add(new MasterlistviewModel()
                    {
                        id = null,
                        name = "Select Ref. Number",
                    });
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            listRefNum.Add(new MasterlistviewModel()
                            {
                                id = query[i].TapalId,
                                name = query[i].TapalNo,
                            });
                        }
                    }
                }
                return listRefNum;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> listRefNum = new List<MasterlistviewModel>();
                return listRefNum;
            }

        }
        public static List<AutoCompleteModel> GetAutoCompleteTapalRefNumberList(string term, int depId)
        {
            try
            {

                List<AutoCompleteModel> stud = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    stud = (from C in context.tblTapal
                            join wf in context.tblTapalWorkflow on C.TapalId equals wf.TapalId
                            orderby C.TapalId descending
                            where wf.MarkTo == depId && C.IsClosed == true
                            && C.TapalNo.Contains(term)
                            group C by C.TapalId into g
                            select new AutoCompleteModel
                            {
                                value = g.Key.ToString(),
                                label = g.Select(m => m.TapalNo).FirstOrDefault()
                            }).ToList();
                    
                }

                return stud;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static List<MasterlistviewModel> GetSponsoredSchemeCodeList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSponsoredSchemes
                                 orderby C.SponsoredSchemesId descending
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].SponsoredSchemesId,
                                name = query[i].SchemeCode,
                            });
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> list = new List<MasterlistviewModel>();
                return list;
            }

        }
        public static string ValidateEnhancementAndExtension(ProjectEnhancementModel model)
        {
            try
            {
                string msg = "Valid";
                using (var context = new IOASDBEntities())
                {
                    if (model.Extension_Qust_1 == "Yes")
                    {
                        var query = context.tblProject.FirstOrDefault(m => m.ProjectId == model.ProjectID && m.Status == "Active" && m.IsSubProject == true);
                        if (query != null)
                        {

                            msg = "You can't do the extension for subproject";
                        }
                    }
                    if (model.Enhancement_Qust_1 == "Yes")
                    {
                        var query = context.tblProject.Any(m => m.MainProjectId == model.ProjectID && m.Status == "Active");
                        if (query)
                        {

                            msg = "You can't do the enhancement for  main project";
                        }
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                return "Something went wrong please contact administrator";
            }
        }
        public static decimal UpdateSanctionValue(int projectId, bool isUpdate = true)
        {

            decimal amt = 0, mainPrjAmt = 0;
            using (var context = new IOASDBEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var Query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId);
                        if (Query != null)
                        {
                            if (Query.IsSubProject == true)
                            {
                                context.tblProjectEnhancement.Where(p => p.ProjectId == projectId && p.Status == "Active")
                                  .ToList()
                                  .ForEach(m =>
                                  {
                                      amt += (m.EnhancedSanctionValue ?? 0);
                                  });

                                if (isUpdate == true)
                                {
                                    amt += Query.BaseValue ?? 0;
                                    Query.SanctionValue = amt;
                                    context.SaveChanges();

                                    int mainProjectId = Query.MainProjectId ?? 0;
                                    (from p in context.tblProject
                                     join enh in context.tblProjectEnhancement on p.ProjectId equals enh.ProjectId
                                     where (p.MainProjectId == mainProjectId && p.Status == "Active" && enh.Status == "Active")
                                     select enh)
                                           .ToList()
                                           .ForEach(m =>
                                           {
                                               mainPrjAmt += (m.EnhancedSanctionValue ?? 0);
                                           });
                                    context.tblProjectEnhancement.Where(p => p.ProjectId == mainProjectId && p.Status == "Active")
                                      .ToList()
                                      .ForEach(m =>
                                      {
                                          mainPrjAmt += (m.EnhancedSanctionValue ?? 0);
                                      });
                                    var mainPrjQuery = context.tblProject.FirstOrDefault(m => m.ProjectId == mainProjectId);
                                    if (mainPrjQuery != null)
                                    {
                                        mainPrjAmt += Query.BaseValue ?? 0;
                                        Query.SanctionValue = amt;
                                        context.SaveChanges();
                                    }
                                }
                                transaction.Commit();
                                return amt;
                            }
                            else
                            {
                                (from p in context.tblProject
                                 join enh in context.tblProjectEnhancement on p.ProjectId equals enh.ProjectId
                                 where (p.MainProjectId == projectId && p.Status == "Active" && enh.Status == "Active")
                                 select enh)
                                       .ToList()
                                       .ForEach(m =>
                                       {
                                           amt += (m.EnhancedSanctionValue ?? 0);
                                       });
                                context.tblProjectEnhancement.Where(p => p.ProjectId == projectId && p.Status == "Active")
                                  .ToList()
                                  .ForEach(m =>
                                  {
                                      amt += (m.EnhancedSanctionValue ?? 0);
                                  });
                                if (isUpdate == true)
                                {
                                    amt += Query.BaseValue ?? 0;
                                    Query.SanctionValue = amt;
                                    context.SaveChanges();
                                }
                                transaction.Commit();
                                return amt;
                            }
                        }
                        else
                            throw new Exception();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public static decimal UpdateSanctionValue1(int projectId, bool isUpdate = true)
        {
            try
            {
                decimal amt = 0;
                using (var context = new IOASDBEntities())
                {
                    context.tblProjectEnhancement.Where(p => p.ProjectId == projectId && p.Status == "Active")
                            .ToList()
                            .ForEach(m =>
                            {
                                amt += (m.EnhancedSanctionValue ?? 0);
                            });

                    if (isUpdate == true)
                    {
                        var Query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId);
                        if (Query != null)
                        {
                            amt += Query.BaseValue ?? 0;

                            Query.SanctionValue = amt;
                            context.SaveChanges();
                        }
                    }

                }

                return amt;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static Nullable<DateTime> GetProjectDueDate(int projectId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var Query = context.tblProject.FirstOrDefault(m => m.ProjectId == projectId && m.IsSubProject == true);
                    if (Query != null)
                    {
                        int mainProjectId = Query.MainProjectId ?? 0;
                        var enhanQuery = context.tblProjectEnhancement.Where(m => m.ProjectId == mainProjectId && m.Status == "Active" && m.ExtendedDueDate != null).OrderByDescending(m => m.ProjectEnhancementId).FirstOrDefault();
                        if (enhanQuery != null)
                        {
                            return enhanQuery.ExtendedDueDate;
                        }

                    }
                    else
                    {
                        var enhanQuery = context.tblProjectEnhancement.Where(m => m.ProjectId == projectId && m.Status == "Active" && m.ExtendedDueDate != null).OrderByDescending(m => m.ProjectEnhancementId).FirstOrDefault();
                        if (enhanQuery != null)
                        {
                            return enhanQuery.ExtendedDueDate;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static Tuple<decimal, decimal> GetMainAndSubProjectValues(int mainPId, int projectId)
        {
            try
            {
                decimal subProjectTtl = 0, mainProjectValue = 0;
                using (var context = new IOASDBEntities())
                {
                    context.tblProject.Where(p => p.MainProjectId == mainPId && p.IsSubProject == true && p.Status == "Active" && p.ProjectId != projectId)
                            .ToList()
                            .ForEach(m =>
                            {
                                subProjectTtl += (m.SanctionValue ?? 0);
                            });
                    var Query = context.tblProject.FirstOrDefault(m => m.ProjectId == mainPId);
                    if (Query != null)
                    {
                        mainProjectValue = Query.SanctionValue ?? 0;
                    }
                }

                return Tuple.Create(mainProjectValue, subProjectTtl);
            }
            catch (Exception ex)
            {
                return Tuple.Create((Decimal)0, (Decimal)0);
            }
        }

        public static List<MasterlistviewModel> getCommitmentType()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryCommitment = (from C in context.tblCodeControl where C.CodeName == "CommitmentType" select C).ToList();
                    if (qryCommitment.Count > 0)
                    {
                        for (int i = 0; i < qryCommitment.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryCommitment[i].CodeValAbbr,
                                name = qryCommitment[i].CodeValDetail
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        public static List<MasterlistviewModel> getBudgetHead()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryBudgetHead = (from C in context.tblBudgetHead select C).ToList();
                    if (qryBudgetHead.Count > 0)
                    {
                        for (int i = 0; i < qryBudgetHead.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryBudgetHead[i].BudgetHeadId,
                                name = qryBudgetHead[i].HeadName
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        public static List<MasterlistviewModel> getCurrency(bool exceptINR = false)
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryCurrency = (from C in context.tblCurrency
                                       where exceptINR == false || (exceptINR && C.CurrencyID != 44)
                                       select C
                                       ).ToList();
                    if (qryCurrency.Count > 0)
                    {
                        for (int i = 0; i < qryCurrency.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryCurrency[i].CurrencyID,
                                name = qryCurrency[i].ISOCode
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        public static List<MasterlistviewModel> getAccountGroup()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryAccountGroup = (from C in context.tblAccountGroup select C).ToList();
                    if (qryAccountGroup.Count > 0)
                    {
                        for (int i = 0; i < qryAccountGroup.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryAccountGroup[i].AccountGroupId,
                                name = qryAccountGroup[i].AccountGroup
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        public static List<MasterlistviewModel> GetBankAccountGroup()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    List = (from C in context.tblAccountGroup
                            where C.Bank_f == true
                            orderby C.AccountGroup
                            select new MasterlistviewModel()
                            {
                                id = C.AccountGroupId,
                                name = C.AccountGroup
                            }).ToList();
                }
                return List;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }
        }

        public static List<MasterlistviewModel> getProjectNumber()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryProjectNo = (from C in context.tblProject select new { C.ProjectId, C.ProjectNumber }).ToList();
                    if (qryProjectNo.Count > 0)
                    {
                        for (int i = 0; i < qryProjectNo.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryProjectNo[i].ProjectId,
                                name = qryProjectNo[i].ProjectNumber
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        public static List<MasterlistviewModel> getVendor()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryProjectNo = (from C in context.tblVendorMaster select new { C.VendorId, C.Name }).ToList();
                    if (qryProjectNo.Count > 0)
                    {
                        for (int i = 0; i < qryProjectNo.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryProjectNo[i].VendorId,
                                name = qryProjectNo[i].Name
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        //public static string getCommitmentNo(int ProjectType)
        //{
        //    try
        //    {
        //        var CommitNo = "";
        //        string Prefix = "";

        //        using (var context = new IOASDBEntities())
        //        {
        //            var Qry = (from C in context.tblCommitment select C).Count();
        //            var FinancialYear = context.tblFinYear.FirstOrDefault(m => m.CurrentYearFlag == true).Year;
        //            if (Qry > 0)

        //                CommitNo = "COM/" + Prefix + "" + FinancialYear + "/0000" + (Qry + 1);
        //        }
        //                else
        //                {
        //            CommitNo = "COM/" + Prefix + "" + FinancialYear + "/0000" + 1;
        //        }
        //    }
        //        }else
        //        {
        //            CommitNo = "0";
        //        }
        //        return CommitNo;
        //    }

        //    catch (Exception ex)
        //    {

        //        throw ex;

        //    }
        //}

        public static Tuple<List<MasterlistviewModel>, string> getProjectNo(int projecttype)
        {
            try
            {
                var CommitNo = "";
                List<MasterlistviewModel> ProjectNo = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qry = (from C in context.tblProject
                               where (C.ProjectType == projecttype && C.Status == "Active")
                               orderby C.ProjectId
                               select new { C.ProjectId, C.ProjectNumber }).ToList();
                    if (qry.Count > 0)
                    {
                        for (int i = 0; i < qry.Count; i++)
                        {
                            ProjectNo.Add(new MasterlistviewModel()
                            {
                                id = qry[i].ProjectId,
                                name = qry[i].ProjectNumber,
                            });
                        }
                    }
                    int num = (from b in context.tblCommitment
                               select b).Max(m => m.SequenceNo) ?? 0;

                    if (num > 0)
                    {
                        num += 1;                       
                        CommitNo = "COM/" + GetCurrentFinYear() + "/" + num.ToString("000000");
                    }
                    else
                    {
                        CommitNo = "COM/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }

                return Tuple.Create(ProjectNo, CommitNo);
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> ProjectNo = new List<MasterlistviewModel>();
                return Tuple.Create(ProjectNo, "0");
            }

        }

        public static string getCommitmentName(int CommitType)
        {
            try
            {
                string CommitmentType = "";
                using (var context = new IOASDBEntities())
                {
                    var qryCT = (from C in context.tblCodeControl where C.CodeName == "CommitmentType" && C.CodeValAbbr == CommitType select C.CodeValDetail).FirstOrDefault();
                    if (qryCT != null)
                    {
                        CommitmentType = qryCT;
                    }
                }
                return CommitmentType;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static List<MasterlistviewModel> getPurpose()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryPurpose = (from C in context.tblCodeControl where C.CodeName == "StaffPurpose" select new { C.CodeValAbbr, C.CodeValDetail }).ToList();
                    if (qryPurpose.Count > 0)
                    {
                        for (int i = 0; i < qryPurpose.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryPurpose[i].CodeValAbbr,
                                name = qryPurpose[i].CodeValDetail
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }
        public static string getVendorName(int ID)
        {
            try
            {
                string VendorName = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblVendorMaster
                                 where C.VendorId == ID
                                 select C.Name).FirstOrDefault();
                    if (query != null)
                    {
                        VendorName = query;
                    }
                }
                return VendorName;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static string getCurrency(int ID)
        {
            try
            {
                string Currency = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCurrency
                                 where C.CurrencyID == ID
                                 select C.ISOCode).FirstOrDefault();
                    if (query != null)
                    {
                        Currency = query;
                    }
                }
                return Currency;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public static string getRefrenceName(int ID)
        {
            try
            {
                string Refrence = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectSource" && C.CodeValAbbr == ID
                                 select C.CodeValDetail).FirstOrDefault();
                    if (query != null)
                    {
                        Refrence = query;
                    }
                }
                return Refrence;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public static string getPurpose(int ID)
        {
            try
            {
                string Purpose = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "StaffPurpose" && C.CodeValAbbr == ID
                                 select C.CodeValDetail).FirstOrDefault();
                    if (query != null)
                    {
                        Purpose = query;
                    }
                }
                return Purpose;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public static string getTaxStatusById(int taxId)
        {
            try
            {
                string Tax = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ConsProjecttaxtype" && C.CodeValAbbr == taxId
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        Tax = query.CodeValDetail;
                    }

                }

                return Tax;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getStaffDepartName(string DepartCode)
        {
            try
            {
                string DepartName = "";
                using (var context = new IOASDBEntities())
                {
                    var depQry = (from C in context.vwFacultyStaffDetails
                                  where C.DepartmentCode == DepartCode
                                  select C.DepartmentName).FirstOrDefault();
                    if (depQry != null)
                    {
                        DepartName = depQry;
                    }
                }
                return DepartName;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        public static string getAllocationHeadName(int AllocationId)
        {
            try
            {
                string AllocationName = "";
                using (var context = new IOASDBEntities())
                {
                    var qryAN = (from C in context.tblCodeControl where C.CodeName == "AllocationHead" && C.CodeValAbbr == AllocationId select C.CodeValDetail).FirstOrDefault();
                    if (qryAN != null)
                    {
                        AllocationName = qryAN;
                    }
                }
                return AllocationName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string getAllocationType(int AllocationId)
        {
            try
            {
                string AllocationType = "";
                using (var context = new IOASDBEntities())
                {
                    var IsRecuring = (from C in context.tblBudgetHead where C.BudgetHeadId == AllocationId select C.IsRecurring).FirstOrDefault();
                    if (IsRecuring != null)
                    {
                        if (IsRecuring == true)
                        {
                            AllocationType = "Recurring";
                        }
                        else
                        {
                            AllocationType = "Non-Recurring";
                        }
                    }
                }
                return AllocationType;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string getStaffCategoryName(int CategoryId)
        {
            try
            {
                string Category = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProjectStaffCategoryMaster
                                 where C.ProjectStaffCategoryId == CategoryId
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        Category = query.ProjectStaffCategory;
                    }

                }

                return Category;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getTypeofPrjoectName(int TypeOfPrj)
        {
            try
            {
                string TypeofPrj = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "TypeofProject" && C.CodeValAbbr == TypeOfPrj
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        TypeofPrj = query.CodeValDetail;
                    }

                }

                return TypeofPrj;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getProjectSubTypeName(int PrjSubType)
        {
            try
            {
                string SubType = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "SponsoredProjectSubtype" && C.CodeValAbbr == PrjSubType
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        SubType = query.CodeValDetail;
                    }

                }

                return SubType;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getSchemeName(int SchemeId)
        {
            try
            {
                string Scheme = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSchemes
                                 where C.SchemeId == SchemeId
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        Scheme = query.SchemeName;
                    }

                }

                return Scheme;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getSchemeCodeName(int SchemeCodeId)
        {
            try
            {
                string SchemeCode = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSponsoredSchemes
                                 where C.SponsoredSchemesId == SchemeCodeId
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        SchemeCode = query.SchemeCode;
                    }

                }

                return SchemeCode;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getPrjoectFundingTypeName(int FundingType)
        {
            try
            {
                string FundType = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectFundingType" && C.CodeValAbbr == FundingType
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        FundType = query.CodeValDetail;
                    }

                }

                return FundType;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getPrjoectFundedByName(int FundedBy)
        {
            try
            {
                string FundedByName = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectFundedBy" && C.CodeValAbbr == FundedBy
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        FundedByName = query.CodeValDetail;
                    }

                }

                return FundedByName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getSponseredPrjCatagoryName(int PrjCatagory)
        {
            try
            {
                string Sponserd = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "SponProjectCategory" && C.CodeValAbbr == PrjCatagory
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        Sponserd = query.CodeValDetail;
                    }

                }

                return Sponserd;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getConsultancyPrjCatagoryName(int FundCatagory)
        {
            try
            {
                string ConsFundCatagory = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblSchemes
                                 where C.SchemeId == FundCatagory
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        ConsFundCatagory = query.SchemeName;
                    }

                }

                return ConsFundCatagory;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getGovFundingBody(int GovFundingId)
        {
            try
            {
                string ConsFundCatagory = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Indfundgovtbody" && C.CodeValAbbr == GovFundingId
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        ConsFundCatagory = query.CodeValDetail;
                    }

                }

                return ConsFundCatagory;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public static decimal getExchangeRate(string fromCurrency)
        {
            try
            {
                string toCurrency = "INR";
                decimal exchangeRate = 0;
                WebClient web = new WebClient();
                var pair = fromCurrency.ToUpper() + "_" + toCurrency.ToUpper();
                string url = string.Format("https://free.currencyconverterapi.com/api/v5/convert?q={0}&compact=y", pair);
                var response = web.DownloadString(url);
                var split = response.Split((new string[] { "\"val\":" }), StringSplitOptions.None);
                var value = split[1].Split('}')[0];
                exchangeRate = decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                return exchangeRate;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static string getDocumentTypeName(int DocId)
        {
            try
            {
                string DocTypeName = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblDocument
                                 where C.DocumentId == DocId
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        DocTypeName = query.DocumentName;
                    }

                }

                return DocTypeName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static string getProjecTypeName(int projectType)
        {
            try
            {
                string PrjTypeName = "";
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Projecttype" && C.CodeValAbbr == projectType
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        PrjTypeName = query.CodeValDetail;
                    }

                }

                return PrjTypeName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public static List<MasterlistviewModel> GetVendorList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblVendorMaster
                                 orderby C.Name
                                 where C.Status == "Active"
                                 select new { C.Name, C.VendorId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].VendorId,
                                name = query[i].Name,
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> list = new List<MasterlistviewModel>();
                return list;
            }

        }

        public static List<MasterlistviewModel> GetBillTypeList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "DeductionCategory"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }

        public static List<MasterlistviewModel> GetTypeOfServiceList(int type)
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblTaxMaster
                                 where (type == 1 && C.Service_f == true)
                                 || (type == 2 && C.Service_f != true)
                                 || type == 3
                                 orderby C.ServiceType
                                 select new { C.ServiceType, C.TaxMasterId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].TaxMasterId,
                                name = query[i].ServiceType,
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> list = new List<MasterlistviewModel>();
                return list;
            }

        }

        public static List<MasterlistviewModel> GetDocTypeList(int functionId)
        {
            try
            {

                List<MasterlistviewModel> Doctype = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from fd in context.tblFunctionDocument
                                 join d in context.tblDocument on fd.DocumentId equals d.DocumentId
                                 where fd.FunctionId == functionId
                                 orderby d.DocumentName
                                 select d).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Doctype.Add(new MasterlistviewModel()
                            {
                                id = query[i].DocumentId,
                                name = query[i].DocumentName,
                            });
                        }
                    }

                }

                return Doctype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static tblTaxMaster GetServiceDetail(int serviceType)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTaxMaster.FirstOrDefault(dup => dup.TaxMasterId == serviceType);
                    if (query != null)
                    {
                        return query;
                    }
                    else
                        return new tblTaxMaster();
                }

            }
            catch (Exception ex)
            {
                return new tblTaxMaster();
            }
        }

        public static List<CheckListModel> GetCheckedList(int functionId)
        {
            try
            {
                List<CheckListModel> list = new List<CheckListModel>();
                using (var context = new IOASDBEntities())
                {
                    list = (from chk in context.tblFunctionCheckList
                            orderby chk.FunctionCheckListId
                            where chk.FunctionId == functionId
                            select new CheckListModel { CheckList = chk.CheckList, FunctionCheckListId = chk.FunctionCheckListId }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                return new List<CheckListModel>();
            }
        }

        public static List<MasterlistviewModel> GetAccountHeadList(int accountGroupId, int accountHead = 0, string tSubCode = "", string transTypeCode = "", bool? isBank = null)
        {
            try
            {

                List<MasterlistviewModel> headList = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    if (!String.IsNullOrEmpty(tSubCode))
                    {
                        var groupIds = (from c in context.tblTransactionDefinition
                                        where c.TransactionTypeCode == transTypeCode
                                        && c.SubCode == tSubCode && c.AccountGroupId == accountGroupId
                                        select new { c.AccountHeadId }).FirstOrDefault();
                        if (groupIds != null)
                        {
                            int headId = accountHead == 0 ? groupIds.AccountHeadId ?? 0 : accountHead;
                            headList = (from ah in context.tblAccountHead
                                        where ah.AccountGroupId == accountGroupId && (headId == 0 || ah.AccountHeadId == headId)
                                        select new MasterlistviewModel()
                                        {
                                            id = ah.AccountHeadId,
                                            name = ah.AccountHead
                                        }).OrderBy(num => num.id != headId ? num.id : -1).ToList();

                        }
                        else
                        {
                            headList = (from ah in context.tblAccountHead
                                        orderby ah.AccountHead
                                        where ah.AccountGroupId == accountGroupId
                                        select new MasterlistviewModel()
                                        {
                                            id = ah.AccountHeadId,
                                            name = ah.AccountHead
                                        }).ToList();
                        }
                    }
                    else
                    {
                        headList = (from ah in context.tblAccountHead
                                    orderby ah.AccountHead
                                    where ah.AccountGroupId == accountGroupId && (isBank == null || ah.Bank_f == isBank)
                                    select new MasterlistviewModel()
                                    {
                                        id = ah.AccountHeadId,
                                        name = ah.AccountHead
                                    }).ToList();
                    }

                }
                return headList;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static string GetNewPOBillNo(string type)
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblBillEntry
                               where (b.BillNumber.Contains(type + "/"))
                               orderby b.BillId descending
                               select b.BillNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return type + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return type + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewTravelBillNo(string type)
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblTravelBill
                               where (b.BillNumber.Contains(type + "/"))
                               orderby b.TravelBillId descending
                               select b.BillNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return type + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return type + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewProjectTransferNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblProjectTransfer
                               orderby b.ProjectTransferId descending
                               select b.TransferNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "PFT" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "PFT" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewContraNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblContra
                               orderby b.ContraId descending
                               select b.ContraNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "CTR" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "CTR" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewBRSNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblBRS
                               orderby b.BRSId descending
                               select b.BRSNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "BRS" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "BRS" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewCommitmentNo()
        {
            try
            {
                string CommitNo = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    int num = (from b in context.tblCommitment
                               select b).Max(m => m.SequenceNo) ?? 0;

                    if (num > 0)
                    {

                        num += 1;
                        return "COM/" + GetCurrentFinYear() + "/" + num.ToString("00000");
                    }
                    else
                    {
                        return "COM/" + GetCurrentFinYear() + "/" + "00001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewVoucherNo(string voucherType)
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblBOA
                               where (b.VoucherNumber.Contains(voucherType + "/"))
                               orderby b.BOAId descending
                               select b.VoucherNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return voucherType + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return voucherType + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewGeneralVoucherNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblGeneralVoucher
                               orderby b.GeneralVoucherId descending
                               select b.VoucherNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "GVR" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "GVR" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewJvNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblJournal
                               where (b.JournalNumber.Contains("JV/"))
                               orderby b.JournalId descending
                               select b.JournalNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "JV/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "JV/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewCrNoteNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblCreditNote
                               orderby b.CreditNoteId descending
                               select b.CreditNoteNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "CRN/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "CRN/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewTempVoucherNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblBOADraft
                               orderby b.BOADraftId descending
                               select b.TempVoucherNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "PBAT/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "PBAT/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetCurrentFinYear()
        {
            try
            {
                var year = "";
                using (var context = new IOASDBEntities())
                {
                    var financialYear = context.tblFinYear.FirstOrDefault(m => m.CurrentYearFlag == true);
                    if (financialYear != null)
                        year = financialYear.Year;
                }

                return year;
            }

            catch (Exception ex)
            {
                return "";
            }
        }
        public static int GetCurrentFinYearId()
        {
            try
            {
                int cId = 0;
                using (var context = new IOASDBEntities())
                {
                    var financialYear = context.tblFinYear.FirstOrDefault(m => m.CurrentYearFlag == true);
                    if (financialYear != null)
                        cId = financialYear.FinYearId;
                }

                return cId;
            }

            catch (Exception ex)
            {
                return 0;
            }
        }
        public static List<MasterlistviewModel> GetAdvancedPercentageList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "AdvancedPercentage"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }

        public static List<MasterlistviewModel> GetBillRMNGPercentageList(string poNumber, Nullable<Int32> vendorId, int negBillId = 0)
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var previousPct = (from b in context.tblBillEntry
                                       where b.PONumber == poNumber && b.VendorId == vendorId && b.BillId != negBillId && b.Status != "Rejected"
                                       select b.AdvancePercentage ?? 0).Sum();
                    previousPct = 100 - previousPct;
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "AdvancedPercentage" && C.CodeValAbbr < previousPct
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static Tuple<decimal, decimal> GetBillRMNGBalance(string poNumber, Nullable<Int32> vendorId, int negBillId = 0)
        {
            try
            {

                decimal avlAmt = 0, avlTaxAmt = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblBillEntry
                                 where b.PONumber == poNumber && b.VendorId == vendorId && b.BillId != negBillId && b.Status != "Rejected"
                                 select b).ToList();
                    if (query.Count > 0)
                    {
                        decimal? preSettAmt = query.Select(m => m.ExpenseAmount).Sum();
                        decimal? preSettTaxAmt = query.Select(m => m.DeductionAmount).Sum();
                        avlAmt = (query[0].BillAmount - preSettAmt) ?? 0;
                        avlTaxAmt = (query[0].BillTaxAmount - preSettTaxAmt) ?? 0;
                    }

                }

                return Tuple.Create(avlAmt, avlTaxAmt);
            }
            catch (Exception ex)
            {
                return Tuple.Create((Decimal)0, (Decimal)0);
            }

        }
        public static Tuple<decimal, decimal, decimal, decimal> GetBillPaidAndRMNGAmt(string poNumber, Nullable<Int32> vendorId, int negBillId = 0)
        {
            try
            {
                decimal paidAmt = 0, paidTaxAmt = 0, billAmt = 0, billTaxAmt = 0;

                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblBillEntry
                                 where b.PONumber == poNumber && b.VendorId == vendorId && b.BillId != negBillId && b.Status != "Rejected"
                                 select b).ToList();
                    if (query.Count > 0)
                    {
                        paidAmt = query.Select(m => m.ExpenseAmount).Sum() ?? 0;
                        paidTaxAmt = query.Select(m => m.DeductionAmount).Sum() ?? 0;
                        billAmt = query[0].BillAmount ?? 0;
                        billTaxAmt = query[0].BillTaxAmount ?? 0;
                    }

                }
                return Tuple.Create(paidAmt, paidTaxAmt, billAmt, billTaxAmt);
            }
            catch (Exception ex)
            {
                return Tuple.Create((Decimal)0, (Decimal)0, (Decimal)0, (Decimal)0);
            }

        }
        public static decimal GetBillRMNGPercentage(string poNumber, Nullable<Int32> vendorId, int negBillId = 0)
        {
            try
            {

                decimal rmngPct = 0;
                using (var context = new IOASDBEntities())
                {
                    var previousPct = (from b in context.tblBillEntry
                                       where b.PONumber == poNumber && b.VendorId == vendorId && b.BillId != negBillId && b.Status != "Rejected"
                                       select b.AdvancePercentage ?? 0).Sum();
                    rmngPct = 100 - previousPct;

                }

                return rmngPct;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static string GetBillStatus(int billId)
        {
            try
            {
                string status = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId);
                    if (query != null)
                        status = query.Status;
                }
                return status;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetBillPONumber(int billId)
        {
            try
            {
                string no = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId);
                    if (query != null)
                        no = query.PONumber;
                }
                return no;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static bool ValidateBillOnEdit(int billId, string type)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.TransactionTypeCode == type && m.Status == "Open");
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
        
        public static bool ValidatePartBillOnEdit(int billId, bool? partADV_f = null)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry.FirstOrDefault(m => m.BillId == billId && m.TransactionTypeCode == "PTM" && m.Status == "Open" && m.PartAdvance_f == partADV_f);
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
        public static List<MasterlistviewModel> GetBillPONumberList(Nullable<Int32> vendorId, string selPONo = "", string transTypeCode = "")
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = context.tblBillEntry
                        .GroupBy(m => new { m.PONumber, m.VendorId })
                        .Where(b => b.Select(m => m.VendorId).FirstOrDefault() == vendorId && b.Select(m => m.Status).FirstOrDefault() == "Completed"
                        && (String.IsNullOrEmpty(transTypeCode) || b.Any(m => m.TransactionTypeCode == transTypeCode))
                        && (b.Key.PONumber == selPONo || (String.IsNullOrEmpty(selPONo) && b.Sum(m => m.AdvancePercentage) < 100 && transTypeCode != "ADV")
                        || (String.IsNullOrEmpty(selPONo) &&
                        !context.tblBillEntry.Any(m => m.PONumber == b.Select(g => g.PONumber).FirstOrDefault() && m.TransactionTypeCode == "STM")
                        && transTypeCode == "ADV"))
                        )
                        .Select(m => new { BillId = m.OrderByDescending(x => x.BillId).FirstOrDefault().BillId, PONumber = m.Key.PONumber, pct = m.Sum(v => v.AdvancePercentage) }).ToList();

                    //(from b in context.tblBillEntry
                    //where b.VendorId == vendorId && b.Status == "Completed"
                    //&& (transTypeCode == "" || b.TransactionTypeCode == transTypeCode)
                    ////(transTypeCode != "ADV" && b.TransactionTypeCode == transTypeCode) ||
                    ////(transTypeCode == "ADV" && (b.TransactionTypeCode == transTypeCode || b.TransactionTypeCode == "STM")))
                    ////&& !context.tblBillEntry.Any(m => m.PONumber == b.PONumber && m.TransactionTypeCode == "STM" && (string.IsNullOrEmpty(selPONo) || m.PONumber != selPONo))
                    ////&& context.tblBillEntry.Where(m => m.PONumber == b.PONumber && (string.IsNullOrEmpty(selPONo) || m.PONumber != selPONo)).Select(m=>m.AdvancePercentage).Sum() < 100
                    //group b by b.PONumber into g
                    //where g.Any(m => m.PONumber == selPONo || g.Sum(v => v.AdvancePercentage) < 100)
                    //select new { BillId = g.OrderByDescending(x => x.BillId).FirstOrDefault().BillId, PONumber = g.Key, pct = g.Sum(v => v.AdvancePercentage) }).ToList();
                    //var query = context.tblBillEntry

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].BillId,
                                name = query[i].PONumber
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static List<MasterlistviewModel> GetCodeControlList(string codeName, string codeDes = "")
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == codeName && (String.IsNullOrEmpty(codeDes) || C.CodeDescription == codeDes)
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static string ValidateSettlement(string poNumber, Nullable<Int32> vendorId, decimal deductAmt, decimal expAmt, int negBillId = 0)
        {
            try
            {

                string msg = "Settlement amount you have enter is invalid!";
                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblBillEntry
                                 where b.PONumber == poNumber && b.VendorId == vendorId && b.BillId != negBillId && b.Status != "Rejected"
                                 select b).ToList();
                    if (query.Count > 0)
                    {
                        decimal? preSettAmt = query.Select(m => m.ExpenseAmount).Sum();
                        decimal? preSettTaxAmt = query.Select(m => m.DeductionAmount).Sum();
                        decimal avlAmt = (query[0].BillAmount - preSettAmt) ?? 0;
                        decimal avlTaxAmt = (query[0].BillTaxAmount - preSettTaxAmt) ?? 0;
                        if (query[0].EligibleForOffset_f == true || query[0].PartiallyEligibleForOffset_f == true)
                        {
                            if (avlAmt != expAmt)
                                msg = "Settlement expence amount you have enter is invalid!";
                            else if (avlTaxAmt != deductAmt)
                                msg = "Settlement deduction amount you have enter is invalid!";
                            else if (deductAmt == 0 && expAmt == 0)
                                msg = "PO number for this bill already settled !";
                            else
                                msg = "Valid";
                        }
                        else
                        {
                            if (expAmt == (avlAmt + avlTaxAmt) && deductAmt == 0)
                                msg = "Valid";
                        }
                    }

                }

                return msg;
            }
            catch (Exception ex)
            {
                return "Not Valid";
            }

        }

        public static int getProjectID(int InvoiceId)
        {
            try
            {
                var projectid = 0;
                var context = new IOASDBEntities();
                var invoicequery = (from inv in context.tblProjectInvoice
                                    where inv.InvoiceId == InvoiceId
                                    select inv).FirstOrDefault();

                if (invoicequery != null)
                {
                    projectid = Convert.ToInt32(invoicequery.ProjectId);

                    return projectid;
                }
                else
                {
                    return projectid;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static int getProjectIdbyReceiptId(int ReceiptId)
        {
            try
            {
                var projectid = 0;
                var context = new IOASDBEntities();
                var recquery = (from rec in context.tblReceipt
                                where rec.ReceiptId == ReceiptId
                                select rec).FirstOrDefault();

                if (recquery != null)
                {
                    projectid = Convert.ToInt32(recquery.ProjectId);

                    return projectid;
                }
                else
                {
                    return projectid;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static List<MasterlistviewModel> getreceivableshead()
        {
            try
            {

                List<MasterlistviewModel> Receivableshead = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblDeductionHead
                                 join A in context.tblAccountHead on C.AccountHeadId equals A.AccountHeadId
                                 where C.TransactionTypeCode == "RCV"
                                 orderby C.DeductionHeadId
                                 select new { C, A }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Receivableshead.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.DeductionHeadId,
                                name = query[i].A.AccountHead,
                                code = query[i].A.AccountHeadCode
                            });
                        }
                    }

                }

                return Receivableshead;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getmodeofreceipt()
        {
            try
            {

                List<CodeControllistviewModel> modeofreceipt = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ModeofReceipt"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            modeofreceipt.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return modeofreceipt;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> getcurrency()
        {
            try
            {

                List<MasterlistviewModel> Currency = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCurrency
                                 orderby C.ISOCode
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Currency.Add(new MasterlistviewModel()
                            {
                                id = query[i].CurrencyID,
                                name = query[i].CurrencyUnit,
                                code = query[i].ISOCode
                            });
                        }
                    }

                }

                return Currency;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> getinvoicetype()
        {
            try
            {

                List<CodeControllistviewModel> Invoicetype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "InvoiceType"
                                 orderby C.CodeValAbbr
                                 select C).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Invoicetype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Invoicetype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetInvoicedetails()
        {
            try
            {

                List<MasterlistviewModel> Invoicedetails = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProjectInvoiceDraft
                                 join P in context.tblProject on C.ProjectId equals P.ProjectId
                                 join user in context.tblUser on P.PIName equals user.UserId
                                 select new { C, P, user.FirstName, user.LastName, user.EMPCode }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Invoicedetails.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.InvoiceDraftId,
                                name = query[i].C.InvoiceDate + "-" + query[i].C.DescriptionofServices + "-" + query[i].FirstName + " " + query[i].LastName,
                            });
                        }
                    }
                }

                return Invoicedetails;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Invoicedetails = new List<MasterlistviewModel>();
                return Invoicedetails;
            }

        }

        public static List<MasterlistviewModel> getinvocenumber(int pId)
        {
            try
            {
                List<MasterlistviewModel> invoicenumber = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProjectInvoice
                                 where C.ProjectId == pId
                                 orderby C.InvoiceId
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            invoicenumber.Add(new MasterlistviewModel()
                            {
                                id = query[i].InvoiceId,
                                name = query[i].InvoiceNumber
                            });
                        }
                    }
                }
                return invoicenumber;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<MasterlistviewModel> getbudgethead()
        {
            try
            {
                List<MasterlistviewModel> budgethead = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from A in context.tblBudgetHead
                                 orderby A.BudgetHeadId
                                 select A).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            budgethead.Add(new MasterlistviewModel()
                            {
                                id = query[i].BudgetHeadId,
                                name = query[i].HeadName,
                            });
                        }
                    }
                }
                return budgethead;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string getReceiptSequenceNumber()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblReceipt
                               orderby b.ReceiptId descending
                               select b.ReceiptNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "RCV" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "RCV" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<CodeControllistviewModel> getbanktransactiontype()
        {
            try
            {

                List<CodeControllistviewModel> Transactiontype = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "TransactionType"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Transactiontype.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return Transactiontype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string getreceiptnumber(int receiptid)
        {
            try
            {
                var receiptnumber = " ";
                var context = new IOASDBEntities();
                var query = (from rcv in context.tblReceipt
                             where rcv.ReceiptId == receiptid
                             select rcv).FirstOrDefault();

                if (query != null)
                {
                    receiptnumber = query.ReceiptNumber;
                    return receiptnumber;
                }
                else
                {
                    return receiptnumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<CodeControllistviewModel> getreceiptstatus()
        {
            try
            {

                List<CodeControllistviewModel> receiptstatus = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ReceiptStatus"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            receiptstatus.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return receiptstatus;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static bool ValidateTravelBillStatus(int billId, string type, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == billId && m.TransactionTypeCode == type && m.Status == status);
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

        public static bool ValidateJournalStatus(int journalId, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblJournal.FirstOrDefault(m => m.JournalId == journalId && m.Status == status);
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

        public static List<MasterlistviewModel> GetStudentList()
        {
            try
            {

                List<MasterlistviewModel> stud = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    stud = (from C in context.vwStudentDetails
                            orderby C.FirstName
                            select new MasterlistviewModel()
                            {
                                code = C.RollNumber,
                                name = C.RollNumber + "-" + C.FirstName
                            }).ToList();

                }

                return stud;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }


        public static List<MasterlistviewModel> GetAgencyDocument()
        {

            try
            {
                List<MasterlistviewModel> Agencydoc = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var documentid = (from FD in context.tblFunctionDocument
                                      where (FD.FunctionId == 19)
                                      select FD.DocumentId).ToArray();
                    if (documentid != null)
                    {
                        var query = (from D in context.tblDocument
                                     where documentid.Contains(D.DocumentId)
                                     select new { D.DocumentId, D.DocumentName }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Agencydoc.Add(new MasterlistviewModel()
                                {

                                    id = query[i].DocumentId,
                                    name = query[i].DocumentName
                                });
                            }
                        }
                    }
                    return Agencydoc;
                }
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Agencydoc = new List<MasterlistviewModel>();
                return Agencydoc;
            }
        }

        public static List<CodeControllistviewModel> getprojectsource()
        {
            try
            {

                List<CodeControllistviewModel> projectsource = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "ProjectSource"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            projectsource.Add(new CodeControllistviewModel()
                            {
                                CodeName = query[i].CodeName,
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeValDetail = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return projectsource;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetGstSupportingDoc()
        {
            try
            {
                List<MasterlistviewModel> Gstsupport = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblDocument
                                 from FD in context.tblFunctionDocument
                                 where D.DocumentId == FD.DocumentId && FD.FunctionId == 40
                                 select new { D.DocumentId, D.DocumentName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Gstsupport.Add(new MasterlistviewModel()
                            {
                                id = query[i].DocumentId,
                                name = query[i].DocumentName
                            });
                        }
                    }
                }
                return Gstsupport;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> Gstsupport = new List<MasterlistviewModel>();
                return Gstsupport;
            }
        }
        public static List<MasterlistviewModel> GetVendorSupportingDoc()
        {
            try
            {
                List<MasterlistviewModel> vendorsupportdoc = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblDocument
                                 from FD in context.tblFunctionDocument
                                 where D.DocumentId == FD.DocumentId && FD.FunctionId == 40
                                 select new { D.DocumentId, D.DocumentName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            vendorsupportdoc.Add(new MasterlistviewModel()
                            {
                                id = query[i].DocumentId,
                                name = query[i].DocumentName
                            });
                        }
                    }
                }
                return vendorsupportdoc;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> vendorsupportdoc = new List<MasterlistviewModel>();
                return vendorsupportdoc;
            }
        }
        public static List<MasterlistviewModel> GetVendorTdsDoc()
        {
            try
            {
                List<MasterlistviewModel> tdssupport = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from D in context.tblDocument
                                 from FD in context.tblFunctionDocument
                                 where D.DocumentId == FD.DocumentId && FD.FunctionId == 40
                                 select new { D.DocumentId, D.DocumentName }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            tdssupport.Add(new MasterlistviewModel()
                            {
                                id = query[i].DocumentId,
                                name = query[i].DocumentName
                            });
                        }
                    }
                }
                return tdssupport;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> tdssupport = new List<MasterlistviewModel>();
                return tdssupport;
            }
        }
        public static List<MasterlistviewModel> GetCategoryService()
        {
            try
            {
                List<MasterlistviewModel> getcategoryservice = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "DeductionCategory"
                                 select new { C.CodeValAbbr, C.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            getcategoryservice.Add(new MasterlistviewModel()
                            {

                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }
                return getcategoryservice;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> getcategoryservice = new List<MasterlistviewModel>();
                return getcategoryservice;
            }
        }
        public static List<MasterlistviewModel> GetServiceTypeList()
        {
            try
            {
                List<MasterlistviewModel> getsupport = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from S in context.tblTaxMaster
                                 where S.Service_f == true
                                 orderby S.ServiceType
                                 select new { S.TaxMasterId, S.ServiceType }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            getsupport.Add(new MasterlistviewModel()
                            {
                                id = query[i].TaxMasterId,
                                name = query[i].ServiceType

                            });
                        }
                    }
                }
                return getsupport;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> getsupport = new List<MasterlistviewModel>();
                return getsupport;
            }
        }
        public static List<MasterlistviewModel> GetSupplierType()
        {
            try
            {
                List<MasterlistviewModel> suppliertype = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from S in context.tblTaxMaster
                                 where S.Service_f == false
                                 select new { S.TaxMasterId, S.ServiceType }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            suppliertype.Add(new MasterlistviewModel()
                            {
                                id = query[i].TaxMasterId,
                                name = query[i].ServiceType
                            });
                        }
                    }
                }
                return suppliertype;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> suppliertype = new List<MasterlistviewModel>();
                return suppliertype;
            }
        }
        public static decimal GetVendorTDSPercentage(int vendorTDSDetailId)
        {
            try
            {
                decimal tdsPct = 0;
                using (var context = new IOASDBEntities())
                {
                    var previousPct = context.tblVendorTDSDetail.FirstOrDefault(m => m.VendorTDSDetailId == vendorTDSDetailId);
                    if (previousPct != null)
                        tdsPct = previousPct.TDSPercentage ?? 0;
                }
                return tdsPct;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static List<MasterlistviewModel> GetVendorTDSList(Nullable<Int32> vendorId)
        {
            try
            {
                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    list = (from b in context.tblVendorTDSDetail
                            from T in context.tblTDSMaster
                            where b.VendorId == vendorId && b.Section == T.TdsMasterId
                            select new MasterlistviewModel()
                            {
                                id = T.TdsMasterId,
                                name = T.Section
                            }).ToList();
                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }

        public static Functionviewmodel GetAccessDetail(int functionId, string username)
        {
            try
            {
                Functionviewmodel model = new Functionviewmodel();

                using (var context = new IOASDBEntities())
                {
                    model = (from ra in context.tblRoleaccess
                             join u in context.tblUser on ra.RoleId equals u.RoleId
                             where ra.FunctionId == functionId && u.UserName == username && u.Status == "Active"
                             select new Functionviewmodel()
                             {
                                 Add = ra.Add_f ?? false,
                                 Delete = ra.Delete_f ?? false,
                                 Approve = ra.Approve_f ?? false,
                                 Read = ra.Read_f ?? false
                             }).FirstOrDefault();
                }
                return model;
            }
            catch (Exception ex)
            {
                return new Functionviewmodel();
            }

        }

        public static List<MasterlistviewModel> GetTravelADVList(int? refADVBillId = null, int? PI = null)
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblTravelBill
                                 where b.Status == "Completed" && b.TransactionTypeCode == "TAD"
                                 && (PI == null || b.PI == PI)
                                 && !context.tblTravelBill.Any(m => m.RefTravelBillId == b.TravelBillId && m.TransactionTypeCode == "TST" && (refADVBillId == null || m.RefTravelBillId != refADVBillId))
                                 select new { b.BillNumber, b.TravelBillId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].TravelBillId,
                                name = query[i].BillNumber
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }

        public static decimal GetTravellerDailyAllowance(int countryId)
        {
            try
            {

                decimal allow = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelDailyAllowance.FirstOrDefault(m => m.CountryId == countryId);

                    if (query != null)
                    {
                        allow = getExchangeRate("USD") * query.AllowanceRateInUSD ?? 0;
                    }

                }

                return Math.Round(allow);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public static decimal GetTravelAdvanceValueWOClearanceAgent(int travelId)
        {
            try
            {

                decimal amt = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelPaymentBreakUpDetail.Where(m => m.TravelBillId == travelId && m.CategoryId != 3).ToList();

                    if (query.Count > 0)
                    {
                        amt = query.Sum(m => m.PaymentAmount ?? 0);
                    }

                }

                return Math.Round(amt);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public static string getSBIEcardnumber(int SBIECardID)
        {
            try
            {
                var SBICardnumber = " ";
                var context = new IOASDBEntities();
                var query = (from EC in context.tblSBIECardDetails
                             where EC.SBIPrepaidCardDetailsId == SBIECardID
                             select EC).FirstOrDefault();

                if (query != null)
                {
                    SBICardnumber = query.SBIPrepaidCardNumber;
                    return SBICardnumber;
                }
                else
                {
                    return SBICardnumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<AutoCompleteModel> GetAutoCompleteStudentList(string term)
        {
            try
            {

                List<AutoCompleteModel> stud = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    stud = (from C in context.vwStudentDetails
                            where string.IsNullOrEmpty(term) || C.RollNumber.Contains(term) || C.FirstName.Contains(term)
                            orderby C.FirstName
                            select new AutoCompleteModel()
                            {
                                value = C.RollNumber,
                                label = C.RollNumber + "-" + C.FirstName
                            }).ToList();

                }

                return stud;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }

        public static List<AutoCompleteModel> GetAutoCompleteProjectList(string term, int? type = null, int? classification = null)
        {
            try
            {

                List<AutoCompleteModel> list = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    list = (from P in context.tblProject
                            join U in context.vwFacultyStaffDetails on P.PIName equals U.UserId
                            where (string.IsNullOrEmpty(term) || P.ProjectNumber.Contains(term) || U.FirstName.Contains(term))
                            && (type == null || type == P.ProjectType)
                            && (classification == null || classification == P.ProjectClassification)
                            && P.Status == "Active"
                            orderby P.ProjectNumber
                            select new
                            {
                                P.ProjectId,
                                P.ProjectNumber,
                                U.FirstName
                            })
                            .AsEnumerable()
                            .Select((x, index) => new AutoCompleteModel()
                            {
                                value = x.ProjectId.ToString(),
                                label = x.ProjectNumber + "-" + x.FirstName
                            }).ToList();

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }

        public static List<AutoCompleteModel> GetAutoCompletePIWithDetails(string term)
        {
            try
            {

                List<AutoCompleteModel> PI = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.vwFacultyStaffDetails
                                 where string.IsNullOrEmpty(term) || C.EmployeeId.Contains(term) || C.FirstName.Contains(term)
                                 //join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                 //where (C.RoleId == 7)
                                 orderby C.FirstName
                                 select new { C.UserId, C.FirstName, C.EmployeeId }).ToList();


                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PI.Add(new AutoCompleteModel()
                            {
                                value = query[i].UserId.ToString(),
                                label = query[i].EmployeeId + "-" + query[i].FirstName,
                            });
                        }
                    }


                }

                return PI;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static List<AutoCompleteModel> GetAutoCompleteInvoceNumber(string term)
        {
            try
            {
                List<AutoCompleteModel> list = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    list = (from C in context.tblProjectInvoice
                            where (String.IsNullOrEmpty(term) || C.InvoiceNumber.Contains(term)) && C.Status != "Completed" && C.Status != "InActive"
                            orderby C.InvoiceId
                            select new AutoCompleteModel()
                            {
                                value = C.InvoiceId.ToString(),
                                label = C.InvoiceNumber
                            }).ToList();


                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }
        }
        public static CommitAllocationHeadDetails getAllocationValue(int ProjectId, int AllocationId)
        {
            CommitAllocationHeadDetails model = new CommitAllocationHeadDetails();
            try
            {
                decimal PrjAllocValue = 0;
                decimal PrjEnhanceVal = 0;
                decimal AllocForCurrYear = 0;
                using (var context = new IOASDBEntities())
                {
                    var queryAlloc = (from C in context.tblProjectAllocation
                                      where C.ProjectId == ProjectId && C.AllocationHead == AllocationId
                                      select C).FirstOrDefault();
                    var queryEnhAlloc = (from C in context.tblProjectEnhancementAllocation
                                         where C.ProjectId == ProjectId && C.AllocationHead == AllocationId && C.IsCurrentVersion == true
                                         select C).FirstOrDefault();
                    var qryPreviousCommit = (from C in context.tblCommitment
                                             from D in context.tblCommitmentDetails
                                             where C.CommitmentId == D.CommitmentId && C.Status == "Active" && D.AllocationHeadId == AllocationId && C.ProjectId == ProjectId
                                             //&& C.CRTD_TS < DateTime.Now
                                             select D.Amount).Sum();
                    var queryOB = (from C in context.tblProjectOBDetail
                                   where C.ProjectId == ProjectId && C.HeadId == AllocationId
                                   select C).FirstOrDefault();
                    if (queryAlloc != null)
                        PrjAllocValue = queryAlloc.AllocationValue ?? 0;
                    if (queryEnhAlloc != null)
                        PrjEnhanceVal = queryEnhAlloc.EnhancedValue ?? 0;
                    var Amount = PrjAllocValue + PrjEnhanceVal;

                    int Year = 1;
                    var YearAlloc = (from C in context.tblProject
                                     from D in context.tblProjectAllocation
                                     where C.ProjectId == D.ProjectId
                                     where C.ProjectId == ProjectId && D.AllocationHead == AllocationId && C.IsYearWiseAllocation == true
                                     select C).FirstOrDefault();
                    if (YearAlloc != null)
                    {
                        model.IsYearWise = YearAlloc.IsYearWiseAllocation ?? false;
                        var prjStartDate = YearAlloc.TentativeStartDate;
                        DateTime todayDate = DateTime.Now;
                        TimeSpan Days = Convert.ToDateTime(todayDate) - Convert.ToDateTime(prjStartDate);
                        int TotDays = Days.Days;
                        int curYear = DateTime.Now.Year;
                        int days;
                        if (DateTime.IsLeapYear(curYear))
                        {
                            days = 366;
                        }
                        else
                        {
                            days = 365;
                        }
                        for (int i = days; i <= TotDays; i += days)
                        {
                            Year += 1;
                            curYear = DateTime.Now.AddDays(days).Year;
                            if (DateTime.IsLeapYear(curYear))
                            {
                                days = 366;
                            }
                            else
                            {
                                days = 365;
                            }
                        }
                    }
                    var AllocYear = (from C in context.tblProject
                                     from D in context.tblProjectAllocation
                                     where C.ProjectId == D.ProjectId
                                     where C.ProjectId == ProjectId && D.AllocationHead == AllocationId && C.IsYearWiseAllocation == true
                                     && D.Year == Year
                                     select D).FirstOrDefault();
                    if (AllocYear != null)
                    {
                        AllocForCurrYear = AllocYear.AllocationValue ?? 0;
                    }
                    model.TotalAllocation = Amount;
                    model.AllocationForCurrentYear = AllocForCurrYear;
                    model.TotalCommitmentTilDate = qryPreviousCommit ?? 0;

                    if (queryOB != null)
                    {
                        model.OpeningBalance = queryOB.OpeningExp ?? 0;
                    }
                    else
                    {
                        model.OpeningBalance = 0;
                    }

                    model.IsAllocation = true;
                    if (model.IsYearWise)
                    {
                        model.TotalCommitForCurrentYear = model.AllocationForCurrentYear - model.TotalCommitmentTilDate - model.OpeningBalance;
                    }
                    else
                    {
                        model.TotalCommitForCurrentYear = model.TotalAllocation - model.TotalCommitmentTilDate - model.OpeningBalance;
                    }

                    if (queryAlloc == null && queryEnhAlloc == null)
                    {
                        var SanctionValue = (from C in context.tblProject
                                             where C.ProjectId == ProjectId && C.Status == "Active"
                                             select C.SanctionValue).FirstOrDefault();
                        var PreviousCommit = (from C in context.tblCommitment
                                              from D in context.tblCommitmentDetails
                                              where C.CommitmentId == D.CommitmentId && C.Status == "Active" && C.ProjectId == ProjectId
                                              && C.CRTD_TS < DateTime.Now
                                              select D.Amount).Sum();
                        model.TotalCommitmentTilDate = PreviousCommit ?? 0;
                        model.SanctionedValue = SanctionValue ?? 0;
                        model.TotalCommitForCurrentYear = model.SanctionedValue - model.TotalCommitmentTilDate;
                        model.IsAllocation = false;
                    }
                }
                return model;
            }
            catch (Exception ex)
            {

                return model;
            }

        }
        public static List<MasterlistviewModel> getAllocationHeadBasedOnProject(int projectID)
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    int Year = 1;
                    bool IsYearWise = false;
                    var YearAlloc = (from C in context.tblProject
                                     from D in context.tblProjectAllocation
                                     where C.ProjectId == D.ProjectId
                                     where C.ProjectId == projectID && C.IsYearWiseAllocation == true
                                     select C).FirstOrDefault();
                    if (YearAlloc != null)
                    {
                        IsYearWise = YearAlloc.IsYearWiseAllocation ?? false;
                        var prjStartDate = YearAlloc.TentativeStartDate;
                        DateTime todayDate = DateTime.Now;
                        TimeSpan Days = Convert.ToDateTime(todayDate) - Convert.ToDateTime(prjStartDate);
                        int TotDays = Days.Days;
                        int curYear = DateTime.Now.Year;
                        int days;
                        if (DateTime.IsLeapYear(curYear))
                        {
                            days = 366;
                        }
                        else
                        {
                            days = 365;
                        }
                        for (int i = days; i <= TotDays; i += days)
                        {
                            Year += 1;
                            curYear = DateTime.Now.AddDays(days).Year;
                            if (DateTime.IsLeapYear(curYear))
                            {
                                days = 366;
                            }
                            else
                            {
                                days = 365;
                            }
                        }

                        var qryBudgetHead = (from C in context.tblBudgetHead
                                             join D in context.tblProjectAllocation on C.BudgetHeadId equals D.AllocationHead
                                             join E in context.tblProject on D.ProjectId equals E.ProjectId
                                             where E.ProjectId == projectID && D.Year == Year
                                             select new { C, D, E }).ToList();
                        if (qryBudgetHead.Count > 0)
                        {
                            for (int i = 0; i < qryBudgetHead.Count; i++)
                            {
                                List.Add(new MasterlistviewModel()
                                {
                                    id = qryBudgetHead[i].C.BudgetHeadId,
                                    name = qryBudgetHead[i].C.HeadName
                                });
                            }
                        }

                    }
                    else
                    {
                        //if(IsYearWise)
                        var qryBudgetHead = (from C in context.tblBudgetHead
                                             join D in context.tblProjectAllocation on C.BudgetHeadId equals D.AllocationHead
                                             join E in context.tblProject on D.ProjectId equals E.ProjectId
                                             where E.ProjectId == projectID
                                             select new { C, D, E }).ToList();
                        if (qryBudgetHead.Count > 0)
                        {
                            for (int i = 0; i < qryBudgetHead.Count; i++)
                            {
                                List.Add(new MasterlistviewModel()
                                {
                                    id = qryBudgetHead[i].C.BudgetHeadId,
                                    name = qryBudgetHead[i].C.HeadName
                                });
                            }
                        }
                        else
                        {
                            var qryAllo = (from C in context.tblBudgetHead select C).ToList();
                            if (qryAllo.Count > 0)
                            {
                                for (int i = 0; i < qryAllo.Count; i++)
                                {
                                    List.Add(new MasterlistviewModel()
                                    {
                                        id = qryAllo[i].BudgetHeadId,
                                        name = qryAllo[i].HeadName
                                    });
                                }
                            }
                        }

                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }

        public static string getprojectTypeName(int projectType)
        {
            try
            {

                string ProjectTypeName = "";

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Projecttype"
                                 where C.CodeValAbbr == projectType
                                 select C).FirstOrDefault();


                    if (query != null)
                    {
                        ProjectTypeName = query.CodeValDetail;
                    }

                }

                return ProjectTypeName;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public static List<MasterlistviewModel> GetEmployeeName()
        {
            List<MasterlistviewModel> model = new List<MasterlistviewModel>();
            try
            {

                using (var context = new IOASDBEntities())
                {

                    var query = (from H in context.vwFacultyStaffDetails
                                 select new
                                 {
                                     H.FirstName,
                                     H.EmployeeId
                                 }).Distinct().ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            model.Add(new MasterlistviewModel()
                            {
                                code = query[i].EmployeeId,
                                name = query[i].EmployeeId + "-" + query[i].FirstName,
                            });
                        }
                    }

                }

                return model;
            }
            catch (Exception ex)
            {
                return model;
            }

        }

        public static int getProjecTypeId(string projectType)
        {
            try
            {
                int PrjTypeid = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "Projecttype" && C.CodeValDetail == projectType
                                 select C).FirstOrDefault();

                    if (query != null)
                    {
                        PrjTypeid = query.CodeValAbbr;
                    }

                }

                return PrjTypeid;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public static List<MasterlistviewModel> GetCommitmentAction()
        {
            List<MasterlistviewModel> Commit = new List<MasterlistviewModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var QryAction = (from C in context.tblCodeControl where C.CodeName == "CommitmentClosedReason" select C).ToList();
                    if (QryAction.Count > 0)
                    {
                        for (int k = 0; k < QryAction.Count; k++)
                        {
                            Commit.Add(new MasterlistviewModel()
                            {
                                id = QryAction[k].CodeValAbbr,
                                name = QryAction[k].CodeValDetail
                            });
                        }
                    }
                }
                return Commit;
            }
            catch (Exception ex)
            {
                return Commit;
            }
        }


        public static string GetCommitClosedReasonById(int Reason)
        {
            try
            {
                string strReason = "";
                using (var context = new IOASDBEntities())
                {
                    var QryAction = (from C in context.tblCodeControl where C.CodeName == "CommitmentClosedReason" && C.CodeValAbbr == Reason select C.CodeValDetail).FirstOrDefault();
                    if (QryAction != null)
                    {
                        strReason = QryAction;
                    }
                }
                return strReason;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static List<AutoCompleteModel> GetAutoCompleteTypeOfServiceList(string term, int? type = null)
        {
            try
            {

                List<AutoCompleteModel> list = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    list = (from C in context.tblTaxMaster
                            where (String.IsNullOrEmpty(term) || C.ServiceType.Contains(term) || C.TaxCode.Contains(term))
                            && (type == null || (type == 1 && C.Service_f == true)
                            || (type == 2 && C.Service_f != true)
                            || type == 3)
                            orderby C.TaxCode
                            select new AutoCompleteModel()
                            {
                                value = C.TaxMasterId.ToString(),
                                label = C.TaxCode + "-" + C.ServiceType
                            }).ToList();

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static ProjectDetailModel GetProjectsDetails(int ProjectId)
        {
            try
            {
                ProjectDetailModel Detail = new ProjectDetailModel();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from pro in context.tblProject
                                 where pro.ProjectId == ProjectId
                                 select new { pro.ProjectId, pro.ProjectTitle, pro.ProjectType, pro.ProjectSubType, pro.ProjectCategory, pro.SponProjectCategory, pro.ConsultancyFundingCategory, pro.SchemeName, pro.SanctionValue, pro.TentativeStartDate }).FirstOrDefault();
                    if (Query != null)
                    {
                        int prjType = Query.ProjectType ?? 0;
                        int prjSubType = Query.ProjectSubType ?? 0;
                        int prjCatagory = Query.ProjectCategory ?? 0;
                        string sponprjCatagory = Query.SponProjectCategory ?? null;
                        int sponSchemeName = Query.SchemeName ?? 0;
                        int consfundCatagory = Query.ConsultancyFundingCategory ?? 0;
                        int sponprjctCatagory = Convert.ToInt32(sponprjCatagory);
                        Detail.ProjectId = Query.ProjectId;
                        Detail.ProjectTittle = Query.ProjectTitle;
                        Detail.SancationValue = Convert.ToDecimal(Query.SanctionValue);
                        Detail.SancationDate = Convert.ToDateTime(Query.TentativeStartDate);
                        var Type = context.tblCodeControl.FirstOrDefault(m => m.CodeValAbbr == prjType && m.CodeName == "Projecttype");
                        if (Type != null)
                        {
                            Detail.ProjectType = Type.CodeValDetail;
                        }
                        else
                        {
                            Detail.ProjectType = "NA";
                        }
                        var prjSub = context.tblCodeControl.FirstOrDefault(m => m.CodeValAbbr == prjSubType && m.CodeName == "ProjectSubtype");
                        if (prjSub != null)
                        {
                            Detail.ProjectSubType = prjSub.CodeValDetail;
                        }
                        else
                        {
                            Detail.ProjectSubType = "NA";
                        }
                        var prjCat = context.tblCodeControl.FirstOrDefault(m => m.CodeValAbbr == prjCatagory && m.CodeName == "CategoryofProject");
                        if (prjCat != null)
                        {
                            Detail.ProjectCategory = prjCat.CodeValDetail;
                        }
                        else
                        {
                            Detail.ProjectCategory = "NA";
                        }
                        var sponprjCat = context.tblCodeControl.FirstOrDefault(m => m.CodeValAbbr == sponprjctCatagory && m.CodeName == "SponProjectCategory");
                        if (sponprjCat != null)
                        {
                            Detail.SponProjectCategory = sponprjCat.CodeValDetail;
                        }
                        else
                        {
                            Detail.SponProjectCategory = "NA";
                        }
                        var sponschemeName = context.tblSchemes.FirstOrDefault(m => m.SchemeId == sponSchemeName && m.ProjectType == prjType);
                        if (sponschemeName != null)
                        {
                            Detail.SponSchemeName = sponschemeName.SchemeName;
                        }
                        else
                        {
                            Detail.SponSchemeName = "NA";
                        }
                        var consFundingCat = context.tblSchemes.FirstOrDefault(m => m.SchemeId == consfundCatagory && m.ProjectType == prjType);
                        if (consFundingCat != null)
                        {
                            Detail.ConsFundingCategory = consFundingCat.SchemeName;
                        }
                        else
                        {
                            Detail.ConsFundingCategory = "NA";
                        }
                    }
                    return Detail;
                }
            }
            catch (Exception ex)
            {
                ProjectDetailModel Detail = new ProjectDetailModel();
                return Detail;
            }
        }


        public static bool ValidateTempAdvBillOnEdit(int tmpadvanceId, string type)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTemporaryAdvance.FirstOrDefault(m => m.TemporaryAdvanceId == tmpadvanceId && m.TransactionTypeCode == type && m.Status == "Open");
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

        public static List<TempAdvlistviewModel> getProjectListofPI(int PIId)
        {
            try
            {

                List<TempAdvlistviewModel> Title = new List<TempAdvlistviewModel>();
                Title.Add(new TempAdvlistviewModel()
                {
                    id = 0,
                    name = "Select Any",
                });
                using (var context = new IOASDBEntities())
                {
                    if (PIId > 0)
                    {
                        var query = (from P in context.tblProject
                                     join U in context.vwFacultyStaffDetails on P.PIName equals U.UserId
                                     where (P.PIName == PIId && P.Status == "Active")
                                     orderby P.ProjectId
                                     select new { U.FirstName, P, U.DepartmentName }).ToList();
                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                Title.Add(new TempAdvlistviewModel()
                                {
                                    id = query[i].P.ProjectId,
                                    name = query[i].P.ProjectNumber + "-" + query[i].P.ProjectTitle + "- " + query[i].FirstName,
                                    code = query[i].FirstName,
                                    pidepartment = query[i].DepartmentName
                                });
                            }
                        }
                        if (query.Count() == 0)
                        {
                            var piquery = (from U in context.vwFacultyStaffDetails
                                           where (U.UserId == PIId)
                                           select U).FirstOrDefault();
                            Title.Add(new TempAdvlistviewModel()
                            {
                                code = piquery.FirstName,
                                pidepartment = piquery.DepartmentName
                            });
                        }

                    }
                }

                return Title;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string GetStateName(int stateId)
        {
            try
            {
                string stateName = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblStateMaster.FirstOrDefault(m => m.StateId == stateId);
                    if (query != null)
                        stateName = query.StateName;
                    return stateName;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static List<TempAdvlistviewModel> getprojectadvancedetails(int ProjectId)
        {
            try
            {

                List<TempAdvlistviewModel> details = new List<TempAdvlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    if (ProjectId > 0)
                    {
                        var query = (from adv in context.tblTemporaryAdvance
                                     where (adv.ProjectId == ProjectId && adv.IsPendingSettlement_f == true)
                                     select adv).ToList();
                        var Projectquery = (from P in context.tblProject
                                            where (P.ProjectId == ProjectId && P.Status == "Active")
                                            select P).SingleOrDefault();
                        var count = query.Count();
                        for (int i = 0; i < query.Count(); i++)
                        {
                            details.Add(new TempAdvlistviewModel()
                            {
                                id = query[i].TemporaryAdvanceId,
                                name = query[i].TemporaryAdvanceNumber,
                                count = count,
                                amount = query[i].TemporaryAdvanceAmountReceived,
                            });
                        }
                    }
                }

                return details;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<MasterlistviewModel> GetMainProjectNumberList(int projectType)
        {
            try
            {
                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 join U in context.vwFacultyStaffDetails on C.PIName equals U.UserId
                                 where C.Status == "Active" && C.IsSubProject != true && C.ProjectType == projectType
                                 orderby C.ProjectId
                                 select new { U.FirstName, C }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Title.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.ProjectId,
                                name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "- " + query[i].FirstName,
                            });
                        }
                    }

                }



                return Title;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }

        public static bool ValidateSummerInternshipOnEdit(int internId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSummerInternshipStudentDetails.FirstOrDefault(m => m.SummerInternshipStudentId == internId && m.Status == "Open");
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
        public static string getSummrinternId()
        {
            try
            {
                var currsummrinternid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblSummerInternshipStudentDetails
                             orderby intern.SummerInternshipStudentId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var internid = query.SummerInternshipStudentId;
                    currsummrinternid = internid + 1;
                    seqnum = currsummrinternid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static Tuple<string, bool> GetTransactionType(int groupId, int headId, string typeCode, string tSubCode)
        {
            try
            {

                using (var context = new IOASDBEntities())
                {
                    var transTypeQuery = context.tblTransactionDefinition.FirstOrDefault(c => c.TransactionTypeCode == typeCode
                                 && c.SubCode == tSubCode && c.AccountGroupId == groupId && (c.AccountHeadId == null || c.AccountHeadId == headId));
                    if (transTypeQuery != null)
                        return Tuple.Create(transTypeQuery.TransactionType, transTypeQuery.IsJV_f ?? false);
                }

                return Tuple.Create("", false);
            }

            catch (Exception ex)
            {
                return Tuple.Create("", false);
            }
        }
        public static List<MasterlistviewModel> GetCompanyType()
        {
            try
            {
                List<MasterlistviewModel> com = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "CompanyType"
                                 orderby C.CodeValDetail
                                 select new { C.CodeValAbbr, C.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            com.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }
                }
                return com;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> com = new List<MasterlistviewModel>();
                return com;
            }
        }
        public static List<MasterlistviewModel> GetGovermentAgy()
        {
            try
            {
                List<MasterlistviewModel> com = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "govertmentagy"
                                 orderby C.CodeValDetail
                                 select new { C.CodeValAbbr, C.CodeValDetail }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            com.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }
                }
                return com;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> com = new List<MasterlistviewModel>();
                return com;
            }
        }

        public static bool ValidateClearancePaymentOnEdit(int billId, string type)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblClearancePaymentEntry.FirstOrDefault(m => m.ClearancePaymentId == billId && m.TransactionTypeCode == type && m.Status == "Open");
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
        public static List<MasterlistviewModel> getbankcreditaccounthead()
        {
            try
            {

                List<MasterlistviewModel> Accounthead = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblAccountHead
                                 where C.AccountGroupId == 38
                                 orderby C.AccountHeadId
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Accounthead.Add(new MasterlistviewModel()
                            {
                                id = query[i].AccountHeadId,
                                name = query[i].AccountHead,
                                code = query[i].AccountHeadCode
                            });
                        }
                    }

                }

                return Accounthead;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<CodeControllistviewModel> GetCLPTypeOfServiceList()
        {
            try
            {

                List<CodeControllistviewModel> list = new List<CodeControllistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where (C.CodeName == "ClearancePaymentServiceType")
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new CodeControllistviewModel()
                            {
                                codevalAbbr = query[i].CodeValAbbr,
                                CodeName = query[i].CodeName,
                                CodeValDetail = query[i].CodeValDetail,
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                List<CodeControllistviewModel> list = new List<CodeControllistviewModel>();
                return list;
            }

        }
        public static List<MasterlistviewModel> GetClearancePaymentPONumberList(Nullable<Int32> agentId, string transTypeCode = "")
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblClearancePaymentEntry
                                 where b.ClearancePaymentAgentId == agentId && b.Status == "Completed" && (transTypeCode == "" || b.TransactionTypeCode == transTypeCode)
                                 group b by b.ReferencePONumber into g
                                 select new { BillId = g.OrderByDescending(x => x.ClearancePaymentId).FirstOrDefault().ClearancePaymentId, RefPONumber = g.Key }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].BillId,
                                name = query[i].RefPONumber
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static decimal GetClearancePaymentRMNGPercentage(string poNumber, Nullable<Int32> agentId, int negBillId = 0)
        {
            try
            {

                decimal rmngPct = 0;
                using (var context = new IOASDBEntities())
                {
                    var previousPct = (from b in context.tblClearancePaymentEntry
                                       where b.ReferencePONumber == poNumber && b.ClearancePaymentAgentId == agentId && b.ClearancePaymentId != negBillId && b.Status != "Rejected"
                                       select b.AdvancePercentage ?? 0).Sum();
                    rmngPct = 100 - previousPct;

                }

                return rmngPct;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static string GetCLPNewNumber(string type)
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblClearancePaymentEntry
                               where (b.BillNumber.Contains(type + "/"))
                               orderby b.ClearancePaymentId descending
                               select b.BillNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return type + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return type + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Tuple<decimal, decimal> GetCPBillRMNGBalance(string poNumber, Nullable<Int32> agentId, int negBillId = 0)
        {
            try
            {

                decimal avlAmt = 0, avlTaxAmt = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblClearancePaymentEntry
                                 where b.ReferencePONumber == poNumber && b.ClearancePaymentAgentId == agentId && b.ClearancePaymentId != negBillId && b.Status != "Rejected"
                                 select b).ToList();
                    if (query.Count > 0)
                    {
                        decimal? preSettAmt = query.Select(m => m.ExpenseAmount).Sum();
                        decimal? preSettTaxAmt = query.Select(m => m.DeductionAmount).Sum();
                        avlAmt = (query[0].BillAmount - preSettAmt) ?? 0;
                        avlTaxAmt = (query[0].BillTaxAmount - preSettTaxAmt) ?? 0;
                    }

                }

                return Tuple.Create(avlAmt, avlTaxAmt);
            }
            catch (Exception ex)
            {
                return Tuple.Create((Decimal)0, (Decimal)0);
            }

        }
        public static List<MasterlistviewModel> GetClearanceAgentTDSList(Nullable<Int32> agentId)
        {
            try
            {
                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    list = (from b in context.tblClearanceAgentTDSDetail
                            where b.ClearanceAgentId == agentId
                            select new MasterlistviewModel()
                            {
                                id = b.ClearanceAgentTDSDetailId,
                                name = b.Section
                            }).ToList();
                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static List<MasterlistviewModel> getClearanceAgent()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var qryProjectNo = (from C in context.tblClearanceAgentMaster
                                        select new { C.ClearanceAgentId, C.Name }).ToList();
                    if (qryProjectNo.Count > 0)
                    {
                        for (int i = 0; i < qryProjectNo.Count; i++)
                        {
                            List.Add(new MasterlistviewModel()
                            {
                                id = qryProjectNo[i].ClearanceAgentId,
                                name = qryProjectNo[i].Name
                            });
                        }
                    }
                }
                return List;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                return List;
            }
        }
        public static List<AutoCompleteModel> GetAutoCompleteClearanceAgent(string term)
        {
            try
            {

                List<AutoCompleteModel> list = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    list = (from C in context.tblClearanceAgentMaster
                            where C.Name.Contains(term) || C.ClearanceAgentCode.Contains(term)
                            orderby C.Name
                            select new AutoCompleteModel()
                            {
                                value = C.ClearanceAgentId.ToString(),
                                label = C.Name
                            }).ToList();


                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static List<MasterlistviewModel> GetClearanceAgentList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblClearanceAgentMaster
                                 orderby C.Name
                                 where C.Status == "Active"
                                 select new { C.Name, C.ClearanceAgentId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].ClearanceAgentId,
                                name = query[i].Name,
                            });
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> list = new List<MasterlistviewModel>();
                return list;
            }

        }
        public static tblCodeControl GetCLPServiceDetail(int serviceType)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblCodeControl.FirstOrDefault(dup => dup.CodeValAbbr == serviceType && dup.CodeName == "ClearancePaymentServiceType");
                    if (query != null)
                    {
                        return query;
                    }
                    else
                        return new tblCodeControl();
                }

            }
            catch (Exception ex)
            {
                return new tblCodeControl();
            }
        }

        public static List<MasterlistviewModel> GetSettlementTypeList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "SettlementType"
                                 orderby C.CodeValAbbr
                                 select C).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].CodeValAbbr,
                                name = query[i].CodeValDetail
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }

        public static List<BillCommitmentDetailModel> GetCommitmentlistbyId(Int32[] CommitmentId)
        {
            try
            {
                List<BillCommitmentDetailModel> Commit = new List<BillCommitmentDetailModel>();
                using (var context = new IOASDBEntities())
                {

                    Commit = (from U in context.tblCommitment
                              join P in context.tblProject on U.ProjectId equals P.ProjectId
                              join D in context.tblCommitmentDetails on U.CommitmentId equals D.CommitmentId
                              join head in context.tblBudgetHead on D.AllocationHeadId equals head.BudgetHeadId
                              orderby D.ComitmentDetailId descending
                              where CommitmentId.Contains(U.CommitmentId)
                              select new BillCommitmentDetailModel()
                              {
                                  CommitmentId = U.CommitmentId,
                                  CommitmentDetailId = D.ComitmentDetailId,
                                  CommitmentNumber = U.CommitmentNumber,
                                  HeadName = head.HeadName,
                                  ProjectId = U.ProjectId,
                                  ProjectNumber = P.ProjectNumber,
                                  BookedAmount = U.CommitmentAmount,
                                  AvailableAmount = D.BalanceAmount ?? 0,
                              }).ToList();

                }
                return Commit;
            }
            catch (Exception ex)
            {
                List<BillCommitmentDetailModel> Commit = new List<BillCommitmentDetailModel>();
                return Commit;
            }
        }

        public static string getRecoupId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from recoup in context.tblSBICardRecoupment
                             orderby recoup.RecoupmentId descending
                             select recoup).FirstOrDefault();

                if (query != null)
                {
                    var num = query.RecoupmentNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getTempSettlAdvId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from tempadv in context.tblTempAdvanceSettlement
                             orderby tempadv.TempAdvSettlementId descending
                             select tempadv).FirstOrDefault();

                if (query != null)
                {
                    var num = query.TempSettlNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static string getTempAdvId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from tempadv in context.tblTemporaryAdvance
                             orderby tempadv.TemporaryAdvanceId descending
                             select tempadv).FirstOrDefault();

                if (query != null)
                {
                    var num = query.TemporaryAdvanceNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string ValidateTempAdvforSettlement(int TempAdvId)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                var Query = context.tblTempAdvanceSettlement.FirstOrDefault(m => m.TemporaryAdvanceId == TempAdvId);
                if (Query != null)
                {
                    msg = "Settlement has been done already for the selected Temporary Advance. Please select another one";
                }
            }

            return msg;

        }
        public static bool ValidateTempAdvSettlementStatus(int TempAdvsettlId, string type, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTempAdvanceSettlement.FirstOrDefault(m => m.TempAdvSettlementId == TempAdvsettlId && m.TransactionTypeCode == type && m.Status == status);
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
        public static bool ValidateSBICardPjctdtlsOnEdit(int ProjectId, string type)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblSBIPrepaidCardProjectDetails.FirstOrDefault(m => m.ProjectId == ProjectId && m.TransactionTypeCode == type && m.Status == "Open");
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
        public static string ValidateSBICardPjctAddition(int ProjectId, int cardId)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                if (cardId != 0)
                {
                    var cardquery = context.tblSBIECardDetails.FirstOrDefault(m => m.SBIPrepaidCardDetailsId == cardId);
                    if (cardquery == null)
                    {
                        msg = "Card does not exist. Please check again";
                        return msg;
                    }
                    else if (cardquery != null)
                    {
                        var noofpjcts = cardquery.NoofProjectsIncluded;
                        if (noofpjcts >= 5)
                        {
                            msg = "Project cannot be mapped to this card since there are already 5 projects mapped. Please select another project.";
                            return msg;
                        }
                    }
                }
                if (ProjectId != 0)
                {
                    var projectquery = context.tblProject.FirstOrDefault(m => m.ProjectId == ProjectId);
                    if (projectquery != null)
                    {
                        var Query = context.tblSBIPrepaidCardProjectDetails.FirstOrDefault(m => m.ProjectId == ProjectId);
                        if (Query != null)
                        {
                            var pjctcardquery = context.tblSBIECardDetails.FirstOrDefault(m => m.SBIPrepaidCardDetailsId == Query.SBIPrepaidCardDetailsId);
                            var CardNumber = pjctcardquery.SBIPrepaidCardNumber;
                            msg = "Project has already been added to a SBI Prepaid Card - " + CardNumber + ". Please select another project";

                        }
                    }
                    else if (projectquery == null)
                    {
                        msg = "Project does not exist. Please check again";
                        return msg;
                    }
                }
            }

            return msg;

        }

        public static bool ValidateImprestOnEdit(int ImpID, string type)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblIMPUserDetails.FirstOrDefault(m => m.IMPUserDetailsId == ImpID && m.TransactionTypeCode == type && m.Status == "Open");
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
        public static string ValidateImprestonAddition(int PIId)
        {
            string msg = "Valid";
            using (var context = new IOASDBEntities())
            {
                if (PIId != 0)
                {
                    var cardquery = context.tblIMPUserDetails.FirstOrDefault(m => m.PIUserId == PIId);
                    if (cardquery == null)
                    {
                        msg = "Imprest Account does not exist. Please check again";
                        return msg;
                    }

                }
                //if (PIId != 0)
                //{
                //    var projectquery = context.tblProject.FirstOrDefault(m => m.PIName == PIId);
                //    if (projectquery != null)
                //    {
                //        var Query = context.tblImprestPaymentDetails.FirstOrDefault(m => m.PIId == PIId);
                //        if (Query != null)
                //        {
                //            var pjctcardquery = context.tblIMPUserDetails.FirstOrDefault(m => m.IMPUserDetailsId == Query.IMPUserDetailsId);
                //            var CardNumber = pjctcardquery.ImprestCardNumber;
                //            msg = "Project has already been added to a SBI Prepaid Card - " + CardNumber + ". Please select another project";

                //        }
                //    }
                //    else if (projectquery == null)
                //    {
                //        msg = "Project does not exist. Please check again";
                //        return msg;
                //    }
                //}
            }

            return msg;

        }
        public static bool ValidateImprestEnhanceOnEdit(int IMEID, string type)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestPaymentDetails.FirstOrDefault(m => m.ImprestPaymentDetailsId == IMEID && m.TransactionTypeCode == type && m.Status == "Open" && m.Is_ImprestEnhance == true);
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
        public static CoPiDetailsModel GetPIdetailsbyProject(int ProjectId)
        {
            try
            {
                CoPiDetailsModel Detail = new CoPiDetailsModel();
                using (var context = new IOASDBEntities())
                {
                    var Query = (from U in context.vwFacultyStaffDetails
                                 join pro in context.tblProject on U.UserId equals pro.PIName
                                 where pro.ProjectId == ProjectId
                                 select new { U.FirstName, U.DepartmentName, U.UserId }).FirstOrDefault();
                    if (Query != null)
                    {
                        Detail.PIName = Query.FirstName;
                        Detail.PIId = Query.UserId;
                        Detail.PIDepartment = Query.DepartmentName;
                    }
                    return Detail;
                }
            }
            catch (Exception ex)
            {
                CoPiDetailsModel Detail = new CoPiDetailsModel();
                return Detail;
            }
        }
        public static string getImprestcardnumber(int ImprestCardID)
        {
            try
            {
                var Cardnumber = " ";
                var context = new IOASDBEntities();
                var query = (from EC in context.tblIMPUserDetails
                             where EC.IMPUserDetailsId == ImprestCardID
                             select EC).FirstOrDefault();

                if (query != null)
                {
                    Cardnumber = query.ImprestCardNumber;
                    return Cardnumber;
                }
                else
                {
                    return Cardnumber;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<MasterlistviewModel> getBank()
        {
            try
            {

                List<MasterlistviewModel> bank = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from b in context.tblIITMBankMaster
                                 orderby b.BankId
                                 select b).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            bank.Add(new MasterlistviewModel()
                            {
                                id = query[i].BankId,
                                name = query[i].BankName,
                            });
                        }
                    }

                }

                return bank;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string getIMPRecoupId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from recoup in context.tblImprestRecoupment
                             orderby recoup.RecoupmentId descending
                             select recoup).FirstOrDefault();

                if (query != null)
                {
                    var num = query.RecoupmentNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static bool ValidateImprestBillRecoupStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestBillRecoupment.FirstOrDefault(m => m.ImprestBillRecoupId == id && m.Status == status);
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
        public static bool ValidateImprestBillStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblImprestRecoupment.FirstOrDefault(m => m.RecoupmentId == id && m.Status == status);
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
        public static string GetImprestBillRecoupNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblImprestBillRecoupment
                               orderby b.ImprestBillRecoupId descending
                               select b.ImprestBillRecoupNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "IBR" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "IBR" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getImprestId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from card in context.tblIMPUserDetails
                             orderby card.IMPUserDetailsId descending
                             select card).FirstOrDefault();

                if (query != null)
                {
                    var num = query.ImprestNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getImprestEnhanceId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from pjct in context.tblImprestPaymentDetails
                             where pjct.Is_ImprestEnhance == true
                             orderby pjct.ImprestPaymentDetailsId descending
                             select pjct).FirstOrDefault();

                if (query != null)
                {
                    var num = query.ImprestEnhanceNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }
            }
            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static bool CheckVendorIsInterState(int ID)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblVendorMaster
                                 where C.VendorId == ID
                                 select C.StateCode).FirstOrDefault();
                    if (query != null)
                    {
                        if (query.Value == 33)
                            return false;
                        else
                            return true;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public static bool CheckClearanceAgentIsInterState(int ID)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblClearanceAgentMaster
                                 where C.ClearanceAgentId == ID
                                 select C.StateCode).FirstOrDefault();
                    if (query != null)
                    {
                        if (query.Value == 33)
                            return false;
                        else
                            return true;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public static List<MasterlistviewModel> GetTdsList()
        {
            try
            {
                List<MasterlistviewModel> tds = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from T in context.tblTDSMaster
                                 orderby T.NatureOfIncome
                                 select new { T.Section, T.TdsMasterId }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            tds.Add(new MasterlistviewModel()
                            {
                                id = query[i].TdsMasterId,
                                name = query[i].Section
                            });
                        }

                    }
                }
                return tds;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> tds = new List<MasterlistviewModel>();
                return tds;
            }
        }

        public static string getInvoiceId()
        {
            try
            {
                // var lastinvoiceid = 0;
                var seqnum = 0;
                var context = new IOASDBEntities();
                var Invoicequery = (from invoice in context.tblProjectInvoice
                                    orderby invoice.InvoiceId descending
                                    select invoice.InvoiceNumber).FirstOrDefault();

                if (Invoicequery != null)
                {
                    //var invoiceid = Invoicequery.InvoiceId;
                    //lastinvoiceid = invoiceid + 1;
                    //seqnum = lastinvoiceid.ToString("D6");
                    //return seqnum;
                    var value = Invoicequery.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    seqnum = Convert.ToInt32(number);
                    seqnum += 1;
                    return seqnum.ToString("000000");
                }
                else
                {
                    return seqnum.ToString();
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getEmployeeid(int PIid, string Category)
        {
            try
            {
                var facultycode = " ";
                var context = new IOASDBEntities();
                var query = (from user in context.vwCombineStaffDetails
                             where user.ID == PIid && user.Category == Category
                             select user).FirstOrDefault();

                if (query != null)
                {
                    facultycode = query.EmployeeId;
                    return facultycode;
                }
                else
                {
                    return facultycode;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getempid(int Id)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = context.vwFacultyStaffDetails.Where(M => M.UserId == Id).Select(m => m.EmployeeId).FirstOrDefault();
                    return qry;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool ValidatePartTimePaymentOnEdit(int internId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblPartTimePayment.FirstOrDefault(m => m.PartTimePaymentId == internId && m.Status == "Open");
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
        public static List<MasterlistviewModel> GetStudentDepartmentList()
        {
            try
            {
                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.vwStudentDetails
                                 select U).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Title.Add(new MasterlistviewModel()
                            {
                                name = query[i].RollNumber,
                                code = query[i].DepartmentName,
                            });
                        }
                    }

                }

                return Title;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static string getParttimeinternId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblPartTimePayment
                             orderby intern.PartTimePaymentId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var num = query.PartTimePaymentNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static bool ValidateProjectFundTransferStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblProjectTransfer.FirstOrDefault(m => m.ProjectTransferId == id && m.Status == status);
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
        public static bool ValidateContraStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblContra.FirstOrDefault(m => m.ContraId == id && m.Status == status);
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
        public static bool ValidateGeneralVoucherStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblGeneralVoucher.FirstOrDefault(m => m.GeneralVoucherId == id && m.Status == status);
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
        public static bool ValidateReceiptStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblReceipt.FirstOrDefault(m => m.ReceiptId == id && m.Status == status);
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
        public static List<TransactionAndTaxesModel> GetTransactionType()
        {
            List<TransactionAndTaxesModel> transtype = new List<TransactionAndTaxesModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblTransactionTypeCode
                             orderby C.TransactionType
                             select new { C.TransactionType, C.TransactionTypeCode }).ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        transtype.Add(new TransactionAndTaxesModel()
                        {
                            TransactionType = query[i].TransactionType,
                            TransactionTypeId = query[i].TransactionTypeCode,
                        });
                    }
                }
            }
            return transtype;
        }
        public static string gettransactioncode(string typecode)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTransactionTypeCode.Where(m => m.TransactionTypeCode == typecode).Select(m => m.TransactionType).FirstOrDefault();
                    return query;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<TransactionAndTaxesModel> GetSubCode()
        {
            List<TransactionAndTaxesModel> transtype = new List<TransactionAndTaxesModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblTransactionDefinition
                             orderby C.SubCode
                             select new { C.SubCode }).Distinct().ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        transtype.Add(new TransactionAndTaxesModel()
                        {
                            SubCodeId = Convert.ToInt32(query[i].SubCode)

                        });
                    }
                }
            }
            return transtype;
        }
        public static List<TransactionAndTaxesModel> GetAccountGroupList()
        {
            List<TransactionAndTaxesModel> transtype = new List<TransactionAndTaxesModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from H in context.tblAccountHead
                             join G in context.tblAccountGroup on H.AccountGroupId equals G.AccountGroupId
                             group G by G.AccountGroup into grp
                             select new
                             {
                                 AccountGroup = grp.Key,
                                 AccountGroupId = grp.Select(m => m.AccountGroupId).FirstOrDefault()
                             }).ToList();

                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        transtype.Add(new TransactionAndTaxesModel()
                        {
                            GroupId = Convert.ToInt32(query[i].AccountGroupId),
                            Group = query[i].AccountGroup,
                        });
                    }
                }
            }
            return transtype;
        }

        public static List<TransactionAndTaxesModel> GetAccountHeadList()
        {
            List<TransactionAndTaxesModel> transtype = new List<TransactionAndTaxesModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from H in context.tblAccountHead
                             select new
                             {
                                 H.AccountHead,
                                 H.AccountHeadId
                             }).Distinct().ToList();

                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        transtype.Add(new TransactionAndTaxesModel()
                        {
                            HeadId = Convert.ToInt32(query[i].AccountHeadId),
                            Head = query[i].AccountHead,
                        });
                    }
                }
            }
            return transtype;
        }
        public static List<TransactionAndTaxesModel> LoadGrpWiseHeadList(int accgrp)
        {
            List<TransactionAndTaxesModel> transtype = new List<TransactionAndTaxesModel>();
            try
            {
                transtype.Add(new TransactionAndTaxesModel()
                {
                    HeadId = 0,
                    Head = "Select Head"
                });
                using (var context = new IOASDBEntities())
                {
                    if (accgrp != 0)
                    {
                        var query = (from H in context.tblAccountHead
                                     where H.AccountGroupId == accgrp
                                     select new
                                     {
                                         H.AccountHead,
                                         H.AccountHeadId
                                     }).Distinct().ToList();

                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                transtype.Add(new TransactionAndTaxesModel()
                                {
                                    HeadId = Convert.ToInt32(query[i].AccountHeadId),
                                    Head = query[i].AccountHead,
                                });
                            }
                        }
                    }
                }
                return transtype;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<TransactionAndTaxesModel> LoadSubCodeList(string transtype)
        {
            List<TransactionAndTaxesModel> model = new List<TransactionAndTaxesModel>();
            try
            {
                model.Add(new TransactionAndTaxesModel()
                {
                    SubCodeId = 0,
                    SubCode = "Select SubCode"
                });
                using (var context = new IOASDBEntities())
                {
                    if (transtype != null)
                    {
                        var query = (from H in context.tblTransactionDefinition
                                     where H.TransactionTypeCode == transtype
                                     select new
                                     {
                                         H.SubCode
                                     }).Distinct().ToList();

                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                model.Add(new TransactionAndTaxesModel()
                                {
                                    SubCodeId = Convert.ToInt32(query[i].SubCode),
                                    SubCode = query[i].SubCode,
                                });
                            }
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<TransactionAndTaxesModel> GetTransactionDetailsList(string transaction, string subcode)
        {
            List<TransactionAndTaxesModel> model = new List<TransactionAndTaxesModel>();
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = (from T in context.tblTransactionDefinition
                               join H in context.tblAccountHead on T.AccountHeadId equals H.AccountHeadId
                               join G in context.tblAccountGroup on H.AccountGroupId equals G.AccountGroupId
                               where (T.TransactionTypeCode == transaction && T.SubCode == subcode)
                               select new
                               {
                                   G.AccountGroup,
                                   G.AccountGroupId,
                                   H.AccountHead,
                                   H.AccountHeadId,
                                   T.TransactionType,
                                   T.IsJV_f
                               }).ToList();
                    if (qry.Count > 0)
                    {
                        for (int i = 0; i < qry.Count; i++)
                        {
                            model.Add(new TransactionAndTaxesModel()
                            {
                                GroupId = Convert.ToInt32(qry[i].AccountGroupId),
                                Group = qry[i].AccountGroup,
                                Head = qry[i].AccountHead,
                                HeadId = Convert.ToInt32(qry[i].AccountHeadId),
                                TransactionType = qry[i].TransactionType,
                                ISJV = Convert.ToBoolean(qry[i].IsJV_f)

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //List<TransactionAndTaxesModel> model = new List<TransactionAndTaxesModel>();
            }
            return model;
        }
        public static List<TransactionAndTaxesModel> GetDeductionCategory()
        {
            List<TransactionAndTaxesModel> model = new List<TransactionAndTaxesModel>();
            try
            {

                using (var context = new IOASDBEntities())
                {

                    var query = (from H in context.tblCodeControl
                                 where H.CodeName == "DeductionCategory"
                                 select new
                                 {
                                     H.CodeValDetail,
                                     H.CodeValAbbr
                                 }).Distinct().ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            model.Add(new TransactionAndTaxesModel()
                            {
                                CategoryId = Convert.ToInt32(query[i].CodeValAbbr),
                                Category = query[i].CodeValDetail,
                            });
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static decimal GetAvailableAmtForCreditNote(int id, bool isEditMode)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    decimal invoiceAmt = 0;
                    decimal previousCreditNote = 0;
                    int invoiceId = 0;
                    if (!isEditMode)
                    {
                        var invquery = (from inv in context.tblProjectInvoice
                                        where inv.InvoiceId == id && inv.Status == "Active"
                                        select inv).FirstOrDefault();
                        if (invquery == null)
                            return 0;
                        invoiceAmt = invquery.TotalInvoiceValue ?? 0;
                        invoiceId = id;
                        previousCreditNote = context.tblCreditNote.Where(m => m.InvoiceId == id && m.Status != "InActive").Sum(m => m.TotalCreditAmount) ?? 0;
                    }
                    else
                    {
                        var invquery = (from cn in context.tblCreditNote
                                        join inv in context.tblProjectInvoice on cn.InvoiceId equals inv.InvoiceId
                                        where cn.CreditNoteId == id
                                        select new { inv, cn }).FirstOrDefault();
                        if (invquery == null)
                            return 0;
                        invoiceAmt = invquery.inv.TotalInvoiceValue ?? 0;
                        invoiceId = invquery.inv.InvoiceId;
                        previousCreditNote = context.tblCreditNote.Where(m => m.InvoiceId == invoiceId && m.Status != "InActive" && m.CreditNoteId != id).Sum(m => m.TotalCreditAmount) ?? 0;
                    }

                    decimal receiptAmt = context.tblReceipt.Where(r => r.InvoiceId == invoiceId && r.Status != "InActive")
                                         .Sum(m => m.ReceiptAmount) ?? 0;
                    decimal withOutTx = invoiceAmt - previousCreditNote - receiptAmt;
                    return withOutTx;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static bool ValidateAdhocPaymentOnEdit(int adhocId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAdhocPayment.FirstOrDefault(m => m.AdhocPaymentId == adhocId && m.Status == "Open");
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
        public static string getAdhocPaymentId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblAdhocPayment
                             orderby intern.AdhocPaymentId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var num = query.AdhocPaymentNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<MasterlistviewModel> gettranstypecode(int Paymenttype)
        {
            try
            {
                List<MasterlistviewModel> transtypcode = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "PaymentType" && C.CodeValAbbr == Paymenttype
                                 select C).FirstOrDefault();
                    if (query != null)
                    {
                        transtypcode.Add(new MasterlistviewModel()
                        {
                            id = query.CodeValAbbr,
                            name = query.CodeDescription
                        });
                    }

                }

                return transtypcode;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static List<MasterlistviewModel> GetFundingBody(int ProjectId)
        {
            List<MasterlistviewModel> List = new List<MasterlistviewModel>();
            try
            {
                List.Add(new MasterlistviewModel()
                {
                    id = 0,
                    name = "NA"
                });
                if (ProjectId > 0)
                {
                    using (var context = new IOASDBEntities())
                    {
                        var Query = (from C in context.tblProjectFundingBody where C.ProjectId == ProjectId select C.IndProjectFundingGovtBody).ToList();
                        if (Query.Count > 0)
                        {
                            for (int i = 0; i < Query.Count; i++)
                            {
                                int NonGovt = Query[i] ?? 0;
                                var qryName = (from c in context.tblCodeControl where c.CodeName == "Indfundgovtbody" && c.CodeValAbbr == NonGovt select c.CodeValDetail).FirstOrDefault();
                                if (qryName != "")
                                {
                                    List.Add(new MasterlistviewModel()
                                    {
                                        id = NonGovt,
                                        name = qryName
                                    });
                                }
                            }
                        }
                        var QueryNon = (from C in context.tblProjectFundingBody where C.ProjectId == ProjectId select C.IndProjectFundingNonGovtBody).ToList();
                        if (QueryNon.Count > 0)
                        {
                            for (int i = 0; i < QueryNon.Count; i++)
                            {
                                int NonGovt = QueryNon[i] ?? 0;
                                var qryName = (from c in context.tblCodeControl where c.CodeName == "Indfundnongovtbody" && c.CodeValAbbr == NonGovt select c.CodeValDetail).FirstOrDefault();
                                if (qryName != null)
                                {
                                    List.Add(new MasterlistviewModel()
                                    {
                                        id = NonGovt,
                                        name = qryName
                                    });
                                }
                            }
                        }

                    }
                }
                return List;
            }
            catch (Exception ex)
            {

                return List;
            }
        }
        public static string GetTypeOfServiceName(int id)
        {
            try
            {

                string name = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTaxMaster.FirstOrDefault(m => m.TaxMasterId == id);
                    if (query != null)
                    {
                        name = query.TaxCode + "-" + query.ServiceType;
                    }
                }

                return name;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        public static bool TSTBillIsReceipt(int billId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTravelBill.FirstOrDefault(m => m.TravelBillId == billId && m.TransactionTypeCode == "TST");
                    if (query != null && query.BalanceinAdvance > 0)
                        isValid = true;
                }
                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static decimal GetAccountHeadBalance(int hdId)
        {
            try
            {
                decimal bal = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblHeadOpeningBalance.FirstOrDefault(m => m.AccountHeadId == hdId && m.Status == "Active");
                    if (query != null)
                        bal = query.OpeningBalance ?? 0;
                    var querySum = context.tblBOASummary.FirstOrDefault(m => m.AccountHeadId == hdId);
                    if (querySum != null)
                        bal = bal + (querySum.Amount ?? 0);
                }
                return bal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MasterlistviewModel> GetTDS()
        {
            List<MasterlistviewModel> tds = new List<MasterlistviewModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblCodeControl
                             where (C.CodeName == "TDS")
                             orderby C.CodeValAbbr
                             select new { C.CodeValAbbr, C.CodeValDetail }).Distinct().ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        tds.Add(new MasterlistviewModel()
                        {
                            id = query[i].CodeValAbbr,
                            name = query[i].CodeValDetail

                        });
                    }
                }
            }
            return tds;
        }
        public static List<MasterlistviewModel> GetReceivedFrom()
        {
            List<MasterlistviewModel> tds = new List<MasterlistviewModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblCodeControl
                             where (C.CodeName == "RequestReceivedFrom")
                             orderby C.CodeValAbbr
                             select new { C.CodeValAbbr, C.CodeValDetail }).Distinct().ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        tds.Add(new MasterlistviewModel()
                        {
                            id = query[i].CodeValAbbr,
                            name = query[i].CodeValDetail

                        });
                    }
                }
            }
            return tds;
        }
        public static bool ValidateDistributionOnEdit(int distributionId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblDistribution.FirstOrDefault(m => m.DistributionId == distributionId && m.TransactionTypeCode == "DIS" && m.Status == "Open");
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
        public static string getDistributionId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblDistribution
                             orderby intern.DistributionId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var num = query.DistributionNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<AutoCompleteModel> GetAutoCompleteProjects(string term)
        {
            try
            {
                List<AutoCompleteModel> PI = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    var schemequery = (from C in context.tblSchemes
                                       where C.SchemeCode == "IC" || C.SchemeCode == "RB" || C.SchemeCode == "RC"
                                       select C).ToList();
                    var query = (from C in context.tblProject
                                 join S in context.tblSchemes on C.ConsultancyFundingCategory equals S.SchemeId
                                 where (string.IsNullOrEmpty(term) || C.ProjectNumber.Contains(term))
                                 && C.ProjectType == 2 && (C.ConsultancyFundingCategory == 5 || C.ConsultancyFundingCategory == 7 || C.ConsultancyFundingCategory == 8)
                                 //join ins in context.tblInstituteMaster on C.InstituteId equals ins.InstituteId
                                 //where (C.RoleId == 7)
                                 orderby C.ProjectId
                                 select new { C.ProjectNumber, C.ProjectId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            PI.Add(new AutoCompleteModel()
                            {
                                value = query[i].ProjectId.ToString(),
                                label = query[i].ProjectNumber,
                            });
                        }
                    }
                }

                return PI;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static List<MasterlistviewModel> getProjectidbynumber(string Projectnumber)
        {
            try
            {
                List<MasterlistviewModel> Projectid = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 where C.ProjectNumber == Projectnumber
                                 select new { C.ProjectTitle, C.ProjectId }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Projectid.Add(new MasterlistviewModel()
                            {
                                id = query[i].ProjectId,
                                name = query[i].ProjectTitle,
                            });
                        }
                    }
                    else
                    {
                        Projectid.Add(new MasterlistviewModel()
                        {
                            id = 0,
                            name = "",
                        });
                    }
                }

                return Projectid;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static List<AutoCompleteModel> GetAutoCompleteStaffList(string term)
        {
            try
            {

                List<AutoCompleteModel> staf = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    staf = (from C in context.vwCombineStaffDetails
                            where (string.IsNullOrEmpty(term) || C.Name.Contains(term) || C.EmployeeId.Contains(term))
                            && C.Category == "Staff"
                            orderby C.Name
                            select new AutoCompleteModel()
                            {
                                value = C.ID.ToString(),
                                label = C.Name + " - " + C.EmployeeId,
                            }).ToList();

                }

                return staf;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }

        public static List<AutoCompleteModel> GetAutoCompleteProfList(string term)
        {
            try
            {

                List<AutoCompleteModel> staf = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    staf = (from C in context.vwCombineStaffDetails
                            where (string.IsNullOrEmpty(term) || C.Name.Contains(term) || C.EmployeeId.Contains(term))
                            && C.Category == "Professor"
                            orderby C.Name
                            select new AutoCompleteModel()
                            {
                                value = C.ID.ToString(),
                                label = C.Name + " - " + C.EmployeeId
                            }).ToList();

                }

                return staf;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static List<DistributionOverheadListModel> getInstituteOHPercentage()
        {
            try
            {
                List<DistributionOverheadListModel> ohpercent = new List<DistributionOverheadListModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblCodeControl
                                 where C.CodeName == "InstituteOverheadPercent"
                                 select C).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            ohpercent.Add(new DistributionOverheadListModel()
                            {
                                OverheadtypeId = query[i].CodeValAbbr,
                                Overheadtypename = query[i].CodeDescription,
                                OverheadPercentage = Convert.ToDecimal(query[i].CodeValDetail),
                            });
                        }
                    }

                }
                return ohpercent;

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string gethonorid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblHonororium
                             orderby intern.HonororiumId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var honid = query.HonororiumId;
                    currhoid = honid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<MasterlistviewModel> GetOH()
        {
            List<MasterlistviewModel> tds = new List<MasterlistviewModel>();
            using (var context = new IOASDBEntities())
            {
                var query = (from C in context.tblCodeControl
                             where (C.CodeName == "OH")
                             orderby C.CodeValAbbr
                             select new { C.CodeValAbbr, C.CodeValDetail }).Distinct().ToList();
                if (query.Count > 0)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        tds.Add(new MasterlistviewModel()
                        {
                            id = query[i].CodeValAbbr,
                            name = query[i].CodeValDetail

                        });
                    }
                }
            }
            return tds;
        }
        public static bool ValidateHonororiumOnEdit(int honorid)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblHonororium.FirstOrDefault(m => m.HonororiumId == honorid && m.Status == "Open");
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
        public static string getpayeetype(int PayeeType)
        {
            var payeename = "";
            if (PayeeType == 1)
            {
                payeename = "PI";
            }
            else if (PayeeType == 2)
            {
                payeename = "Student";
            }
            else
            {
                payeename = "Others";
            }
            return payeename;
        }
        public static bool ApprovalForHonororium(int HonorId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblHonororium.FirstOrDefault(m => m.HonororiumId == HonorId);
                    if (query.Status == "Open")
                    {
                        query.Status = "Approval Pending";
                        query.UPTD_By = LoggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ApprovalPendingForHonororium(int HonorId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblHonororium.FirstOrDefault(m => m.HonororiumId == HonorId);
                    if (query.Status == "Approval Pending")
                    {
                        query.Status = "Approved";
                        query.UPTD_By = LoggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static string getSBICardId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from card in context.tblSBIPrepaidCardProjectDetails
                             orderby card.SBIECardProjectDetailsId descending
                             select card).FirstOrDefault();

                if (query != null)
                {
                    var num = query.SBIPrepaidProjectDetailNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static List<MasterlistviewModel> GetBankAccountHeadList(bool excludingImprest = true)
        {
            try
            {
                List<MasterlistviewModel> headList = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    headList = (from ah in context.tblAccountHead
                                orderby ah.AccountHead
                                where (!excludingImprest || (excludingImprest == true && ah.AccountGroupId != 61))
                                && ah.Bank_f == true
                                select new MasterlistviewModel()
                                {
                                    id = ah.AccountHeadId,
                                    name = ah.AccountHead
                                }).ToList();

                }
                return headList;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static List<AutoCompleteModel> GetAutoCompleteProjectStaffList(string term)
        {
            try
            {

                List<AutoCompleteModel> staf = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    staf = (from C in context.vwCombineStaffDetails
                            where (string.IsNullOrEmpty(term) || C.Name.Contains(term) || C.EmployeeId.Contains(term))
                            && C.Category == "Project Staff"
                            orderby C.Name
                            select new AutoCompleteModel()
                            {
                                value = C.ID.ToString(),
                                label = C.Name + " - " + C.EmployeeId,
                            }).ToList();

                }

                return staf;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }
        public static string getGSTOffsetid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblGSTOffset
                             orderby intern.GSTOffsetid descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var gstid = query.GSTOffsetid;
                    currhoid = gstid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static bool ApprovalForGSTOffset(int GSTOffsetId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblGSTOffset.FirstOrDefault(m => m.GSTOffsetid == GSTOffsetId);
                    if (query.Status == "Open")
                    {
                        query.Status = "Approval Pending";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ApprovalPendingForGSTOffset(int GSTOffsetId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblGSTOffset.FirstOrDefault(m => m.GSTOffsetid == GSTOffsetId);
                    if (query.Status == "Approval Pending")
                    {
                        query.Status = "Approved";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ValidateGSTOffsetOnEdit(int GSTOffsetId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblGSTOffset.FirstOrDefault(m => m.GSTOffsetid == GSTOffsetId && m.Status == "Open");
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
        public static List<MasterlistviewModel> GetFunctionStatus(int funId)
        {

            try
            {
                List<MasterlistviewModel> status = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblFunctionStatus.Where(x => x.FunctionId == funId).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            status.Add(new MasterlistviewModel()
                            {
                                id = query[i].FunctionId,
                                name = query[i].Status
                            });
                        }
                    }

                }
                return status;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> status = new List<MasterlistviewModel>();
                return status;
            }
        }
        public static string GetAccountHeadName(int headId)
        {
            try
            {
                string name = "";
                using (var context = new IOASDBEntities())
                {
                    var qryAN = context.tblAccountHead.FirstOrDefault(m => m.AccountHeadId == headId);
                    if (qryAN != null)
                    {
                        name = qryAN.AccountHead;
                    }
                }
                return name;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string GetNegativeBalanceNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblNegativeBalance
                               orderby b.NegativeBalanceId descending
                               select b.NegativeBalanceNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "NBL" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "NBL" + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool ValidateNegativeBalanceStatus(int id, string status)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblNegativeBalance.FirstOrDefault(m => m.NegativeBalanceId == id && m.Status == status);
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
        public static List<AutoCompleteModel> GetProjectNumber(string term, int? pType = null)
        {
            try
            {

                List<AutoCompleteModel> pro = new List<AutoCompleteModel>();

                using (var context = new IOASDBEntities())
                {
                    pro = (from C in context.tblProject
                           where C.ProjectNumber.Contains(term)
                             && (pType == null || C.ProjectType == pType)
                           orderby C.ProjectNumber
                           select new AutoCompleteModel()
                           {
                               value = C.ProjectId.ToString(),
                               label = C.ProjectNumber
                           }).ToList();

                }

                return pro;
            }
            catch (Exception ex)
            {
                return new List<AutoCompleteModel>();
            }

        }

        public static List<FellowShipModel> GetCommitmentNo(int projid)
        {
            List<FellowShipModel> fellow = new List<FellowShipModel>();
            try
            {
                fellow.Add(new FellowShipModel()
                {
                    ViewBagCommitNo = "Select CommitmentNumber"
                });
                using (var context = new IOASDBEntities())
                {
                    if (projid != 0)
                    {
                        var query = (from H in context.tblProject
                                     join P in context.tblCommitment on H.ProjectId equals P.ProjectId
                                     where H.ProjectId == projid && P.CommitmentBalance > 0
                                     select new
                                     {
                                         P.CommitmentNumber,


                                     }).ToList();

                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                fellow.Add(new FellowShipModel()
                                {
                                    ViewBagCommitNo = query[i].CommitmentNumber,
                                });
                            }
                        }
                    }
                }
                return fellow;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static FellowShipModel GetAvailableBalance(string commitmentno)
        {
            FellowShipModel fellow = new FellowShipModel();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    if (commitmentno != null)
                    {
                        fellow = (from H in context.tblCommitment
                                  where H.CommitmentNumber == commitmentno
                                  select new FellowShipModel()
                                  {
                                      AvailableBalance = H.CommitmentBalance,
                                  }).FirstOrDefault();

                    }
                }
                return fellow;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string getFellowShipid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblFellowShip
                             orderby intern.FellowShipId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var tdsid = query.FellowShipId;
                    currhoid = tdsid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static bool ApprovalForFellowShip(int FellowId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblFellowShip.FirstOrDefault(m => m.FellowShipId == FellowId);
                    if (query.Status == "Open" || query.Status == "Revised")
                    {
                        query.Status = "Approval Pending";
                        query.UPTD_By = LoggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ApprovalPendingForFellowShip(int FellowId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblFellowShip.FirstOrDefault(m => m.FellowShipId == FellowId);
                    if (query.Status == "Approval Pending")
                    {
                        query.Status = "Active";
                        query.UPTD_By = LoggedInUser;
                        query.UPTD_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ValidateFellowShipOnEdit(int FellowId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblFellowShip.FirstOrDefault(m => m.FellowShipId == FellowId && (m.Status == "Open"));
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
        public static string getpayeename(int Payeeid)
        {
            try
            {
                var payname = "";
                List<FellowShipModel> model = new List<FellowShipModel>();
                using (var context = new IOASDBEntities())
                {

                    var query = (from c in context.vwFacultyStaffDetails
                                 where (c.UserId == Payeeid)
                                 select new
                                 {
                                     c.FirstName
                                 }).FirstOrDefault();
                    payname = query.FirstName;


                }
                return payname;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<TDSPaymentModel> GetSection()
        {
            List<TDSPaymentModel> tds = new List<TDSPaymentModel>();
            try
            {
                tds.Add(new TDSPaymentModel()
                {
                    HeadId = 0,
                    Head = "Select Section"
                });
                using (var context = new IOASDBEntities())
                {
                    int[] id = { 39, 40, 41, 42, 43 };
                    var query = (from H in context.tblAccountHead
                                 where id.Contains(H.AccountHeadId)
                                 select new
                                 {
                                     H.AccountHead,
                                     H.AccountHeadId
                                 }).Distinct().ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            tds.Add(new TDSPaymentModel()
                            {
                                HeadId = Convert.ToInt32(query[i].AccountHeadId),
                                Head = query[i].AccountHead,
                            });
                        }
                    }

                }
                return tds;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string getTDSPaymentid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblTDSPayment
                             orderby intern.tblTDSPaymentId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var tdsid = query.tblTDSPaymentId;
                    currhoid = tdsid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getcategory(int category)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblCodeControl.Where(m => m.CodeName == "TDSPaymentCategory" && (m.CodeValAbbr == category)).Select(m => m.CodeValDetail).FirstOrDefault();
                    return query;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getsection(int section)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAccountHead.Where(m => m.AccountHeadId == section).Select(m => m.AccountHead).FirstOrDefault();
                    return query;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool ApprovalForTDSPayment(int TDSPaymentId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTDSPayment.FirstOrDefault(m => m.tblTDSPaymentId == TDSPaymentId);
                    if (query.Status == "Open")
                    {
                        query.Status = "Approval Pending";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ApprovalPendingForTDSPayment(int TDSPaymentId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTDSPayment.FirstOrDefault(m => m.tblTDSPaymentId == TDSPaymentId);
                    if (query.Status == "Approval Pending")
                    {
                        query.Status = "Approved";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ValidateTDSPaymentOnEdit(int TDSPaymentId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblTDSPayment.FirstOrDefault(m => m.tblTDSPaymentId == TDSPaymentId && (m.Status == "Open" || m.Status == "Approved"));
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
        public static List<TDSPaymentModel> Section()
        {
            try
            {
                List<TDSPaymentModel> tds = new List<TDSPaymentModel>();
                using (var context = new IOASDBEntities())
                {
                    int[] id = { 39, 40, 41, 42, 43 };
                    var query = (from H in context.tblAccountHead
                                 where id.Contains(H.AccountHeadId)
                                 select new
                                 {
                                     H.AccountHead,
                                     H.AccountHeadId
                                 }).Distinct().ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            tds.Add(new TDSPaymentModel()
                            {
                                HeadId = query[i].AccountHeadId,
                                Head = query[i].AccountHead
                            });
                        }

                    }
                }
                return tds;
            }
            catch (Exception ex)
            {
                List<TDSPaymentModel> tds = new List<TDSPaymentModel>();
                return tds;
            }
        }
        public static List<MasterlistviewModel> getForeignSupplierList()
        {
            try
            {

                List<MasterlistviewModel> list = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from V in context.tblVendorMaster
                                 join C in context.tblCountries on V.Country equals C.countryID
                                 where V.Country != 128
                                 orderby V.VendorId
                                 select new { V, C }).ToList();

                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            list.Add(new MasterlistviewModel()
                            {
                                id = query[i].V.VendorId,
                                name = query[i].V.Name + " - " + query[i].C.countryName + " - " + query[i].V.VendorCode
                            });
                        }
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }
        }
        public static bool ValidateForeignRemitOnEdit(int foreignRemitId)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblForeignRemittance.FirstOrDefault(m => m.ForeignRemitId == foreignRemitId && m.TransactionTypeCode == "FRM" && m.Status == "Open");
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
        public static string getForeignRemitId()
        {
            try
            {
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from t in context.tblForeignRemittance
                             orderby t.ForeignRemitId descending
                             select t).FirstOrDefault();

                if (query != null)
                {
                    var num = query.ForeignRemitNumber;
                    var value = num.Split('/').Last();
                    string number = Regex.Replace(value, @"\D", "");
                    var lastnumber = Convert.ToInt32(number);
                    lastnumber += 1;
                    seqnum = lastnumber.ToString("000000");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static decimal getBoaTransactionAmount(int id)
        {
            try
            {
                var context = new IOASDBEntities();
                var query = context.tblBOATransaction.Where(m => m.BOATransactionId == id).Select(m => m.Amount).FirstOrDefault();
                if (query != null)
                {
                    return Convert.ToDecimal(query);
                }
                else
                {
                    return Convert.ToDecimal(query);
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }

        public static List<AutoCompleteModel> GetStaffname(string term)
        {
            List<AutoCompleteModel> ManDay = new List<AutoCompleteModel>();
            try
            {

                using (var context = new IOASDBEntities())
                {

                    ManDay = (from H in context.vwCombineStaffDetails

                              where H.Name.Contains(term) && H.Category == "Staff"
                              select new AutoCompleteModel()
                              {
                                  label = H.EmployeeId + "-" + H.Name,
                                  value = H.ID.ToString(),
                                  desc = H.DepartmentName,
                                  icon = H.DepartmentCode
                              }).ToList();



                }
                return ManDay;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static string getmandayid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblManDay
                             orderby intern.ManDayId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var manid = query.ManDayId;
                    currhoid = manid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getdepname(string deptcode)
        {
            try
            {
                string dep = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.vwCombineStaffDetails
                             where intern.DepartmentCode == deptcode
                             select new { intern.DepartmentName }).FirstOrDefault();

                if (query != null)
                {
                    dep = query.DepartmentName;
                    return dep;
                }
                else
                {
                    return dep;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static List<ManDayListmodel> ValidateManDay(int projid, int days, DateTime monyr, int mandayid = 0)
        {
            List<ManDayListmodel> ManDay = new List<ManDayListmodel>();
            try
            {

                using (var context = new IOASDBEntities())
                {
                    if (projid != 0)
                    {

                        var query = (from H in context.tblManDayDetails
                                     where ((H.MonthYear == monyr) && (H.StaffId == projid) && (!(H.ManDayId == mandayid) || (mandayid == 0)))
                                     select new
                                     {
                                         H.NoofDays
                                     }).ToList();

                        if (query.Count == 0)
                        {
                            ManDay.Add(new ManDayListmodel()
                            {
                                errmsgid = 1
                            });
                        }
                        if (query.Count > 0)
                        {

                            for (int i = 0; i < query.Count; i++)
                            {
                                ManDay.Add(new ManDayListmodel()
                                {
                                    NoOfDays = query[i].NoofDays
                                });
                            }




                        }

                    }
                }
                return ManDay;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static bool ApprovalForManDay(int Mandayid, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblManDay.FirstOrDefault(m => m.ManDayId == Mandayid);
                    if (query.Status == "Open")
                    {
                        query.Status = "Approval Pending";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ApprovalPendingForManDay(int Mandayid, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblManDay.FirstOrDefault(m => m.ManDayId == Mandayid);
                    if (query.Status == "Approval Pending")
                    {
                        query.Status = "Approved";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ValidateManDayOnEdit(int Mandayid)
        {
            try
            {
                bool isValid = false;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblManDay.FirstOrDefault(m => m.ManDayId == Mandayid && m.Status == "Open");
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
        public static List<MasterlistviewModel> GetAllYearMonths()
        {
            List<MasterlistviewModel> List = new List<MasterlistviewModel>();
            int Year = DateTime.Now.Year;
            for (int i = 1; i <= 12; i++)
            {
                List.Add(new MasterlistviewModel()
                {
                    id = i,
                    name = DateTimeFormatInfo.CurrentInfo.GetMonthName(i).Substring(0, 3) + " - " + Year.ToString()
                });

            }
            return List;
        }

        public static string getAgencyEmployeeName(string EmployeeID)
        {
            try
            {
                var Name = "";
                using (var context = new IOASDBEntities())
                {
                    var query = context.vwAppointmentMaster.FirstOrDefault(m => m.EmployeeId == EmployeeID).EmployeeName;
                    if (query != null)
                        Name = query;
                }
                return Name;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public static Tuple<string, int> getAgencySalarySequenceNumber()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblAgencySalary
                               orderby b.AgencySalaryId descending
                               select b.PaymentNo).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return Tuple.Create("SLA" + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000"), seqnum);
                    }
                    else
                    {
                        return Tuple.Create("SLA" + "/" + GetCurrentFinYear() + "/" + "000001", 1);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string getPaymentNo(int AgencyID)
        {
            try
            {
                string payNo = "";
                using (var context = new IOASDBEntities())
                {
                    var Month = DateTime.Now.Month;
                    var qryAgency = (from b in context.tblAgencySalary
                                     where b.AgencySalaryId == AgencyID
                                     select b).FirstOrDefault();
                    if (qryAgency != null)
                    {
                        payNo = qryAgency.PaymentNo;
                    }
                }
                return payNo;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static decimal getSumNetSalary(int AgencyID)
        {
            try
            {
                decimal Amount = 0;
                using (var context = new IOASDBEntities())
                {
                    var Month = DateTime.Now.Month;
                    var qryAmount = (from b in context.tblAgencySalary
                                     where b.AgencySalaryId == AgencyID
                                     select b.TotalAmount).FirstOrDefault();
                    if (qryAmount != null || qryAmount != 0)
                    {
                        Amount = qryAmount ?? 0;
                    }
                }
                return Amount;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string GetFinancialYear(int FinYear)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = context.tblFinYear.Where(m => m.FinYearId == FinYear).Select(m => m.Year).FirstOrDefault();
                    return qry;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<MasterlistviewModel> GetBankAccount()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    int[] id = { 38, 61 };
                    List = (from C in context.tblAccountHead
                            where id.Contains(C.AccountGroupId ?? 0)
                            orderby C.AccountHeadId
                            select new MasterlistviewModel()
                            {
                                id = C.AccountHeadId,
                                name = C.AccountHead
                            }).ToList();
                }
                return List;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }
        }
        public static string GetBankName(int BankId)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var qry = context.tblAccountHead.Where(m => m.AccountHeadId == BankId).Select(m => m.AccountHead).FirstOrDefault();
                    return qry;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetNewOHPNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblOverheadsPosting
                               orderby b.OverheadsPostingId descending
                               select b.OverheadsPostingNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "OHP/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "OHP/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<MasterlistviewModel> GetProjectNumberList(int? PIId, int Classificationid)
        {
            try
            {
                List<MasterlistviewModel> pjct = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblProject
                                 where (U.PIName == PIId && U.ProjectClassification == Classificationid)
                                 select new { U.ProjectId, U.ProjectNumber }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            pjct.Add(new MasterlistviewModel()
                            {
                                id = query[i].ProjectId,
                                name = query[i].ProjectNumber
                            });
                        }
                    }

                }
                return pjct;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                return User;
            }
        }
        public static List<MasterlistviewModel> GetDDFProjectNumberList(string PIDepartment)
        {
            try
            {
                List<MasterlistviewModel> pjct = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblDDFMapping
                                 where (U.DepartmentName == PIDepartment)
                                 select U).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            pjct.Add(new MasterlistviewModel()
                            {
                                id = query[i].Id,
                                name = query[i].ProjectNumber
                            });
                        }
                    }

                }
                return pjct;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                return User;
            }
        }
        public static List<MasterlistviewModel> GetOHPostingBankAccountGroup()
        {
            try
            {
                List<MasterlistviewModel> List = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    List = (from C in context.tblAccountGroup
                            where C.AccountGroupId == 38
                            orderby C.AccountGroup
                            select new MasterlistviewModel()
                            {
                                id = C.AccountGroupId,
                                name = C.AccountGroup
                            }).ToList();
                }
                return List;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }
        }
        public static string GetNewReceiptNumber(string type = "RCV")
        {
            try
            {
                var srbNum = string.Empty;
                using (var context = new IOASDBEntities())
                {
                    var num = (from r in context.tblReceipt
                               where (r.ReceiptNumber.Contains(type + "/"))
                               orderby r.ReceiptId descending
                               select r.ReceiptNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return type + "/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return type + "/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<MasterlistviewModel> GetBankList(int Classificationcode, int projecttype = 0, int projectscheme = 0, string category = null)
        {
            try
            {
                List<MasterlistviewModel> pjct = new List<MasterlistviewModel>();
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblBankAccountMapping
                                 where U.ClassificationCode == Classificationcode && (U.ProjectType == projecttype || projecttype == 0)
                                 && (U.ProjectScheme == projectscheme || projectscheme == 0) && (U.CategoryName == category || category == null || category == "")
                                 select U).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            pjct.Add(new MasterlistviewModel()
                            {
                                id = query[i].BankAccountId,
                                name = query[i].BankName
                            });
                        }
                    }

                }
                return pjct;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                return User;
            }
        }
        public static string GetNewDOPNo()
        {
            try
            {
                var no = string.Empty;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblPCFDistribution
                               orderby b.Id descending
                               select b.ReferenceNumber).FirstOrDefault();

                    if (!String.IsNullOrEmpty(num))
                    {
                        var value = num.Split('/').Last();
                        string number = Regex.Replace(value, @"\D", "");
                        var seqnum = Convert.ToInt32(number);
                        seqnum += 1;
                        return "DOP/" + GetCurrentFinYear() + "/" + seqnum.ToString("000000");
                    }
                    else
                    {
                        return "DOP/" + GetCurrentFinYear() + "/" + "000001";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<MasterlistviewModel> GetPaymentNumberListbyType(int paymenttype)
        {
            try
            {

                List<MasterlistviewModel> Numberlist = new List<MasterlistviewModel>();
                Numberlist.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {
                    if (paymenttype == 1)
                    {
                        var query = (from C in context.tblDistribution
                                     orderby C.DistributionId
                                     select C).ToList();
                        for (int i = 0; i < query.Count(); i++)
                        {
                            Numberlist.Add(new MasterlistviewModel()
                            {
                                id = query[i].DistributionId,
                                name = query[i].DistributionNumber,
                            });
                        }
                    }
                    if (paymenttype == 2)
                    {
                        var query = (from C in context.tblHonororium
                                     orderby C.HonororiumId
                                     select C).ToList();
                        for (int i = 0; i < query.Count(); i++)
                        {
                            Numberlist.Add(new MasterlistviewModel()
                            {
                                id = query[i].HonororiumId,
                                name = query[i].HonororiumNo,
                            });
                        }
                    }
                    if (paymenttype == 3)
                    {
                        var query = (from C in context.tblReceipt
                                     join P in context.tblProject on C.ProjectId equals P.ProjectId
                                     where C.ReceiptNumber.Contains("RCV") && C.Status == "Completed" && P.ProjectType == 2
                                     orderby C.ReceiptId
                                     select C).ToList();
                        for (int i = 0; i < query.Count(); i++)
                        {
                            Numberlist.Add(new MasterlistviewModel()
                            {
                                id = query[i].ReceiptId,
                                name = query[i].ReceiptNumber,
                            });
                        }
                    }

                }

                return Numberlist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<MasterlistviewModel> GetAllBankList()
        {
            try
            {
                List<MasterlistviewModel> pjct = new List<MasterlistviewModel>();
                pjct.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "Select Any"
                });
                using (var context = new IOASDBEntities())
                {
                    var query = (from U in context.tblBankAccountMapping
                                 orderby U.Id
                                 select U).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            pjct.Add(new MasterlistviewModel()
                            {
                                id = query[i].BankAccountId,
                                name = query[i].BankName
                            });
                        }
                    }

                }
                return pjct;
            }
            catch (Exception ex)
            {
                List<MasterlistviewModel> User = new List<MasterlistviewModel>();
                return User;
            }
        }
        public static Int32 GetAccountGroupIdbyAcId(int accountheadid)
        {
            try
            {
                Int32 gId = 0;
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblAccountHead.FirstOrDefault(m => m.AccountHeadId == accountheadid);
                    if (query != null)
                        gId = Convert.ToInt32(query.AccountGroupId);
                    return gId;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static List<MasterlistviewModel> GetProjectNumbers()
        {
            try
            {
                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();
                Title.Add(new MasterlistviewModel()
                {
                    id = null,
                    name = "",
                });
                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 join U in context.vwFacultyStaffDetails on C.PIName equals U.UserId
                                 where C.Status == "Active"
                                 orderby C.ProjectId
                                 select new { U.FirstName, C }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Title.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.ProjectId,
                                name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "- " + query[i].FirstName,
                            });
                        }
                    }

                }

                return Title;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static string getinstituteclaimsid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblInstituteClaims
                             orderby intern.InstituteClaimsId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var insid = query.InstituteClaimsId;
                    currhoid = insid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string getreceiptinstituteclaimsid()
        {
            try
            {
                var currhoid = 0;
                string seqnum = "";
                var context = new IOASDBEntities();
                var query = (from intern in context.tblReceiptClaim
                             orderby intern.ReceiptClaimsId descending
                             select intern).FirstOrDefault();

                if (query != null)
                {
                    var insid = query.ReceiptClaimsId;
                    currhoid = insid + 1;
                    seqnum = currhoid.ToString("D6");
                    return seqnum;
                }
                else
                {
                    return seqnum;
                }

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public static string GetClaimType(int id)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblCodeControl.Where(m => m.CodeName == "ClaimType" && m.CodeValAbbr == id).Select(m => m.CodeValDetail).FirstOrDefault();
                    return query;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool ApprovalForInstituteClaims(int InsClaimId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblInstituteClaims.FirstOrDefault(m => m.InstituteClaimsId == InsClaimId);
                    if (query.Status == "Open")
                    {
                        query.Status = "Approval Pending";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static bool ApprovalPendingForInstituteClaims(int InsClaimId, int LoggedInUser)
        {
            bool update = false;
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblInstituteClaims.FirstOrDefault(m => m.InstituteClaimsId == InsClaimId);
                    if (query.Status == "Approval Pending")
                    {
                        query.Status = "Active";
                        query.UPDT_By = LoggedInUser;
                        query.UPDT_TS = DateTime.Now;
                        context.SaveChanges();
                        update = true;
                    }
                }
                return update;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static decimal GetClaimValue(int Id)
        {
            try
            {
                using (var context = new IOASDBEntities())
                {
                    var query = context.tblInstituteClaims.Where(m => m.InstituteClaimsId == Id).Select(m => m.ClaimAmount).FirstOrDefault();
                    return Convert.ToDecimal(query);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<MasterlistviewModel> GetProjectNumberList()
        {
            try
            {
                List<MasterlistviewModel> Title = new List<MasterlistviewModel>();

                using (var context = new IOASDBEntities())
                {
                    var query = (from C in context.tblProject
                                 join U in context.vwFacultyStaffDetails on C.PIName equals U.UserId
                                 where C.Status == "Active"
                                 orderby C.ProjectId
                                 select new { U.FirstName, C }).ToList();
                    if (query.Count > 0)
                    {
                        for (int i = 0; i < query.Count; i++)
                        {
                            Title.Add(new MasterlistviewModel()
                            {
                                id = query[i].C.ProjectId,
                                name = query[i].C.ProjectNumber + "-" + query[i].C.ProjectTitle + "- " + query[i].FirstName,
                            });
                        }
                    }

                }



                return Title;
            }
            catch (Exception ex)
            {
                return new List<MasterlistviewModel>();
            }

        }
        public static int GetCommitmentSequenceno()
        {
            try
            {
                int seqNo = 0;

                using (var context = new IOASDBEntities())
                {
                    var num = (from b in context.tblCommitment
                               select b).Max(m => m.SequenceNo) ?? 0;

                    if (num > 0)
                    {
                        seqNo = num + 1;
                    }
                    else
                    {
                        seqNo = 1;
                    }
                }
                return seqNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<InstituteClaims> GetComitmentNo(int projid)
        {
            List<InstituteClaims> fellow = new List<InstituteClaims>();
            try
            {
                fellow.Add(new InstituteClaims()
                {
                    ViewBagCommitNo = "Select CommitmentNumber",
                    ViewBagCommitId = 0
                });
                using (var context = new IOASDBEntities())
                {
                    if (projid != 0)
                    {
                        var query = (from H in context.tblProject
                                     join P in context.tblCommitment on H.ProjectId equals P.ProjectId
                                     where H.ProjectId == projid && P.CommitmentBalance > 0
                                     select new
                                     {
                                         P.CommitmentNumber,
                                         P.CommitmentId

                                     }).ToList();

                        if (query.Count > 0)
                        {
                            for (int i = 0; i < query.Count; i++)
                            {
                                fellow.Add(new InstituteClaims()
                                {
                                    ViewBagCommitNo = query[i].CommitmentNumber,
                                    ViewBagCommitId = query[i].CommitmentId
                                });
                            }
                        }
                    }
                }
                return fellow;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static decimal GetSpendBalance(int commitmentid)
        {
            InstituteClaims fellow = new InstituteClaims();
            try
            {
                decimal AmountSpend = 0;
                using (var context = new IOASDBEntities())
                {
                    if (commitmentid > 0)
                    {
                        var qry = (from H in context.vwCommitmentSpentBalance
                                   where H.CommitmentId == commitmentid
                                   select new
                                   {
                                       H.AmountSpent,
                                   }).ToList();
                        AmountSpend = qry.Select(m => m.AmountSpent).Sum() ?? 0;

                    }
                }
                return AmountSpend;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static int getInvoiceId(int financialyear, int projecttype)
        {
            try
            {
                var lastseqnum = 0;
                var context = new IOASDBEntities();
                var invquery = (from inv in context.tblProjectInvoice
                                where inv.FinancialYear == financialyear && inv.ProjectType == projecttype
                                select inv.SequenceNumber).Max();

                int seqnum = invquery ?? 0;
                lastseqnum = seqnum + 1;
                return lastseqnum;

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
    }
}