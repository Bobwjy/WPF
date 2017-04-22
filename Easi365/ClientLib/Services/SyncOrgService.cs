using ClientLib.Common;
using ClientLib.Core;
using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Services
{
    public class SyncOrgService
    {
        public static DbHelperOleDb dbHelper { get; set; }
        public static OrgService orgService { get; set; }

        public SyncOrgService(string dataBasePath, OrgService service)
        {
            dbHelper = new DbHelperOleDb(dataBasePath);
            orgService = service;
        }

        public async virtual void Init()
        {
            try
            {
                var syncRel = await SyncOrgRel();
                var syncDepts = await SyncDepts();
                var syncStaffs = await SyncStaffs();
            }
            catch { }
        }

        public Task<bool> SyncOrgRel()
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    var deleteSql = "DELETE FROM ORGREL";
                    var result = dbHelper.ExecuteSql(deleteSql);

                    List<OrgRel> orgRelList = new List<OrgRel>();
                    orgRelList = orgService.GetOrgRelList();

                    foreach (var orgRel in orgRelList)
                    {
                        var insertSql = string.Format("INSERT INTO ORGREL([StaffName],[StaffId],[DeptId]) VALUES('{0}', {1}, {2})", orgRel.StaffName, orgRel.StaffId, orgRel.DeptId);
                        dbHelper.ExecuteSql(insertSql);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("同步组织架构关系映射表报错", ex);
                    return false;
                }
            });
        }

        public Task<bool> SyncDepts() {
            return Task<bool>.Factory.StartNew(() => {
                try
                {
                    var localDepts = orgService.GetDeptsFromLocal();
                    var serverDepts = orgService.GetDeptsFromServer();

                    // 遍历本地数据
                    foreach (var dept in localDepts)
                    {
                        var arr = serverDepts.Where(list => list.ID == dept.ID).ToArray<Department>();
                        // 过滤掉在服务端没有的数据(在服务端没有说明已被管理员删除)，同时在本地删除该数据
                        if (arr.Length == 0)
                        {
                            var deleteSql = string.Format("DELETE FROM DEPTS WHERE [ID] = {0}", dept.ID);
                            dbHelper.ExecuteSql(deleteSql);
                        }
                        else
                        {
                            // 判断是否有更新，有则更新本地数据
                            var sDept = arr[0];
                            if (sDept.Modified != dept.Modified)
                            {
                                var updateSql = string.Format("UPDATE DEPTS SET [Dept] = '{0}',[ParentDept] = '{1}',[ParentID] = {2},[SpaceUri] = '{3}',[Modified] = '{4}' " +
                                    "WHERE [ID] = {5}", sDept.DeptName, sDept.ParentName, sDept.ParentID, sDept.SpaceUri, sDept.Modified, dept.ID);
                                dbHelper.ExecuteSql(updateSql);
                            }
                        }
                    }

                    // 遍历服务端数据
                    foreach (var dept in serverDepts) {
                        // 查询服务器端数据是否已在本地存在，不存在就同步到本地
                        var arr = localDepts.Where(list => list.ID == dept.ID).ToArray<Department>();
                        if (arr.Length == 0) {
                            var insertSql = string.Format("INSERT INTO DEPTS([ID],[Dept],[ParentDept],[ParentID],[SpaceUri],[Modified]) " +
                                "VALUES({0},'{1}','{2}',{3},'{4}','{5}')", dept.ID, dept.DeptName, dept.ParentName, dept.ParentID, dept.SpaceUri, dept.Modified);
                            dbHelper.ExecuteSql(insertSql);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("组织架构部门信息同步报错", ex);
                    return false;
                }
            });
        }

        public Task<bool> SyncStaffs() {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    var localStaffs = orgService.GetStaffsFromLocal();
                    var serverStaffs = orgService.GetStaffsFromServer();

                    // 遍历本地数据
                    foreach (var staff in localStaffs)
                    {
                        var arr = serverStaffs.Where(list => list.ID == staff.ID).ToArray<Staff>();
                        // 过滤掉在服务端没有的数据(在服务端没有说明已被管理员删除)，同时在本地删除该数据
                        if (arr.Length == 0)
                        {
                            var deleteSql = string.Format("DELETE FROM STAFFS WHERE [ID] = {0}", staff.ID);
                            dbHelper.ExecuteSql(deleteSql);
                        }
                        else
                        {
                            // 判断是否有更新，有则更新本地数据
                            var sStaff = arr[0];
                            if (sStaff.Modified != staff.Modified)
                            {
                                var updateSql = string.Format("UPDATE STAFFS SET [StaffName] = '{0}',[Account] = '{1}',[TelPhone] = '{2}',[Dept] = '{3}',[Modified] = '{4}' " +
                                    "WHERE [ID] = {5}", sStaff.UserName, sStaff.Account, sStaff.TelPhone, sStaff.Dept, sStaff.Modified, sStaff.ID);
                                dbHelper.ExecuteSql(updateSql);
                            }
                        }
                    }

                    // 遍历服务端数据
                    foreach (var staff in serverStaffs)
                    {
                        // 查询服务器端数据是否已在本地存在，不存在就同步到本地
                        var arr = localStaffs.Where(list => list.ID == staff.ID).ToArray<Staff>();
                        if (arr.Length == 0)
                        {
                            var insertSql = string.Format("INSERT INTO Staffs([ID],[StaffName],[Account],[TelPhone],[Dept],[Modified]) " +
                                "VALUES({0},'{1}','{2}','{3}','{4}','{5}')", staff.ID, staff.UserName, staff.Account, staff.TelPhone, staff.Dept, staff.Modified);
                            dbHelper.ExecuteSql(insertSql);
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("组织架构人员信息同步报错", ex);
                    return false;
                }
            });
        }
    }

}
