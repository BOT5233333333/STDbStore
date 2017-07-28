using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace DeleteSTRows2
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = File.OpenRead(@"E:\股票tick数据导入完成记录\开始导入20170713 10-30-04.txt");
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            string line = null;
            List<string> stockcodes = new List<string>();
            List<string> marketid = new List<string>();

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("csv"))
                {
                    marketid.Add(line.Substring(27, 2));
                    stockcodes.Add(line.Substring(29, 6));
                }
            }
            fs.Close();
            sr.Close();

            string connStr = "Server=192.168.2.129;User ID=root;Password=123456;Database=STHisDBTick2013;CharSet=utf8";
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
            foreach(var tName in tableNames)
            {
                query = "DELETE FROM " + tName + " WHERE ";
                for(int i=0;i<marketid.Count; ++i)
                {
                    if(i == marketid.Count-1)
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

            File.WriteAllLines(@"E:\股票tick数据删除处理语句.txt", querys);
        }
    }
}
