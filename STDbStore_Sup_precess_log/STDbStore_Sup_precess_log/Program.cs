using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace STDbStore_Sup_precess_log
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dir = new DirectoryInfo(@"E:\ProgramWorkSpace\Visual Studio 2015\STDbStore_Concurrent\STDbStore_Concurrent\bin\Release\Logs");
            foreach(var file in dir.GetFiles())
            {
                FileStream fs = file.OpenRead();
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string line = null;
                while((line = sr.ReadLine())!=null)
                {
                    if(line.Contains("开始导入") && line.Contains("tick2013y"))
                    {
                        file.CopyTo(@"F:\ST开始导入\" + file.Name);
                    }
                    else if(line.Contains("导入完成") && line.Contains("tick2013y"))
                    {
                        file.CopyTo(@"F:\ST导入完成\" + file.Name);
                    }
                }
                fs.Close();
                sr.Close();
            }
        }
    }
}
