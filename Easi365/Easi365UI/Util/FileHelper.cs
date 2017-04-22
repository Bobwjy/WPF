using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.Core;

namespace Easi365UI.Util
{
    public class FileHelper
    {
        //重命名
        public static FileSystemInfo Rename(string sourceName, string dirName, bool isDirectory)
        {
            string path = sourceName.Substring(0, sourceName.LastIndexOf(Path.DirectorySeparatorChar) + 1) + dirName;

            if (isDirectory)
            {
                if (!Directory.Exists(path))
                    Directory.Move(sourceName, path);
            }
            else
            {
                if (!File.Exists(path))
                    File.Move(sourceName, path);
            }

            if (isDirectory)
                return new DirectoryInfo(path);
            return new FileInfo(path);
        }

        //新建文件夹
        public static string NewFolder(string parentPath)
        {
            string folderName = "新建文件夹";
            string path = string.Format("{0}{1}{2}", parentPath, Path.DirectorySeparatorChar, folderName);
            bool flag = true;
            int index = 1;
            do
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    break;
                }
                else
                {
                    path = string.Format("{0}{1}{2}({3})", parentPath, Path.DirectorySeparatorChar, folderName, index);
                    index++;
                }
            } while (flag);

            return path;
        }

        //新建文件
        public static string NewFile(string parentPath, string ext)
        {
            string folderName = Files[ext];
            string path = string.Format("{0}{1}{2}{3}", parentPath, Path.DirectorySeparatorChar, folderName, ext);
            bool flag = true;
            int index = 1;
            do
            {
                if (!File.Exists(path))
                {
                    string tempExt = Path.GetExtension(path);
                    if (tempExt == ".xlsx" || tempExt == ".accdb")
                    {
                        string templatePath = string.Empty;
                        switch (tempExt)
                        {
                            case ".xlsx":
                                templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\Microsoft Excel 工作表.xlsx");
                                break;
                            case ".accdb":
                                templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\Microsoft Access 数据库.accdb");
                                break;
                        }
                        File.Copy(templatePath, path);
                    }
                    else
                    {
                        File.Create(path);
                    }
                    break;
                }
                else
                {
                    path = string.Format("{0}{1}{2}({3}){4}", parentPath, Path.DirectorySeparatorChar, folderName, index, ext);
                    index++;
                }
            } while (flag);

            return path;
        }

        public static string GetFileTemplate(string ext)
        {
            string templatePath = string.Empty;
            try
            {
                switch (ext)
                {
                    case ".txt":
                        templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\文本文件.txt");
                        break;
                    case ".docx":
                        templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\Microsoft Word 文档.docx");
                        break;
                    case ".xlsx":
                        templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\Microsoft Excel 工作表.xlsx");
                        break;
                    case ".pptx":
                        templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\Microsoft PowerPoint 演示文稿.pptx");
                        break;
                    case ".accdb":
                        templatePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Template\\Microsoft Access 数据库.accdb");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.Add("获取文件模板出现异常", ex);
            }

            return templatePath;
        }

        public static Dictionary<string, string> Files = new Dictionary<string, string>() 
        {
            {".txt", "文本文件"}, 
            {".docx", "Microsoft Word 文档"}, 
            {".xlsx", "Microsoft Excel 工作表"},
            {".pptx", "Microsoft PowerPoint 演示文稿"},
            {".accdb", "Microsoft Access 数据库"}
        };

        //删除文件
        public static void Delete(string path, bool isDirectory)
        {
            if (isDirectory)
            {
                if (!Directory.Exists(path)) return;
                Directory.Delete(path, true);
            }
            else
            {
                if (!File.Exists(path)) return;
                File.Delete(path);
            }
        }

        //剪切文件/夹
        public static void Cut(string source, string dirPath, bool isDirectory)
        {
            if (!File.Exists(source)) return;

            string path = dirPath + Path.DirectorySeparatorChar + source.Substring(source.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            if (isDirectory)
            {
                if (Directory.Exists(path)) return;
                Directory.Move(source, path);
            }
            else
            {
                if (File.Exists(path)) return;
                File.Move(source, path);
            }
        }

        //复制文件/夹
        public static void CopyTo(string source, string dirPath, bool isDirectory)
        {
            if (!File.Exists(source)) return;

            string path = dirPath + Path.DirectorySeparatorChar + source.Substring(source.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            if (isDirectory)
            {
                if (Directory.Exists(path)) return;
                //Directory.CreateDirectory(path);
                //CopyDirectory(source, dirPath);
            }
            else
            {
                string ext = Path.GetExtension(source);
                string name = Path.GetFileNameWithoutExtension(source);
                bool flag = true;
                int index = 1;
                do
                {
                    if (!File.Exists(path))
                    {
                        File.Copy(source, path, true);
                        break;
                    }
                    else
                    {
                        path = string.Format("{0}{1}{2}({3}){4}", dirPath, Path.DirectorySeparatorChar, name, index, ext);
                        index++;
                    }
                } while (flag);
            }
        }

        private static void CopyDirectory(string srcPath, string dirPath)
        {
            if (dirPath[dirPath.Length - 1] != Path.DirectorySeparatorChar) dirPath += Path.DirectorySeparatorChar;

            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string[] fileList = Directory.GetFileSystemEntries(srcPath);
            foreach (string file in fileList)
            {
                if (Directory.Exists(file))
                {
                    CopyDirectory(file, dirPath + Path.GetFileName(file));
                }
                else
                {
                    File.Copy(file, dirPath + Path.GetFileName(file), true);
                }
            }
        }
    }
}
