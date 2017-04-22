using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.Common;
using ClientLib.Exceptions;
using System.Data.OleDb;
using Easi365UI.Entities;

namespace Easi365UI.Service
{
    public class UserHelper
    {
        public static DbHelperOleDb dbHelper { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="databasePath">数据库文件路径</param>
        public UserHelper(string databasePath) {
            dbHelper = new DbHelperOleDb(databasePath);
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="passWord">密码</param>
        /// <param name="rememberPwd">是否记住密码</param>
        /// <param name="autoLogin">是否自动登录</param>
        public void UserInfoSetting(string loginName, string passWord, bool rememberPwd, bool autoLogin)
        {
            try
            {
                SymmetricMethod sm = new SymmetricMethod();
                string pwd = "", sqlStr = "", dt = DateTime.Now.ToString();
                if (!string.IsNullOrEmpty(passWord))
                    pwd = sm.Encrypto(passWord);

                if (!IsUserInfoExists(loginName))
                    sqlStr = string.Format("INSERT INTO USERINFO([LoginName],[PassWord],[IsRememberPwd],[IsAutoLogin],[LastLoginTime]) VALUES('{0}','{1}',{2},{3},'{4}')", loginName, pwd, rememberPwd, autoLogin, dt);
                else
                    sqlStr = string.Format("UPDATE USERINFO SET [PassWord] = '{0}',[IsRememberPwd] = {1},[IsAutoLogin] = {2},[LastLoginTime] = '{3}' WHERE [LoginName] = '{4}'", pwd, rememberPwd, autoLogin, dt, loginName);

                var result = dbHelper.ExecuteSql(sqlStr);
            }
            catch (Exception ex)
            {
                new DBException("数据库错误信息读取记录失败.", ex);
            }
        }

        /// <summary>
        /// 判断用户信息是否存在
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        public bool IsUserInfoExists(string loginName) {
            try
            {
                var sqlStr = string.Format("SELECT COUNT(*) FROM USERINFO WHERE [LoginName] = '{0}'", loginName);
                var isExists = dbHelper.Exists(sqlStr);
                return isExists;
            }
            catch (Exception ex)
            {
                new DBException("数据库错误信息读取记录失败.", ex);
                return false;
            }
        }

        /// <summary>
        /// 获取最后登录的用户
        /// </summary>
        public UserInfo GetLastLoginUserInfo() {
            UserInfo user = new UserInfo();
            OleDbDataReader reader = null;
            try
            {
                var sqlStr = "SELECT TOP 1 * FROM USERINFO ORDER BY [LastLoginTime] DESC";
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        user.LoginName = "" + reader["LoginName"].ToString();
                        user.PassWord = "" + reader["PassWord"].ToString();
                        user.IsRememberPwd = (bool)reader["IsRememberPwd"];
                        user.IsAutoLogin = (bool)reader["IsAutoLogin"];
                    }
                    else {
                        user = null;
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                new DBException("数据库错误信息读取记录失败.", ex);
                return null;
            }
        }

        /// <summary>
        /// 根据账号获取用户
        /// </summary>
        public UserInfo GetUserByLoginName(string loginName)
        {
            UserInfo user = new UserInfo();
            try
            {
                var sqlStr = string.Format("SELECT * FROM USERINFO WHERE [LoginName] = '{0}'", loginName);
                OleDbDataReader reader = null;
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        user.LoginName = "" + reader["LoginName"].ToString();
                        user.PassWord = "" + reader["PassWord"].ToString();
                        user.IsRememberPwd = (bool)reader["IsRememberPwd"];
                        user.IsAutoLogin = (bool)reader["IsAutoLogin"];
                    }
                    else
                    {
                        user = null;
                    }
                }
            }
            catch (Exception ex)
            {
                new DBException("数据库错误信息读取记录失败.", ex);
                return null;
            }
            return user;
        }

        /// <summary>
        /// 获取所有账户
        /// </summary>
        public List<UserInfo> GetUsers() {
            List<UserInfo> users = new List<UserInfo>();
            try
            {
                var sqlStr = "SELECT * FROM USERINFO ORDER BY [LastLoginTime] DESC";
                OleDbDataReader reader = null;
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    while (reader.Read()) {
                        UserInfo user = new UserInfo();
                        user.LoginName = "" + reader["LoginName"].ToString();
                        user.PassWord = "" + reader["PassWord"].ToString();
                        user.IsRememberPwd = (bool)reader["IsRememberPwd"];
                        user.IsAutoLogin = (bool)reader["IsAutoLogin"];
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                new DBException("数据库错误信息读取记录失败.", ex);
                return null;
            }
            return users;
        }

    }
}
