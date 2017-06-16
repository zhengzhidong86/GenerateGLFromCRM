using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIssueAndShip
{
    public class TxtOperate
    {
        public static string companyID;
        public static DbHelperSQL dbHelper;
        public static bool WriteTxt(string MyOutput)
        {
            try
            {
                MyOutput = MyOutput.Replace('\'', ' ');
                string strSql = "";
                string key1 = System.Guid.NewGuid().ToString();
                strSql = "INSERT INTO ICE.UD30(Company,Key1,Character01,Date01,ShortChar01)";
                strSql += "Values('" + companyID + "','" + key1 + "',N'" + MyOutput + "','" + DateTime.Now + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                int rowsCount = dbHelper.ExecuteSql(strSql);

                string SaveFileName = "";
                System.IO.StreamWriter myStreamWriter = default(System.IO.StreamWriter);
                SaveFileName = GetFileName();
                myStreamWriter = System.IO.File.AppendText(SaveFileName);
                myStreamWriter.WriteLine(MyOutput);
                myStreamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void CreateFileDir(string fileDir)
        {
            if (System.IO.Directory.Exists(fileDir) == false)
            {
                System.IO.Directory.CreateDirectory(fileDir);
            }
        }

        public static string GetFileName()
        {
            string returnValue = "";
            string fileDir = System.AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(fileDir + "ExcuteLog"))
            {
                Directory.CreateDirectory(fileDir + "ExcuteLog");
            }
            fileDir = fileDir + "ExcuteLog\\";
            CreateFileDir(fileDir);
            string strDatatime = "";
            DateTime dt = DateTime.Now;
            strDatatime = dt.Year.ToString() + "-" + dt.Month.ToString() + "-" + (dt.Day.ToString() + ".log");
            returnValue = fileDir + strDatatime;
            return returnValue;
        }
    }
}
