using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace AutoIssueAndShip
{
    public class Authentication
    {
        public static string serverUrl;
        /// <summary>
        /// 取得EpicorSession
        /// </summary>
        /// <returns></returns>
        public static Ice.Core.Session GetEpicorSession()
        {
            try
            {
                //string serverUrl = ConfigurationManager.AppSettings["ServerUrl"];
                string userName = ConfigurationManager.AppSettings["EpicorLoginName"];
                string passWord = ConfigurationManager.AppSettings["EpicorLoginPassword"];
                
                //passWord = DESEncrypt.Decrypt(passWord);
                string configFile = System.AppDomain.CurrentDomain.BaseDirectory + "/Config/Default.sysconfig";
                Ice.Core.Session epicorSession = new Ice.Core.Session(userName, passWord, serverUrl, Ice.Core.Session.LicenseType.Default,configFile);
                return epicorSession;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

    }
}