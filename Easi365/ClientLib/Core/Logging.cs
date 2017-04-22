using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClientLib.Core
{
    public class Logging
    {
        public static string LogFilePath { get; set; }
		private static DateTime time = DateTime.Now;
		private static StreamWriter writer;

        //用户操作日志流
        private static StreamWriter operWriter;

		public static void Initialize()
		{
			//如果禁止记录日志则返回
			if (!CoreManager.ConfigManager.Settings.Logging)
				return;
			//日志文件路径
			string path = Path.Combine(CoreManager.StartupPath, @"Log");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
            //string t = string.Concat(time.Year.ToString(), time.Month.ToString(), time.Day.ToString(),
            //                                "-", time.Hour.ToString(), time.Minute.ToString(), time.Second.ToString(), ".log");

            string t = string.Concat(time.Year.ToString(), time.Month.ToString(), time.Day.ToString(),
                                            "-", time.Hour.ToString(), ".log");
            string o = string.Concat("操作日志", time.Year.ToString(), time.Month.ToString(), time.Day.ToString(), ".log");


            string operPath = Path.Combine(path,o);
			path = Path.Combine(path, t);

			LogFilePath = path;
			writer = new StreamWriter(path, true);
			writer.WriteLine("Easi365网盘日志文件，生成于：");
			writer.WriteLine(time.ToString());
			writer.Flush();

            operWriter = new StreamWriter(operPath, true);
            operWriter.WriteLine("Easi365网盘用户操作日志文件，生成于：");
            operWriter.WriteLine(time.ToString());
            operWriter.Flush();
		}

		/// <summary>
        /// 向日志中添加记录
		/// </summary>
		/// <param name="bugDesc">错误描述</param>
		/// <param name="e"></param>
		public static void Add(string bugDesc,Exception e)
		{
			//如果禁止记录日志则返回
			if (!CoreManager.ConfigManager.Settings.Logging)
				return;

			writer.WriteLine();
			writer.WriteLine("--------------------");
            writer.WriteLine(bugDesc + Environment.NewLine);
            if (e != null)
            {
                writer.WriteLine("New Exception: {0}", DateTime.Now.ToString());
                if (e.Source != null)
                    writer.WriteLine("Source: {0}", e.Source);
                if (e.TargetSite != null)
                    writer.WriteLine("Target Site: {0}", e.TargetSite.Name);
                if (!string.IsNullOrEmpty(e.HelpLink))
                    writer.WriteLine("HelpLink: {0}", e.HelpLink);
                if (!string.IsNullOrEmpty(e.Message))
                    writer.WriteLine("Exception: {0}", e.Message);
                if (!string.IsNullOrEmpty(e.StackTrace))
                    writer.WriteLine("StackTrace：{0}", e.StackTrace);
                if (e.InnerException != null)
                    writer.WriteLine("Inner Exception: {0}", e.InnerException.Message);
            }
			writer.Flush();
		}

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="e"></param>
        public static void Add(Exception e)
        {
            Add("", e);
        }

        /// <summary>
        /// 添加描述信息
        /// </summary>
        /// <param name="msg"></param>
        public static void AddInfo(string msg)
        {
            Add(msg, null);
        }

		/// <summary>
		/// 释放资源
		/// </summary>
		public static void Exit()
		{
			try
			{
				if (CoreManager.ConfigManager.Settings.Logging)
				{
                    writer.Close();
                    writer.Dispose();

                    
				}

                operWriter.Close();
                operWriter.Dispose();
			}
			catch
			{
			}
		}

        /// <summary>
        /// 记录用户操作日志
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteOperLog(string msg)
        {
            operWriter.WriteLine(msg);
            operWriter.Flush();
        }
    }
}
