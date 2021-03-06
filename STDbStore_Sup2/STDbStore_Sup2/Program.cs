﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDbStore_Sup2
{
    class Program
    {
        static void Main(string[] args)
        {

            FileStream fs = File.OpenRead(@"F:\股票tick数据导入完成记录\文件记录20170727 10-25-48.txt");
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            List<string> fileImported = new List<string>();
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("csv"))
                {
                    fileImported.Add(line.Substring(27, 12));
                }
            }

            fs.Close();
            sr.Close();

            DirectoryInfo dir_st_logs_imported = new DirectoryInfo(@"F:\ST导入完成");
            foreach(var file in dir_st_logs_imported.GetFiles())
            {
                fs = file.OpenRead();
                sr = new StreamReader(fs, Encoding.UTF8);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("csv"))
                    {
                        fileImported.Add(line.Substring(27, 12));
                    }
                }
                fs.Close();
                sr.Close();
            }

            int count = 0;

            #region 移动已导入的文件到另外的文件夹

            DirectoryInfo dir = new DirectoryInfo(@"F:\股票tick数据\tick2013y");
            foreach (var file in dir.GetFiles())
            {
                if (fileImported.Contains(file.Name))
                {
                    ++count;
                    file.MoveTo(@"F:\股票tick数据_导入完成\tick2013y\" + file.Name);
                    Console.Clear();
                    Console.Write("{0}/{1}", count, fileImported.Count);
                    File.AppendAllLines(@"F:\文件移动记录.txt", new string[1] { file.FullName + @"到F:\股票tick数据_导入完成\tick2013y\" + file.Name });
                }
            }
            #endregion

            List<string> marketid = new List<string>();
            List<string> stockcode = new List<string>();

            #region 找出数据库需删除的文件，生成删除语句
            List<string> fileBeginImport = new List<string>();
            FileStream fs2 = File.OpenRead(@"F:\股票tick数据导入完成记录\开始导入20170727 10-25-48.txt");
            StreamReader sr2 = new StreamReader(fs2, Encoding.UTF8);
            string line2 = null;
            while ((line2 = sr2.ReadLine()) != null)
            {
                if (line2.Contains("csv"))
                {
                    fileBeginImport.Add(line2.Substring(27, 12));
                }
            }

            DirectoryInfo dir_st_logs_beginimport = new DirectoryInfo(@"F:\ST开始导入");
            foreach(var file in dir_st_logs_beginimport.GetFiles())
            {
                fs = file.OpenRead();
                sr = new StreamReader(fs, Encoding.UTF8);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("csv"))
                    {
                        fileBeginImport.Add(line.Substring(27, 12));
                    }
                }
                fs.Close();
                sr.Close();
            }

            foreach (var file in fileBeginImport)
            {
                if (!fileImported.Contains(file))
                {
                    marketid.Add(file.Substring(0, 2));
                    stockcode.Add(file.Substring(2, 6));
                    File.AppendAllLines(@"F:\需删除.txt", new string[1] { file });
                }
            }
            #endregion

            #region 生成删除语句
            string connStr = "Server=192.168.2.159;User ID=root;Password=123456;Database=STHisDBTick2013;CharSet=utf8";
            List<string> tableNames = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select TABLE_NAME from INFORMATION_SCHEMA.tables where TABLE_SCHEMA = \"sthisdbtick2013\"";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tableNames.Add(reader[0].ToString());
                }
            }

            List<string> querys = new List<string>();
            string query = "";
            foreach (var tName in tableNames)
            {
                query = "DELETE FROM " + tName + " WHERE ";
                for (int i = 0; i < marketid.Count; ++i)
                {
                    if (i == marketid.Count - 1)
                    {
                        query += string.Format("(marketid=\"{0}\" AND stockcode=\"{1}\");", marketid[i], stockcode[i]);
                    }
                    else
                    {
                        query += string.Format("(marketid=\"{0}\" AND stockcode=\"{1}\") OR ", marketid[i], stockcode[i]);
                    }
                }

                querys.Add(query);
            }

            File.WriteAllLines(@"F:\股票tick数据删除处理语句.txt", querys);
            #endregion
        }
    }
}
