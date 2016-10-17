using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace frame
{
    public partial class MysqlDBForm : Form
    {
        MySqlConnection myCon;
        DataTable dt;
        MySqlDataAdapter da;
        MySqlCommandBuilder cb;
        public MysqlDBForm()
        {
            InitializeComponent();
        }

        private void MysqlDBForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dt = new DataTable();
            string mySqlConnStr = "server=192.168.4.13;user id=root;password=root;database=jeesite;CharSet=utf8;pooling=false;"; //根据自己的设置
            myCon = new MySqlConnection(mySqlConnStr);
            myCon.Open();
            da = new MySqlDataAdapter("select * from orders", myCon);
            cb = new MySqlCommandBuilder(da);
            da.Fill(dt);
            this.dataGridView1.DataSource = dt;
            myCon.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable change = dt.GetChanges();
            if (change != null)
            {
                da.Update(change);
                dt.AcceptChanges();
            }
        }

        private void MysqlDBForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (myCon != null)
            {
                myCon.Dispose();
            }
        }
    }
}
