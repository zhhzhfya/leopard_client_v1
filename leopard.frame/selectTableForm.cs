using framework.utils;
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
    public partial class selectTableForm : Form
    {
        DataTable dt = null;
        public selectTableForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void selectTableForm_Load(object sender, EventArgs e)
        {
            // 查询表数据
            string sql = "select table_name, table_comment from information_schema.tables  where table_schema='check' order by table_name asc";
            WebServiceFactory.sqlToGridWithHeader(this.dataGridView1, sql);
            dt = DataGridView2DataTable(this.dataGridView1, "", 333);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var q1 = from dt1 in dt.AsEnumerable()//查询
                     where dt1.Field<string>("table_name").Contains(this.textBox1.Text)//条件
                     select dt1;
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = q1.AsDataView();
        }

        public DataTable DataGridView2DataTable(DataGridView dgv, String tblName, int minRow = 0)
        {

            DataTable dt = new DataTable(tblName);

            // Header columns
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                DataColumn dc = new DataColumn(column.HeaderText.ToString());
                dt.Columns.Add(dc);
            }

            // Data cells
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
                }
                dt.Rows.Add(dr);
            }

            // Related to the bug arround min size when using ExcelLibrary for export
            for (int i = dgv.Rows.Count; i < minRow; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dr[j] = "  ";
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }


}
