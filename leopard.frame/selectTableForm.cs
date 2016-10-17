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
        DataTable dt = new DataTable();
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
            dt = GetDataTableFromDGV(this.dataGridView1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var q1 = from dt1 in dt.AsEnumerable()//查询
                     where dt1.Field<string>("table_name").Contains(this.textBox1.Text)//条件
                     select dt1;
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = q1.AsDataView();
        }

        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Visible)
                {
                    dt.Columns.Add(new DataColumn(column.HeaderText));
                }
            }

            object[] cellValues = new object[dgv.Columns.Count];
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cellValues[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cellValues);
            }

            return dt;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 1)
            {
                
            }
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("drag over");
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            Console.WriteLine(e);
        }
    }


}
