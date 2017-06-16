using AutoIssueAndShip;
using Epicor.ServiceModel.Channels;
using Erp.BO;
using Erp.Proxy.BO;
using Ice.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenerateGLFromCRM
{
    public partial class GenerateGL : Form
    {
        string appServerUrl = "net.tcp://192.168.40.235/EpicorTrain";
        string companyID = "LXCS01";
        public GenerateGL(string[] args)
        {
            InitializeComponent();

            foreach (string frmName in args)
            {
                if (frmName.StartsWith("Company"))
                {
                    string[] strArr = frmName.Split('=');
                    if (strArr.Length == 2)
                    {
                        companyID = strArr[1];
                    }
                }
                else if (frmName.StartsWith("AppServerUrl"))
                {
                    string[] strArr = frmName.Split('=');
                    if (strArr.Length == 2)
                    {
                        appServerUrl = strArr[1];
                    }
                }
            }
            DateTime curDate = DateTime.Now;
            txt_FiscalYear.Text = curDate.Year.ToString();
            txt_FiscalPeriod.Text = curDate.Month.ToString();
            lb_Msg.Text = "当前公司：" + companyID + ",APP Server:" + appServerUrl;
        }

        private void btn_ImportGL_Click(object sender, EventArgs e)
        {
            BeginToOperate();
            epiUltraGridC1.DataSource = logDt;
            MessageBox.Show("凭证导入完成，详细请看日志");
        }
        private void BeginToOperate()
        {
            Authentication.serverUrl = appServerUrl;
            epicorSession = Authentication.GetEpicorSession();
            epicorSession.CompanyID = companyID;
            Operate();
            epicorSession.Dispose();
        }
        public Session epicorSession;
        private void GenerateGL_Load(object sender, EventArgs e)
        {
            CreateLogDt();
            GetConnectString();

            if (companyID == "JT0000")
            {
                btn_ImportAll.Visible = true;
            }
            else
            {
                btn_ImportAll.Visible = false;
            }
        }
        
        DataTable logDt = new DataTable();
        private void CreateLogDt()
        {
            logDt.Columns.Add("公司ID");
            logDt.Columns.Add("日志内容");
            logDt.Columns.Add("时间");
            logDt.Columns.Add("日志类型");
            logDt.Columns.Add("状态");
        }

        string sqlConnectionString = "";
        string crmConnectionString = "";
        private void GetConnectString()
        {
            sqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["sqlConnectionString"];
            crmConnectionString = System.Configuration.ConfigurationManager.AppSettings["crmConnectionString"];
        }

        private DataTable GetDataBySql(string SQLString)
        {
            if (sqlConnectionString == "") throw new Exception("连接字符串未初始化");

            DataTable dt = new DataTable("DefaultDataTable");
            DataSet ds = Query(SQLString, sqlConnectionString);
            dt = ds.Tables[0];
            return dt;
        }

        private DataTable GetDataBySql(string SQLString, string connectionString)
        {
            DataTable dt = new DataTable("DefaultDataTable");
            DataSet ds = Query(SQLString, connectionString);
            dt = ds.Tables[0];
            return dt;
        }

        public DataSet Query(string SQLString, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        public int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public int ExecuteSql(string SQLString, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        return 0;
                        //throw new Exception("执行语句：" + SQLString + "时发生错误：" + e.Message);
                    }
                }
            }
        }

        private void Operate()
        {
            string groupIDPre = txt_GroupPre.Text;
            if (groupIDPre == "")
            {
                MessageBox.Show("群组前缀不能为空");
                return;
            }
            decimal maxGlCount = Convert.ToDecimal(txt_MaxGLCount.Text);
            if (maxGlCount == 0)
            {
                MessageBox.Show("群组最大凭证数不能为0");
                return;
            }
            int year = Convert.ToInt32(txt_FiscalYear.Text);
            int month = Convert.ToInt32(txt_FiscalPeriod.Text);

            DateTime curDt;
            string logType = "";
            string logStr = "";
            string statueFlag = "";


            try
            {
                //写入科目段落值
                string strSql = @"SELECT [company],[COACode],[SegmentNbr],[SegmentCode],[SegmentDesc],[SegmentAbbrev],[EffectiveFrom],[acc_status],[log]
				    FROM [crm].[Gl_COASegvalues] with (nolock)
				    where [company]='{0}' and [acc_status]='0'
			    ";
                strSql = string.Format(strSql, companyID);

                DataTable crmSegDt = GetDataBySql(strSql, crmConnectionString);
                int segDtCount = crmSegDt.Rows.Count;
                if (segDtCount > 0)
                {
                    curDt = DateTime.Now;
                    logType = "描述";
                    logStr = "开始执行写入科目段落值";
                    statueFlag = "成功";
                    CreateUD04Recond(logStr, logType, curDt, statueFlag);
                }
                strSql = "select COACode,SegmentNbr,SegmentCode from ERP.COASegValues with (nolock) where Company='" + companyID + "' ";
                DataTable coaSegDt = GetDataBySql(strSql);
                int ii = 0;
                foreach (DataRow dr in crmSegDt.Rows)
                {
                    ii++;
                    lb_Process.Text = "写入科目段落值:" + ii + "/" + crmSegDt.Rows.Count;
                    Application.DoEvents();

                    string coACode = dr["COACode"].ToString().Trim();
                    int segmentNbr = Convert.ToInt32(dr["SegmentNbr"]);
                    string segmentCode = dr["SegmentCode"].ToString().Trim();
                    string segmentDesc = dr["SegmentDesc"].ToString().Trim();
                    string segmentAbbrev = dr["SegmentAbbrev"].ToString().Trim();
                    Nullable<DateTime> effectiveFrom = null;
                    if (dr["EffectiveFrom"] != DBNull.Value) effectiveFrom = Convert.ToDateTime(dr["EffectiveFrom"]);
                    DataRow[] drArr = coaSegDt.Select("COACode='" + coACode + "' and SegmentNbr=" + segmentNbr + " and SegmentCode='" + segmentCode + "'");
                    if (drArr.Length == 0)
                    {
                        bool isOK = GetNewCOASegValuesMethod(coACode, segmentNbr, segmentCode, segmentDesc, segmentAbbrev, effectiveFrom);
                        if (!isOK) continue;
                    }
                    UpdateCrmSegFlag(coACode, segmentNbr, segmentCode);

                    //				curDt = DateTime.Now;
                    //				logType = "科目段落值";
                    //				logStr = "coACode:" + coACode + ";segmentNbr:" + segmentNbr + ";segmentCode:" + segmentCode + "，写入成功";
                    //				statueFlag = "成功";
                    //				CreateUD04Recond(logStr,logType,curDt,statueFlag);
                }
                if (segDtCount > 0)
                {
                    curDt = DateTime.Now;
                    logType = "描述";
                    logStr = "科目段落值写入成功，共写入" + segDtCount + "条记录";
                    statueFlag = "成功";
                    CreateUD04Recond(logStr, logType, curDt, statueFlag);
                }
                crmSegDt.Dispose();

                //delete gl
                strSql = @"
				select GroupID,JournalNum,JournalLine,JEDate,seq_no from crm.GL_Accvouch_ReceiveIncome with (nolock)
				where Company='{0}' and acc_status='2' and iyear={1} and iperiod={2}
--and receiveIncomeId=263058
				order by GroupID
			";

                strSql = string.Format(strSql, companyID, year, month);
                DataTable glDt = GetDataBySql(strSql, crmConnectionString);
                int glRowsCount = glDt.Rows.Count;
                List<string> deleteJournalNumList = new List<string>();
                foreach (DataRow dr in glDt.Rows)
                {
                    string journalNum = dr["JournalNum"].ToString();
                    if (!deleteJournalNumList.Contains(journalNum)) deleteJournalNumList.Add(journalNum);
                }
                if (glRowsCount > 0)
                {
                    curDt = DateTime.Now;
                    logType = "描述";
                    logStr = "开始删除凭证";
                    statueFlag = "成功";
                    CreateUD04Recond(logStr, logType, curDt, statueFlag);
                }

                List<string> errorSqlList = new List<string>();
                List<string> groupIDList = new List<string>();
                Hashtable glStatusHt = new Hashtable();
                ii = 0;
                foreach (DataRow dr in glDt.Rows)
                {
                    ii++;
                    lb_Process.Text = "正在删除:" + ii + "/" + glDt.Rows.Count;
                    Application.DoEvents();
                    string tempGroupID = dr["GroupID"].ToString();
                    int glFlag = 0;
                    string _journalNum = dr["JournalNum"].ToString();
                    if (!glStatusHt.ContainsKey(_journalNum))
                    {
                        glFlag = CheckGlPostStatus(dr);
                        glStatusHt.Add(_journalNum, glFlag);
                    }
                    else
                    {
                        glFlag = Convert.ToInt32(glStatusHt[_journalNum]);
                    }
                    if (groupIDList.Contains(tempGroupID))
                    {
                        if (glFlag != 4 && glFlag!=0 && glFlag!=1)
                        {
                            DeleteJournal(tempGroupID, deleteJournalNumList);//可能还没删除完
                        }
                        string acc_status = "3";
                        if (glFlag == 2) continue;
                        else if (glFlag == 1 || glFlag==0) acc_status = "4";
                        string updateSql = "update crm.GL_Accvouch_ReceiveIncome set acc_status='" + acc_status + "' where Company='" + companyID + "' and seq_no='" + dr["seq_no"] + "'";
                        ExecuteSql(updateSql, crmConnectionString);
                        continue;
                    }
                    groupIDList.Add(tempGroupID);

                    if (glFlag == 1 || glFlag == 0)//已过账，状态修改为4，下面生成红冲凭证
                    {
                        string updateSql = "update crm.GL_Accvouch_ReceiveIncome set acc_status='4' where Company='" + companyID + "' and seq_no='" + dr["seq_no"] + "'";
                        ExecuteSql(updateSql, crmConnectionString);
                    }
                    else if (glFlag == 2)
                    {
                        curDt = DateTime.Now;
                        logType = "删除凭证";
                        logStr = "凭证：" + dr["JournalNum"] + "正在审核，将跳过";
                        statueFlag = "失败";
                        CreateUD04Recond(logStr, logType, curDt, statueFlag);
                        continue;
                    }
                    else if (glFlag == 3)
                    {
                        DeleteJournal(tempGroupID, deleteJournalNumList);
                        string updateSql = "update crm.GL_Accvouch_ReceiveIncome set acc_status='3' where Company='" + companyID + "' and seq_no='" + dr["seq_no"] + "'";
                        ExecuteSql(updateSql, crmConnectionString);
                        continue;
                    }
                    else//找不到
                    {
                        string updateSql = "update crm.GL_Accvouch_ReceiveIncome set acc_status='3' where Company='" + companyID + "' and seq_no='" + dr["seq_no"] + "'";
                        ExecuteSql(updateSql, crmConnectionString);
                    }

                }

                if (glRowsCount > 0)
                {
                    curDt = DateTime.Now;
                    logType = "描述";
                    logStr = "删除凭证完成，删除" + deleteJournalNumList.Count + "条凭证";
                    statueFlag = "成功";
                    CreateUD04Recond(logStr, logType, curDt, statueFlag);
                }


                //写凭证
                strSql = @"
				    select * from crm.GL_Accvouch_ReceiveIncome with (nolock)
				    where Company='{0}' and acc_status in ('0','4') and iyear={1} and iperiod={2}
    --and receiveIncomeId=294522
				    order by acc_status desc,receiveIncomeId,inid,JEDate
			    ";
                strSql = string.Format(strSql, companyID, year, month);
                glDt = GetDataBySql(strSql, crmConnectionString);
                glRowsCount = glDt.Rows.Count;
                if (glRowsCount > 0)
                {
                    curDt = DateTime.Now;
                    logType = "描述";
                    logStr = "开始写入凭证";
                    statueFlag = "成功";
                    CreateUD04Recond(logStr, logType, curDt, statueFlag);
                }
                int glCount = 0;
                List<int> tempGlJournalList = new List<int>();

                string groupID = "";
                ii = 0;
                foreach (DataRow dr in glDt.Rows)
                {
                    ii++;
                    lb_Process.Text = "写入凭证:" + ii + "/" + glDt.Rows.Count;
                    Application.DoEvents();
                    int tempJournalNum = Convert.ToInt32(dr["receiveIncomeId"]);
                    string _JournalNum = dr["JournalNum"].ToString();
                    if (!glStatusHt.ContainsKey(_JournalNum))
                    {
                        int glFlag = CheckGlPostStatus(dr);
                        glStatusHt.Add(_JournalNum, glFlag);
                    }
                    DateTime jeDate = Convert.ToDateTime(dr["JEDate"]);
                    jeDate = Convert.ToDateTime(jeDate.ToString("yyyy-MM-dd"));    
                    if (!tempGlJournalList.Contains(tempJournalNum))
                    {
                        if (glCount == 0 || (glCount == maxGlCount + 1))
                        {
                            if (groupID != "")
                            {
                                UnLockGroup(groupID);
                            }
                            groupID = CreateGroupID(groupIDPre, jeDate);
                            if (groupID == "") return;
                            glCount = 0;
                        }

                        string desc = dr["Description"].ToString().Trim();
                        string tranDocTypeID = dr["TranDocTypeID"].ToString().Trim();
                        bool isRedStorno = false;

                        if (dr["acc_status"].ToString() == "4")
                        {
                            
                            int glFlag = -1;
                            if(glStatusHt.ContainsKey(_JournalNum)) glFlag = Convert.ToInt32(glStatusHt[_JournalNum]);
                            if (glFlag == 1 || glFlag==0)
                            {                                
                                desc = "冲销" + desc;
                            }
                            if (glFlag == 0)
                            {
                                isRedStorno = true;
                            }
                        }
                        glCount++;
                        tempGlJournalList.Add(tempJournalNum);
                        //金额有负数的就作为红冲
                        //bool isNegative = false;
                        DataRow[] drArr = glDt.Select("receiveIncomeId=" + tempJournalNum);
                        foreach (DataRow _dr in drArr)
                        {
                            if (_dr["DebitAmount"] == DBNull.Value) _dr["DebitAmount"] = 0;
                            if (_dr["CreditAmount"] == DBNull.Value) _dr["CreditAmount"] = 0;
                            if (Convert.ToDecimal(_dr["DebitAmount"]) + Convert.ToDecimal(_dr["CreditAmount"]) < 0)
                            {
                                int glFlag = Convert.ToInt32(glStatusHt[_JournalNum]);
                                if (glFlag == 0)
                                {
                                    _dr["DebitAmount"] = Convert.ToDecimal(_dr["DebitAmount"]) * -1;
                                    _dr["CreditAmount"] = Convert.ToDecimal(_dr["CreditAmount"]) * -1;
                                    isRedStorno = false;
                                }
                                else
                                {
                                    isRedStorno = true;
                                }
                            }
                            else //正数变负数
                            {
                                if (dr["acc_status"].ToString() == "4")
                                {
                                    isRedStorno = true;
                                    _dr["DebitAmount"] = Convert.ToDecimal(_dr["DebitAmount"]) * -1;
                                    _dr["CreditAmount"] = Convert.ToDecimal(_dr["CreditAmount"]) * -1;
                                }
                            }
                        }
                        AddGLJournalHead(desc, jeDate, tranDocTypeID, groupID, isRedStorno);
                    }
                    int[] intArr = AddGLJournalLine(dr, groupID);
                    if (intArr[1] == 0) continue;
                    string strAccStatus = "1";
                    int glFlag1 = Convert.ToInt32(glStatusHt[_JournalNum]);
                    string _updateSql = "";
                    if (glFlag1 == 1 || glFlag1==0)
                    {
                        strAccStatus = "5";
                        _updateSql = "update crm.GL_Accvouch_ReceiveIncome set acc_status='" + strAccStatus + "' where Company='" + companyID + "' and seq_no='" + dr["seq_no"] + "'";
                    }
                    else
                    {
                        _updateSql = "update crm.GL_Accvouch_ReceiveIncome set acc_status='" + strAccStatus + "',GroupID='" + groupID + "',JournalNum=" + intArr[0] + ",JournalLine=" + intArr[1] + " where Company='" + companyID + "' and seq_no='" + dr["seq_no"] + "'";
                    }
                    int excuRow = ExecuteSql(_updateSql, crmConnectionString);
                    if (excuRow == 0)
                    {
                        int testCount = 0;
                        while (true)
                        {
                            testCount++;
                            if (testCount == 5)
                            {
                                errorSqlList.Add(_updateSql);
                                break;
                            }
                            excuRow = ExecuteSql(_updateSql, crmConnectionString);
                            if (excuRow > 0) break;
                        }
                    }
                    //				curDt = DateTime.Now;
                    //				logType = "生成凭证";
                    //				logStr = "群组ID：" + groupID + ",凭证号：" + intArr[0] + "，凭证行："+ intArr[1] +"，DebitAmount："+ dr["DebitAmount"] +"，CreditAmount："+ dr["CreditAmount"];
                    //				statueFlag = "成功";
                    //				CreateUD04Recond(logStr,logType,curDt,statueFlag);

                }
                if (groupID != "")
                {
                    UnLockGroup(groupID);
                }
                foreach (string _updateSql in errorSqlList)
                { 
                    int excuRow = ExecuteSql(_updateSql, crmConnectionString);
                    if (excuRow == 0)
                    {
                        curDt = DateTime.Now;
                        logType = "需要手工执行SQL";
                        logStr = _updateSql;
                        statueFlag = "失败";
                        CreateUD04Recond(logStr, logType, curDt, statueFlag);
                    }
                }
                if (glRowsCount > 0)
                {
                    curDt = DateTime.Now;
                    logType = "描述";
                    logStr = "写入凭证完成，共写入" + tempGlJournalList.Count + "条凭证";
                    statueFlag = "成功";
                    CreateUD04Recond(logStr, logType, curDt, statueFlag);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //返回值1：已过账，２：正在审核，３：还在凭证entry里，４：都找不到,0:已过账且是负数金额
        private int CheckGlPostStatus(DataRow dr)
        {
            DateTime jeDate = Convert.ToDateTime(dr["JEDate"]);
            string strWhere = "where Company='" + companyID + "' and FiscalYear=" + jeDate.Year + " and FiscalPeriod=" + jeDate.Month + " and JournalNum='" + dr["JournalNum"] + "'";
            string strSql = "SELECT Company,bookcreditamount+bookdebitamount as 'AoumtAdd' FROM erp.GlJrnDtl " + strWhere;
            DataTable dt = GetDataBySql(strSql);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToDecimal(dt.Rows[0]["AoumtAdd"]) > 0)
                    return 1;
                else
                    return 0;
            }

            strSql = "select JournalNum from erp.RvJrnTrDtl " + strWhere;
            dt = GetDataBySql(strSql);
            if (dt.Rows.Count > 0) return 2;

            strSql = "select JournalNum from erp.GLJrnDtlMnl " + strWhere;
            dt = GetDataBySql(strSql);
            if (dt.Rows.Count > 0) return 3;
            else return 4;
        }

        private bool UpdateCrmSegFlag(string coACode, int segmentNbr, string segmentCode)
        {
            try
            {
                string updateSql = @"UPDATE [crm].[Gl_COASegvalues] SET [acc_status]=1 where Company='{0}' and [COACode]='{1}' and [SegmentNbr]={2} and [SegmentCode]='{3}'";
                updateSql = string.Format(updateSql, companyID, coACode, segmentNbr, segmentCode);
                int excuRow = ExecuteSql(updateSql, crmConnectionString);
                if (excuRow > 0) return true;
                else return false;
            }
            catch (Exception ex)
            {
                DateTime curDt = DateTime.Now;
                string logType = "科目段落值";
                string logStr = "coACode:" + coACode + ";segmentNbr:" + segmentNbr + ";segmentCode:" + segmentCode + "，更新CMR标识失败：" + ex.Message;
                string statueFlag = "失败";
                CreateUD04Recond(logStr, logType, curDt, statueFlag);
                return false;
            }
        }

        private bool GetNewCOASegValuesMethod(string coACode, int segmentNbr, string segmentCode, string segmentDesc, string segmentAbbrev, DateTime? effectiveFrom)
        {
            try
            {
                COASegValuesImpl adapterCOASegValues = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<COASegValuesImpl>(epicorSession, ImplBase<Erp.Contracts.COASegValuesSvcContract>.UriPath);
                COASegValuesDataSet ds = new COASegValuesDataSet();
                

                adapterCOASegValues.GetNewCOASegValues(ds,coACode, segmentNbr);
                adapterCOASegValues.ChangeSegvalue(segmentCode,ds);
                ds.Tables["COASegValues"].Rows[0]["COACode"] = coACode;
                ds.Tables["COASegValues"].Rows[0]["SegmentNbr"] = segmentNbr;
                ds.Tables["COASegValues"].Rows[0]["SegmentCode"] = segmentCode;
                ds.Tables["COASegValues"].Rows[0]["SegmentName"] = segmentDesc;
                ds.Tables["COASegValues"].Rows[0]["SegmentDesc"] = segmentDesc;
                ds.Tables["COASegValues"].Rows[0]["SegmentAbbrev"] = segmentAbbrev;
                if (effectiveFrom != null) ds.Tables["COASegValues"].Rows[0]["EffectiveFrom"] = effectiveFrom;
                adapterCOASegValues.Update(ds);
                // Cleanup Adapter Reference
                adapterCOASegValues.Dispose();
                return true;

            }
            catch (System.Exception ex)
            {
                //throw new Exception("写入科目段落值失败,coACode:" + coACode + ";segmentNbr:" + segmentNbr + ";segmentCode:" + segmentCode + ".原因:" + ex.Message);
                DateTime curDt = DateTime.Now;
                string logType = "科目段落值";
                string logStr = "coACode:" + coACode + ";segmentNbr:" + segmentNbr + ";segmentCode:" + segmentCode + "，写入失败:" + ex.Message;
                string statueFlag = "失败";
                CreateUD04Recond(logStr, logType, curDt, statueFlag);
                return false;
            }
        }

        private string CreateGroupID(string preGroupID, DateTime jeDate)
        {
            try
            {
                string groupID = preGroupID + DateTime.Now.ToString("yyMM");
                int nextSeq = GenerateNo(groupID);
                groupID = groupID + nextSeq.ToString().PadLeft(3, '0');

                GLJrnGrpImpl adapterGLJrnGrp = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<GLJrnGrpImpl>(epicorSession, ImplBase<Erp.Contracts.GLJrnGrpSvcContract>.UriPath);
                GLJrnGrpDataSet ds = new GLJrnGrpDataSet();

                adapterGLJrnGrp.GetNewGLJrnGrp(ds);
                ds.Tables["GLJrnGrp"].Rows[0]["GroupID"] = groupID;
                
                string opMessage = "";
                adapterGLJrnGrp.CheckFiscalYear(ds,groupID, jeDate, out opMessage);
                ds.Tables["GLJrnGrp"].Rows[0]["JEDate"] = jeDate;
                ds.Tables["GLJrnGrp"].Rows[0]["FiscalYear"] = jeDate.Year;
                ds.Tables["GLJrnGrp"].Rows[0]["FiscalPeriod"] = jeDate.Month;
                ds.Tables["GLJrnGrp"].Rows[0]["JournalCode"] = "HRXS";
                adapterGLJrnGrp.Update(ds);
                adapterGLJrnGrp.Dispose();
                return groupID;
            }
            catch (Exception ex)
            {
                DateTime curDt = DateTime.Now;
                string logType = "生成凭证群组";
                string logStr = "群组创建失败：" + ex.Message;
                string statueFlag = "失败";
                CreateUD04Recond(logStr, logType, curDt, statueFlag);
                return "";
            }
        }

        GLJournalEntryImpl adapterGLJournalEntry;
        GLJournalEntryDataSet ds;
        private bool AddGLJournalHead(string desc, DateTime jeDate, string ipTranDocTypeID, string groupID, bool isRedStorno)
        {
            try
            {
                adapterGLJournalEntry = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<GLJournalEntryImpl>(epicorSession, ImplBase<Erp.Contracts.GLJournalEntrySvcContract>.UriPath);
                ds = new GLJournalEntryDataSet();
                adapterGLJournalEntry.GetNewGlJrnHedTran(ds,groupID);
                adapterGLJournalEntry.OnChangeTranDocTypeID(ipTranDocTypeID,ds);
                //ds.Tables["GLJrnHed"].Rows[0]["JEDate"] = ipTranDocTypeID;
               
                if (isRedStorno)
                {
                    ds.Tables["GLJrnHed"].Rows[0]["RedStorno"] = true;
                    //ds.Tables["GLJrnHed"].Rows[0]["Reverse"] = true;
                    //ds.Tables["GLJrnHed"].Rows[0]["ReverseDate"] = jeDate;

                }
                ds.Tables["GLJrnHed"].Rows[0]["Description"] = desc;
                ds.Tables["GLJrnHed"].Rows[0]["JEDate"] = jeDate;
                bool requiresUserInput = false;
                adapterGLJournalEntry.PreUpdate(ds,out requiresUserInput);
                adapterGLJournalEntry.Update(ds);
                int index = ds.Tables["GLJrnHed"].Rows.Count - 1;
                bookID = ds.Tables["GLJrnHed"].Rows[index]["BookID"].ToString();
                fiscalYear = Convert.ToInt32(ds.Tables["GLJrnHed"].Rows[index]["FiscalYear"]);
                fiscalPeriod = Convert.ToInt32(ds.Tables["GLJrnHed"].Rows[index]["FiscalPeriod"]);
                fiscalYearSuffix = ds.Tables["GLJrnHed"].Rows[index]["fiscalYearSuffix"].ToString();
                journalCode = ds.Tables["GLJrnHed"].Rows[index]["journalCode"].ToString();
                journalNum = Convert.ToInt32(ds.Tables["GLJrnHed"].Rows[index]["JournalNum"]);

                return true;
            }
            catch (Exception ex)
            {
                DateTime curDt = DateTime.Now;
                string logType = "生成凭证";
                string logStr = "凭证描述：" + desc + "，记账日期：" + jeDate + ",文件类型：" + ipTranDocTypeID + ",群组ID：" + groupID + ",是否红冲：" + isRedStorno + ".错误原因：" + ex.Message;
                string statueFlag = "失败";
                CreateUD04Recond(logStr, logType, curDt, statueFlag);
                return false;
            }
        }

        string bookID = "";
        int fiscalYear = 0;
        int fiscalPeriod = 0;
        string fiscalYearSuffix = "";
        string journalCode = "";
        int journalNum = 0;


        private int[] AddGLJournalLine(DataRow dr, string groupID)
        {
            int journalLine = 0;
            string segmentValueCode1 = "";
            try
            {
                //GLJournalEntryImpl adapterGLJournalEntry = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<GLJournalEntryImpl>(epicorSession, ImplBase<Erp.Contracts.GLJournalEntrySvcContract>.UriPath);
                //GLJournalEntryDataSet ds = new GLJournalEntryDataSet();
                //ds = adapterGLJournalEntry.GetByGroupID(groupID);
                bool isCreateRedStorno = Convert.ToBoolean(ds.Tables["GLJrnHed"].Rows[ds.Tables["GLJrnHed"].Rows.Count-1]["RedStorno"]);
                adapterGLJournalEntry.GetNewGLJrnDtlMnl(ds,bookID, fiscalYear, fiscalYearSuffix, journalCode, journalNum, journalLine);
                int index = ds.Tables["GLJrnDtlMnl"].Rows.Count - 1;

                for (int i = 1; i <= 20; i++)
                {
                    if (dr["Segment" + i] == DBNull.Value) dr["Segment" + i] = "";
                    segmentValueCode1 += dr["Segment" + i].ToString().Trim() + "|";

                    ds.Tables["GLJrnDtlMnl"].Rows[index]["SegValue" + i] = dr["Segment" + i];
                }
                segmentValueCode1 = segmentValueCode1.TrimEnd('|');

                ds.Tables["GLJrnDtlMnl"].Rows[index]["GLAccount"] = segmentValueCode1;

                decimal debitAmt = 0;
                decimal creditAmt = 0;
                if (dr["DebitAmount"] != DBNull.Value) debitAmt = Convert.ToDecimal(dr["DebitAmount"]);
                if (dr["CreditAmount"] != DBNull.Value) creditAmt = Convert.ToDecimal(dr["CreditAmount"]);
                if (isCreateRedStorno && (debitAmt * creditAmt>0))
                {
                    debitAmt = debitAmt * -1;
                    creditAmt = creditAmt * -1;
                }
                if (debitAmt!=0) ds.Tables["GLJrnDtlMnl"].Rows[index]["TotDebit"] = debitAmt;
                if (creditAmt!=0) ds.Tables["GLJrnDtlMnl"].Rows[index]["TotCredit"] = creditAmt;
                adapterGLJournalEntry.Update(ds);

                journalLine = Convert.ToInt32(ds.Tables["GLJrnDtlMnl"].Rows[index]["JournalLine"]);

                int[] intArr = new int[2] { journalNum, journalLine };
                return intArr;
            }
            catch (Exception ex)
            {
                DateTime curDt = DateTime.Now;
                string logType = "生成凭证";
                string logStr = "群组ID：" + groupID + ",凭证号：" + journalNum + "，科目：" + segmentValueCode1 + "，DebitAmount：" + dr["DebitAmount"] + "，CreditAmount：" + dr["CreditAmount"] + ".错误原因：" + ex.Message.Replace("'", "");
                string statueFlag = "失败";
                CreateUD04Recond(logStr, logType, curDt, statueFlag);
                int[] intArr = new int[2] { 0, 0 };
                return intArr;
            }

        }

        private bool UnLockGroup(string groupID)
        {
            GLJrnGrpImpl adapterGLJrnGrp = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<GLJrnGrpImpl>(epicorSession, ImplBase<Erp.Contracts.GLJrnGrpSvcContract>.UriPath);
            GLJrnGrpDataSet ds = new GLJrnGrpDataSet();
            adapterGLJrnGrp.GetByID(groupID);
            adapterGLJrnGrp.UnlockGroup(groupID);
            adapterGLJrnGrp.Dispose();
            return true;
        }

        private int GenerateNo(string classStr)
        {
            string sql = "select top 1 GroupID from ERP.GLJrnGrp where Company='" + companyID + "' and GroupID like '" + classStr + "%' order By GroupID DESC";
            DataTable dt = GetDataBySql(sql);
            //MessageBox.Show(sql);
            if (dt.Rows.Count == 0)
            {
                return 1;
            }
            string lastCode = dt.Rows[0]["GroupID"].ToString();
            if (lastCode.Length < 3)
            {
                MessageBox.Show("该分类的编号有问题,请把最新的编号删除并重新新建");
                return 0;
            }
            int codeLast2Str = Convert.ToInt32(lastCode.Substring(lastCode.Length - 3, 3));
            int newNo = codeLast2Str + 1;
            return newNo;
        }

        private bool DeleteJournal(string groupID, List<string> deleteJournalNumList)
        {
            try
            {
                string sql = "select GroupID from ERP.GLJrnGrp where Company='" + companyID + "' and GroupID='" + groupID + "'";
                DataTable dt = GetDataBySql(sql);
                if (dt.Rows.Count == 0) return true;

                sql = "select GroupID from ERP.GLJrnHed where Company='" + companyID + "' and GroupID='" + groupID + "'";
                dt = GetDataBySql(sql);
                if (dt.Rows.Count == 0)
                {
                    DeleteGroup(groupID);
                    return true;
                }

                GLJournalEntryImpl adapterGLJournalEntry = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<GLJournalEntryImpl>(epicorSession, ImplBase<Erp.Contracts.GLJournalEntrySvcContract>.UriPath);
                GLJournalEntryDataSet ds = new GLJournalEntryDataSet();

                ds = adapterGLJournalEntry.GetByGroupID(groupID);
                for (int i = 0; i < ds.Tables["GLJrnHed"].Rows.Count; i++)
                {
                    string journalNum = ds.Tables["GLJrnHed"].Rows[i]["JournalNum"].ToString();
                    if (deleteJournalNumList.Contains(journalNum))
                    {
                        adapterGLJournalEntry.VoidLegalNumber("同步程序删除凭证",ds);
                        ds.Tables["GLJrnHed"].Rows[i]["LegalNumber"] = "";
                        ds.Tables["GLJrnHed"].Rows[i]["RowMod"] = "U";
                        adapterGLJournalEntry.Update(ds);
                    }
                }

                for (int i = 0; i < ds.Tables["GLJrnHed"].Rows.Count; i++)
                {
                    string journalNum = ds.Tables["GLJrnHed"].Rows[i]["JournalNum"].ToString();
                    if (deleteJournalNumList.Contains(journalNum))
                    {
                        ds.Tables["GLJrnHed"].Rows[i].Delete();                        
                    }
                }
                adapterGLJournalEntry.Update(ds);
                //result = adapterGLJournalEntry.GetByGroupID(groupID);
                if (ds.Tables["GLJrnHed"].Rows.Count == 0)
                {
                    DeleteGroup(groupID);
                }

                // Cleanup Adapter Reference
                adapterGLJournalEntry.Dispose();
                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool DeleteGroup(string groupID)
        {
             GLJrnGrpImpl adapterGLJrnGrp = Ice.Lib.Framework.WCFServiceSupport.CreateImpl<GLJrnGrpImpl>(epicorSession, ImplBase<Erp.Contracts.GLJrnGrpSvcContract>.UriPath);
             GLJrnGrpDataSet ds = new GLJrnGrpDataSet();

            ds = adapterGLJrnGrp.GetByID(groupID);
            ds.Tables["GLJrnGrp"].Rows[0].Delete();
            adapterGLJrnGrp.Update(ds);

            adapterGLJrnGrp.Dispose();
            return true;
        }

        private bool CreateUD04Recond(string logStr, string logType, DateTime logTime, string logStatus)
        {
            string strGUID = Guid.NewGuid().ToString();
            string strLogTime = logTime.ToString("yyyy-MM-dd HH:mm:ss");
            string strSql = "INSERT INTO ICE.UD04(Company,Key1,Character01,ShortChar01,ShortChar02,ShortChar03,Date01,Number01,Number02)";
            strSql += " Values ('" + companyID + "','" + strGUID + "','" + logStr + "','" + strLogTime + "','" + logType + "','" + logStatus + "','" + logTime + "','" + logTime.Year + "','" + logTime.Month + "')";
            ExecuteSql(strSql);
            logDt.Rows.Add(companyID, logStr, strLogTime, logType, logStatus);
            return true;
        }

        private void btn_ImportAll_Click(object sender, EventArgs e)
        {
            if (companyID != "JT0000")
            {
                MessageBox.Show("此功能只针对JT0000-集团标准账套使用");
                return;
            }
            int year = Convert.ToInt32(txt_FiscalYear.Text);
            int month = Convert.ToInt32(txt_FiscalPeriod.Text);

            string tempCompanyID = companyID;
            string strSql = "select distinct company from crm.GL_Accvouch_ReceiveIncome with (nolock) where iyear=" + year + " and iperiod=" + month + " and acc_status in (0,2,4) union  select 'JT0000'";
            DataTable dt = GetDataBySql(strSql,crmConnectionString);
            foreach (DataRow dr in dt.Rows)
            {
                companyID = dr["Company"].ToString();
                lb_Msg.Text = "当前公司：" + companyID + ",APP Server:" + appServerUrl;
                Application.DoEvents();
                BeginToOperate();
            }
            epiUltraGridC1.DataSource = logDt;
            MessageBox.Show("凭证导入完成，详细请看日志");
            companyID = "JT0000";
        }

        
    }
}
