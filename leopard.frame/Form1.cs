using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.OracleClient;
using framework.utils;
using Microsoft.Win32;
using System.Data.SQLite;

namespace framework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!LocalDBBusiness.GetInstance().CheckTableExist("CO_URL_LOGS"))
            {
                SQLiteCommand command = new SQLiteCommand("CREATE TABLE CO_URL_LOGS(URL VARCHAR(30), SUPER_PASS VARCHAR(20), FLAG INT)");
                LocalDBSQLiteHelper.GetInstance().ExecuteNonQuery(command);
            }
            else
            {
                // 如果存在则把历史数据加载到combox里
                SQLiteCommand command = new SQLiteCommand("SELECT * FROM CO_URL_LOGS");
                DataSet ds = LocalDBSQLiteHelper.GetInstance().ExecuteDataSet(command);
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["FLAG"].ToString().Equals("1"))
                    {
                        this.cb_url1.Items.Add(dr["URL"].ToString());
                    }
                    else if (dr["FLAG"].ToString().Equals("2"))
                    {
                        this.cb_url2.Items.Add(dr["URL"].ToString());
                    }
                }
            }

            grid1.BorderStyle = BorderStyle.FixedSingle;
            grid1.ColumnsCount = 30;
            grid1.FixedRows = 1;
            grid1.Rows.Insert(0);
            for (int i = 0; i < 30; i++ )
            {
                grid1[0, i] = new SourceGrid.Cells.ColumnHeader("String");
            }
            for (int r = 1; r < 100; r++)
            {
                grid1.Rows.Insert(r);
                for (int i = 0; i < 30; i++)
                {
                    grid1[r, i] = new SourceGrid.Cells.Cell("+" + r.ToString(), typeof(string));
                }
            }
            grid1.AutoSizeCells();
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String svnLogs = this.textBox1.Text;
            String engStr = "Revision";
            if (!svnLogs.Trim().Equals(""))
            {
                if (svnLogs.IndexOf("版本:") >= 0)
                {
                    engStr = "版本";
                }
                Hashtable fv = new Hashtable();
                string[] rs = Regex.Split(svnLogs, "\r\n\r\n" + engStr, RegexOptions.IgnoreCase);
                string logs;
                if (!svnLogs.StartsWith(engStr + ":"))
                {
                    this.textBox2.Text = "";
                    return;
                }
                foreach (string i in rs)
                {
                    logs = i;
                    if (!logs.StartsWith(engStr))
                    {
                        logs = engStr + i;
                    }
                    if (logs.IndexOf("\r\n") < 0 || logs.Length < 9)
                    {
                        continue;
                    }
                    string ver = logs.Substring(logs.IndexOf(":") + 2, logs.IndexOf("\r\n") - logs.IndexOf(":") - 2);
                    string[] filesBlock = Regex.Split(logs, "\r\n----\r\n", RegexOptions.IgnoreCase);
                    if (filesBlock.Length != 2)
                    {
                        continue;
                    }
                    string[] files = Regex.Split(filesBlock[1], "\r\n", RegexOptions.IgnoreCase);
                    
                    foreach (string fs in files)
                    {
                        if (fs.Equals(""))
                        {
                            continue;
                        }
                        string[] fileNames = fs.Split(':');
                        if (fileNames.Length == 2)
                        {
                            string fileName = fileNames[1];
                            if (fileName.Trim().Equals(""))
                            {
                                continue;
                            }
                            if (fv.ContainsKey(fileName.Trim()))
                            {
                                int v = Convert.ToInt32(fv[fileName.Trim()]);
                                if (Convert.ToInt32(ver) > v)
                                {
                                    fv[fileName.Trim()] = ver;
                                }
                            }
                            else
                            {
                                fv.Add(fileName.Trim(), ver);
                            }

                        } 
                    }
                }
                StringBuilder text = new StringBuilder();
                ArrayList list = new ArrayList(fv.Keys); list.Sort();
                foreach (string key in list)
                {
                    text.Append(key);
                    if (this.checkBox3.Checked)
                    {
                        text.Append(fv[key]);
                    }
                    text.Append("\r\n");
                }
                this.textBox2.Text = text.ToString();
            }
            else
            {
                this.textBox2.Text = "";
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void idsStr_TextChanged(object sender, EventArgs e)
        {
            makeIdsString();

        }

        private void makeIdsString()
        {
            string sp = "";
            if (this.radioButton1.Checked)
            {
                sp = this.radioButton1.Text;
            }
            else
            {
                sp = this.radioButton2.Text;
            }
            string ids = idsStr.Text;
            if (this.checkBox2.Checked)
            {
                ids = ids.Replace(" ", "");
            }
            if (!ids.Trim().Equals(""))
            {
                string[] idsArray = Regex.Split(ids, "\r\n", RegexOptions.IgnoreCase);
                StringBuilder sb = new StringBuilder();
                Hashtable tb = new Hashtable();
                for (int i = 0; i < idsArray.Length; i++)
                {
                    if (!idsArray[i].Trim().Equals("")
                        && (!this.checkBox1.Checked || (this.checkBox1.Checked && !tb.Contains(idsArray[i]))))
                    {
                        sb.Append(idsArray[i]).Append(sp);
                    }
                    if (!tb.Contains(idsArray[i]))
                    {
                        tb.Add(idsArray[i], idsArray[i]);
                    }
                }
                sb.Length = sb.Length - sp.Length;
                this.idsResult.Text = sb.ToString();
            }
            else
            {
                this.idsResult.Text = "";
            }
        }

        private void idsStr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                this.idsStr.SelectAll();
            }
        }

        private void idsResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                this.idsResult.SelectAll();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                this.textBox1.SelectAll();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.A))
            {
                this.textBox2.SelectAll();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            makeIdsString();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            makeIdsString();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            makeIdsString();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            makeIdsString();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void toolStripButton7_Click_1(object sender, EventArgs e)
        {
            string strconn = "User ID=hdeam_product;Password=d3B68Apk29v34Dj;Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST=192.168.6.10)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)))";

            OracleConnection conn = new OracleConnection(strconn);//创建一个新连接
            try
            {
                conn.Open();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select PROC_CODE, PROC_NAME, Proc_type, PROC_REMARK from SY_WF_PROC_DEF where 1=1 and proc_type=3";//在这儿写sql语句
                OracleDataReader odr = cmd.ExecuteReader();//创建一个OracleDateReader对象
                while (odr.Read())//读取数据，如果odr.Read()返回为false的话，就说明到记录集的尾部了
                {

                    dataGridView1.Rows.Add(new Object[] { odr.GetOracleString(0).ToString(), odr.GetOracleString(1).ToString(), odr.GetOracleNumber(2),odr.GetOracleString(3).ToString() ,"迁移到右边"});
                }
                odr.Close();
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                conn.Close(); //关闭连接
            }
        }

        private void cb_url1_Leave(object sender, EventArgs e)
        {
            if (!this.cb_url1.Text.Equals(""))
            {
                MessageBox.Show(cb_url1.Text);
            }
            //SQLiteCommand command = new SQLiteCommand("CREATE TABLE CO_URL_LOGS(NAME VARCHAR(200), SUPER_PASS VARCHAR(50), FLAG INT)");
            //LocalDBSQLiteHelper.GetInstance().ExecuteNonQuery(command);

            //command = new SQLiteCommand("select v.name from e_vendor v where v.id =@id");
            //command.Parameters.Add(new SQLiteParameter("@id", ""));
            //DataSet dataSet = LocalDBSQLiteHelper.GetInstance().ExecuteDataSet(command);
        }

        private void cb_url1_Click(object sender, EventArgs e)
        {
            this.cb_url1.DroppedDown = true;
        }

        private void cb_url2_Click(object sender, EventArgs e)
        {
            this.cb_url2.DroppedDown = true;
        }

        private void fffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm f = new LoginForm();
            f.Show();
        }

        private void ReloadMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm f = new LoginForm();
            f.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = this.txt_sql.Text;
            WebServiceFactory.sqlToGridWithHeader(this.dataGridView4, sql);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.dataGridView4.Rows.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string registData;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.OpenSubKey("CBSTEST\\CBSTestWDSLogFile", true);
            registData = aimdir.GetValue("").ToString();
            MessageBox.Show(registData);
        }
    }
}